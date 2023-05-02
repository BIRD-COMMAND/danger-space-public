using Extensions;
using UnityEngine;

/// <summary>
/// A pickup that heals the player.
/// </summary>
public class Pickup_Health : Pickup
{

	/// <summary>
	/// The amount of health this pickup heals.
	/// </summary>
	public float healValue = 3f;

	protected override void Start()
	{
		base.Start();
		// Starts a coroutine that begins to despawn the pickup after 5 seconds, and flashes the pickup for 3 seconds
		StartCoroutine(Despawner(Color.white, 5f, 3f, gameObject.FindComponents<Shapes.ShapeRenderer>()));
	}

	/// <summary>
	/// Heals the player or entity when picked up.
	/// </summary>
	protected override void PickupEffect(Entity entity) { entity.Heal(healValue, null); }

}
