using System;
using ResourceManagement;
using ResourceManagement.Data;
using StartScreen.Fsm.States.Base;
using UI.Data;
using UnityEngine;

namespace StartScreen.Fsm.States
{
    public class FsmStateIntroScreen : FsmStateUI
    {
        private Canvas _introScreenCanvas;

        public FsmStateIntroScreen(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm, assetPreloader, playerInputActions)
        {
        }

        public override void Enter()
        {
            if (!_introScreenCanvas)
            {
                LoadIntroScreen();
            }
            else
            {
                _introScreenCanvas.gameObject.SetActive(true);
                _introScreenCanvas.enabled = true;
            }

            PlayerInputActions.Menu.StartScreenPress.Enable();
            PlayerInputActions.Menu.StartScreenPress.performed += _ =>
            {
                Exit();
                Fsm.SetState<FsmStateMenu>();
                PlayerInputActions.Menu.StartScreenPress.Disable();
            };
        }

        public override void Exit()
        {
            _introScreenCanvas.enabled = false;
            _introScreenCanvas.gameObject.SetActive(false);
        }

        private void LoadIntroScreen()
        {
            AssetPreloader.StartPreloadingAsset(AssetName.StartScreenPressAnyButton.ToString(), (GameObject o)  =>  
            {
                _introScreenCanvas = GameObject.Instantiate(o, null).GetComponent<Canvas>();
                _introScreenCanvas.worldCamera = Camera.main;
            });
        }
    }
}