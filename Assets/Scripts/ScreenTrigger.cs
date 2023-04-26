using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(BoxCollider2D))]
public class ScreenTrigger : MonoBehaviour
{

	public static BoxCollider2D Collider => instance.screenTrigger;
	private BoxCollider2D screenTrigger;
	private static ScreenTrigger instance;

	private void Awake() { 
		instance = this;
		if (this.FindComponent(out Camera cam) && cam.orthographic) {
			float height = 2.0f * cam.orthographicSize;
			float width = height * cam.aspect;
			screenTrigger = GetComponent<BoxCollider2D>();
			screenTrigger.size = new Vector2(width, height);
		}
	}

	public static bool IsOnScreen(Vector2 position) {
		return instance.screenTrigger.OverlapPoint(position);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		
		if (!isActiveAndEnabled) { return; }
		
		// Return() Projectiles to their pool when they go off screen
		if (collision.FindComponent(out Projectile projectile)) { 
			projectile.Return(); //Debug.Log("Projectile Return()ed by ProjectileReturnTrigger"); 
		}

	}

}
