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
            NativeArray<NativeArray<float3>> terrainMatrix = ConvertTo2DimVertices(meshLength + 1, mesh.vertices);

            TerrainData terrainData = new TerrainData();

            int countOneDim = meshLength / chunkLength;

            for (int y = 0; y < countOneDim; y++)
            {
                for (int x = 0; x < countOneDim; x++)
                {
                    var StartPositionArr = new NativeArray<int2>(1, Allocator.TempJob);
                    StartPositionArr[0] = new int2(x, y) * chunkLength;
                    var dimArr = new NativeArray<int>(1, Allocator.TempJob);
                    dimArr[0] = chunkLength;
                    var createVerticesJob = new CreateVerticiesJob()
                    {
                        EnterArray = terrainMatrix, 
                        StartPositionArray =StartPositionArr,
                        DimArray = dimArr
                    };
                    var handle = createVerticesJob.Schedule();
                    handle.Complete();
                    //float3[] vertices = CreateVertices(terrainMatrix, new int2(x, y) * chunkLength, chunkLength);
                    int[] triangles = CreateTriangles(chunkLength);

                    // get half of chunk length to obtain center
                    float3 chunkCenterPosition =
                        new float3(y, 0, x) * chunkLength + new float3(1, 0, 1) * chunkLength / 2f;

                    terrainData.AddChunk(new ChunkSerializableData(chunkCenterPosition, createVerticesJob.ExitArray, triangles));
                }
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

        public static int[] CreateTriangles(int terrainSize)
        {
            int[] triangles = new int[terrainSize * terrainSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < terrainSize; z++)
            {
                for (int x = 0; x < terrainSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + terrainSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + terrainSize + 1;
                    triangles[tris + 5] = vert + terrainSize + 2;

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

            for (int i = 0, y = startPosition.y; y <= startPosition.y + dim; y++)
            {
                for (int x = startPosition.x; x <= startPosition.x + dim; x++)
                {
                    ExitArray[i] = matrix[y][x];
                    ++i;
                }
            }
        }
    }
}