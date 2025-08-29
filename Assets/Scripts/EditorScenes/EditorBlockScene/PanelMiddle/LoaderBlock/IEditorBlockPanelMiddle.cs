using UnityEngine;

namespace Game.Scene.Editor.Block
{
    public interface IEditorBlockPanelMiddle
    {
        public void Open();
        public void Close();
        public void Redraw();
    }
}
