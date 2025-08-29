using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelLoader: MonoBehaviour, IEditorBlockPanelMiddle
    {
        [Header("Prefabs")]
        [Required][SerializeField] private EditorBlockLoaderModInfo _prefabModInfo;
        [Required][SerializeField] private EditorBlockLoaderBlockInfo _prefabBlockInfo;

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void Redraw() 
        {
            
        }

        void IEditorBlockPanelMiddle.Redraw()
        {
            Redraw();
        }
    }
}
