using System.Threading.Tasks;
using Map.Data;
using ResourceManagement;
using ResourceManagement.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Map
{
    public class ChunkLoader
    {
        private MapData _currentMapData;

        
        private Vector3[,] _chunksInitPositions;
        private Chunk.Chunk[,] _currentLoadedChunks;

        [Inject]
        public void Initialize(GameplayAssetPreloader gap)
        {
            gap.StartPreloadingAsset(AssetName.MapData.ToString(), (ScriptableObject so) =>
            {
                _currentMapData = so as MapData;
                InitChunkPositions();
            });
        }


        private void InitChunkPositions()
        {
            _chunksInitPositions = new Vector3[_currentMapData.chunks.Y[0].X.Count, _currentMapData.chunks.Y.Count];
            _currentLoadedChunks = new Chunk.Chunk[_currentMapData.chunks.Y[0].X.Count, _currentMapData.chunks.Y.Count];

            Vector3 currentPosition =
                new Vector3(0, 0, _currentMapData.chunks.Y.Count * _currentMapData.ChunkBoundLength);

            for (int y = 0; y < _currentMapData.chunks.Y.Count; ++y)
            {
                for (int x = 0; x < _currentMapData.chunks.Y[y].X.Count; ++x)
                {
                    _chunksInitPositions[y, x] = currentPosition;
                    currentPosition += new Vector3(_currentMapData.ChunkBoundLength, 0, 0);
                }

                currentPosition = new Vector3(0, 0, currentPosition.z);
                currentPosition -= new Vector3(0, 0, _currentMapData.ChunkBoundLength);
            }

        }

        public async Task<Chunk.Chunk> CreateChunk(Vector2Int matrixCoord)
        {
            if (matrixCoord.x < 0 || matrixCoord.y < 0 ||
                matrixCoord.x >= _currentMapData.chunks.Y[0].X.Count ||
                matrixCoord.y >= _currentMapData.chunks.Y.Count)
                return null;

            if (_currentLoadedChunks[matrixCoord.y, matrixCoord.x] != null)
                return _currentLoadedChunks[matrixCoord.y, matrixCoord.x];

            var chunk = await Chunk.Chunk.Construct(GetChunkAsset(matrixCoord),
                GetChunkInitPosition(matrixCoord));

            _currentLoadedChunks[matrixCoord.y, matrixCoord.x] = chunk;

            return chunk;
        }

        public async void CreateChunks(bool[,] field, Vector2Int center)
        {
            for (int y = 0; y < field.GetLength(0); ++y)
            {
                for (int x = 0; x < field.GetLength(1); ++x)
                {
                    if (field[y, x])
                    {
                        await CreateChunk(center + new Vector2Int(x - 1, y - 1));
                    }
                }
            }
        }

        public Vector2Int GetClosestChunkMatrixCoord(Vector3 targetPosition)
        {
            Vector3 currentChunkPosition = _chunksInitPositions[0, 0];
            Vector2Int closestChunkMatrixCoord = Vector2Int.zero;
            for (int y = 0; y < _chunksInitPositions.GetLength(0); ++y)
            {
                for (int x = 0; x < _chunksInitPositions.GetLength(1); ++x)
                {
                    var trackedLast = targetPosition - currentChunkPosition;
                    var trackedCurrent = targetPosition - _chunksInitPositions[y, x];
                    if (trackedCurrent.magnitude < trackedLast.magnitude)
                    {
                        currentChunkPosition = _chunksInitPositions[y, x];
                        closestChunkMatrixCoord = new Vector2Int(x, y);
                    }
                }
            }

            return closestChunkMatrixCoord;
        }
        public float ChunkLength => _currentMapData.ChunkBoundLength;
        public Vector3 GetChunkInitPosition(Vector2Int matrixCoord) =>
            _chunksInitPositions[matrixCoord.y, matrixCoord.x];

        public AssetReference GetChunkAsset(Vector2Int matrixCoord) =>
            _currentMapData.chunks.Y[matrixCoord.y].X[matrixCoord.x];
    }
}