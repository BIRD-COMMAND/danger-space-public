using Extensions;
using Shapes;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : Poolable
{
	[HideInInspector] public Entity shooter;
	[HideInInspector] public Weapon weapon;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public new Collider2D collider;
	public GameObject poolableTrailPrefab;
	[HideInInspector] public float timer;

	private void Awake() { body = GetComponent<Rigidbody2D>(); collider = GetComponent<Collider2D>(); }

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
	public override Poolable Return()
	{
		if (!isActiveAndEnabled) { return this; }
		try { GetComponentInChildren<PoolableTrail>().Detach(); } catch {}
		body.velocity = Vector2.zero;
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) { Return(); /*Debug.Log("Projectile Return()ed by Projectile.Update timer");*/ return; }
	}

	public void IgnoreCollisionsWithShooter(Component launcher, bool ignore) {
		foreach (Collider2D item in launcher.FindComponents<Collider2D>()) { Physics2D.IgnoreCollision(collider, item, ignore); }
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{ 
		if (collision.transform.FindComponent(out Entity entity)) { entity.Damage(weapon.projectileDamage, shooter); }
		if (weapon.impactEffect) { PoolManager.Get(weapon.impactEffect).Activate(transform.position, transform.rotation); }
		//Debug.Log("Projectile Return()ed by Projectile.OnCollisionEnter2D");
		Return();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		// Exited shooter's trigger collider
		if (shooter && collision.FindComponent(out Entity entity) && entity == shooter && collision == entity.Bounds) {
			IgnoreCollisionsWithShooter(shooter, false);
		}
	}

}