using UnityEngine;


/// <summary>
/// BulletTimeScaler is a utility component intended to enable an Entity to move normally during BulletTime.
/// </summary>
[DisallowMultipleComponent]
public class BulletTimeScaler : MonoBehaviour
{

    /// <summary>
    /// Whether the Entity's velocity was adjusted during Start() to account for BulletTime.
    /// </summary>
    private bool velocityAdjusted = false;

	/// <summary>
	/// Mark the entity as inBulletTime and adjust its velocity to compensate for BulletTime
	/// </summary>
	void Start()
    {
        if (GameManager.BulletTime && TryGetComponent(out Entity entity)) {
            entity.inBulletTime = true;
            entity.Body.velocity /= Time.timeScale; velocityAdjusted = true;
        }
    }

    // during Update we check if BulletTime is still active
    // if BulletTime is no longer active we do three things:
    // - we mark the entity as not inBulletTime
    // - we restore its velocity to normal levels if it was adjusted during Start()
    // - we destroy this component, as it is no longer needed

    void Update()
    {
        if (!GameManager.BulletTime) {
			if (TryGetComponent(out Entity entity)) { 
                entity.inBulletTime = false;
                if (velocityAdjusted) { entity.Body.velocity *= GameManager.BulletTimeFactor; }
            }
			Destroy(this);
        }
    }

}