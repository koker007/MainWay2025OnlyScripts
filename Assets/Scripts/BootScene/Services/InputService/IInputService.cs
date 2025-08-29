using Game.Scene.Editor.Block;
using Game.Testing;
using UnityEngine;

namespace Game.Services
{
    public interface IInputService : ITestingSystem
    {
        public EditorBlockSceneControls editorBlockControls { get; }

        public void SaveAll();
        public void LoadAll();
    }
}
