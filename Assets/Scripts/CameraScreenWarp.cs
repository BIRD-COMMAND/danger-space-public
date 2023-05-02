using Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// CameraScreenWarp handles warping the player to the other side of the screen whenever they go off screen.<br/>
/// It also manages the dummy player objects that visualize the player's simultaneous position across the screen.
/// </summary>
public class CameraScreenWarp : MonoBehaviour
{


	/// <summary>
	/// The player-dummy that will be used as a base to create the other dummies.
	/// </summary>
	[Tooltip("The player-dummy that will be used as a base to create the other dummies.")]
	public GameObject dummy;
	/// <summary>
	/// The screen trigger, a trigger BoxCollider2D that will be used to detect when the player goes off screen.
	/// </summary>
	[Tooltip("The screen trigger, a trigger BoxCollider2D that will be used to detect when the player goes off screen.")]
	public BoxCollider2D screenTrigger;
	
	private GameObject player;
	private GameObject[] dummies = new GameObject[8];

	private Vector2 screenSize;
	private Vector3[] dummyOffsets;


	void Start() {
		
		// calculate screen size
		if (TryGetComponent(out Camera cam) && cam.orthographic) {
			float height = 2.0f * cam.orthographicSize;
			float width = height * cam.aspect;
			screenSize = new Vector2(width, height);
		}

		// create a pool object for the player dummies and spawn all the dummies
		GameObject dummiesParent = new GameObject("PlayerDummies");
		dummiesParent.transform.SetParent(transform);
		dummiesParent.transform.SetPositionAndRotation(transform.position, transform.rotation);
		dummies[0] = dummy; dummy.transform.parent = dummiesParent.transform;
		for (int i = 1; i < 8; i++) { dummies[i] = Instantiate(dummy, dummiesParent.transform); }

		// set the offsets used for positioning dummies
		dummyOffsets = new Vector3[]
		{
			new Vector3(-screenSize.x, -screenSize.y, 0),
			new Vector3(-screenSize.x, 0, 0),
			new Vector3(-screenSize.x, screenSize.y, 0),
			new Vector3(0, -screenSize.y, 0),
			new Vector3(0, screenSize.y, 0),
			new Vector3(screenSize.x, -screenSize.y, 0),
			new Vector3(screenSize.x, 0, 0),
			new Vector3(screenSize.x, screenSize.y, 0)
		};

		// update all dummy positions
		Update();

	}

	private void Update()
	{

		// spawn destruction effect and hide dummies when player is dead
		if (!GameManager.Player) { FakeDestroyDummies(); return; }
		else { ReactivateDummies(); }
		
		player = GameManager.Player.gameObject;

		// set the position and rotation for all dummies and dummy children
		for (int i = 0; i < dummies.Length; i++) {
			
			dummies[i].transform.position = player.transform.position + dummyOffsets[i];
			dummies[i].transform.rotation = player.transform.rotation;

			for (int j = 0; j < dummies[i].transform.childCount; j++) {
				dummies[i].transform.GetChild(j).localPosition = player.transform.GetChild(j).localPosition;
				dummies[i].transform.GetChild(j).rotation = player.transform.GetChild(j).rotation;
			}

		}
			
		// handle warping the player
		for (int i = 0; i < dummies.Length; i++) {

			// if the screenTrigger ever contains a dummy, it's time to teleport the player to that position
			if (screenTrigger.OverlapPoint(dummies[i].transform.position)) {
				
				Vector3 endPos = dummies[i].transform.position;
				
				// this *elegant* code handles the frustrating task of swapping the player's and dummies' trail renderers 
				// so none of them get cut off or unnecessarily teleported across the screen, creating an erroneous trail
				
				// you might think that disabling the TrailRenderers and teleporting them along with the dummies would be easier,
				// but that actually won't work due to the way Unity's TrailRenderer is implemented

				if (player.GetComponentInChildren<TrailRenderer>(true)) {
					TrailRenderer trail, dummyTrail, oppositeTrail;
					trail = player.GetComponentInChildren<TrailRenderer>();
					dummyTrail = dummies[i].GetComponentInChildren<TrailRenderer>();
					oppositeTrail = dummies[7 - i].GetComponentInChildren<TrailRenderer>();
					if (trail && dummyTrail && oppositeTrail) {
						Vector2 localPosition = trail.transform.localPosition;
						trail.transform.SetParent(null);
						dummyTrail.transform.SetParent(null);
						oppositeTrail.transform.SetParent(null);
						oppositeTrail.Clear();
						oppositeTrail.emitting = false;
						player.transform.position = endPos;
						dummyTrail.transform.SetParent(player.transform);
						dummyTrail.transform.localPosition = localPosition;
						Update();
						trail.transform.SetParent(dummies[7 - i].transform);
						oppositeTrail.transform.SetParent(dummies[i].transform);
						trail.transform.localPosition = localPosition;
						oppositeTrail.transform.localPosition = localPosition;
						StartCoroutine(ReactivateTrail(oppositeTrail));
						break;
					}
				}

				// actually teleport the player
				player.transform.position = endPos;

				// run update again to update all dummy positions
				Update();

			}
		}

	}


	// during player screen warping, at least one TrailRenderer needs to be teleported
	// across the screen, and disabling it for at least two frames is the only way to
	// avoid it creating a line across the screen when it's re-enabled
	// that's what these are for
	private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
	private System.Collections.IEnumerator ReactivateTrail(TrailRenderer trail) { 
		yield return endOfFrame; yield return endOfFrame; trail.emitting = true; 
	}


	/// <summary>
	/// Tracks whether dummies have been hidden already when the Player is dead
	/// </summary>
	private bool dummiesHidden = false;
	/// <summary>
	/// Play the player's destroy effect and hide the dummies
	/// </summary>
	private void FakeDestroyDummies()
	{
		if (dummiesHidden) { return; }
		for (int i = 0; i < 8; i++) {
			PoolManager
				.Get(dummies[i].GetComponent<Entity>().DestroyEffectPrefab)
				.Activate(dummies[i].transform.position, dummies[i].transform.rotation);
			dummies[i].SetActive(false);
		}
		dummiesHidden = true;
	}
	/// <summary>
	/// Reactivate the dummies
	/// </summary>
	private void ReactivateDummies()
	{
		if (!dummiesHidden) { return; }
		for (int i = 0; i < 8; i++) { dummies[i].SetActive(true); }
		dummiesHidden = false;
	}

}