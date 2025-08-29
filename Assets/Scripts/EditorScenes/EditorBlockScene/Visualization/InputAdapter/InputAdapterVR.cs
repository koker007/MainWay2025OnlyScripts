using UnityEngine;

namespace Game.Scene.Editor.Block
{
    public class InputAdapterVR : IInputAdapter
    {
        private readonly EditorBlockSceneControls.InputAdapterVRActions _map;

        public Vector2 LookDelta => _map.Look.ReadValue<Vector2>();
        public float ZoomDelta => GetZoomDelta();

        public InputAdapterVR(EditorBlockSceneControls controls)
        {
            _map = controls.InputAdapterVR;
            _map.Look.Enable();
            _map.Zoom.Enable();
        }

        private float GetZoomDelta() 
        {
            Vector2 leftStick = _map.Zoom.ReadValue<Vector2>();
            return leftStick.y;
        }
    }
}
