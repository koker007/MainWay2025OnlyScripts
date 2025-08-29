using Game.Data.Block;
using Game.Testing;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Data.Managers {
    //Осуществляет хранение, загрузку и выдачу по запросу блоков
    public class BlockManager : DataManager<BlockData>, ITestingSystem
    {
        private const string ERROR_TEXT_BLOCKS_IS_NOT_FOUND = "Block is not found";
        private const string ERROR_TEXT_BLOCKS_CANT_TO_LOAD_BLOCK_DATA = "Not can to load block data";

        private const int BLOCK_COUNT_MAX = 100000;
        private const int BLOCK_GET_TRY_MAX = 100;
        private const int ID_CHARS_MOD = 3;
        private const int ID_CHARS_NAME = 3;

        private float _testCoefficientReady = 0.0f;
        private string _testingSystemMessage;
        private TestResult _testResult;
        private int _blocksCountInDirectory;
        private int _blocksCountAdded;
        private float _timeLastCalcBlocksCount = -100;
        private int _lastCheckNumMod = 0;
        private int _lastCheckNumBlock = 0;

        public bool IsAsync => false;
        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => _testingSystemMessage;

        private int GetBlockFirstID(string ModName, string BlockName)
        {
            //Получаем текстовое сокрашение
            string abbreviatura = "";
            //Первые 3 символа мода
            for (int num = 0; num < ID_CHARS_MOD; num++)
            {
                //Если текущий символ больше чем имя
                if (num >= ModName.Length)
                    abbreviatura += "_";
                else abbreviatura += ModName[num];
            }

            //Первые 3 символа имени
            for (int num = 0; num < ID_CHARS_NAME; num++)
            {
                //Если текущий символ больше чем имя
                if (num >= BlockName.Length)
                    abbreviatura += "_";
                else abbreviatura += BlockName[num];
            }

            //Теперь есть Абревиатура получаем ID
            int abbreviaturaID = 0;
            //Перебираем каждый символ
            for (int num = 0; num < abbreviatura.Length; num++)
            {
                abbreviaturaID += abbreviatura[num] * (int)Mathf.Pow(5, num);
            }

            while (abbreviaturaID >= _storageData.Count)
            {
                abbreviaturaID -= _storageData.Count;
            }

            return abbreviaturaID;
        }
        public int GetBlockID(string ModName, string BlockName, bool onlyExistData = true)
        {
            //Получаем первичный id
            int idStart = GetBlockFirstID(ModName, BlockName);
            int idNow = idStart;

            for (int num = 0; num < BLOCK_GET_TRY_MAX; num++)
            {
                BlockData blockData = _storageData.Get(idNow, 0);

                //Проверяем по id что блок есть и у него совпадает мод и имя
                if (blockData != null && blockData.mod == ModName && blockData.name == BlockName)
                    return idNow;

                idNow++;
            }

            //Если нигде совпадений не нашлось то ищем первое пустое
            if (!onlyExistData) 
            {
                //возвращаем к старту
                idNow = idStart;
                for (int num = 0; num < BLOCK_GET_TRY_MAX; num++, idNow++)
                {
                    BlockData blockData = _storageData.Get(idNow, 0);

                    //Проверяем по id что блок есть и у него совпадает мод и имя
                    if (blockData == null || blockData.mod == ModName && blockData.name == BlockName)
                        return idNow;

                }

            }
            return -1;
        }

        public TestResult TestIt()
        {
            _testResult ??= new TestResult(nameof(BlockManager));
            _testingSystemMessage = nameof(BlockManager);

            CalcCountOfAllFolders();
            TryLoad();

            return _testResult;
        }

        public override void TryLoad()
        {
            _storageData = new StorageData<BlockData>(BLOCK_COUNT_MAX);

            //Проверяем есть ли папка мод
            if (!Directory.Exists(StrC.FOLDER_NAME_MOD))
                Directory.CreateDirectory(StrC.FOLDER_NAME_MOD);

            string[] directoriesMod = Directory.GetDirectories(StrC.FOLDER_NAME_MOD);

            for (int numMod = _lastCheckNumMod; numMod < directoriesMod.Length; numMod++) 
            {
                _lastCheckNumMod = numMod;
                //Проверяем список содержимого мода
                string pathModBlocks = GetPathBlock(directoriesMod[numMod]);
                //Проверяем что папка блоков существует
                if (!Directory.Exists(pathModBlocks))
                    continue;

                //Получаем список блоков
                string[] pathsBlockdata = Directory.GetDirectories(pathModBlocks);
                for (int numBlock = _lastCheckNumBlock; numBlock < pathsBlockdata.Length;)
                {
                    _lastCheckNumBlock = numBlock;
                    BlockData[] blockVariants = LoadDatas(pathsBlockdata[numBlock]);
                    int id = GetBlockID(blockVariants[0].mod, blockVariants[0].name, false);

                    //Если данного блока в списке нет надо добавить
                    if (id < 0)
                    {
                        _testResult.AddProblem($"{ERROR_TEXT_BLOCKS_CANT_TO_LOAD_BLOCK_DATA}: {blockVariants[0].mod} {blockVariants[0].name}", TypeProblem.Error);
                    }

                    _storageData.Add(id, blockVariants);

                    numBlock++;
                    _lastCheckNumBlock = numBlock;
                    _blocksCountAdded++;
                    break;
                }

                numMod++;
                _lastCheckNumMod = numMod;
                break;
            }

            if (_blocksCountInDirectory != 0)
                _testCoefficientReady = _blocksCountAdded / _blocksCountInDirectory;
            else 
            {
                _testCoefficientReady = 1;
                _testResult.AddProblem(ERROR_TEXT_BLOCKS_IS_NOT_FOUND, TypeProblem.Error);
            }
        }

        public override void Save(BlockData data, uint variation)
        {
            
        }

        public override BlockData Load(string path)
        {
            throw new System.NotImplementedException();
        }

        private void CalcCountOfAllFolders() 
        {
            if (Time.unscaledTime - _timeLastCalcBlocksCount < 20)
                return;

            _timeLastCalcBlocksCount = Time.unscaledTime;
            _blocksCountInDirectory = 0;

            bool isExist = Directory.Exists(StrC.FOLDER_NAME_MOD);
            if (!isExist) 
            {
                Directory.CreateDirectory(StrC.FOLDER_NAME_MOD);
            }

            string[] directoriesMod = Directory.GetDirectories(StrC.FOLDER_NAME_MOD);

            foreach (string directoryMod in directoriesMod)
            {
                string pathModBlocks = GetPathBlock(directoryMod);
                //Проверяем что папка блоков существует
                if (!Directory.Exists(pathModBlocks))
                    continue;

                //Получаем список блоков
                string[] pathsBlockdata = Directory.GetDirectories(pathModBlocks);
                _blocksCountInDirectory += pathsBlockdata.Length;
            }
        }

        private string GetPathBlock(string pathMod) => $"{pathMod}\\{StrC.FOLDER_NAME_BLOCKS}";

        static public BlockData[] LoadDatas(string pathBlock)
        {
            //Нужно узнать сколько есть вариантов в папке блока
            string[] pathVariants = Directory.GetDirectories(pathBlock);

            int maxVar = 0;
            //Создаем список блоков и вытаскиваем данные в него
            List<BlockData> blockDatasList = new List<BlockData>();
            foreach (string pathVar in pathVariants)
            {
                BlockData blockData = LoadData(pathVar);
                blockDatasList.Add(blockData);

                //обновляем максимум
                if (maxVar <= blockData.variant)
                    maxVar = blockData.variant + 1;
            }

            //Создаем массив вариантов
            BlockData[] blockDatas = new BlockData[maxVar];

            //Запихиваем
            foreach (BlockData blockData in blockDatasList)
            {
                if (blockDatas[blockData.variant] != null)
                {
                    Debug.LogError("Block load Error: have variant| " + blockData.mod + " " + blockData.name + " " + blockData.variant);
                    continue;
                }

                blockDatas[blockData.variant] = blockData;
            }

            return blockDatas;
        }

        static public BlockData LoadData(string pathBlockVariant)
        {
            //Если папки блока нет - выходим
            if (!Directory.Exists(pathBlockVariant))
            {
                Debug.Log(pathBlockVariant + " Not exist");
                return null;
            }

            string mod = "";
            string name = "";
            int variant = 0;
            BlockData.Type type = BlockData.Type.block;

            BlockData resultData;

            TypeBlock typeBlock = new TypeBlock();
            //TypeVoxel typeVoxel = new TypeVoxel();
            //TypeLiquid typeLiquid = new TypeLiquid();

            //Загрузка основных данных блока
            loadBlockMain(pathBlockVariant);

            if (type == BlockData.Type.voxels)
            {
                //typeVoxel.loadVoxel(pathBlockVariant);
                //resultData = typeVoxel;
                return null;
            }
            else if (type == BlockData.Type.liquid)
            {
                //typeLiquid.loadLiquid(pathBlockVariant);
                //resultData = typeLiquid;
                return null;
            }
            else //Type Block
            {
                typeBlock.loadBlock(pathBlockVariant);
                resultData = typeBlock;
            }

            resultData.mod = mod;
            resultData.name = name;
            resultData.variant = variant;

            loadBlockPhysics(pathBlockVariant);

            return resultData;

            void loadBlockMain(string path)
            {
                //Вытаскиваем путь
                string[] pathParts1 = path.Split("/");

                List<string> pathList = new List<string>();
                foreach (string pathCut in pathParts1)
                {
                    string[] pathParts2 = pathCut.Split("\\");
                    foreach (string part in pathParts2)
                    {
                        pathList.Add(part);
                    }
                }

                string[] pathMass = pathList.ToArray();
                //for (int num = 0; num < pathList.Count; num++) {
                //    pathMass[num] = pathList[num];
                //}



                if (pathMass.Length <= 3)
                {
                    pathMass = path.Split("\\");
                }

                if (pathMass.Length <= 3)
                {
                    Debug.LogError(path + " load name error");
                    return;
                }

                mod = pathMass[pathMass.Length - 4];
                name = pathMass[pathMass.Length - 2];
                variant = System.Convert.ToInt32(pathMass[pathMass.Length - 1]);

                //Нужно загрузить файл с основными данными
                loadMainTXT();

                void loadMainTXT()
                {
                    string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

                    //проверяем существование файла
                    if (!File.Exists(pathMainStr))
                    {
                        //Файла нет, ошибка
                        Debug.LogError("File main.txt not exist " + pathMainStr);
                        return;
                    }

                    //Вытаскиваем данные файла
                    string[] datasStr = File.ReadAllLines(pathMainStr);

                    //Проверяем все строки на данные
                    foreach (string dataStr in datasStr)
                    {
                        string[] data = dataStr.Split(StrC.SEPARATOR);

                        if (data.Length > 2)
                        {
                            Debug.LogError("Bad parametr: " + dataStr + " in " + pathMainStr);
                            continue;
                        }

                        GetType(data[0], data[1]);
                    }
                    //////////////////////////////////////////////////////////////////////////////////////
                    ///

                    void GetType(string name, string value)
                    {
                        if (name == StrC.type)
                        {
                            if (value == StrC.TBlock)
                                type = BlockData.Type.block;
                            else if (value == StrC.TVoxels)
                                type = BlockData.Type.voxels;
                            else if (value == StrC.TLiquid)
                                type = BlockData.Type.liquid;
                            else
                                Debug.LogError("Bad parametr of " + name + ": " + value);

                        }
                    }
                }
            }
            void loadBlockPhysics(string path)
            {
                string pathPhysics = path + "/" + StrC.physics;
                resultData.physics.loadColliderZone(pathPhysics);
            }
        }
    }
}
