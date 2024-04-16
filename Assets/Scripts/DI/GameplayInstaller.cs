using Cinemachine;
using DefaultNamespace;
using Map;
using Movement.Input.Base;
using UnityEngine;
using UnityEngine.Serialization;
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
            Container.Bind<ITickable>().To<ChunkTracker>().AsSingle().NonLazy();
        }
    }
}