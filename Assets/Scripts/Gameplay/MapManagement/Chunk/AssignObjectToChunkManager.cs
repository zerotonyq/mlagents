using UnityEngine;
using Zenject;

namespace Map
{
    public class AssignObjectToChunkManager
    {
        private ChunkLoader _chunkLoader;

        [Inject]
        public void Initialize(ChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;
        }

        public void AssignObjectToChunk(GameObject gameObject)
        {
            var chunkCenterPosition = _chunkLoader.GetClosestChunkCenterPosition(gameObject.transform.position, 5);
            _chunkLoader.GetChunk(chunkCenterPosition).AddObjectToChunk(gameObject);
        }

        public void RemoveObjectFromChunk(GameObject gameObject)
        {
            var chunkCenterPosition = _chunkLoader.GetClosestChunkCenterPosition(gameObject.transform.position, 5);
            _chunkLoader.GetChunk(chunkCenterPosition).RemoveObjectFromChunk(gameObject);
        }
    }
}