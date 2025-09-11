using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Game.Testing;
using Zenject;

namespace Game.Services
{
    public class EventSystemController : MonoBehaviour, IEventSystemController
    {
        private const string ERROR_TEXT_ENVIRONMENT_FIRST = "Environment service must be initialize first";

        [Header("PC")]
        [Required][SerializeField] private InputSystemUIInputModule _inputSystemUIInputModule;
        [Header("VR")]
        [Required][SerializeField] private XRUIInputModule _XRUIInputModule;

        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage = nameof(EventSystemController);
        private IEnvironmentService _environmentService;

        TestResult _testResult = new TestResult(nameof(EventSystemController));

        public bool IsAsync => false;

        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => _testingSystemMessage;

        [Inject]
        void Construct(IEnvironmentService environmentService) 
        {
            _environmentService = environmentService;
        }

        public TestResult TestIt()
        {
            if (_environmentService.runtimeMode == RuntimeMode.None)
            {
                _testResult.AddProblem(ERROR_TEXT_ENVIRONMENT_FIRST, TypeProblem.Error);
                throw new System.NotImplementedException();
            }

            AllDisabled();
            switch (_environmentService.runtimeMode) 
            {
                case RuntimeMode.PC:
                    _inputSystemUIInputModule.enabled = true;
                    break;
                case RuntimeMode.VR:
                    _XRUIInputModule.enabled = true;
                    break;
            }
            _testCoefficientReady = 1.0f;
            return _testResult;
        }

        private void AllDisabled() 
        {
            _inputSystemUIInputModule.enabled = false;
            _XRUIInputModule.enabled = false;   
        }
    }
}
