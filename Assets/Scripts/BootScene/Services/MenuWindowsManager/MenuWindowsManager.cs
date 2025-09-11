using Game.Testing;
using Game.UI.Menu;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Zenject;
using Game.Tools.VR;

namespace Game.Services.Managers
{
    public class MenuWindowsManager : MonoBehaviour, IMenuWindowsManager
    {
        public const float DIST_WINDOW_OFFSET = -2000;
        public const float SPEED_MOVE = 10.0f;

        [Header("Windows Parent Object")]
        [Required]
        [SerializeField] Transform _windowsParentTransform;
        [SerializeField] MenuWindowBase _windowSelected;

        [Header("Canvas")]
        [Required][SerializeField] CanvasControllerVR _canvasControllerVR;

        [Header("Windows Prefabs")]
        [Required][SerializeField] MenuWindowMain _windowMainMenu;
        [Required][SerializeField] MenuWindowEditors _windowMainEditors;
        [Required][SerializeField] MenuWindowQuit _windowQuit;

        [Header("Service")]
        [Required][SerializeField] TranslateService _translateService;
        [Required][SerializeField] ScenesManager _scenesManager;

        public bool IsAsync => true;

        private float _testCoefficientReady = 0;
        private string _testingSystemMessage = string.Empty;

        private readonly List<MenuWindowBase> _menuWindowBases = new List<MenuWindowBase>();

        private TestResult _testResult = new TestResult(nameof(MenuWindowsManager));
        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        public CanvasControllerVR CanvasControllerVR => _canvasControllerVR;

        public TestResult TestIt()
        {
            _testingSystemMessage = nameof(MenuWindowsManager);
            return _testResult;
        }

        public void CloseAll()
        {
            int closeNeed = _menuWindowBases.Count;
            for (int num = 0; num < closeNeed; num++) 
            {
                _menuWindowBases[num].Destroy();
            }
            _menuWindowBases.Clear();
        }
        public void Close(MenuWindowBase window)
        {
            for (int num = 0; num < _menuWindowBases.Count; num++)
            {
                if (_menuWindowBases[num] != window)
                    continue;
                
                _menuWindowBases.RemoveAt(num);
                break;            
            }
        }

        public bool IsOpen(MenuWindowBase window)
        {
            foreach (MenuWindowBase windowTest in _menuWindowBases) 
            {
                if(windowTest == window)
                    return true;
            }
            return false;
        }

        public void Update()
        {
            UpdateWindowsMove();
        }

        private void UpdateWindowsMove() 
        {
            for (int num = 0; num < _menuWindowBases.Count; num++) 
            {
                float offsetNeed = (_menuWindowBases.Count - 1 - num) * DIST_WINDOW_OFFSET;
                RectTransform rect = _menuWindowBases[num].GetComponent<RectTransform>();

                Vector2 currentPosition = rect.anchoredPosition;
                currentPosition.x += (offsetNeed - currentPosition.x) * Time.unscaledDeltaTime * SPEED_MOVE;
                rect.anchoredPosition = currentPosition;
            }
        }

        public bool OpenWindow<T>() where T : MenuWindowBase
        {
            //»щем это окно в списке
            T window = _menuWindowBases.OfType<T>().FirstOrDefault();

            bool isCreatedNow = false;

            if (window == null) 
            {
                window = CreateWindow<T>();
                isCreatedNow = true;
            }

            if (!isCreatedNow)
            {
                //необходимо закрыть все окна открытые до текущего
                for (int num = _menuWindowBases.Count - 1; num >= 0; num--)
                {
                    if (!(_menuWindowBases[num] is T))
                    {
                        _menuWindowBases.RemoveAt(num);
                        continue;
                    }
                    break;
                }
            }

            if (window == null)
                return false;
            else return true;

        }
        private T CreateWindow<T>() where T : MenuWindowBase
        {
            T windowNew = null;
            switch (typeof(T))
            {
                case Type _ when typeof(T) == typeof(MenuWindowMain):
                    windowNew = Instantiate(_windowMainMenu, _windowsParentTransform) as T;
                    break;
                case Type _ when typeof(T) == typeof(MenuWindowEditors):
                    windowNew = Instantiate(_windowMainEditors, _windowsParentTransform) as T;
                    break;
                case Type _ when typeof(T) == typeof(MenuWindowQuit):
                    windowNew = Instantiate(_windowQuit, _windowsParentTransform) as T;
                    break;
                default:
                    throw new System.InvalidOperationException($"Cannot create window of type {typeof(T)}");
            }

            windowNew.Initialize(this, _translateService, _scenesManager);

            RectTransform rect = windowNew.gameObject.GetComponent<RectTransform>();
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition.x = -DIST_WINDOW_OFFSET;
            rect.anchoredPosition = anchoredPosition;

            _menuWindowBases.Add(windowNew);

            return windowNew;
        }
    }
}
