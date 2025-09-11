using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Scene.Main
{
    public class CameraRigPC : MonoBehaviour, ICameraRig
    {
        [Required][SerializeField] private Camera _camera;

        public Camera MainCamera => _camera;

        public void Rotate(Vector2 lookDelta)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateTransform()
        {
            throw new System.NotImplementedException();
        }

        public void Zoom(float delta)
        {
            throw new System.NotImplementedException();
        }
    }
}
