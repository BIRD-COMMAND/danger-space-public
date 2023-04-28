using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EnemyAssault : Agent
{

	/// <summary>
	/// Agent will flee when health percentage is below this value
	/// </summary>
	[Header("Enemy Assault"), Range(0f, 1f)]
	public float fleeThreshold = 0.5f;

	private void FixedUpdate()
	{

		if ((state == State.Self_Idle || state == State.Self_Flee) && HealthPercent > fleeThreshold) {
			path.Clear(); state = State.Enemy_Pursue;
		}
		
		//EvaluateState:
		switch (state) {
			case State.Self_Idle:									Self_Idle();	break;
			case State.Self_Drift:													break;
			case State.Self_Patrol:													break;
			case State.Self_Flee:									Self_Flee();	break;
			case State.Self_FleeBattle:												break;
			case State.Self_FleeSeekAllies:											break;
			case State.Leader_Seek:													break;
			case State.Leader_SeekArrive:											break;
			case State.Leader_Pursue:												break;
			case State.Leader_PursueArrive:											break;
			case State.Leader_Flee:													break;
			case State.Leader_Attack:												break;
			case State.Leader_Defend:												break;
			case State.Leader_Repair:												break;
			case State.Leader_Buff:													break;
			case State.Formation_Seek:												break;
			case State.Formation_SeekArrive:										break;
			case State.Formation_Pursue:											break;
			case State.Formation_PursueArrive:										break;
			case State.Formation_Flee:												break;
			case State.Formation_Attack:											break;
			case State.Formation_Defend:											break;
			case State.Formation_Repair:											break;
			case State.Formation_Buff:												break;
			case State.Formation_Create:											break;
			case State.Formation_Destroy:											break;
			case State.Formation_Enter:												break;
			case State.Formation_Fly:												break;
			case State.Formation_AttackFrom:										break;
			case State.Formation_DefendFrom:										break;
			case State.Formation_RepairFrom:										break;
			case State.Formation_BuffFrom:											break;
			case State.Enemy_Seek:													break;
			case State.Enemy_SeekArrive:											break;
			case State.Enemy_Pursue:								Enemy_Pursue(); break;
			case State.Enemy_PursueArrive:											break;
			case State.Enemy_Flee:													break;
			case State.Enemy_Attack:												break;
			case State.Enemy_Defend:												break;
			case State.Enemy_Repair:												break;
			case State.Enemy_Buff:													break;
			case State.Zone_ApproachRally:											break;
			case State.Zone_ApproachFallback:										break;
			case State.Zone_Approach1:												break;
			case State.Zone_Approach2:												break;
			case State.Zone_Approach3:												break;
			case State.Zone_Approach4:												break;
			default:break;
		}
	}

	private void Enemy_Pursue()
	{
		
		if (HealthPercent < fleeThreshold) { path.Clear(); state = State.Self_Flee; Self_Flee(); return; }

		target = GameManager.Player;
		if (!target) { state = State.Self_Idle; return; }

		Vector2 accel = Vector2.zero;
		if (path.Empty) { path = new LinePath(transform.position, target.Position); }
		else { path[1] = target.Position; }
		accel += GetInRange(target, 40f); // maintain range of 40
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		Steer(accel);		
		if (Vector2.Distance(Position, target.Position) > 50f) { FaceHeading(); }
		else {  FaceTarget(); } // face target if within 50

		if (IsOnScreen && TargetInSight(target)) { GetComponent<Weapon>().Fire(this); }

	}

	private void Self_Flee()
	{
		if (path.Empty) { path = new LinePath(transform.position, ZoneManager.Fallback.RandomPointInZone()); }
		if (PathingComplete) { path.Clear(); state = State.Self_Idle; Self_Idle(); return; }
		Steer(Arrive(path[1])); FaceHeading();
	}

	private void Self_Idle() {
		Steer(Wander2()); FaceHeading();
	}

}
