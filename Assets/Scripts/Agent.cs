using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

/// <summary>
/// An Agent is an Entity with movement and steering capabilities implemented. It is the base class for all moving AI units in the game.
/// </summary>
public abstract class Agent : Entity
{

	// adapted from https://github.com/sturdyspoon/unity-movement-ai/	
	// more info on steering behaviors at https://github.com/libgdx/gdx-ai/wiki/Steering-Behaviors#individual-behaviors

	#region States

	public enum State
	{

		Self_Idle, Self_Drift, Self_Patrol, Self_Flee, Self_FleeBattle, Self_FleeSeekAllies, 

		Leader_Seek, Leader_SeekArrive, Leader_Pursue, Leader_PursueArrive, 
		Leader_Flee, Leader_Attack, Leader_Defend, Leader_Repair, Leader_Buff, 

		Formation_Seek, Formation_SeekArrive, Formation_Pursue, Formation_PursueArrive, 
		Formation_Flee, Formation_Attack, Formation_Defend, Formation_Repair, Formation_Buff, 
		Formation_Create, Formation_Destroy, Formation_Enter, Formation_Fly, 
		Formation_AttackFrom, Formation_DefendFrom, Formation_RepairFrom, Formation_BuffFrom, 

		Enemy_Seek, Enemy_SeekArrive, Enemy_Pursue, Enemy_PursueArrive, 
		Enemy_Flee, Enemy_Attack, Enemy_Defend, Enemy_Repair, Enemy_Buff, 

		Zone_ApproachRally, Zone_ApproachFallback, Zone_Approach1, Zone_Approach2, Zone_Approach3, Zone_Approach4, 

	}

	#endregion

	/// <summary>
	/// The current AI state of the Agent
	/// </summary>
	[Header("Agent"), Tooltip("The current AI state of the Agent")]
	public State state;

	/// <summary>
	/// The current path the agent is following
	/// </summary>
	[Tooltip("The current path the agent is following")]
	public LinePath path;

	/// <summary>
	/// Set up Entity default path and hide distance.
	/// </summary>
	protected override void Awake() {
		base.Awake();
		path = new LinePath(Position, Vector3.zero);
		hideDistanceFromBoundary = Radius * 1.5f;
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
	[Tooltip("The radius from the target that means we are close enough and have arrived")]
	public float arriveTargetRadius = 0.005f;
	/// <summary>
	/// The radius from the target where we start to slow down
	/// </summary>
	[Tooltip("The radius from the target where we start to slow down")]
	public float arriveSlowRadius = 1f;
	/// <summary>
	/// The time in which we want to achieve the targetSpeed
	/// </summary>
	[Tooltip("The time in which we want to achieve the targetSpeed")]
	public float arriveTimeToTarget = 0.1f;

	/// <summary>
	/// A seek steering behavior. Will return the steering for the current game object to seek a given position
	/// </summary>
	public Vector2 Seek(Vector2 targetPosition, float maxSeekAcceleration)
	{
		//Get the direction
		Vector2 acceleration = targetPosition - Position;
		//Accelerate to the target
		return acceleration.normalized * maxSeekAcceleration;
	}

	/// <summary>
	/// A seek steering behavior. Will return the steering for the current game object to seek a given position
	/// </summary>
	public Vector2 Seek(Vector2 targetPosition) { return Seek(targetPosition, maxAcceleration); }

	/// <summary>
	/// Returns the steering for a character so it arrives at the target
	/// </summary>
	public Vector2 Arrive(Vector2 targetPosition)
	{

		//Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

		//Get the right direction for the linear acceleration

		Vector2 targetVelocity = targetPosition - Position;

		//Get the distance to the target
		float dist = targetVelocity.magnitude;

		//If we are within the stopping radius then stop
		if (dist < arriveTargetRadius) {
			Velocity = Vector2.zero;
			return Vector2.zero;
		}

		//Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance
		float targetSpeed;
		if (dist > arriveSlowRadius) {
			targetSpeed = maxVelocity;
		}
		else {
			targetSpeed = maxVelocity * (dist / arriveSlowRadius);
		}

		//Give targetVelocity the correct speed
		targetVelocity.Normalize();
		targetVelocity *= targetSpeed;

		//Calculate the linear acceleration we want
		Vector2 acceleration = targetVelocity - Velocity;

		// Rather than accelerate the character to the correct speed in 1 second, 
		// accelerate so we reach the desired speed in arriveTimeToTarget seconds
		// (if we were to actually accelerate for the full arriveTimeToTarget seconds).
		acceleration *= 1 / arriveTimeToTarget;

		//Make sure we are accelerating at max acceleration
		if (acceleration.magnitude > maxAcceleration) {
			acceleration.Normalize();
			acceleration *= maxAcceleration;
		}
		
		return acceleration;
	}

	/// <summary>
	/// Returns true if the Agent is within the arriveSlowRadius of its targetPosition
	/// </summary>
	public bool IsArriving(Vector2 targetPosition) { return (targetPosition - Position).magnitude < arriveSlowRadius; }

	#endregion

	#region Pursuit

	[Header("Pursuit")]

	/// <summary>
	/// Maximum prediction time the pursue will predict in the future
	/// </summary>
	[Tooltip("Maximum prediction time the pursue will predict in the future")]
	public float maxPredictionTime = 1f;

	/// <summary>
	/// Radius in which the pursuing agent will start to slow down
	/// </summary>
	[Tooltip("Radius in which the pursuing agent will start to slow down")]
	public float pursuitSlowRadius = 10f;

	/// <summary>
	/// Predicts the targets location and calculates the steering to catch it
	/// </summary>
	public Vector2 Pursue(Entity target)
	{
		//Calculate the distance to the target
		Vector2 displacement = target.Position - Position;
		float distance = displacement.magnitude;

		//Get the character's speed 
		float speed = Velocity.magnitude;

		//Calculate the prediction time
		float prediction;
		if (speed <= distance / maxPredictionTime) {
			prediction = maxPredictionTime;
		}
		else {
			prediction = distance / speed;
		}

		//Put the target together based on where we think the target will be
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget);

		return Seek(explicitTarget);
	}

