using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimator : MonoBehaviour
{
    private bool lerping = false;
    private Vector3 startPosition, newPosition;
    private float startZoom, newZoom, lerpStartTime;

    // Update is called once per frame
    void Update()
    {
        if (!lerping) {
            startPosition = transform.position;
            do { newPosition = (Vector3)(Random.insideUnitCircle * 400f) + new Vector3(0f, 0f, transform.position.z); }
            while (Vector3.Distance(newPosition, startPosition) < 225f || Vector3.Distance(newPosition, startPosition) > 275f);
            startZoom = transform.GetChild(0).localScale.x; 
            newZoom = Random.Range(6f, 10f);
            lerpStartTime = Time.time;
            lerping = true;
        }
        else {
            float t = Mathf.Clamp01((Time.time - lerpStartTime) / 10f);
            transform.position = Vector3.Lerp(startPosition, newPosition, t);
            transform.GetChild(0).localScale = new Vector3(Mathf.Lerp(startZoom, newZoom, t), 1f, 1f);
			transform.GetChild(1).localScale = new Vector3(1f, Mathf.Lerp(startZoom, newZoom, t), 1f);
			if (t == 1f) { lerping = false; }
        }
	}
}
