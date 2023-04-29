using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundary : MonoBehaviour
{

    private static ScreenBoundary instance;

    private new CompositeCollider2D collider;
    public BoxCollider2D innerBounds;
    public BoxCollider2D outerBounds;

	private void Awake() { 
        instance = this; 
        collider = GetComponent<CompositeCollider2D>(); 
    }

	// Start is called before the first frame update
	void Start()
    {
        innerBounds.size = ScreenTrigger.Collider.size;
        outerBounds.size = innerBounds.size * 2f;
    }

	public static bool Contains(Vector2 position) { 
        return instance.collider.OverlapPoint(position); 
    }

}
