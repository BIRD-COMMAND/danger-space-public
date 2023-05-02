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

	private void Start() { 
		// set a default path to the world origin
		path = new LinePath(Position, Vector3.zero); 
	}

	private void FixedUpdate()
	{
		
		// try to target the player, otherwise wander

		target = GameManager.Player;
		if (!target) { Self_Flee(); return; }

		// set current agent state based on health percentage and fleeThreshold

		state = HealthPercent > fleeThreshold ? State.Enemy_Pursue : State.Self_Flee;

		switch (state) {
			case State.Self_Flee:		Self_Flee();		break;
			case State.Enemy_Pursue:	Enemy_Pursue();		break;
			default:break;
		}

	}

	private void Enemy_Pursue()
	{
		
		// update path with target position
		path[0] = Position; path[1] = target.Position;

		// maintain range of 40, avoid obstacles, avoid collisions with allies
		Vector2 accel = Vector2.zero;
		accel += GetInRange(target, 40f);
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		accel += FlowField.GetForce(this) * 3f;
		Steer(accel);

		// face target if within 50, else face heading
		if (Vector2.Distance(Position, target.Position) < 50f) { FaceTarget(); }
		else { FaceHeading(); }

		// if can fireWeapon, IsOnScreen, and TargetInSight, fire weapon
		if (fireWeapon && IsOnScreen && TargetInSight(target)) { GetComponent<Weapon>().Fire(this); }

	}

	private void Self_Flee() {
		// this combination is a wandering behavior avoiding collisions and obstacles
		Steer(Wander() + AvoidObstacles() + AvoidCollisionsAll());
		FaceHeading(); 
	}

}
