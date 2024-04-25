using System;
using System.Collections.Generic;

namespace TerrainGeneration.Data
{
    [Serializable]
    public class GraphData : Utilities.SaveLoad.Data
    {
        public List<NodeData> nodes = new();

        public int x, y;
        public void AddNode(NodeData nd) => nodes.Add(nd);
    }
}