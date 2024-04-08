using System.Collections;
using System.Collections.Generic;
using StartScreen;
using StartScreen.Fsm;
using UnityEngine;
using Zenject;

public class StartScreenInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FsmStartScreen>().AsSingle();
        
    }
}
