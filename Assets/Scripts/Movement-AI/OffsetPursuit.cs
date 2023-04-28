using UnityEngine;


[RequireComponent(typeof(Agent))]
public class OffsetPursuit : MonoBehaviour
{
    /// <summary>
    /// Maximum prediction time the pursue will predict in the future
    /// </summary>
    public float maxPrediction = 1f;

    Agent agent;

    void Awake() { agent = GetComponent<Agent>(); }

    public Vector2 GetSteering(Agent target, Vector2 offset)
    {
        Vector2 targetPos;
        return GetSteering(target, offset, out targetPos);
    }

    public Vector2 GetSteering(Agent target, Vector2 offset, out Vector2 targetPos)
    {
        Vector2 worldOffsetPos = target.Position + (Vector2)target.transform.TransformDirection(offset);

        //Debug.DrawLine(transform.position, worldOffsetPos);

        /* Calculate the distance to the offset point */
        Vector3 displacement = worldOffsetPos - (Vector2)transform.position;
        float distance = displacement.magnitude;

        /* Get the character's speed */
        float speed = agent.Velocity.magnitude;

        /* Calculate the prediction time */
        float prediction;
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        /* Put the target together based on where we think the target will be */
        targetPos = worldOffsetPos + target.Velocity * prediction;

        return agent.Arrive(targetPos);
    }
}
