using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

	public static Zone Fallback => instance.fallback;
	public static Zone General1 => instance.general1;
	public static Zone General2 => instance.general2;
	public static Zone General3 => instance.general3;
	public static Zone General4 => instance.general4;
	public static Zone Support1 => instance.support1;
	public static Zone Support2 => instance.support2;
	public static Zone Support3 => instance.support3;
	public static Zone Support4 => instance.support4;
	public static Zone PlayerFront => instance.playerFront;
	public static Zone PlayerBack => instance.playerBack;
	public static Zone PlayerLeft => instance.playerLeft;
	public static Zone PlayerRight => instance.playerRight;

	public static ZoneManager instance;

	public static Zone GetZone(Zone.Type t) => instance[t];

	public Zone this[Zone.Type t] { get {
			switch (t) {
				default: case Zone.Type.Rally: return rally;
				case Zone.Type.Fallback: return fallback;
				case Zone.Type.General1: return general1;
				case Zone.Type.General2: return general2;
				case Zone.Type.General3: return general3;
				case Zone.Type.General4: return general4;
				case Zone.Type.Support1: return support1;
				case Zone.Type.Support2: return support2;
				case Zone.Type.Support3: return support3;
				case Zone.Type.Support4: return support4;
				case Zone.Type.PlayerFront:  return playerFront;
				case Zone.Type.PlayerBack:   return playerBack;
				case Zone.Type.PlayerLeft:   return playerLeft;
				case Zone.Type.PlayerRight:  return playerRight;
			}
		}
	}

	public bool visualize = true;

	public Zone rally = new Zone();
	public Zone fallback = new Zone();
	public Zone general1 = new Zone();
	public Zone general2 = new Zone();
	public Zone general3 = new Zone();
	public Zone general4 = new Zone();
	public Zone support1 = new Zone();
	public Zone support2 = new Zone();
	public Zone support3 = new Zone();
	public Zone support4 = new Zone();
	public Zone playerFront = new Zone();
	public Zone playerBack = new Zone();
	public Zone playerLeft = new Zone();
	public Zone playerRight = new Zone();

	private void Awake() { instance = this; }

	private float rotation;
	private void Update()
	{
		// keep player zones relative to the player
		if (PlayerController.player) {
			rotation = Mathf.Deg2Rad * PlayerController.player.Rotation;
			playerFront.position = PlayerController.player.transform.UnitsForward(40f);		playerFront.rotation = rotation;
			playerBack.position =  PlayerController.player.transform.UnitsBackward(40f);	playerBack.rotation =  rotation;
			playerLeft.position =  PlayerController.player.transform.UnitsLeft(40f);		playerLeft.rotation =  rotation;
			playerRight.position = PlayerController.player.transform.UnitsRight(40f);		playerRight.rotation = rotation;
		}

	}

	private void OnDrawGizmos()
	{

		if (!visualize) { return; }

		Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle; Shapes.Draw.DashSnap = Shapes.DashSnapping.EndToEnd;
		Shapes.Draw.DashSizeUniform = 8f; Shapes.Draw.Thickness = 0.3f; Shapes.Draw.DashSpace = Shapes.DashSpace.Relative;

		if (rally.active)	 { rally.	Draw(Color.green);	 }
		if (fallback.active) { fallback.Draw(Color.white);	 }
		if (general1.active) { general1.Draw(Color.yellow);	 }
		if (general2.active) { general2.Draw(Color.yellow);	 }
		if (general3.active) { general3.Draw(Color.yellow);	 }
		if (general4.active) { general4.Draw(Color.yellow);	 }
		if (support1.active) { support1.Draw(Color.magenta); }
		if (support2.active) { support2.Draw(Color.magenta); }
		if (support3.active) { support3.Draw(Color.magenta); }
		if (support4.active) { support4.Draw(Color.magenta); }
		if (playerFront.active) { playerFront. Draw(Color.red); }
		if (playerBack.active)  { playerBack.  Draw(Color.red); }
		if (playerLeft.active)  { playerLeft.  Draw(Color.red); }
		if (playerRight.active) { playerRight. Draw(Color.red); }

	}

}
