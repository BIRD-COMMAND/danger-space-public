using Extensions;
using UnityEngine;

/// <summary>
/// Projectile class represents projectiles fired by weapons in the game.
/// </summary>
public class Projectile : Poolable
{
	/// <summary>
	/// Prefab for the trail effect attached to the projectile
	/// </summary>
	public GameObject poolableTrailPrefab;
	/// <summary>
	/// Prefab for the impact effect spawned when the projectile hits something.
	/// </summary>
	public GameObject impactEffectPrefab;
	/// <summary>
	/// The damage the projectile deals
	/// </summary>
	public float damage = 0f;

	/// <summary>
	/// The entity that fired the projectile
	/// </summary>
	[HideInInspector] public Entity shooter;
	/// <summary>
	/// The weapon that fired the projectile
	/// </summary>
	[HideInInspector] public Weapon weapon;
	/// <summary>
	/// The rigidbody attached to the projectile
	/// </summary>
	[HideInInspector] public Rigidbody2D body;
	/// <summary>
	/// The collider attached to the projectile
	/// </summary>
	[HideInInspector] public new Collider2D collider;
	/// <summary>
	/// Timer for the projectile's lifespan
	/// </summary>
	[HideInInspector] public float timer;

	private void Awake() { body = GetComponent<Rigidbody2D>(); collider = GetComponent<Collider2D>(); }

	/// <summary>
	/// Activates the projectile at the specified position and rotation.
	/// </summary>
	/// <param name="position">The position to activate the projectile at.</param>
	/// <param name="rotation">The rotation to activate the projectile with.</param>
	/// <returns>The activated projectile.</returns>
	public override Poolable Activate(Vector3 position, Quaternion rotation) {
		if (isActiveAndEnabled) { return this; }
		gameObject.SetActive(true);
		if (collider && shooter) { IgnoreCollisionsWithShooter(shooter, true); }
		transform.SetPositionAndRotation(position, rotation);
		if (poolableTrailPrefab) {
			PoolManager
				.Get(poolableTrailPrefab)
				.Activate(position, rotation)
				.transform.SetParent(transform);
		}
		return this;
	}
	
	/// <summary>
	/// Returns the projectile to its object pool.
	/// </summary>
	/// <returns>The returned projectile.</returns>
	public override Poolable Return()
	{
		if (!isActiveAndEnabled) { return this; }
		try { GetComponentInChildren<PoolableTrail>().Detach(); } catch {}
		body.velocity = Vector2.zero;
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	/// <summary>
	/// Handles the projectile's lifespan timer.
	/// </summary>
	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) { Return(); return; }
	}

	/// <summary>
	/// Ignores collisions between the projectile and the specified launcher.
	/// </summary>
	/// <param name="launcher">The launcher to ignore collisions with.</param>
	/// <param name="ignore">Whether to ignore the collisions or not.</param>
	public void IgnoreCollisionsWithShooter(Component launcher, bool ignore) {
		foreach (Collider2D item in launcher.FindComponents<Collider2D>()) { Physics2D.IgnoreCollision(collider, item, ignore); }
	}

	/// <summary>
	/// The entity that was hit by the projectile.
	/// </summary>
	protected static Entity hitEntity;
	/// <summary>
	/// OnCollisionEnter2D is called when the projectile collides with another object.
	/// Handles damaging the collided entity and creating an impact effect.
	/// </summary>
	/// <param name="collision">The collision data.</param>
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{ 
		if (collision.transform.FindComponent(out hitEntity)) { hitEntity.Damage(damage, shooter); }
		if (impactEffectPrefab) { PoolManager.Get(impactEffectPrefab).Activate(transform.position, transform.rotation); }
		Return();
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		// Exited shooter's trigger collider
		if (shooter && collision.FindComponent(out Entity entity) && entity == shooter && collision == entity.Bounds) {
			IgnoreCollisionsWithShooter(shooter, false);
		}
	}

}