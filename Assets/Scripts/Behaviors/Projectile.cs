using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	
	[HideInInspector] public Module launcher;
	
	private System.Collections.Generic.Queue<Projectile> pool;
	private Collider2D launchCollider;
	private Rigidbody2D body;
	private float timer;

	public bool IsTrigger { 
		get { return GetComponentsInChildren<Collider2D>().Any(c => c.isTrigger);  }
		set { foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) { c.isTrigger = value; } }
	}

	public Projectile Activate() { gameObject.SetActive(true); return this; }
	public Projectile Initialize(System.Collections.Generic.Queue<Projectile> pool) {
		body = GetComponent<Rigidbody2D>();
		gameObject.SetActive(false);
		this.pool = pool;
		return this;
	}

	public void Launch(Module m)
	{
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
		if (collision.collider.TryGetComponent(out Module m)) { 
			m.Hit(this); Debug.Log(m.name, m.gameObject); Return();
		}
	}
}