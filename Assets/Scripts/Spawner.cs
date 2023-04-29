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
		foreach (GameObject item in spawned) {
			if (!ScreenTrigger.Contains(item.transform.position) && !ScreenBoundary.Contains(item.transform.position)) {
				item.transform.position = transform.position;
			}
		}
	}

	private void OnDrawGizmos()
	{
		// draw spawn point indicator ring
		Shapes.Draw.Thickness = 0.3f; Shapes.Draw.Ring(transform.position, 3f, Color.red);
	}

}
