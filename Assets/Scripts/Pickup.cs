using Extensions;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Poolable] Base class for items which the player picks up with a magnet-like force.
/// </summary>
public class Pickup : Poolable
{

	/// <summary>
	/// Inner trigger of the pickup, as soon as the innerTrigger contains the player, the pickup is picked up.
	/// </summary>
    protected CircleCollider2D innerTrigger;
	/// <summary>
	/// Outer trigger of the pickup, as soon as the outerTrigger touches the player, the pickup is pulled towards the player.
	/// </summary>
    protected CircleCollider2D outerTrigger;

	/// <summary>
	/// Rigidbody2D of the pickup
	/// </summary>
	protected Rigidbody2D body;

	/// <summary>
	/// Should the pickup currently be lerped towards the player?
	/// </summary>
	private bool lerpToPlayer = false;


	// Validate pickup configuration
	protected virtual void Start() {

		// get the rigidbody
		body = GetComponent<Rigidbody2D>();

		// verify that the pickup has the correct components
		if (GetComponents<CircleCollider2D>().Length != 2) {
			Debug.LogError("Destroying invalid Pickup: Missing innerTrigger or outerTrigger", gameObject);
			Destroy(this); return;
		}

		// set the inner and outer triggers based on radius
		CircleCollider2D[] triggers = GetComponents<CircleCollider2D>();
		if (triggers[0].radius < triggers[1].radius) {
			innerTrigger = triggers[0];
			outerTrigger = triggers[1];
		}
		else {
			innerTrigger = triggers[1];
			outerTrigger = triggers[0];
		}

	}

	// Handle lerping towards player
	protected virtual void FixedUpdate()
	{
		// if not lerping to player, return
		if (!lerpToPlayer) { return; }
		
		// if lerping to player and the player is dead, return the poolable
		if (!GameManager.Player) { Return(); return; }
		
		// lerp towards the player (2 lerp speeds based on current distance to player)
		if (Vector3.Distance(transform.position, GameManager.Player.transform.position) > outerTrigger.radius / 2f) {
			body.MovePosition(Vector3.Lerp(transform.position, GameManager.Player.transform.position, 0.1f));
		} else {
			body.MovePosition(Vector3.Lerp(transform.position, GameManager.Player.transform.position, 0.33f));
		}

		// if the inner trigger contains the player, activate the pickup and then return the poolable
		if (innerTrigger.OverlapPoint(GameManager.Player.transform.position)) { 
			PickupEffect(GameManager.Player); Return();
		}
	}

	// Detect pickup trigger interaction
	protected virtual void OnTriggerEnter2D(Collider2D collision) {
		if (collision.FindComponent<PlayerController>()) { lerpToPlayer = true; }
	}
	

	/// <summary>
	/// The effect that will be activated when the pickup is picked up by the specified entity
	/// </summary>
	protected virtual void PickupEffect(Entity entity) { }

	/// <summary>
	/// After the given initialDelay, flash the given color on the given shapes for the given despawnDuration, then return the poolable
	/// </summary>
	protected IEnumerator Despawner(Color color, float initialDelay, float despawnDuration, IEnumerable<ShapeRenderer> shapes)
	{
		yield return new WaitForSeconds(initialDelay);
		foreach (var shape in shapes) { shape.FlashColor(color, 0.38f * despawnDuration); }
		yield return new WaitForSeconds(0.4f * despawnDuration);
		foreach (var shape in shapes) { shape.FlashColor(color, 0.28f * despawnDuration); }
		yield return new WaitForSeconds(0.3f * despawnDuration);
		foreach (var shape in shapes) { shape.FlashColor(color, 0.18f * despawnDuration); }
		yield return new WaitForSeconds(0.2f * despawnDuration);
		foreach (var shape in shapes) { shape.FlashColor(color, 0.08f * despawnDuration); }
		yield return new WaitForSeconds(0.1f * despawnDuration);
		Return();
	}

	/// <summary>
	/// Activate the poolable with the given position and rotation
	/// </summary>
	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		transform.position = position; transform.rotation = rotation; 
		gameObject.SetActive(true); return this;
	}

	/// <summary>
	/// Reset, deactivate, and return the poolable
	/// </summary>
	public override Poolable Return()
	{
		lerpToPlayer = false; gameObject.SetActive(false); 
		pool.queue.Enqueue(this); return this;
	}

}