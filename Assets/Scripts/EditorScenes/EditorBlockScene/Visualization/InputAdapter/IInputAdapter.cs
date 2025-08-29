using UnityEngine;

namespace Game.Scene.Editor.Block
{
    //указатель/луч, нажатия, модификаторы
    public interface IInputAdapter
    {
        Vector2 LookDelta { get; }
        float ZoomDelta { get; }
    }
}
