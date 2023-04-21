using Shapes;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[HideInInspector] public bool enableCollision = false;
	[HideInInspector] public Module launcher;
	[HideInInspector] public Weapon weapon;
	[HideInInspector] public System.Collections.Generic.Queue<Projectile> pool;
	[HideInInspector] public Collider2D launchCollider;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public new Collider2D collider;
	[HideInInspector] public TrailRenderer trail;
	[HideInInspector] public float timer;

	public bool IsTrigger { 
		get { return GetComponentsInChildren<Collider2D>().Any(c => c.isTrigger);  }
		set { foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) { c.isTrigger = value; } }
	}

	public Projectile Activate() { if (trail) { trail.Clear(); } gameObject.SetActive(true); return this; }
	public Projectile Initialize(System.Collections.Generic.Queue<Projectile> pool) {
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		trail = GetComponent<TrailRenderer>();
		gameObject.SetActive(false);
		this.pool = pool;
		return this;
	}

	public void Launch(Module m)
	{
		enableCollision = true;
		launcher = m;
		IsTrigger = true;
		transform.SetPositionAndRotation(m.projectileSpawn.position, m.projectileSpawn.rotation);
		body.velocity = transform.up * m.projectileSpeed;
		launchCollider = m.collider;
		timer = m.projectileLifetime;
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) { Return(); }
	}

	public void Return() { gameObject.SetActive(false); pool.Enqueue(this); }

	private void OnTriggerExit2D(Collider2D other) { if (other == launchCollider) { IsTrigger = false; } }

	private void OnCollisionEnter2D(Collision2D collision)
	{ 
		if (!enableCollision) { return; }
		if (collision.collider.TryGetComponent(out Module m)) { 
			m.Hit(this); Debug.Log(m.name, m.gameObject); Return(); return;
		}
		AI ai = collision.transform.GetComponent<AI>();
		if (!ai) { ai = collision.transform.GetComponentInParent<AI>(); }
		if (ai) { 
			//TODO do some damage when a projectile hits an AI unit
			ai.GetComponentsInChildren<ShapeRenderer>().ToList().ForEach(d => d.FlashColor(Color.red));
			if (weapon) {
				ai.Damage((int)weapon.projectileDamage);
				if (weapon.impactEffect) { Destroy(Instantiate(weapon.impactEffect, transform.position, transform.rotation), 1f); }
			}
			Return();
		}
	}
}