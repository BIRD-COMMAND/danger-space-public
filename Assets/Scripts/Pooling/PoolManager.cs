using Extensions;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PoolManager creates, populates, and manages ObjectPools for the different types of prefabs in the game.
/// </summary>
public class PoolManager : MonoBehaviour
{

	/// <summary>
	/// A singleton instance of the PoolManager.
	/// </summary>
	private static PoolManager instance;
	
	/// <summary>
	/// A dictionary of ObjectPools, keyed by the prefab they are associated with.
	/// </summary>
	private Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

	/// <summary>
	/// Awake method is called when the script instance is being loaded.
	/// Initializes the object pools for various game objects.
	/// </summary>
	private void Awake() {
		instance = this;
		CreatePoolsForResources("Prefabs/Particles",	"Particle",		20);
		CreatePoolsForResources("Prefabs/Pickups",		"Pickup",		20);
		CreatePoolsForResources("Prefabs/Projectiles",	"Projectile",	60);
		CreatePoolsForResources("Prefabs/Ships",		"Ship",			04);
		CreatePoolsForResources("Prefabs/Trails",		"Trail",		60);				
	}

	/// <summary>
	/// Creates object pools for prefabs found at the specified path in the Resources folder.
	/// </summary>
	/// <param name="resourcePath">The path in the Resources folder containing the prefabs.</param>
	/// <param name="poolType">The name of the parent game object for the pool.</param>
	/// <param name="initialCapacity">The initial capacity of the pool.</param>
	private void CreatePoolsForResources(string resourcePath, string poolType, int initialCapacity)
	{
		Transform poolRoot = new GameObject(poolType + "Pool").transform;
		poolRoot.parent = transform;

		foreach (GameObject prefab in Resources.LoadAll<GameObject>(resourcePath)) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			pools[prefab] = CreateObjectPool(prefab, poolType + "_" + prefab.name, initialCapacity);
			pools[prefab].transform.parent = poolRoot;
		}
	}

	/// <summary>
	/// Creates an object pool for the specified prefab.
	/// </summary>
	/// <param name="prefab">The prefab to create the object pool for.</param>
	/// <param name="name">The suffix to append to the pool's game object name.</param>
	/// <param name="initialCapacity">The initial capacity of the pool.</param>
	/// <returns>The created ObjectPool.</returns>
	public ObjectPool CreateObjectPool(GameObject prefab, string name, int initialCapacity)
	{
		ObjectPool pool = new GameObject(name, typeof(ObjectPool)).GetComponent<ObjectPool>();
		pool.prefab = prefab;
		for (int i = 0; i < initialCapacity; i++) { 
			Instantiate(prefab, pool.transform)
				.GetComponent<Poolable>().Initialize(pool);
		}
		return pool;
	}

	/// <summary>
	/// Gets an instance of the specified prefab from its object pool.
	/// </summary>
	/// <param name="prefab">The prefab to get an instance of.</param>
	/// <returns>A Poolable instance of the specified prefab.</returns>
	public static Poolable Get(GameObject prefab) { return CanGet(prefab) ? instance.pools[prefab].Get() : null; }

	/// <summary>
	/// Returns true if the specified prefab exists and has an object pool.
	/// </summary>
	public static bool CanGet(GameObject prefab) { return prefab && instance.pools.ContainsKey(prefab); }

}