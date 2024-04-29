using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Task = UnityEditor.VersionControl.Task;
using TerrainData = TerrainGeneration.Data.TerrainData;

namespace Map.Chunk
{
    public class Chunk
    {
        public Vector3 CenterPosition => _centerPosition;

        public IReadOnlyList<GameObject> GameObjects => _gameObjects;

        private GameObject _instance;
        private Vector3 _centerPosition;

        private List<GameObject> _gameObjects = new();

        private Chunk(GameObject instance, Vector3 centerPosition)
        {
            _centerPosition = centerPosition;
            _instance = instance;
        }

        public void AddObjectToChunk(GameObject obj) => _gameObjects.Add(obj);

        public void RemoveObjectFromChunk(GameObject obj)
        {
            if (!_gameObjects.Contains(obj))
                return;
            _gameObjects.Remove(obj);
        }

        public static async Task<Chunk> Construct(Mesh m, Vector3 position, Material mat)
        {
            var obj = new GameObject(position.ToString());

            obj.transform.position = position;

            var terrainData = await CreateTerrainData(m);

            var terrain = obj.AddComponent<Terrain>();
            terrain.terrainData = terrainData;
            terrain.materialTemplate = mat;
            terrain.heightmapPixelError = 1;
            obj.AddComponent<TerrainCollider>().terrainData = terrainData;

            return new Chunk(obj, position);
        }

        private static async Task<UnityEngine.TerrainData> CreateTerrainData(Mesh m)
        {
            var terrainData = new UnityEngine.TerrainData();


            var sqrtInt = (int)Math.Sqrt(m.vertices.Length);

            terrainData.heightmapResolution = sqrtInt;
            terrainData.size = new Vector3(sqrtInt - 1, m.bounds.size.y, sqrtInt - 1);
            var heights = await CreateHeightMap(sqrtInt, m);
            terrainData.SetHeights(0, 0, heights);

            return terrainData;
        }

        private static async Task<float[,]> CreateHeightMap(int size, Mesh m)
        {
            float[,] height = new float[size, size];
            var vertices = m.vertices;
            for (int i = 0, y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    height[y, x] = vertices[i].y / m.bounds.size.y;
                    ++i;
                }
            }

            return height;
        }

        public void Deactivate() => _instance.SetActive(false);

        public void Activate() => _instance.SetActive(true);
    }
}