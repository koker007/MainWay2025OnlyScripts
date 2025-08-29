using UnityEngine;

namespace Game.Data.Managers
{
    public abstract class DataManager<T> : MonoBehaviour
    {
        protected StorageData<T> _storageData;
        public abstract void TryLoad();
        public abstract void Save(T data, uint variation);
        public abstract T Load(string path);
    }
}
