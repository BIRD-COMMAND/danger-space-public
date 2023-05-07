using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

/// <summary>
/// Basic debug camera controller with middle-click mouse panning and scroll wheel zoom.<br/>
/// Not currently used in the project, but useful when prototyping.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

	public bool panEnabled = true;
	public bool zoomEnabled = true;

	[Range(1f, 1024f)]
	public int scrollZoomFactor = 10;

	private Camera cam;

	private bool currentlyMousePanning = false;
	private Vector2 lastMousePosition;

	void Awake() { cam = GetComponent<Camera>(); }

	private void Start() { lastMousePosition = Mouse.WorldPosition; }

	void Update()
	{

		// Mouse-scroll camera zooming
		if (zoomEnabled && Mouse.DidScrollY && Mouse.IsInsideGameWindow) { cam.orthographicSize -= Mouse.ScrollY / scrollZoomFactor; }

		// Middle-click camera panning
		if (panEnabled) {
			// Start middle-click camera panning
			if (!currentlyMousePanning && Mouse.MiddleDown) { currentlyMousePanning = true; }
			// If middle-click is pressed perform camera panning
			else if (Mouse.MiddleDown) { transform.position -= (Vector3)(Mouse.WorldPosition - lastMousePosition); }
			// If middle-click is not pressed, panning = false
			else { currentlyMousePanning = false; }
		}

		// Record last mouse position
		lastMousePosition = Mouse.WorldPosition;

	}

}