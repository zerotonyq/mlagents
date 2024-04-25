using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TerrainGeneration.SerializationData;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using TerrainData = TerrainGeneration.SerializationData.TerrainData;

namespace TerrainGeneration.Converter
{
    public class TerrainConverter
    {
        public static Dictionary<Vector3, Mesh> ConvertToEngineFormat(TerrainData data)
        {
            Dictionary<Vector3, Mesh> engineData = new();

            foreach (var chunkSerializableData in data.Chunks)
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

            return engineData;
        }

        /*public static TerrainData ConvertToSerializationFormat(Mesh mesh, int chunkLength, int meshLength)
        {
            float3[,] terrainMatrix = ConvertTo2DimVertices(meshLength + 1, mesh.vertices);

            TerrainData terrainData = new TerrainData();

            int countOneDim = meshLength / chunkLength;

            for (int y = 0; y < countOneDim; y++)
            {
                for (int x = 0; x < countOneDim; x++)
                {
                    float3[] vertices = CreateVertices(terrainMatrix, new int2(x, y) * chunkLength, chunkLength);
                    int[] triangles = CreateTriangles(chunkLength);

                    // get half of chunk length to obtain center
                    float3 chunkCenterPosition =
                        new float3(y, 0, x) * chunkLength + new float3(1, 0, 1) * chunkLength / 2f;

                    terrainData.AddChunk(new ChunkSerializableData(chunkCenterPosition, vertices, triangles));
                }
            }

            return terrainData;
        }*/

        public static TerrainData ConvertToSerializationFormat(Mesh mesh, int chunkLength, int meshLength)
        {

            if ((meshLength * meshLength) % (chunkLength * chunkLength) != 0 || meshLength % chunkLength != 0)
                throw new ArgumentException("mesh area must divisible without remainder");
            TerrainData terrainData = new TerrainData();
            float3 startPosition = mesh.vertices[0];
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
                
                var currentVector =currentChunkArray[^1] - currentChunkArray[0];
                float3 chunkCenterPosition = currentChunkArray[0] + new float3(currentVector.x / 2, 0, currentVector.z / 2);
                var data = new ChunkSerializableData(chunkCenterPosition, currentChunkArray, CreateTriangles(chunkLength));
                terrainData.AddChunk(data);
                //currentChunkArray.Dispose();
                
                if((i+1) % (meshLength/chunkLength) == 0)
                    clMultX=0;
                else
                    clMultX++;
                
                if ((i + 1) % (meshLength / chunkLength) == 0)
                    clMultY++;
            }



            foreach (var chunkSerializableData in terrainData.Chunks)
            {
                Debug.Log(chunkSerializableData.Position);
            }

            return terrainData;
        }

        /*private static float3[,] ConvertTo2DimVertices(int terrainDim, Vector3[] vertices)
        {
            float3[,] terrainMatrix = new float3[terrainDim, terrainDim];
            for (int i = 0, y = 0; y < terrainDim; ++y)
            {
                for (int x = 0; x < terrainDim; x++)
                {
                    terrainMatrix[y, x] = new float3(vertices[i].x, vertices[i].y, vertices[i].z);
                    ++i;
                }
            }

            return terrainMatrix;
        }*/
        private static NativeArray<NativeArray<float3>> ConvertTo2DimVertices(int terrainDim, Vector3[] vertices)
        {
            NativeArray<NativeArray<float3>> terrainMatrix =
                new NativeArray<NativeArray<float3>>(terrainDim, Allocator.TempJob);
            for (int i = 0; i < terrainMatrix.Length; i++)
            {
                terrainMatrix[i] = new NativeArray<float3>(terrainDim, Allocator.TempJob);
            }

            for (int i = 0, y = 0; y < terrainDim; ++y)
            {
                for (int x = 0; x < terrainDim; x++)
                {
                    var current = terrainMatrix[y];
                    current[x] = new float3(vertices[i].x, vertices[i].y, vertices[i].z);
                    ++i;
                }
            }

            return terrainMatrix;
        }

        private static float3[] CreateVertices(float3[,] matrix, int2 startPosition, int dim)
        {
            float3[] vertices = new float3[(dim + 1) * (dim + 1)];

            for (int i = 0, y = startPosition.y; y <= startPosition.y + dim; y++)
            {
                for (int x = startPosition.x; x <= startPosition.x + dim; x++)
                {
                    vertices[i] = matrix[y, x];
                    ++i;
                }
            }

            return vertices;
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

    public struct CreateVerticiesJob : IJob
    {
        public NativeArray<NativeArray<float3>> EnterArray;
        public NativeArray<float3> ExitArray;
        public NativeArray<int2> StartPositionArray;
        public NativeArray<int> DimArray;

        public void Execute()
        {
            CreateVertices(EnterArray, StartPositionArray[0], DimArray[0]);
        }

        private void CreateVertices(NativeArray<NativeArray<float3>> matrix, int2 startPosition, int dim)
        {
            ExitArray = new NativeArray<float3>((dim + 1) * (dim + 1), Allocator.TempJob);
        }
    }
}