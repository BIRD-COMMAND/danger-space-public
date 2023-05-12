using UnityEngine;

/// <summary>
/// Poolable is an abstract class representing a poolable object, which can be instantiated, reused and returned to an object pool.
/// </summary>
public abstract class Poolable : MonoBehaviour
{

	/// <summary> Reference to the object pool that this poolable object belongs to. </summary>
	[HideInInspector] public ObjectPool pool;

	/// <summary> Initializes the poolable object by setting its object pool, enqueuing it into the pool, and deactivating it. </summary>
	/// <param name="pool">The object pool to assign this poolable object to.</param>
	/// <returns>The initialized poolable object.</returns>
	public Poolable Initialize(ObjectPool pool) { 
		this.pool = pool; pool.queue.Enqueue(this); gameObject.SetActive(false); return this; 
	}
	
	/// <summary> Activates the poolable object, setting its position and rotation. </summary>
	/// <param name="position">The new position of the poolable object.</param>
	/// <param name="rotation">The new rotation of the poolable object.</param>
	/// <returns>The activated poolable object.</returns>
	public abstract Poolable Activate(Vector3 position, Quaternion rotation);
	
	/// <summary> Deactivates the poolable object, resets its state, and returns the poolable object to its object pool. </summary>
	/// <returns>The returned/deactivated poolable object.</returns>
	public abstract Poolable Return();

}