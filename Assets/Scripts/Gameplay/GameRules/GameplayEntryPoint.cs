using System;
using System.Collections.Generic;
using Character;
using Cinemachine;
using Gameplay.MapManagement.Graph;
using Movement.Fsm.States;
using Movement.Fsm.View;
using Movement.Input.Base;
using Player.Data;
using ResourceManagement;
using ResourceManagement.Data;
using TerrainGeneration;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class GameplayEntryPoint
    {
        public Action GameEnded;
        
        private IMovementInputManager _playerInputManager;
        

        private List<FsmMovementView> _charactersMovement = new();

        public Action<Transform> PlayerCreated;

        [Inject]
        public void Initialize(GameplayAssetPreloader gameplayAssetPreloader,
            IMovementInputManager inputManager,
            CinemachineVirtualCamera cinemachineVirtualCamera,
            Timer.Timer timer,
            GraphManager graphManager)
        {
            _playerInputManager = inputManager;

            PlayerCreated += transform =>
            {
                cinemachineVirtualCamera.Follow = transform;
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                    new Vector3(0, 20, -10);
            };
            PlayerCreated += transform =>
            {
                var node1 = graphManager.FindNearestNode(transform.position);
                var node2 = graphManager.FindNearestNode(transform.position + new Vector3(10, 10, 10));

                AStarPathfinder.FindPath(node1, node2);
            };

        gameplayAssetPreloader.StartPreloadingAsset(AssetName.Player.ToString(), PlayerAssetDownloaded);

            StartGame();
        }

        public void StartGame()
        {
            GameEnded += BlockAllCharacters;
        }
        
        private void PlayerAssetDownloaded(ScriptableObject asset)
        {
            var character = CharacterFactory.CreateCharacter(((CharacterDataAsset)asset).prefab, _playerInputManager);

            _charactersMovement.Add(character.GetComponent<FsmMovementView>());

            PlayerCreated?.Invoke(character.transform);
        }

        private void BlockAllCharacters()
        {
            if (_charactersMovement.Count == 0)
                return;
            
            for (int i = 0; i < _charactersMovement.Count; ++i)
            {
                _charactersMovement[i].Fsm.SetState<FsmStateBlocked>();
            }
        }

    }
}