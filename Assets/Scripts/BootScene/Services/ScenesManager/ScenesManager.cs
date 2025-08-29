using Game.Testing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Services.Managers
{
    public class ScenesManager : MonoBehaviour, IScenesManager
    {
        private const string SCENE_NAME_BOOT = "BootScene";
        private const string SCENE_NAME_MAIN = "MainScene";
        private const string SCENE_NAME_EDITOR_BLOCK = "EditorBlockScene";

        // Трекер активных операций загрузки
        private Dictionary<TypeScene, AsyncOperation> _activeLoadOperations = new();
        private HashSet<string> _loadedScenes = new();

        private TestResult _testResult;

        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage = string.Empty;

        public bool IsAsync => false;
        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => _testingSystemMessage;

        public async Task<bool> CloseAllScenes()
        {
            bool allOk = true;
            foreach (TypeScene scene in Enum.GetValues(typeof(TypeScene)))
            {
                if (!await CloseScene(scene))
                    allOk = false;
            }
            return allOk;
        }

        public async Task<bool> CloseScene(TypeScene typeScene)
        {
            string sceneName = GetSceneName(typeScene);
            if (!_loadedScenes.Contains(sceneName))
                return true;

            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            await TrackOperation(unloadOp, typeScene, isLoad: false);
            return unloadOp.isDone;
        }

        public async Task<bool> OpenScene(TypeScene typeScene, bool setActive = false)
        {
            string sceneName = GetSceneName(typeScene);

            if (_loadedScenes.Contains(sceneName))
            {
                if (setActive) SetActiveScene(sceneName);
                return true;
            }

            if (_activeLoadOperations.TryGetValue(typeScene, out var existingOp))
            {
                await existingOp;
                return true;
            }

            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = true;
            _activeLoadOperations[typeScene] = loadOp;

            await TrackOperation(loadOp, typeScene, isLoad: true);

            if (setActive) SetActiveScene(sceneName);
            return loadOp.isDone;
        }

        private async Task TrackOperation(AsyncOperation op, TypeScene scene, bool isLoad)
        {
            while (!op.isDone)
                await Task.Yield();

            string sceneName = GetSceneName(scene);
            if (isLoad)
                _loadedScenes.Add(sceneName);
            else
                _loadedScenes.Remove(sceneName);

            _activeLoadOperations.Remove(scene);
        }

        private void SetActiveScene(string sceneName)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid())
                SceneManager.SetActiveScene(scene);
        }

        private string GetSceneName(TypeScene typeScene)
        {
            switch (typeScene)
            {
                case TypeScene.Boot:
                    return SCENE_NAME_BOOT;
                case TypeScene.Main:
                    return SCENE_NAME_MAIN;
                case TypeScene.EditorBlock:
                    return SCENE_NAME_EDITOR_BLOCK;
            }

            throw new ArgumentOutOfRangeException();
        }

        public TestResult TestIt()
        {
            _testingSystemMessage = nameof(ScenesManager);
            _testResult = new TestResult(nameof(ScenesManager));

            TryTest();

            return _testResult;
        }

        private async void TryTest() 
        {
            _testingSystemMessage = nameof(ScenesManager);
            _testResult = new TestResult(nameof(ScenesManager));

            int countNow = 0;
            int countMax = Enum.GetValues(typeof(TypeScene)).Length;

            foreach (TypeScene typeScene in Enum.GetValues(typeof(TypeScene)))
            {

                switch (typeScene)
                {
                    case TypeScene.Main:
                        break;
                    case TypeScene.EditorBlock:
                        _testingSystemMessage += $" {typeScene}";
                        await OpenScene(TypeScene.EditorBlock);
                        await CloseScene(TypeScene.EditorBlock);
                        break;
                }
                countNow++;
                _testCoefficientReady = (float)countNow / countMax;
            }
        }
    }
}
