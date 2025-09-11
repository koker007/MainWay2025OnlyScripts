using Game.Testing;
using UnityEngine;
using UnityEngine.XR.Management;
using Zenject;

namespace Game.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string ERROR_RUNTIME_MODE = "Critical error, wrong runtime mode";

        private bool _isInitialized = false;
        private RuntimeMode _runtimeMode;

        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage = nameof(EnvironmentService);

        private TestResult _testResult = new TestResult(nameof(EnvironmentService));

        public bool IsAsync => false;
        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        public RuntimeMode runtimeMode => _runtimeMode;
        public bool isInitialized => _isInitialized;

        private IOpenXRService _openXRService;

        public TestResult TestIt()
        {
            ChooseMode();

            if (_runtimeMode == RuntimeMode.None)
            {
                _testResult.AddProblem($"{ERROR_RUNTIME_MODE}: {_runtimeMode}", TypeProblem.Error);
                _testCoefficientReady = 0.0f;
            }
            else 
            {
                _testCoefficientReady = 1.0f;
                _isInitialized = true;
            }

                return _testResult;
        }

        private void ChooseMode() 
        {
            _openXRService ??= ProjectContext.Instance.Container.Resolve<IOpenXRService>();

            if (_openXRService.IsActive)
            {
                _runtimeMode = RuntimeMode.VR;
            }
            else 
            {
                _runtimeMode = RuntimeMode.PC;
            }
        }
    }
}
