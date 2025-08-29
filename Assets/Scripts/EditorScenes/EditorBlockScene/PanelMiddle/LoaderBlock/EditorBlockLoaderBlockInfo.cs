using Game.Data.Block;
using Game.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockLoaderBlockInfo: MonoBehaviour
    {
        [Required][SerializeField] private UITextBackground _name;
        [Required][SerializeField] private UITextBackground _id;
        [Required][SerializeField] private UITextBackground _tBlockCount;
        [Required][SerializeField] private UITextBackground _tVoxelCount;
        [Required][SerializeField] private UITextBackground _tLiquidCount;
        [Required][SerializeField] private UITextBackground _variations;

        private void Initialize(string name, int id, int TBlocksCount, int TVoxelCount, int TLiquidCount, string variations)
        {
            _name.SetText(name);
            _id.SetText(id.ToString());
            _tBlockCount.SetText(TBlocksCount.ToString());
            _tVoxelCount.SetText(TVoxelCount.ToString());
            _tLiquidCount.SetText(TLiquidCount.ToString());
            _variations.SetText(variations);
        }
    }
}
