using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float duration = 5f;

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(Time.time, duration) / duration);
	}

	private void OnDrawGizmosSelected()
	{
		Draw.Thickness = 0.3f; 
        Draw.UseDashes = true;
		Draw.Line(startPosition, endPosition, Color.white);
        Draw.UseDashes = false;
        Draw.Ring(startPosition, 3f, Color.green);
		Draw.Ring(endPosition, 3f, Color.red);
	}

    [ContextMenu("Set Start Position")]
    private void SetStartPosition() { startPosition = transform.position; }

    [ContextMenu("Set End Position")]
    private void SetEndPosition() { endPosition = transform.position; }

}
