using FsmBase;
using ResourceManagement;

namespace StartScreen.Fsm.States.Base
{
    public abstract class FsmStateStartScreen : FsmState
    {
        protected GameplayAssetPreloader AssetPreloader;
        protected PlayerInputActions PlayerInputActions;
        public FsmStateStartScreen(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm)
        {
            AssetPreloader = assetPreloader;
            PlayerInputActions = playerInputActions;
        }
    }
}