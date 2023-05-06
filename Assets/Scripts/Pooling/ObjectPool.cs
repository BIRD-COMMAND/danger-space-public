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
	/// Initializes the object pool with a specified number of instances of the prefab.
	/// </summary>
	/// <param name="initialQuantity">The number of instances to create initially.</param>
	public void Initialize(int initialQuantity = 1)
	{
		// Set the name of the object pool
		gameObject.name = prefab.name + gameObject.name;

		// Instantiate the initial quantity of objects and add them to the pool
		for (int i = 0; i < initialQuantity; i++) {
			Instantiate(prefab, transform).GetComponent<Poolable>().Initialize(this);
		}
	}

	/// <summary>
	/// Retrieves an available object from the pool. If there are no available objects, a new one is created.
	/// </summary>
	/// <returns>An available Poolable object.</returns>
	public Poolable Get()
	{
		// If the queue is empty, create a new object and add it to the pool
		if (queue.Count == 0) { Instantiate(prefab, transform).GetComponent<Poolable>().Initialize(this); }
		// Return the next available object from the queue
		return queue.Dequeue();
	}

}
