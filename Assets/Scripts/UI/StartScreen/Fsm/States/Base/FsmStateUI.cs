using FsmBase;
using ResourceManagement;
using UnityEngine;

namespace StartScreen.Fsm.States.Base
{
    public abstract class FsmStateUI : FsmState
    {
        protected GameplayAssetPreloader AssetPreloader;
        protected PlayerInputActions PlayerInputActions;
        public FsmStateUI(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm)
        {
            AssetPreloader = assetPreloader;
            PlayerInputActions = playerInputActions;
        }

        
    }
}