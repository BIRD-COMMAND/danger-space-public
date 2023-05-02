using Shapes;
using UnityEngine;

/// <summary>
/// LerpPosition class is responsible for smoothly moving a GameObject between two positions in a ping-pong fashion.
/// </summary>
public class LerpPosition : MonoBehaviour
{

	/// <summary>
	/// The starting position of the GameObject.
	/// </summary>
	public Vector3 startPosition;
	/// <summary>
	/// The ending position of the GameObject.
	/// </summary>
	public Vector3 endPosition;
	/// <summary>
	/// The duration of the transition between the startPosition and endPosition.
	/// </summary>
	public float duration = 5f;

	/// <summary>
	/// Update is called once per frame to move the GameObject between startPosition and endPosition.
	/// </summary>
	void Update() {
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(Time.time, duration) / duration);
	}

	/// <summary>
	/// Draws Gizmos in the editor to visualize the startPosition and endPosition.
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		Draw.Thickness = 0.3f; 
        Draw.UseDashes = true;
		Draw.Line(startPosition, endPosition, Color.white);
        Draw.UseDashes = false;
        Draw.Ring(startPosition, 3f, Color.green);
		Draw.Ring(endPosition, 3f, Color.red);
	}

	/// <summary>
	/// Context menu item to set the startPosition to the current position of the GameObject.
	/// </summary>
	[ContextMenu("Set Start Position")]
    private void SetStartPosition() { startPosition = transform.position; }

	/// <summary>
	/// Context menu item to set the endPosition to the current position of the GameObject.
	/// </summary>
	[ContextMenu("Set End Position")]
    private void SetEndPosition() { endPosition = transform.position; }

}