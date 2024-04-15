using System;
using Map.Chunk.Data;
using UnityEngine;

namespace Map.Chunk
{
    public class ChunkComponent : MonoBehaviour
    {
        private Vector3 _initChunkPosition;
        public void Start() => _initChunkPosition = transform.position;
       
        public Vector3 InitChunkPosition => _initChunkPosition;
    }
}