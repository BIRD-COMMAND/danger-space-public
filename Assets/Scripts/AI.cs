using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class AI : MonoBehaviour
{

	public enum Class
	{
		Assault,
		Support,
		Boss,
		Drone,
		Missile,
		Drifter,
	}

	public enum State
	{

		Self_Idle,
		Self_Drift,
		Self_Patrol,
		Self_Flee,
		Self_FleeBattle,
		Self_FleeSeekAllies,

		Leader_Seek,
		Leader_SeekArrive,
		Leader_Pursue,
		Leader_PursueArrive,
		Leader_Flee,
		Leader_Attack,
		Leader_Defend,
		Leader_Repair,
		Leader_Buff,

		Formation_Seek,
		Formation_SeekArrive,
		Formation_Pursue,
		Formation_PursueArrive,
		Formation_Flee,
		Formation_Attack,
		Formation_Defend,
		Formation_Repair,
		Formation_Buff,
		Formation_Create,
		Formation_Destroy,
		Formation_Enter,
		Formation_Fly,
		Formation_AttackFrom,
		Formation_DefendFrom,
		Formation_RepairFrom,
		Formation_BuffFrom,

		Enemy_Seek,
		Enemy_SeekArrive,
		Enemy_Pursue,
		Enemy_PursueArrive,
		Enemy_Flee,
		Enemy_Attack,
		Enemy_Defend,
		Enemy_Repair,
		Enemy_Buff,

	}

	public Class _Class;
	public State _State;
	
	public Approach.Type _ApproachType;

	public bool hasTarget;
	public Transform targetTransform;
	public Rigidbody2D targetBody;
	public Vector2 targetPosition;
	protected Vector2 Target => targetTransform ? targetTransform.position : targetPosition;
	protected float DistToTarget => targetTransform ? Vector2.Distance(transform.position, targetTransform.position) : Vector2.Distance(transform.position, targetPosition);

	public float maxSpeed = 5.0f;
	public float maxForce = 5.0f;

	protected Rigidbody2D rb;
	protected List<Rigidbody2D> flock;

	protected static RaycastHit2D query;

	void Start() { rb = GetComponent<Rigidbody2D>(); }

	public void Arrive(float slowingRadius)
	{
		//Vector2 desired = Target - (Vector2)transform.position;
		//desired.Normalize();
		//if (DistToTarget < slowingRadius) { desired *= maxSpeed * (DistToTarget / slowingRadius); }
		//else { desired *= maxSpeed; }
		// Vector2 steering = desired - rb.velocity;
		//return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		if (DistToTarget < slowingRadius) { 
			rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed * (DistToTarget / slowingRadius)) - rb.velocity, maxForce)); 
		}
		else { 
			rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce)); 
		}
	}
	public Vector2 Seek()
	{
		//Vector2 desired = Target - (Vector2)transform.position;
		//desired.Normalize();
		//desired *= maxSpeed; 
		//// Vector2 steering = desired - rb.velocity;
		//return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		return Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Seek(Vector2 target)
	{
		return Vector2.ClampMagnitude(((target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Flee()
	{
		//Vector2 desired = (Vector2)transform.position - Target;
		//desired.Normalize();
		//desired *= maxSpeed;
		// Vector2 steering = desired - rb.velocity;
		// return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		return Vector2.ClampMagnitude((((Vector2)transform.position - Target).normalized * maxSpeed) - rb.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude((((Vector2)transform.position - Target).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Flee(Vector2 target)
	{		
		return Vector2.ClampMagnitude((((Vector2)transform.position - target).normalized * maxSpeed) - rb.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude((((Vector2)transform.position - Target).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Pursuit(Rigidbody2D body, float predictionTime = 1f)
	{
		//Vector2 targetFuturePosition = body.position + body.velocity * predictionTime;
		return Seek(body.position + body.velocity * predictionTime);
	}
	public Vector2 Pursuit(IEnumerable<Rigidbody2D> bodies, float predictionTime = 1f)
	{
		return Seek(
			bodies.Select(b => b.position).Average() +
			bodies.Select(b => b.velocity).Average()
			* predictionTime
		);
	}
	public Vector2 Evade(Rigidbody2D body, float predictionTime = 1f)
	{
		//Vector2 pursuerFuturePosition = body.position + body.velocity * predictionTime;
		return Flee(body.position + body.velocity * predictionTime);
	}
	public Vector2 Evade(IEnumerable<Rigidbody2D> bodies, float predictionTime = 1f)
	{
		return Flee(
			bodies.Select(b => b.position).Average() +
			bodies.Select(b => b.velocity).Average()
			* predictionTime
		);
	}
	public Vector2 ObstacleAvoidance()
	{
		query = Physics2D.CircleCast(transform.position, 5f, transform.up, 20f);
		if (query) {
			Vector2 avoidanceForce = query.point - (Vector2)transform.position;
			avoidanceForce = avoidanceForce.normalized * (1 - query.distance / 20f) * 5f;
			return avoidanceForce;
		}
		else { return Vector2.zero; }
	}

	[Header("Flocking Parameters")]
	public float separationWeight = 1.5f;
	public float alignmentWeight = 1.0f;
	public float cohesionWeight = 1.0f;
	public float separationRadius = 3.0f;

	public Vector2 Flock()
	{

		if (flock == null || flock.Count == 0) { return Vector2.zero; }

		Vector2 separationForce = Vector2.zero;
		Vector2 alignmentForce = Vector2.zero;
		Vector2 cohesionForce = Vector2.zero;

		int separationCount = 0;
		int alignmentCount = 0;
		int cohesionCount = 0;

		foreach (Rigidbody2D other in flock) {
			
			if (other == GetComponent<Rigidbody2D>()) { continue; }

			float distance = Vector2.Distance(transform.position, other.transform.position);

			// Separation
			if (distance < separationRadius) {
				separationForce += (Vector2)(transform.position - other.transform.position);
				separationCount++;
			}

			// Alignment
			alignmentForce += other.velocity;
			alignmentCount++;

			// Cohesion
			cohesionForce += (Vector2)other.transform.position;
			cohesionCount++;
		}

		if (separationCount > 0) {
			separationForce /= separationCount;
			separationForce = separationForce.normalized * separationWeight;
		}

		if (alignmentCount > 0) {
			alignmentForce /= alignmentCount;
			alignmentForce = alignmentForce.normalized * alignmentWeight;
		}

		if (cohesionCount > 0) {
			cohesionForce /= cohesionCount;
			cohesionForce = (cohesionForce - (Vector2)transform.position).normalized * cohesionWeight;
		}

		Vector2 flockingForce = separationForce + alignmentForce + cohesionForce;
		return flockingForce;

	}

}
