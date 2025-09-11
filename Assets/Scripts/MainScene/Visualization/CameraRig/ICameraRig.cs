using UnityEngine;

namespace Game.Scene.Main
{
    public interface ICameraRig
    {
        public Camera MainCamera { get; }

        void Rotate(Vector2 lookDelta);
        void Zoom(float delta);
        void UpdateTransform();
    }
}
