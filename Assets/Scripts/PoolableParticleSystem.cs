using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticleSystem : Poolable
{

	[HideInInspector] public ParticleSystem particles;
	private void Awake() { particles = GetComponent<ParticleSystem>(); }

	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		transform.SetPositionAndRotation(position, rotation);
		particles.Stop();
		gameObject.SetActive(true);
		particles.Play(true);
		return this;
	}
	
	public override Poolable Return()
	{
		if (!isActiveAndEnabled || !particles || pool.queue == null) { return this; }
		particles.Stop();
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	private void OnParticleSystemStopped() { Return(); }

}
