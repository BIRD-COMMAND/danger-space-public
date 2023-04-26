using Extensions;
using Shapes;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : Poolable
{
	[HideInInspector] public Agent shooter;
	[HideInInspector] public Weapon weapon;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public new Collider2D collider;
	public GameObject poolableTrailPrefab;
	private PoolableTrail poolableTrail;
	[HideInInspector] public float timer;

	private void Awake() { body = GetComponent<Rigidbody2D>(); collider = GetComponent<Collider2D>(); }

	public override Poolable Activate(Vector3 position, Quaternion rotation) {
		if (isActiveAndEnabled) { return this; }
		gameObject.SetActive(true);
		if (collider && shooter) { IgnoreCollisionsWithShooter(shooter, true); }
		transform.SetPositionAndRotation(position, rotation);
		if (poolableTrailPrefab) {
			poolableTrail = PoolManager.Get(poolableTrailPrefab) as PoolableTrail;
			poolableTrail.transform.SetParent(transform);
			poolableTrail.Activate(position, rotation);
		}
		return this;
	}
	public override Poolable Return()
	{
		if (!isActiveAndEnabled) { return this; }
		if (poolableTrail) { poolableTrail.Detach(); }
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
		if (collision.transform.FindComponent(out Agent agent)) { agent.Damage(weapon.projectileDamage); }
		if (weapon.impactEffect) { PoolManager.Get(weapon.impactEffect).Activate(transform.position, transform.rotation); }
		//Debug.Log("Projectile Return()ed by Projectile.OnCollisionEnter2D");
		Return();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		// Exited shooter's trigger collider
		if (shooter && collision.FindComponent(out Agent agent) && agent == shooter && collision == agent.bounds) {
			IgnoreCollisionsWithShooter(shooter, false);
		}
	}

}