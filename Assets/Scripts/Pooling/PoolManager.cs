using Extensions;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PoolManager is a class for managing object pools in the game.
/// </summary>
public class PoolManager : MonoBehaviour
{

	private static PoolManager instance;
	
	private Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

	/// <summary>
	/// Awake method is called when the script instance is being loaded.
	/// Initializes the object pools for various game objects.
	/// </summary>
	private void Awake() {
		instance = this;
		CreatePoolsForResources("Prefabs/Particles",	"ParticlesPool",	20);
		CreatePoolsForResources("Prefabs/Pickups",		"PickupsPool",		20);
		CreatePoolsForResources("Prefabs/Projectiles",	"ProjectilesPool",	60);
		CreatePoolsForResources("Prefabs/Ships",		"ShipsPool",		04);
		CreatePoolsForResources("Prefabs/Trails",		"TrailsPool",		60);				
	}

	/// <summary>
	/// Creates object pools for prefabs found at the specified path in the Resources folder.
	/// </summary>
	/// <param name="resourcePath">The path in the Resources folder containing the prefabs.</param>
	/// <param name="poolName">The name of the parent game object for the pool.</param>
	/// <param name="initialCapacity">The initial capacity of the pool.</param>
	private void CreatePoolsForResources(string resourcePath, string poolName, int initialCapacity)
	{
		Transform poolRoot = new GameObject(poolName).transform;
		poolRoot.parent = transform;

		foreach (GameObject prefab in Resources.LoadAll<GameObject>(resourcePath)) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			pools[prefab] = CreateObjectPool(prefab, "_" + poolName, initialCapacity);
			pools[prefab].transform.parent = poolRoot;
		}
	}

	/// <summary>
	/// Creates an object pool for the specified prefab.
	/// </summary>
	/// <param name="prefab">The prefab to create the object pool for.</param>
	/// <param name="suffix">The suffix to append to the pool's game object name.</param>
	/// <param name="initialCapacity">The initial capacity of the pool.</param>
	/// <returns>The created ObjectPool.</returns>
	public ObjectPool CreateObjectPool(GameObject prefab, string suffix, int initialCapacity)
	{
		ObjectPool pool = new GameObject(suffix, typeof(ObjectPool)).GetComponent<ObjectPool>();
		pool.prefab = prefab; pool.Initialize(initialCapacity);
		return pool;
	}

	/// <summary>
	/// Gets an instance of the specified prefab from its object pool.
	/// </summary>
	/// <param name="prefab">The prefab to get an instance of.</param>
	/// <returns>A Poolable instance of the specified prefab.</returns>
	public static Poolable Get(GameObject prefab) { return instance.MGet(prefab); }
	private Poolable MGet(GameObject prefab)
	{
		if (!prefab || !pools.ContainsKey(prefab)) { return null; }
		return pools[prefab].Get();
	}

	/// <summary>
	/// Returns true if the specified prefab has an object pool.
	/// </summary>
	public static bool CanGet(GameObject prefab) {
		return prefab && instance.pools.ContainsKey(prefab);
	}

}