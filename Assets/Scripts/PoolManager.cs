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
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Projectiles")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ProjectilesPool", 60);
		}
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Particles")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ParticlesPool", 20);
		}
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Ships")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_ShipsPool", 20);
		}
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Trails")) {
			if (!prefab.FindComponent<Poolable>()) { continue; }
			queues[prefab] = CreateObjectPool(prefab, "_TrailsPool", 60);
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