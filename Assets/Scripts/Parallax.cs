using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    public Transform objectToTrack;
    public float parallaxEffect;
    private float length, startpos;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    void Update()
    {
        float distanceMoved = (objectToTrack.position.x * (1 - parallaxEffect));
        float distance = (objectToTrack.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);

        if(distanceMoved > startpos + length){
            startpos += length;
        } else if(distanceMoved < startpos - length){
            startpos -= length;
        } 

    }
}
