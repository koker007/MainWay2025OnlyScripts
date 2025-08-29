using Game.Testing;
using OpenCover.Framework.Model;
using UnityEngine;
using Zenject;

namespace Game.Services
{
    public class DebugAudioService : MonoBehaviour, IAudioService
    {
        private const string ErrorService = "Error";

        [SerializeField] private AudioSource _audioSource;

        private float _testCoefficientReady = 0.0f;
        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => nameof(DebugAudioService);

        public bool IsAsync => true;

        TestResult ITestingSystem.TestIt()
        {
            TestResult result = new TestResult(nameof(DebugAudioService));

            try 
            {
                if (_audioSource == null)
                    result.AddProblem($"{ErrorService}", TypeProblem.Error);

            }
            catch 
            {
                result.AddProblem($"{ErrorService}", TypeProblem.Error);
                return result;
            }

            int countTestMax = 214748364;
            for (int num = 0; num < countTestMax; num++) 
            {
                _testCoefficientReady = (float)num / countTestMax;
            }

            return result;
        }
    }
}