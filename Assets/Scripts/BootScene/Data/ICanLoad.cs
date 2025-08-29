using UnityEngine;

namespace Game.Data
{
    public interface ICanLoad<T>
    {
        public T LoadData(string pathLoad);
    }
}
