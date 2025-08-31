using UnityEngine;
using Sirenix.OdinInspector;
using Game.UI;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelLoaderWarning : MonoBehaviour
    {
        [Required][SerializeField] UIButton _buttonÑancel;
        [Required][SerializeField] UIButton _buttonLoad;
        [Required][SerializeField] UIButton _buttonSaveAndLoad;

        EditorBlockPanelLoader _editorBlockPanelLoader;
        ITranslateService _translateService;

        private void Awake()
        {
            Subscribe();
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }

        [Inject]
        void Construct(ITranslateService translateService) 
        {
            _translateService = translateService;
        }

        public void Initialize(EditorBlockPanelLoader editorBlockPanelLoader)
        {
            _editorBlockPanelLoader = editorBlockPanelLoader;
        }
        private void Subscribe() 
        {
            _buttonÑancel.Initialize(_translateService);
            _buttonLoad.Initialize(_translateService);
            _buttonSaveAndLoad.Initialize(_translateService);

            _buttonÑancel.Button.onClick.AddListener(OnClickÑancel);
            _buttonLoad.Button.onClick.AddListener(OnClickLoad);
            _buttonSaveAndLoad.Button.onClick.AddListener(OnClickSaveAndLoad);
        }
        private void Unsubscribe() 
        {
            _buttonÑancel.Button.onClick.RemoveAllListeners();
            _buttonLoad.Button.onClick.RemoveAllListeners();
            _buttonSaveAndLoad.Button.onClick.RemoveAllListeners();
        }

        private void OnClickÑancel() 
        {
            _editorBlockPanelLoader.CloseWarning();
        }
        private void OnClickLoad() 
        {
            _editorBlockPanelLoader.OnSelectBlockToLoad(true);
            _editorBlockPanelLoader.CloseWarning();
        }
        private void OnClickSaveAndLoad() 
        {
            _editorBlockPanelLoader.OnSaveAndLoad();
            _editorBlockPanelLoader.CloseWarning();
        }
    }
}
