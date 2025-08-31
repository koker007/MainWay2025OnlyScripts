using Game.Data.Block;
using Game.Services;
using Game.UI;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockLoaderBlockInfo: MonoBehaviour, IPointerClickHandler
    {
        private const string EXEPTION_WRONG_TYPE = "Wrong type for block";

        [Required][SerializeField] private UITextBackground _name;
        [Required][SerializeField] private UITextBackground _id;
        [Required][SerializeField] private UITextBackground _tBlockCount;
        [Required][SerializeField] private UITextBackground _tVoxelCount;
        [Required][SerializeField] private UITextBackground _tLiquidCount;
        [Required][SerializeField] private UITextBackground _variations;

        private EditorBlockPanelLoader _editorBlockPanelLoader;
        private IEnvironmentService _environmentService;
        private BlockData[] _blockDataArray;
        private int _blockID;

        public void Initialize(EditorBlockPanelLoader editorBlockPanelLoader, IEnvironmentService environmentService, BlockData[] blockDataArray, int blockID)
        {
            _editorBlockPanelLoader = editorBlockPanelLoader;
            _environmentService = environmentService;
            _blockDataArray = blockDataArray;
            _blockID = blockID;

            _name.SetText(blockDataArray[0].name);

            int countTBlock = 0;
            int countTVoxels = 0;
            int countTLiquid = 0;

            foreach (BlockData blockData in blockDataArray)
            {
                switch (blockData.type)
                {
                    case BlockData.Type.block:
                        countTBlock++;
                        break;
                    case BlockData.Type.voxels:
                        countTVoxels++;
                        break;
                    case BlockData.Type.liquid:
                        countTLiquid++;
                        break;
                    default:
                        throw new ArgumentException(EXEPTION_WRONG_TYPE);
                }
            }

            _id.SetText(_blockID.ToString());
            _tBlockCount.SetText(countTBlock.ToString());
            _tVoxelCount.SetText(countTVoxels.ToString());
            _tLiquidCount.SetText(countTLiquid.ToString());
            _variations.SetText(_blockDataArray.Length.ToString());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_environmentService.runtimeMode == RuntimeMode.PC)
            {
                _editorBlockPanelLoader.SelectData(_blockDataArray);
                _editorBlockPanelLoader.OnSelectBlockToLoad();
            }
            else if(_environmentService.runtimeMode == RuntimeMode.VR)
            {
                _editorBlockPanelLoader.SelectData(_blockDataArray);
                _editorBlockPanelLoader.OnSelectBlockToLoad();
            }
        }
    }
}
