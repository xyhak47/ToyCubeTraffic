using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Enums;


public class AIController : MonoBehaviour
{
    // controller
    public static AIController Instance = null;
    AIController()
    {
        Instance = this;
    }

    private List<AIData> List_AIData = new List<AIData>();

    private Dictionary<AI_TYPE, List<GameObject>> Map_AI = new Dictionary<AI_TYPE, List<GameObject>>();

    private static int[] AI_Level_Limit = { 2, 3, 2, 3 };

    void Awake()
    {
        AttachAIData();
    }

    private GameObject SpawnAI(AI_TYPE InType, AI_LEVEL InLevel, GameObject InParent)
    {
        // for test
        InLevel = (AI_LEVEL)Mathf.Min((int)InLevel, AI_Level_Limit[(int)InType]);

        AIData AIToSpawn = List_AIData.Find(it => InType == it.type && InLevel == it.level);
        string prefabName = AIData.AI_Name[(int)AIToSpawn.type] + (int)AIToSpawn.level;
        GameObject AI = Resources.Load(Config.Folder_AITraffic + prefabName) as GameObject;
        AI = Instantiate(AI, InParent.transform.position - InParent.GetComponent<TrafficStation>().Offset, Quaternion.identity);

        return AI;
    }

    private void AttachAIData()
    {
        for (int i = 0; i < (int)AI_TYPE.NUM; i++)
            for (int j = 0; j < (int)AI_LEVEL.NUM; j++)
                List_AIData.Add(new AIData((AI_TYPE)i, (AI_LEVEL)j));
    }

    public void SendAI(AI_TYPE InType, AI_LEVEL InLevel, GameObject InParent)
    {
        List<GameObject> List_AI;
        if (!Map_AI.TryGetValue(InType, out List_AI))
        {
            List_AI = new List<GameObject>();
            Map_AI.Add(InType, List_AI);
        }

        for (int i = 0; i < (int)InLevel; i++)
        {
            GameObject AI = SpawnAI(InType, (AI_LEVEL)i, InParent);
            AI.GetComponent<TrafficAI>().type = InType;

            List_AI.Add(AI);
        }
    }

    public void ReCycleAI(AI_TYPE InType)
    {
        if(Map_AI.ContainsKey(InType))
        {
            List<GameObject> List_AI = Map_AI[InType];
            List_AI.ForEach(AI => Destroy(AI));
            List_AI.Clear();
            Map_AI.Remove(InType);
        }
    }

    public void ClearAll()
    {
        foreach (var type in Map_AI.Keys) ReCycleAI(type);
    }
}




// model
public class AIData
{
    public static string[] AI_Name = { "Train_", "Car_", "Airplane_", "Ship_" };

    public AI_TYPE type { get; private set; }
    public AI_LEVEL level { get; private set; }

    public AIData(AI_TYPE InType, AI_LEVEL InLevel)
    {
        type = InType;
        level = InLevel;
    }
}
