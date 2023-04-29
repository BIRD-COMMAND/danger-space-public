using Shapes;
using Extensions;
using System.Collections;
using UnityEngine;

public class FlowField : MonoBehaviour
{

	private static FlowField instance;

	public int width = 10;
	public int height = 10;
	public float cellSize = 1f;
	public float detectionRadius = 1f;
	public bool showFlowField = true;

	public Vector2[,] flowField;
	private readonly LayerMask fieldMask = (1 << 3) | (1 << 9);

	private void Awake() { instance = this; }
	void Start() { GenerateFlowField(); StartCoroutine(RegenerateFlowField()); }
	private IEnumerator RegenerateFlowField()
	{
		while (Application.isPlaying) {
			yield return new WaitForSeconds(0.25f);
			GenerateFlowField();
		}
	}

	private void Update()
	{
		
		//if (Mouse.MiddleClick) { showFlowField = !showFlowField; }

		if (showFlowField) {
			using (Draw.Command(Camera.main)) {
				Draw.Ring(Mouse.WorldPosition, 6f, 0.3f, Color.green);
				Debug.DrawLine(Mouse.WorldPosition, Mouse.WorldPosition + M_GetForce(Mouse.WorldPosition, 6f));
			}
		}

	}

	private Collider2D[] colliders;
	private Vector2 flowVector;

	[ContextMenu("Generate Flow Field")]
	void GenerateFlowField()
	{
		flowField = new Vector2[width, height];
		Vector2 worldPosition, flowVector;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				worldPosition = new Vector2((x - width / 2) * cellSize, (y - height / 2) * cellSize) + (Vector2)transform.position;
				flowVector = CalculateFlowVector(worldPosition);
				flowField[x, y] = flowVector;
			}
		}
	}

	Vector2 CalculateFlowVector(Vector2 worldPosition)
	{
		colliders = Physics2D.OverlapCircleAll(worldPosition, detectionRadius, fieldMask);
		flowVector = Vector2.zero;
		foreach (Collider2D collider in colliders) {
			if (!collider.isTrigger) {
				if (collider is CompositeCollider2D) {
					flowVector += ((Vector2)collider.transform.position - worldPosition).normalized;
				}
				else {
					flowVector += (worldPosition - (Vector2)collider.transform.position).normalized;
				}
			}
		}
		return flowVector;
	}

	public static Vector2 GetForce(Entity entity) { return GetForce(entity.Position, entity.Radius); }
	public static Vector2 GetForce(Vector2 position, float radius) { return instance.M_GetForce(position, radius); }
	public Vector2 M_GetForce(Vector2 position, float radius)
	{
		
		// Calculate grid coordinates of the center cell
		int centerX = Mathf.FloorToInt(Mathf.InverseLerp(transform.position.x - ((width * cellSize) * 0.5f),  transform.position.x + ((width * cellSize) * 0.5f),  position.x) * width);
		int centerY = Mathf.FloorToInt(Mathf.InverseLerp(transform.position.y - ((height * cellSize) * 0.5f), transform.position.y + ((height * cellSize) * 0.5f), position.y) * height);

		// Calculate the number of cells to check in each direction based on the radius
		int cellsToCheck = Mathf.CeilToInt(radius / cellSize);

		// Initialize the cumulative flow vector
		Vector2 cumulativeFlowVector = Vector2.zero;

		// Loop through the grid cells within the specified radius
		for (int x = centerX - cellsToCheck; x <= centerX + cellsToCheck; x++) {
			for (int y = centerY - cellsToCheck; y <= centerY + cellsToCheck; y++) {
				// Check if the grid coordinates are within the flow field bounds
				if (x >= 0 && x < width && y >= 0 && y < height) {
					// Calculate the world space position of the current cell's center
					Vector2 cellCenter = new Vector2(
						Mathf.Lerp(transform.position.x - ((width * cellSize) * 0.5f),  transform.position.x + ((width * cellSize) * 0.5f),  x / (float)width),
						Mathf.Lerp(transform.position.y - ((height * cellSize) * 0.5f), transform.position.y + ((height * cellSize) * 0.5f), y / (float)height)
					);
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

	private void OnDrawGizmos() { if (showFlowField) { DrawFlowField(); } }

	void DrawFlowField()
	{
		if (flowField == null) { return; }
		Vector2 worldPosition;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				worldPosition = new Vector2((x - width / 2) * cellSize, (y - height / 2) * cellSize) + (Vector2)transform.position;
				Debug.DrawLine(worldPosition, worldPosition + flowField[x, y] * cellSize * 0.5f, Color.white);
			}
		}
	}

}