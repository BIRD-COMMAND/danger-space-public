using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Agent))]
public class VelocityMatch : MonoBehaviour
{
    public float facingCosine = 90;
    public float timeToTarget = 0.1f;
    public float maxAcceleration = 4f;

    float facingCosineVal;

    Agent agent;

    void Awake()
    {
        facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);
        agent = GetComponent<Agent>();
    }

    public Vector3 GetSteering(ICollection<Agent> targets)
    {
        Vector3 accel = Vector3.zero;
        int count = 0;

        foreach (Agent r in targets)
        {
            if (agent.IsFacing(r.Position, facingCosineVal))
            {
                /* Calculate the acceleration we want to match this target */
                Vector3 a = r.Velocity - agent.Velocity;
                /* Rather than accelerate the character to the correct speed in 1 second, 
                    * accelerate so we reach the desired speed in timeToTarget seconds 
                    * (if we were to actually accelerate for the full timeToTarget seconds). */
                a = a / timeToTarget;

                accel += a;

                count++;
            }
        }

        if (count > 0)
        {
            accel = accel / count;

            /* Make sure we are accelerating at max acceleration */
            if (accel.magnitude > maxAcceleration)
            {
                accel = accel.normalized * maxAcceleration;
            }
        }

        return accel;
    }
}
