using Game.Services.Managers;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.UI.Menu
{
    public abstract class MenuWindowBase : MonoBehaviour
    {
        [Header("Tittle")]
        [SerializeField] protected string _tittleKey;
        [SerializeField] protected UITextBackground _tittleText;

        protected IMenuWindowsManager _menuWindowsManager;
        protected ITranslateService _translateService;
        protected IScenesManager _scenesManager;

        public virtual void Initialize(IMenuWindowsManager menuWindowsManager, ITranslateService translateService, IScenesManager scenesManager) 
        {
            _menuWindowsManager = menuWindowsManager;
            _translateService = translateService;
            _scenesManager = scenesManager;

            TranslateTittle();
        }

        public void Update()
        {
            if (!_menuWindowsManager.IsOpen(this))
            {
                DestroyMove();
            }
        }

        private void TranslateTittle() 
        {
            if (_tittleText == null)
                return;

            _tittleText.SetText(_translateService.GetTextFromKey(_tittleKey, _tittleText.Text));
        }

        public void DestroyMove() 
        {
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition.x += (-MenuWindowsManager.DIST_WINDOW_OFFSET - anchoredPosition.x) * Time.unscaledDeltaTime * MenuWindowsManager.SPEED_MOVE;
            rect.anchoredPosition = anchoredPosition;

            if (Mathf.Abs(-MenuWindowsManager.DIST_WINDOW_OFFSET - anchoredPosition.x) < 10.0f) 
            {
                Destroy(this.gameObject);
            }
        }

        public void Open()
        {
        
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
