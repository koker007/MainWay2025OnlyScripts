using UnityEngine;

namespace Game.Scene.Editor.Block
{
    public interface IEditorBlockPanelMiddle
    {
        public bool IsOpen { get; }
        public void Open();
        public void Close();
        public void Redraw();
    }
}
