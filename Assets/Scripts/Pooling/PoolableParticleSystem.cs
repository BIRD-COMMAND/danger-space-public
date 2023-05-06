using UnityEngine;

/// <summary>
/// PoolableParticleSystem is a class representing a poolable object for particle systems.<br/>
/// Inherits from the Poolable abstract class.
/// </summary>
public class PoolableParticleSystem : Poolable
{

	/// <summary>
	/// Reference to the ParticleSystem component of the poolable object
	/// </summary>
	[HideInInspector] public ParticleSystem particles;
	private void Awake() { particles = GetComponent<ParticleSystem>(); }

	/// <summary>
	/// Activates the poolable particle system, setting its position, rotation, and playing the particles.
	/// </summary>
	/// <param name="position">The new position of the poolable object.</param>
	/// <param name="rotation">The new rotation of the poolable object.</param>
	/// <returns>The activated poolable particle system.</returns>
	public override Poolable Activate(Vector3 position, Quaternion rotation)
	{
		transform.SetPositionAndRotation(position, rotation);
		particles.Stop();
		gameObject.SetActive(true);
		particles.Play(true);
		return this;
	}

	/// <summary>
	/// Returns the poolable particle system to its object pool, stops the particles and deactivates the object.
	/// </summary>
	/// <returns>The returned poolable particle system.</returns>
	public override Poolable Return()
	{
		if (!isActiveAndEnabled || !particles || pool.queue == null) { return this; }
		particles.Stop();
		gameObject.SetActive(false);
		pool.queue.Enqueue(this);
		return this;
	}

	/// <summary>
	/// OnParticleSystemStopped is called when the ParticleSystem stops playing.<br/>
	/// Returns the poolable particle system to its object pool.
	/// </summary>
	private void OnParticleSystemStopped() { Return(); }

}