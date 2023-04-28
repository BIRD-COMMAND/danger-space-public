using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using Shapes;

public abstract class Agent : Entity
{

	// adapted from https://github.com/sturdyspoon/unity-movement-ai/	
	// more info on steering behaviors at https://github.com/libgdx/gdx-ai/wiki/Steering-Behaviors#individual-behaviors

	#region States

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

	[Header("Agent")]

	public State state;
	
	public LinePath path;

	protected override void Awake() {
		base.Awake();
		//Create a vector to a target position on the wander circle
		float theta = Random.value * 2 * Mathf.PI;
		wander2Target = new Vector2(wander2Radius * Mathf.Cos(theta), wander2Radius * Mathf.Sin(theta));
		mainWhiskerLen = obstacleAvoidDistance * obstacleCheckDistanceMultiplier;
		sideWhiskerLen = obstacleAvoidDistance * obstacleCheckDistanceMultiplier * 0.75f;
	}	

	/// <summary>
	/// Updates the velocity of the current game object by the given linear
	/// acceleration
	/// </summary>
	public void Steer(Vector2 linearAcceleration)
	{
		Velocity += linearAcceleration * Time.deltaTime;

		if (Velocity.magnitude > maxVelocity) {
			Velocity = Velocity.normalized * maxVelocity;
		}
	}

	#region Seek & Arrive

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

	/// <summary>
	/// A seek steering behavior. Will return the steering for the current game object to seek a given position
	/// </summary>
	public Vector2 Seek(Vector2 targetPosition, float maxSeekAccel)
	{
		/* Get the direction */
		Vector2 acceleration = targetPosition - Position;

		acceleration.Normalize();

		/* Accelerate to the target */
		acceleration *= maxSeekAccel;

		return acceleration;
	}

	public Vector2 Seek(Vector2 targetPosition)
	{
		return Seek(targetPosition, maxAcceleration);
	}

	/// <summary>
	/// Returns the steering for a character so it arrives at the target
	/// </summary>
	public Vector2 Arrive(Vector2 targetPosition)
	{
		
		//Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

		/* Get the right direction for the linear acceleration */
		Vector2 targetVelocity = targetPosition - Position;
		//Debug.Log("Displacement " + targetVelocity.ToString("f4"));

		/* Get the distance to the target */
		float dist = targetVelocity.magnitude;

		/* If we are within the stopping radius then stop */
		if (dist < arriveTargetRadius) {
			Velocity = Vector2.zero;
			return Vector2.zero;
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
		Vector2 acceleration = targetVelocity - Velocity;
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

	public bool IsArriving(Vector2 targetPosition) { return (targetPosition - Position).magnitude < arriveSlowRadius; }

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
	public Vector2 Pursue(Entity target)
	{
		/* Calculate the distance to the target */
		Vector2 displacement = target.Position - Position;
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
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget);

		return Seek(explicitTarget);
	}

	/// <summary>
	/// Predicts the targets location and calculates the steering to reach it while maintaining a safe distance
	/// </summary>
	public Vector2 GetInRange(Entity target, float range)
	{
		/* Calculate the distance to the target */
		Vector2 displacement = target.Position - Position;
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
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget + ((Position - explicitTarget).normalized * range));

		return Arrive(explicitTarget + ((Position - explicitTarget).normalized * range));
	}

	public bool IsArrivingInRange(Entity target, float range) {
		Vector2 displacement = target.Position - Position;
		float distance = displacement.magnitude;
		float speed = Velocity.magnitude;
		float prediction = (speed <= distance / maxPredictionTime) ? maxPredictionTime : distance / speed;
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;
		return (explicitTarget + ((Position - explicitTarget).normalized * range) - Position).magnitude < arriveSlowRadius;
	}

	#endregion

	#region Interpose

