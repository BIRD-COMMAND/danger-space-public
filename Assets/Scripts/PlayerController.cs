using UnityEngine;
using Extensions;
using Shapes;

/// <summary>
/// PlayerController class is responsible for handling the player's movement, interactions, and weapon firing in the game.
/// </summary>
public class PlayerController : Agent
{

	/// <summary>
	/// The glow effect GameObject attached to the Player's ship
	/// </summary>
	[Header("Player"), SerializeField, Tooltip("The glow effect GameObject attached to the Player's ship")] 
	private GameObject glow;
	/// <summary>
	/// The base weapon GameObject that the player uses to fire
	/// </summary>
	[SerializeField, Tooltip("The base weapon GameObject that the player uses to fire")]
	private Weapon baseWeapon;
	/// <summary>
	/// The maximum speed the player can reach
	/// </summary>
	[SerializeField, Tooltip("The maximum speed the player can reach")]
	private float maxSpeed = 40f;
	/// <summary>
	/// The factor applied to braking when the player exceeds the maximum speed
	/// </summary>
	[SerializeField, Tooltip("The factor applied to braking when the player exceeds the maximum speed")]
	private float maxSpeedBrakeFactor = 0.025f;
	/// <summary>
	/// The maximum thrust force applied to the player's movement
	/// </summary>
	[SerializeField, Tooltip("The maximum thrust force applied to the player's movement")]
	private float maxThrust = 10000f;
	/// <summary>
	/// The factor applied to braking when the player releases a movement key
	/// </summary>
	[SerializeField, Tooltip("The factor applied to braking when the player releases a movement key")]
	private float brakeFactor = 0.1f;
	/// <summary>
	/// The factor applied to turning the player's ship towards the mouse cursor
	/// </summary>
	[SerializeField, Tooltip("The factor applied to turning the player's ship towards the mouse cursor")]
	private float turnFactor = 0.3f;
	/// <summary>
	/// A boolean value to enable or disable the aim visualization during debugging
	/// </summary>
	[SerializeField, Tooltip("A boolean value to enable or disable the aim visualization during debugging")]
	private bool debugVisualizeAim = false;

	/// <summary>
	/// The maximum amount of energy the player can have
	/// </summary>
	[Header("Energy"), SerializeField, Tooltip("The maximum amount of engery the player can have")]
	private float maxEnergy = 100f;
	public float MaxEnergy => maxEnergy;
	/// <summary>
	/// The current amount of energy the player has
	/// </summary>
	[SerializeField, Tooltip("The current amount of energy the player has")] 
	private float currentEnergy = 100f;
	public float CurrentEnergy { get => currentEnergy; set => currentEnergy = Mathf.Clamp(value, 0f, maxEnergy); }
	/// <summary>
	/// The percentage of energy the player has
	/// </summary>
	public float EnergyPercent => currentEnergy / maxEnergy;


	#region Properties for getting the state of the movement keys

	private Vector2 MoveVector =>
		new Vector2(
			(KeyA ? -1 : 0f) + (KeyD ? 1f : 0f),
			(KeyW ? 1 : 0f) + (KeyS ? -1f : 0f)
		);
	private bool KeyW => Input.GetKey(KeyCode.W);
	private bool KeyA => Input.GetKey(KeyCode.A);
	private bool KeyS => Input.GetKey(KeyCode.S);
	private bool KeyD => Input.GetKey(KeyCode.D);

	#endregion


	private void Update()
	{
		// return if the game is paused
		if (GameManager.IsPaused) { return; }

		// reposition thrusters based on whether current velocity is moving toward or away from mouse
		foreach (ThrustRepositioning item in GetComponentsInChildren<ThrustRepositioning>()) { 
			item.ApplyThrust(Vector2.Dot(body.velocity.normalized, transform.DirToMouse()));
		}

		Ability_BulletTime();

		// fire weapon on left click
		if (Mouse.LeftDown) { baseWeapon.Fire(this); }
		
	}

	void FixedUpdate()
	{

		// Lerp up vector toward mouse position in worldspace
		body.LookAt(Mouse.WorldPosition, inBulletTime ? Mathf.Max(turnFactor, 0.6f) : turnFactor);
		
		// debug aim visualization
		if (debugVisualizeAim) { Debug.DrawLine(transform.position, transform.position + (transform.up * transform.position.DistTo(Mouse.WorldPosition)), Color.green); }

		// Movement
		if (inBulletTime) { body.velocity *= GameManager.BulletTimeFactor; }

		// limit max speed - this approach allows the player to exceed the max speed, but the braking intensifies the faster the player goes
		if (body.velocity.magnitude > maxSpeed) { body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, maxSpeedBrakeFactor); }

		for (float increment = Time.timeScale; !inBulletTime || increment < 1f; increment += Time.timeScale) {

			// get input from WASD and apply as a force to body
			body.AddForce(MoveVector * maxThrust / Time.timeScale);

			// reduce vertical velocity if relevant key is not held down
			if (!KeyW && body.velocity.y > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }
			if (!KeyS && body.velocity.y < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithY(0f), brakeFactor); }

			// reduce horizontal velocity if relevant key is not held down
			if (!KeyD && body.velocity.x > 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }
			if (!KeyA && body.velocity.x < 0f) { body.velocity = Vector2.Lerp(body.velocity, body.velocity.WithX(0f), brakeFactor); }

			if (!inBulletTime) { break; }
		}

		if (inBulletTime) { body.velocity /= GameManager.BulletTimeFactor; }

	}

	/// <summary>
	/// The Player's ability to slow down time while holding down the right mouse button
	/// </summary>
	private void Ability_BulletTime()
	{
		if (Mouse.RightDown) { CurrentEnergy -= Time.unscaledDeltaTime * 10f; }
		if (inBulletTime) {
			if (!Mouse.RightDown || CurrentEnergy <= 0f) { 
				inBulletTime = false; GameManager.BulletTime = false;
				if (TryGetComponent(out BulletTimeScaler scaler)) { Destroy(scaler); }
				body.velocity *= GameManager.BulletTimeFactor;
				SetGlowColor(Color.white);
			}
		}
		else {
			if (Mouse.RightDown && CurrentEnergy > 0f) { 
				inBulletTime = true; GameManager.BulletTime = true;
				gameObject.AddComponent<BulletTimeScaler>();
				SetGlowColor(Color.cyan);
			}
		}
	}

	/// <summary>
	/// Sets the color of the glow around the Player's ship
	/// </summary>
	public void SetGlowColor(Color color)
	{
		glow.GetComponent<Disc>().ColorInner = color.WithAlpha(0.6f);
		glow.GetComponent<Disc>().ColorOuter = color.WithAlpha(0f);
	}
	
}