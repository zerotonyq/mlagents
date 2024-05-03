using System.Collections.Generic;
using TerrainGeneration;
using TerrainGeneration.Data;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities.GraphGenerator.Converter
{
    public class GraphConverter
    {
        public static GraphData ConvertToSerializationFormat(Node[,] domain, int _xDim, int _yDim)
        {
            GraphData graphData = new GraphData();
            for (int y = 0; y < _yDim; y++)
            {
                for (int x = 0; x < _xDim; x++)
                {
                    var currentNode = domain[y, x];

                    if (currentNode.Deactivated)
                        continue;

                    var neighbors = new List<float3>();
                    foreach (var neighbor in currentNode.Neighbors)
                    {
                        if (neighbor.Deactivated)
                            continue;
                        neighbors.Add(neighbor.Position);
                    }

                    NodeData currentNodeData = new NodeData(currentNode.Position, neighbors);

                    graphData.AddNode(currentNodeData);
                }
            }

            graphData.x = _xDim;
            graphData.y = _yDim;
         
            return graphData;
        }

        public static List<Node> ConvertToEngineFormat(GraphData graphData)
        {
            Dictionary<Vector3, Node> graph = new();
            List<Node> nodes = new();
            foreach (var nodeData in graphData.nodes)
            {
                Node n;
                if (graph.TryGetValue(nodeData.Position, out Node nn))
                {
                    n = nn;
                }
                else
                {
                    n = new Node(nodeData.Position);
                }

                graph.TryAdd(n.Position, n);
                nodes.Add(n);

                foreach (var position in nodeData.NeighborsPositions)
                {
                    if (graph.TryGetValue(position, out Node neighbor))
                    {
                        n.Neighbors.Add(neighbor);
                        graph.TryAdd(neighbor.Position, neighbor);
                    }
                    else
                    {
                        var newNeighbor = new Node(position);
                        n.Neighbors.Add(newNeighbor);
                        graph.TryAdd(newNeighbor.Position, newNeighbor);
                    }
                }
            }

            return nodes;
        }
    }
}