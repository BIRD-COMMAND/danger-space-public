using UnityEngine;
using Extensions;

/// <summary>
/// The Missile class inherits from the Agent class and represents a missile object in the game world.<br/>
/// It contains behavior for pursuing enemies, avoiding obstacles, and handling collisions.
/// </summary>
public class Missile : Agent
{

	/// <summary>
	/// The missile performs its steering behavior on each FixedUpdate.
	/// </summary>
	private void FixedUpdate()
	{
		switch (state) {
			case State.Enemy_Pursue:	Enemy_Pursue();	break;
			default: break;
		}
	}

	/// <summary>
	/// This method handles the missile's behavior when it's pursuing an enemy target.<br/>
	/// It calculates the missile's acceleration based on several steering behaviors.
	/// </summary>
	private void Enemy_Pursue()
	{

		target = GameManager.Player;
		if (!target) { return; }

		// Calculate acceleration based on various steering behaviors
		Vector2 accel = Vector2.zero;
		accel += Pursue(target);
		accel += SeparationAll();
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		accel += FlowField.GetForce(this);

		// Apply the resulting acceleration
		Steer(accel);

	}

	/// <summary>
	/// This method is called when the missile enters a collision with another object.<br/>
	/// It handles destruction if the collision force is above a certain threshold.
	/// </summary>
	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		if (collision.ApproximateForce().magnitude < 1f) { return; }
		OnWillBeDestroyed();
	}

}