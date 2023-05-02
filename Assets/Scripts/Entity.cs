using System;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;

/// <summary>
/// An Entity represents a physics-driven entity in the game world with properties facilitating basic gameplay interactions.<br/>
/// Things like faction, health, damage, movement, and drops are all defined in the Entity class.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{

	#region General Fields and Properties (Component References, Drops, Health)

	/// <summary>
	/// The Factions this Entity belongs to.
	/// </summary>
	[Header("General")]
	public Faction faction = Faction.Neutral;

	/// <summary>
	/// The Rigidbody2D for the Entity.
	/// </summary>
	[SerializeField, HideInInspector]
	protected Rigidbody2D body;
	/// <summary>
	/// The Rigidbody2D for the Entity.
	/// </summary>
	public Rigidbody2D Body => body;

	/// <summary>
	/// The CircleCollider2D representing the bounds for the Entity. This is the source for the Entity's Radius property.
	/// </summary>
	[SerializeField, Tooltip("The CircleCollider2D representing the bounds for the Entity. This is the source for the Entity's Radius property.")]
	protected CircleCollider2D bounds;
	/// <summary>
	/// The CircleCollider2D representing the bounds for the Entity. This is the source for the Entity's Radius property.
	/// </summary>
	public CircleCollider2D Bounds => bounds;

	/// <summary>
	/// The Entity's current target.
	/// </summary>
	[SerializeField, Tooltip("The Entity's current target.")]
	protected Entity target;
	/// <summary>
	/// The Entity's current target.
	/// </summary>
	public Entity Target => target;
	
	/// <summary>
	/// An array of Drops that the Entity can drop when destroyed.
	/// </summary>
	[SerializeField]
	protected Drop[] drops;
	/// <summary>
	/// An array of Drops that the Entity can drop when destroyed.
	/// </summary>
	public Drop[] Drops => drops;

	/// <summary>
	/// The maximum health for the Entity.
	/// </summary>
	[SerializeField]
	protected float maxHealth = 10f;
	/// <summary>
	/// The maximum health for the Entity.
	/// </summary>
	public float MaxHealth => maxHealth;

	/// <summary>
	/// The current health for the Entity.
	/// </summary>
	[SerializeField]
	protected float health = 10f;
	/// <summary>
	/// The current health for the Entity.
	/// </summary>
	public float Health => health;
	
	/// <summary>
	/// The current health for the Entity as a percentage [0.0f - 1.0f] of its maximum health.
	/// </summary>
	public float HealthPercent => health / maxHealth;

	/// <summary>
	/// The point value for the Entity.
	/// </summary>
	[SerializeField]
	protected float pointValue = 100f;
	/// <summary>
	/// The point value for the Entity.
	/// </summary>
	public float PointValue => pointValue;

	/// <summary>
	/// All Shapes that are part of this Entity.
	/// </summary>
	protected List<ShapeRenderer> shapes;
	/// <summary>
	/// All Shapes that are part of this Entity.
	/// </summary>
	public List<ShapeRenderer> Shapes => shapes;

	#endregion

	#region Damage and Destruction Fields and Properties

	/// <summary>
	/// The amount of damage the Entity deals on collision.
	/// </summary>
	[Header("Damage")]
	[SerializeField]
	protected float damageOnCollision = 2f;
	/// <summary>
	/// The amount of damage the Entity deals on collision.
	/// </summary>
	public float DamageOnCollision => damageOnCollision;

	/// <summary>
	/// The effect prefab used when the Entity is destroyed.
	/// </summary>
	[SerializeField]
	protected GameObject destroyEffectPrefab;
	/// <summary>
	/// The effect prefab used when the Entity is destroyed.
	/// </summary>
	public GameObject DestroyEffectPrefab => destroyEffectPrefab;
	
	/// <summary>
	/// The wreckage prefab used when the Entity is destroyed.
	/// </summary>
	[SerializeField]
	protected GameObject wreckagePrefab;
	/// <summary>
	/// The wreckage prefab used when the Entity is destroyed.
	/// </summary>
	public GameObject WreckagePrefab => wreckagePrefab;

	/// <summary>
	/// A boolean representing whether the Entity is currently invulnerable.
	/// </summary>
	[SerializeField]
	protected bool invulnerable;
	/// <summary>
	/// A boolean representing whether the Entity is currently invulnerable.
	/// </summary>
	public bool Invulnerable { get => invulnerable; set => invulnerable = value; }

	#endregion

	#region Movement Fields and Properties

	/// <summary>
	/// The maximum velocity for the Entity.
	/// </summary>
	[Header("Movement")]
	public float maxVelocity = 3.5f;
	/// <summary>
	/// The maximum acceleration for the Entity.
	/// </summary>
	public float maxAcceleration = 10f;
	/// <summary>
	/// The maximum angular velocity for the Entity.
	/// </summary>
	public float turnSpeed = 20f;
	/// <summary>
	/// A boolean representing whether the Entity is currently in Bullet Time.<br/>
	/// A value of true means the Entity should move normally while GameManager.BulletTime is true.
	/// </summary>
	public bool inBulletTime = false;

	[Header("Look Direction Smoothing")]
	/// <summary>
	/// Smoothing controls if the character's look direction should be an
	/// average of its previous directions (to smooth out momentary changes
	/// in directions)
	/// </summary>
	public bool smoothing = true;
	/// <summary>
	/// The number of samples to use for smoothing the look direction.
	/// </summary>
	public int numSamplesForSmoothing = 5;
	/// <summary>
	/// The samples used for smoothing the look direction.
	/// </summary>
	protected Queue<Vector2> rotationSamples = new Queue<Vector2>();

	#endregion

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
	
	
	/// <summary>
	/// Validate bounds and assign component references. Destroy the Entity if the bounds are not valid.
	/// </summary>
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
	
	/// <summary>
	/// Attempt to automatically set up bounds when an Entity component is added to a GameObject with a RegularPolygon component.
	/// </summary>
	private void Reset()
	{
		if (!bounds && TryGetComponent(out RegularPolygon shape)) {
			bounds = gameObject.AddComponent<CircleCollider2D>();
			bounds.radius = shape.Radius; bounds.isTrigger = true;
		}
	}


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
	/// Makes the Entity look in the direction it is moving
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
	/// Makes the Entity look towards its target
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
	/// <summary>
	/// Makes the Entity look towards the given direction
	/// </summary>
	public void LookAtDirection(Vector2 direction)
	{
		direction.Normalize();

		/* If we have a non-zero direction then look towards that direciton otherwise do nothing */
		if (direction.sqrMagnitude > 0.001f) {
			float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
			Rotation = Mathf.LerpAngle(Rotation + 90f, toRotation - 90f, Time.deltaTime * turnSpeed);
		}
	}
	/// <summary>
	/// Makes the Entity look towards the given direction
	/// </summary>
	/// <param name="toRotation"></param>
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
	/// Returns true if the target is in front of the Entity
	/// </summary>
	public bool IsFacing(Vector2 target)
	{
		Vector2 facing = transform.up.normalized;
		Vector2 directionToTarget = (target - Position).normalized;
		return Vector2.Dot(facing, directionToTarget) > 0f;
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

	#endregion

	#region Damage and Destruction

	/// <summary>
	/// Heals the entity by the given amount.
	/// </summary>
	public virtual void Heal(float healing, Entity source)
	{
		if (health > 0) {
			health += healing;
			OnTookHealing(healing, source);
		}
		if (health > maxHealth) { health = maxHealth; }
	}
	/// <summary>
	/// Event called after an entity has been healed. The default implementation flashes the entity green.
	/// </summary>
	public virtual void OnTookHealing(float healing, Entity source) { 
		if (health > 0) { FlashColor(Color.green); } 
	}

	/// <summary>
	/// Damages the entity by the given amount.
	/// </summary>
	public virtual void Damage(float damage, Entity source)
	{
		if (Invulnerable == false) {
			health -= damage;
			OnTookDamage(damage, source);
		}
		if (health <= 0) {
			OnWillBeDestroyed();
			Destroy(gameObject);
		}
	}
	/// <summary>
	/// Event called after an entity has been damaged. The default implementation flashes the entity red.
	/// </summary>
	public virtual void OnTookDamage(float damage, Entity source)
	{
		// flash red when damaged if the agent is still alive
		if (health > 0) { FlashColor(Color.red); }
	}

	/// <summary>
	/// Event called when the entity is about to be destroyed.<br/>
	/// The default implementation spawns the destroyEffectPrefab and wreckagePrefab and adds to GameManager.score
	/// </summary>
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

	/// <summary>
	/// Event called when the entity collides with something.<br/>
	/// The default implementation damages the other entity if another entity was involved in the collision.
	/// </summary>
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

	#region Faction

	[Flags]
	public enum Faction
	{
		None = 0, Structure = 1, Player = 2, Neutral = 4,
		Alliance = 8, Rebellion = 16, Empire = 32, Raider = 64, Pirate = 128
	}
	
	public bool IsPlayer => faction.HasFlag(Faction.Player);
	public bool IsStructure => faction.HasFlag(Faction.Structure);
	public bool IsNeutral => faction.HasFlag(Faction.Neutral);
	
	/// <summary>
	/// Returns true if the Entities share a Faction flag besides None, Structure, or Player.
	/// </summary>
	public bool IsFriendly(Entity other) { return (int)(faction & other.faction) > 3; }

	#endregion

}
