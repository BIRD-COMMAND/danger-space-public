using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all objects that can be pooled
/// </summary>
public abstract class Poolable : MonoBehaviour
{

	[HideInInspector] public ObjectPool pool;

	public Poolable Initialize(ObjectPool pool) { 
		this.pool = pool; pool.queue.Enqueue(this); gameObject.SetActive(false); return this; 
	}
    public abstract Poolable Activate(Vector3 position, Quaternion rotation);
	public abstract Poolable Return();
}
