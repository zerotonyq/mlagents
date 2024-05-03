using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public class Node
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public Node(Vector3 pos, Vector3 meshNormal = default)
        {
            Position = pos;
            Normal = meshNormal;
        }
        public bool Deactivated { get; private set; }

        public void Deactivate() => Deactivated = true;
        
        public Node Connection { get; private set; }
        
        public List<Node> Neighbors { get; private set; } = new();
        
        public float G { get; private set; }
        private float H { get; set; }
        
        public float F => G + H;
        
        public void SetConnection(Node node) => Connection = node;
        
        public bool Discharged { get; private set; }
        
        public void SetDischarged() => Discharged = true;
        
        public void SetUndischarged() => Discharged = false;

        public float GetDistance(Node other) => (other.Position - Position).magnitude; 
        
        public void SetG(float i) => G = i;
        
        public void SetH(float i) => H = i;
        
    }
}