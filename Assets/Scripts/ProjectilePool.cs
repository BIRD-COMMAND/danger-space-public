using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

	// Singleton implementation
	private static ProjectilePool _instance;
	public static ProjectilePool Instance {
		get {
			if (_instance == null) { _instance = FindFirstObjectByType<ProjectilePool>(); }
			return _instance;
		}
	}

	private static Projectile next;

	[SerializeField] private GameObject cannonShellPrefab;
	private static Queue<Projectile> cannonShellPool = new Queue<Projectile>();

	private void Awake() { 
		if (!_instance) { _instance = this; InitializePools(); }
		else { Destroy(gameObject); }		 
	}

	public static Projectile GetProjectile(Module m)
	{
		if (!m.projectile) { return null; }
		if (m.projectile.gameObject == Instance.cannonShellPrefab) { return CannonShell; }
		return null;
	}

	private void InitializePools()
	{
		for (int i = 0; i < 20; i++) { 
			cannonShellPool.Enqueue(Instantiate(cannonShellPrefab, transform).GetComponent<Projectile>().Initialize(cannonShellPool)); 
		}
	}

	public static Projectile CannonShell {
		get { next = null;
			while (!next) {
				if (cannonShellPool.Count > 0) { next = cannonShellPool.Dequeue(); }
				else { next = Instantiate(Instance.cannonShellPrefab, Instance.transform).GetComponent<Projectile>().Initialize(cannonShellPool); }
			} return next.Activate(); 
		}
	}

}