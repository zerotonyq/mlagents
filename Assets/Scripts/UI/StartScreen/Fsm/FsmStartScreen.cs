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
        public void Initialize(GameplayAssetPreloader assetPreloader, PlayerInputActions playerInputActions)
        {
            _fsm = new FsmBase.Fsm();

            _fsm.AddState(new FsmStateIntroScreen(_fsm, assetPreloader, playerInputActions));
            _fsm.AddState(new FsmStateSettings(_fsm, assetPreloader, playerInputActions));
            _fsm.AddState(new FsmStateMenu(_fsm, assetPreloader, playerInputActions));

            _fsm.SetState<FsmStateIntroScreen>();


            assetPreloader.StartPreloadingAsset(AssetName.StartScreenFill.ToString(), (GameObject o) =>
            {
                var fillCanvas = GameObject.Instantiate(o, null).GetComponent<Canvas>();
                fillCanvas.worldCamera = Camera.main;
            });
        }

        public FsmBase.Fsm Fsm => _fsm;
    }
}