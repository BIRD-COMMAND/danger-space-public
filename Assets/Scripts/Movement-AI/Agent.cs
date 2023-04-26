using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using Shapes;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Agent : MonoBehaviour
{

	// adapted from https://github.com/sturdyspoon/unity-movement-ai/	
	// more info on steering behaviors at https://github.com/libgdx/gdx-ai/wiki/Steering-Behaviors#individual-behaviors

	#region States

	public enum AgentType
	{
		Player,
		EnemyAssault,
		EnemyDrone,
		EnemyMiner,
		NeutralAssault,
		NeutralDrone,
		NeutralMiner,
		AllyAssault,
		AllyDrone,
		AllyMiner,
		Asteroid,
		Hazard,
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

	#endregion

	public bool IsFriendly(Agent other)
	{
		switch (role) {
			case AgentType.Player:			return other.role == AgentType.AllyDrone || other.role == AgentType.AllyMiner || other.role == AgentType.AllyAssault;
			case AgentType.EnemyAssault:	return other.role == AgentType.EnemyDrone || other.role == AgentType.EnemyMiner || other.role == AgentType.EnemyAssault;
			case AgentType.EnemyDrone:		return other.role == AgentType.EnemyDrone || other.role == AgentType.EnemyMiner || other.role == AgentType.EnemyAssault;
			case AgentType.EnemyMiner:		return other.role == AgentType.EnemyDrone || other.role == AgentType.EnemyMiner || other.role == AgentType.EnemyAssault;
			case AgentType.AllyAssault:		return other.role == AgentType.AllyDrone || other.role == AgentType.AllyMiner || other.role == AgentType.AllyAssault || other.role == AgentType.Player;
			case AgentType.AllyDrone:		return other.role == AgentType.AllyDrone || other.role == AgentType.AllyMiner || other.role == AgentType.AllyAssault || other.role == AgentType.Player;
			case AgentType.AllyMiner:		return other.role == AgentType.AllyDrone || other.role == AgentType.AllyMiner || other.role == AgentType.AllyAssault || other.role == AgentType.Player;
			case AgentType.Asteroid:		return false;
			case AgentType.Hazard:			return false;
			default: return false;
		}		
	}

	public AgentType role;
	public State state;

	public Agent target;

	public float maxHealth = 10f;
	public float health = 10f;
	public float HealthPercent => health / maxHealth;
	public bool invulnerable = false;

	public GameObject deathEffectPrefab;

	[HideInInspector] public List<ShapeRenderer> shapes;

	public LinePath path;

	protected static readonly LayerMask defaultExcludingProjectiles;
	static Agent() { defaultExcludingProjectiles = Physics2D.DefaultRaycastLayers & ~(1 << 8); }

	protected virtual void Awake() { 
		if (!bounds) { 
			Debug.LogError(
				$"Destroying Invalid Agent '{gameObject.name}': no Bounds assigned. The Bounds should be a trigger Collider2D enclosing the ship.",
				gameObject
			);
			Destroy(this); return;
		}
		SetUp(); 
		shapes = transform.FindComponents<ShapeRenderer>();
		//Create a vector to a target position on the wander circle
		float theta = Random.value * 2 * Mathf.PI;
		wander2Target = new Vector3(wander2Radius * Mathf.Cos(theta), wander2Radius * Mathf.Sin(theta), 0f);
	}

	public virtual void OnTookDamage(float damage) {
		if (health > 0) { FlashColor(Color.red); }
	}
	public virtual void OnWillBeDestroyed() {
		if (deathEffectPrefab) { PoolManager.Get(deathEffectPrefab).Activate(transform.position, transform.rotation); }
	}

	public void Damage(float damage)
	{
		if (invulnerable == false) {
			health -= damage;
			OnTookDamage(damage);
		}
		if (health <= 0) {
			OnWillBeDestroyed();
			Destroy(gameObject);
		}
	}

	public void FlashColor(Color color, float duration = 0.5f) {
		foreach (ShapeRenderer shape in shapes) { shape.FlashColor(color, duration); }
	}

	#region Body

	public Collider2D bounds;
    protected Rigidbody2D body;

	/// <summary>
	/// The radius for the current game object. If the game object does not have a circle collider this will return -1.
	/// </summary>
	public float Radius => radius;
	[SerializeField] private float radius = 1f;

	/// <summary>
	/// Sets up the MovementAIRigidbody so it knows about its underlying collider and rigidbody.
	/// </summary>
	public void SetUp() { body = GetComponent<Rigidbody2D>(); }

    void Start() { StartCoroutine(DebugDraw()); }
    IEnumerator DebugDraw()
    {
        yield return new WaitForFixedUpdate();

        Vector3 origin = ColliderPosition;
        Debug.DrawLine(origin, origin + (Velocity.normalized), Color.red, 0f, false);

        //SteeringBasics.debugCross(colliderPosition, 0.5f, Color.red, 0, false);
        //Debug.Log(rb3D.velocity.magnitude);
        //Debug.Log(rb3D.velocity.y + " " + movementNormal.ToString("f4") + " " + wallNormal.ToString("f4") + " " + count);
        //Debug.Log("--------------------------------------------------------------------------------");

        StartCoroutine(DebugDraw());
    }

	// Rigidbody2D Properties

	/// <summary>
	/// The position that should be used for most movement AI code.
	/// </summary>
	public Vector3 Position => body.position;

	/// <summary>
	/// The rotation for this rigidbody.
	/// </summary>
	public float Rotation { get => body.rotation - 90f; set => body.MoveRotation(value); }

	/// <summary>
	/// The velocity that should be used for movement AI code.
    /// </summary>
	public Vector3 Velocity { get => body.velocity; set => body.velocity = value; }

	/// <summary>
	/// The angularVelocity for the rigidbody.
	/// </summary>
	public float AngularVelocity { get => body.angularVelocity; set => body.angularVelocity = value; }

	/// <summary>
	/// The rotation for this rigidbody in radians.
	/// </summary>
	public float RotationInRadians => (body.rotation - 90f) * Mathf.Deg2Rad;
	/// <summary>
	/// The rotation for this rigidbody as a Vector.
	/// </summary>
	public Vector3 RotationAsVector => OrientationToVector(RotationInRadians);

	/// <summary>
	/// Gets the position of the collider (which can be offset from the transform position).
	/// </summary>
	public Vector3 ColliderPosition => transform.TransformPoint(bounds.offset);
	
	/// <summary>
	/// Returns the vector with the Z component zeroed out.
	/// </summary>
	public Vector3 ConvertVector(Vector3 v) { v.z = 0; return v; }

	#endregion


	#region Movement

	// adapted from https://github.com/sturdyspoon/unity-movement-ai/
	// more info on steering behaviors at https://github.com/libgdx/gdx-ai/wiki/Steering-Behaviors#individual-behaviors

	[Header("General")]
	public float maxVelocity = 3.5f;
	public float maxAcceleration = 10f;
	public float turnSpeed = 20f;
	public float collisionDamageToPlayer = 2f;

	[Header("Arrive")]
	/// <summary>
	/// The radius from the target that means we are close enough and have arrived
	/// </summary>
	public float arriveTargetRadius = 0.005f;
	/// <summary>
	/// The radius from the target where we start to slow down
	/// </summary>
	public float arriveSlowRadius = 1f;
	/// <summary>
	/// The time in which we want to achieve the targetSpeed
	/// </summary>
	public float arriveTimeToTarget = 0.1f;

	[Header("Look Direction Smoothing")]
	/// <summary>
	/// Smoothing controls if the character's look direction should be an
	/// average of its previous directions (to smooth out momentary changes
	/// in directions)
	/// </summary>
	public bool smoothing = true;
	public int numSamplesForSmoothing = 5;
	Queue<Vector3> rotationSamples = new Queue<Vector3>();


	/// <summary>
	/// Updates the velocity of the current game object by the given linear
	/// acceleration
	/// </summary>
	public void Steer(Vector3 linearAcceleration)
	{
		Velocity += linearAcceleration * Time.deltaTime;

		if (Velocity.magnitude > maxVelocity) {
			Velocity = Velocity.normalized * maxVelocity;
		}
	}

	#region Angle and Orientation

	/// <summary>
	/// Makes the current game object look where it is going
	/// </summary>
	public void FaceHeading()
	{
		Vector3 direction = Velocity;

		if (smoothing) {
			if (rotationSamples.Count == numSamplesForSmoothing) {
				rotationSamples.Dequeue();
			}

			rotationSamples.Enqueue(Velocity);

			direction = Vector3.zero;

			foreach (Vector3 v in rotationSamples) {
				direction += v;
			}

			direction /= rotationSamples.Count;
		}

		LookAtDirection(direction);
	}
	/// <summary>
	/// Makes the current game object look towards its target
	/// </summary>
	public void FaceTarget()
	{

		if (!target) { FaceHeading(); return; }

		Vector3 direction = target.Position - Position;

		if (smoothing) {

			if (rotationSamples.Count == numSamplesForSmoothing) {
				rotationSamples.Dequeue();
			}

			rotationSamples.Enqueue(direction);

			direction = Vector3.zero;
			foreach (Vector3 v in rotationSamples) { direction += v; }
			direction /= rotationSamples.Count;
		}

		LookAtDirection(direction);
	}
	public void LookAtDirection(Vector3 direction)
	{
		direction.Normalize();

		/* If we have a non-zero direction then look towards that direciton otherwise do nothing */
		if (direction.sqrMagnitude > 0.001f) {
			float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
			Rotation = Mathf.LerpAngle(Rotation + 90f, toRotation - 90f, Time.deltaTime * turnSpeed);
		}
	}
	public void LookAtDirection(Quaternion toRotation) { LookAtDirection(toRotation.eulerAngles.z); }
	/// <summary>
	/// Makes the character's rotation lerp closer to the given target rotation (in degrees).
	/// </summary>
	/// <param name="toRotation">the desired rotation to be looking at in degrees</param>
	public void LookAtDirection(float toRotation)
	{
		Rotation = Mathf.LerpAngle(Rotation + 90f, toRotation - 90f, Time.deltaTime * turnSpeed);
	}

	/// <summary>
	/// Checks to see if the target is in front of the character
	/// </summary>
	public bool IsInFront(Vector3 target)
	{
		return IsFacing(target, 0);
	}
	public bool IsFacing(Vector3 target, float cosineValue)
	{
		Vector3 facing = transform.up.normalized;

		Vector3 directionToTarget = (target - transform.position);
		directionToTarget.Normalize();

		return Vector3.Dot(facing, directionToTarget) >= cosineValue;
	}

	/// <summary>
	/// Returns the given orientation (in radians) as a unit vector
	/// </summary>
	/// <param name="orientation">the orientation in radians</param>
	public static Vector3 OrientationToVector(float orientation)
	{
		return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
	}

	/// <summary>
	/// Gets the orientation of a vector as radians around the Z axis.
	/// </summary>
	/// <param name="direction">the direction vector</param>
	/// <returns>orientation in radians</returns>
	public static float VectorToOrientation(Vector3 direction) { return Mathf.Atan2(direction.y, direction.x); }

	/// <summary>
	/// Returns true if the target is within view and there is an unobstructed path of width (Radius * 0.5) to the target. (Ignores projectiles)
	/// </summary>
	public bool TargetInSight(Agent target)
	{
		bool defaultQueriesStartInColliders = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.CircleCast(ColliderPosition, (Radius * 0.5f), target.Position - Position, 1000f, defaultExcludingProjectiles);
		Physics2D.queriesStartInColliders = defaultQueriesStartInColliders;
		return hit && hit.collider && hit.collider.FindComponent(out Agent agent) && agent == target;
	}

	public bool IsOnScreen => ScreenTrigger.IsOnScreen(Position);

	#endregion

	#region Seek & Arrive

	/// <summary>
	/// A seek steering behavior. Will return the steering for the current game object to seek a given position
	/// </summary>
	public Vector3 Seek(Vector3 targetPosition, float maxSeekAccel)
	{
		/* Get the direction */
		Vector3 acceleration = ConvertVector(targetPosition - transform.position);

		acceleration.Normalize();

		/* Accelerate to the target */
		acceleration *= maxSeekAccel;

		return acceleration;
	}

	public Vector3 Seek(Vector3 targetPosition)
	{
		return Seek(targetPosition, maxAcceleration);
	}

	/// <summary>
	/// Returns the steering for a character so it arrives at the target
	/// </summary>
	public Vector3 Arrive(Vector3 targetPosition)
	{
		
		//Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

		targetPosition = ConvertVector(targetPosition);

		/* Get the right direction for the linear acceleration */
		Vector3 targetVelocity = targetPosition - Position;
		//Debug.Log("Displacement " + targetVelocity.ToString("f4"));

		/* Get the distance to the target */
		float dist = targetVelocity.magnitude;

		/* If we are within the stopping radius then stop */
		if (dist < arriveTargetRadius) {
			Velocity = Vector3.zero;
			return Vector3.zero;
		}

		/* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
		float targetSpeed;
		if (dist > arriveSlowRadius) {
			targetSpeed = maxVelocity;
		}
		else {
			targetSpeed = maxVelocity * (dist / arriveSlowRadius);
		}

		/* Give targetVelocity the correct speed */
		targetVelocity.Normalize();
		targetVelocity *= targetSpeed;

		/* Calculate the linear acceleration we want */
		Vector3 acceleration = targetVelocity - Velocity;
		/* Rather than accelerate the character to the correct speed in 1 second, 
            * accelerate so we reach the desired speed in timeToTarget seconds 
            * (if we were to actually accelerate for the full timeToTarget seconds). */
		acceleration *= 1 / arriveTimeToTarget;

		/* Make sure we are accelerating at max acceleration */
		if (acceleration.magnitude > maxAcceleration) {
			acceleration.Normalize();
			acceleration *= maxAcceleration;
		}
		//Debug.Log("Accel " + acceleration.ToString("f4"));
		return acceleration;
	}

	public bool IsArriving(Vector3 targetPosition) { return (targetPosition - Position).magnitude < arriveSlowRadius; }

	#endregion

	#region Pursuit

	[Header("Pursuit")]

	/// <summary>
	/// Maximum prediction time the pursue will predict in the future
	/// </summary>
	public float maxPredictionTime = 1f;

	/// <summary>
	/// Radius in which the pursuing agent will start to slow down
	/// </summary>
	public float pursuitSlowRadius = 10f;

	/// <summary>
	/// Predicts the targets location and calculates the steering to catch it
	/// </summary>
	public Vector3 Pursue(Agent target)
	{
		/* Calculate the distance to the target */
		Vector3 displacement = target.Position - transform.position;
		float distance = displacement.magnitude;

		/* Get the character's speed */
		float speed = Velocity.magnitude;

		/* Calculate the prediction time */
		float prediction;
		if (speed <= distance / maxPredictionTime) {
			prediction = maxPredictionTime;
		}
		else {
			prediction = distance / speed;
		}

		/* Put the target together based on where we think the target will be */
		Vector3 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget);

		return Seek(explicitTarget);
	}

	/// <summary>
	/// Predicts the targets location and calculates the steering to reach it while maintaining a safe distance
	/// </summary>
	public Vector3 GetInRange(Agent target, float range)
	{
		/* Calculate the distance to the target */
		Vector3 displacement = target.Position - transform.position;
		float distance = displacement.magnitude;

		/* Get the character's speed */
		float speed = Velocity.magnitude;

		/* Calculate the prediction time */
		float prediction;
		if (speed <= distance / maxPredictionTime) {
			prediction = maxPredictionTime;
		}
		else {
			prediction = distance / speed;
		}

		/* Put the target together based on where we think the target will be */
		Vector3 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget + ((Position - explicitTarget).normalized * range));

		return Arrive(explicitTarget + ((Position - explicitTarget).normalized * range));
	}

	public bool IsArrivingInRange(Agent target, float range) {
		Vector3 displacement = target.Position - transform.position;
		float distance = displacement.magnitude;
		float speed = Velocity.magnitude;
		float prediction = (speed <= distance / maxPredictionTime) ? maxPredictionTime : distance / speed;
		Vector3 explicitTarget = target.Position + target.Velocity * prediction;
		return (explicitTarget + ((Position - explicitTarget).normalized * range) - Position).magnitude < arriveSlowRadius;
	}

	#endregion

	#region Interpose

	/// <summary>
	/// Calculates the steering for an agent so it stays positioned between two targets
	/// </summary>
	public Vector3 Interpose(Agent target1, Agent target2)
	{
		Vector3 midPoint = (target1.Position + target2.Position) / 2;

		float timeToReachMidPoint = Vector3.Distance(midPoint, transform.position) / maxVelocity;

		Vector3 futureTarget1Pos = target1.Position + target1.Velocity * timeToReachMidPoint;
		Vector3 futureTarget2Pos = target2.Position + target2.Velocity * timeToReachMidPoint;

		midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

		return Arrive(midPoint);
	}

	#endregion

	#region Pathing

	[Header("Pathing")]

	public float stopRadius = 0.005f;

	public float pathOffset = 0.71f;

	public float pathDirection = 1f;

	public Vector3 FollowPath(LinePath path)
	{
		return FollowPath(path, false);
	}

	public Vector3 FollowPath(LinePath path, bool pathLoop)
	{
		Vector3 targetPosition;
		return FollowPath(path, pathLoop, out targetPosition);
	}

	public Vector3 FollowPath(LinePath path, bool pathLoop, out Vector3 targetPosition)
	{

		/* If the path has only one node then just go to that position. */
		if (path.Length == 1) { targetPosition = path[0]; }
		/* Else find the closest spot on the path to the character and go to that instead. */
		else {
			/* Get the param for the closest position point on the path given the character's position */
			float param = path.GetParam(transform.position, this);

			//Debug.DrawLine(transform.position, path.getPosition(param, pathLoop), Color.red, 0, false);

			if (!pathLoop) {
				Vector3 finalDestination;

				/* If we are close enough to the final destination then stop moving */
				if (IsAtEndOfPath(path, param, out finalDestination)) {
					targetPosition = finalDestination;

					Velocity = Vector3.zero;
					return Vector3.zero;
				}
			}

			/* Move down the path */
			param += pathDirection * pathOffset;

			/* Set the target position */
			targetPosition = path.GetPosition(param, pathLoop);

			//Debug.DrawLine(transform.position, targetPosition, Color.red, 0, false);
		}

		return Arrive(targetPosition);
	}

	/// <summary> 
	/// Will return true if the character is at the end of the given path 
	/// </summary>
	public bool PathingComplete { 
		get {
			/* If the path has only one node then just check the distance to that node. */
			if (path.Length == 1) {
				Vector3 endPos = ConvertVector(path[0]);
				return Vector3.Distance(Position, endPos) < stopRadius;
			}
			/* Else see if the character is at the end of the path. */
			else {
				Vector3 finalDestination;

				/* Get the param for the closest position point on the path given the character's position */
				float param = path.GetParam(transform.position, this);

				return IsAtEndOfPath(path, param, out finalDestination);
			}
		}
	}

	bool IsAtEndOfPath(LinePath path, float param, out Vector3 finalDestination)
	{
		bool result;

		/* Find the final destination of the character on this path */
		finalDestination = (pathDirection > 0) ? path.Last : path.First;
		finalDestination = ConvertVector(finalDestination);

		/* If the param is closest to the last segment then check if we are at the final destination */
		if (param >= path.distances[path.Length - 2]) {
			result = Vector3.Distance(Position, finalDestination) < stopRadius;
		}
		/* Else we are not at the end of the path */
		else { result = false; }

		return result;
	}

	#endregion

	#region Obstacle Avoidance

	[Header("Obstacle Avoidance")]

	/// <summary>
	/// The distance away from the collision that we wish go
	/// </summary>
	public float obstacleAvoidDistance = 0.5f;

	/// <summary>
	/// Multiplier applied to the Seek vector produced by ObstacleAvoidance.
	/// </summary>
	public float obstacleAvoidanceMultiplier = 4f;

	/// <summary>
	/// How far ahead the ray should extend
	/// </summary>
	public float mainWhiskerLen = 1.25f;

	public float sideWhiskerLen = 0.701f;

	public float sideWhiskerAngle = 45f;

	public enum ObstacleDetection { Raycast, Spherecast }
	public ObstacleDetection obstacleDetection = ObstacleDetection.Spherecast;

	public LayerMask castMask = Physics.DefaultRaycastLayers;

	public Vector3 AvoidObstacles()
	{
		if (Velocity.magnitude > 0.005f) {
			return AvoidObstacles(Velocity);
		}
		else {
			return AvoidObstacles(RotationAsVector);
		}
	}

	public Vector3 AvoidObstacles(Vector3 facingDir)
	{
		Vector3 acceleration = Vector3.zero;

		RaycastHit2D hit;

		/* If no collision do nothing */
		if (!FindObstacle(facingDir, out hit)) {
			return acceleration;
		}

		/* Create a target away from the wall to seek */
		Vector3 targetPostition = hit.point + hit.normal * obstacleAvoidDistance;

		/* If velocity and the collision normal are parallel then move the target a bit to
            * the left or right of the normal */
		float angle = Vector3.Angle(Velocity, hit.normal);
		if (angle > 165f) {
			Vector3 perp = new Vector3(-hit.normal.y, hit.normal.x, 0f);
			/* Add some perp displacement to the target position propotional to the angle between the wall normal
                * and facing dir and propotional to the wall avoidance distance (with 2f being a magic constant that
                * feels good) */
			targetPostition = targetPostition + (perp * Mathf.Sin((angle - 165f) * Mathf.Deg2Rad) * 2f * obstacleAvoidDistance);
		}

		//SteeringBasics.debugCross(targetPostition, 0.5f, new Color(0.612f, 0.153f, 0.69f), 0.5f, false);

		return Seek(targetPostition, maxAcceleration * obstacleAvoidanceMultiplier);
	}

	bool FindObstacle(Vector3 facingDir, out RaycastHit2D firstHit)
	{
		facingDir = ConvertVector(facingDir).normalized;

		/* Create the direction vectors */
		Vector3[] dirs = new Vector3[3];
		dirs[0] = facingDir;

		float orientation = VectorToOrientation(facingDir);

		dirs[1] = OrientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
		dirs[2] = OrientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

		return CastWhiskers(dirs, out firstHit);
	}

	bool CastWhiskers(Vector3[] dirs, out RaycastHit2D firstHit)
	{
		firstHit = new RaycastHit2D();
		bool foundObs = false;

		for (int i = 0; i < dirs.Length; i++) {
			float dist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

			RaycastHit2D hit;

			if (GenericCast(dirs[i], out hit, dist)) {
				foundObs = true;
				firstHit = hit;
				break;
			}
		}

		return foundObs;
	}

	bool GenericCast(Vector3 direction, out RaycastHit2D hit, float distance = Mathf.Infinity)
	{
		bool defaultQueriesStartInColliders = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;

		hit = Physics2D.CircleCast(ColliderPosition, (Radius * 0.5f), direction, distance, castMask.value);

		Physics2D.queriesStartInColliders = defaultQueriesStartInColliders;

		//Debug.DrawLine(ColliderPosition, ColliderPosition + direction * distance, Color.cyan, 0f, false);

		return hit.collider != null && !hit.collider.FindComponent<Agent>();
	}

	#endregion

	#region Collision Avoidance

	[Header("Collision Avoidance")]

	[Tooltip("How much space can be between two characters before they are considered colliding")]
	/// <summary>
	/// How much space can be between two characters before they are considered colliding
	/// </summary>
	public float distanceBetween;

	[Tooltip("Multiplier applied to collision detection radius")]
	/// <summary>
	/// Multiplier applied to collision detection radius.
	/// </summary>
	public float collisionDetectionRadiusMultiplier = 4f;

	[Tooltip("Multiplier applied to the Seek vector produced by ObstacleAvoidance.")]
	/// <summary>
	/// Multiplier applied to the Seek vector produced by ObstacleAvoidance.
	/// </summary>
	public float collisionAvoidanceMultiplier = 1.5f;

	public Vector3 AvoidCollisionsWithAllies() { return AvoidCollisionsWithAllies(Radius); }
	public Vector3 AvoidCollisionsWithAllies(float radius)	{
		return AvoidCollisions(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a))); 
	}

	public Vector3 AvoidCollisionsAll() { return AvoidCollisionsAll(Radius); }
	public Vector3 AvoidCollisionsAll(float radius) { return AvoidCollisions(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier)); }

	public Vector3 AvoidCollisions(IEnumerable<Agent> targets)
	{
		Vector3 acceleration = Vector3.zero;

		/* 1. Find the target that the character will collide with first */

		/* The first collision time */
		float shortestTime = float.PositiveInfinity;

		/* The first target that will collide and other data that
            * we will need and can avoid recalculating */
		Agent firstTarget = null;
		float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
		Vector3 firstRelativePos = Vector3.zero, firstRelativeVel = Vector3.zero;

		foreach (Agent agent in targets) {
			/* Calculate the time to collision */
			Vector3 relativePos = ColliderPosition - agent.ColliderPosition;
			Vector3 relativeVel = Velocity - agent.Velocity;
			float distance = relativePos.magnitude;
			float relativeSpeed = relativeVel.magnitude;

			if (relativeSpeed == 0) {
				continue;
			}

			float timeToCollision = -1 * Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

			/* Check if they will collide at all */
			Vector3 separation = relativePos + relativeVel * timeToCollision;
			float minSeparation = separation.magnitude;

			if (minSeparation > Radius + agent.Radius + distanceBetween) {
				continue;
			}

			/* Check if its the shortest */
			if (timeToCollision > 0 && timeToCollision < shortestTime) {
				shortestTime = timeToCollision;
				firstTarget = agent;
				firstMinSeparation = minSeparation;
				firstDistance = distance;
				firstRelativePos = relativePos;
				firstRelativeVel = relativeVel;
				firstRadius = agent.Radius;
			}
		}

		/* 2. Calculate the steering */

		/* If we have no target then exit */
		if (firstTarget == null) {
			return acceleration;
		}

		/* If we are going to collide with no separation or if we are already colliding then 
            * steer based on current position */
		if (firstMinSeparation <= 0 || firstDistance < Radius + firstRadius + distanceBetween) {
			acceleration = ColliderPosition - firstTarget.ColliderPosition;
		}
		/* Else calculate the future relative position */
		else {
			acceleration = firstRelativePos + firstRelativeVel * shortestTime;
		}

		/* Avoid the target */
		acceleration = ConvertVector(acceleration);
		acceleration.Normalize();
		acceleration *= maxAcceleration * collisionAvoidanceMultiplier;

		return acceleration;
	}

	public IEnumerable<Agent> GetAgentsInRadius() { return GetAgentsInRadius(Radius); }
	public IEnumerable<Agent> GetAgentsInRadius(float radius) {
		return
			Physics2D.OverlapCircleAll(ColliderPosition, radius)
			.Where(c => c.FindComponent(out Agent agent) && agent != this )
			.Select(c => c.FindComponent<Agent>()).Distinct();
	}

	#endregion

	#region Flee

	[Header("Flee")]

	public float fleePanicDist = 3.5f;

	public bool fleeDecelerateOnStop = true;

	public float fleeTimeToTarget = 0.1f;

	public Vector3 Flee(Vector3 targetPosition)
	{
		/* Get the direction */
		Vector3 acceleration = transform.position - targetPosition;

		/* If the target is far way then don't flee */
		if (acceleration.magnitude > fleePanicDist) {
			/* Slow down if we should decelerate on stop */
			if (fleeDecelerateOnStop && Velocity.magnitude > 0.001f) {
				/* Decelerate to zero velocity in time to target amount of time */
				acceleration = -Velocity / fleeTimeToTarget;

				if (acceleration.magnitude > maxAcceleration) {
					acceleration = FleeGiveMaxAccel(acceleration);
				}

				return acceleration;
			}
			else {
				Velocity = Vector3.zero;
				return Vector3.zero;
			}
		}

		return FleeGiveMaxAccel(acceleration);
	}

	Vector3 FleeGiveMaxAccel(Vector3 v)
	{
		v.Normalize();

		/* Accelerate to the target */
		v *= maxAcceleration;

		return v;
	}

	#endregion

	#region Evade

	[Header("Evade")]

	/// <summary>
	/// Maximum prediction time the pursue will predict in the future
	/// </summary>
	public float evadeMaxPredictionTime = 1f;

	public Vector3 Evade(Agent target)
	{
		/* Calculate the distance to the target */
		Vector3 displacement = target.Position - transform.position;
		float distance = displacement.magnitude;

		/* Get the targets's speed */
		float speed = target.Velocity.magnitude;

		/* Calculate the prediction time */
		float prediction;
		if (speed <= distance / evadeMaxPredictionTime) {
			prediction = evadeMaxPredictionTime;
		}
		else {
			prediction = distance / speed;
			//Place the predicted position a little before the target reaches the character
			prediction *= 0.9f;
		}

		/* Put the target together based on where we think the target will be */
		Vector3 explicitTarget = target.Position + target.Velocity * prediction;

		return Flee(explicitTarget);
	}

	#endregion

	#region Hide

	[Header("Hide")]

	public float hideDistanceFromBoundary = 0.6f;

	public Vector3 Hide(Agent target, ICollection<Agent> obstacles)
	{
		Vector3 bestHidingSpot;
		return Hide(target, obstacles, out bestHidingSpot);
	}

	public Vector3 Hide(Agent target, ICollection<Agent> obstacles, out Vector3 bestHidingSpot)
	{
		/* Find the closest hiding spot. */
		float distToClostest = Mathf.Infinity;
		bestHidingSpot = Vector3.zero;

		foreach (Agent r in obstacles) {
			Vector3 hidingSpot = GetHidingPosition(r, target);

			float dist = Vector3.Distance(hidingSpot, transform.position);

			if (dist < distToClostest) {
				distToClostest = dist;
				bestHidingSpot = hidingSpot;
			}
		}

		/* If no hiding spot is found then just evade the enemy. */
		if (distToClostest == Mathf.Infinity) {
			return Evade(target);
		}

		//Debug.DrawLine(transform.position, bestHidingSpot);

		return Arrive(bestHidingSpot);
	}

	Vector3 GetHidingPosition(Agent obstacle, Agent target)
	{
		float distAway = obstacle.Radius + hideDistanceFromBoundary;

		Vector3 dir = obstacle.Position - target.Position;
		dir.Normalize();

		return obstacle.Position + dir * distAway;
	}

	#endregion

	#region Wander1

	[Header("Wander1")]

	/// <summary>
	/// The forward offset of the wander square
	/// </summary>
	public float wanderOffset = 1.5f;

	/// <summary>
	/// The radius of the wander square
	/// </summary>
	public float wanderRadius = 4;

	/// <summary>
	/// The rate at which the wander orientation can change in radians
	/// </summary>
	public float wanderRate = 0.4f;

	float wanderOrientation = 0;

	public Vector3 Wander1()
	{
		float characterOrientation = RotationInRadians;

		/* Update the wander orientation */
		wanderOrientation += RandomBinomial() * wanderRate;

		/* Calculate the combined target orientation */
		float targetOrientation = wanderOrientation + characterOrientation;

		/* Calculate the center of the wander circle */
		Vector3 targetPosition = transform.position + (OrientationToVector(characterOrientation) * wanderOffset);

		//debugRing.transform.position = targetPosition;

		/* Calculate the target position */
		targetPosition = targetPosition + (OrientationToVector(targetOrientation) * wanderRadius);

		//Debug.DrawLine (transform.position, targetPosition);

		return Seek(targetPosition);
	}

	/* Returns a random number between -1 and 1. Values around zero are more likely. */
	float RandomBinomial() { return Random.value - Random.value; }

	#endregion

	#region Wander2

	public float wander2Radius = 1.2f;

	public float wander2Distance = 2f;

	/// <summary>
	/// Maximum amount of random displacement a second
	/// </summary>
	public float wander2Jitter = 40f;

	Vector3 wander2Target;

	public Vector3 Wander2()
	{
		/* Get the jitter for this time frame */
		float jitter = wander2Jitter * Time.deltaTime;

		/* Add a small random vector to the target's position */
		wander2Target += new Vector3(Random.Range(-1f, 1f) * jitter, Random.Range(-1f, 1f) * jitter, 0f);

		/* Make the wanderTarget fit on the wander circle again */
		wander2Target.Normalize();
		wander2Target *= wander2Radius;

		/* Move the target in front of the character */
		Vector3 targetPosition = transform.position + transform.right * wander2Distance + wander2Target;

		//Debug.DrawLine(transform.position, targetPosition);

		return Seek(targetPosition);
	}

	#endregion

	#region Separation

	/// <summary>
	/// The maximum acceleration for separation
	/// </summary>
	public float sepMaxAcceleration = 25;

	/// <summary>
	/// This should be the maximum separation distance possible between a
	/// separation target and the character. So it should be: separation
	/// sensor radius + max target radius
	/// </summary>
	public float maxSepDist = 1f;

	public Vector3 SeparationFromAllies() { return SeparationFromAllies(Radius); }
	public Vector3 SeparationFromAllies(float radius) {
		return Separation(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a)));
	}

	public Vector3 SeparationAll() { return SeparationAll(Radius * 4f); }
	public Vector3 SeparationAll(float radius) {
		return Separation(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier));
	}

	public Vector3 Separation(IEnumerable<Agent> targets)
	{
		Vector3 acceleration = Vector3.zero;

		foreach (Agent agent in targets) {
			if (!IsFriendly(agent)) { continue; }
			/* Get the direction and distance from the target */
			Vector3 direction = ColliderPosition - agent.ColliderPosition;
			float dist = direction.magnitude;

			if (dist < maxSepDist) {
				/* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
				var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - Radius - agent.Radius);

				/* Added separation acceleration to the existing steering */
				direction = ConvertVector(direction);
				direction.Normalize();
				acceleration += direction * strength;
			}
		}

		return acceleration;
	}

	#endregion

	/// <summary>
	/// Creates a debug cross at the given position in the scene view to help with debugging.
	/// </summary>
	public static void DebugCross(Vector3 position, float size = 0.5f, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		Vector3 xStart = position + Vector3.right * size * 0.5f;
		Vector3 xEnd = position - Vector3.right * size * 0.5f;

		Vector3 yStart = position + Vector3.up * size * 0.5f;
		Vector3 yEnd = position - Vector3.up * size * 0.5f;

		Vector3 zStart = position + Vector3.forward * size * 0.5f;
		Vector3 zEnd = position - Vector3.forward * size * 0.5f;

		Debug.DrawLine(xStart, xEnd, color, duration, depthTest);
		Debug.DrawLine(yStart, yEnd, color, duration, depthTest);
		Debug.DrawLine(zStart, zEnd, color, duration, depthTest);
	}

	#endregion

}

