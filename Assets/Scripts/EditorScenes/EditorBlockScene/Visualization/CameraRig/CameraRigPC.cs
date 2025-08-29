using Game.Scene.Editor.Block;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Sirenix.OdinInspector;

namespace Game.Scene.Editor.Block
{
    public class CameraRigPC : MonoBehaviour, ICameraRig
    {
        private const float DISTANCE_TO_BLOCK_MININUM = 1.3f;
        private const float DISTANCE_TO_BLOCK_MAXIMUM = 10.0f;
        private const float ZOOM_SENSITIVITY = 0.1f;
        private readonly Vector3 _targetPosition = new Vector3(0.5f, 0.5f, 0.5f);

        private float _yaw;
        private float _pitch;
        private float _distance = 2f;

        [Required][SerializeField] Camera _camera;
        public Camera MainCamera => _camera;

        public void Rotate(Vector2 lookDelta)
        {
            _yaw += lookDelta.x;
            _pitch = Mathf.Clamp(_pitch - lookDelta.y, -80f, 80f);
        }

        public void Zoom(float delta)
        {
            _distance = Mathf.Clamp(_distance - delta * _distance * ZOOM_SENSITIVITY, DISTANCE_TO_BLOCK_MININUM, DISTANCE_TO_BLOCK_MAXIMUM);
        }

        public void UpdateTransform()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rotation * new Vector3(0, 0, -_distance);
            gameObject.transform.position = _targetPosition + offset;
            gameObject.transform.LookAt(_targetPosition);
        }
    }
}
