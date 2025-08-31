using Game.Data.Block;
using Game.Data.Managers;
using Game.Services;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelLoader: MonoBehaviour, IEditorBlockPanelMiddle
    {
        [Required][SerializeField] private EditorBlockPanelLoaderWarning _panelWarning;
        [Header("Prefabs")]
        [Required][SerializeField] private EditorBlockLoaderModInfo _prefabModInfo;
        [Required][SerializeField] private EditorBlockLoaderBlockInfo _prefabBlockInfo;
        [Space]
        [Required][SerializeField] private RectTransform _content;

        private BlockData[] _selectedData;
        private readonly List<GameObject> _elements = new List<GameObject>();

        BlockManager _blockManager;
        EditorBlocksController _editorBlocksController;
        IEnvironmentService _environmentService;

        public bool IsOpen => gameObject.activeInHierarchy;

        [Inject]
        private void Construct(EditorBlocksController editorBlocksController, IEnvironmentService environmentService, BlockManager blockManager) 
        {
            _editorBlocksController = editorBlocksController;
            _environmentService = environmentService;
            _blockManager = blockManager;
        }

        public void Awake()
        {
            Close();
            _panelWarning.Initialize(this);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            CloseWarning();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Redraw();
        }

        public void SelectData(BlockData[] selectedData)
        {
            _selectedData = selectedData;
        }
        public void OnSaveAndLoad() 
        {
            _editorBlocksController.Save();
            _editorBlocksController.SetEditBlock(_selectedData);
            Close();
        }
        public void OnSelectBlockToLoad(bool forcedly = false)
        {
            if (_selectedData == null) 
            {
                return;
            }

            if (_editorBlocksController.IsSaved || forcedly)
            {
                _editorBlocksController.SetEditBlock(_selectedData);
                Close();
                return;
            }

            OpenWarning();
        }
        public void CloseWarning()
        {
            _panelWarning.gameObject.SetActive(false);
        }
        public void OpenWarning() 
        {
            _panelWarning.gameObject.SetActive(true);
        }

        private void Redraw() 
        {
            foreach (GameObject element in _elements)
                Destroy(element);
            _elements.Clear();

            //Получаем список всех блоков
            List<string[]> mods = _blockManager.GetAllPath();
            for (int modNum = 0; modNum < mods.Count; modNum++) 
            {
                EditorBlockLoaderModInfo modInfo = Instantiate(_prefabModInfo, _content);
                _elements.Add(modInfo.gameObject);
                for (int blockNum = 0; blockNum < mods[modNum].Length; blockNum++) 
                {
                    BlockData[] blockDataArray = _blockManager.Load(mods[modNum][blockNum]);
                    if (blockNum == 0) 
                    {
                        modInfo.Initialize(blockDataArray[blockNum].mod);
                    }

                    EditorBlockLoaderBlockInfo blockInfo = Instantiate(_prefabBlockInfo, _content);
                    _elements.Add(blockInfo.gameObject);
                    blockInfo.Initialize(this, _environmentService, blockDataArray, _blockManager.GetBlockID(blockDataArray[0], false));
                }
            }
        }

        void IEditorBlockPanelMiddle.Redraw()
        {
            Redraw();
        }
    }
}
