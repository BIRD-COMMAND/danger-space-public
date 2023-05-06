using UnityEngine;

/// <summary>
/// A Drop is a serializable reference to a prefab with an associated spawnChance.<br/>
/// The Entity class has a Drop[] field that is used to determine what drops are available (typically spawned when the Entity dies).
/// </summary>
[System.Serializable]
public class Drop
{

	/// <summary>
	/// The prefab to spawn for this Drop.
	/// </summary>
	[Tooltip("The prefab to spawn for this Drop.")]
	public GameObject prefab;

	/// <summary>
	/// The chance for an object to spawn. 0 is a 0% change, 1 is a 100% chance.
	/// </summary>
	[Range(0f, 1f), Tooltip("The chance for an object to spawn. 0 is a 0% change, 1 is a 100% chance.")]
	public float spawnChance;

	/// <summary>
	/// Try to spawn the prefab at the given Transform based on the spawnChance
	/// </summary>
	public bool TrySpawn(Transform transform)
	{
		if (Random.value <= spawnChance) { 
			PoolManager.Get(prefab).Activate(transform.position, transform.rotation);
			return true; 
		}
		return false;
	}

}