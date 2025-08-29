using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelMiddle: MonoBehaviour
    {
        [Required][SerializeField] private EditorBlockPanelLoader _panelLoader;

        private IEditorBlockPanelMiddle _selectedPanel;

        private EditorBlocksController _editorBlocksController;

        [Inject]
        void Construct(EditorBlocksController editorBlocksController) 
        {
            _editorBlocksController = editorBlocksController;
        }

        private void OnEnable() 
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe() 
        {
            _editorBlocksController.PanelLeft.buttonLoad.Button.onClick.AddListener(OnClickButtonLoad);
        }
        private void Unsubscribe() 
        {
            _editorBlocksController.PanelLeft.buttonLoad.Button.onClick.RemoveListener(OnClickButtonLoad);
        }

        private void OnClickButtonLoad() 
        {
            if (_selectedPanel != null)
                _selectedPanel.Close();

            _selectedPanel = _panelLoader;

            _selectedPanel.Open();
            _selectedPanel.Redraw();
        }
    }
}
