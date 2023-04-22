using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Projectiles : MonoBehaviour
{

	private static Projectiles instance;
	
	private Dictionary<GameObject, Queue<Projectile>> queuesByPrefab = new Dictionary<GameObject, Queue<Projectile>>();
	private Dictionary<Queue<Projectile>, GameObject> prefabsByQueue = new Dictionary<Queue<Projectile>, GameObject>();
	private Dictionary<string, Queue<Projectile>> queuesByName = new Dictionary<string, Queue<Projectile>>();
	private Transform pool;

	private void Awake() {
		instance = this; pool = new GameObject("ProjectilePool").transform; pool.SetParent(transform);
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Projectiles")) { InitializePool(prefab); }
	}

	public static Projectile Get(GameObject prefab) { return instance.MGet(prefab); }
	public static Projectile Get(string name) { return instance.MGet(name); }	
	private Projectile MGet(GameObject prefab)
	{
		if (!prefab || !queuesByPrefab.ContainsKey(prefab)) { return DequeueProjectile(queuesByName["Default"]); }
		return DequeueProjectile(queuesByPrefab[prefab]);
	}
	private Projectile MGet(string name)
	{
		if (name == null || !queuesByName.ContainsKey(name)) { return DequeueProjectile(queuesByName["Default"]); }
		return DequeueProjectile(queuesByName[name]);
	}
	private Projectile DequeueProjectile(Queue<Projectile> queue)
	{
		if (queue.Count > 0) { return queue.Dequeue().Activate(); }
		else { return Instantiate(prefabsByQueue[queue], pool).GetComponent<Projectile>().Initialize(queue).Activate(); }
	}

	private void InitializePool(GameObject prefab)
	{
		if (!prefab || !prefab.GetComponent<Projectile>()) { return; }
		Queue<Projectile> prefabPool = new Queue<Projectile>();
		queuesByPrefab.Add(prefab, prefabPool);
		prefabsByQueue.Add(prefabPool, prefab);
		queuesByName.Add(prefab.name, prefabPool);
		for (int i = 0; i < 20; i++) { 
			prefabPool.Enqueue(
				Instantiate(prefab, pool)
				.GetComponent<Projectile>()
				.Initialize(prefabPool)
			);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Projectile projectile)) {
			StartCoroutine(ReturnProjectile(projectile));
		}
	}
	private WaitForSeconds returnDelay = new WaitForSeconds(0.5f);
	private System.Collections.IEnumerator ReturnProjectile(Projectile projectile) { yield return returnDelay; projectile.Return(); }

}