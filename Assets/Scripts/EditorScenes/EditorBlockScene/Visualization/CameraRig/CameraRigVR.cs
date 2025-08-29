using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Scene.Editor.Block
{
    public class CameraRigVR : MonoBehaviour, ICameraRig
    {
        private readonly Vector3 _targetPosition = new Vector3(0.5f, 0.5f, 0.5f);
        private const float DISTANCE_TO_BLOCK_MININUM = 1.3f;
        private const float DISTANCE_TO_BLOCK_MAXIMUM = 10.0f;
        private const float SENSITIVITY_ZOOM = 0.1f;

        private float _yaw;
        private float _pitch;
        private float _distance = 2f;

        private float _lastTurnTime;
        private float _turnCooldown = 0.5f; // полсекунды между рывками
        private float _snapAngle = 45f;     // угол одного рывка

        [Required][SerializeField] Camera _camera;

        public Camera MainCamera => _camera;

        public void Rotate(Vector2 lookDelta)
        {
            if (Time.unscaledTime - _lastTurnTime < _turnCooldown)
                return;

            if (lookDelta.x > 0.5f)
            {
                _yaw -= _snapAngle;
                _lastTurnTime = Time.unscaledTime;
            }
            else if (lookDelta.x < -0.5f)
            {
                _yaw += _snapAngle;
                _lastTurnTime = Time.unscaledTime;
            }

            if (lookDelta.y > 0.5f)
            {
                _pitch = Mathf.Clamp(_pitch + lookDelta.y * _snapAngle, -80f, 80f);
                _lastTurnTime = Time.unscaledTime;
            }
            else if (lookDelta.y < -0.5f)
            {
                _pitch = Mathf.Clamp(_pitch + lookDelta.y * _snapAngle, -80f, 80f);
                _lastTurnTime = Time.unscaledTime;
            }
        }

        public void Zoom(float delta)
        {
            if (Mathf.Abs(delta) < 0.5f)
                return;

            _distance = Mathf.Clamp(_distance - delta * SENSITIVITY_ZOOM, DISTANCE_TO_BLOCK_MININUM, DISTANCE_TO_BLOCK_MAXIMUM);
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
