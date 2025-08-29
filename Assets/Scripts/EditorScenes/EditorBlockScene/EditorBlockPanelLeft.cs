using Game.Data.Block;
using Game.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelLeft : MonoBehaviour
    {
        public const int VARIATION_MAX = 10;
        public const int VARIATION_MIN = 1;

        [Required][SerializeField] UIButton _buttonLoad;
        [Required][SerializeField] UIButton _buttonSave;
        [Space]
        [Required][SerializeField] UIInputField _inputModName;
        [Required][SerializeField] UIInputField _inputBlockName;
        [Required][SerializeField] UISlider _sliderVariationMaximum;
        [Required][SerializeField] UISlider _sliderVariationSelected;
        [Required][SerializeField] UISlider _sliderBlockType;

        private EditorBlocksController _blocksController;

        public UIButton buttonLoad => _buttonLoad;
        public UIButton buttonSave => _buttonSave;
        public UIInputField InputModName => _inputModName;
        public UIInputField InputBlockName => _inputBlockName;
        public UISlider SliderVariationMaximum => _sliderVariationMaximum;
        public UISlider SliderVariationSelected => _sliderVariationSelected;
        public UISlider SliderBlockType => _sliderBlockType;

        [Inject]
        void Construct(EditorBlocksController blocksController) 
        {
            _blocksController = blocksController;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize() 
        {
            SliderVariationMaximum.slider.maxValue = VARIATION_MAX;
            SliderVariationMaximum.slider.minValue = VARIATION_MIN;
            SliderVariationMaximum.slider.value = 1;
        }

        public void Redraw()
        {
            BlockData[] blockDatas = _blocksController.blockDatas;

            InputModName.inputfield.SetTextWithoutNotify(blockDatas[0].mod);
            InputBlockName.inputfield.SetTextWithoutNotify(blockDatas[0].name);

            SliderVariationMaximum.slider.value = blockDatas.Length;
            SliderVariationMaximum.SetValueText();

            SliderVariationSelected.slider.maxValue = blockDatas.Length - 1;
            SliderVariationSelected.slider.minValue = 0;
            SliderVariationMaximum.SetValueText();
        }

        private void getModName()
        {
            if (_blocksController.blockDatas[0].mod == null ||
                _blocksController.blockDatas[0].mod.Length <= 0)
            {

                InputModName.inputfield.text = "";
                return;
            }

            InputModName.inputfield.text = _blocksController.blockDatas[0].mod;
        }
        private void getBlockName()
        {
            if (_blocksController.blockDatas[0].name == null ||
                _blocksController.blockDatas[0].name.Length <= 0)
            {

                _inputBlockName.inputfield.text = "";
                return;
            }

            _inputBlockName.inputfield.text = _blocksController.blockDatas[0].name;
        }

        // перерисовать ползунки
        private void redrawVariation()
        {
            //ќбновл¤ем слайдер максимума
            SliderVariationMaximum.slider.minValue = 1;
            SliderVariationMaximum.slider.maxValue = 10;
            SliderVariationMaximum.slider.value = _blocksController.blockDatas.Length;
            SliderVariationMaximum.UpdateText();

            //ќбновл¤ем слайдер выбранного
            SliderVariationSelected.slider.minValue = 0;
            SliderVariationSelected.slider.maxValue = SliderVariationMaximum.slider.value - 1;
            SliderVariationSelected.UpdateText();
        }
        private void redrawType()
        {
            BlockData data = _blocksController.blockData;
            TypeBlock typeBlock = data as TypeBlock;
            //TypeVoxel typeVoxel = data as TypeVoxel;
            //TypeLiquid typeLiquid = data as TypeLiquid;

            int maximum = (int)BlockData.Type.block;
            if (maximum < (int)BlockData.Type.liquid)
                maximum = (int)BlockData.Type.liquid;
            else if (maximum < (int)BlockData.Type.voxels)
                maximum = (int)BlockData.Type.voxels;

            SliderBlockType.slider.minValue = 0;
            SliderBlockType.slider.maxValue = maximum;


            //ћен¤ем значение слайдера
            if (typeBlock != null)
            {
                SliderBlockType.slider.value = (int)BlockData.Type.block;
                SliderBlockType.SetValueText(StrC.TBlock);
            }
            /*
            else if (typeVoxel != null)
            {
                sliderBlockType.slider.value = (int)BlockData.Type.voxels;
                sliderBlockType.SetValueText(StrC.TVoxels);
            }
            else if (typeLiquid != null)
            {
                sliderBlockType.slider.value = (int)BlockData.Type.liquid;
                sliderBlockType.SetValueText(StrC.TLiquid);
            }
            */
        }
    }
}
