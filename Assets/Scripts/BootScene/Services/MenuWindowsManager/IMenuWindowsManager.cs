using Game.Testing;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Menu;

namespace Game.Services.Managers
{
    public interface IMenuWindowsManager : ITestingSystem
    {
        public void CloseAll();
        public void Close(MenuWindowBase window);
        public bool IsOpen(MenuWindowBase window);
        public bool OpenWindow<T>() where T : MenuWindowBase;
    }
}
