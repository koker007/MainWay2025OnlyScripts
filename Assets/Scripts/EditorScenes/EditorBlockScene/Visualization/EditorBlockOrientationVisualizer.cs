using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;
using Game.Scene.Editor.Block;
using UniRx;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockOrientationVisualizer : MonoBehaviour
    {
        private const float RIGHT_ANGLE = 90.0f;

        [Required][SerializeField] private Transform _up;
        [Required][SerializeField] private Transform _down;

        private EditorBlocksController _editorBlocksController;

        [Inject]
        void Construct(EditorBlocksController editorBlocksController)
        {
            _editorBlocksController = editorBlocksController;
        }

        void Update()
        {
            float angleY = _editorBlocksController.CameraRig.MainCamera.transform.eulerAngles.y;

            int countRot = (int)(angleY / RIGHT_ANGLE);
            //face
            if (countRot == 0)
            {
                if (_editorBlocksController.CameraRig.MainCamera.transform.position.z < 0.0f)
                {
                    SetRot(countRot, _up);
                    SetRot(countRot, _down);
                }
                else
                {
                    SetRot(countRot + 1, _up);
                    SetRot(countRot + 1, _down);
                }
            }
            //Left
            else if (countRot == 1)
            {
                if (_editorBlocksController.CameraRig.MainCamera.transform.position.x < 0.0f)
                {
                    SetRot(countRot, _up);
                    SetRot(countRot, _down);
                }
                else
                {
                    SetRot(countRot + 1, _up);
                    SetRot(countRot + 1, _down);
                }
            }
            //Back
            else if (countRot == 2)
            {
                if (_editorBlocksController.CameraRig.MainCamera.transform.position.z > 1.0f)
                {
                    SetRot(countRot, _up);
                    SetRot(countRot, _down);
                }
                else
                {
                    SetRot(countRot + 1, _up);
                    SetRot(countRot + 1, _down);
                }
            }
            //right
            else
            {
                if (_editorBlocksController.CameraRig.MainCamera.transform.position.x > 1.0f)
                {
                    SetRot(countRot, _up);
                    SetRot(countRot, _down);
                }
                else
                {
                    SetRot(countRot + 1, _up);
                    SetRot(countRot + 1, _down);
                }
            }

            void SetRot(int numPos, Transform transform)
            {
                Vector3 rot = transform.eulerAngles;
                rot.y = numPos * RIGHT_ANGLE;
                transform.eulerAngles = rot;

            }
        }
    }
}