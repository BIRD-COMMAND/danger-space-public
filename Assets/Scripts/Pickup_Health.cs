using Extensions;
using UnityEngine;

public class Pickup_Health : Pickup
{

	public float healValue = 3f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(Despawner(Color.white, 5f, 3f, gameObject.FindComponents<Shapes.ShapeRenderer>()));
	}

	protected override void PickupEffect(Entity entity) {
		entity.Heal(healValue, null);
	}

}
