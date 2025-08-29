using System.ComponentModel.Design;
using System.Globalization;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Testing;

namespace Game.Services
{
    public class ResourcesService : MonoBehaviour, IResourcesService, ITestingSystem
    {
        [Header("Materials")]
        [Required][SerializeField] private Material materialBlock;
        [Required][SerializeField] private Material materialVoxel;

        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage = nameof(ResourcesService);

        private TestResult _testResult = new TestResult(nameof(ResourcesService));
        private bool _isCriticalError = false;

        public Material MaterialBlock => materialBlock;
        public Material MaterialVoxels => materialVoxel;
        public bool IsAsync => false;

        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        public TestResult TestIt()
        {
            if (materialBlock == null)
                SetCriticalError(nameof(materialBlock));
            if (materialVoxel == null)
                SetCriticalError(nameof(materialVoxel));


            if (!_isCriticalError)
                _testCoefficientReady = 1.0f;

            return _testResult;
        }

        private void SetCriticalError(string resourceName) 
        {
            _isCriticalError = true;
            _testResult.AddProblem(resourceName, TypeProblem.Error);
            
        }
    }
}
