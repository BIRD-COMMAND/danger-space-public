using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Debug
{

	[RequireComponent(typeof(Camera))]
	public class DebugCameraController : MonoBehaviour
	{

		[Range(1f, 1024f)]
		public int scrollZoomFactor = 512;

		private Camera cam;
		private bool currentlyMousePanning = false;

		// Start is called before the first frame update
		void Start() { cam = GetComponent<Camera>(); }

		// Update is called once per frame
		void Update()
		{

			// Mouse-scroll camera zooming
			if (Mouse.DidScrollY && Mouse.IsInsideGameWindow) { cam.orthographicSize -= Mouse.ScrollY / scrollZoomFactor; }

			// Middle-click camera panning
			// Start middle-click camera panning
			if (!currentlyMousePanning && Mouse.MiddleDown) { currentlyMousePanning = true; }
			// If middle-click is pressed perform camera panning
			else if (Mouse.MiddleDown) { transform.position -= (Vector3)Mouse.DeltaFrame; }
			// If middle-click is not pressed, panning = false
			else { currentlyMousePanning = false; }

		}
	}

}