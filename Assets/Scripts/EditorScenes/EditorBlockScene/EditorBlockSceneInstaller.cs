using Game.Data.Managers;
using Game.Scene.Loading;
using Game.Services;
using Game.Services.GPU;
using Game.Services.Managers;
using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockSceneInstaller : MonoInstaller
    {
        [Required][SerializeField] private EditorBlocksController _editorBlocksController;

        public override void InstallBindings()
        {
            BindInstance(_editorBlocksController);
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
