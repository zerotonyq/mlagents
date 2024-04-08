using ResourceManagement;
using ResourceManagement.Data;
using StartScreen.Fsm.States;
using UI.Data;
using UnityEngine;
using Zenject;

namespace StartScreen.Fsm
{
    public class FsmStartScreen
    {
        private FsmBase.Fsm _fsm;

        [Inject]
        public void Initialize(GameplayAssetPreloader assetPreloader)
        {
            _fsm = new FsmBase.Fsm();
            
            _fsm.AddState(new FsmStateIntroScreen(_fsm));
            _fsm.AddState(new FsmStateMainMenu(_fsm));
            _fsm.AddState(new FsmStateSettings(_fsm));
            _fsm.AddState(new FsmStateLevelSelection(_fsm));
            
            _fsm.SetState<FsmStateIntroScreen>();
            
            assetPreloader.StartPreloadingAsset(AssetName.StartScreenScrollView.ToString(), so =>
            {
                var scroll = GameObject.Instantiate((so as UICanvasAsset).Canvas, null);
                scroll.worldCamera = Camera.main;
                scroll.gameObject.SetActive(false);
            });
            assetPreloader.StartPreloadingAsset(AssetName.StartScreenFill.ToString(), so =>
            {
                var fillCanvas = GameObject.Instantiate((so as UICanvasAsset).Canvas, null);
                fillCanvas.worldCamera = Camera.main;
            });
            
            assetPreloader.StartPreloadingAsset(AssetName.StartScreenPressAnyButton.ToString(), so =>
            {
                var pressAnyButtonCanvas = GameObject.Instantiate((so as UICanvasAsset).Canvas, null);
                pressAnyButtonCanvas.worldCamera = Camera.main;
            });
        }
        
        public FsmBase.Fsm Fsm => _fsm;
    }
}