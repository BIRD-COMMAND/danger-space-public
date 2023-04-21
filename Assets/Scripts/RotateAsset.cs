using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAsset : MonoBehaviour
{

    private Transform imageAsset;
    public float rotationAmount;
    public float interval = 1.0f;
    private float currRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = interval;
        imageAsset = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currRotation += rotationAmount;
        Debug.Log("Rotationg: " + currRotation);
        imageAsset.rotation = Quaternion.Euler(0f, 0f, currRotation);
    }
}
