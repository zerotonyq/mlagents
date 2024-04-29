using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration.Data
{
    public class TerrainData
    {
        public Dictionary<Vector3, Mesh> ChunkMeshes = new();
        public float ChunkLength;
    }
}