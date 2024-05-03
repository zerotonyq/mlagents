using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceManagement;
using ResourceManagement.Data;
using TerrainGeneration;
using UnityEngine;
using Zenject;
using TerrainData = TerrainGeneration.Data.TerrainData;
using Vector3 = UnityEngine.Vector3;

namespace Map
{
    public class ChunkLoader
    {
        public float OffsetToLoadNewChunk => _offsetToLoadNewChunk;


        private float _offsetToLoadNewChunk;

        private Vector3[,] _chunksInitPositions;

        private Chunk.Chunk[,] _currentLoadedChunks;

        public TerrainData TerrainData = new();

        //first - position on the plane, second - posititon in world space 
        public Dictionary<Vector3, Vector3> ChunkPlanePositions = new();

        public Dictionary<Vector3, Chunk.Chunk> InstantiatedChunks = new();

        private AddressableAssetPreloader _gap;
        private Material _chunkMaterial;
        public Action MaterialInitialized;

        [Inject]
        public async void Initialize(AddressableAssetPreloader gap)
        {
            _gap = gap;
            
            _gap.StartPreloadingAsset(GameplayAssetName.TerrainMaterial.ToString(), (Material material) =>
            {
                AssignChunkMaterial(material);
                MaterialInitialized?.Invoke();
            });
            
            TerrainData = await TerrainGenerator.LoadTerrainDataStatic();

            _offsetToLoadNewChunk = TerrainData.ChunkLength / 4;

            foreach (var key in TerrainData.ChunkMeshes.Keys)
            {
                ChunkPlanePositions.Add(new Vector3(key.x, 0, key.z), key);
            }
        }

        private void AssignChunkMaterial(Material m) => _chunkMaterial = m;


        public Chunk.Chunk GetChunk(Vector3 position)
        {
            if (InstantiatedChunks.TryGetValue(position, out Chunk.Chunk chunk))
                return chunk;

            return CreateChunk(position).Result;
        }

        public Vector3 GetClosestChunkCenterPosition(Vector3 targetPosition, Vector3 previousChunkCenterPosition,
            int searchRadiusFactor)
        {
            float minDistance = 1000000f;
            Vector3 resultChunkCenterPos = Vector3.zero;

            for (int y = -searchRadiusFactor; y <= searchRadiusFactor; y++)
            {
                for (int x = -searchRadiusFactor; x <= searchRadiusFactor; x++)
                {
                    var chunkCenterPos = new Vector3(previousChunkCenterPosition.x, 0, previousChunkCenterPosition.z) +
                                         Vector3.right * x * TerrainData.ChunkLength +
                                         Vector3.forward * y * TerrainData.ChunkLength;

                    if (!ChunkPlanePositions.ContainsKey(chunkCenterPos))
                        continue;

                    if ((chunkCenterPos - targetPosition).magnitude < minDistance)
                    {
                        minDistance = (chunkCenterPos - targetPosition).magnitude;
                        resultChunkCenterPos = chunkCenterPos;
                    }
                }
            }

            if (!ChunkPlanePositions.ContainsKey(resultChunkCenterPos))
                return ChunkPlanePositions[ChunkPlanePositions.Keys.First()];
            return ChunkPlanePositions[resultChunkCenterPos];
        }

        public Vector3 GetClosestChunkCenterPosition(Vector3 targetPosition, int searchRadiusFactor)
        {
            var xFactor = (int)targetPosition.x / (int)TerrainData.ChunkLength;
            var yFactor = (int)targetPosition.y / (int)TerrainData.ChunkLength;
            var zFactor = (int)targetPosition.z / (int)TerrainData.ChunkLength;

            var clampedPosition = new Vector3(xFactor, yFactor, zFactor) * TerrainData.ChunkLength +
                                  new Vector3(1, 0, 1) * TerrainData.ChunkLength / 2;


            return GetClosestChunkCenterPosition(targetPosition, clampedPosition, searchRadiusFactor);
        }

        public Queue<Vector3> FindChunkCenterPositionsAround(Vector3 chunkCenterPosition,
            int distanceScale, bool onlyEdge = true)
        {
            var positions = new Queue<Vector3>();
            for (int y = -distanceScale; y <= distanceScale; y++)
            {
                for (int x = -distanceScale; x <= distanceScale; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (onlyEdge && (Mathf.Abs(x) != distanceScale && Mathf.Abs(y) != distanceScale))
                        continue;

                    var currentCenter = new Vector3(chunkCenterPosition.x, 0, chunkCenterPosition.z) +
                                        Vector3.right * x * TerrainData.ChunkLength +
                                        Vector3.forward * y * TerrainData.ChunkLength;

                    if (!ChunkPlanePositions.ContainsKey(currentCenter))
                        continue;

                    positions.Enqueue(ChunkPlanePositions[currentCenter]);
                }
            }

            return positions;
        }

        public async Task<Chunk.Chunk> CreateChunk(Vector3 pos)
        {
            if (InstantiatedChunks.TryGetValue(pos, out Chunk.Chunk chunk))
            {
                chunk.Activate();
                return chunk;
            }

            if (!TerrainData.ChunkMeshes.TryGetValue(pos, out Mesh chunkMesh))
            {
                Debug.Log(
                    "Cannot create chunk. There is no chunk in chunkDatas at this position " + pos);
                return null;
            }

            var newChunk = await Chunk.Chunk.Construct(chunkMesh,
                pos - new Vector3(TerrainData.ChunkLength / 2f, 0, TerrainData.ChunkLength / 2f), _chunkMaterial);

            InstantiatedChunks.Add(pos, newChunk);
            return newChunk;
        }

        public void CreateChunks(Queue<Vector3> positions)
        {
            while (positions.TryDequeue(out Vector3 position))
                CreateChunk(position);
        }

        private void DeleteChunk(Vector3 position)
        {
            if (!InstantiatedChunks.TryGetValue(position, out Chunk.Chunk chunk)) return;
            chunk.Deactivate();
        }

        public void DeleteChunks(Queue<Vector3> positions)
        {
            while (positions.TryDequeue(out Vector3 position))
            {
                DeleteChunk(position);
            }
        }
    }
}