	/// <summary>
	/// Calculates the steering for an agent so it stays positioned between two targets
	/// </summary>
	public Vector2 Interpose(Entity target1, Entity target2)
	{
		Vector2 midPoint = (target1.Position + target2.Position) / 2;

		float timeToReachMidPoint = Vector2.Distance(midPoint, transform.position) / maxVelocity;

		Vector2 futureTarget1Pos = target1.Position + target1.Velocity * timeToReachMidPoint;
		Vector2 futureTarget2Pos = target2.Position + target2.Velocity * timeToReachMidPoint;

		midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

		return Arrive(midPoint);
	}

	#endregion

	#region Pathing

	[Header("Pathing")]

	public float stopRadius = 0.005f;

	public float pathOffset = 0.71f;

	public float pathDirection = 1f;

	public Vector2 FollowPath(LinePath path)
	{
		return FollowPath(path, false);
	}

	public Vector2 FollowPath(LinePath path, bool pathLoop)
	{
		Vector2 targetPosition;
		return FollowPath(path, pathLoop, out targetPosition);
	}

	public Vector2 FollowPath(LinePath path, bool pathLoop, out Vector2 targetPosition)
	{

		/* If the path has only one node then just go to that position. */
		if (path.Length == 1) { targetPosition = path[0]; }
		/* Else find the closest spot on the path to the character and go to that instead. */
		else {
			/* Get the param for the closest position point on the path given the character's position */
			float param = path.GetParam(transform.position, this);

			//Debug.DrawLine(transform.position, path.getPosition(param, pathLoop), Color.red, 0, false);

			if (!pathLoop) {
				Vector2 finalDestination;

				/* If we are close enough to the final destination then stop moving */
				if (IsAtEndOfPath(path, param, out finalDestination)) {
					targetPosition = finalDestination;

					Velocity = Vector2.zero;
					return Vector2.zero;
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
				Vector2 endPos = path[0];
				return Vector2.Distance(Position, endPos) < stopRadius;
			}
			/* Else see if the character is at the end of the path. */
			else {
				Vector2 finalDestination;

				/* Get the param for the closest position point on the path given the character's position */
				float param = path.GetParam(transform.position, this);

				return IsAtEndOfPath(path, param, out finalDestination);
			}
		}
	}

	bool IsAtEndOfPath(LinePath path, float param, out Vector2 finalDestination)
	{
		bool result;

		/* Find the final destination of the character on this path */
		finalDestination = (pathDirection > 0) ? path.Last : path.First;

		/* If the param is closest to the last segment then check if we are at the final destination */
		if (param >= path.distances[path.Length - 2]) {
			result = Vector2.Distance(Position, finalDestination) < stopRadius;
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
	/// Multiplier applied to the distance of the ray cast for obstacle avoidance.
	/// </summary>
	public float obstacleCheckDistanceMultiplier = 1f;

	/// <summary>
	/// Draws debug visualizations for the obstacle avoidance algorithm when set to true.
	/// </summary>
	public bool debugObstacleAvoidance = false;

	/// <summary>
	/// How far ahead the ray should extend
	/// </summary>
	[HideInInspector] public float mainWhiskerLen = 1.25f;
	[HideInInspector] public float sideWhiskerLen = 0.701f;
	[HideInInspector] public float sideWhiskerAngle = 45f;

	public Vector2 AvoidObstacles()
	{
		if (Velocity.magnitude > 0.005f) {
			return AvoidObstacles(Velocity);
		}
		else {
			return AvoidObstacles(RotationAsVector);
		}
	}

	public Vector2 AvoidObstacles(Vector2 facingDir)
	{
		Vector2 acceleration = Vector2.zero;

		RaycastHit2D hit;

		/* If no collision do nothing */
		if (!FindObstacle(facingDir, out hit)) {
			return acceleration;
		}

		/* Create a target away from the wall to seek */
		Vector2 targetPostition = hit.point + hit.normal * obstacleAvoidDistance;

		/* If velocity and the collision normal are parallel then move the target a bit to
            * the left or right of the normal */
		float angle = Vector2.Angle(Velocity, hit.normal);
		if (angle > 165f) {
			Vector2 perp = new Vector2(-hit.normal.y, hit.normal.x);
			/* Add some perp displacement to the target position propotional to the angle between the wall normal
                * and facing dir and propotional to the wall avoidance distance (with 2f being a magic constant that
                * feels good) */
			targetPostition = targetPostition + (perp * Mathf.Sin((angle - 165f) * Mathf.Deg2Rad) * 2f * obstacleAvoidDistance);
		}

		if (debugObstacleAvoidance) {
			DebugCross(targetPostition, 0.5f, new Color(0.612f, 0.153f, 0.69f), 0.5f, false);
		}

		return Seek(targetPostition, maxAcceleration * obstacleAvoidanceMultiplier);
	}

	bool FindObstacle(Vector2 facingDir, out RaycastHit2D firstHit)
	{
		facingDir.Normalize();

		/* Create the direction vectors */
		Vector2[] dirs = new Vector2[3];
		dirs[0] = facingDir;

		float orientation = VectorToOrientation(facingDir);

		dirs[1] = OrientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
		dirs[2] = OrientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

		return CastWhiskers(dirs, out firstHit);
	}

	bool CastWhiskers(Vector2[] dirs, out RaycastHit2D firstHit)
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

	bool GenericCast(Vector2 direction, out RaycastHit2D hit, float distance = Mathf.Infinity)
	{
		bool defaultQueriesStartInColliders = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;

		hit = Physics2D.CircleCast(ColliderPosition, (Radius * 0.5f), direction, distance, 1 << 9 /*Environment Layer*/ );

		Physics2D.queriesStartInColliders = defaultQueriesStartInColliders;

		if (debugObstacleAvoidance) {
			Debug.DrawLine(ColliderPosition, ColliderPosition + direction * distance, Color.cyan, 0f, false);
		}
		if (hit.collider != null) {
			if (hit.collider.FindComponent(out Entity entity)) { return entity.IsStructure; }
			else { return true; }
		}
		else { return false; }
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

	public Vector2 AvoidCollisionsWithAllies() { return AvoidCollisionsWithAllies(Radius); }
	public Vector2 AvoidCollisionsWithAllies(float radius)	{
		return AvoidCollisions(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a))); 
	}

	public Vector2 AvoidCollisionsAll() { return AvoidCollisionsAll(Radius); }
	public Vector2 AvoidCollisionsAll(float radius) { return AvoidCollisions(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier)); }

	public Vector2 AvoidCollisions(IEnumerable<Entity> targets)
	{
		Vector2 acceleration = Vector2.zero;

		/* 1. Find the target that the character will collide with first */

		/* The first collision time */
		float shortestTime = float.PositiveInfinity;

		/* The first target that will collide and other data that
            * we will need and can avoid recalculating */
		Entity firstTarget = null;
		float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
		Vector2 firstRelativePos = Vector2.zero, firstRelativeVel = Vector2.zero;

		foreach (Entity agent in targets) {
			/* Calculate the time to collision */
			Vector2 relativePos = ColliderPosition - agent.ColliderPosition;
			Vector2 relativeVel = Velocity - agent.Velocity;
			float distance = relativePos.magnitude;
			float relativeSpeed = relativeVel.magnitude;

			if (relativeSpeed == 0) {
				continue;
			}

			float timeToCollision = -1 * Vector2.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

			/* Check if they will collide at all */
			Vector2 separation = relativePos + relativeVel * timeToCollision;
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
		acceleration.Normalize();
		acceleration *= maxAcceleration * collisionAvoidanceMultiplier;

		return acceleration;
	}

	public IEnumerable<Entity> GetAgentsInRadius() { return GetAgentsInRadius(Radius); }
	public IEnumerable<Entity> GetAgentsInRadius(float radius) {
		return
			Physics2D.OverlapCircleAll(ColliderPosition, radius)
			.Where(c => c.FindComponent(out Entity entity) && entity != this )
			.Select(c => c.FindComponent<Entity>()).Distinct();
	}

	#endregion

	#region Flee

	[Header("Flee")]

	public float fleePanicDist = 3.5f;

	public bool fleeDecelerateOnStop = true;

	public float fleeTimeToTarget = 0.1f;

	public Vector2 Flee(Vector2 targetPosition)
	{
		/* Get the direction */
		Vector2 acceleration = Position - targetPosition;

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
				Velocity = Vector2.zero;
				return Vector2.zero;
			}
		}

		return FleeGiveMaxAccel(acceleration);
	}

