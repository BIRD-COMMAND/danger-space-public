using Shapes;
using Extensions;
using System.Collections;
using UnityEngine;

/// <summary>
/// A class for generating 2D flow field data for regions with environmental or other obstructions.
/// </summary>
public class FlowField : MonoBehaviour
{

	private static FlowField instance;

	/// <summary>
	/// The width of the flow field grid in cells.
	/// </summary>
	public int width = 10;
	/// <summary>
	/// The height of the flow field grid in cells.
	/// </summary>
	public int height = 10;
	/// <summary>
	/// The size of each cell in the flow field grid.
	/// </summary>
	public float cellSize = 1f;
	/// <summary>
	/// The radius around each cell for detecting colliders.
	/// </summary>
	public float detectionRadius = 1f;
	/// <summary>
	/// Whether the flow field should be visualized in the scene view.
	/// </summary>
	public bool showFlowField = true;
	
	/// <summary>
	/// The 2D array storing the flow vectors for each cell in the grid.
	/// </summary>
	public Vector2[,] flowField;
	
	/// <summary>
	/// LayerMask for filtering colliders during flow vector calculation
	/// </summary>
	private readonly LayerMask fieldMask = (1 << 3) | (1 << 9);

	// Initialization
	private void Awake() { instance = this; }

	// Generate flow field on start
	void Start() { 
		flowField = new Vector2[width, height];
		StartCoroutine(RegenerateFlowField());
	}

	// Coroutine for periodically regenerating flow field
	private IEnumerator RegenerateFlowField()
	{
		while (Application.isPlaying) {
			GenerateFlowField();
			yield return new WaitForSecondsRealtime(0.2f);
		}
	}

	// Update logic, including visualization if enabled
	private void Update()
	{
		if (showFlowField) {
			using (Draw.Command(Camera.main)) {
				Draw.Ring(Mouse.WorldPosition, 6f, 0.3f, Color.green);
				Debug.DrawLine(Mouse.WorldPosition, Mouse.WorldPosition + M_GetForce(Mouse.WorldPosition, 6f));
			}
		}

	}

	// Temporary arrays for storing colliders and flow vector calculation
	private Collider2D[] colliders;
	private Vector3 flowVector;
	private Rect bounds;

	/// <summary>
	/// Generates the flow field by calculating the flow vector for each cell.
	/// </summary>
	void GenerateFlowField()
	{
		// Calculate bounds of the flow field in world space
		bounds = new Rect(
			transform.position.x - (width * cellSize * 0.5f),
			transform.position.y + (height * cellSize * 0.5f),
			width * cellSize, 
			height * cellSize
		);
		Vector2 localPosition = Vector2.zero, transformPosition = transform.position;
		
		// Loop through all cells in the grid
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				localPosition.x = (x - width / 2) * cellSize;
				localPosition.y = (y - height / 2) * cellSize;
				flowField[x, y] = CalculateFlowVector(localPosition + transformPosition);
			}
		}

		// Visualize flow field if enabled
		if (showFlowField) { DrawFlowField(); }

	}

	/// <summary>
	/// Calculates the flow vector for a cell based on the surrounding colliders.
	/// </summary>
	/// <param name="worldPosition">The world position of the cell.</param>
	/// <returns>The calculated flow vector for the cell.</returns>
	Vector2 CalculateFlowVector(Vector3 worldPosition)
	{
		// Gather colliders within the detection radius around the cell
		colliders = Physics2D.OverlapCircleAll(worldPosition, detectionRadius, fieldMask);
		flowVector = Vector3.zero;
		// Loop through colliders and calculate flow vector
		foreach (Collider2D collider in colliders) {
			if (!collider.isTrigger) {
				if (collider is CompositeCollider2D) {
					flowVector += (collider.transform.position - worldPosition).normalized;
				} else {
					flowVector += (worldPosition - collider.transform.position).normalized;
				}
			}
		}
		return flowVector;
	}

	/// <summary>
	/// Gets the flow field force at a given entity's position and radius.
	/// </summary>
	/// <param name="entity">The entity whose position to check.</param>
	/// <returns>The cumulative flow vector within the entity's radius.</returns>
	public static Vector2 GetForce(Entity entity) { return GetForce(entity.Position, entity.Radius); }
	/// <summary>
	/// Gets the flow field force at a given position and radius.
	/// </summary>
	/// <param name="position">The world space position to check.</param>
	/// <param name="radius">The radius around the position to sample the flow field.</param>
	/// <returns>The cumulative flow vector within the specified radius.</returns>
	public static Vector2 GetForce(Vector2 position, float radius) { return instance.M_GetForce(position, radius); }
	public Vector2 M_GetForce(Vector2 position, float radius)
	{
		
		// Check if the input position is within the flow field bounds
		if (!bounds.Contains(position)) { return Vector2.zero; }

		// Calculate grid coordinates of the center cell
		Vector2 positionNormalized = Rect.PointToNormalized(bounds, position);
		int centerX = Mathf.FloorToInt(positionNormalized.x * width);
		int centerY = Mathf.FloorToInt(1f - (positionNormalized.y * height));

		// Calculate the number of cells to check in each direction based on the radius
		int cellsToCheck = Mathf.CeilToInt(radius / cellSize);

		// Initialize vectors
		Vector2 cumulativeFlowVector = Vector2.zero, cellCenter;

		// Loop through the grid cells within the specified radius
		for (int x = centerX - cellsToCheck; x <= centerX + cellsToCheck; x++) {
			for (int y = centerY - cellsToCheck; y <= centerY + cellsToCheck; y++) {
				// Check if the grid coordinates are within the flow field bounds
				if (x >= 0 && x < width && y >= 0 && y < height) {
					// Calculate the world space position of the current cell's center
					cellCenter = Rect.NormalizedToPoint(bounds, new Vector2(x / (float)width, 1f - (y / (float)height)));
					// Check if the distance between the cell's center and the input position is within the radius
					if (Vector2.Distance(position, cellCenter) <= radius) {
						// Add the flow vector of the current cell to the cumulative flow vector
						cumulativeFlowVector += flowField[x, y];
					}
				}
			}
		}

		return cumulativeFlowVector;
	}

	/// <summary>
	/// Draws the flow field in the scene view for visualization purposes.
	/// </summary>
	void DrawFlowField()
	{
		if (flowField == null) { return; }
		Vector2 worldPosition; float duration = 0.24f * Time.timeScale;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				worldPosition = new Vector2((x - width / 2) * cellSize, (y - height / 2) * cellSize) + (Vector2)transform.position;
				Debug.DrawLine(worldPosition, worldPosition + flowField[x, y] * cellSize * 0.5f, Color.white, duration);
			}
		}
	}

}