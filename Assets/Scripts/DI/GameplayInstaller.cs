using DefaultNamespace;
using Map;
using Movement.Input.Base;
using Zenject;

namespace DI
{

    public class GameplayInstaller : MonoInstaller
    {
        
        public override void InstallBindings()
        {
            Container.Bind<IMovementInputManager>().To<PlayerMovementInputManager>().AsSingle();
            Container.Bind<GameplayEntryPoint>().AsSingle().NonLazy();
            Container.Bind<ChunkLoader>().AsSingle().NonLazy();
            Container.Bind<ITickable>().To<ChunkTracker>().AsSingle().NonLazy();
        }
    }
}