using Game.Scene.Editor.Block;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scene.Editor.Block
{
    public class InputAdapterPC : IInputAdapter
    {
        private const float LOOK_SENSITIVITY = 300f;

        private readonly EditorBlockSceneControls.InputAdapterPCActions _map;

        private Vector2 _lookOld = Vector2.zero;

        public Vector2 LookDelta => GetLookDelta();
        public float ZoomDelta => _map.Zoom.ReadValue<float>();

        public InputAdapterPC(EditorBlockSceneControls controls)
        {
            _map = controls.InputAdapterPC;
            _map.IsChangeLook.Enable();
            _map.Look.Enable();
            _map.Zoom.Enable();
        }

        private Vector2 GetLookDelta() 
        {
            bool IsChangeLook = _map.IsChangeLook.ReadValue<float>() > 0;
            Vector2 look = _map.Look.ReadValue<Vector2>();

            Vector2 result = Vector2.zero;

            if (IsChangeLook) 
            {
                result = (look - _lookOld)/Screen.width * LOOK_SENSITIVITY;
            }

            _lookOld = look;

            return result;
        }
    }
}
