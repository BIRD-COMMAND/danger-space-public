using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AsteroidCluster : Entity
{
	
	[Header("Asteroid Cluster")]

	[SerializeField] private Asteroid core;
	private List<Asteroid> asteroids = new List<Asteroid>();

	protected override void Awake()
	{
		base.Awake();
		asteroids = GetComponentsInChildren<Asteroid>().ToList();
		if (asteroids.Contains(core)) { asteroids.Remove(core); }
	}

	private void FixedUpdate()
	{
		if (!core) {
			foreach (Asteroid item in asteroids) {
				if (item && item.FindComponent(out SpringJoint2D spring)) {
					spring.enabled = false; spring.connectedBody = null;
					item.transform.SetParent(null);
				}
			}
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision && collision.FindComponent(out Asteroid asteroid) && !asteroid.isCore) {
			if (!asteroids.Contains(asteroid) && asteroid.FindComponent(out SpringJoint2D spring)) {
				spring.connectedAnchor = (core.Position - asteroid.Position).normalized * (core.Radius + asteroid.Radius + 3f);
				spring.connectedBody = core.Body;
				spring.enabled = true;
				asteroids.Add(asteroid);
				asteroid.transform.SetParent(transform);
			}
		}
	}

}
