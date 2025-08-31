using UnityEngine;

namespace Game.Data
{
    public class StorageData<T>
    {
        private const int COUNT_NULL_SEARCH_DEPTH = 100;

        private readonly T[] _dataset;
        private readonly int _count;

        public int Count => _count;

        public StorageData(int count)
        {
            if (count < 0)
                count = 0;

            _count = count;
            _dataset = new T[_count];
        }
        public bool Add(int ID, T dataVariations, bool isReplace = false)
        {
            bool isComplete = false;

            if (ID >= _dataset.Length)
            {
                return false;
            }

            if (isReplace)
            {
                _dataset[ID] = dataVariations;
                isComplete = true;
            }
            else
            {
                for (int i = 0; i < COUNT_NULL_SEARCH_DEPTH; i++)
                {
                    int idNow = ID + i;
                    if (_dataset[idNow] != null)
                        continue;

                    _dataset[idNow] = dataVariations;
                    isComplete = true;
                    break;
                }
            }
            return isComplete;
        }
        public T Get(int ID)
        {
            int IDNow = ID % _dataset.Length;
            //if (ID >= _dataset.Length)
            //{
            //    IDNow -= _dataset.Length;
            //}
            return _dataset[ID];
        }
    }
}
