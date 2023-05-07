using Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AsteroidCluster manages a group of Asteroids that are loosely bound together by simulated gravitational attraction using SpringJoint2Ds attached to a single Core asteroid
/// </summary>
public class AsteroidCluster : Entity
{
	
	// the AsteroidCluster represents a group of asteroids that are loosely bound together by gravitational attraction
	//
	// the root object of the hierarchy is a kinematic Rigidbody2D which is used to move the cluster around the scene,
	// but it also acts as the anchor body for a SpringJoint2D on the core asteroid of the cluster
	//
	// by decoupling the AsteroidCluster component from the core asteroid gameobject, we achieve two main benefits:
	// - we can move the cluster around the scene without jostling the asteroids' spring joints around
	// - and we avoid the confusion of having additional trigger colliders on any one asteroid,
	//   as the cluster's trigger collider is used solely for detecting and adding asteroids to the cluster

	[Header("Asteroid Cluster")]
	[SerializeField, Tooltip("The core Asteroid of the cluster. The other asteroids are bound to it by loose SpringJoint2Ds.")]
	///<summary>
	///The core Asteroid of the cluster. The other asteroids are bound to it by loose SpringJoint2Ds.
	/// </summary>
	private Asteroid core;
	/// <summary>
	/// The other asteroids in the cluster. They are bound to the core by loose SpringJoint2Ds.
	/// </summary>
	private List<Asteroid> asteroids = new List<Asteroid>();

	protected override void Awake()
	{
		base.Awake();

		// get all child Asteroids and add them to the cluster list (except for the core)
		asteroids = GetComponentsInChildren<Asteroid>().ToList();
		if (asteroids.Contains(core)) { asteroids.Remove(core); }

	}

	private void FixedUpdate()
	{

		// remove invalid asteroids from the cluster list
		asteroids.RemoveAll(a => a == null);

		// if the core is missing, destroy the cluster
		if (!core) { OnWillBeDestroyed(); }

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{

		// the code below adds rogue asteroids to the cluster if they don't belong to one already		
		
		// if a non-core asteroid enters the trigger and the asteroid is not already in the cluster

		if (collision && collision.FindComponent(out Asteroid asteroid) && !asteroid.isCore) {
			if (!asteroids.Contains(asteroid) && asteroid.FindComponent(out SpringJoint2D spring)) {
				
				// bind the asteroid to the cluster by setting its connectedBody and connectedAnchor
				// enable the spring joint, add the asteroid to the cluster list, and set its parent transform
				
				spring.connectedAnchor = (core.Position - asteroid.Position).normalized * (core.Radius + asteroid.Radius + 3f);
				spring.connectedBody = core.Body;
				spring.enabled = true;
				asteroids.Add(asteroid);
				asteroid.transform.SetParent(transform);

			}
		}

	}

	/// <summary>
	/// Do not allow entire Asteroid Clusters to be duplicated
	/// </summary>
	public override Entity Duplicate() { return this; }

	/// <summary>
	/// Disband the cluster before destroying it
	/// </summary>
	public override void OnWillBeDestroyed()
	{
		DisbandCluster();
		base.OnWillBeDestroyed();
	}

	/// <summary>
	/// Disband the cluster, releasing all asteroids from the core's gravitational pull
	/// </summary>
	public void DisbandCluster()
	{
		// disable each asteroid's SpringJoint2D, set its connectedBody to null
		foreach (Asteroid item in asteroids) {
			if (item && item.FindComponent(out SpringJoint2D spring)) {
				spring.enabled = false; spring.connectedBody = null;
			}
		}
	}

}