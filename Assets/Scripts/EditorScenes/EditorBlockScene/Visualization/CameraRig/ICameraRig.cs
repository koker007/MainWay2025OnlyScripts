using UnityEngine;

namespace Game.Scene.Editor.Block
{
    //движение камеры/рига, орбита/пан
    public interface ICameraRig
    {
        public Camera MainCamera { get; }

        void Rotate(Vector2 lookDelta);
        void Zoom(float delta);
        void UpdateTransform();
    }
}
