using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Enums;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static Dictionary<AI_TYPE, List<MeshFilter>> Map_Meshes = new Dictionary<AI_TYPE, List<MeshFilter>>();


    [System.NonSerialized]
    public AI_TYPE type;


    public void CollectMeshs()
    {
        List<MeshFilter> meshs;
        if (!Map_Meshes.TryGetValue(type, out meshs))
        {
            meshs = new List<MeshFilter>();
            Map_Meshes.Add(type, meshs);
        }
        meshs.Add(GetComponent<MeshFilter>());
    }

    private void UnCollectMeshs()
    {
        List<MeshFilter> meshs;
        if (!Map_Meshes.TryGetValue(type, out meshs))
        {
            return;
        }
        meshs.Remove(GetComponent<MeshFilter>());
    }

    void OnDestroy()
    {
        UnCollectMeshs();
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources, int InArea, AI_TYPE InType)
    {
        sources.Clear();

        for (var i = 0; i < Map_Meshes[InType].Count; ++i)
        {
            var mf = Map_Meshes[InType][i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = InArea;
            sources.Add(s);
        }
    }

    public static void UnCollect(AI_TYPE InType)
    {
        Map_Meshes.Remove(InType);
    }
}
