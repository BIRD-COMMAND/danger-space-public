using Extensions;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A pickup that activates bullet time when picked up.
/// </summary>
public class Pickup_BulletTime : Pickup
{

	/// <summary>
	/// How long bullet time lasts when the pickup effect is activated.
	/// </summary>
	public float bulletTimeDuration = 5f;

	protected override void Start()
	{
		base.Start();
		// Start a coroutine that will despawn this pickup after a certain amount of time
		StartCoroutine(Despawner(Color.white, 5f, 3f, gameObject.FindComponents<Shapes.ShapeRenderer>()));
	}

	/// <summary>
	/// Activates bullet time when picked up. Adds a BulletTimeScaler component to the player if one doesn't already exist.
	/// </summary>
	protected override void PickupEffect(Entity entity)
	{
		if (!entity.FindComponent<BulletTimeScaler>()) { entity.AddComponent<BulletTimeScaler>(); }
		GameManager.BulletTime_Start(bulletTimeDuration);
	}

}