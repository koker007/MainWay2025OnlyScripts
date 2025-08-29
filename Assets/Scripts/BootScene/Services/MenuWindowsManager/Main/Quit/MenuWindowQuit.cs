using Game.Services.Managers;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.UI.Menu
{
    public class MenuWindowQuit: MenuWindowBase
    {
        [Header("Buttons")]
        [Required][SerializeField] private UIButton _buttonYes;
        [Required][SerializeField] private UIButton _buttonNo;

        public override void Initialize(IMenuWindowsManager menuWindowsManager, ITranslateService translateService, IScenesManager scenesManager)
        {
            base.Initialize(menuWindowsManager, translateService, scenesManager);
            Subscribe();
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }
        void Subscribe()
        {
            _buttonYes.Initialize(_translateService);
            _buttonNo.Initialize(_translateService);

            _buttonYes.Button.onClick.AddListener(OnClickYes);
            _buttonNo.Button.onClick.AddListener(OnClickNo);
        }
        void Unsubscribe()
        {
            _buttonYes.Button.onClick.RemoveAllListeners();
            _buttonNo.Button.onClick.RemoveAllListeners();
        }

        private void OnClickYes()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        private void OnClickNo()
        {
            _menuWindowsManager.Close(this);
        }
    }
}
