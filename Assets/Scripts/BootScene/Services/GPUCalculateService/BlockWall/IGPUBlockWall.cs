using Game.Data.Block;

namespace Game.Services.GPU
{
    public interface IGPUBlockWall
    {
        void InitializeBlockWall();
        public void Calculate(BlockWall dataBlockWall);
    }
}
