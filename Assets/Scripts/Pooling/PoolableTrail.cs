using UnityEngine;

/// <summary>
/// PoolableTrail is a class representing a poolable object for trail renderers.<br/>
/// Inherits from the Poolable abstract class.
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class PoolableTrail : Poolable
{

	/// <summary>
	/// Reference to the TrailRenderer component of the poolable object
	/// </summary>
	[HideInInspector] public TrailRenderer trail;
	private void Awake() { trail = GetComponent<TrailRenderer>(); }

	/// <summary>
	/// Activates the poolable trail, setting its position, rotation, and enabling the trail emission.
	/// </summary>
	/// <param name="position">The new position of the poolable object.</param>
	/// <param name="rotation">The new rotation of the poolable object.</param>
	/// <returns>The activated poolable trail.</returns>
	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		if (isActiveAndEnabled) { return this; }
		transform.SetPositionAndRotation(position, rotation);
		trail.emitting = true;
		trail.Clear();
		gameObject.SetActive(true);
		return this;
	}

	/// <summary>
	/// Returns the poolable trail to its object pool, stops trail emission and deactivates the object.
	/// </summary>
	/// <returns>The returned poolable trail.</returns>
	public override Poolable Return()
	{
		if (!isActiveAndEnabled) { return this; }
		trail.emitting = false;
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	/// <summary>
	/// Detaches the poolable trail object from its current parent and sets the object pool as its new parent.
	/// </summary>
	public void Detach() { transform.SetParent(pool.transform); }

	/// <summary>
	/// Checks if the trail should be returned to the object pool.
	/// </summary>
	private void FixedUpdate() {
		if (transform.parent == pool.transform && trail.positionCount < 2) { Return(); }
	}

}