using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class Missile : Agent
{

	private void FixedUpdate()
	{
		//EvaluateState:
		switch (state) {
			case State.Enemy_Pursue:					Enemy_Pursue();	break;
			default: break;
		}
	}

	private void Enemy_Pursue()
	{

		target = GameManager.Player;
		if (!target) { return; }

		Vector2 accel = Vector2.zero;
		accel += Pursue(target);
		accel += SeparationAll();
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		accel += FlowField.GetForce(this) * 6f;
		Steer(accel);

	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		if (collision.ApproximateForce().magnitude < 1f) { return; }
		OnWillBeDestroyed(); Destroy(gameObject);
	}

}
