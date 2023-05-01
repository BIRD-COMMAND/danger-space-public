using Extensions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_BulletTime : Pickup
{

	public float bulletTimeDuration = 5f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(Despawner(Color.white, 5f, 3f, gameObject.FindComponents<Shapes.ShapeRenderer>()));
	}

	protected override void PickupEffect(Entity entity)
	{
		if (!entity.FindComponent<BulletTimeScaler>()) { entity.AddComponent<BulletTimeScaler>(); }
		GameManager.BulletTime_Start(bulletTimeDuration);
	}

}
