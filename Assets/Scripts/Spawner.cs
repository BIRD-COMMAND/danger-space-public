using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject prefab;
    public int minimumActiveSpawns = 1;
	public float spawnCooldown = 3f;
	public float initialSpawnDelay = 1f;
	private float lastSpawn = 0f;
    private List<GameObject> spawned = new List<GameObject>();

	private void Start() { 
		if (!prefab) { Debug.LogWarning("Destroying invalid Spawner: no prefab assigned", gameObject); Destroy(this); }
		lastSpawn = -spawnCooldown + initialSpawnDelay;
	}

	private void FixedUpdate()
	{
		spawned.RemoveAll(x => x == null);
		if (spawned.Count < minimumActiveSpawns && Time.time - lastSpawn > spawnCooldown) {
			spawned.Add(Instantiate(prefab, transform.position, prefab.transform.rotation));
			lastSpawn = Time.time;
		}
	}

	//private void OnDrawGizmos()
	//{
	//	if (useTargetPositionOverride) {
	//		Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
	//		Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
	//		Shapes.Draw.Line(transform.position, targetPositionOverride);
	//	}
	//}

}
