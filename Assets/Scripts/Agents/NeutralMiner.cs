using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeutralMiner : Agent
{

	/// <summary>
	/// Cooldown time to use for the mining cooldown timer
	/// </summary>
	[Header("Miner"), Tooltip("Cooldown time for the mining behavior"), SerializeField]
	protected float miningCooldown = 3f;
	/// <summary>
	/// Cooldown time to use for the mining cooldown timer
	/// </summary>
	public float MiningCooldown => miningCooldown;

	/// <summary>
	/// Cooldown timer for mining
	/// </summary>
	protected float miningCooldownTimer = 0f;
	/// <summary>
	/// Cooldown timer for mining
	/// </summary>
	public float MiningCooldownTimer => miningCooldownTimer;

	/// <summary>
	/// Distance at which the miner can mine
	/// </summary>
	[SerializeField]
	protected float miningRange = 2f;
	/// <summary>
	/// Distance at which the miner can mine
	/// </summary>
	public float MiningRange => miningRange;

	/// <summary>
	/// Offset to use for mining the target: includes the target's radius, the miner's radius, and the mining range
	/// </summary>
	public float MiningTargetOffset => target ? target.Radius + Radius + miningRange : Radius;

	/// <summary>
	/// The LineRenderer representing the mining beam
	/// </summary>
	protected LineRenderer line;
	/// <summary>
	/// The number of points to use for the LineRenderer
	/// </summary>
	public int beamPoints = 12;
	/// <summary>
	/// The amount of jitter to apply to the LineRenderer
	/// </summary>
	public float beamJitter = 0.12f;
	
	/// <summary>
	/// Get component references
	/// </summary>
	protected override void Awake() { base.Awake(); line = GetComponent<LineRenderer>(); }

	/// <summary>
	/// The miner performs its steering behavior on each FixedUpdate.
	/// </summary>
	private void FixedUpdate()
	{

		// update mining cooldown timer
		miningCooldownTimer -= Time.fixedDeltaTime;
		if (miningCooldownTimer < 0f) { miningCooldownTimer = 0f; }

		// return if no target
		if (!target) { return; }

		// pursue target for mining
		Enemy_Pursue();

		// generate lightning effect on mining beam LineRenderer
		if (line.enabled) {
			float j = beamJitter; line.positionCount = beamPoints;
			line.SetPosition(0, Position); line.SetPosition(beamPoints - 1, target.Position);
			for (int i = 1; i < beamPoints - 1; i++) {
				line.SetPosition(i, Vector3.Lerp(Position, target.Position, (float)i / (beamPoints - 1))
					+ new Vector3(Random.Range(-j, j), Random.Range(-j, j), Random.Range(-j, j)));
			}
		}

	}

	/// <summary>
	/// Pursue asteroid target and mine it if in range
	/// </summary>
	private void Enemy_Pursue()
	{

		// update path with target position
		path[0] = Position; path[1] = target.Position;

		// get close to target, avoid obstacles
		Vector2 accel = Vector2.zero;
		arriveTargetRadius = MiningTargetOffset;
		accel += Arrive(target.Position);
		accel += FlowField.GetForce(this) * 10f;


		// face target if in range, else face heading
		if (IsArrivingInRange(target, MiningTargetOffset)) {
			if (TargetInSight(target) && !line.enabled) { line.enabled = true; }
			Steer(accel); FaceTarget();
			if (Velocity.magnitude < 2f && miningCooldownTimer <= 0f) {
				target.Damage(1f, this); miningCooldownTimer = miningCooldown;
				if (faction.HasFlag(Faction.Player)) { GameManager.AddScore(5f); }
			}
		}
		else {
			line.enabled = false;
			accel += AvoidObstacles();
			Steer(accel); FaceHeading(); 
		}

	}

}
