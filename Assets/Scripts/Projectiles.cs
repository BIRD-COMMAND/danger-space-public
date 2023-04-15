using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Projectile Manager", order = 1), 
	FilePath("Assets/Prefabs/Projectiles/ProjectileManager.so", FilePathAttribute.Location.ProjectFolder)]
public class Projectiles : ScriptableSingleton<Projectiles>
{

	private Dictionary<GameObject, Queue<Projectile>> queuesByPrefab = new Dictionary<GameObject, Queue<Projectile>>();
	private Dictionary<Queue<Projectile>, GameObject> prefabsByQueue = new Dictionary<Queue<Projectile>, GameObject>();
	private Dictionary<string, Queue<Projectile>> queuesByName = new Dictionary<string, Queue<Projectile>>();
	private Transform pool;

	private void OnBegin() {
		pool = new GameObject("ProjectilePool").transform;
		queuesByPrefab.Clear(); prefabsByQueue.Clear(); queuesByName.Clear();
		foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Projectiles")) { InitializePool(prefab); }
	}
	private void OnEnd() { }

#if UNITY_EDITOR
	protected void OnEnable() { EditorApplication.playModeStateChanged += OnPlayStateChange; }
	protected void OnDisable() { EditorApplication.playModeStateChanged -= OnPlayStateChange; }
	void OnPlayStateChange(PlayModeStateChange state) {
		if (state == PlayModeStateChange.EnteredPlayMode) { OnBegin(); }
		else if (state == PlayModeStateChange.ExitingPlayMode) { OnEnd(); }
	}
#else
        protected void OnEnable() { OnBegin(); } 
        protected void OnDisable() { OnEnd(); }
#endif

	public static Projectile Get(Module m) { return Get(m.projectile); }
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

}