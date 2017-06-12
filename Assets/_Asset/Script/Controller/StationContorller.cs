using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MST;
using Enums;

public class StationContorller : MonoBehaviour
{
    public static StationContorller Instance = null;
    StationContorller()
    {
        Instance = this;
    }

    private Dictionary<AI_TYPE, List<TrafficStation> > Map_Stations = new Dictionary<AI_TYPE, List<TrafficStation> >();

    public Dictionary<AI_TYPE, Dictionary<int, Vector3>> Map_Positions = new Dictionary<AI_TYPE, Dictionary<int, Vector3>>();

    public Dictionary<AI_TYPE, AI_LEVEL> Map_Level = new Dictionary<AI_TYPE, AI_LEVEL>();

    private Dictionary<AI_TYPE, Coroutine> Map_AI_Coroutine = new Dictionary<AI_TYPE, Coroutine>();

    private Graph G;

    [System.Serializable]
    public class Station
    {
        public GameObject Prefab;
        public AI_TYPE type;
    }

    public List<Station> List_Station;



    public void BuildNewStations(AI_TYPE InType, Vector3[] InPositions)
    {
        List<Edge> edges = ResetGraphData(InType, InPositions);

        List<int> MainPoints = FindMainPoints(InType);

        ResetAllStations(InType);

        ReDrawLines(InType, edges, MainPoints);

        ResetSendAICoroutine(InType);
    }

    public void BuildNewStation(AI_TYPE InType, Vector3 InPosition)
    {
        List<Edge> edges = ResetGraphData(InType, InPosition);

        List<int> MainPoints = FindMainPoints(InType);

        ResetAllStations(InType);

        ReDrawLines(InType, edges, MainPoints);

        ResetSendAICoroutine(InType);
    }

    private List<Edge> ResetGraphData(AI_TYPE InType, Vector3[] InPositions)
    {
        // Lazy 
        Dictionary<int, Vector3> Positions;
        if (!Map_Positions.TryGetValue(InType, out Positions))
        {
            Positions = new Dictionary<int, Vector3>();
            Map_Positions.Add(InType, Positions);
        };
        Positions.Clear();
        
        foreach(var pos in InPositions) Positions.Add(Positions.Count, pos);

        // Reset Graph, Restore edges of Prim to graph
        G = new Graph(Positions);
        List<Edge> edges = G.Prim();
        G.ResetAdj(edges);

        return edges;
    }

    private List<Edge> ResetGraphData(AI_TYPE InType, Vector3 InPosition)
    {
        // Lazy 
        Dictionary<int, Vector3> Positions;
        if (!Map_Positions.TryGetValue(InType, out Positions))
        {
            Positions = new Dictionary<int, Vector3>();
            Map_Positions.Add(InType, Positions);
        };
        Positions.Add(Positions.Count, InPosition);

        // Reset Graph, Restore edges of Prim to graph
        G = new Graph(Positions);
        List<Edge> edges = G.Prim();
        G.ResetAdj(edges);

        return edges;
    }

    private void ReDrawLines(AI_TYPE InType, List<Edge> edges, List<int> InMainPoints)
    {
        // Clear old lines
        LineRenderController.Instance.Clear(InType);

        // Reset bridge
        BridgeController.Instance.ClearMakers(InType);

        if (InType == AI_TYPE.SHIP) // Water path affect all
        {
            BridgeController.Instance.ClearAllBridges();
        }
        else
        {
            BridgeController.Instance.ClearBridges(InType);
        }

        // Distribute edges
        Dictionary<int, Vector3> Positions = Map_Positions[InType];
        List<Edge> MainEdges = new List<Edge>();
        List<Edge> BranchEdges = new List<Edge>();
        foreach (var e in edges)
        {
            if(NeedSmoothPath(InType) && InMainPoints.Count >= 3 && InMainPoints.Contains(e.Begin) && InMainPoints.Contains(e.End))
            {
                MainEdges.Add(e);
            }
            else
            {
                BranchEdges.Add(e);
            }
        }

        // Set path level
        if (Map_Level.ContainsKey(InType))
        {
            Map_Level[InType] = (AI_LEVEL)(InMainPoints.Count - 1);
        }
        else
        {
            Map_Level.Add(InType, (AI_LEVEL)(InMainPoints.Count - 1));
        }

        // Draw line path
        foreach (Edge e in BranchEdges)
        {
            Vector3[] Line = new Vector3[2];
            Line[0] = Positions[e.Begin];
            Line[1] = Positions[e.End];
             
            LineRenderController.Instance.BuildNewTrafficLine(InType, Line, false);
        }

        // Draw smooth path
        if (MainEdges.Count >= 2&& InMainPoints.Count >=3)
        {
            // Smooth path need 3 points at least
            Vector3[] Paths = new Vector3[InMainPoints.Count];

            for (int i = 0; i < InMainPoints.Count; i++)
            {
                Paths[i] = Positions[InMainPoints[i]];
            }
            LineRenderController.Instance.BuildNewTrafficLine(InType, Paths, true);
        }
    }

