using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic Object Pool for Poolable objects. This class handles the creation and management of <br/>
/// a pool of objects, allowing for efficient reuse of frequently instantiated and destroyed objects.
/// </summary>
public class ObjectPool : MonoBehaviour
{
	
	/// <summary>
	/// The prefab to be used for creating new objects in the pool
	/// </summary>
	public GameObject prefab;

	/// <summary>
	/// A queue to store the available Poolable objects
	/// </summary>
	[HideInInspector] public Queue<Poolable> queue = new Queue<Poolable>();

	/// <summary>
	/// Retrieves an available object from the pool.
	/// If there are no available objects, a new one is created.
	/// </summary> <returns>An available Poolable object.</returns>
	public Poolable Get()
	{
		// If the queue is empty, create a new object and add it to the pool
		if (queue.Count == 0) { 
			Instantiate(prefab, transform)
				.GetComponent<Poolable>().Initialize(this);
		}
		// Return the next available object from the queue
		return queue.Dequeue();
	}

}