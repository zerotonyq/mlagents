using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        public Action<MapData> MapDataLoaded;

        [Inject]
        public void Initialize(GameplayAssetPreloader gap)
        {
            gap.StartPreloadingAsset(AssetName.MapData.ToString(), (ScriptableObject so) =>
            {
                _currentMapData = so as MapData;
                MapDataLoaded?.Invoke(_currentMapData);
                InitChunkPositions();
            });
        }


        private void InitChunkPositions()
        {
            _chunksInitPositions = new Vector3[_currentMapData.chunks.Y[0].X.Count, _currentMapData.chunks.Y.Count];
            _currentLoadedChunks = new Chunk.Chunk[_currentMapData.chunks.Y[0].X.Count, _currentMapData.chunks.Y.Count];

            Vector3 currentPosition =
                new Vector3(0, 0, (_currentMapData.chunks.Y.Count - 1) * _currentMapData.ChunkBoundLength);

            for (int y = 0; y < _currentMapData.chunks.Y.Count; ++y)
            {
                for (int x = 0; x < _currentMapData.chunks.Y[y].X.Count; ++x)
                {
                    Debug.Log(currentPosition);
                    _chunksInitPositions[y, x] = currentPosition;
                    currentPosition += new Vector3(_currentMapData.ChunkBoundLength, 0, 0);
                }

                currentPosition = new Vector3(0, 0, currentPosition.z);
                currentPosition -= new Vector3(0, 0, _currentMapData.ChunkBoundLength);
            }
            
        }

        public async Task<Chunk.Chunk> CreateChunk(Vector2Int matrixCoord)
        {
            if (!ValidateCoordinate(matrixCoord))
                return null;

            if (_currentLoadedChunks[matrixCoord.y, matrixCoord.x] != null)
            {
                _currentLoadedChunks[matrixCoord.y, matrixCoord.x].Enable();
                return _currentLoadedChunks[matrixCoord.y, matrixCoord.x];
            }
                

            var chunk = await Chunk.Chunk.ConstructFromPrefab(GetChunkAsset(matrixCoord),
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

        //O(n)
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

        //O(1)
        public (Chunk.Chunk, Vector2Int) GetChunkAround(Vector2Int centerCoordinate, Vector3 trackedPosition)
        {
            Chunk.Chunk currentClosest = _currentLoadedChunks[centerCoordinate.y, centerCoordinate.x];

            float minDistance = _currentMapData.ChunkBoundLength * 2;
            Vector2Int newCenter = centerCoordinate;
            for (int y = -1; y <= 1; ++y)
            {
                for (int x = -1; x <= 1; ++x)
                {
                    var currentCoordinate = centerCoordinate + new Vector2Int(x, y);

                    if (!ValidateCoordinate(currentCoordinate))
                        continue;

                    var currentDistance =
                        (trackedPosition - GetChunkInitPosition(currentCoordinate)).magnitude;

                    if (currentDistance < minDistance &&
                        _currentLoadedChunks[currentCoordinate.y, currentCoordinate.x] != null)
                    {
                        minDistance = currentDistance;
                        currentClosest = _currentLoadedChunks[currentCoordinate.y, currentCoordinate.x];
                        newCenter = currentCoordinate;
                    }
                }
            }

            return (currentClosest, newCenter);
        }

        private bool ValidateCoordinate(Vector2Int coord)
        {
            return coord.x >= 0 &&
                   coord.y >= 0 &&
                   coord.x < _currentMapData.chunks.Y[0].X.Count &&
                   coord.y < _currentMapData.chunks.Y.Count;
        }

        public float ChunkLength => _currentMapData.ChunkBoundLength;

        public Vector3 GetChunkInitPosition(Vector2Int matrixCoord) =>
            _chunksInitPositions[matrixCoord.y, matrixCoord.x];


        public AssetReference GetChunkAsset(Vector2Int matrixCoord) =>
            _currentMapData.chunks.Y[matrixCoord.y].X[matrixCoord.x];
        
        public void HideChunks(Vector2Int centerCoord, int nonHideDistance)
        {
            Debug.Log("HIDE " + centerCoord);
            var hideDelta = nonHideDistance + 1;
            for (int y = -1 * hideDelta; y <= hideDelta; ++y)
            {
                for (int x = -1 * hideDelta; x <= hideDelta; ++x)
                {
                    if(Math.Abs(y) != hideDelta && Math.Abs(x) != hideDelta)
                        continue;
                    
                    var currentCoord = centerCoord + new Vector2Int(x, y);
                    
                    if(!ValidateCoordinate(currentCoord) || _currentLoadedChunks[currentCoord.y, currentCoord.x] == null)
                        continue;
                        
                    _currentLoadedChunks[currentCoord.y, currentCoord.x].Disable();
                }
            }
        }
    }
}