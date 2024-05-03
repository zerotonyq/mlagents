using System;
using System.Collections.Generic;
using DefaultNamespace.StormFlower.Data;
using Gameplay.MapManagement.Graph;
using Map;
using TerrainGeneration;
using UniRx;
using UnityEngine;
using Utilities.CoroutineExecutor;
using Zenject;
using Random = UnityEngine.Random;

namespace DefaultNamespace.StormFlower
{
    public class StormFlowerGraphManager
    {
        private List<StormFlowerData> flowersPull = new();
        private List<StormFlowerData> path = new();

        public Action FlowersCreated;
        private Transform _trackedTransform;

        private GraphManager _graphManager;
        private ChunkLoader _chunkLoader;
        private AssignObjectToChunkManager _assignObjectToChunkManager;
        private CoroutineExecutor _coroutineExecutor;

        [Inject]
        public void Initialize(
            GraphManager graphManager,
            ChunkLoader chunkLoader,
            AssignObjectToChunkManager assignObjectToChunkManager, GameplayEntryPoint gameplayEntryPoint)
        {
            _graphManager = graphManager;
            _chunkLoader = chunkLoader;
            _assignObjectToChunkManager = assignObjectToChunkManager;

            gameplayEntryPoint.PlayerCreated += transform => _trackedTransform = transform;
            _coroutineExecutor = new GameObject("executor").AddComponent<CoroutineExecutor>();
        }

        //TODO: refactor
        public void CreateFlowers(int count)
        {
            _coroutineExecutor.Execute((int i) =>
            {
                var randomPositionX = Random.Range(0, _chunkLoader.TerrainData.TerrainLength);
                var randomPositionY = Random.Range(0, _chunkLoader.TerrainData.TerrainLength);

                var node = _graphManager.FindNearestNode(new Vector3(randomPositionX, 0, randomPositionY));

                StormFlowerData currentFlowerData;

                if (i < flowersPull.Count && flowersPull.Count != 0)
                {
                    currentFlowerData = flowersPull[i];
                }
                else
                {
                    var view = new GameObject("flower" + node.Position).AddComponent<StormFlowerView>();
                    currentFlowerData = new StormFlowerData(node, view);
                    flowersPull.Add(currentFlowerData);
                }

                currentFlowerData.View.transform.position = node.Position;
                currentFlowerData.SetNewNode(node);
                _assignObjectToChunkManager.AssignObjectToChunk(currentFlowerData.View.gameObject);
            }, count, 1f);

            _coroutineExecutor.Executed.AddListener((() =>
            {
                ActivateFlowers(count);
                path.Clear();
                path = new();
                ConstructPath(_trackedTransform.position);
                FindPathes();    
            }));
            
        }

        private void FindPathes()
        {
            _coroutineExecutor.Execute((int i) =>
            {
                
                var n1 = path[i];
                var n2 = path[i + 1];
                Func<List<Node>> a = () => AStarPathfinder.FindPath(n1.Node, n2.Node);
                
                var handle = Observable.Start(a);
                Observable.WhenAll(handle)
                    .ObserveOnMainThread() // return to main thread
                    .Subscribe(sx =>
                    {
                        AStarPathfinder.VisualizePath(sx[0]);
                        Debug.Log(n1.Node.Position + " +++++" + n2.Node.Position);
                    });
            }, path.Count - 1, 1f);
            _coroutineExecutor.Executed.AddListener(() => Debug.Log("END"));
            
        }


        public void DeactivateFlowers()
        {
            foreach (var flowerData in flowersPull)
            {
                flowerData.Deactivate();
            }
        }

        public void ActivateFlowers(int count)
        {
            Debug.Log("Act");
            for (int i = 0; i < count; i++)
            {
                flowersPull[i].Activate();
            }
        }

        public void ConstructPath(Vector3 fromPosition)
        {
            float minDistance = 10000f;
            StormFlowerData resultFlower = null;
            for (int i = 0; i < flowersPull.Count; i++)
            {
                var currentFlower = flowersPull[i];

                if (currentFlower.Deactivated)
                    continue;

                if (path.Contains(currentFlower))
                    continue;

                if ((fromPosition - currentFlower.Node.Position).magnitude < minDistance)
                {
                    resultFlower = currentFlower;
                    minDistance = (fromPosition - currentFlower.Node.Position).magnitude;
                }
            }

            if (resultFlower == null)
                return;

            path.Add(resultFlower);

            ConstructPath(resultFlower.Node.Position);
        }

        public void PrintPath()
        {
            Debug.Log("--------------------");
            Debug.Log("FLOWERS PATH: ");
            foreach (var flowerData in path)
            {
                Debug.Log(flowerData.Node.Position);
            }

            Debug.Log("--------------------");
        }
    }
}