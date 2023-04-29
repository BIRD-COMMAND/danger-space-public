using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Health : Pickup
{

	public float healValue = 3f;

	protected override void PickupEffect(Entity entity) {
		entity.Heal(healValue, null);
	}

}
