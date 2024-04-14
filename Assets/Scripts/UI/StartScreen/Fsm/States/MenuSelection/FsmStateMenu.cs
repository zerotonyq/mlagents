﻿using ResourceManagement;
using ResourceManagement.Data;
using StartScreen.Fsm.States.Base;
using StartScreen.Fsm.States.CanvasContainer;
using UI.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartScreen.Fsm.States
{
    public class FsmStateMenu : FsmStateUI
    {

        private MenuContainer _menuContainer;
        public FsmStateMenu(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) :
            base(fsm, assetPreloader, playerInputActions)
        {
        }

        public override void Enter()
        {
            if (!_menuContainer)
                LoadScrollView();
            else
                _menuContainer.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            _menuContainer.gameObject.SetActive(false);
        }

        private void LoadScrollView()
        {
            AssetPreloader.StartPreloadingAsset(AssetName.Menu.ToString(), (GameObject o) =>
            {
                _menuContainer = GameObject.Instantiate(o, null).GetComponent<MenuContainer>();
                _menuContainer.playButton.Triggered += () => SceneManager.LoadSceneAsync(1);
            });
        }
    }
}