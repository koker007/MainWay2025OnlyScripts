using Game.UI;
using UnityEngine;
using Zenject;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockPanelRight: MonoBehaviour
    {
        private UISlider _sliderPanels;

        private EditorBlocksController _blocksController;

        [Inject]
        void Construct(EditorBlocksController blocksController)
        {
            _blocksController = blocksController;
        }
    }
}
