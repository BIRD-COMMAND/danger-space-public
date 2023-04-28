using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Asteroid : Entity
{

	[Header("Asteroid")]

	public bool isCore = false;

	protected override void Awake()
	{
		base.Awake();
		if (isCore) { return; }
		Body.position = Vector2.Lerp(
			Position, Position + ((Position - GetComponent<SpringJoint2D>().connectedBody.position) * 0.75f),
			UnityEngine.Random.Range(0f, 1f)
		);
	}

	public override void Damage(float damage) {
		FlashColor(Color.white);
	}

	protected override void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.collider.FindComponent<Asteroid>()) { base.OnCollisionEnter2D(collision); }
	}

}
