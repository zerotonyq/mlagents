using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Node = TerrainGeneration.Node;

namespace Gameplay.MapManagement.Graph
{
    public class GraphManager
    {
        private List<Node> nodes;

        [Inject]
        private void Initialize()
        {
            LoadGraph(Application.dataPath + "graphData");
        }
        public void LoadGraph(string path)
        {
            nodes = GraphGenerator.LoadAndConvertGraph(path);
        }

        public Node FindNearestNode(Vector3 position)
        {
            Node nearestNode = null;
            
            float minDist = (nodes[0].Position - position).magnitude;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                var currentDist = (nodes[i].Position - position).magnitude;
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    nearestNode = nodes[i];
                }
            }

            return nearestNode;
        }
        
    }
}