﻿using System;
using DefaultNamespace.UIManagement.GameplayHud.Data;
using ResourceManagement;
using ResourceManagement.Data;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace DefaultNamespace.UIManagement.GameplayHud
{
    public class TimerUIManager
    {
        private TextMeshProUGUI _timerText;
        private GameRuleManager _gameRuleManager;
        
        [Inject]
        public void Initialize(GameRuleManager gameRuleManager, GameplayAssetPreloader gameplayAssetPreloader)
        {
            _gameRuleManager = gameRuleManager;
            
            gameplayAssetPreloader.StartPreloadingAsset(AssetName.TimerData.ToString(), TimerAssetDownloaded);
        }
        
        private void TimerAssetDownloaded(ScriptableObject so)
        {
            _timerText = CreateTimerText(so as TimerDataAsset);

            _gameRuleManager.Timer.TimeUpdated += UpdatingTimerText;

            _gameRuleManager.GameEnded += EndTimerText;
        }

        private void UpdatingTimerText(float a) => _timerText.text = "REMAINING TIME: " + Math.Round(a, 2);

        private void EndTimerText() => _timerText.text = "GAME ENDED";

        private TextMeshProUGUI CreateTimerText(TimerDataAsset timerDataAsset) =>
            GameObject.Instantiate(timerDataAsset.TimerTextCanvas.Canvas).GetComponentInChildren<TextMeshProUGUI>();
    }
}
