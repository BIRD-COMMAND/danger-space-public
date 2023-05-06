using UnityEngine;
using Extensions;

/// <summary>
/// A pickup that restores Energy
/// </summary>
public class Pickup_Energy : Pickup
{

	/// <summary>
	/// The amount of energy restored when picked up.
	/// </summary>
	[Tooltip("The amount of energy restored when picked up.")]
	public float energyRestored = 50f;

	/// <summary>
	/// Start a coroutine that will despawn this pickup after a certain amount of time
	/// </summary>
	protected override void Start()
	{
		base.Start();
		StartCoroutine(Despawner(Color.white, 5f, 3f, gameObject.FindComponents<Shapes.ShapeRenderer>()));
	}

	/// <summary>
	/// Restores energy to the player when picked up.
	/// </summary>
	protected override void PickupEffect(Entity entity) {
		if (entity is PlayerController) { (entity as PlayerController).CurrentEnergy += energyRestored; }
	}

}