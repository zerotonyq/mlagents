using System;
using System.Collections.Generic;
using System.Text;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Map
{
    public class MapPositionTracker : ITickable
    {
        private ChunkLoader _chunkLoader;

        private Transform _trackedTransform;

        private Vector3 _currentChunkCenterPosition;
        private Queue<Vector3> _creationQueue = new();

        [Inject]
        public void Initialize(GameplayEntryPoint gameplayEntryPoint, ChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;
            gameplayEntryPoint.PlayerCreated += transform =>
            {
                _trackedTransform = transform;
                _currentChunkCenterPosition = _chunkLoader.GetClosestChunkCenterPosition(_trackedTransform.position, 5);
                _chunkLoader.CreateChunk(_currentChunkCenterPosition);
            };
        }


        public void TryCreateChuksAround(Vector3 trackedPosition)
        {
            
            if ((trackedPosition - _currentChunkCenterPosition).magnitude < _chunkLoader.OffsetToLoadNewChunk)
                return;
            
            var position = _trackedTransform.position;

            _currentChunkCenterPosition = _chunkLoader.GetClosestChunkCenterPosition(position,
                _currentChunkCenterPosition, 1);
            
            var createPositions = _chunkLoader.FindChunkCenterPositionsAround(_currentChunkCenterPosition, 1);
            
            _chunkLoader.CreateChunks(createPositions);
        }

        private void TryDeleteChunksAround(Vector3 trackedPosition)
        {
            _currentChunkCenterPosition = _chunkLoader.GetClosestChunkCenterPosition(trackedPosition,
                _currentChunkCenterPosition, 1);
            
            var deletePositions = _chunkLoader.FindChunkCenterPositionsAround(_currentChunkCenterPosition, 2);
            
            _chunkLoader.DeleteChunks(deletePositions);
        }

        public void Tick()
        {
            if (!_trackedTransform)
                return;
            Debug.DrawLine(_currentChunkCenterPosition, _currentChunkCenterPosition+Vector3.up*100f, Color.green);
            var position = _trackedTransform.position;
            TryDeleteChunksAround(position);
            TryCreateChuksAround(position);
        }
    }
}