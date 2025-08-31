using UnityEngine;

namespace Game.Data.Managers
{
    public abstract class DataManager<T> : MonoBehaviour
    {
        protected StorageData<T> _storageData;
        public abstract void Initialize();
        public abstract void TryLoad();
        public abstract void Save(T data);
        public abstract T Load(string path);

        public T GetFromID(int ID) 
        {
            if (ID >= _storageData.Count)
                return default;

            return _storageData.Get(ID);
        }
    }
}
