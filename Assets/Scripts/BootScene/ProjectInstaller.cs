using Game.Data.Managers;
using Game.Scene.Loading;
using Game.Services;
using Game.Services.GPU;
using Game.Services.Managers;
using Game.Testing;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Zenject;

namespace Game
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Services")]
        [Required][SerializeField] private TestingService _testingService;
        [Required][SerializeField] private LoadingManager _loadingManager;
        [Required][SerializeField] private ResourcesService _resourcesService;
        [Required][SerializeField] private OpenXRService _openXRService;
        [Required][SerializeField] private InputService _inputService;
        [Required][SerializeField] private EventSystemController _eventSystemController;
        [Required][SerializeField] private TranslateService _translateService;
        [Required][SerializeField] private SteamService _steamService;
        [Required][SerializeField] private DebugAudioService _audioService;
        [Required][SerializeField] private GPUGlobePerlin2D _gpuGlobePerlin2D;
        [Required][SerializeField] private ScenesManager _scenesManager;
        [Required][SerializeField] private MenuWindowsManager _menuWindowsManager;
        [Required][SerializeField] private GPUBlockWall _gpuBlockWallService;
        [Required][SerializeField] private BlockManager _blockManager;

        private IEnvironmentService _environmentService = new EnvironmentService();

        public override void InstallBindings()
        {
            BindInstance(_testingService);
            //BindInstance<LoadingManager>(_loadingManager);
            BindInstance(_loadingManager);

            //Services
            BindInstance(_resourcesService);
            BindInstance(_openXRService);
            BindInstance(_inputService);
            BindInstance(_environmentService);
            BindInstance(_eventSystemController);
            BindInstance(_translateService);
            BindInstance(_steamService);
            BindInstance(_audioService);
            BindInstance(_scenesManager);
            BindInstance(_menuWindowsManager);

            BindInstance(_gpuGlobePerlin2D);
            BindInstance(_gpuBlockWallService);

            BindInstance(_blockManager);

            InitializeTestingService();
        }
        private void InitializeTestingService() 
        {
            _blockManager.Initialize();

            //Порядок заполнения важен
            _testingService.AddToTestingList(_resourcesService);
            _testingService.AddToTestingList(_openXRService);
            _testingService.AddToTestingList(_inputService);
            _testingService.AddToTestingList(_environmentService);
            _testingService.AddToTestingList(_eventSystemController);
            _testingService.AddToTestingList(_translateService);
            _testingService.AddToTestingList(_steamService);
            _testingService.AddToTestingList(_audioService);
            _testingService.AddToTestingList(_scenesManager);
            _testingService.AddToTestingList(_menuWindowsManager);

            _testingService.AddToTestingList(_gpuGlobePerlin2D);

            _testingService.AddToTestingList(_blockManager);
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
