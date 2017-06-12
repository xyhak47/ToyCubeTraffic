using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using MST;
using Enums;

public class TrafficStation : MonoBehaviour
{
    [System.NonSerialized]
    public int Id;

    [System.NonSerialized]
    public Graph ParentGraph;

    [System.NonSerialized]
    public AI_TYPE type;

    [System.NonSerialized]
    public Vector3 Offset = new Vector3(0, 0, 0);

    void Start()
    {
        transform.position += Offset;
    }


    void OnTriggerEnter(Collider other)
    {
        TrafficAI AI = other.gameObject.GetComponent<TrafficAI>();
        if(AI && AI.type == type)
        {
            Vector3 dest = GetRandomDestination(AI);

            AI.MoveToNextDestination(dest);
        }
    }

    private Vector3 GetRandomDestination(TrafficAI InAI)
    {
        var Positions_Map = StationContorller.Instance.Map_Positions;
        List<Edge> edges = ParentGraph.adj(Id);

        edges.Sort((Left, Right) => { return Left.WeightAsRoad - Right.WeightAsRoad; });
        Edge minWeightEdge = edges[0];
        minWeightEdge.WeightAsRoad++;

        return Positions_Map[type][minWeightEdge.End];
    }

    public void SetColor(Color InColor)
    {
       // GetComponent<MeshRenderer>().material.SetColor("_Color", InColor);
    }

    private void ResetTransform(GameObject In, Vector3 InBegin, Vector3 InEnd)
    {
        Vector3 direction = InBegin - InEnd;
        float distance = Vector3.Distance(InBegin, InEnd);

        // Rotate
        float angle = Vector3.Angle(direction, Vector3.right);
        angle = direction.z > 0 ? -angle : angle;

        In.transform.Rotate(Vector3.up, angle);
    }
}
