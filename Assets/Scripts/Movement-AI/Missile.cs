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
			case State.Self_Idle:										break;
			case State.Self_Drift:										break;
			case State.Self_Patrol:										break;
			case State.Self_Flee:										break;
			case State.Self_FleeBattle:									break;
			case State.Self_FleeSeekAllies:								break;
			case State.Leader_Seek:										break;
			case State.Leader_SeekArrive:								break;
			case State.Leader_Pursue:									break;
			case State.Leader_PursueArrive:								break;
			case State.Leader_Flee:										break;
			case State.Leader_Attack:									break;
			case State.Leader_Defend:									break;
			case State.Leader_Repair:									break;
			case State.Leader_Buff:										break;
			case State.Formation_Seek:									break;
			case State.Formation_SeekArrive:							break;
			case State.Formation_Pursue:								break;
			case State.Formation_PursueArrive:							break;
			case State.Formation_Flee:									break;
			case State.Formation_Attack:								break;
			case State.Formation_Defend:								break;
			case State.Formation_Repair:								break;
			case State.Formation_Buff:									break;
			case State.Formation_Create:								break;
			case State.Formation_Destroy:								break;
			case State.Formation_Enter:									break;
			case State.Formation_Fly:									break;
			case State.Formation_AttackFrom:							break;
			case State.Formation_DefendFrom:							break;
			case State.Formation_RepairFrom:							break;
			case State.Formation_BuffFrom:								break;
			case State.Enemy_Seek:										break;
			case State.Enemy_SeekArrive:								break;
			case State.Enemy_Pursue:					Enemy_Pursue();	break;
			case State.Enemy_PursueArrive:								break;
			case State.Enemy_Flee:										break;
			case State.Enemy_Attack:									break;
			case State.Enemy_Defend:									break;
			case State.Enemy_Repair:									break;
			case State.Enemy_Buff:										break;
			case State.Zone_ApproachRally:								break;
			case State.Zone_ApproachFallback:							break;
			case State.Zone_Approach1:									break;
			case State.Zone_Approach2:									break;
			case State.Zone_Approach3:									break;
			case State.Zone_Approach4:									break;
			default: break;
		}
	}

	private void Enemy_Pursue()
	{

		target = PlayerController.player;
		if (!target) { return; }

		Vector3 accel = Vector3.zero;
		accel += Pursue(target);
		accel += SeparationAll();
		accel += AvoidObstacles();
		accel += AvoidCollisionsWithAllies();
		Steer(accel);

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.ApproximateForce().magnitude < 1f) { return; }
		if (collision.transform.FindComponent(out PlayerController player)) { player.Damage(collisionDamageToPlayer); }
		OnWillBeDestroyed();
		Destroy(gameObject);
	}

}
