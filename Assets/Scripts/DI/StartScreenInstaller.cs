using CameraRaycasting;
using StartScreen.Fsm;
using Zenject;

public class StartScreenInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FsmStartScreen>().AsSingle().NonLazy();
        Container.Bind<CameraRaycaster>().AsSingle().NonLazy();
    }
}