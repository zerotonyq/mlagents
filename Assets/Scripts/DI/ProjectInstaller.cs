using ResourceManagement;
using Zenject;

namespace DI
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerInputActions>().AsSingle().NonLazy();
            Container.Bind<GameplayAssetPreloader>().AsSingle().NonLazy();    
        }
    }
}