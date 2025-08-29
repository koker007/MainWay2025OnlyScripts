using UnityEngine;

namespace Game.Services.GPU
{
    public interface IGPUBiomePerlin3D
    {
        void InitializePerlin3D();

        public void Calculate(BiomePerlin3D chankPerlin3D);
    }
}
