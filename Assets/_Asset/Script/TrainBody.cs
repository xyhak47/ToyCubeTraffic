using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrainBody : MonoBehaviour
{
    Queue<Vector3> dests_q = new Queue<Vector3>();

    public void Follow(GameObject parent)
    {
        NavMeshAgent agentParent = parent.GetComponent<NavMeshAgent>();

        dests_q.Enqueue(agentParent.destination);

       // NavMeshAgent agent = GetComponent<NavMeshAgent>();

        //agent.SetPath(agentParent.path);
        // agent.SetDestination(agentParent.destination);
    }

    void Update()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (dests_q.Count != 0 && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= float.Epsilon)
        { 
            Vector3 dest = dests_q.Dequeue();
            agent.SetDestination(dest);
        }
    }
}
