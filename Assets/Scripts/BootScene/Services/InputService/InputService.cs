using Game.Scene.Editor.Block;
using Game.Testing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Services
{
    public class InputService : MonoBehaviour, IInputService
    {
        private const string ERROR_CONTROLS_DATA_IS_NULL = "Critical error, input data is null";

        private const string KEY_BINDS_EDITOR_BLOCK = "KeyBindsEditorBlock";

        private EditorBlockSceneControls _editorBlockControls;

        private float _testCoefficientReady = 0f;
        private string _testingSystemMessage = nameof(InputService);
        private bool _isCriticalProblem = false;

        private TestResult _testResult = new TestResult(nameof(InputService));

        public EditorBlockSceneControls editorBlockControls => _editorBlockControls;
        public bool IsAsync => false;
        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        public void LoadAll()
        {
            //Загружает строку со всеми назначенными клавишами
            if (PlayerPrefs.HasKey(KEY_BINDS_EDITOR_BLOCK))
            {
                string binds = PlayerPrefs.GetString(KEY_BINDS_EDITOR_BLOCK);
                _editorBlockControls.LoadBindingOverridesFromJson(binds);
            }
        }

        public void SaveAll()
        {
            //Сохраняет строку со всеми назначенными клавишами в PlayerPrefs
            string binds = _editorBlockControls.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(KEY_BINDS_EDITOR_BLOCK, binds);
            PlayerPrefs.Save();
        }

        public TestResult TestIt()
        {
            Initialize();

            if (_editorBlockControls == null)
                SetCriticalErrorDataIsNull<EditorBlockSceneControls>();

            _testCoefficientReady = _isCriticalProblem ? 0.0f : 1.0f;

            return _testResult;
        }

        private void Initialize() 
        {
            _editorBlockControls ??= new EditorBlockSceneControls();
        }

        private void SetCriticalErrorDataIsNull<T>() 
        {
            _testCoefficientReady = 0;
            _testResult.AddProblem($"{ERROR_CONTROLS_DATA_IS_NULL}: {nameof(T)}", TypeProblem.Error);
            _isCriticalProblem = true;
        }
    }
}
