using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Enums;

public class TrafficAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [System.NonSerialized]
    public AI_TYPE type;

    [System.NonSerialized]
    public int JustArrivedStationId = -1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
       // agent.radius = 0.06f;
    }

    public void MoveToNextDestination(Vector3 InPosition)
    {
        if (agent.isOnNavMesh == false)
        {
            print("agent.isOnNavMesh == false");
            return;
        }
        agent.Stop(true);
        agent.SetDestination(InPosition);
        agent.Resume();
    }

    public void WaitForResume()
    {
        agent.Stop(true);
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public void Resume(GameObject InNewParent)
    {
        transform.parent = InNewParent.transform;
        agent.Stop(false);
        gameObject.SetActive(true);
    }
}
