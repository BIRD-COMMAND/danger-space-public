using UnityEngine;

[System.Serializable]
public class Drop
{
	
	public GameObject prefab;
	
	[Range(0f, 1f)]
	public float spawnChance;

	public bool TrySpawn(Transform transform)
	{
		if (Random.value < spawnChance) { 
			PoolManager.Get(prefab).Activate(transform.position, transform.rotation);
			return true; 
		}
		return false;
	}

}