using TreeEditor;
using UnityEngine;
using Zenject;
using Game.Services;

namespace Game.Services.GPU
{
    public class GlobePerlin2D
    {
        public const short SIZE = 32;

        private static int LastPerlin2DID = 0;

        public const float factor = 0.875170906246f;

        public int id = 0;
        public bool calculated = false;

        public float[,,] result = new float[32, 32, 1];


        public float scale = 1;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;

        public float frequency = 1;

        public float offsetX = 0;
        public float offsetY = 0;
        public float offsetZ = 0;

        public int octaves = 1;

        public int repeatX; //Через сколько пикселей текстура повторяется по X
        public int repeatY; //Через сколько пикселей текстура повторяется по Y
        public float regionX; //регион старта от 0 до 1
        public float regionY;

        private static IGPUGlobePerlin2D _GPUCalculateService;

        public static void SetGPUCalculateService(IGPUGlobePerlin2D GPUCalculateService) 
        {
            if(_GPUCalculateService == null)
                _GPUCalculateService = GPUCalculateService;
        }

        public GlobePerlin2D(float scaleX, float scaleY, float scaleZ, float frequency, float offsetX, float offsetY, float offsetZ, int octaves, int repeatX, int repeatY, float regionX, float regionY)
        {
            id = LastPerlin2DID;
            LastPerlin2DID++;

            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
            this.frequency = frequency;

            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.offsetZ = offsetZ;

            this.octaves = octaves;

            this.repeatX = repeatX;
            this.repeatY = repeatY;

            this.regionX = regionX;
            this.regionY = regionY;
        }

        //Вычислить перлина
        public bool Calculate()
        {
            if (calculated) return calculated;

            _GPUCalculateService.Calculate(this);

            return calculated;
        }

        static public float[,] GetArrayMap(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ)
        {
            //Создаем новый массив
            float[,] arrayMap = new float[mapSizeX, mapSizeY];

            //Ищем фактор чанка
            float FactorChankX = (factor / ScaleX) * 32;
            float FactorChankY = (factor / ScaleY) * 32;
            float FactorChankZ = (factor / ScaleZ) * 32;

            //Определяем количество чанков
            int chankXMax = mapSizeX / 32;
            int chankXremain = mapSizeX % 32;
            if (chankXremain > 0)
                chankXMax++;

            int chankYMax = mapSizeY / 32;
            int chankYremain = mapSizeY % 32;
            if (chankYremain > 0)
                chankYMax++;

            for (int chankX = 0; chankX < chankXMax; chankX++)
            {
                int chankPixelStartX = chankX * 32;

                for (int chankY = 0; chankY < chankYMax; chankY++)
                {
                    int chankPixelStartY = chankY * 32;

                    float offSetX = OffSetX + FactorChankX * chankX;
                    float offSetY = OffSetY + FactorChankY * chankY;
                    float offSetZ = OffSetZ;

                    if (TimeZ)
                        offSetZ += Time.time * 0.1f;

                    if (TimeX)
                        offSetX += Time.time * 0.1f;

                    float regionX = (chankX * 32) / (float)mapSizeX;
                    float regionY = (chankY * 32) / (float)mapSizeY;

                    GlobePerlin2D dataPerlin2D = new GlobePerlin2D(ScaleX, ScaleY, ScaleZ, Freq, offSetX, offSetY, offSetZ, Octaves, mapSizeX, mapSizeY, regionX, regionY);
                    dataPerlin2D.Calculate();

                    //Если крайний чанк с остатком
                    int maxX = 32;
                    int maxY = 32;
                    if (chankX == chankXMax - 1 && chankXremain > 0)
                        maxX = chankXremain;
                    if (chankY == chankYMax - 1 && chankYremain > 0)
                        maxY = chankYremain;

                    //Запихиваем данные в текстуру
                    for (int x = 0; x < maxX; x++)
                    {
                        int posMapX = chankPixelStartX + x;
                        for (int y = 0; y < maxY; y++)
                        {
                            int posMapY = chankPixelStartY + y;
                            arrayMap[posMapX, posMapY] = dataPerlin2D.result[x, y, 0];
                        }
                    }
                }
            }
            return arrayMap;
        }
    }
}
