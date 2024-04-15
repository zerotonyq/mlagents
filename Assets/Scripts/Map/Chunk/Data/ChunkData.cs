using UnityEngine;

namespace Map.Chunk.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Map/ChunkData", fileName = "ChunkData")]
    public class ChunkData : ScriptableObject
    {
        public Mesh terrainPrefab;
    }
}