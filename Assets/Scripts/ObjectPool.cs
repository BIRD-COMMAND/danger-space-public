using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic Object Pool for Poolables
/// </summary>
public class ObjectPool : MonoBehaviour
{

	public GameObject prefab;
	[HideInInspector] public Queue<Poolable> queue = new Queue<Poolable>();

	public void Initialize(int initialQuantity = 1)
	{
		gameObject.name = prefab.name + gameObject.name;
		for (int i = 0; i < initialQuantity; i++) {
			Instantiate(prefab, transform).GetComponent<Poolable>().Initialize(this);
		}
	}

	public Poolable Get()
	{
		if (queue.Count == 0) { Instantiate(prefab, transform).GetComponent<Poolable>().Initialize(this); }
		return queue.Dequeue();
	}

}
