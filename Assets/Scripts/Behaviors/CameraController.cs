using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

/// <summary>
/// Basic camera controller with WASD camera movement and E/Q camera zoom.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

	public bool panEnabled = true;
	public bool zoomEnabled = true;

	private Camera cam;
	void Awake() { cam = GetComponent<Camera>(); }

	void Update() {
		
		if (!Application.isFocused) { return; }
		
		// E/Q camera zooming
		if (zoomEnabled) { 
			cam.orthographicSize += Input.GetAxis("CameraZoom") * 50f * Time.deltaTime; 
		}

		// WASD camera movement
		if (panEnabled) {
			cam.transform.Translate(
				Input.GetAxis("Horizontal") * cam.orthographicSize * Time.deltaTime,
				Input.GetAxis("Vertical") * cam.orthographicSize * Time.deltaTime,
				0f
			);
		}

	}

}