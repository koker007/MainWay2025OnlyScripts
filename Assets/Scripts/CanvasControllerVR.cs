using Game.Services;
using Sirenix.OdinInspector;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.Tools.VR
{
    public class CanvasControllerVR: MonoBehaviour, ICanvasControllerVR
    {
        [Required][SerializeField] private Canvas _canvas;
        [Required][SerializeField] private Vector2 _pixels = new Vector2 (1920, 1080);
        [Required][SerializeField] private float _size = 1.0f;

        [Header("UpdateData")]
        [SerializeField] private bool _updatePosition = false;
        [SerializeField] private Vector3 _forwardOffset = new Vector3(0,0,1);
        [SerializeField] private bool _updateRotate = false;
        [SerializeField] private Vector3 _eulerRotate = new Vector3(0,0,0); 


        RectTransform _rectTransform;
        Transform _camera;
        Vector3 _cameraForwardsOffset;
        Transform _parent;
        Quaternion _canvasRotate;

        private Vector3 _canvasScale = new Vector3(1,1,1);

        private IEnvironmentService _environmentService;

        [Inject]
        public void Construct(IEnvironmentService environmentService) 
        {
            _environmentService = environmentService;
        }

        public void Start()
        {
            Initialize(destroyCancellationToken);
        }
        private void Update()
        {
            UpdateData();
        }

        public async void Initialize(CancellationToken token) 
        {
            while (!_environmentService.isInitialized)
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (_environmentService.runtimeMode != RuntimeMode.VR) 
            {
                Destroy(this);
                return;
            }

            _rectTransform ??= _canvas.GetComponent<RectTransform>();

            Rect rect = _rectTransform.rect;
            rect.width = _pixels.x;
            rect.height = _pixels.y;

            _canvas.renderMode = RenderMode.WorldSpace;

            _canvasScale = _size / _canvas.renderingDisplaySize.x * Vector3.one;

            _canvas.transform.localScale = _canvasScale;
        }

        public void ResetTransform(Camera camera, Vector3 cameraForwardsOffset, Transform parent, bool isRotateAxisX, bool isRotateAxisY, bool isRotateAxisZ)
        {
            Quaternion quaternion = camera.transform.rotation;
            Vector3 eulerAngles = camera.transform.eulerAngles;

            if (!isRotateAxisX)
                eulerAngles.x = 0;
            if (!isRotateAxisY)
                eulerAngles.y = 0;
            if (!isRotateAxisZ)
                eulerAngles.z = 0;

            quaternion.eulerAngles = eulerAngles;

            ResetTransform(camera, cameraForwardsOffset, parent, quaternion);
        }
        public void ResetTransform(Camera camera, Vector3 cameraForwardsOffset, Transform parent, Quaternion rotateToMove)
        {
            _camera = camera.transform;
            if (cameraForwardsOffset != Vector3.zero)
                _forwardOffset = cameraForwardsOffset;

            if (camera != null && _canvas.worldCamera != camera)
                _canvas.worldCamera = camera;

            if (parent != null)
            {
                _canvas.gameObject.transform.parent = parent;
                _canvas.gameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                _canvas.transform.localPosition = _camera.transform.localPosition;
            }

            _canvas.transform.rotation = rotateToMove;

            _canvas.transform.Translate(_forwardOffset, Space.Self);
        }

        public void UpdateData()
        {
            //Надо обновить позицию канваса относительно камеры
            if (_updatePosition)
            {
                _canvas.gameObject.transform.localPosition = Vector3.zero;
                _canvas.transform.Translate(_forwardOffset, Space.Self);
            }

            if (_updateRotate) 
            {
                Quaternion quaternion = _canvas.gameObject.transform.localRotation;
                quaternion.eulerAngles = _eulerRotate;
                _canvas.gameObject.transform.localRotation = quaternion;
            }

        }
    }
}
