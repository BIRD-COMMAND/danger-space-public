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
	[Tooltip("The prefab to spawn.")]
	public GameObject prefab;
	/// <summary>
	/// Minimum number of active spawned game objects.
	/// </summary>
	[Tooltip("Minimum number of active spawned game objects.")]
	public int minimumActiveSpawns = 1;
	/// <summary>
	/// Minimum number of active spawned game objects.
	/// </summary>
	public int MinimumActiveSpawns { get => minimumActiveSpawns; set => minimumActiveSpawns = value; }
	/// <summary>
	/// Sets minimum number of active spawned game objects.
	/// </summary>
	public void SetMinimumActiveSpawns(float spawns) { minimumActiveSpawns = (int)spawns; }
	/// <summary>
	/// The cooldown time between spawns.
	/// </summary>
	[Tooltip("The cooldown time between spawns.")]
	public float spawnCooldown = 3f;
	/// <summary>
	/// The cooldown time between spawns.
	/// </summary>
	public float SpawnCooldown { get => spawnCooldown; set => spawnCooldown = value; }
	/// <summary>
	/// The initial spawn delay.
	/// </summary>
	[Tooltip("The initial spawn delay.")]
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

		if (GameManager.EditMode) { return; }

		// remove invalid references
		spawned.RemoveAll(x => !x || !x.activeSelf);
		
		// spawn new if necessary
		if (spawned.Count < minimumActiveSpawns && Time.time - lastSpawn > spawnCooldown) {
			Entity ship = (Entity)PoolManager.Get(prefab);
			spawned.Add(ship.gameObject);
			ship.Activate(transform.position, prefab.transform.rotation);
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
	/// Draws visual indicators for the spawner
	/// </summary>
	public void EditModeDrawSpawner()
	{
		Shapes.Draw.Ring(transform.position, 1f, 0.6f, Color.white);
		if (TryGetComponent(out LerpPosition lerp)) {
			Shapes.Draw.Ring(lerp.startPosition, 0.3f, 0.6f, Color.white);
			Shapes.Draw.Ring(lerp.endPosition, 0.3f, 0.6f, Color.white);
			Shapes.Draw.UseDashes = true;
			Shapes.Draw.Line(lerp.startPosition, lerp.endPosition, 0.6f, Color.white);
			Shapes.Draw.UseDashes = false;
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