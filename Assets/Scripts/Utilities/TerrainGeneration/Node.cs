using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public class Node
    {
        public readonly Vector3 Position;
        public Node(Vector3 pos)
        {
            Position = pos;
        }
        
        public Node Connection { get; private set; }

        public Color color { get; private set; }

        public void SetColor(Color c) => color = c;
        public List<Node> Neighbors { get; private set; } = new List<Node>();
        public float G { get; private set; }
        public float H { get; private set; }
        
        public float F => G + H;
        
        public void SetConnection(Node node) => Connection = node;

        public float GetDistance(Node other) => (other.Position - this.Position).magnitude; 
        public void SetG(float i) => G = i;
        
        public void SetH(float i) => H = i;
    }
}