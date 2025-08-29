using Game.Testing;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Services.Managers
{
    public interface IScenesManager : ITestingSystem
    {
        public Task<bool> CloseAllScenes();
        public Task<bool> CloseScene(TypeScene typeScene);
        public Task<bool> OpenScene(TypeScene typeScene, bool setActive = false);
    }

    public enum TypeScene
    {
        Boot,
        Main,
        EditorBlock
    }
}
