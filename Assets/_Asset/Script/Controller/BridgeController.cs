using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class BridgeController : MonoBehaviour
{
    public static BridgeController Instance = null;
    BridgeController()
    {
        Instance = this;
    }

    [SerializeField]
    private GameObject Prefab_BridgeMaker;

    [System.Serializable]
    public class BridgeData
    {
        public GameObject Prefab;
        public AI_TYPE type;
    }

    public List<BridgeData> List_BridgeData;

    private float BridgeDistanceLimit = 20f;

    private Dictionary<AI_TYPE, List<GameObject>> Map_Maker = new Dictionary<AI_TYPE, List<GameObject>>();
    private Dictionary<AI_TYPE, List<GameObject>> Map_Bridge = new Dictionary<AI_TYPE, List<GameObject>>();


    public void SendBridgeMaker(AI_TYPE InType, Vector3[] InPositions)
    {
        for(int i = 0; i < InPositions.Length - 1; i++)
        {
            SendBridgeMaker(InType, InPositions[i], InPositions[i + 1]);
        }
    }

    private void SendBridgeMaker(AI_TYPE InType, Vector3 InBeginPosition, Vector3 InEndPosition)
    {
        Vector3 center = (InBeginPosition + InEndPosition) / 2;
        GameObject bridgeMaker = Instantiate(Prefab_BridgeMaker, center, Quaternion.identity);
        bridgeMaker.transform.parent = transform;
        bridgeMaker.GetComponent<BridgeMaker>().type = InType;
        bridgeMaker.GetComponent<BridgeMaker>().Begin = InBeginPosition;
        bridgeMaker.GetComponent<BridgeMaker>().End = InEndPosition;

        bridgeMaker.GetComponent<NavMeshSourceTag>().type = InType;
        bridgeMaker.GetComponent<NavMeshSourceTag>().CollectMeshs();

        ResetTransform(bridgeMaker, InBeginPosition, InEndPosition);

        
        // Lazy 
        List<GameObject> makers;
        if (!Map_Maker.TryGetValue(InType, out makers))
        {
            makers = new List<GameObject>();
            Map_Maker.Add(InType, makers);
        }

        makers.Add(bridgeMaker);
    }

    public void BuildNewBridge(AI_TYPE InType, Vector3 InPosition, Quaternion InRotation)
    {
        // Lazy
        List<GameObject> bridges;
        if (!Map_Bridge.TryGetValue(InType, out bridges))
        {
            bridges = new List<GameObject>();
            Map_Bridge.Add(InType, bridges);
        }

        if (AllowedToBuild(InType, bridges, InPosition))
        {
            GameObject Prefab_Bridge = List_BridgeData.Find(b => b.type == InType).Prefab;

            GameObject bridge = Instantiate(Prefab_Bridge, InPosition, InRotation);
            bridge.transform.parent = gameObject.transform;

            bridges.Add(bridge);
        }
    }

    private void ResetTransform(GameObject InBridge, Vector3 InBeginPosition, Vector3 InEndPosition)
    {
        Vector3 direction = InEndPosition - InBeginPosition;
        float distance = Vector3.Distance(InBeginPosition, InEndPosition);

        // Scale
       // InBridge.transform.localScale = new Vector3(distance, 200, 0.0001f);
        InBridge.transform.localScale = new Vector3(distance + 4, 1, 10);
        InBridge.GetComponent<BoxCollider>().size = new Vector3(distance/ (distance + 4), 50, 0.0001f);

        // Rotate
        float angle = Vector3.Angle(direction, Vector3.right);
        angle = direction.z > 0 ? -angle : angle;

        InBridge.transform.Rotate(Vector3.up, angle);
    }

    // --------------------Clear------------------------
    public void ClearMakers(AI_TYPE InType)
    {
        List<GameObject> makers;
        if (!Map_Maker.TryGetValue(InType, out makers))
        {
            makers = new List<GameObject>();
            Map_Maker.Add(InType, makers);
        }

        foreach (var m in makers) Destroy(m.gameObject);
        makers.Clear();
    }

    public void ClearBridges(AI_TYPE InType)
    {
        List<GameObject> bridges;
        if (!Map_Bridge.TryGetValue(InType, out bridges))
        {
            bridges = new List<GameObject>();
            Map_Bridge.Add(InType, bridges);
        }

        foreach (var b in bridges) Destroy(b.gameObject);
        bridges.Clear();
    }

    public void ClearAllBridges()
    {
        foreach(var bridges in Map_Bridge.Values)
        {
            foreach (var b in bridges)
            {
                Destroy(b.gameObject);
            }
            bridges.Clear();
        }
        Map_Bridge.Clear();
    }

    public void Clear(AI_TYPE InType)
    {
        ClearBridges(InType);
        ClearMakers(InType);
    }
    // ----------------------------------------

    private bool AllowedToBuild(AI_TYPE InType, List<GameObject> InBridges, Vector3 InPosition)
    {
        return true;


        //GameObject bridge = InBridges.Find(b => (Vector3.Distance(b.transform.position, InPosition) <= BridgeDistanceLimit));
        //if (bridge != null)
        //{
        //    bridge.transform.position = (bridge.transform.position + InPosition) / 2;
        //    return false;
        //}

        //return true;
    }
}
