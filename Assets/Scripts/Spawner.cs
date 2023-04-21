using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

	public AI.Pattern overridePattern = AI.Pattern.Sine;
	public List<Zone.Type> patrolZones = new List<Zone.Type>();
    public int minimumActiveSpawns = 1;
	public float spawnCooldown = 3f;
	private float lastSpawn;
    public GameObject prefab;
	public bool visualizePath = true;
	public bool useTargetPositionOverride = false;
	public Vector2 targetPositionOverride = Vector2.zero;
    private List<GameObject> spawned = new List<GameObject>();

	private void Start() { 
		if (!prefab) { Debug.LogWarning("Destroying invalid Spawner: no prefab assigned", gameObject); Destroy(this); }
		lastSpawn = -spawnCooldown; 
	}

	private void FixedUpdate()
	{
		spawned.RemoveAll(x => x == null);
		if (spawned.Count < minimumActiveSpawns && Time.time - lastSpawn > spawnCooldown) {
			spawned.Add(Instantiate(prefab, transform.position, prefab.transform.rotation));
			if (spawned[spawned.Count - 1] && spawned[spawned.Count - 1].TryGetComponent(out AI ai)) {
				foreach (Zone.Type item in patrolZones) { ai.patrol.Add(ZoneManager.instance[item]); }
				if (useTargetPositionOverride) { ai.targetPosition = targetPositionOverride; }
				if (visualizePath) { ai.visualizePath = visualizePath; }
				if (overridePattern != AI.Pattern.None) { 
					ai.pattern = overridePattern; ai.path.Clear(); ai.PathSetNextPosition(true); 
				}
			}
			lastSpawn = Time.time;
		}
	}

	private void OnDrawGizmos()
	{
		if (useTargetPositionOverride) {
			Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
			Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
			Shapes.Draw.Line(transform.position, targetPositionOverride);
		}
	}

}
