using DefaultNamespace;
using DefaultNamespace.UIManagement.GameplayHud;
using Movement.Input.Base;
using Zenject;

namespace DI
{

    public class GameplayInstaller : MonoInstaller
    {
        
        public override void InstallBindings()
        {
            Container.Bind<Timer.Timer>().AsTransient().OnInstantiated((context, o) =>
            {
                context.Container.Resolve<TickableManager>().Add(o as ITickable);
            } );


            Container.Bind<IMovementInputManager>().To<PlayerMovementInputManager>().AsSingle();
            Container.Bind<GameRuleManager>().AsSingle().NonLazy();
            Container.Bind<TimerUIManager>().AsSingle().NonLazy();
        }
    }
}