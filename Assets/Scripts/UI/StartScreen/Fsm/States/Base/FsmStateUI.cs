using FsmBase;
using ResourceManagement;
using UnityEngine;

namespace StartScreen.Fsm.States.Base
{
    public abstract class FsmStateUI : FsmState
    {
        protected AddressableAssetPreloader AssetPreloader;
        protected PlayerInputActions PlayerInputActions;
        public FsmStateUI(FsmBase.Fsm fsm, AddressableAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm)
        {
            AssetPreloader = assetPreloader;
            PlayerInputActions = playerInputActions;
        }

        
    }
}