using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Agent))]
public class Cohesion : MonoBehaviour
{
    public float facingCosine = 120f;

    float facingCosineVal;

    private Agent agent;

    void Awake() { agent = GetComponent<Agent>(); facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad); }

    public Vector2 GetSteering(ICollection<Agent> targets)
    {
        Vector2 centerOfMass = Vector2.zero;
        int count = 0;

        /* Sums up everyone's position who is close enough and in front of the character */
        foreach (Agent r in targets)
        {
            if (agent.IsFacing(r.Position, facingCosineVal))
            {
                centerOfMass += r.Position;
                count++;
            }
        }

        if (count == 0)
        {
            return Vector2.zero;
        }
        else
        {
            centerOfMass = centerOfMass / count;

            return agent.Arrive(centerOfMass);
        }
    }
}
