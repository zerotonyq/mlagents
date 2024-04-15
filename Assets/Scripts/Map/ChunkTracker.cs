using System;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Map
{
    public class ChunkTracker : ITickable 
    {

        private const float OffsetToLoad = 5f;
        private Transform _trackedTransform;
        
        private readonly bool[,] _trackedTransformAroundMatrix = new bool[3, 3];

        private Chunk.Chunk _currentChunk;

        private Vector2Int _currentCoord;
        
        private ChunkLoader _chunkLoader;
        
        [Inject]
        public void Initialize(GameplayEntryPoint gameplayEntryPoint, ChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;
            gameplayEntryPoint.PlayerCreated += OnPlayerCreated;
        }
        
        private async void OnPlayerCreated(Transform trackedTransform)
        {
            _trackedTransform = trackedTransform;

            _currentCoord = _chunkLoader.GetClosestChunkMatrixCoord(_trackedTransform.position);
            
            _currentChunk = await _chunkLoader.CreateChunk(_currentCoord);
        }
        
        private void ClearTrackedTransformAroundMatrix()
        {
            for (int i = 0; i < _trackedTransformAroundMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < _trackedTransformAroundMatrix.GetLength(1); ++j)
                {
                    _trackedTransformAroundMatrix[i, j] = false;
                }
            }
        }

        public void CheckPosition(Vector3 trackedPosition)
        {
            if (_currentChunk == null)
                throw new ArgumentException("there is no chunk to check position");
            
            if ((trackedPosition - _currentChunk.ChunkPosition).magnitude > OffsetToLoad)
                return;

            ClearTrackedTransformAroundMatrix();

            //[y, x]
            if (trackedPosition.x - _currentChunk.ChunkPosition.x >= 0)
                _trackedTransformAroundMatrix[1, 2] = true;
            if (trackedPosition.x - _currentChunk.ChunkPosition.x < 0)
                _trackedTransformAroundMatrix[1, 0] = true;

            if (trackedPosition.z - _currentChunk.ChunkPosition.z >= 0)
                _trackedTransformAroundMatrix[0, 1] = true;
            if (trackedPosition.z - _currentChunk.ChunkPosition.z < 0)
                _trackedTransformAroundMatrix[2, 1] = true;

            if (_trackedTransformAroundMatrix[1, 0] && _trackedTransformAroundMatrix[0, 1])
                _trackedTransformAroundMatrix[0, 0] = true;
            if (_trackedTransformAroundMatrix[0, 1] && _trackedTransformAroundMatrix[1, 2])
                _trackedTransformAroundMatrix[0, 2] = true;
            if (_trackedTransformAroundMatrix[1, 0] && _trackedTransformAroundMatrix[2, 1])
                _trackedTransformAroundMatrix[2, 0] = true;
            if (_trackedTransformAroundMatrix[2, 1] && _trackedTransformAroundMatrix[1, 2])
                _trackedTransformAroundMatrix[2, 2] = true;

            _chunkLoader.CreateChunks(_trackedTransformAroundMatrix, _currentCoord);
        }
        
        public void Tick()
        {
            if (!_trackedTransform || _currentChunk == null)
                return;

            var position = _trackedTransform.position;
            CheckPosition(position);
            TrySwitchChunk(position);
        }

        private void TrySwitchChunk(Vector3 trackedPosition)
        {
            if ((trackedPosition - _currentChunk.ChunkPosition).magnitude > _chunkLoader.ChunkLength)
            {
                _currentChunk = _chunkLoader.GetClosestChunkAround()
            }
        }
    }
}