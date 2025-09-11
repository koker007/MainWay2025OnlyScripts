using UnityEngine;

namespace Game.Tools.VR
{
    public interface ICanvasControllerVR
    {
        public void ResetTransform(Camera camera, Vector3 cameraForwardsOffset, Transform parent, bool isRotateAxisX, bool isRotateAxisY, bool isRotateAxisZ);
        public void ResetTransform(Camera camera, Vector3 cameraForwardsOffset, Transform parent, Quaternion rotateToMove);
    }
}
