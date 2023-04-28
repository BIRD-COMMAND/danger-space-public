using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class PoolableTrail : Poolable
{

    [HideInInspector] public TrailRenderer trail;

	private void Awake() { trail = GetComponent<TrailRenderer>(); }

	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		if (isActiveAndEnabled) { return this; }
		transform.SetPositionAndRotation(position, rotation);
		trail.emitting = true;
		trail.Clear();
		gameObject.SetActive(true);
		return this;
	}
	public override Poolable Return()
	{
		if (!isActiveAndEnabled) { return this; }
		trail.emitting = false;
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	public void Detach() { transform.SetParent(pool.transform); }

	private void FixedUpdate() {
		if (transform.parent == pool.transform && trail.positionCount < 2) { Return(); }
	}

}
