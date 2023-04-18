using Freya;
using Extensions;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnemyController : MonoBehaviour
{

    private Approach approach = null;
    public GameObject target;
    public Approach.Type approachType = Approach.Type.None;
    public float curveFactor = 0.5f;
    public bool targetRight;


	// Start is called before the first frame update
	void Start() { }

    // Update is called once per frame
    void Update()
    {
        
        if (approach == null) {
            if (!target) { return; }
            approach = new Approach(transform, target.transform, approachType)
            { targetRight = targetRight, curveFactor = curveFactor };
        }
        
        approach.Update(true);
        
    }

    [ContextMenu("Reset Approach")]
    private void ResetApproach()
    {
        transform.position = new Vector2(-60f, 30f);
        approach.targetRight = targetRight;
        approach.curveFactor = curveFactor;
        approach.type = approachType;
        approach.Reset();
    }

}
