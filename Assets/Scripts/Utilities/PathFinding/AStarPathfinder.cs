using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainGeneration
{
    public static class AStarPathfinder
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f);
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);

        public static List<Node> FindPath(Node startNode, Node targetNode)
        {
            var toSearch = new List<Node>() { startNode };
            var processed = new List<Node>();

            while (toSearch.Any())
            {
                var currentNode = toSearch[0];
                for (int i = 0; i < toSearch.Count; ++i)
                    if (toSearch[i].F < currentNode.F ||
                        Math.Abs(currentNode.F - toSearch[i].F) < 0.01f &&
                        toSearch[i].F < currentNode.F)
                        currentNode = toSearch[i];

                processed.Add(currentNode);
                toSearch.Remove(currentNode);

                if (currentNode == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<Node>();
                    var count = 200;
                    while (currentPathTile != startNode)
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0)
                        {
                         Debug.Log("bad path");
                         break;
                        }
                    }

                    path.Add(currentPathTile);

#if UNITY_EDITOR
                    Debug.Log("path count " + path.Count);
#endif

        //            VisualizePath(path);
                    return path;
                }
                
                foreach (var neighbor in currentNode.Neighbors.Where(t => !processed.Contains(t)))
                {
                    if(neighbor.Deactivated)
                        continue;
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = currentNode.G + currentNode.GetDistance(neighbor);
                    
                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(currentNode);

                        if (!inSearch)
                        {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }
            Debug.Log("pathfinding ended without result");
            return null;
        }

        public static void VisualizePath(List<Node> path)
        {
            var visualizer = new GameObject("path_visualizer").AddComponent<LineRenderer>();
            visualizer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                visualizer.SetPosition(i, path[i].Position);
            }
        }
    }
}