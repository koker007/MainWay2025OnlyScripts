using Game.Services;
using Game.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Unity.VisualScripting.Icons;

namespace Game.Services
{
    public class TranslateService : MonoBehaviour, ITranslateService
    {
        private const string ERROR_INITIALIZE = "Error initialize";
        private const string ERROR_LANGUAGE_ENGLISH_NOT_FOUND = "Error - English language is not found";
        private const string ERROR_LANGUAGE_DIRECTORY_NOT_FOUND = "Error - language directory not found";
        private const string ERROR_LANGUAGE_NOT_FOUND = "Not found";
        private const string ERROR_LANGUAGE_VALUE_FOR_KEY = "value for key";
        private const string DIRECTIVE = "AppData\\Language";
        private const string ENGLISH_STR = "English";

        private TestResult testingProblems = new TestResult(nameof(TranslateService));
        private bool isInitialize = false;

        short _languageSelectedNum = -1;
        short _languageEnglishNum = -1;

        private readonly List<Language> _languages = new List<Language>();

        public float _testCoefficientReady;
        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => nameof(TranslateService);

        public bool IsAsync => true;

        private void Initialize()
        {
            if (isInitialize)
                return;

            string fullPath = $"{Application.dataPath}\\{DIRECTIVE}";

            //Перебираем все папки из директории DIRECTIVE
            if (!Directory.Exists(fullPath))
            {
                testingProblems.AddProblem($"{ERROR_LANGUAGE_DIRECTORY_NOT_FOUND} {fullPath}", TypeProblem.Error);
                return;
            }

            string[] folders = Directory.GetDirectories(fullPath);
            //Загружаем все языки
            for(int num = 0; num < folders.Length; num++)
            {
                _testCoefficientReady = (float)num / folders.Length;

                Language language = new Language(folders[num], testingProblems);
                _languages.Add(language);
            }
            //Ищем английский
            for (short num = 0; num < _languages.Count; num++)
            {
                if (_languages[num].NameFolder != ENGLISH_STR) 
                {
                    continue;
                }
                _languageEnglishNum = num;
            }
            if (_languageEnglishNum == -1) 
            {
                testingProblems.AddProblem($"{ERROR_LANGUAGE_ENGLISH_NOT_FOUND}", TypeProblem.Error);
            }

            isInitialize = true;
        }

        TestResult ITestingSystem.TestIt()
        {
            testingProblems = new TestResult(nameof(TranslateService));

            try
            {

                Initialize();

                if (!isInitialize)
                    testingProblems.AddProblem($"{ERROR_INITIALIZE}", TypeProblem.Error);

            }
            catch
            {
                testingProblems.AddProblem($"{ERROR_INITIALIZE}", TypeProblem.Error);
                return testingProblems;
            }

            return testingProblems;
        }

        public string GetTextFromKey(string key, string defaultText)
        {
            string text = string.Empty;

            //Пытаемся получить значение по выбранному языку
            if (_languageSelectedNum != -1)
            {
                text = _languages[_languageSelectedNum].GetTextFromKey(key);
                if (text == string.Empty)
                {
                    testingProblems.AddProblem($"{ERROR_LANGUAGE_NOT_FOUND} {_languages[_languageSelectedNum].NameTranslated} {ERROR_LANGUAGE_VALUE_FOR_KEY}: {key}", TypeProblem.Error);
                }
            }
            if (text == string.Empty && _languageEnglishNum != -1)
            {
                text = _languages[_languageEnglishNum].GetTextFromKey(key);
                if (text == string.Empty)
                {
                    testingProblems.AddProblem($"{ERROR_LANGUAGE_NOT_FOUND} {_languages[_languageEnglishNum].NameTranslated} {ERROR_LANGUAGE_VALUE_FOR_KEY}: {key}", TypeProblem.Error);
                }
            }

            if (text == string.Empty && defaultText != string.Empty)
            {
                text = defaultText;
            }

            return text;
        }
        public string GetTextFromKey(string key)
        {
            return GetTextFromKey(key, string.Empty);
        }

        public bool SetTextFromKey(string key, string textNew) 
        {
            bool allOk = true;
            if (_languageSelectedNum == -1)
            {
                allOk = false;
                return allOk;
            }

            _languages[_languageEnglishNum].SetTextFromKey(key, textNew);
            return allOk;
        }

    }

    [Serializable]
    class KeyAndText
    {
        public string key;
        public string text;

        public KeyAndText(string key, string text)
        {
            this.key = key;
            this.text = text;
        }

        public KeyAndText LineToKaT(string textLine, string separator)
        {
            string[] separate = { separator };
            string[] textSplite = textLine.Split(separate, System.StringSplitOptions.None);

            //Если текст разделен не на 2 части то это ошибка
            if (textSplite.Length != 2)
            {
                return null;
            }

            string key = textSplite[0];
            string text = textSplite[1];

            //Текст разделен по ключу убираем пробелы в ключе
            string keyNew = "";
            foreach (char symbol in key)
            {
                //Добавляем символ
                keyNew += symbol;

                //Если символ не пробел
                if (symbol != ' ')
                {
                    //Перезаписываем старый текст - новым
                    key = keyNew;
                }
            }

            //Теперь ключ без пробелов
            //Создаем связку ключ - значение
            KeyAndText keyAndTextNew = new KeyAndText(key, text);

            return keyAndTextNew;
        }
        public KeyAndText[] GetKATs(string[] dataLines, string separator)
        {
            List<KeyAndText> keyAndTextsList = new List<KeyAndText>();

            foreach (string data in dataLines)
            {
                KeyAndText keyAndTextNew = LineToKaT(data, separator);
                if (keyAndTextNew == null)
                    continue;

                keyAndTextsList.Add(keyAndTextNew);
            }

            return keyAndTextsList.ToArray();
        }
    }

