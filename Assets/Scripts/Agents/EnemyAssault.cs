using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// This Unit has 2 states: Enemy_Pursue and Self_Flee.<br/>
/// While pursuing it attempts to maintain a certain distance and fires at the player as much as possible.<br/>
/// If its HealthPercent drops below the FleeThreshold or it has no target it wanders, avoiding all engagements.
/// </summary>
public class EnemyAssault : Agent
{

	/// <summary>
	/// Agent will flee when health percentage is below this value
	/// </summary>
	[Header("Enemy Assault"), Range(0f, 1f), Tooltip("Agent will flee when health percentage is below this value")]
	public float fleeThreshold = 0.5f;

	/// <summary>
	/// The agent can only fire its weapon if this is set to true
	/// </summary>
	[Tooltip("The agent can only fire its weapon if this is set to true")]
	public bool fireWeapon = true;

	/// <summary>
	/// Execute the appropriate behavior based on the Agent's current state
	/// </summary>
	private void FixedUpdate()
	{
		
		// try to target the player, otherwise wander

		target = GameManager.Player;
		if (!target) { state = State.Self_Flee; Self_Flee(); return; }

		// set current agent state based on health percentage and fleeThreshold

		state = HealthPercent > fleeThreshold ? State.Enemy_Pursue : State.Self_Flee;

		switch (state) {
			case State.Self_Flee:		Self_Flee();		break;
			case State.Enemy_Pursue:	Enemy_Pursue();		break;
			default:break;
		}

	}

	/// <summary>
	/// A pursuit behavior that maintains a certain distance from the target and fires at it if possible
	/// </summary>
	private void Enemy_Pursue()
	{
		
		// update path with target position
		path[0] = Position; path[1] = target.Position;

		// maintain range of 40, avoid obstacles, avoid collisions with allies
		Vector2 accel = Vector2.zero;
		accel += GetInRange(target, 40f);
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		accel += FlowField.GetForce(this);
		Steer(accel);

		// face target if within 50, else face heading
		if (Vector2.Distance(Position, target.Position) < 50f) { FaceTarget(); }
		else { FaceHeading(); }

		// if can fireWeapon, IsOnScreen, and TargetInSight, fire weapon
		if (fireWeapon && IsOnScreen && TargetInSight(target)) { GetComponent<Weapon>().Fire(this); }

	}

	/// <summary>
	/// A wandering behavior avoiding collisions and obstacles
	/// </summary>
	private void Self_Flee() {
		Steer(Wander() + AvoidObstacles() + AvoidCollisionsAll());
		FaceHeading(); 
	}

}
