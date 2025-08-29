using Game.Data.Block;
using Game.Services;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockVisualizer: MonoBehaviour
    {
        [Required][SerializeField] private MeshFilter _meshFilter;
        [Required][SerializeField] private MeshRenderer _meshRenderer;

        private EditorBlocksController _editorBlockController;
        private IResourcesService _resourceService;

        [Inject]
        private void Construct(EditorBlocksController editorBlocksController, IResourcesService resourceService) 
        {
            _editorBlockController = editorBlocksController;
            _resourceService = resourceService;
        }
        private void Start()
        {
            Subscribe();
        }

        private void Subscribe() 
        {
            _editorBlockController.OnDataMeshChange
                .Subscribe(_ => UpdateMesh())
                .AddTo(this);
        }

        private void UpdateMesh() 
        {
            if (_meshFilter.sharedMesh != null)
            {
                //meshFilter.sharedMesh.Clear(false);
                //Destroy(_meshFilter.sharedMesh);

                //Удаляем материалы
                for (int num = 0; num < _meshRenderer.materials.Length; num++)
                {
                    Destroy(_meshRenderer.materials[num]);
                }
            }

            //получаем блок
            BlockData blockData = _editorBlockController.blockData;

            TypeBlock typeBlock = blockData as TypeBlock;

            if (typeBlock != null)
                MeshBlock();

            void MeshBlock()
            {
                //Нужно не получать новый меш а изменять старый, если старый есть
                _meshFilter.sharedMesh = typeBlock.GetMesh(true, true, true, true, true, true, _meshFilter.sharedMesh);
                _meshFilter.sharedMesh.Optimize();

                Material materialBlock = _resourceService.MaterialBlock;

                //применяем материалы к мешу
                _meshRenderer.materials = new Material[6]{
                    materialBlock,
                    materialBlock,
                    materialBlock,
                    materialBlock,
                    materialBlock,
                    materialBlock,
                };

                _meshRenderer.materials[0].mainTexture = typeBlock.wallFace.texture;
                _meshRenderer.materials[1].mainTexture = typeBlock.wallBack.texture;
                _meshRenderer.materials[2].mainTexture = typeBlock.wallRight.texture;
                _meshRenderer.materials[3].mainTexture = typeBlock.wallLeft.texture;
                _meshRenderer.materials[4].mainTexture = typeBlock.wallUp.texture;
                _meshRenderer.materials[5].mainTexture = typeBlock.wallDown.texture;
            }
        }
    }
}
