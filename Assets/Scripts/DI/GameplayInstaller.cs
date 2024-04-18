using Cinemachine;
using DefaultNamespace;
using Map;
using Movement.Input.Base;
using UnityEngine;
using Zenject;

namespace DI
{

    public class GameplayInstaller : MonoInstaller
    {

        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        public override void InstallBindings()
        {
            Container.BindInstance(virtualCamera).AsSingle().NonLazy();
            Container.Bind<IMovementInputManager>().To<PlayerMovementInputManager>().AsSingle();
            Container.Bind<GameplayEntryPoint>().AsSingle().NonLazy();
            Container.Bind<ChunkLoader>().AsSingle().NonLazy();
            
            Container.Bind<Timer.Timer>().AsTransient().OnInstantiated((context, o) =>
            {
                context.Container.Resolve<TickableManager>().Add(o as ITickable);
            } );
            
            Container.Bind<ITickable>().To<MapPositionTracker>().AsSingle().NonLazy();
        }
    }
}