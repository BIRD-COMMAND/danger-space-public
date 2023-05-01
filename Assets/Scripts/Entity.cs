using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{

	[Flags]
	public enum Faction
	{
		None = 0, Structure = 1, Player = 2, Neutral = 4, 
		Alliance = 8, Rebellion = 16, Empire = 32, Raider = 64, Pirate = 128
	}
	public bool IsFriendly(Entity other) { return (int)(faction & other.faction) > 3; }
	public bool IsPlayer => faction.HasFlag(Faction.Player);
	public bool IsStructure => faction.HasFlag(Faction.Structure);
	public bool IsNeutral => faction.HasFlag(Faction.Neutral);

	[Header("General")]
	public Faction faction = Faction.Neutral;
	public Rigidbody2D Body => body;
	[SerializeField, HideInInspector] protected Rigidbody2D body;
	public CircleCollider2D Bounds => bounds;
	[SerializeField] protected CircleCollider2D bounds;
	[SerializeField] protected Drop[] drops;
	[HideInInspector] public List<ShapeRenderer> shapes;
	public Entity target;
	public GameObject destroyEffectPrefab;
	public GameObject wreckagePrefab;
	public float MaxHealth => maxHealth;
	[SerializeField] protected float maxHealth = 10f;
	public float Health => health;
	[SerializeField] protected float health = 10f;
	public float PointValue => pointValue;
	[SerializeField] protected float pointValue = 100f;
	public float DamageOnCollision => damageOnCollision;
	[SerializeField] protected float damageOnCollision = 2f;
	public bool invulnerable = false;
	public float HealthPercent => health / maxHealth;

	[Header("Movement")]
	public float maxVelocity = 3.5f;
	public float maxAcceleration = 10f;
	public float turnSpeed = 20f;
	public bool inBulletTime = false;

	[Header("Look Direction Smoothing")]
	/// <summary>
	/// Smoothing controls if the character's look direction should be an
	/// average of its previous directions (to smooth out momentary changes
	/// in directions)
	/// </summary>
	public bool smoothing = true;
	public int numSamplesForSmoothing = 5;
	Queue<Vector2> rotationSamples = new Queue<Vector2>();

	protected virtual void Awake()
	{
		if (!bounds) {
			Debug.LogError($"Destroying Invalid Entity '{gameObject.name}': no Bounds assigned. The Bounds should be a trigger CircleCollider2D enclosing the Entity.", gameObject);
			Destroy(this); 
			return;
		}
		body = GetComponent<Rigidbody2D>();
		shapes = transform.FindComponents<ShapeRenderer>();
	}
	private void Reset()
	{
		if (!bounds && TryGetComponent(out RegularPolygon shape)) {
			bounds = gameObject.AddComponent<CircleCollider2D>();
			bounds.radius = shape.Radius; bounds.isTrigger = true;
		}
	}

	#region Entity Body Properties

	/// <summary>
	/// The radius for the current game object. If the game object does not have a circle collider this will return -1.
	/// </summary>
	public float Radius => bounds.radius;

	/// <summary>
	/// The position that should be used for most movement AI code.
	/// </summary>
	public Vector2 Position => body.position;

	/// <summary>
	/// The rotation for this rigidbody.
	/// </summary>
	public float Rotation { get => body.rotation - 90f; set => body.MoveRotation(value); }

	/// <summary>
	/// The velocity that should be used for movement AI code.
	/// </summary>
	public Vector2 Velocity { get => body.velocity; set => body.velocity = value; }

	/// <summary>
	/// The angularVelocity for the rigidbody.
	/// </summary>
	public float AngularVelocity { get => body.angularVelocity; set => body.angularVelocity = value; }

	/// <summary>
	/// The rotation for this rigidbody in radians.
	/// </summary>
	public float RotationInRadians => (body.rotation - 90f) * Mathf.Deg2Rad;
	
	/// <summary>
	/// The rotation for this rigidbody as a Vector.
	/// </summary>
	public Vector2 RotationAsVector => OrientationToVector(RotationInRadians);

	/// <summary>
	/// Gets the position of the collider (which can be offset from the transform position).
	/// </summary>
	public Vector2 ColliderPosition => transform.TransformPoint(bounds.offset);

	/// <summary>
	/// Returns true if the Entity's Position is within the ScreenTrigger bounds.
	/// </summary>
	public bool IsOnScreen => ScreenTrigger.IsOnScreen(Position);

	#endregion

	#region Angle and Orientation

	/// <summary>
	/// Returns true if the target is within view and there is an unobstructed path of width (Radius * 0.5) to the target. (Ignores projectiles)
	/// </summary>
	public bool TargetInSight(Entity target)
	{
		bool defaultQueriesStartInColliders = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.CircleCast(
			ColliderPosition, (Radius * 0.5f), target.Position - Position,
			1000f, GameManager.RaycastLayersExcludingProjectiles
		);
		Physics2D.queriesStartInColliders = defaultQueriesStartInColliders;
		return hit && hit.collider && hit.collider.FindComponent(out Entity entity) && entity == target;
	}

	/// <summary>
	/// Makes the current game object look where it is going
	/// </summary>
	public void FaceHeading()
	{
		Vector2 direction = Velocity;

		if (smoothing) {
			if (rotationSamples.Count == numSamplesForSmoothing) {
				rotationSamples.Dequeue();
			}

			rotationSamples.Enqueue(Velocity);

			direction = Vector2.zero;

			foreach (Vector2 v in rotationSamples) {
				direction += v;
			}

			direction /= rotationSamples.Count;
		}

		LookAtDirection(direction);
	}
	/// <summary>
	/// Makes the current game object look towards its target
	/// </summary>
	public void FaceTarget()
	{

		if (!target) { FaceHeading(); return; }

		Vector2 direction = target.Position - Position;

		if (smoothing) {

			if (rotationSamples.Count == numSamplesForSmoothing) {
				rotationSamples.Dequeue();
			}

			rotationSamples.Enqueue(direction);

			direction = Vector2.zero;
			foreach (Vector2 v in rotationSamples) { direction += v; }
			direction /= rotationSamples.Count;
		}

		LookAtDirection(direction);
	}
	public void LookAtDirection(Vector2 direction)
	{
		direction.Normalize();

		/* If we have a non-zero direction then look towards that direciton otherwise do nothing */
		if (direction.sqrMagnitude > 0.001f) {
			float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
			Rotation = Mathf.LerpAngle(Rotation + 90f, toRotation - 90f, Time.deltaTime * turnSpeed);
		}
	}
	public void LookAtDirection(Quaternion toRotation) { LookAtDirection(toRotation.eulerAngles.z); }
	/// <summary>
	/// Makes the character's rotation lerp closer to the given target rotation (in degrees).
	/// </summary>
	/// <param name="toRotation">the desired rotation to be looking at in degrees</param>
	public void LookAtDirection(float toRotation)
	{
		Rotation = Mathf.LerpAngle(Rotation + 90f, toRotation - 90f, Time.deltaTime * turnSpeed);
	}

	/// <summary>
	/// Checks to see if the target is in front of the character
	/// </summary>
	public bool IsInFront(Vector2 target)
	{
		return IsFacing(target, 0);
	}
	public bool IsFacing(Vector2 target, float cosineValue)
	{
		Vector2 facing = transform.up.normalized;
		Vector2 directionToTarget = (target - Position).normalized;
		return Vector2.Dot(facing, directionToTarget) >= cosineValue;
	}

	/// <summary>
	/// Returns the given orientation (in radians) as a unit vector
	/// </summary>
	/// <param name="orientation">the orientation in radians</param>
	public static Vector2 OrientationToVector(float orientation)
	{
		return new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
	}

	/// <summary>
	/// Gets the orientation of a vector as radians around the Z axis.
	/// </summary>
	/// <param name="direction">the direction vector</param>
	/// <returns>orientation in radians</returns>
	public static float VectorToOrientation(Vector2 direction) { return Mathf.Atan2(direction.y, direction.x); }

	protected IEnumerator DebugDraw()
	{
		while (Application.isPlaying) {
			yield return new WaitForFixedUpdate();

			Vector2 origin = ColliderPosition;
			Debug.DrawLine(origin, origin + (Velocity.normalized), Color.red, 0f, false);

			//SteeringBasics.debugCross(colliderPosition, 0.5f, Color.red, 0, false);
			//Debug.Log(rb3D.velocity.magnitude);
			//Debug.Log(rb3D.velocity.y + " " + movementNormal.ToString("f4") + " " + wallNormal.ToString("f4") + " " + count);
			//Debug.Log("--------------------------------------------------------------------------------");
		}
	}

	#endregion

	#region Damage and Destruction

	public virtual void Heal(float healing, Entity source)
	{
		OnIncomingHealing(healing, source);
		if (health > 0) {
			OnWillTakeHealing(healing, source);
			health += healing;
			OnTookHealing(healing, source);
		}
		if (health > maxHealth) { health = maxHealth; }
	}
	public virtual void RemoteHealing(float damage, Entity source) { }
	public virtual void OnIncomingHealing(float healing, Entity source) { }
	public virtual void OnWillTakeHealing(float healing, Entity source) { }
	public virtual void OnTookHealing(float healing, Entity source) { 
		if (health > 0) { FlashColor(Color.green); } 
	}

	public virtual void Damage(float damage, Entity source)
	{
		OnIncomingDamage(damage, source);
		if (invulnerable == false) {
			OnWillTakeDamage(damage, source);
			health -= damage;
			OnTookDamage(damage, source);
		}
		if (health <= 0) {
			OnWillBeDestroyed();
			Destroy(gameObject);
		}
	}
	public virtual void RemoteDamage(float damage, Entity source) { }
	public virtual void OnIncomingDamage(float damage, Entity source) { }
	public virtual void OnWillTakeDamage(float damage, Entity source) { }
	public virtual void OnTookDamage(float damage, Entity source)
	{
		// flash red when damaged if the agent is still alive
		if (health > 0) { FlashColor(Color.red); }
	}

	public virtual void OnWillBeDestroyed() {
		try {

			// if there's a destroyEffectPrefab, get one from the Pool and Activate it
			if (destroyEffectPrefab) {
				PoolManager.Get(destroyEffectPrefab).Activate(transform.position, transform.rotation);
			}

			// if there's a wreckagePrefab, get one from the Pool and Activate it
			if (wreckagePrefab) {
				PoolManager.Get(wreckagePrefab).Activate(transform.position, transform.rotation);
			}

			// add points when the agent is destroyed
			// the player can have a negative point value to penalize death
			GameManager.AddScore(pointValue);

			// spawn drops
			if (drops != null) { foreach (Drop item in drops) { item.TrySpawn(transform); } }

		}
		catch { }
	}
	protected virtual void OnDestroy() { }

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.FindComponent(out Entity entity)) {
			 entity.Damage(DamageOnCollision, this);
		}
	}

	#endregion

	#region Utility and Misc.

	/// <summary>
	/// Sets all Shapes on the Entity to a given color and smoothly fades them back over the specified duration
	/// </summary>
	public void FlashColor(Color color, float duration = 0.5f)
	{
		foreach (ShapeRenderer shape in shapes) { shape.FlashColor(color, duration); }
	}

	/// <summary>
	/// Creates a debug cross at the given position in the scene view to help with debugging.
	/// </summary>
	public void DebugCross(Vector2 position, float size = 0.5f, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		Vector2 xStart = position + Vector2.right * size * 0.5f;
		Vector2 xEnd = position - Vector2.right * size * 0.5f;

		Vector2 yStart = position + Vector2.up * size * 0.5f;
		Vector2 yEnd = position - Vector2.up * size * 0.5f;

		Debug.DrawLine(xStart, xEnd, color, duration, depthTest);
		Debug.DrawLine(yStart, yEnd, color, duration, depthTest);
		
		Debug.DrawLine(Position, position, color, duration, depthTest);

	}

	#endregion

}
