using Game.Data.Block;
using Game.Data.Managers;
using Game.Scene.Loading;
using Game.Services.GPU;
using Game.Services.Managers;
using Game.Testing;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using Zenject;

namespace Game.Services
{
    public class TestingService : MonoBehaviour
    {
        private readonly List<ITestingSystem> _testingSystems = new List<ITestingSystem>();
        public List<ITestingSystem> TestingSystems => _testingSystems;

        public void Awake()
        {
            InitializeXR();
        }

        public IEnumerator InitializeXR() 
        {
            // ∆дЄм инициализацию XR Manager
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.Log("XR not started, running in PC mode");
            }
            else
            {
                Debug.Log("XR started: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
            }
        }

        public void AddToTestingList(ITestingSystem testingSystem) 
        {
            _testingSystems.Add(testingSystem);
        }
    }
}
