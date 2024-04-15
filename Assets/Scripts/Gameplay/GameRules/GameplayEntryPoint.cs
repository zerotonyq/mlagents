using System;
using System.Collections.Generic;
using Character;
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


        private IMovementInputManager _playerInputManager;

        private List<FsmMovementView> _charactersMovement = new();

        public Action<Transform> PlayerCreated;

        [Inject]
        public void Initialize(GameplayAssetPreloader gameplayAssetPreloader, IMovementInputManager inputManager)
        {
            _playerInputManager = inputManager;

            gameplayAssetPreloader.StartPreloadingAsset(AssetName.Player.ToString(), PlayerAssetDownloaded);
        }

        public void StartGame(float time)
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