    class Language 
    {
        private const string KEY_LANGUAGE_NAMING = "keyLanguageNaming";
        private const string FILE_NAME_MAIN = "main.txt";
        private const int MAXIMUM_KEY_ONE_SUMBOL = 250;
        private const string SEPARATOR = "|=|";

        readonly KeyAndText[] _keyAndText = new KeyAndText[char.MaxValue * 2 * MAXIMUM_KEY_ONE_SUMBOL];
        string _nameTranslated = string.Empty;
        string _nameFolder = string.Empty;

        public string NameTranslated => _nameTranslated;
        public string NameFolder => _nameFolder;

        public Language(string pathFolder, TestResult testingProblems)
        {
            _nameFolder = Path.GetFileName(pathFolder);
            LoadLanguage(pathFolder, testingProblems);

            _nameTranslated = GetTextFromKey(KEY_LANGUAGE_NAMING);
            if (_nameTranslated == string.Empty) 
            {
                testingProblems.AddProblem($"Not have value for key ''{KEY_LANGUAGE_NAMING}'' for language {_nameFolder}", TypeProblem.Warning);
            }
        }

        static private int GetStartNum(string key)
        {
            if (key == null ||
                key.Length == 0)
                return -1;


            int numStart = 0;

            //находим стартовый номер в массиве по первому и последнему символу ключа
            numStart = key[0] + key[key.Length - 1];

            return numStart;
        }

        bool LoadLanguage(string folder, TestResult testingProblems)
        {
            bool allOk = true;

            //создаем путь к файлу
            string path = folder + $"/{FILE_NAME_MAIN}";

            if (File.Exists(path))
            {
                string encodeText = "Test Тест テスト 測試 تست mitä";
                //получаем текст из файла
                string[] fileText = File.ReadAllLines(path, System.Text.Encoding.GetEncoding(1201));

                //Получили строки файла, теперь разделяем по ключу и заполняем
                foreach (string textFull in fileText)
                {
                    SetText(textFull);
                }
            }
            else
            {
                string textError = "File " + path + " Not Found";
                testingProblems.AddProblem($"{textError}", TypeProblem.Error);
                return false;
            }

            return allOk;


            void SetText(string textFull)
            {
                string[] separate = { SEPARATOR };
                string[] textSplite = textFull.Split(separate, System.StringSplitOptions.None);

                //Если текст разделен не на 2 части то это ошибка
                if (textSplite.Length != 2)
                {
                    return;
                }

                string key = textSplite[0];
                string text = textSplite[1];

                //Текст разделен по ключу убираем пробелы в ключе
                string keyNew = "";
                foreach (char symbol in key)
                {
                    //Добавляем символ
                    keyNew += symbol;

                    //Если символ не пробел
                    if (symbol != ' ')
                    {
                        //Перезаписываем старый текст - новым
                        key = keyNew;
                    }
                }

                //Теперь ключ без пробелов
                //Создаем связку ключ - значение
                KeyAndText keyAndTextNew = new KeyAndText(key, text);

                //находим стартовый номер в массиве по первому и последнему символу ключа
                int numStart = GetStartNum(keyAndTextNew.key);


                for (int num = numStart; num < _keyAndText.Length; num++)
                {
                    //если ячейка не свободна переключается дальше
                    if (_keyAndText[num] != null)
                    {
                        continue;
                    }

                    //Запоминаем
                    _keyAndText[num] = keyAndTextNew;
                    //Завершаем цикл
                    break;
                }
            }
        }

        public string GetTextFromKey(string key)
        {
            string text = string.Empty;

            //получаем номер поиска
            int numStart = GetStartNum(key);

            if (numStart < 0)
                return text;

            //Проверяем в выбранном языке
            int numTry = 0;
            for (int num = numStart; _keyAndText != null && num < _keyAndText.Length && numTry <= MAXIMUM_KEY_ONE_SUMBOL; num++)
            {
                numTry++;

                //Если эта ячейка пустая или в ней не совпадают ключи ищем дальше
                if (_keyAndText[num] == null || _keyAndText[num].key != key)
                {
                    continue;
                }

                //Ключи совпали вытаскиваем текст
                text = _keyAndText[num].text;

                //завершаем цикл
                break;
            }

            //Если текст обнаружен передаем
            if (text.Length > 0)
            {
                return text;
            }

            return text;
        }

        public void SetTextFromKey(string key, string textNew)
        {
            //получаем номер поиска
            int numStart = GetStartNum(key);

            if (numStart < 0)
                return;

            //Проверяем в выбранном языке
            int numTry = 0;
            for (int num = numStart; num < _keyAndText.Length && numTry <= MAXIMUM_KEY_ONE_SUMBOL; num++)
            {
                numTry++;

                //Если эта ячейка не пустая и в ней не совпадают ключи ищем дальше
                if (_keyAndText[num] != null && _keyAndText[num].key != key)
                {
                    continue;
                }

                //Теперь ключ без пробелов
                //Создаем связку ключ - значение
                KeyAndText keyAndTextNew = new KeyAndText(key, textNew);
                //Запихиваем
                _keyAndText[num] = keyAndTextNew;

                //завершаем цикл
                break;
            }
        }

    }
}