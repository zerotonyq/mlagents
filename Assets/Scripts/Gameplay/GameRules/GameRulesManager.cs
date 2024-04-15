using System;
using System.Collections.Generic;
using Character;
using DefaultNamespace.UIManagement.GameplayHud.Data;
using Movement.Fsm.States;
using Movement.Fsm.View;
using Movement.Input.Base;
using Player.Data;
using ResourceManagement;
using ResourceManagement.Data;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace DefaultNamespace
{
    public class GameRulesManager
    {
        public Action GameEnded;

        private Timer.Timer _timer;
        private IMovementInputManager _playerInputManager;

        private List<FsmMovementView> _charactersMovement = new();

        public Action<Transform> PlayerCreated;

        [Inject]
        public void Initialize(GameplayAssetPreloader gameplayAssetPreloader, Timer.Timer timer,
            IMovementInputManager inputManager)
        {
            _playerInputManager = inputManager;
            _timer = timer;

            gameplayAssetPreloader.StartPreloadingAsset(AssetName.TimerData.ToString(),
                scriptableObject => { StartGame(((TimerDataAsset)scriptableObject).Time); });

            gameplayAssetPreloader.StartPreloadingAsset(AssetName.Player.ToString(), PlayerAssetDownloaded);
        }

        private void PlayerAssetDownloaded(ScriptableObject asset)
        {
            var character = CharacterFactory.CreateCharacter(((CharacterDataAsset)asset).prefab, _playerInputManager);
            PlayerCreated?.Invoke(character.transform);
            _charactersMovement.Add(character.GetComponent<FsmMovementView>());
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

        public void StartGame(float time)
        {
            _timer.SetTime(time);
            _timer.StartTimer();
            GameEnded += BlockAllCharacters;
            _timer.EndTimer += () => GameEnded?.Invoke();
        }

        public Timer.Timer Timer => _timer;
    }
}