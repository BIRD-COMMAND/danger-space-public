using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : Entity
{

	private void Start() { invulnerable = true; }

	public override void OnIncomingDamage(float damage, Entity source)
	{
		if (GameManager.Player) { 
			GameManager.Player.RemoteDamage(damage, source);
			if (GameManager.Player && GameManager.Player.Health > 0) { FlashColor(Color.red); }
		}
	}

	public override void OnIncomingHealing(float healing, Entity source)
	{
		if (GameManager.Player) { GameManager.Player.RemoteHealing(healing, source); }
	}

}
