using System;
using System.Text;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Map
{
    public class ChunkTracker : ITickable 
    {

        private const float OffsetToLoad = 10f;
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
            
            //Debug.Log((trackedPosition - _currentChunk.ChunkPosition).magnitude);
            if ((trackedPosition - _currentChunk.ChunkPosition).magnitude < OffsetToLoad)
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

            //PrintMatrix();            
            _chunkLoader.CreateChunks(_trackedTransformAroundMatrix, _currentCoord);
        }

        private void PrintMatrix()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < _trackedTransformAroundMatrix.GetLength(0); ++y)
            {
                for (int x = 0; x < _trackedTransformAroundMatrix.GetLength(1); ++x)
                {
                    sb.Append(_trackedTransformAroundMatrix[y, x] + " ");
                }

                sb.Append("\n");
            }
            Debug.Log(sb.ToString());
        }
        public void Tick()
        {
            if (!_trackedTransform || _currentChunk == null)
                return;

            var position = _trackedTransform.position;
            CheckPosition(position);
            TrySwitchChunk(_currentCoord, position);
        }

        private void TrySwitchChunk(Vector2Int currentCoord, Vector3 trackedPosition)
        {
            if ((trackedPosition - _currentChunk.ChunkPosition).magnitude < _chunkLoader.ChunkLength)
                return;
            var newChunkAndCoord = _chunkLoader.GetChunkAround(currentCoord, trackedPosition);
            _currentChunk = newChunkAndCoord.Item1;
            _currentCoord = newChunkAndCoord.Item2;
            //Debug.Log((trackedPosition - _currentChunk.ChunkPosition).magnitude + " " + _currentChunk.ChunkPosition);
            Debug.Log(_currentChunk.ChunkPosition);
        }
    }
}