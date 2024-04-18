using UnityEngine;
using UnityEngine.Serialization;

namespace Map.Chunk.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Map/ChunkData", fileName = "ChunkData")]
    public class ChunkData : ScriptableObject
    {
        public Mesh terrainMesh;
        public GameObject prefab;
    }
}