	/// <summary>
	/// Predicts the targets location and calculates the steering to reach it while maintaining a safe distance
	/// </summary>
	public Vector2 GetInRange(Entity target, float range)
	{
		//Calculate the distance to the target
		Vector2 displacement = target.Position - Position;
		float distance = displacement.magnitude;

		//Get the character's speed 
		float speed = Velocity.magnitude;

		//Calculate the prediction time
		float prediction;
		if (speed <= distance / maxPredictionTime) {
			prediction = maxPredictionTime;
		}
		else {
			prediction = distance / speed;
		}

		//Put the target together based on where we think the target will be
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		//Debug.DrawLine(transform.position, explicitTarget + ((Position - explicitTarget).normalized * range));

		return Arrive(explicitTarget + ((Position - explicitTarget).normalized * range));
	}

	/// <summary>
	/// Returns true if the Agent is within the arriveSlowRadius of the given range relative to its targetPosition.
	/// </summary>
	public bool IsArrivingInRange(Entity target, float range) {
		Vector2 displacement = target.Position - Position;
		float distance = displacement.magnitude;
		float speed = Velocity.magnitude;
		float prediction = (speed <= distance / maxPredictionTime) ? maxPredictionTime : distance / speed;
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;
		return (explicitTarget + ((Position - explicitTarget).normalized * range) - Position).magnitude < arriveSlowRadius;
	}

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

	/// <summary>
	/// The radius at which the character will stop moving along the path.
	/// </summary>
	[Header("Pathing"), Tooltip("The radius at which the character will stop moving along the path.")]
	public float stopRadius = 0.05f;

	/// <summary>
	/// A float that modifies the distance from the path that the character will seek to.
	/// </summary>
	[Tooltip("A float that modifies the distance from the path that the character will seek to.")]
	public float pathOffset = 0.71f;

	/// <summary>
	/// A float that modifies the direction of the path. 1 is forward, -1 is backward.
	/// </summary>
	[Tooltip("A float that modifies the direction of the path. 1 is forward, -1 is backward.")]
	public float pathDirection = 1f;

