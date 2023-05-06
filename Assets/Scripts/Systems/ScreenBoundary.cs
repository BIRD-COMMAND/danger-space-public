using UnityEngine;

/// <summary>
/// ScreenBoundary class is responsible for maintaining and checking the screen boundaries for game objects.
/// </summary>
public class ScreenBoundary : MonoBehaviour
{
	
    /// <summary>
	/// Singleton instance of the ScreenBoundary class
	/// </summary>
	private static ScreenBoundary instance;

	/// <summary>
	/// CompositeCollider2D component that acts as the screen boundary
	/// </summary>
	private new CompositeCollider2D collider;
	/// <summary>
	/// Inner boundary of the composite2D boundary collider
	/// </summary>
	public BoxCollider2D innerBounds;
	/// <summary>
	/// Outer boundary of the composite2D boundary collider
	/// </summary>
	public BoxCollider2D outerBounds;

	/// <summary>
	/// Initializes the instance of the ScreenBoundary class and the reference to the CompositeCollider2D component.
	/// </summary>
	private void Awake() { instance = this; collider = GetComponent<CompositeCollider2D>(); }

	/// <summary>
	/// Initializes the sizes of the inner and outer bounds.
	/// </summary>
	void Start() { innerBounds.size = ScreenTrigger.Collider.size; outerBounds.size = innerBounds.size * 2f; }

	/// <summary>
	/// Checks if the given position is within the screen boundary.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>True if the position is within the screen boundary, false otherwise.</returns>
	public static bool Contains(Vector2 position) { return instance.collider.OverlapPoint(position); }

}