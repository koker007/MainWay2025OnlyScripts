using Game.Testing;
using Game.Tools.VR;
using Game.UI.Menu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Services.Managers
{
    public interface IMenuWindowsManager : ITestingSystem
    {
        public CanvasControllerVR CanvasControllerVR { get; }

        public void CloseAll();
        public void Close(MenuWindowBase window);
        public bool IsOpen(MenuWindowBase window);
        public bool OpenWindow<T>() where T : MenuWindowBase;
    }
}
