using Boo.Lang.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MST
{
    public class Edge
    {
        public Edge(int begin, int end, int weight)
        {
            this.Begin = begin;
            this.End = end;
            this.Weight = weight;
        }

        public int Begin { get; private set; }
        public int End { get; private set; }
        public int Weight { get; private set; }
        public int WeightAsRoad = 0;

        public override string ToString()
        {
            return string.Format(
              "Begin[{0}], End[{1}], Weight[{2}]",
              Begin, End, Weight);
        }

        public Edge Reserve()
        {
            return new Edge(this.End, this.Begin, this.Weight);
        }
    }

    public class Graph
    {
        private Dictionary<int, List<Edge>> _adjacentEdges = new Dictionary<int, List<Edge>>();

        public List<int> GetSingleAttachedPoint()
        {
            List<int> keys = new List<int>();
            foreach(var pair in _adjacentEdges)
            {
                if(pair.Value.Count == 1)
                {
                    keys.Add(pair.Key);
                }
            }

            return keys;
        }

        public List<Edge> adj(int v)
        {
            return _adjacentEdges[v];
        }

        public void ResetAdj(List<Edge> edges)
        {
            foreach (var list in _adjacentEdges.Values) list.Clear();
            foreach (var e in edges)
            {
                _adjacentEdges[e.Begin].Add(e);
                _adjacentEdges[e.End].Add(e.Reserve());
            }
        }

        public Graph(Dictionary<int, Vector3> D)
        {
            this.VertexCount = D.Count;

            for (int i = 0; i < D.Count; i++)
            {
                for (int j = 0; j < D.Count; j++)
                {
                    if (i != j) // 避免自环
                    {
                        AddEdge(i, j, Mathf.FloorToInt(Vector3.Distance(D[i], D[j]) * 1000000));
                    }
                }
            }
        }

        public int VertexCount { get; private set; }

        public IEnumerable<int> Vertices { get { return _adjacentEdges.Keys; } }

        public IEnumerable<Edge> Edges
        {
            get { return _adjacentEdges.Values.SelectMany(e => e); }
        }

        public int EdgeCount { get { return this.Edges.Count(); } }

        public void AddEdge(int begin, int end, int weight)
        {
            if (!_adjacentEdges.ContainsKey(begin))
            {
                var edges = new List<Edge>();
                _adjacentEdges.Add(begin, edges);
            }

            _adjacentEdges[begin].Add(new Edge(begin, end, weight));
        }

        public List<Edge> Prim()
        {
            // Array to store constructed MST
            int[] parent = new int[VertexCount];

            // Key values used to pick minimum weight edge in cut
            int[] keySet = new int[VertexCount];

            // To represent set of vertices not yet included in MST
            bool[] mstSet = new bool[VertexCount];

            // Initialize all keys as INFINITE
            for (int i = 0; i < VertexCount; i++)
            {
                keySet[i] = int.MaxValue;
                mstSet[i] = false;
            }

            // Always include first 1st vertex in MST.
            // Make key 0 so that this vertex is picked as first vertex
            keySet[0] = 0;
            parent[0] = -1; // First node is always root of MST 

            // The MST will have V vertices
            for (int i = 0; i < VertexCount - 1; i++)
            {
                // Pick thd minimum key vertex from the set of vertices
                // not yet included in MST
                int u = CalculateMinDistance(keySet, mstSet);

                // Add the picked vertex to the MST Set
                mstSet[u] = true;

                // Update key value and parent index of the adjacent vertices of
                // the picked vertex. Consider only those vertices which are not yet
                // included in MST
                for (int v = 0; v < VertexCount; v++)
                {
                    // graph[u, v] is non zero only for adjacent vertices of m
                    // mstSet[v] is false for vertices not yet included in MST
                    // Update the key only if graph[u, v] is smaller than key[v]
                    if (!mstSet[v]
                      && _adjacentEdges.ContainsKey(u)
                      && _adjacentEdges[u].Exists(e => e.End == v))
                    {
                        int d = _adjacentEdges[u].Single(e => e.End == v).Weight;
                        if (d < keySet[v])
                        {
                            keySet[v] = d;
                            parent[v] = u;
                        }
                    }
                }
            }

            // get all MST edges
            List<Edge> mst = new List<Edge>();
            for (int i = 1; i < VertexCount; i++)
                mst.Add(_adjacentEdges[parent[i]].Single(e => e.End == i));

            return mst;
        }

        private int CalculateMinDistance(int[] keySet, bool[] mstSet)
        {
            int minDistance = int.MaxValue;
            int minDistanceIndex = -1;

            for (int v = 0; v < VertexCount; v++)
            {
                if (!mstSet[v] && keySet[v] <= minDistance)
                {
                    minDistance = keySet[v];
                    minDistanceIndex = v;
                }
            }

            return minDistanceIndex;
        }
    }

    // 使用深度优先搜索查找图中的路径
    public class DepthFirstPaths
    {
        private bool[] marked;       //这个顶点上调用过dfs()了吗？
        private int[] edgeTo;               //从起点到一个顶点的已知路径上的最后一个顶点
        private int s;            // 起点

        public DepthFirstPaths(Graph G, int s)
        {
            marked = new bool[G.VertexCount];
            edgeTo = new int[G.VertexCount];
            this.s = s;
            dfs(G, s);
        }

        private void dfs(Graph G, int v)
        {
            marked[v] = true;
            foreach (var e in G.adj(v))
                if (!marked[e.End])
                {
                    edgeTo[e.End] = v;
                    dfs(G, e.End);
                }
        }

        public bool hasPathTo(int v)
        { return marked[v]; }

        public List<int> pathTo(int v)
        {
            if (!hasPathTo(v)) return null;
            List<int> path = new List<int>();
            for (int x = v; x != s; x = edgeTo[x])
                path.Add(x);
            path.Add(s);
            return path;
        }
    }
}

