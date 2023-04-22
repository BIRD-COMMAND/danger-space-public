using Extensions;
using Shapes;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[HideInInspector] public bool enableCollision = false;
	[HideInInspector] public Weapon weapon;
	[HideInInspector] public System.Collections.Generic.Queue<Projectile> pool;
	[HideInInspector] public Collider2D launchCollider;
	[HideInInspector] public Rigidbody2D body;
	[HideInInspector] public new Collider2D collider;
	[HideInInspector] public TrailRenderer trail;
	[HideInInspector] public float timer;

	public Projectile Activate() { gameObject.SetActive(true); return this; }
	public Projectile Initialize(System.Collections.Generic.Queue<Projectile> pool) {
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		trail = GetComponent<TrailRenderer>();
		gameObject.SetActive(false);
		this.pool = pool;
		return this;
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) { Return(); }
	}

	public void Return() { 
		if (trail) { trail.emitting = false; }
		gameObject.SetActive(false); 
		pool.Enqueue(this); 
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{ 
		if (!enableCollision) { return; }
		if (collision.transform.TryGetComponentInParent(out AI ai)) { 
			//TODO do some damage when a projectile hits an AI unit
			if (weapon) {
				ai.Damage((int)weapon.projectileDamage);
				if (weapon.impactEffect) { Instantiate(weapon.impactEffect, transform.position, transform.rotation); }
			}
			Return();
		}
	}
}