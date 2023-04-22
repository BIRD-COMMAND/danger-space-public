using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;


public class PlayerController : MonoBehaviour
{

	public static PlayerController player;

	public bool debugVisualizeAim = false;

	public Weapon baseWeapon;
	public Rigidbody2D body;
	public float maxSpeed = 20f;
	public float maxSpeedBrakeFactor = 0.025f;
	public float maxThrust = 10000f;
	public float brakeFactor = 0.1f;
	public float turnFactor = 0.2f;

	private Vector2 MoveVector =>
		new Vector2(
			(KeyA ? -1 : 0f) + (KeyD ? 1f : 0f),
			(KeyW ? 1 : 0f) + (KeyS ? -1f : 0f)
		);
	private bool KeyW => Input.GetKey(KeyCode.W);
	private bool KeyA => Input.GetKey(KeyCode.A);
	private bool KeyS => Input.GetKey(KeyCode.S);
	private bool KeyD => Input.GetKey(KeyCode.D);

	private void Awake() { 
		player = this; 
		body = GetComponent<Rigidbody2D>(); 
		baseWeapon = GetComponent<Weapon>();
	}

	// Start is called before the first frame update
	private void Update()
	{
		// reposition thrusters based on whether current velocity is moving toward or away from mouse
		foreach (ThrustRepositioning item in GetComponentsInChildren<ThrustRepositioning>()) { 
			item.ApplyThrust(Vector2.Dot(body.velocity.normalized, transform.DirToMouse()));
		}

		if (Mouse.LeftDown) { baseWeapon.Fire(); }

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

		// limit max speed
		if (body.velocity.magnitude > maxSpeed) { body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, maxSpeedBrakeFactor); }

		// reduce velocity if key is not held down
		if (!KeyW && body.velocity.y > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }
		if (!KeyS && body.velocity.y < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }

		// reduce velocity if key is not held down
		if (!KeyD && body.velocity.x > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
		if (!KeyA && body.velocity.x < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		
	}

}
