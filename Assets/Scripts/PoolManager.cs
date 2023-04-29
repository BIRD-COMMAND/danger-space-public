using Extensions;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

	private static PoolManager instance;
	
	private Dictionary<GameObject, ObjectPool> queues = new Dictionary<GameObject, ObjectPool>();

	private void Awake() {
		instance = this;
		Transform particlesPoolRoot = new GameObject("ParticlesPool").transform;
		particlesPoolRoot.parent = transform;
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Particles")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ParticlesPool", 20);
			queues[prefab].transform.parent = particlesPoolRoot;
		}
		Transform pickupsPoolRoot = new GameObject("PickupsPool").transform;
		pickupsPoolRoot.parent = transform;
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Pickups")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_PickupsPool", 20);
			queues[prefab].transform.parent = pickupsPoolRoot;
		}
		Transform projectilesPoolRoot = new GameObject("ProjectilesPool").transform;
		projectilesPoolRoot.parent = transform;
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Projectiles")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ProjectilesPool", 60);
			queues[prefab].transform.parent = projectilesPoolRoot;
		}
		Transform shipsPoolRoot = new GameObject("ShipsPool").transform;
		shipsPoolRoot.parent = transform;
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Ships")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ShipsPool", 20);
			queues[prefab].transform.parent = shipsPoolRoot;
		}
		Transform trailsPoolRoot = new GameObject("TrailsPool").transform;
		trailsPoolRoot.parent = transform;
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Trails")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_TrailsPool", 60);
			queues[prefab].transform.parent = trailsPoolRoot;
		}
	}

	public ObjectPool CreateObjectPool(GameObject prefab, string suffix, int initialCapacity)
	{
		ObjectPool pool = new GameObject(suffix, typeof(ObjectPool)).GetComponent<ObjectPool>();
		pool.prefab = prefab; pool.Initialize(initialCapacity);
		return pool;
	}

	public static Poolable Get(GameObject prefab) { return instance.MGet(prefab); }
	private Poolable MGet(GameObject prefab)
	{
		if (!prefab || !queues.ContainsKey(prefab)) { return null; }
		return queues[prefab].Get();
	}

}