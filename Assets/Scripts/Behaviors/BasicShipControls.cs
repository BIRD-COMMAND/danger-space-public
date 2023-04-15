using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;


public class BasicShipControls : MonoBehaviour
{

	public float brakeFactor = 0.1f;
	public float maxThrust = 10000f;
	public Rigidbody2D body;

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
	}

	void FixedUpdate()
	{
		// Look at mouse position
		// transform.LookUp(Mouse.WorldPosition);
		transform.rotation = Quaternion.Slerp(
			transform.rotation, 
			Quaternion.FromToRotation(transform.up, (Vector3)Mouse.WorldPosition - transform.position) * transform.rotation, 
			0.1f
		);			

		// Movement
		// get input from WASD and apply as a force to body
		Vector2 force = MoveVector * maxThrust;
		body.AddForce(force);

		// reduce velocity if key is not held down
		if (!KeyW && body.velocity.y > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }
		if (!KeyS && body.velocity.y < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }

		// reduce velocity if key is not held down
		if (!KeyD && body.velocity.x > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
		if (!KeyA && body.velocity.x < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
	}

}
