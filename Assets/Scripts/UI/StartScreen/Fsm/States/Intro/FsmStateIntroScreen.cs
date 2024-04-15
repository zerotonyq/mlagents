using System;
using DG.Tweening;
using ResourceManagement;
using ResourceManagement.Data;
using StartScreen.Fsm.States.Base;
using StartScreen.Fsm.States.CanvasContainer;
using UI.Data;
using UnityEngine;

namespace StartScreen.Fsm.States
{
    public class FsmStateIntroScreen : FsmStateUI
    {
        private IntroScreenContainer _introScreenContainer;

        public FsmStateIntroScreen(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm, assetPreloader, playerInputActions)
        {
        }

        public override void Enter()
        {
            if (!_introScreenContainer)
                LoadIntroScreen();
            else
                _introScreenContainer.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            //_introScreenContainer.gameObject.SetActive(false);
        }

        private void LoadIntroScreen()
        {
            AssetPreloader.StartPreloadingAsset(AssetName.Intro.ToString(),
                (GameObject o) =>
                {
                    _introScreenContainer = GameObject.Instantiate(o, null).GetComponent<IntroScreenContainer>();
                    _introScreenContainer.MenuButton.Triggered += () =>
                    {
                        Fsm.SetState<FsmStateMenu>();
                        OpenDoors();
                    };
                });
        }

        private void OpenDoors()
        {
            var door1 = _introScreenContainer.door1;
            var door2 = _introScreenContainer.door2;

            door1.DOMove(door1.transform.position - Vector3.right * 5, 3f);
            door2.DOMove(door2.transform.position + Vector3.right * 5, 3f);
        }
    }
}