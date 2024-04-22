using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
    public class Node : IDisposable
    {
        public readonly Vector3 Position;
        public readonly Vector2Int DomainPosition;
        public readonly Vector3 Normal;
        public Node(Vector3 pos, Vector2Int domainPos, Vector3 meshNormal)
        {
            Position = pos;
            DomainPosition = domainPos;
            Normal = meshNormal;
        }
        public bool Deactivated { get; private set; }

        public void Deactivate() => Deactivated = true;
        public Node Connection { get; private set; }

        public Color color { get; private set; }

        public void SetColor(Color c) => color = c;
        public List<Node> Neighbors { get; private set; } = new List<Node>();
        public float G { get; private set; }
        public float H { get; private set; }
        
        public float F => G + H;
        
        public void SetConnection(Node node) => Connection = node;
        
        public bool DischargeDone { get; private set; }

        public void Discharged() => DischargeDone = true;
        public void Undischarged() => DischargeDone = false;

        public float GetDistance(Node other) => (other.Position - this.Position).magnitude; 
        public void SetG(float i) => G = i;
        
        public void SetH(float i) => H = i;
        
        
        private bool disposed = false;
 
        // реализация интерфейса IDisposable.
        public void Dispose()
        {
            // освобождаем неуправляемые ресурсы
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }
 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                // Освобождаем управляемые ресурсы
            }
            // освобождаем неуправляемые объекты
            disposed = true;
        }
 
        // Деструктор
        ~Node()
        {
            Dispose (false);
        }
    }
}