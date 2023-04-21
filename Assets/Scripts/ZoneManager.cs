using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

	public static ZoneManager instance;

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
				case Zone.Type.Player1:  return player1;
				case Zone.Type.Player2:  return player2;
				case Zone.Type.Player3:  return player3;
				case Zone.Type.Player4:  return player4;
				case Zone.Type.Player5:  return player5;
				case Zone.Type.Player6:  return player6;
				case Zone.Type.Player7:  return player7;
				case Zone.Type.Player8:  return player8;
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
	public Zone player1 = new Zone();
	public Zone player2 = new Zone();
	public Zone player3 = new Zone();
	public Zone player4 = new Zone();
	public Zone player5 = new Zone();
	public Zone player6 = new Zone();
	public Zone player7 = new Zone();
	public Zone player8 = new Zone();

	private readonly Vector3 p1 = new Vector3(0f, 30f, 0f);
	private readonly Vector3 p2 = new Vector3(18.5f, 18.5f, 0f);
	private readonly Vector3 p3 = new Vector3(30f, 0f, 0f);
	private readonly Vector3 p4 = new Vector3(18.5f, -18.5f, 0f);
	private readonly Vector3 p5 = new Vector3(0f, -30f, 0f);
	private readonly Vector3 p6 = new Vector3(-18.5f, -18.5f, 0f);
	private readonly Vector3 p7 = new Vector3(-30f, 0f, 0f);
	private readonly Vector3 p8 = new Vector3(-18.5f, 18.5f, 0f);

	private void Awake() { instance = this; }

	private void Update()
	{
		// keep player zones relative to the player
		if (PlayerController.player) {
			player1.position = PlayerController.player.transform.position + p1;
			player2.position = PlayerController.player.transform.position + p2;
			player3.position = PlayerController.player.transform.position + p3;
			player4.position = PlayerController.player.transform.position + p4;
			player5.position = PlayerController.player.transform.position + p5;
			player6.position = PlayerController.player.transform.position + p6;
			player7.position = PlayerController.player.transform.position + p7;
			player8.position = PlayerController.player.transform.position + p8;
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
		if (player1.active)  { player1. Draw(Color.red);	 }
		if (player2.active)  { player2. Draw(Color.red);	 }
		if (player3.active)  { player3. Draw(Color.red);	 }
		if (player4.active)  { player4. Draw(Color.red);	 }
		if (player5.active)  { player5. Draw(Color.red);	 }
		if (player6.active)  { player6. Draw(Color.red);	 }
		if (player7.active)  { player7. Draw(Color.red);	 }
		if (player8.active)  { player8. Draw(Color.red);	 }

	}

}
