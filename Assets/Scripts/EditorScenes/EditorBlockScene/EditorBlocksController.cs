using Game.Data.Block;
using Game.Services;
using Game.Testing;
using Game.UI;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.IO;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlocksController: MonoBehaviour, ITestingSystem
    {
        public const float VOXEL_SIZE = 0.0625f;
        private const int BUFFER_SIZE_MAX = 500;

        private static BlockData[] _blockVariationsLast = new BlockData[0];
        private static BlockData[][] _blocksDataBuffer = new BlockData[BUFFER_SIZE_MAX][];
        private static BlockData[][] _blocksDataBufferNext = new BlockData[BUFFER_SIZE_MAX][];

        [Header("Panels")]
        [Required][SerializeField] EditorBlockPanelLeft _panelLeft;
        [Required][SerializeField] EditorBlockPanelMiddle _panelMiddle;
        [Required][SerializeField] EditorBlockPanelRight _panelRight;

        [Header("Prefabs CameraRig")]
        [Required][SerializeField] CameraRigPC _prefabCameraRigPC;
        [Required][SerializeField] CameraRigVR _prefabCameraRigVR;

        private float _testCoefficientReady = 1.0f;
        private TestResult _testResult;

        private ICameraRig _cameraRig;
        private IInputAdapter _inputAdapter;

        private IEnvironmentService _environmentService;
        private IInputService _inputService;

        private Subject<Unit> onDataMeshChange = new Subject<Unit>();

        public ICameraRig CameraRig => _cameraRig;
        public EditorBlockPanelLeft PanelLeft => _panelLeft;
        public EditorBlockPanelRight PanelRight => _panelRight;
        public BlockData blockData
        {
            get 
            {
                if (_blocksDataBuffer[0] == null)
                    return null;

                if (_blocksDataBuffer[0][0] == null)
                    return null;

                return _blocksDataBuffer[0][(int)_panelLeft.SliderVariationSelected.slider.value];
            }
        }
        public BlockData[] blockDatas => _blocksDataBuffer[0];
        public IObservable<Unit> OnDataMeshChange => onDataMeshChange;

        public bool IsAsync => true;

        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => nameof(EditorBlocksController);

        [Inject]
        private void Construct(IEnvironmentService environmentService, IInputService inputService) 
        {
            _environmentService = environmentService;
            _inputService = inputService;
        }

        private void Awake()
        {
            Initialize();
            Subscribe();
        }
        private void Start()
        {
            RedrawAll();
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Update()
        {
            UpdateCameraRig();
        }

        private void Subscribe() 
        {
            _panelLeft.buttonLoad.Button.onClick.AddListener(OnClickLoad);
            _panelLeft.buttonSave.Button.onClick.AddListener(OnClickSave);
            _panelLeft.InputModName.inputfield.onEndEdit.AddListener(OnEndEditModificationName);
            _panelLeft.InputBlockName.inputfield.onEndEdit.AddListener(OnEndEditBlockName);
            _panelLeft.SliderVariationMaximum.OnValueChanged
                .Subscribe(OnChangeVariationMaximum)
                .AddTo(this);
            _panelLeft.SliderVariationSelected.OnValueChanged
                .Subscribe(OnChangeVariationSelect)
                .AddTo(this);
            _panelLeft.SliderBlockType.OnValueChanged
                .Subscribe(OnChangeBlockType)
                .AddTo(this);
        }
        private void Unsubscribe() 
        {
            _panelLeft.buttonLoad.Button.onClick.RemoveListener(OnClickLoad);
            _panelLeft.buttonSave.Button.onClick.RemoveListener(OnClickSave);
            _panelLeft.InputModName.inputfield.onEndEdit.RemoveListener(OnEndEditModificationName);
            _panelLeft.InputBlockName.inputfield.onEndEdit.RemoveListener(OnEndEditBlockName);
        }

        private void Initialize()
        {
            InitializeCameraRig();
            InitializeInputAdapter();

            _blocksDataBuffer = new BlockData[BUFFER_SIZE_MAX][];
            _blocksDataBuffer[0] = new BlockData[1];
            _blocksDataBuffer[0][0] = new TypeBlock();
        }

        private void InitializeCameraRig() 
        {
            switch (_environmentService.runtimeMode)
            {
                case RuntimeMode.PC:
                    _cameraRig = Instantiate(_prefabCameraRigPC);
                    break;

                case RuntimeMode.VR:
                    _cameraRig = Instantiate(_prefabCameraRigVR);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        private void InitializeInputAdapter() 
        {
            switch (_environmentService.runtimeMode)
            {
                case RuntimeMode.PC:
                    _inputAdapter = new InputAdapterPC(_inputService.editorBlockControls);
                    break;

                case RuntimeMode.VR:
                    _inputAdapter = new InputAdapterVR(_inputService.editorBlockControls);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCameraRig() 
        {
            _cameraRig.Rotate(_inputAdapter.LookDelta);
            _cameraRig.Zoom(_inputAdapter.ZoomDelta);
            _cameraRig.UpdateTransform();
        }

        private void OnClickLoad() 
        {

            RedrawAll();
        }
        private void OnClickSave() 
        {
            Save();
        }

        private void OnEndEditModificationName(string value)
        {
            AddBlockDataInBuffer(_blocksDataBuffer[0]);
            ChangeNames(value, _blocksDataBuffer[0][0].name);
            UpdateLastVariations();
            RedrawAll();
        }
        private void OnEndEditBlockName(string value)
        {
            AddBlockDataInBuffer(_blocksDataBuffer[0]);
            ChangeNames(_blocksDataBuffer[0][0].mod, value);
            UpdateLastVariations();
            RedrawAll();
        }
        private void OnChangeVariationMaximum(float value)
        {
            AddBlockDataInBuffer(_blocksDataBuffer[0]);
            ChangeVariationsMax(value);
            UpdateLastVariations();
            RedrawAll();
        }
        private void OnChangeVariationSelect(float value) 
        {
            RedrawAll();
        }
        private void OnChangeBlockType(float value) 
        {
            RedrawAll();
        }

        private void RedrawAll() 
        {
            _panelLeft.Redraw();
            onDataMeshChange.OnNext(Unit.Default);
        }

        private void Save() 
        {
            string blockPath = _blocksDataBuffer[0][0].GetPathBlock();
            if(Directory.Exists(blockPath))
                Directory.Delete(blockPath, true);
            
            for (int variation = 0; variation < _blocksDataBuffer[0].Length; variation++)
            {
                _blocksDataBuffer[0][variation].SaveData();
            }
        }
        private void AddBlockDataInBuffer(BlockData[] blockData) 
        {
            _blocksDataBufferNext = new BlockData[BUFFER_SIZE_MAX][];

            //—мещаем данные в буфере чтобы освободить первое место
            for (int num = BUFFER_SIZE_MAX - 1; num > 0; num--) 
            {
                _blocksDataBuffer[num] = _blocksDataBuffer[num - 1];
            }
            _blocksDataBuffer[0] = blockData;
        }

        private void ChangeNames(string modName, string blockName) 
        {
            for (int num = 0; num < _blocksDataBuffer[0].Length; num++) 
            {
                _blocksDataBuffer[0][num].mod = modName;
                _blocksDataBuffer[0][num].name = blockName;
            }
        }
        private void UpdateLastVariations() 
        {
            //ѕеребираем все данные начина¤ от самых старых до последних
            for (int num = _blocksDataBuffer.Length - 1; num > 0; num--)
            {
                if (_blocksDataBuffer[num] == null)
                    continue;

                //≈сли в новых данных размерность выше чем запомнили
                if (_blocksDataBuffer[num].Length > _blockVariationsLast.Length)
                {
                    //—оздаем новую большую размерность
                    BlockData[] blockDatasNew = new BlockData[_blocksDataBuffer[num].Length];
                    for (int var = 0; var < blockDatasNew.Length; var++)
                    {
                        //≈сли така¤ вариаци¤ есть - запоминаем
                        if (var < _blockVariationsLast.Length)
                        {
                            blockDatasNew[var] = _blockVariationsLast[var];
                        }
                        else
                        {
                            blockDatasNew[var] = _blocksDataBuffer[num][var];
                        }
                    }
                    //ѕереписываем чтобы была размерность больше
                    _blockVariationsLast = blockDatasNew;
                }
                //≈сли размерность таже либо ниже
                else
                {
                    //ѕросто переписываем варианты более новыми данными, если така¤ вариаци¤ имеетс¤
                    for (int variant = 0; variant < _blockVariationsLast.Length && variant < _blocksDataBuffer[num].Length; variant++)
                    {
                        _blockVariationsLast[variant] = _blocksDataBuffer[num][variant];
                    }
                }
            }
        }
        private void ChangeVariationsMax(float value) 
        {
            int count = (int)value;
            BlockData[] variationsNew = new BlockData[count];
            BlockData[] blockDatasNew = new BlockData[count];
            for (int variation = 0; variation < count; variation++) 
            {
                //если размерность больше чем мы запоминали
                if (variation >= _blockVariationsLast.Length)
                {
                    //Новый блок
                    variationsNew[variation] = new TypeBlock();
                    variationsNew[variation].name = _blocksDataBuffer[0][0].name;
                    variationsNew[variation].mod = _blocksDataBuffer[0][0].mod;
                    variationsNew[variation].variant = variation;
                }
                else 
                {
                    variationsNew[variation] = _blockVariationsLast[variation];
                }

                //переносим последние данные вариативности
                if (variation < _blocksDataBuffer[0].Length)
                {
                    blockDatasNew[variation] = _blocksDataBuffer[0][variation];
                }
                else 
                {
                    blockDatasNew[variation] = variationsNew[variation];
                }
            }
            _blocksDataBuffer[0] = blockDatasNew;
        }

        public TestResult TestIt()
        {
            return _testResult;
        }
    }
}
