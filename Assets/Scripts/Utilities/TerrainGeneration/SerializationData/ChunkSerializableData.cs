using System;
using Unity.Collections;
using Unity.Mathematics;

namespace TerrainGeneration.SerializationData
{
    [Serializable]
    public class ChunkSerializableData : Utilities.SaveLoad.Data
    {
        private float3 _position;
        private float3[] _vertices;
        private int[] _triangles;

        /*
        public ChunkSerializableData(float3 position, float3[] vertices, int[] triangles)
        {
            _position = position;
            _vertices = vertices;
            _triangles = triangles;
        }
        */
        public ChunkSerializableData(float3 position, float3[] vertices, int[] triangles)
        {
            _position = position;
            _vertices = vertices;
            _triangles = triangles;
        }
        //private NativeArray<float3> _vertices;
        public float3 Position => _position;

        public float3[] Vertices => _vertices;
        //public NativeArray<float3> Vertices => _vertices;

        public int[] Triangles => _triangles;
    }
}