	/// <summary>
	/// Returns a steering force that will move the agent along the given path.
	/// </summary>
	public Vector2 FollowPath(LinePath path, bool pathLoop = false) { return FollowPath(path, pathLoop, out _); }

	/// <summary>
	/// Returns a steering force that will move the agent along the given path.
	/// </summary>
	public Vector2 FollowPath(LinePath path, bool pathLoop, out Vector2 targetPosition)
	{

		//If the path has only one node then just go to that position.
		if (path.Length == 1) { targetPosition = path[0]; }
		//Else find the closest spot on the path to the character and go to that instead. 
		else {
			//Get the param for the closest position point on the path given the character's position 

		   float param = path.GetParam(transform.position, this);

			//Debug.DrawLine(transform.position, path.getPosition(param, pathLoop), Color.red, 0, false);

			if (!pathLoop) {
				Vector2 finalDestination;

				//If we are close enough to the final destination then stop moving
				if (IsAtEndOfPath(path, param, out finalDestination)) {
					targetPosition = finalDestination;

					Velocity = Vector2.zero;
					return Vector2.zero;
				}
			}

			//Move down the path
			param += pathDirection * pathOffset;

			//Set the target position
			targetPosition = path.GetPosition(param, pathLoop);

			//Debug.DrawLine(transform.position, targetPosition, Color.red, 0, false);
		}

		return Arrive(targetPosition);
	}

	/// <summary> 
	/// Returns true if the character is at the end of the given path 
	/// </summary>
	public bool PathingComplete { 
		get {
			//If the path has only one node then just check the distance to that node.
			if (path.Length == 1) {
				Vector2 endPos = path[0];
				return Vector2.Distance(Position, endPos) < stopRadius;
			}
			//Else see if the character is at the end of the path. 
			else {
				Vector2 finalDestination;

				//Get the param for the closest position point on the path given the character's position 

				float param = path.GetParam(transform.position, this);

				return IsAtEndOfPath(path, param, out finalDestination);
			}
		}
	}

	/// <summary>
	/// Returns true if the character is at the end of the given path
	/// </summary>
	bool IsAtEndOfPath(LinePath path, float param, out Vector2 finalDestination)
	{
		bool result;

		//Find the final destination of the character on this path
		finalDestination = (pathDirection > 0) ? path.Last : path.First;

		//If the param is closest to the last segment then check if we are at the final destination
		if (param >= path.distances[path.Length - 2]) {
			result = Vector2.Distance(Position, finalDestination) < stopRadius;
		}
		//Else we are not at the end of the path
		else { result = false; }

		return result;
	}

	#endregion

	#region Obstacle Avoidance

	[Header("Obstacle Avoidance")]

	/// <summary>
	/// The distance away from the collision that we wish go
	/// </summary>
	[Tooltip("The distance away from the collision that we wish go")]
	public float obstacleAvoidDistance = 4f;

	/// <summary>
	/// Multiplier applied to the Seek vector produced by ObstacleAvoidance.
	/// </summary>
	[Tooltip("Multiplier applied to the Seek vector produced by ObstacleAvoidance.")]
	public float obstacleAvoidanceMultiplier = 4f;

	/// <summary>
	/// The length of the main whisker ray
	/// </summary>
	[Range(0.125f, 40f), Tooltip("The length of the main whisker ray")]
	public float mainWhiskerLength = 12.5f;
	/// <summary>
	/// The length of the whisker rays to the left and right of the main whisker
	/// </summary>
	[Range(0.07f, 40f), Tooltip("The length of the whisker rays to the left and right of the main whisker")]
	public float sideWhiskerLength = 7f;
	/// <summary>
	/// The angle of the side whiskers
	/// </summary>
	[Range(1f, 90f), Tooltip("The angle of the side whiskers")]
	public float sideWhiskerAngle = 45f;

	/// <summary>
	/// Draws debug visualizations for the obstacle avoidance algorithm when set to true.
	/// </summary>
	public bool debugObstacleAvoidance = false;

	protected static readonly LayerMask LayerMask_EnvironmentAndLevelBoundsOnly = (1 << 9) | (1 << 3);
	protected static List<RaycastHit2D> hits = new List<RaycastHit2D>();
	protected static RaycastHit2D hit;

