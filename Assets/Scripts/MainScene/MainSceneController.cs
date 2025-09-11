using Game.Services;
using Game.Services.Managers;
using Game.UI.Menu;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Scene.Main
{
    public class MainSceneController: MonoBehaviour
    {
        [Required][SerializeField] private readonly Vector3 _forwardOffsetVR = new Vector3(0, -0.20f, 1.0f);
        [Required][SerializeField] Transform _parentCameraRig;

        [Header("Prefabs CameraRig")]
        [Required][SerializeField] CameraRigPC _prefabCameraRigPC;
        [Required][SerializeField] CameraRigVR _prefabCameraRigVR;

        private ICameraRig _cameraRig;
        private IInputAdapter _inputAdapter;

        private IMenuWindowsManager _menuWindowsManager;
        private IEnvironmentService _environmentService;

        [Inject]
        private void Construct(IEnvironmentService environmentService, IMenuWindowsManager menuWindowsManager) 
        {
            _menuWindowsManager = menuWindowsManager;
            _environmentService = environmentService;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            InitializeCameraRig();
            InitializeInputAdapter();
        }

        private async void InitializeCameraRig()
        {
            switch (_environmentService.runtimeMode)
            {
                case RuntimeMode.PC:
                    _cameraRig = Instantiate(_prefabCameraRigPC, _parentCameraRig);
                    break;

                case RuntimeMode.VR:
                    _cameraRig = Instantiate(_prefabCameraRigVR, _parentCameraRig);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Yield();
            _menuWindowsManager.CanvasControllerVR.ResetTransform(_cameraRig.MainCamera, _forwardOffsetVR, null, false, true, false);
        }
        private void InitializeInputAdapter()
        {
            switch (_environmentService.runtimeMode)
            {
                case RuntimeMode.PC:
                    //надо будет передать input map
                    _inputAdapter = new InputAdapterPC();
                    break;

                case RuntimeMode.VR:
                    _inputAdapter = new InputAdapterVR();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
