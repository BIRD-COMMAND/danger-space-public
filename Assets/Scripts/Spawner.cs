using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner class is responsible for spawning and managing game objects during runtime.
/// </summary>
public class Spawner : MonoBehaviour
{

	/// <summary>
	/// The prefab to spawn.
	/// </summary>
	public GameObject prefab;
	/// <summary>
	/// Minimum number of active spawned game objects.
	/// </summary>
	public int minimumActiveSpawns = 1;
	/// <summary>
	/// The cooldown time between spawns.
	/// </summary>
	public float spawnCooldown = 3f;
	/// <summary>
	/// The initial spawn delay.
	/// </summary>
	public float initialSpawnDelay = 1f;
	/// <summary>
	/// The last spawn time.
	/// </summary>
	private float lastSpawn = 0f;
	/// <summary>
	/// A list of spawned game objects.
	/// </summary>
    private List<GameObject> spawned = new List<GameObject>();

	/// <summary>
	/// Validates prefab and sets initial spawn time
	/// </summary>
	private void Start() { 
		// check for invalid prefab
		if (!prefab) { Debug.LogWarning("Destroying invalid Spawner: no prefab assigned", gameObject); Destroy(this); }
		lastSpawn = -spawnCooldown + initialSpawnDelay;
	}

	/// <summary>
	/// Handles spawning, despawning, and out-of-bounds objects
	/// </summary>
	private void FixedUpdate()
	{
		// remove null references
		spawned.RemoveAll(x => x == null);
		
		// spawn new if necessary
		if (spawned.Count < minimumActiveSpawns && Time.time - lastSpawn > spawnCooldown) {
			spawned.Add(Instantiate(prefab, transform.position, prefab.transform.rotation));
			lastSpawn = Time.time;
		}
		
		// reset out-of-bounds objects
		foreach (GameObject item in spawned) {
			if (!ScreenTrigger.Contains(item.transform.position) && !ScreenBoundary.Contains(item.transform.position)) {
				item.transform.position = transform.position;
			}
		}

	}

	/// <summary>
	/// Draws a spawn point indicator ring
	/// </summary>
	private void OnDrawGizmos()
	{
		Shapes.Draw.Thickness = 0.3f; Shapes.Draw.Ring(transform.position, 3f, Color.red);
	}

}