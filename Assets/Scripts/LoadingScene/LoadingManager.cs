using Game.Services;
using Game.Services.Managers;
using Game.Testing;
using Game.UI;
using Game.UI.Menu;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.Scene.Loading
{
    public class LoadingManager : MonoBehaviour, ILoadingManager
    {
        private const string MainSceneName = "MainScene";
        private const string TEXT_LOADING = "Loading";
        private const string TEXT_WAITING_ZENJECT_INSTANCE = "Waiting Zenject instance";
        private const string TEXT_TESTING_FAIL = "FAIL";
        private const string TEXT_TESTING_DONE = "DONE";

        private static LoadingManager _instance;

        [Required][SerializeField] private Image progressBarImage;

        [Required][SerializeField] private GameObject _loadingMenu;
        [Required][SerializeField] private UIText _loadingMainText;
        [Required][SerializeField] private ScrollRect _logErrorScrollView;
        [Required][SerializeField] private GameObject _contentScrollView;

        [Header("Prefabs")]
        [Required][SerializeField] private UITextBackground _prefabTextLoadintLog;

        private bool operationScene = false;
        DiContainer _container;
        [Required][SerializeField] private TestingService _testingService;
        [Required][SerializeField] private MenuWindowsManager _menuWindowsManager;
        [Required][SerializeField] private ScenesManager _scenesManager;
        private ITestingSystem _testingSystemNow;
        private int testingSystemsCountDone = 0;

        private float _progressTesting = 0;

        [Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }

        void Awake()
        {
            _logErrorScrollView.gameObject.SetActive(false);

            // Реализация паттерна Singleton
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                LoadSceneAsync(MainSceneName);
            }
            else
            {
                // Уничтожаем дубликаты
                Destroy(gameObject);
            }
        }

        void Update() 
        {
            UpdateProgressBar();
        }

        public async Task LoadSceneAsync(string targetSceneName)
        {
            operationScene = false;
            _progressTesting = 0;

            SetLoadingText($"{TEXT_WAITING_ZENJECT_INSTANCE}");
            await Task.Delay(1000);
            if (!ProjectContext.HasInstance)
            {
                await Task.Yield();
            }
            _loadingMenu.SetActive(true);

            testingSystemsCountDone = -1;
            foreach (ITestingSystem testingSystem in _testingService.TestingSystems) 
            {
                testingSystemsCountDone++;
                _testingSystemNow = testingSystem;
                //SetLoadingText($"{_testingSystemNow.TestingSystemMessage}");

                TestResult testResult = null;
                //Если асинхронное то запускаем и не паримся
                if (_testingSystemNow.IsAsync)
                    testResult = await Task.Run(() => testingSystem.TestIt());
                //Если не асинхронное то выполняем итеративно пока не выполнятся
                else 
                {
                    while (_testingSystemNow.TestCoefficientReady != 1) {
                        testResult = testingSystem.TestIt();
                        if(!testResult.AllOk)
                            break;
                        await Task.Yield();
                    }
                }

                if (!testResult.AllOk)
                {
                    AddTextResultLog(testResult);
                    await Task.Delay(1000);
                }
                await Task.Yield();
            }
            //Все модули были проверены теперь грузим сцену
            _testingSystemNow = null;

            // Закрытие Boot
            SetLoadingText($"Clearing boot");
            await _scenesManager.CloseScene(TypeScene.Boot);
            await Task.Delay(500);

            // Асинхронная загрузка целевой сцены через игровой менеджер сцен
            SetLoadingText($"{targetSceneName}");
            await _scenesManager.OpenScene(TypeScene.Main);

            await Task.Delay(1000);

            _loadingMenu.SetActive(false);

            _menuWindowsManager.CloseAll();
            _menuWindowsManager.OpenWindow<MenuWindowMain>();
        }

        private void UpdateProgressBar()
        {
            if(_testingSystemNow != null)
                SetLoadingText($"{_testingSystemNow.TestingSystemMessage}");

            float progress = 0;
            float shareOfOneSystem = 1.0f / _testingService.TestingSystems.Count;
            if (_testingSystemNow != null)
                _progressTesting = testingSystemsCountDone * shareOfOneSystem + shareOfOneSystem * _testingSystemNow.TestCoefficientReady;
            else if (operationScene)
            {
                progress = 1;//Mathf.Clamp01(operationScene.progress / 0.9f); // Прогресс до 90%, остальное — финализация
            }

            progress += _progressTesting;
            progress /= 2;

            progressBarImage.fillAmount = progress;
        }
        private void SetLoadingText(string status)
        {
            _loadingMainText.SetText($"{TEXT_LOADING}: {status}");
        }

        private void AddTextResultLog(TestResult testResult) 
        {
            _logErrorScrollView.gameObject.SetActive(true);

            for (int num = 0; num < testResult.ProblemsList.Count; num++) 
            {
                UITextBackground text = Instantiate(_prefabTextLoadintLog, _contentScrollView.gameObject.transform);
                text.SetText($"{testResult.TestedModuleName}: {testResult.ProblemsList[num]}");

                if (testResult.TypeProblems[num] == TypeProblem.Error)
                    text.SetColor(Color.red);
                else if (testResult.TypeProblems[num] == TypeProblem.Warning)
                    text.SetColor(Color.orange);
            }
        }
    }
}