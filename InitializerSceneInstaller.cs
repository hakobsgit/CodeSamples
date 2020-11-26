using Controllers;
using DI.Factories;
using DI.Interfaces;
using Zenject;

namespace DI.Installers.Scene {
    public class InitializerSceneInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container.Bind<IPrefabFactory>().To<PrefabFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitializerSceneController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ProgressPopupController>().AsSingle();
        }
    }
}