	/// <summary>
	/// Returns a steering vector that will avoid obstacles in front of the character.
	/// </summary>
	public Vector2 AvoidObstacles()
	{
		// Use either the velocity or the forward direction of the character to determine the facing direction
		Vector2 facingDir = Velocity.magnitude > 0.01f ? Velocity.normalized : transform.up.normalized;

		hits.Clear();
		float closestDistance = float.PositiveInfinity;

		// Check for obstacles in front of the character and to the left and right
		if (DetectObstacles(facingDir, out hit, mainWhiskerLength)) { 
			hits.Add(hit); closestDistance = Vector2.Distance(hit.point, Position); 
		}
		if (DetectObstacles(OrientationToVector(VectorToOrientation(facingDir) + sideWhiskerAngle * Mathf.Deg2Rad), out hit, sideWhiskerLength)) { 
			if (Vector2.Distance(hit.point, Position) < closestDistance) { hits.Add(hit); closestDistance = Vector2.Distance(hit.point, Position); }
		}
		if (DetectObstacles(OrientationToVector(VectorToOrientation(facingDir) - sideWhiskerAngle * Mathf.Deg2Rad), out hit, sideWhiskerLength)) { 
			if (Vector2.Distance(hit.point, Position) < closestDistance) { hits.Add(hit); }
		}

		// If there are no obstacles then return zero acceleration
		if (hits.Count == 0) { return Vector2.zero; }

		// base acceleration on the closest (last) raycast hit
		hit = hits[hits.Count - 1];
		Vector2 targetPosition = hit.point + hit.normal * obstacleAvoidDistance;
		Vector2 acceleration = Seek(targetPosition, maxAcceleration * obstacleAvoidanceMultiplier);
		if (debugObstacleAvoidance) { 
			Debug.DrawLine(hit.point, targetPosition, Color.blue, Time.fixedDeltaTime, false);
			Debug.DrawRay(Position, acceleration.normalized, Color.green, Time.fixedDeltaTime, false);
		}
		return acceleration;

	}

	/// <summary>
	/// Returns true if the given direction is obstructed by an Environment or LevelBounds collider.
	/// </summary>
	bool DetectObstacles(Vector2 direction, out RaycastHit2D hit, float distance = Mathf.Infinity)
	{
		bool defaultQueriesStartInColliders = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;

		//TODO try this circlecast using radius instead of radius * 0.5f, see how that effects obstacle avoidance
		hit = Physics2D.CircleCast(ColliderPosition, (Radius * 0.5f), direction, distance, LayerMask_EnvironmentAndLevelBoundsOnly);

		Physics2D.queriesStartInColliders = defaultQueriesStartInColliders;

		if (debugObstacleAvoidance) { Debug.DrawLine(ColliderPosition, ColliderPosition + direction * distance, Color.cyan, Time.fixedDeltaTime, false); }

		if (hit.collider == null) { return false; }				// no obstacle detected, return false
		if (hit.collider.FindComponent(out Entity entity)) {	//
			if (entity == target) { return false; }				// target is not an obstacle, return false
			else { return entity.IsStructure; }					// structures are obstacles, other entities are not
		}														//
		else { return true; }									// a normal obstacle was detected, return true
	}

	#endregion

	#region Collision Avoidance

	[Header("Collision Avoidance")]

	/// <summary>
	/// How much space can be between two entities before they are considered colliding
	/// </summary>
	[Tooltip("How much space can be between two entities before they are considered colliding")]
	public float collisionDistanceBetween;

	/// <summary>
	/// Multiplier applied to collision detection radius.
	/// </summary>
	[Tooltip("Multiplier applied to collision detection radius")]
	public float collisionDetectionRadiusMultiplier = 4f;

	/// <summary>
	/// Multiplier applied to the Seek vector produced by ObstacleAvoidance.
	/// </summary>
	[Tooltip("Multiplier applied to the Seek vector produced by ObstacleAvoidance.")]
	public float collisionAvoidanceMultiplier = 1.5f;

