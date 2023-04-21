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

	//public Class _Class;
	public State _State;
	protected bool Idle => _State == State.Self_Idle;
	protected bool NotIdle => _State != State.Self_Idle;


	public Approach.Type _ApproachType;

	public int health = 30;
	public bool pathing;
	public bool useTargetPosition;
	public Transform targetTransform;
	public Rigidbody2D targetBody;
	public Vector2 targetPosition;
	public Vector2 pathTargetPosition;
	public List<Vector2> path = new List<Vector2>();
	protected Vector2 Target => pathing ? pathTargetPosition : (useTargetPosition ? targetPosition : (targetTransform ? targetTransform.position : targetPosition));
	protected float DistToTarget => pathing ? Vector2.Distance(transform.position, pathTargetPosition) : (useTargetPosition ? Vector2.Distance(transform.position, targetPosition) : (targetTransform ? Vector2.Distance(transform.position, targetTransform.position) : Vector2.Distance(transform.position, targetPosition)));
	protected Vector2 DirToTarget => pathing ? (pathTargetPosition - (Vector2)transform.position).normalized : (useTargetPosition ? (targetPosition - (Vector2)transform.position).normalized : (targetTransform ? (targetTransform.position - transform.position).normalized : (targetPosition - (Vector2)transform.position).normalized));
	protected Vector2 ToTarget => pathing ? pathTargetPosition - (Vector2)transform.position : (useTargetPosition ? targetPosition - (Vector2)transform.position : (targetTransform ? targetTransform.position - transform.position : targetPosition - (Vector2)transform.position));
	protected Vector2 Heading => body ? body.velocity.normalized : Vector2.zero;

	public float maxSpeed = 5.0f;
	public float maxForce = 5.0f;
	public float turnFactor = 0.1f;
	public float arrivalRadius = 10f;

	protected Rigidbody2D body;
	protected List<Rigidbody2D> flock;

	protected static RaycastHit2D query;

	void Start() { body = GetComponent<Rigidbody2D>(); StartCoroutine(ManageVelocity()); }

	private static float velocityMagnitude;
	private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
	private IEnumerator ManageVelocity()
	{
		while (true) { 
			yield return waitForFixedUpdate; if (!body) { break; }
			// Calculate speed
			velocityMagnitude = body.velocity.magnitude;
			// Process pathing queue
			PathSetNextPosition();
			if (NotIdle) {
				if (velocityMagnitude > 0f) {
					// Accelerate
					body.AddForce(Heading * maxForce);
					// Enforce maxSpeed
					if (velocityMagnitude > maxSpeed) { 
						body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed); 
						velocityMagnitude = body.velocity.magnitude; 
					}
					// Lerp velocity toward target
					body.velocity = Vector2.Lerp(body.velocity, DirToTarget * velocityMagnitude, turnFactor);
				}
				else { body.AddForce(DirToTarget * maxForce); }
			}
		}
	}

	public void PathSetNextPosition(bool forceSet = false) {		
		if (forceSet || (pathing && DistToTarget < arrivalRadius)) {
			if (path.Count > 0) { pathTargetPosition = path[0]; path.RemoveAt(0); }
			else { pathing = false; _State = State.Self_Idle; }
		}
	}

	public void Damage(int damage)
	{
		health -= damage;
		if (health <= 0) { Destroy(gameObject); }
	}

	public void State_Self_Idle() { _State = State.Self_Idle; }
	public void State_Self_Patrol() { _State = State.Self_Patrol; }

	public Vector2 Arrive(float slowingRadius)
	{
		//Vector2 desired = transform.To(Target).normalized;
		//if (DistToTarget < slowingRadius) { desired *= maxSpeed * (DistToTarget / slowingRadius); }
		//else { desired *= maxSpeed; }
		// Vector2 steering = desired - rb.velocity;
		//return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		if (DistToTarget < slowingRadius) { 
			return Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed * (DistToTarget / slowingRadius)) - body.velocity, maxForce);
		}
		else { 
			return Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - body.velocity, maxForce);
		}
	}
	public Vector2 Seek()
	{
		//Vector2 desired = Target - (Vector2)transform.position;
		//desired.Normalize();
		//desired *= maxSpeed; 
		//// Vector2 steering = desired - rb.velocity;
		//return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		return Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - body.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Seek(Vector2 target)
	{
		return Vector2.ClampMagnitude(((target - (Vector2)transform.position).normalized * maxSpeed) - body.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude(((Target - (Vector2)transform.position).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Flee()
	{
		//Vector2 desired = (Vector2)transform.position - Target;
		//desired.Normalize();
		//desired *= maxSpeed;
		// Vector2 steering = desired - rb.velocity;
		// return Vector2.ClampMagnitude(desired - rb.velocity, maxForce);
		return Vector2.ClampMagnitude((((Vector2)transform.position - Target).normalized * maxSpeed) - body.velocity, maxForce);
		//rb.AddForce(Vector2.ClampMagnitude((((Vector2)transform.position - Target).normalized * maxSpeed) - rb.velocity, maxForce));
	}
	public Vector2 Flee(Vector2 target)
	{		
		return Vector2.ClampMagnitude((((Vector2)transform.position - target).normalized * maxSpeed) - body.velocity, maxForce);
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

		Vector2 separationForce = Vector2.zero, alignmentForce = Vector2.zero, cohesionForce = Vector2.zero;
		int separationCount = 0, alignmentCount = 0, cohesionCount = 0;

		foreach (Rigidbody2D other in flock) {
			
			if (other == body) { continue; }
			float distance = Vector2.Distance(transform.position, other.transform.position);
			// Separation
			if (distance < separationRadius) { separationForce += (Vector2)(transform.position - other.transform.position); separationCount++; }
			// Alignment
			alignmentForce += other.velocity;						alignmentCount++;
			// Cohesion
			cohesionForce += (Vector2)other.transform.position;		cohesionCount++;
		}

		if (separationCount > 0) { separationForce /= separationCount; separationForce = separationForce.normalized * separationWeight; }
		if (alignmentCount > 0) { alignmentForce /= alignmentCount; alignmentForce = alignmentForce.normalized * alignmentWeight; }
		if (cohesionCount > 0) { cohesionForce /= cohesionCount; cohesionForce = (cohesionForce - (Vector2)transform.position).normalized * cohesionWeight; }

		return separationForce + alignmentForce + cohesionForce;
	
	}

}
