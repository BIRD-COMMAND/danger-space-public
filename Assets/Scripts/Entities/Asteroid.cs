using Extensions;
using UnityEngine;

/// <summary>
/// An Asteroid is an environmental Entity, they can damage other entities on contact, but are currently invulnerable to damage themselves.
/// </summary>
public class Asteroid : Entity
{

	[Header("Asteroid"), Tooltip("Indicates whether this asteroid is the core of an asteroid cluster")]	
	/// <summary>
	/// Indicates whether this asteroid is the core of an asteroid cluster
	/// </summary>
	public bool isCore = false;

	protected override void Awake()
	{
		base.Awake();
		
		if (isCore) { return; }
		
		// this is a random positioning of the asteroid relative to the core asteroid of the cluster.
		// this allows for placing multiple prefabs of the same cluster, rotating each of them a bit,
		// and ending up with relatively unique looking clusters, as the spring oscillations will be different.
		// note the return statement above, we don't want to do this for the core asteroid

		Body.position = Vector2.Lerp(
			Position, Position + ((Position - GetComponent<SpringJoint2D>().connectedBody.position) * 0.75f),
			UnityEngine.Random.Range(0f, 1f)
		);

	}

	// currently asteroids are invulnerable, so we just flash them white when they take damage, for the aesthetic
	public override void Damage(float damage, Entity source) { FlashColor(Color.white); }

	protected override void OnCollisionEnter2D(Collision2D collision) {
		// this code is to prevent asteroids from flashing every time they bump into each other
		if (!collision.collider.FindComponent<Asteroid>()) { base.OnCollisionEnter2D(collision); }
	}

	/// <summary>
	/// Edit mode draw logic
	/// </summary>
	public override void OnEditModeDisplay() { DrawSpring(Color.white); }

	/// <summary>
	/// Reposition spring when moved
	/// </summary>
	public override void OnEditModeMoved(Vector2 oldPosition)
	{
		if (isCore) { return; } SpringJoint2D spring = GetComponent<SpringJoint2D>(); if (!spring) { return; }
		if (spring.connectedBody) { spring.connectedAnchor = spring.connectedBody.transform.InverseTransformPoint(Position); }
		else { spring.connectedAnchor = Vector2.zero; }
	}

}
