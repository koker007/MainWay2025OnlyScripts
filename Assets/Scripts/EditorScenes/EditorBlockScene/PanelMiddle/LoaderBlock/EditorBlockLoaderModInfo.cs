using Game.Data.Block;
using Game.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scene.Editor.Block
{
    public class EditorBlockLoaderModInfo: MonoBehaviour
    {
        [Required][SerializeField] private UITextBackground _mod;

        public void Initialize(string mod)
        {
            _mod.SetText(mod);
        }
    }
}
