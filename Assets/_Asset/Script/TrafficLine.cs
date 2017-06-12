using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Enums;

public class TrafficLine : MonoBehaviour
{
    private List<Vector3> List_Position = new List<Vector3>();

    private LineRenderer lineRenderer;


    [System.NonSerialized]
    public AI_TYPE type;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawCurve(Vector3[] InPositions)
    {
        if(NeedDraw())
        {
            lineRenderer.positionCount = InPositions.Length;
            lineRenderer.SetPositions(InPositions);
        }

        List_Position.AddRange(InPositions);
    }

    public List<Vector3> GetPositions()
    {
        return List_Position;
    }

    public bool NeedDraw()
    {
        return true;
    }
}
