using Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraScreenWarp : MonoBehaviour
{

	public GameObject player;
	public GameObject dummy;
	public BoxCollider2D screenTrigger;
	public GameObject[] dummies = new GameObject[8];

	private Vector2 screenSize;
	private Vector3[] dummyOffsets;

	// Start is called before the first frame update
	void Start() {
		if (GetComponent<Camera>() && GetComponent<Camera>().orthographic) {
			float height = 2.0f * GetComponent<Camera>().orthographicSize;
			float width = height * GetComponent<Camera>().aspect;
			screenSize = new Vector2(width, height);
		}
		dummies[0] = dummy;
		for (int i = 1; i < 8; i++) { dummies[i] = Instantiate(dummy, transform); }
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
		Update();
	}

	private void Update()
	{

		if (!GameManager.Player) { return; }
		player = GameManager.Player.gameObject;

		for (int i = 0; i < dummies.Length; i++) {
			
			dummies[i].transform.position = player.transform.position + dummyOffsets[i];
			dummies[i].transform.rotation = player.transform.rotation;

			for (int j = 0; j < dummies[i].transform.childCount; j++) {
				dummies[i].transform.GetChild(j).localPosition = player.transform.GetChild(j).localPosition;
				dummies[i].transform.GetChild(j).rotation = player.transform.GetChild(j).rotation;
			}

		}
				
		for (int i = 0; i < dummies.Length; i++) {
			if (screenTrigger.OverlapPoint(dummies[i].transform.position)) {
				Vector3 endPos = dummies[i].transform.position;
				TrailRenderer trail = null, dummyTrail = null,  oppositeTrail = null;
				if (player.GetComponentInChildren<TrailRenderer>(true)) {
					trail = player.GetComponentInChildren<TrailRenderer>(); //.Where(t => t.emitting).FirstOrDefault();
					dummyTrail = dummies[i].GetComponentInChildren<TrailRenderer>();
					oppositeTrail = dummies[7 - i].GetComponentInChildren<TrailRenderer>();
					if (trail && dummyTrail && oppositeTrail) {
						//dummies[i].transform.position = transform.position.WithZ(-20f);
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
				//dummies[i].transform.position = transform.position.WithZ(-20f);
				player.transform.position = endPos;
				Update();
			}
		}

	}

	private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
	private System.Collections.IEnumerator ReactivateTrail(TrailRenderer trail) { 
		yield return endOfFrame; yield return endOfFrame; trail.emitting = true; 
	}

}
