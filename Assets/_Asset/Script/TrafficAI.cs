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

    static string[] SoundComingName = { Config.SOUND_TrainComing, Config.SOUND_CarComing, Config.SOUND_AirplaneComing, Config.SOUND_ShipComing };

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        SoundController.Instance.PlayMusic(SoundComingName[(int)type]);
    }

    public void MoveToNextDestination(Vector3 InPosition)
    {
        agent.SetDestination(InPosition);
    }
}