	/// <summary>
	/// Returns a steering vector that avoids collisions with all friendly Entities in this Entity's Radius multiplied by the collisionDetectionRadiusMultiplier.
	/// </summary>
	public Vector2 AvoidCollisionsWithAllies() { return AvoidCollisionsWithAllies(Radius); }
	/// <summary>
	/// Returns a steering vector that avoids collisions with all friendly Entities in a given radius multiplied by the collisionDetectionRadiusMultiplier.
	/// </summary>
	public Vector2 AvoidCollisionsWithAllies(float radius)	{
		return AvoidCollisions(GetEntities(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a))); 
	}

	/// <summary>
	/// Returns a steering vector that avoids collisions with all Entities in this Entity's Radius multiplied by the collisionDetectionRadiusMultiplier.
	/// </summary>
	public Vector2 AvoidCollisionsAll() { return AvoidCollisionsAll(Radius); }
	/// <summary>
	/// Returns a steering vector that avoids collisions with all Entities in a given radius multiplied by the collisionDetectionRadiusMultiplier.
	/// </summary>
	public Vector2 AvoidCollisionsAll(float radius) { 
		return AvoidCollisions(GetEntities(radius * collisionDetectionRadiusMultiplier)); 
	}

	/// <summary>
	/// Returns a steering vector that avoids collisions with the given Entities.
	/// </summary>
	public Vector2 AvoidCollisions(IEnumerable<Entity> targets)
	{
		Vector2 acceleration = Vector2.zero;

		// 1.Find the target that the character will collide with first

		// The first collision time
		float shortestTime = float.PositiveInfinity;

		// The first target that will collide and other data that we will need and can avoid recalculating
		Entity firstTarget = null;
		float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
		Vector2 firstRelativePos = Vector2.zero, firstRelativeVel = Vector2.zero;

		foreach (Entity agent in targets) {
			//Calculate the time to collision
			Vector2 relativePos = ColliderPosition - agent.ColliderPosition;
			Vector2 relativeVel = Velocity - agent.Velocity;
			float distance = relativePos.magnitude;
			float relativeSpeed = relativeVel.magnitude;

			if (relativeSpeed == 0) {
				continue;
			}

			float timeToCollision = -1 * Vector2.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

			//Check if they will collide at all
			Vector2 separation = relativePos + relativeVel * timeToCollision;
			float minSeparation = separation.magnitude;

			if (minSeparation > Radius + agent.Radius + collisionDistanceBetween) {
				continue;
			}

			//Check if it's the shortest 
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

		// 2. Calculate the steering

		// If we have no target then exit 
		if (firstTarget == null) {
			return acceleration;
		}

		//If we are going to collide with no separation or if we are already colliding then steer based on current position 
		if (firstMinSeparation <= 0 || firstDistance < Radius + firstRadius + collisionDistanceBetween) {
			acceleration = ColliderPosition - firstTarget.ColliderPosition;
		}
		//Else calculate the future relative position 
		else {
			acceleration = firstRelativePos + firstRelativeVel * shortestTime;
		}

		//Avoid the target 
		acceleration.Normalize();
		acceleration *= maxAcceleration * collisionAvoidanceMultiplier;

		return acceleration;
   }

	#endregion

	#region Flee

	/// <summary>
	/// The distance at which the entity will start to flee.
	/// </summary>
	[Header("Flee"), Tooltip("The distance at which the entity will start to flee.")]
	public float fleePanicDistance = 3.5f;

	/// <summary>
	/// The time over which to achieve target speed.
	/// </summary>
	[Tooltip("The time over which to achieve target speed.")]
	public float fleeTimeToTarget = 0.1f;

	/// <summary>
	/// If true then the entity will decelerate to zero velocity when it stops fleeing.
	/// </summary>
	[Tooltip("If true then the entity will decelerate to zero velocity when it stops fleeing.")]
	public bool fleeDecelerateOnStop = true;
	
	/// <summary>
	/// Returns a steering vector that flees from the given target.
	/// </summary>
	public Vector2 Flee(Entity entity) { return Flee(entity.Position); }
	
	/// <summary>
	/// Returns a steering vector that flees from the given target.
	/// </summary>
	public Vector2 Flee(Vector2 targetPosition)
	{
		//Get the direction
		Vector2 acceleration = Position - targetPosition;

		//If the target is far way then don't flee 
		if (acceleration.magnitude > fleePanicDistance) {
			//Slow down if we should decelerate on stop
			if (fleeDecelerateOnStop && Velocity.magnitude > 0.001f) {
				//Decelerate to zero velocity in time to target amount of time
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

	/// <summary>
	/// Returns the flee vector with the maximum acceleration applied.
	/// </summary>
	Vector2 FleeGiveMaxAccel(Vector2 v) { return v.normalized * maxAcceleration; }

	#endregion

	#region Evade

	[Header("Evade")]

	/// <summary>
	/// Maximum prediction time the pursue will predict in the future
	/// </summary>
	[Tooltip("Maximum prediction time the pursue will predict in the future")]
	public float evadeMaxPredictionTime = 1f;

	/// <summary>
	/// Returns a steering vector that evades the given target.
	/// </summary>
	public Vector2 Evade(Entity target)
	{
		//Calculate the distance to the target
		Vector2 displacement = target.Position - Position;
		float distance = displacement.magnitude;

		//Get the targets's speed 
		float speed = target.Velocity.magnitude;

		//Calculate the prediction time
		float prediction;
		if (speed <= distance / evadeMaxPredictionTime) {
			prediction = evadeMaxPredictionTime;
		}
		else {
			prediction = distance / speed;
			//Place the predicted position a little before the target reaches the character
			prediction *= 0.9f;
		}

		//Put the target together based on where we think the target will be
		Vector2 explicitTarget = target.Position + target.Velocity * prediction;

		return Flee(explicitTarget);
	}

	#endregion

	#region Hide

	//[Header("Hide")]
	/// <summary>
	/// The distance from the boundary of the structure that the character will hide at.
	/// </summary>
	[HideInInspector] public float hideDistanceFromBoundary = 6f;

	/// <summary>
	/// Returns a steering vector that hides the character from the given danger behind one of the structures tracked by the GameManager.<br/>
	/// If no valid hiding spot is found then the character will Evade() the danger.
	/// </summary>
	public Vector2 Hide(Entity danger) { return Hide(danger, GameManager.Structures); }

	/// <summary>
	/// Returns a steering vector that hides the character from the given danger behind one of the obstacles.<br/>
	/// If no valid hiding spot is found then the character will Evade() the danger.
	/// </summary>
	public Vector2 Hide(Entity danger, ICollection<Entity> obstacles) { return Hide(danger, obstacles, out _); }

	/// <summary>
	/// Returns a steering vector that hides the character from the given danger behind one of the obstacles.<br/>
	/// If no valid hiding spot is found then the character will Evade() the danger.<br/>
	/// If any valid hiding spots are found, <paramref name="bestHidingSpot"/> will be set to the best hiding spot.
	/// </summary>
	public Vector2 Hide(Entity danger, ICollection<Entity> obstacles, out Vector2 bestHidingSpot)
	{
		
		//Find the closest hiding spot.
				
		float distToClostest = Mathf.Infinity;
		Vector2 hidingSpot; bestHidingSpot = Vector2.zero;		

		foreach (Entity obstacle in obstacles) {
			
			hidingSpot = GetHidingPosition(obstacle, danger);
			
			float dist = Vector2.Distance(hidingSpot, Position);

			if (dist < distToClostest) {
				distToClostest = dist;
				bestHidingSpot = hidingSpot;
			}

		}

		//If no hiding spot is found then just evade the enemy. 
		if (distToClostest == Mathf.Infinity) { return Evade(danger); }

		//Debug.DrawLine(transform.position, bestHidingSpot);
		return Arrive(bestHidingSpot);
	}

	/// <summary>
	/// Get the hiding position behind the obstacle from the danger
	/// </summary>
	protected Vector2 GetHidingPosition(Entity obstacle, Entity danger)
	{
		float distAway = obstacle.Radius + Radius + hideDistanceFromBoundary;

		Vector2 dir = (obstacle.Position - danger.Position).normalized;

		return obstacle.Position + dir * distAway;
	}

	#endregion

	#region Wander

	[Header("Wander")]

	/// <summary>
	/// The forward offset of the wander square
	/// </summary>
	[Tooltip("The forward offset of the wander square")]
	public float wanderOffset = 16f;

	/// <summary>
	/// The radius of the wander square
	/// </summary>
	[Tooltip("The radius of the wander square")]
	public float wanderRadius = 6;

	/// <summary>
	/// The rate at which the wander orientation can change in radians
	/// </summary>
	[Tooltip("The rate at which the wander orientation can change in radians")]
	public float wanderRate = 1f;

	/// <summary>
	/// The current orientation of the wander behavior in radians
	/// </summary>
	float wanderOrientation = 0;

	/// <summary>
	/// Returns a steering vector that makes the Entity wander around
	/// </summary>
	public Vector2 Wander()
	{
		float characterOrientation = RotationInRadians;

		//Update the wander orientation
		wanderOrientation += RandomBinomial * wanderRate;

		//Calculate the combined target orientation
		float targetOrientation = wanderOrientation + characterOrientation;

		//Calculate the center of the wander circle
		Vector2 targetPosition = Position + ((Vector2)transform.up * wanderOffset);

		//Calculate the target position
		targetPosition = targetPosition + (OrientationToVector(targetOrientation) * wanderRadius);

		// Debug.DrawLine (transform.position, targetPosition);

		return Seek(targetPosition);
	}

	/// <summary>
	/// Returns a random number between -1 and 1. Values around zero are more likely.
	/// </summary>
	private float RandomBinomial => Random.value - Random.value;

	#endregion

	#region Separation

	/// <summary>
	/// The maximum acceleration for separation
	/// </summary>
	[Tooltip("The maximum acceleration for separation")]
	public float maxSeparationAcceleration = 25;

	/// <summary>
	/// This should be the maximum separation distance possible between a separation target and the character
	/// </summary>
	[Tooltip("This should be the maximum separation distance possible between a separation target and the character")]
	public float maxSeparationDistance = 1f;

	/// <summary>
	/// Returns a steering vector that separates the character from all friendly Entities in the Entity's Radius multiplied by the collisionDetectionRadiusMultiplier
	/// </summary>
	public Vector2 SeparationFromAllies() { return SeparationFromAllies(Radius); }
	/// <summary>
	/// Returns a steering vector that separates the character from all friendly Entities in a given radius multiplied by the collisionDetectionRadiusMultiplier
	/// </summary>
	public Vector2 SeparationFromAllies(float radius) {
		return Separation(GetEntities(radius * collisionDetectionRadiusMultiplier).Where(a => IsFriendly(a)));
	}

	/// <summary>
	/// Returns a steering vector that separates the character from all Entities in the Entity's Radius multiplied by the collisionDetectionRadiusMultiplier
	/// </summary>
	public Vector2 SeparationAll() { return SeparationAll(Radius * 4f); }
	/// <summary>
	/// Returns a steering vector that separates the character from all Entities in a given radius multiplied by the collisionDetectionRadiusMultiplier
	/// </summary>
	public Vector2 SeparationAll(float radius) {
		return Separation(GetEntities(radius * collisionDetectionRadiusMultiplier));
	}

	/// <summary>
	/// Returns a steering vector that separates the character from the given targets
	/// </summary>
	public Vector2 Separation(IEnumerable<Entity> targets)
	{
		Vector2 acceleration = Vector2.zero;

		foreach (Entity entity in targets) {
			//if (!IsFriendly(entity)) { continue; }
			//Get the direction and distance from the target
			Vector2 direction = ColliderPosition - entity.ColliderPosition;
			float dist = direction.magnitude;

			if (dist < maxSeparationDistance) {
				//Calculate the separation strength(can be changed to use inverse square law rather than linear)
				float strength = maxSeparationAcceleration * (maxSeparationDistance - dist) / (maxSeparationDistance - Radius - entity.Radius);

				//Added separation acceleration to the existing steering
				direction.Normalize();
				acceleration += direction * strength;
			}
		}

		return acceleration;
	}

	#endregion

}