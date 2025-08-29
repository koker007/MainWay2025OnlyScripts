using UnityEngine;

namespace Game.Services.GPU
{
    public interface IGPUGlobePerlin2D
    {
        void InitializePerlin2D();

        public void Calculate(GlobePerlin2D chankPerlin2D);
    }
}
