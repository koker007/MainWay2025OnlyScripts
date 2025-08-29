using UnityEngine;

namespace Game.Data
{
    public class StorageData<T>
    {
        private readonly T[][] _dataset;
        private readonly int _count;

        public int Count => _count;

        public StorageData(int count)
        {
            if (count < 0)
                count = 0;

            _count = count;
            _dataset = new T[_count][];
        }
        public bool Add(int ID, T[] dataVariations)
        {
            bool isComplete = false;

            if (ID >= _dataset.Length)
            {
                return false;
            }

            _dataset[ID] = dataVariations;

            isComplete = true;
            return isComplete;
        }
        public T[] Get(int ID)
        {
            int IDNow = ID;
            if (ID >= _dataset.Length)
            {
                IDNow -= _dataset.Length;
            }
            return _dataset[ID];
        }
        public T Get(int ID, int variation) 
        {
            T[] data = Get(ID);

            if (data == null)
                return default;

            if (data.Length >= variation)
                return default;

            return data[variation];
        }
    }
}
