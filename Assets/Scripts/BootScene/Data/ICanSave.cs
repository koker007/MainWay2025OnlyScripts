using UnityEngine;

namespace Game.Data
{
    public interface ICanSave<T>
    {
        abstract void SaveData();
    }
}
