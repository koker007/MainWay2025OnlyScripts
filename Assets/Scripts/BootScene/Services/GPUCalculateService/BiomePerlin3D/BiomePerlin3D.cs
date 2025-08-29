using System;
using TreeEditor;
using UnityEngine;

namespace Game.Services.GPU
{
    public class BiomePerlin3D
    {
        public const float factor = 0.875170906246f;

        public int id = 0;
        public bool calculated = false;

        public float[,,,] result;


        //public BiomeData.GenRule[] genRules;

        public int repeatX; //Через сколько пикселей текстура повторяется по X
        public int repeatY;
        public int repeatZ; //Через сколько пикселей текстура повторяется по Y
        public float regionX; //регион старта от 0 до 1
        public float regionY;
        public float regionZ;

        private static IGPUBiomePerlin3D _GPUBiomePerlin3D;
        public static void SetGPUCalculateService(IGPUBiomePerlin3D gpuBiomePerlin3D)
        {
            if (_GPUBiomePerlin3D == null)
                _GPUBiomePerlin3D = gpuBiomePerlin3D;
        }

        /// <summary>
        /// Нужно передать правила генерации блоков и позицию участка для которого генерируем
        /// </summary>
        /// <param name="genRules"></param>
        /// <param name="repeatX"></param>
        /// <param name="repeatZ"></param>
        /// <param name="regionX"></param>
        /// <param name="regionZ"></param>
        public BiomePerlin3D(/*BiomeData.GenRule[] genRules,*/ int repeatX, int repeatY, int repeatZ, float regionX, float regionY, float regionZ)
        {
            throw new NotImplementedException();
            //result = new float[32, 32, 32, genRules.Length];

            //this.genRules = genRules;

            this.repeatX = repeatX;
            this.repeatY = repeatY;
            this.repeatZ = repeatZ;

            this.regionX = regionX;
            this.regionY = regionY;
            this.regionZ = regionZ;
        }

        //Вычислить перлина
        public bool Calculate()
        {
            if (calculated)
                return calculated;

            _GPUBiomePerlin3D.Calculate(this);

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
