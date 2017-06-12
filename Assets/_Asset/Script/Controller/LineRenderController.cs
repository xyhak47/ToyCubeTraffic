using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MST;
using Enums;
using System;

public class LineRenderController : MonoBehaviour
{
    public static LineRenderController Instance = null;
    LineRenderController()
    {
        Instance = this;
    }

    [System.Serializable]
    public class LineType
    {
        public AI_TYPE type;
        public GameObject prefab;
    }

    public List<LineType> List_LineType;


    [SerializeField]
    private GameObject Prefab_NavMesh;

    private int SmoothAmountTimeForPath = 8;
    private int SmoothAmountTimeAI = 2;

    private Dictionary<AI_TYPE, List<TrafficLine>> Map = new Dictionary<AI_TYPE, List<TrafficLine>>();


    private static int[] NavMeshWidth = { 10, 10, 10, 10 };

    public void BuildNewTrafficLine(AI_TYPE InType, Vector3[] InPositions, bool SmoothPath)
    {
        // lazy load Prefab_TrafficLine
        List<TrafficLine> List_TrafficLine;
        if (!Map.TryGetValue(InType, out List_TrafficLine))
        {
            List_TrafficLine = new List<TrafficLine>();
            Map.Add(InType, List_TrafficLine);
        }

        LineType line = List_LineType.Find(it => it.type == InType);

        GameObject t = Instantiate(line.prefab) as GameObject;
        t.transform.parent = this.gameObject.transform;
        TrafficLine trafficLine = t.GetComponent<TrafficLine>();
        trafficLine.type = InType; // need to be set before addnewposition
        List_TrafficLine.Add(trafficLine);

        Vector3[] SmoothPathPostions = InPositions;
        Vector3[] NavmeshPostions = InPositions;
        if (SmoothPath)
        {
            SmoothPathPostions = GenerateSmoothPath(InPositions, SmoothAmountTimeForPath);
            NavmeshPostions = GenerateSmoothPath(InPositions, SmoothAmountTimeAI);
        }

        // Draw curve path
        trafficLine.DrawCurve(SmoothPathPostions);

        // Use bridge mesh as nav mesh
        BridgeController.Instance.SendBridgeMaker(InType, NavmeshPostions);
    }

    public void Clear(AI_TYPE InType)
    {
        NavMeshSourceTag.UnCollect(InType);

        List<TrafficLine> List_TrafficLine;
        if (!Map.TryGetValue(InType, out List_TrafficLine))
        {
            return;
        }

        foreach (var t in List_TrafficLine) Destroy(t.gameObject);
        List_TrafficLine.Clear();
    }

    public void ClearAll()
    {
        foreach (var type in Map.Keys) Clear(type);
    }

    private Vector3[] GenerateSmoothPath(Vector3[] InPositions, int InSmoothAmount)
    {
        Vector3[] vector3s = iTween.PathControlPointGenerator(InPositions);

        int SmoothAmount = InPositions.Length * InSmoothAmount;
        Vector3[] OutPositions = new Vector3[SmoothAmount + 1];

        for (int i = 0; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            OutPositions[i] = iTween.Interp(vector3s, pm);
        }

        return OutPositions;
    }
}