using System;
using System.Collections.Generic;
using Character;
using Cinemachine;
using Movement.Fsm.States;
using Movement.Fsm.View;
using Movement.Input.Base;
using Player.Data;
using ResourceManagement;
using ResourceManagement.Data;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class GameplayEntryPoint
    {
        public Action GameEnded;

        private List<FsmMovementView> _charactersMovement = new();

        private IMovementInputManager _playerInputManager;
        private AddressableAssetPreloader _addressableAssetPreloader;
        private CinemachineVirtualCamera _mainCamera;
        
        public Action<Transform> PlayerCreated;

        [Inject]
        public void Initialize(AddressableAssetPreloader addressableAssetPreloader,
            IMovementInputManager inputManager,
            CinemachineVirtualCamera cinemachineVirtualCamera,
            Timer.Timer timer)
        {
            _playerInputManager = inputManager;
            _addressableAssetPreloader = addressableAssetPreloader;
            _mainCamera = cinemachineVirtualCamera;
            
            StartGame();
        }

        private void StartGame()
        {
            PlayerCreated += transform =>
            { 
                _mainCamera.Follow = transform;
                _mainCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                    new Vector3(0, 20, 10);
            };

            _addressableAssetPreloader.StartPreloadingAsset(GameplayAssetName.Player.ToString(), PlayerAssetDownloaded);
            GameEnded += BlockAllCharacters;
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

        private void PlayerAssetDownloaded(ScriptableObject asset)
        {
            var character = CharacterFactory.CreateCharacter(((CharacterDataAsset)asset).prefab, _playerInputManager);

            _charactersMovement.Add(character.GetComponent<FsmMovementView>());

            PlayerCreated?.Invoke(character.transform);
        }
    }
}