using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace TerrainGeneration.SerializationData
{
    [Serializable]
    public class TerrainSerializableData : Utilities.SaveLoad.Data
    {
        private List<ChunkSerializableData> _chunks = new();

        public List<ChunkSerializableData> Chunks => _chunks;

        public void AddChunk(ChunkSerializableData data)
        {
            _chunks.Add(data);   
        }
    }
}