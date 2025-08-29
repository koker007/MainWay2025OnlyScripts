using Game.Services.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject.Asteroids;

namespace Game.UI.Menu {
    public class MenuWindowMain: MenuWindowBase
    {
        [Header("Logo")]
        [Required][SerializeField] private RectTransform _LogoMain;
        [Required][SerializeField] private RectTransform _LogoWay;

        [Header("Buttons")]
        [Required][SerializeField] private UIButton _buttonPlay;
        [Required][SerializeField] private UIButton _buttonSettings;
        [Required][SerializeField] private UIButton _buttonEditor;
        [Required][SerializeField] private UIButton _buttonQuit;

        protected void Update()
        {
            base.Update();
            UpdateLogo();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public override void Initialize(IMenuWindowsManager menuWindowsManager, ITranslateService translateService, IScenesManager scenesManager) 
        {
            base.Initialize(menuWindowsManager, translateService, scenesManager);
            Subscribe();
        }

        public void UpdateLogo() 
        {
            RectTransform rect = gameObject.GetComponent<RectTransform>();

            Vector2 anchoredPosition = _LogoWay.anchoredPosition;
            anchoredPosition.x = -rect.anchoredPosition.x * 2;
            _LogoWay.anchoredPosition = anchoredPosition;
        }

        void Subscribe() 
        {
            _buttonPlay.Initialize(_translateService);
            _buttonSettings.Initialize(_translateService);
            _buttonEditor.Initialize(_translateService);
            _buttonQuit.Initialize(_translateService);

            _buttonPlay.Button.onClick.AddListener(OnClickPlay);
            _buttonSettings.Button.onClick.AddListener(OnClickSettings);
            _buttonEditor.Button.onClick.AddListener(OnClickEditor);
            _buttonQuit.Button.onClick.AddListener(OnClickQuit);
        }
        void Unsubscribe() 
        {
            _buttonPlay.Button.onClick.RemoveAllListeners();
            _buttonSettings.Button.onClick.RemoveAllListeners();
            _buttonEditor.Button.onClick.RemoveAllListeners();
            _buttonQuit.Button.onClick.RemoveAllListeners();
        }

        private void OnClickPlay()
        {
            
        }
        private void OnClickSettings() {
        
        }
        private void OnClickEditor() 
        {
            _menuWindowsManager.OpenWindow<MenuWindowEditors>();
        }
        private void OnClickQuit() 
        {
            _menuWindowsManager.OpenWindow<MenuWindowQuit>();
        }
    }
}