	Vector2 FleeGiveMaxAccel(Vector2 v)
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

	public Vector2 Evade(Agent target)
	{
		/* Calculate the distance to the target */
		Vector2 displacement = target.Position - Position;
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
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		return Flee(explicitTarget);
	}

	#endregion

	#region Hide

	[Header("Hide")]

	public float hideDistanceFromBoundary = 0.6f;

	public Vector2 Hide(Agent target, ICollection<Agent> obstacles)
	{
		Vector2 bestHidingSpot;
		return Hide(target, obstacles, out bestHidingSpot);
	}

	public Vector2 Hide(Agent target, ICollection<Agent> obstacles, out Vector2 bestHidingSpot)
	{
		/* Find the closest hiding spot. */
		float distToClostest = Mathf.Infinity;
		bestHidingSpot = Vector2.zero;

		foreach (Agent r in obstacles) {
			Vector2 hidingSpot = GetHidingPosition(r, target);

			float dist = Vector2.Distance(hidingSpot, transform.position);

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

	Vector2 GetHidingPosition(Agent obstacle, Agent target)
	{
		float distAway = obstacle.Radius + hideDistanceFromBoundary;

		Vector2 dir = obstacle.Position - target.Position;
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

	public Vector2 Wander1()
	{
		float characterOrientation = RotationInRadians;

		/* Update the wander orientation */
		wanderOrientation += RandomBinomial() * wanderRate;

		/* Calculate the combined target orientation */
		float targetOrientation = wanderOrientation + characterOrientation;

		/* Calculate the center of the wander circle */
		Vector2 targetPosition = Position + (OrientationToVector(characterOrientation) * wanderOffset);

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

	Vector2 wander2Target;

	public Vector2 Wander2()
	{
		/* Get the jitter for this time frame */
		float jitter = wander2Jitter * Time.deltaTime;

		/* Add a small random vector to the target's position */
		wander2Target += new Vector2(Random.Range(-1f, 1f) * jitter, Random.Range(-1f, 1f) * jitter);

		/* Make the wanderTarget fit on the wander circle again */
		wander2Target.Normalize();
		wander2Target *= wander2Radius;

		/* Move the target in front of the character */
		Vector2 targetPosition = Position + (Vector2)transform.right * wander2Distance + wander2Target;

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

	public Vector2 SeparationFromAllies() { return SeparationFromAllies(Radius); }
	public Vector2 SeparationFromAllies(float radius) {
		return Separation(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a)));
	}

	public Vector2 SeparationAll() { return SeparationAll(Radius * 4f); }
	public Vector2 SeparationAll(float radius) {
		return Separation(GetAgentsInRadius(radius * collisionDetectionRadiusMultiplier));
	}

	public Vector2 Separation(IEnumerable<Entity> targets)
	{
		Vector2 acceleration = Vector2.zero;

		foreach (Entity agent in targets) {
			if (!IsFriendly(agent)) { continue; }
			/* Get the direction and distance from the target */
			Vector2 direction = ColliderPosition - agent.ColliderPosition;
			float dist = direction.magnitude;

			if (dist < maxSepDist) {
				/* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
				var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - Radius - agent.Radius);

				/* Added separation acceleration to the existing steering */
				direction.Normalize();
				acceleration += direction * strength;
			}
		}

		return acceleration;
	}

	#endregion

}

