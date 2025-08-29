using Game.Services.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Menu
{
    public class MenuWindowEditors: MenuWindowBase
    {
        [Header("Buttons")]
        [Required][SerializeField] private UIButton _buttonBlocks;

        [Required][SerializeField] private UIButton _buttonBack;

        public override void Initialize(IMenuWindowsManager menuWindowsManager, ITranslateService translateService, IScenesManager scenesManager)
        {
            base.Initialize(menuWindowsManager, translateService, scenesManager);
            Subscribe();
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }
        private void Subscribe()
        {
            _buttonBlocks.Initialize(_translateService);
            _buttonBack.Initialize(_translateService);

            _buttonBlocks.Button.onClick.AddListener(OnClickBlocks);
            _buttonBack.Button.onClick.AddListener(OnClickBack);
        }
        private void Unsubscribe()
        {
            _buttonBlocks.Button.onClick.RemoveAllListeners();
            _buttonBack.Button.onClick.RemoveAllListeners();
        }

        private void OnClickBlocks()
        {
            _scenesManager.CloseAllScenes();
            _scenesManager.OpenScene(TypeScene.EditorBlock);
            _menuWindowsManager.CloseAll();
        }
        private void OnClickBack()
        {
            _menuWindowsManager.Close(this);
        }
    }
}
