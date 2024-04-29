using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerrainGeneration.SerializationData;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration.Converter
{
    public class TerrainConverter
    {
        public static async Task<Data.TerrainData> ConvertToEngineFormat(TerrainSerializableData serializableData)
        {
            Data.TerrainData terrainData = new();
            Dictionary<Vector3, Mesh> engineData = new();

            foreach (var chunkSerializableData in serializableData.Chunks)
            {
                var mesh = new Mesh();
                Vector3[] vertices = new Vector3[chunkSerializableData.Vertices.Length];
                for (int i = 0; i < chunkSerializableData.Vertices.Length; i++)
                {
                    vertices[i] = new Vector3(chunkSerializableData.Vertices[i].x, chunkSerializableData.Vertices[i].y,
                        chunkSerializableData.Vertices[i].z);
                }

                mesh.vertices = vertices;
                mesh.triangles = chunkSerializableData.Triangles;

                var position = new Vector3(chunkSerializableData.Position.x, chunkSerializableData.Position.y,
                    chunkSerializableData.Position.z);


                engineData.TryAdd(position, mesh);
            }

            terrainData.ChunkMeshes = engineData;
            terrainData.ChunkLength = serializableData.Chunks[0].Position.x*2;
            return terrainData;
        }

        public static async Task<TerrainSerializableData> ConvertToSerializationFormat(Mesh mesh, int chunkLength,
            int meshLength)
        {
            if ((meshLength * meshLength) % (chunkLength * chunkLength) != 0 || meshLength % chunkLength != 0)
                throw new ArgumentException("mesh area must divisible without remainder");

            TerrainSerializableData terrainSerializableData = new TerrainSerializableData();

            int count = meshLength * meshLength / (chunkLength * chunkLength);

            var clMultY = 0;
            var clMultX = 0;

            for (int i = 0; i < count; i++)
            {
                float3[] currentChunkArray =
                    new float3[(chunkLength + 1) * (chunkLength + 1)];

                for (int j = 0, y = 0; y <= (meshLength + 1) * chunkLength; y += meshLength + 1)
                {
                    var currentY = y + (meshLength + 1) * chunkLength * clMultY;
                    for (int x = currentY + clMultX * chunkLength; x <= currentY + (clMultX + 1) * chunkLength; x++)
                    {
                        currentChunkArray[j] = new float3(mesh.vertices[x].x, mesh.vertices[x].y, mesh.vertices[x].z);
                        ++j;
                    }
                }

                var currentVector = currentChunkArray[^1] - currentChunkArray[0];

                float3 chunkInitPosition = new float3(currentChunkArray[0].x + currentVector.x / 2, 0,
                    currentChunkArray[0].z + currentVector.z / 2);

                var data = new ChunkSerializableData(chunkInitPosition, currentChunkArray,
                    CreateTriangles(chunkLength));

                terrainSerializableData.AddChunk(data);


                if ((i + 1) % (meshLength / chunkLength) == 0)
                    clMultX = 0;
                else
                    clMultX++;

                if ((i + 1) % (meshLength / chunkLength) == 0)
                    clMultY++;
            }

            return terrainSerializableData;
        }

        public static int[] CreateTriangles(int chunkSize)
        {
            int[] triangles = new int[chunkSize * chunkSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + chunkSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + chunkSize + 1;
                    triangles[tris + 5] = vert + chunkSize + 2;

                    vert++;
                    tris += 6;
                }

                vert++;
            }

            return triangles;
        }
    }
}