using System;
using System.Collections.Generic;
using DefaultNamespace;
using Map.Chunk;
using Map.Data;
using ResourceManagement;
using ResourceManagement.Data;
using UnityEngine;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Map
{
    public class ChunkLoadManager : ITickable
    {
        private const float OffsetToLoad = 5f;

        private MapData _currentMapData;

        private bool[,] _map;
        private Vector3[,] _chunkPositions;
        private Transform _trackedTransform;

        private bool[,] _trackedTransformAroundMatrix = new bool[3, 3];

        private ChunkComponent _currentChunk;

        [Inject]
        public void Initialize(GameRulesManager grm, GameplayAssetPreloader gap)
        {
            gap.StartPreloadingAsset(AssetName.MapData.ToString(), (ScriptableObject so) =>
            {
                _currentMapData = so as MapData;
                _map = new bool[_currentMapData.chunks.X[0].Y.Count, _currentMapData.chunks.X.Count];
                _chunkPositions = new Vector3[_currentMapData.chunks.X[0].Y.Count, _currentMapData.chunks.X.Count];
            });
            grm.PlayerCreated += (t) =>
            {
                _trackedTransform = t;
                InitCheck(t.position);
            };
        }

        private void InitChunkPositionsInScene(Vector3[,] chunkPositions, float boundLength)
        {
            for (int i = 0; i < chunkPositions.Length; i++)
            {
                for (int j = 0; j < chunkPositions.Length; j++)
                {
                    
                }
            }
        }
        private void InitCheck(Vector3 pos)
        {
            Vector2Int initChunkCoord;
            for (int i = 0; i < _map.Length; ++i)
            {
                for (int j = 0; j < _map.Length; ++j)
                {
                    if
                }
            }
        }

        public void Tick()
        {
            if (!_trackedTransform)
                return;

            CheckPosition(_trackedTransform.position);
        }


        private void ClearAroundMatrix()
        {
            for (int i = 0; i < _trackedTransformAroundMatrix.Length; ++i)
            {
                for (int j = 0; j < _trackedTransformAroundMatrix.Length; ++j)
                {
                    _trackedTransformAroundMatrix[i, j] = false;
                }
            }
        }

        public void CheckPosition(Vector3 trackedPosition)
        {
            if (!_currentChunk)
                throw new ArgumentException("there is no chunk to check position");

            if ((trackedPosition - _currentChunk.InitChunkPosition).sqrMagnitude > OffsetToLoad)
                return;

            ClearAroundMatrix();

            if (trackedPosition.x - _currentChunk.InitChunkPosition.x >= 0)
                _trackedTransformAroundMatrix[1, 2] = true;
            if (trackedPosition.x - _currentChunk.InitChunkPosition.x < 0)
                _trackedTransformAroundMatrix[1, 0] = true;

            if (trackedPosition.z - _currentChunk.InitChunkPosition.z >= 0)
                _trackedTransformAroundMatrix[0, 1] = true;
            if (trackedPosition.z - _currentChunk.InitChunkPosition.z < 0)
                _trackedTransformAroundMatrix[2, 1] = true;

            if (_trackedTransformAroundMatrix[1, 0] && _trackedTransformAroundMatrix[0, 1])
                _trackedTransformAroundMatrix[0, 0] = true;
            if (_trackedTransformAroundMatrix[0, 1] && _trackedTransformAroundMatrix[1, 2])
                _trackedTransformAroundMatrix[0, 2] = true;
            if (_trackedTransformAroundMatrix[1, 0] && _trackedTransformAroundMatrix[2, 1])
                _trackedTransformAroundMatrix[2, 0] = true;
            if (_trackedTransformAroundMatrix[2, 1] && _trackedTransformAroundMatrix[1, 2])
                _trackedTransformAroundMatrix[2, 2] = true;
        }
    }
}