    private void ResetAllStations(AI_TYPE InType)
    {
        // Lazy new list of traffic station
        List<TrafficStation> List_TrafficStation;
        if (!Map_Stations.TryGetValue(InType, out List_TrafficStation))
        {
            List_TrafficStation = new List<TrafficStation>();
            Map_Stations.Add(InType, List_TrafficStation);
        }

        // Clear old stations
        ClearStations(List_TrafficStation);

        // Add new stations
        GameObject prefab = List_Station.Find(s => s.type == InType).Prefab;
        for (int i = 0; i < Map_Positions[InType].Count; i++)
        {
            GameObject instance = Instantiate(prefab, Map_Positions[InType][i] + new Vector3(0,2,0), prefab.transform.rotation) as GameObject;
            TrafficStation station = instance.GetComponent<TrafficStation>();
            station.Id = i;
            station.ParentGraph = G;
            station.type = InType;
            station.transform.parent = transform;

            List_TrafficStation.Add(station);
        }
    }

    private IEnumerator SendAI(AI_TYPE InType)
    {
        List<TrafficStation> stations = Map_Stations[InType];

        if (stations != null )
        {
            // ReCycle AI by type
            AIController.Instance.ReCycleAI(InType);

            if (stations.Count >= 2)
            {
                // Reset navgation mesh dynamicly
                yield return LocalNavMeshBuilder.Instance.ResetNavMesh(InType);

                GameObject ParentStation = stations[Random.Range(0, stations.Count)].gameObject;

                // Send new AI
                AIController.Instance.SendAI(InType, Map_Level[InType], ParentStation);
            }
        }
    }
    
    private List<int> FindMainPoints(AI_TYPE InType)
    {
        List<int> points = G.GetSingleAttachedPoint();

        List<List<int>> resultPath = new List<List<int>>();

        foreach(var p1 in points)
        {
            DepthFirstPaths dfs = new DepthFirstPaths(G, p1);

            foreach (var p2 in points)
            {
                if(p1 != p2)
                {
                    List<int> paths =  dfs.pathTo(p2);

                    resultPath.Add(paths);
                }
            }
        }

        resultPath.Sort((Left, Right) => { return Right.Count - Left.Count; });

        return resultPath.Count > 0 ? resultPath[0] : new List<int>();
    }



    private bool NeedSmoothPath(AI_TYPE InType)
    {
        return true;
    }

    public List<TrafficStation> GetTrafficStations(AI_TYPE InType)
    {
        // Lazy new list of traffic station
        List<TrafficStation> List_TrafficStation = null;
        if (!Map_Stations.TryGetValue(InType, out List_TrafficStation))
        {
            List_TrafficStation = new List<TrafficStation>();
            Map_Stations.Add(InType, List_TrafficStation);
        }

        return List_TrafficStation;
    }

    private void ResetSendAICoroutine(AI_TYPE InType)
    {
        ClearMapAICoroutine(InType);

        Coroutine c = StartCoroutine(SendAI(InType));
        if (c != null)  Map_AI_Coroutine.Add(InType, c);
    }

    //--------------------------- Clear---------------------------------------------
    private void ClearMapPositions(AI_TYPE Intype)
    {
        Map_Positions.Remove(Intype);
    }

    //public void ClearMapStationsAll()
    //{
    //    foreach (var Item in Map_Stations.Values) ClearStations(Item);
    //}

    public void ClearMapStations(AI_TYPE InType)
    {
        List<TrafficStation> List_TrafficStation;
        if (!Map_Stations.TryGetValue(InType, out List_TrafficStation))
        {
            return;
        }

        ClearStations(List_TrafficStation);
    }

    private void ClearStations(List<TrafficStation> InList_TrafficStation)
    {
        foreach (var t in InList_TrafficStation) Destroy(t.gameObject);
        InList_TrafficStation.Clear();
    }

    private void ClearMapAICoroutine(AI_TYPE InType)
    {
        Coroutine c = null;
        if (Map_AI_Coroutine.TryGetValue(InType, out c))
        {
            StopCoroutine(c);
            Map_AI_Coroutine.Remove(InType);
        }
    }

    public void Clear(AI_TYPE InType)
    {
        ClearMapStations(InType);
        ClearMapPositions(InType);
        ClearMapAICoroutine(InType);
    }
    // --------------------------------------------
}
