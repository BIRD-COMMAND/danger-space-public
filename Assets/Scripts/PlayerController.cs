using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;
using Unity.VisualScripting;

public class PlayerController : Agent
{
	[Header("Player")]
	[SerializeField] private Weapon baseWeapon;
	[SerializeField] private float maxSpeed = 40f;
	[SerializeField] private float maxSpeedBrakeFactor = 0.025f;
	[SerializeField] private float maxThrust = 10000f;
	[SerializeField] private float brakeFactor = 0.1f;
	[SerializeField] private float turnFactor = 0.3f;

	[SerializeField] private bool debugVisualizeAim = false;

	private Vector2 MoveVector =>
		new Vector2(
			(KeyA ? -1 : 0f) + (KeyD ? 1f : 0f),
			(KeyW ? 1 : 0f) + (KeyS ? -1f : 0f)
		);
	private bool KeyW => Input.GetKey(KeyCode.W);
	private bool KeyA => Input.GetKey(KeyCode.A);
	private bool KeyS => Input.GetKey(KeyCode.S);
	private bool KeyD => Input.GetKey(KeyCode.D);

	// Start is called before the first frame update
	private void Update()
	{
		// reposition thrusters based on whether current velocity is moving toward or away from mouse
		foreach (ThrustRepositioning item in GetComponentsInChildren<ThrustRepositioning>()) { 
			item.ApplyThrust(Vector2.Dot(body.velocity.normalized, transform.DirToMouse()));
		}

		if (Mouse.LeftDown) { baseWeapon.Fire(this); }

		//if (Mouse.RightClick) { Time.timeScale = Time.timeScale < 0.9f ? 1f : 0.1f; }

	}

	void FixedUpdate()
	{

		// when angular velocity is less than 1, immediately set it to 0, this cuts some of the slowness out of the mouse offset correction
		if (body.angularVelocity > 0f && body.angularVelocity < 2f) { body.angularVelocity = 0f; }

		// Lerp up vector toward mouse position in worldspace
		
		// when the body's angular velocity is low we lerp normally
		if (body.angularVelocity < 0.5f) { body.LookAt(Mouse.WorldPosition, turnFactor); }
		// the higher the body's angular velocity, the more we lerp to compensate
		else { body.LookAt(Mouse.WorldPosition, Mathf.Clamp01(1f - (1f / body.angularVelocity) + turnFactor)); }
		
		// debug aim visualization
		if (debugVisualizeAim) { Debug.DrawLine(transform.position, transform.position + (transform.up * transform.position.DistTo(Mouse.WorldPosition)), Color.green); }

		// Movement
		// get input from WASD and apply as a force to body
		Vector2 force = MoveVector * maxThrust;
		body.AddForce(force);

		// limit max speed - this approach allows the player to exceed the max speed, but the braking intensifies the faster the player goes
		if (body.velocity.magnitude > maxSpeed) { body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, maxSpeedBrakeFactor); }

		// reduce vertical velocity if relevant key is not held down
		if (!KeyW && body.velocity.y > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }
		if (!KeyS && body.velocity.y < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }

		// reduce horizontal velocity if relevant key is not held down
		if (!KeyD && body.velocity.x > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
		if (!KeyA && body.velocity.x < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
	}

	// handle passthrough damage and healing from PlayerDummy class instances
	public override void RemoteDamage(float damage, Entity source) { base.Damage(damage, source); }
	public override void RemoteHealing(float damage, Entity source) { base.Heal(damage, source); }

}
