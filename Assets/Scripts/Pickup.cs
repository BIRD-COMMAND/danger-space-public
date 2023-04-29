using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Poolable
{

    protected CircleCollider2D innerTrigger;
    protected CircleCollider2D outerTrigger;

	private bool lerpToPlayer = false;

	protected virtual void Start() { SetupTriggers(); }

	protected void SetupTriggers()
	{
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

	protected virtual void PickupEffect(Entity entity) { }

	private void FixedUpdate()
	{
		if (lerpToPlayer && GameManager.Player) { 
			if (Vector3.Distance(transform.position, GameManager.Player.transform.position) > outerTrigger.radius / 2f) {
				transform.position = Vector3.Lerp(transform.position, GameManager.Player.transform.position, 0.1f);
			}
			else {
				transform.position = Vector3.Lerp(transform.position, GameManager.Player.transform.position, 0.33f);
			}
			if (innerTrigger.OverlapPoint(GameManager.Player.transform.position)) { 
				PickupEffect(GameManager.Player.GetComponent<Entity>()); Return();
			} 
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision) {
		if (collision.FindComponent(out PlayerController player)) { lerpToPlayer = true; }
	}

	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		transform.position = position; transform.rotation = rotation; 
		gameObject.SetActive(true); return this;
	}

	public override Poolable Return()
	{
		lerpToPlayer = false; gameObject.SetActive(false); 
		pool.queue.Enqueue(this); return this;
	}
}
