using UnityEngine;
using Extensions;

/// <summary>
/// The ScreenTrigger is a BoxCollider2D that is the size of the screen.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class ScreenTrigger : MonoBehaviour
{
	
	private static ScreenTrigger instance;
	private static Vector2 min, max;

	/// <summary>
	/// The BoxCollider2D for the ScreenTrigger.
	/// </summary>
	public static BoxCollider2D Collider => instance.screenTrigger;
	private BoxCollider2D screenTrigger;

	/// <summary>
	/// Initializes the static instance of the class and sets the ScreenTrigger to the size of the screen.
	/// </summary>
	private void Awake() { 
		instance = this;
		if (this.FindComponent(out Camera cam) && cam.orthographic) {
			float height = 2.0f * cam.orthographicSize;
			float width = height * cam.aspect;
			screenTrigger = GetComponent<BoxCollider2D>();
			screenTrigger.size = new Vector2(width, height);
			min = screenTrigger.bounds.min;
			max = screenTrigger.bounds.max;
		}
	}

	/// <summary>
	/// Draws the bounds of the ScreenTrigger.
	/// </summary>
	public static void DrawScreenBounds()
	{
		using (Shapes.Draw.Command(Camera.main)) {
			Shapes.Draw.UseDashes = true;
			Shapes.Draw.Line(min, new Vector2(min.x, max.y), 0.6f, Color.white);
			Shapes.Draw.Line(min, new Vector2(max.x, min.y), 0.6f, Color.white);
			Shapes.Draw.Line(max, new Vector2(max.x, min.y), 0.6f, Color.white);
			Shapes.Draw.Line(max, new Vector2(min.x, max.y), 0.6f, Color.white);
			Shapes.Draw.UseDashes = false;
		}
	}

	/// <summary>
	/// Returns true if the given position is within the ScreenTrigger.
	/// </summary>
	public static bool Contains(Vector2 position) { 
		return instance.screenTrigger.OverlapPoint(position);
	}
	/// <summary>
	/// Returns true if the given position is within the ScreenTrigger.
	/// </summary>
	public static bool IsOnScreen(Vector2 position) {
		return Contains(position);
	}

	/// <summary>
	/// Handles returning Projectiles to their pool when they go off screen.
	/// </summary>
	private void OnTriggerExit2D(Collider2D collision)
	{
		
		if (!isActiveAndEnabled) { return; }
		
		// Return() Projectiles to their pool when they go off screen
		if (collision.FindComponent(out Projectile projectile)) { 
			projectile.Return(); //Debug.Log("Projectile Return()ed by ProjectileReturnTrigger"); 
		}

	}

}