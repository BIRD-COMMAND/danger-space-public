using UnityEngine;
using System.Collections.Generic;


public class NearSensor : MonoBehaviour
{
    HashSet<Agent> _targets = new HashSet<Agent>();

    public HashSet<Agent> Targets
    {
        get
        {
            //Remove any agents that have been destroyed
            _targets.RemoveWhere(IsNull);
            return _targets;
        }
    }

    static bool IsNull(Agent agent)
    {
        return (agent == null || agent.Equals(null));
    }

    void TryAdd(Component other)
    {
        Agent agent = other.GetComponent<Agent>();
        if (agent) { _targets.Add(agent); }
    }

    void TryRemove(Component other)
    {
        Agent agent = other.GetComponent<Agent>();
        if (agent) { _targets.Remove(agent); }
    }

    void OnTriggerEnter2D(Collider2D other) { TryAdd(other); }
    void OnTriggerExit2D(Collider2D other) { TryRemove(other); }
}
