using ResourceManagement;
using ResourceManagement.Data;
using StartScreen.Fsm.States.Base;
using UI.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace StartScreen.Fsm.States
{
    public class FsmStateMenu : FsmStateUI
    {
        private Canvas _scrollView;
        public FsmStateMenu(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : 
            base(fsm, assetPreloader, playerInputActions)
        {
        }

        public override void Enter()
        {
            if (!_scrollView)
            {
                LoadScrollView();
            }
            else
            {
                _scrollView.gameObject.SetActive(true);
                _scrollView.enabled = true;
            }
        }

        public override void Exit()
        {
            _scrollView.enabled = false;
            _scrollView.gameObject.SetActive(false);
        }

        private void LoadScrollView()
        {
            AssetPreloader.StartPreloadingAsset(AssetName.StartScreenScrollView.ToString(), (GameObject o) =>
            {
                _scrollView = GameObject.Instantiate(o, null).GetComponent<Canvas>();
                _scrollView.worldCamera = Camera.main;
            });
        }
    }
}