using System;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;
using System.Linq;

/// <summary>
/// An Entity represents a physics-driven entity in the game world with properties facilitating basic gameplay interactions.<br/>
/// Things like faction, health, damage, movement, and drops are all defined in the Entity class.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Entity : Poolable
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
	[SerializeField, Tooltip("An array of Drops that the Entity can drop when destroyed.")]
	protected Drop[] drops;
	/// <summary>
	/// An array of Drops that the Entity can drop when destroyed.
	/// </summary>
	public Drop[] Drops => drops;

	/// <summary>
	/// The maximum health for the Entity.
	/// </summary>
	[SerializeField, Tooltip("The maximum health for the Entity.")]
	protected float maxHealth = 10f;
	/// <summary>
	/// The maximum health for the Entity.
	/// </summary>
	public float MaxHealth => maxHealth;

	/// <summary>
	/// The current health for the Entity.
	/// </summary>
	[SerializeField, Tooltip("The current health for the Entity.")]
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
	[SerializeField, Tooltip("The point value for the Entity.")]
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

	/// <summary>
	/// Returns true if the Entity has an associated Object Pool.
	/// </summary>
	public bool Poolable => pool != null;

	#endregion

	#region Damage and Destruction Fields and Properties

	/// <summary>
	/// The amount of damage the Entity deals on collision.
	/// </summary>
	[Header("Damage"), SerializeField, Tooltip("The amount of damage the Entity deals on collision.")]
	protected float damageOnCollision = 2f;
	/// <summary>
	/// The amount of damage the Entity deals on collision.
	/// </summary>
	public float DamageOnCollision => damageOnCollision;

	/// <summary>
	/// The effect prefab used when the Entity is destroyed.
	/// </summary>
	[SerializeField, Tooltip("The effect prefab used when the Entity is destroyed.")]
	protected GameObject destroyEffectPrefab;
	/// <summary>
	/// The effect prefab used when the Entity is destroyed.
	/// </summary>
	public GameObject DestroyEffectPrefab => destroyEffectPrefab;
	
	/// <summary>
	/// The wreckage prefab used when the Entity is destroyed.
	/// </summary>
	[SerializeField, Tooltip("The wreckage prefab used when the Entity is destroyed.")]
	protected GameObject wreckagePrefab;
	/// <summary>
	/// The wreckage prefab used when the Entity is destroyed.
	/// </summary>
	public GameObject WreckagePrefab => wreckagePrefab;

	/// <summary>
	/// A boolean representing whether the Entity is currently invulnerable.
	/// </summary>
	[SerializeField, Tooltip("A boolean representing whether the Entity is currently invulnerable.")]
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
	[Header("Movement"), Tooltip("The maximum velocity for the Entity.")]
	public float maxVelocity = 3.5f;
	/// <summary>
	/// The maximum acceleration for the Entity.
	/// </summary>
	[Tooltip("The maximum acceleration for the Entity.")]
	public float maxAcceleration = 10f;
	/// <summary>
	/// The maximum angular velocity for the Entity.
	/// </summary>
	[Tooltip("The maximum angular velocity for the Entity.")]
	public float turnSpeed = 20f;
	/// <summary>
	/// The multiplier applied to the Entity's acceleration from the flow field.
	/// </summary>
	[Range(1f, 40f), Tooltip("The multiplier applied to the Entity's acceleration from the flow field.")]
	public float flowFieldMultiplier = 4f;
	/// <summary>
	/// Whether the Entity is currently in Bullet Time. A value of true means the Entity should move normally while GameManager.BulletTime is true.
	/// </summary>
	[Tooltip("Whether the Entity is currently in Bullet Time. A value of true means the Entity should move normally while GameManager.BulletTime is true.")]
	public bool inBulletTime = false;

	[Header("Look Direction Smoothing")]
	/// <summary>
	/// Smoothing controls if the character's look direction should be an average of its previous directions (to smooth out momentary changes in directions)
	/// </summary>
	[Tooltip("Smoothing controls if the character's look direction should be an average of its previous directions (to smooth out momentary changes in directions)")]
	public bool smoothing = true;
	/// <summary>
	/// The number of samples to use for smoothing the look direction.
	/// </summary>
	[Tooltip("The number of samples to use for smoothing the look direction.")]
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
	protected virtual void Reset()
	{
		health = maxHealth;
		if (!bounds && TryGetComponent(out RegularPolygon shape)) {
			bounds = gameObject.AddComponent<CircleCollider2D>();
			bounds.radius = shape.Radius; bounds.isTrigger = true;
		}
	}

	/// <summary>
	/// Activation logic for poolable Entities. Called by whatever spawns the poolable Entity.
	/// </summary>
	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		transform.SetPositionAndRotation(position, rotation);

		// resolve any overlaps with other colliders
		if (!GameManager.EditMode) { ResolveOverlaps(); }
		
		// enable the SpawnInvulnerability component if present
		if (TryGetComponent(out SpawnInvulnerability component)) { component.enabled = true; }
		
		// set the gameobject to active
		gameObject.SetActive(true);
		
		// clear TrailRenderer if present
		if (gameObject.FindComponent(out TrailRenderer trail)) { trail.Clear(); }
		
		// fix any lingering color flashes
		FlashColor(Color.white, 0.1f);
		
		return this;
	}

	/// <summary>
	/// Return logic for poolable Entities. Called by OnWillBeDestroyed().<br/>
	/// If the Entity has no associated pool, it will be Destroy()ed instead.
	/// </summary>
	public override Poolable Return()
	{
		health = maxHealth;				// reset health
		gameObject.SetActive(false);	// disable the gameobject
		pool.queue.Enqueue(this);		// return to the pool
		return this;
	}

	/// <summary>
	/// Duplicates the Entity and returns the duplicate.
	/// </summary>
	public virtual Entity Duplicate()
	{
		Entity duplicate;
		if (pool) { duplicate = pool.Get().Activate(transform.position, transform.rotation) as Entity; }
		else { duplicate = Instantiate(this, transform.position, transform.rotation, transform.parent); }
		return duplicate;
	}


	#region Edit Mode Callbacks

	/// <summary>
	/// Override this method to add custom logic for drawing visualizations for Edit Mode.
	/// </summary>
	public virtual void OnEditModeDisplay() { }

	/// <summary>
	/// Override this method to add custom logic for when Edit Mode is started
	/// </summary>
	public virtual void OnEditModeStarted() { }

	/// <summary>
	/// Override this method to add custom logic for when Edit Mode is stopped
	/// </summary>
	public virtual void OnEditModeStopped() { }

	/// <summary>
	/// Override this method to add custom logic for when the Entity is moved in Edit Mode.
	/// </summary>
	public virtual void OnEditModeMoved(Vector2 oldPosition) { }

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

		if (pool) { Return(); } 
		else { Destroy(gameObject); }

	}

	/// <summary>
	/// Event called when the entity collides with something.<br/>
	/// The default implementation damages the other entity if another entity was involved in the collision.
	/// </summary>
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.activeSelf && collision.transform.FindComponent(out Entity entity)) {
			 entity.Damage(DamageOnCollision, this);
		}
	}

	#endregion

	#region Utility and Misc.

	/// <summary>
	/// Resolve all Collider2D overlaps between this Entity and other colliders<br/>
	/// Overlaps are resolved by moving the Entity away from the world origin by the overlap distance plus a small buffer
	/// </summary>
	public void ResolveOverlaps()
	{
		bool overlapsResolved = false; float overlap;
		while (!overlapsResolved) {			
			overlaps = Physics2D.OverlapCircleAll(bounds.bounds.center, bounds.radius, LevelBoundsMask); overlapsResolved = true;
			foreach (Collider2D collider in overlaps) {
				if (collider.isTrigger || collider.transform.IsChildOf(transform)) { continue; }
				overlap = bounds.radius + collider.bounds.extents.magnitude - Vector2.Distance(bounds.bounds.center, collider.bounds.center);
				if (overlap > 0) { transform.position = transform.position + transform.position.normalized * (overlap * 1.2f); overlapsResolved = false; }
			}
		}
	}
	/// <summary>
	/// Collider2D[] used to store overlaps in ResolveOverlaps()
	/// </summary>
	protected static Collider2D[] overlaps;
	/// <summary>
	/// LayerMask used to ignore the LevelBounds layer in ResolveOverlaps()
	/// </summary>
	protected static readonly LayerMask LevelBoundsMask = ~(1 << 3);


	/// <summary>
	/// Gets all entities other than the target within this Entity's Radius
	/// </summary>
	public IEnumerable<Entity> GetEntitiesExcludingTarget() { return GetEntitiesExcludingTarget(Radius); }
	/// <summary>
	/// Gets all entities other than the target within the given radius
	/// </summary>
	public IEnumerable<Entity> GetEntitiesExcludingTarget(float radius)
	{
		if (!float.IsNormal(radius) || radius < 0f) { radius = Radius; }
		return
			Physics2D.OverlapCircleAll(ColliderPosition, radius)
			.Where(c => c.FindComponent(out Entity entity) && entity != this && entity != target)
			.Select(c => c.FindComponent<Entity>()).Distinct();
	}

	/// <summary>
	/// Gets all entities within this Entity's Radius
	/// </summary>
	public IEnumerable<Entity> GetEntities() { return GetEntities(Radius); }
	/// <summary>
	/// Gets all entities within the given radius
	/// </summary>
	public IEnumerable<Entity> GetEntities(float radius)
	{
		if (!float.IsNormal(radius) || radius < 0f) { radius = Radius; }
		return
			Physics2D.OverlapCircleAll(ColliderPosition, radius)
			.Where(c => c.FindComponent(out Entity entity) && entity != this)
			.Select(c => c.FindComponent<Entity>()).Distinct();
	}

	/// <summary>
	/// Sets all Shapes on the Entity to a given color and smoothly fades them back over the specified duration
	/// </summary>
	public void FlashColor(Color color, float duration = 0.5f)
	{
		if (!gameObject.activeSelf) { return; }
		foreach (ShapeRenderer shape in shapes) { shape.FlashColor(color, duration); }
	}

	/// <summary>
	/// Draws a circle representing the Entity's Radius CircleCollider2D.
	/// </summary>
	public void DrawRadius(Color color) {
		using (Draw.Command(Camera.main)) { Draw.Ring(Position, Radius, 0.3f, color); }
	}

	/// <summary>
	/// Draws a line representing the SpringJoint2D attached to the Entity.
	/// </summary>
	public void DrawSpring(Color color)
	{
		drawSpring = GetComponent<SpringJoint2D>();
		if (!drawSpring) { return; }
		using (Draw.Command(Camera.main)) {
			if (drawSpring.connectedBody) { Draw.Line(transform.TransformPoint(drawSpring.anchor), drawSpring.connectedBody.transform.TransformPoint(drawSpring.connectedAnchor), color); }
			else { Draw.Line(transform.TransformPoint(drawSpring.anchor), transform.TransformPoint(drawSpring.connectedAnchor), color); }			 
		}
	}
	private static SpringJoint2D drawSpring;

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

	/// <summary>
	/// An enum used to denote the Faction of an Entity.
	/// </summary>
	[Flags]
	public enum Faction
	{
		None = 0, Structure = 1, Player = 2, Neutral = 4,
		Alliance = 8, Rebellion = 16, Empire = 32, Raider = 64, Pirate = 128
	}
	
	/// <summary>
	/// Returns true if the Entity has the Player Faction flag.
	/// </summary>
	public bool IsPlayer => faction.HasFlag(Faction.Player);
	/// <summary>
	/// Returns true if the Entity has the Structure Faction flag.
	/// </summary>
	public bool IsStructure => faction.HasFlag(Faction.Structure);
	/// <summary>
	/// Returns true if the Entity has the Neutral Faction flag.
	/// </summary>
	public bool IsNeutral => faction.HasFlag(Faction.Neutral);
	
	/// <summary>
	/// Returns true if the Entities share a Faction flag besides None, Structure, or Player.
	/// </summary>
	public bool IsFriendly(Entity other) { return (int)(faction & other.faction) > 3; }

	#endregion

}
