using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeScaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.SlowTime) { Destroy(this); return; }
        if (TryGetComponent(out Entity entity)) {
            entity.inBulletTime = true;
            entity.Body.velocity /= Time.timeScale; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.SlowTime) {
			if (TryGetComponent(out Entity entity)) { 
                entity.inBulletTime = false;
                entity.Body.velocity *= GameManager.SlowTimeFactor; 
            }
			Destroy(this); return;
        }
    }
}
