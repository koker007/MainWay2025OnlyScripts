using Game.Scene.Editor.Block;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Scene.Main
{
    public class MainSceneInstaller: MonoInstaller
    {
        [Required][SerializeField] private MainSceneController _mainSceneController;

        public override void InstallBindings()
        {
            BindInstance(_mainSceneController);
            //BindInstance<LoadingManager>(_loadingManager);
        }

        private void BindInstance<T>(object instanceObject)
        {
            Container.BindInterfacesAndSelfTo<T>()
                .FromInstance(instanceObject)
                .AsSingle()
                .NonLazy();
        }
        private void BindInstance(object instanceObject)
        {
            var type = instanceObject.GetType();

            Container.BindInterfacesAndSelfTo(type)
                .FromInstance(instanceObject)
                .AsSingle()
                .NonLazy();
        }
    }
}
