using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using Shapes;

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


		Zone_ApproachRally,
		Zone_ApproachFallback,
		Zone_Approach1,
		Zone_Approach2,
		Zone_Approach3,
		Zone_Approach4,

	}

	//public Class _Class;
	public State _State;
	protected bool Idle => _State == State.Self_Idle;
	protected bool NotIdle => _State != State.Self_Idle;

	public List<Zone> patrol = new List<Zone>();
	private int patrolIndex = 0;
	public int health = 30;
	public bool pathing;
	public bool visualizePath;
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

	void Start() { 
		body = GetComponent<Rigidbody2D>(); 
		StartCoroutine(ManageVelocity());
		PathSetNextPosition(true);
	}

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
			else { 
				if (patrol.Count > 0) {
					if (patrolIndex >= patrol.Count) { patrolIndex = 0; }
					targetPosition = patrol[patrolIndex].RandomPointInZone();
					patrolIndex++;
					GeneratePath();
					if (path.Count > 0) {
						pathing = true; _State = State.Self_Patrol;
						pathTargetPosition = path[0]; path.RemoveAt(0); 
					}
				}
				else { pathing = false; _State = State.Self_Idle; }
			}
		}
	}

	public enum Pattern
	{
		None,
		Sine,
		Spiral,
		Orbit,
	}
	public Pattern pattern;
	public float radius = 16f;
	public float sineWaveAmplitude = 1.0f;
	public float sineWaveFrequency = 1.0f;
	private void GeneratePath()
	{
		switch (pattern) {
			default: case Pattern.None: break;
			case Pattern.Sine:
				path.Clear();
				Vector2 start = transform.position, end = targetPosition;
				float distance = Vector2.Distance(start, end);
				int numberOfPoints = Mathf.CeilToInt(distance / 0.5f);
				Vector2 direction = (end - start).normalized;
				float angle = Mathf.Atan2(direction.y, direction.x), t, yOffset;
				int numberOfCycles = Mathf.FloorToInt(distance / (Mathf.PI * sineWaveAmplitude * 2f));
				for (int i = 0; i <= numberOfPoints; i++) {
					t = (float)i / numberOfPoints;
					yOffset = sineWaveAmplitude * Mathf.Sin(t * Mathf.PI * numberOfCycles * 2f);
					path.Add(Vector2.Lerp(start, end, t) + new Vector2(yOffset * Mathf.Cos(angle - Mathf.PI / 2), yOffset * Mathf.Sin(angle - Mathf.PI / 2)));
				}
				break;
			case Pattern.Spiral: break;
			case Pattern.Orbit:
				numberOfPoints = Mathf.Max(1, Mathf.CeilToInt(2 * Mathf.PI * radius / 0.5f));
				for (int i = 0; i < numberOfPoints; i++) {
					angle = (float)i / numberOfPoints * 2 * Mathf.PI;
					path.Add(targetPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
				}
				break;
		}

	}

	public void Damage(int damage)
	{
		health -= damage;
		if (health <= 0) { Destroy(gameObject); }
		foreach (ShapeRenderer shape in GetComponentsInChildren<ShapeRenderer>()) { shape.FlashColor(Color.red); } 
	}

	private void OnDrawGizmos()
	{
		//Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
		//Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
		//Shapes.Draw.Line(transform.position, Target);
		//Shapes.Draw.UseDashes = false;
		if (!visualizePath) { return; }
		for (int i = 1; i < path.Count; i++) {
			Debug.DrawLine(path[i - 1], path[i], Color.HSVToRGB((float)i / path.Count, 1f, 1f));
		}
	}

}