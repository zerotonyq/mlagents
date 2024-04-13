using ResourceManagement;
using StartScreen.Fsm.States.Base;

namespace StartScreen.Fsm.States
{
    public class FsmStateSettings : FsmStateUI
    {
        public FsmStateSettings(FsmBase.Fsm fsm, GameplayAssetPreloader assetPreloader,
            PlayerInputActions playerInputActions) : base(fsm, assetPreloader, playerInputActions)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }
    }
}