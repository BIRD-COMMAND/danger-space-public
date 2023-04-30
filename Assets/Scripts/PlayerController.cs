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

	[Header("Bullet Time")]
	[SerializeField] private float slowTimeFactor = 0.2f;
	[SerializeField] private bool slowTime = false;

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

		if (Mouse.RightClick) {
			if (!slowTime) { 
				Time.timeScale = slowTimeFactor;
				body.velocity /= slowTimeFactor;
				slowTime = true;
			}
			else { 
				Time.timeScale = 1f;
				body.velocity *= slowTimeFactor;
				slowTime = false;
			}
		}

	}

	void FixedUpdate()
	{

		// Lerp up vector toward mouse position in worldspace
		body.LookAt(Mouse.WorldPosition, slowTime ? Mathf.Max(turnFactor, 0.6f) : turnFactor);
		
		// debug aim visualization
		if (debugVisualizeAim) { Debug.DrawLine(transform.position, transform.position + (transform.up * transform.position.DistTo(Mouse.WorldPosition)), Color.green); }

		// Movement
		if (slowTime) { body.velocity *= slowTimeFactor; }

		// limit max speed - this approach allows the player to exceed the max speed, but the braking intensifies the faster the player goes
		if (body.velocity.magnitude > maxSpeed) { body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, maxSpeedBrakeFactor); }

		for (float increment = Time.timeScale; !slowTime || increment < 1f; increment += Time.timeScale) {

			// get input from WASD and apply as a force to body
			body.AddForce(MoveVector * maxThrust / Time.timeScale);

			// reduce vertical velocity if relevant key is not held down
			if (!KeyW && body.velocity.y > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }
			if (!KeyS && body.velocity.y < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }

			// reduce horizontal velocity if relevant key is not held down
			if (!KeyD && body.velocity.x > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
			if (!KeyA && body.velocity.x < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }

			if (!slowTime) { break; }
		}

		if (slowTime) { body.velocity /= slowTimeFactor; }

	}

	// handle passthrough damage and healing from PlayerDummy class instances
	public override void RemoteDamage(float damage, Entity source) { base.Damage(damage, source); }
	public override void RemoteHealing(float damage, Entity source) { base.Heal(damage, source); }

}
