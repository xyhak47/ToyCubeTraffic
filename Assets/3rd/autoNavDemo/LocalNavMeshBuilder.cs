using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using Enums;

// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
[DefaultExecutionOrder(-102)]
public class LocalNavMeshBuilder : MonoBehaviour
{
    public static LocalNavMeshBuilder Instance = null;
    LocalNavMeshBuilder()
    {
        Instance = this;
    }

    // The center of the build
    public Transform m_Tracked;

    // The size of the build bounds
    public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

    Dictionary<AI_TYPE, NavMeshDataInstance> Map_MeshInstance = new Dictionary<AI_TYPE, NavMeshDataInstance>();
    Dictionary<AI_TYPE, NavMeshData> Map_NavMesh = new Dictionary<AI_TYPE, NavMeshData>();

    private static string[] AREAS = { "TRAIN", "CAR", "AIRPLANE", "SHIP" };


    public AsyncOperation ResetNavMesh(AI_TYPE InType)
    {
        NavMeshDataInstance dataInstance;
        if (Map_MeshInstance.TryGetValue(InType, out dataInstance))
        {
            NavMesh.RemoveNavMeshData(dataInstance);
            Map_MeshInstance.Remove(InType);
        }

        NavMeshData meshData = new NavMeshData();
        dataInstance = NavMesh.AddNavMeshData(meshData);
        Map_MeshInstance.Add(InType, dataInstance);

        int agentIndex = (int)InType;

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

        NavMeshSourceTag.Collect(ref sources, GetArea(agentIndex), InType);

        NavMeshBuildSettings defaultBuildSettings = NavMesh.GetSettingsByIndex(agentIndex);

        //print(agentIndex + "," + defaultBuildSettings.agentRadius);

        var bounds = QuantizedBounds();

        return NavMeshBuilder.UpdateNavMeshDataAsync(meshData, defaultBuildSettings, sources, bounds);
    }

    static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }

    Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        var center = m_Tracked ? m_Tracked.position : transform.position;
        return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var bounds = QuantizedBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = Color.green;
        var center = m_Tracked ? m_Tracked.position : transform.position;
        Gizmos.DrawWireCube(center, m_Size);
    }

    private int GetArea(int InAgentIndex)
    {
        return NavMesh.GetAreaFromName(AREAS[InAgentIndex]);
    }

    public void Clear(AI_TYPE InType)
    {
        NavMeshDataInstance dataInstance;
        if (Map_MeshInstance.TryGetValue(InType, out dataInstance))
        {
            NavMesh.RemoveNavMeshData(dataInstance);
            Map_MeshInstance.Remove(InType);
        }
    }
}
