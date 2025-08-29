using Game.Testing;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.Data.Block
{
    public class BlockWall
    {
        //16*16 количество вокселей
        //5 количество сторон (лицевая и боковые)
        //2 количество треугольников на каждую сторону
        //3 количество вершин на каждый треугольник
        //const int verticesCount = 16 * 16 * 5 * 2 * 3;


        //16*16 количество вокселей
        //3 количество сторон у вокселя (лицевая, право, вверх)
        //2 количество треугольников на каждую сторону

        //прибавляем 16 нижних сторон + 16 левых сторон + общее количество сторон и умножаем это все на 2 количество треугольников в каждой стороне
        public const int COUNT_VERTICES = (16 + 16 + (16 * 16 * 3)) * 2 * 3;

        public BlockForms blockForms;
        public Side side;

        public BlockWall(BlockForms blockForms, Side side)
        {
            this.blockForms = blockForms;
            this.side = side;

            //создаем вершин на каждый воксель
            this.blockForms.vertices = new Vector3[COUNT_VERTICES];
            this.blockForms.triangles = new int[COUNT_VERTICES];

            //Создаем тексель на каждый воксель
            this.blockForms.uv = new Vector2[COUNT_VERTICES];
            this.blockForms.uvShadow = new Vector2[COUNT_VERTICES];
        }
    }

    [Serializable]
    public class BlockForms
    {
        public float[,] voxel = new float[16, 16];
        //Стена 16 на 16
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        public Vector2[] uvShadow;

        [System.Serializable]
        public class Voxels
        {
            public float[][] height;
            public Voxels() 
            {
                height = new float[16][];
                for (int i = 0; i < 16; i++)
                    height[i] = new float[16];
            }

            public float[,] To2D()
            {
                int width = height.Length;
                int heightInner = height[0].Length;
                float[,] result = new float[width, heightInner];

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < heightInner; y++)
                        result[x, y] = height[x][y];

                return result;
            }
            public void From2D(float[,] source)
            {
                int width = source.GetLength(0);
                int heightInner = source.GetLength(1);
                height = new float[width][];
                for (int x = 0; x < width; x++)
                {
                    height[x] = new float[heightInner];
                    for (int y = 0; y < heightInner; y++)
                        height[x][y] = source[x, y];
                }
            }
        }
    }

    public enum Side
    {
        face = 0,
        back = 1,
        right = 2,
        left = 3,
        up = 4,
        down = 5
    }
}