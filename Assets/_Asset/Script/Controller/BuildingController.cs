using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance = null;
    BuildingController()
    {
        Instance = this;
    }

    Dictionary<AI_TYPE, List<GameObject>> Map_Building = new Dictionary<AI_TYPE, List<GameObject>>();

    public void BuildLittleStuff(AI_TYPE InType)
    {
        if (!NeedToBuild(InType)) return;

        List<GameObject> List_Building;
        if(!Map_Building.TryGetValue(InType, out List_Building))
        {
            List_Building = new List<GameObject>();
            Map_Building.Add(InType, List_Building);
        }
        else
        {
            foreach (var b in List_Building) Destroy(b);
            List_Building.Clear();
        }

        string stuffPath = Config.Folder_BuildingStuff + GetResFloder(InType) + RandomName(InType);
        GameObject prefab = Resources.Load(stuffPath) as GameObject;


        //Dictionary<AI_TYPE, List<TrafficStation>> Map_Stations = StationContorller.Instance.Map_Stations;
        //List<TrafficStation> list = Map_Stations[InType];

        Dictionary<AI_TYPE, List<GameObject>> map = BridgeController.Instance.Map_Maker;
        List<GameObject> list = map[InType];


        for (int i = 0; i < list.Count; i += 5)
        {
            GameObject item = list[i];

            Vector3 pos1 = item.gameObject.transform.position + RandomPositionOffset();
            GameObject stuff = Instantiate(prefab, pos1, Quaternion.identity);
            stuff.transform.parent = transform;
            List_Building.Add(stuff);

            Vector3 pos2 = item.gameObject.transform.position - RandomPositionOffset();
            stuff = Instantiate(prefab, pos2, prefab.transform.rotation);
            stuff.transform.parent = transform;
            List_Building.Add(stuff);
        }
    }

    private string GetResFloder(AI_TYPE InType)
    {
        if (InType == AI_TYPE.TRAIN) return "Tree/";
        else if (InType == AI_TYPE.CAR) return "Tree/";
        else if (InType == AI_TYPE.SHIP) return "Mountain/";
        else return "Tree/";
    }

    private string RandomName(AI_TYPE InType)
    {
        if (InType == AI_TYPE.TRAIN) return Random.Range(0, 6).ToString();
        else if (InType == AI_TYPE.CAR) return Random.Range(0, 6).ToString();
        else if (InType == AI_TYPE.SHIP) return Random.Range(0, 5).ToString();

        return "0";
    }

    private bool NeedToBuild(AI_TYPE InType)
    {
        return InType != AI_TYPE.AIRPLANE;
    }

    private Vector3 RandomPositionOffset()
    {
        return new Vector3(Random.Range(30, 35), 0, Random.Range(30, 35));
    }
}
