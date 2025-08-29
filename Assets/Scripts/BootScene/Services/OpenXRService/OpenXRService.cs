using Game.Testing;
using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

namespace Game.Services
{
    public class OpenXRService: MonoBehaviour, IOpenXRService
    {
        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage = nameof(OpenXRService);
        private TestResult _testResult = new TestResult(nameof(OpenXRService));
        private bool _isActive = false;

        public bool IsAsync => true;
        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        public bool IsActive => _isActive;

        IEnumerator Start() 
        {
            if (XRGeneralSettings.Instance != null) 
            {
                // ∆дЄм инициализацию XR Manager
                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            }

            if (XRGeneralSettings.Instance == null ||
                XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                _isActive = false;
            }
            else
            {
                _isActive = true;
                Debug.Log("XR started: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
            }
            _testCoefficientReady = 1.0f;
        }


        public TestResult TestIt()
        {
            return _testResult;
        }
    }
}
