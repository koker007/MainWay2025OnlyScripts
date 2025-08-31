using Game.Services.GPU;
using Game.Testing;
using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

namespace Game.Data.Block
{
    public class TypeBlock : BlockData
    {
        private const string FOLDER_NAME_WALL = "Wall";
        private const string FOLDER_NAME_TEXTURE = "Texture";
        private const string FILE_NAME_FACE = "Face";
        private const string FILE_NAME_BACK = "Back";
        private const string FILE_NAME_LEFT = "Left";
        private const string FILE_NAME_RIGHT = "Right";
        private const string FILE_NAME_UP = "Up";
        private const string FILE_NAME_DOWN = "Down";
        private const string FILE_TYPE_JSON = ".json";


        private static IGPUBlockWall _GPUBlockWall;
        public static IGPUBlockWall GPUBlockWall { set { _GPUBlockWall = value ?? throw new ArgumentNullException(nameof(value)); } }

        public Wall wallFace;
        public Wall wallBack;
        public Wall wallLeft;
        public Wall wallRight;
        public Wall wallUp;
        public Wall wallDown;

        public TypeBlock(BlockData blockData) : base(blockData)
        {
            TypeBlock typeBlock = blockData as TypeBlock;

            if (typeBlock == null)
                return;

            type = Type.block;
            wallFace = typeBlock.wallFace;
            wallBack = typeBlock.wallBack;
            wallLeft = typeBlock.wallLeft;
            wallRight = typeBlock.wallRight;
            wallUp = typeBlock.wallUp;
            wallDown = typeBlock.wallDown;
        }

        /// <summary>
        /// Визуальная часть стены
        /// </summary>
        public class Wall
        {
            public Texture2D texture;
            public BlockForms forms;

            private Side _side;

            public Side side => _side;

            public Wall(Side side)
            {
                _side = side;
                forms = new BlockForms();
            }

            public void LoadFrom(string path)
            {
                if (!Directory.Exists(path))
                {
                    Debug.Log(path + " Block wall not Exist");
                    return;
                }

                string pathWall;
                string pathTexture;

                GetFullPatch(path, out pathWall, out pathTexture);

                Texture2D texture = new Texture2D(16, 16);
                texture.filterMode = FilterMode.Point;

                if (File.Exists(pathTexture))
                {
                    byte[] data = File.ReadAllBytes(pathTexture);
                    texture.LoadImage(data);
                }

                if (File.Exists(pathWall))
                {
                    string json = File.ReadAllText(pathWall);
                    BlockForms.Voxels voxs = JsonUtility.FromJson<BlockForms.Voxels>(json);

                    forms.voxel = voxs.To2D();
                }

                this.texture = texture;
            }
            public void SaveTo(string path)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string pathWall;
                string pathTexture;

                GetFullPatch(path, out pathWall, out pathTexture);

                byte[] textureData = texture.EncodeToPNG();
                FileStream textureStream = File.Open(pathTexture, FileMode.OpenOrCreate);
                textureStream.Write(textureData);
                textureStream.Close();

                BlockForms.Voxels voxels = new BlockForms.Voxels();
                voxels.From2D(forms.voxel);

                string json = JsonUtility.ToJson(voxels);
                File.WriteAllText($"{pathWall}{FILE_TYPE_JSON}", json);
            }

            //Установить цвет тестовую текстуру
            public void SetTextureTest()
            {
                texture = new Texture2D(16, 16);

                float one = 1.0f / 16.0f;

                //x = red //y = green
                if (_side == Side.face)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color(one * x, one * y, 0.0f));
                        }
                    }
                }
                else if (_side == Side.back)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color(1 - (one * x), one * y, 1.0f));
                        }
                    }
                }
                //x = blue //y = green
                else if (_side == Side.left)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color(0.0f, one * y, 1 - (one * x)));
                        }
                    }
                }
                else if (_side == Side.right)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color(1.0f, one * y, one * x));
                        }
                    }
                }
                //x = r // y = blue
                else if (_side == Side.up)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color(x * one, 1.0f, one * y));
                        }
                    }
                }
                //x = r // y = blue
                else if (_side == Side.down)
                {
                    for (int x = 0; x < forms.voxel.GetLength(0); x++)
                    {
                        for (int y = 0; y < forms.voxel.GetLength(1); y++)
                        {
                            texture.SetPixel(x, y, new Color((x * one), 0.0f, 1 - one * y));
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error cube wall side");
                }

                texture.Apply();
                texture.filterMode = FilterMode.Point;
            }

            public void calcVertices()
            {
                //создаем плоскость стены изходя из глубины вокселей

                _GPUBlockWall.Calculate(new BlockWall(forms, _side));
            }

            private void GetFullPatch(string path, out string pathWall, out string pathTexture) 
            {
                pathWall = $"{path}\\{FOLDER_NAME_WALL}";
                pathTexture = $"{path}\\{FOLDER_NAME_TEXTURE}";

                if (_side == Side.face)
                {
                    pathWall += FILE_NAME_FACE;
                    pathTexture += FILE_NAME_FACE;
                }
                else if (_side == Side.back)
                {
                    pathWall += FILE_NAME_BACK;
                    pathTexture += FILE_NAME_BACK;
                }
                else if (_side == Side.left)
                {
                    pathWall += FILE_NAME_LEFT;
                    pathTexture += FILE_NAME_LEFT;
                }
                else if (_side == Side.right)
                {
                    pathWall += FILE_NAME_RIGHT;
                    pathTexture += FILE_NAME_RIGHT;
                }
                else if (_side == Side.up)
                {
                    pathWall += FILE_NAME_UP;
                    pathTexture += FILE_NAME_UP;
                }
                else
                {
                    pathWall += FILE_NAME_DOWN;
                    pathTexture += FILE_NAME_DOWN;
                }
                pathTexture += StrC.formatPNG;
            }

            public bool IsProblem(in TestResult testResult)
            {
                bool isProblem = false;

                if (forms.vertices.Length != BlockWall.COUNT_VERTICES)
                {
                    testResult.AddProblem($"wall vertices count isn't {BlockWall.COUNT_VERTICES}. This is {forms.vertices.Length}", TypeProblem.Error);
                    isProblem = true;
                }
                if (forms.triangles.Length != BlockWall.COUNT_VERTICES)
                {
                    testResult.AddProblem($"wall triangles count isn't {BlockWall.COUNT_VERTICES}. This is {forms.triangles.Length}", TypeProblem.Error);
                    isProblem = true;
                }
                if (forms.uv.Length != BlockWall.COUNT_VERTICES)
                {
                    testResult.AddProblem($"wall uv count isn't {BlockWall.COUNT_VERTICES}. This is {forms.uv.Length}", TypeProblem.Error);
                    isProblem = true;
                }
                if (forms.uvShadow.Length != BlockWall.COUNT_VERTICES)
                {
                    testResult.AddProblem($"wall uvShadow count isn't {BlockWall.COUNT_VERTICES}. This is {forms.uvShadow.Length}", TypeProblem.Error);
                    isProblem = true;
                }

                foreach (Vector3 point in forms.vertices)
                {
                    if (point.x < -1f || point.x > 2f ||
                        point.y < -1f || point.y > 2f ||
                        point.z < -1f || point.z > 2f)
                    {
                        testResult.AddProblem($"wall vertices point is too far. This is {point}", TypeProblem.Error);
                    }
                }

                return isProblem;
            }
        }

        public TypeBlock()
        {
            type = Type.block;

            wallFace = new Wall(Side.face);
            wallBack = new Wall(Side.back);
            wallRight = new Wall(Side.right);
            wallLeft = new Wall(Side.left);
            wallUp = new Wall(Side.up);
            wallDown = new Wall(Side.down);

            wallFace.SetTextureTest();
            wallBack.SetTextureTest();
            wallRight.SetTextureTest();
            wallLeft.SetTextureTest();
            wallUp.SetTextureTest();
            wallDown.SetTextureTest();

            wallFace.calcVertices();
            wallBack.calcVertices();
            wallRight.calcVertices();
            wallLeft.calcVertices();
            wallUp.calcVertices();
            wallDown.calcVertices();
        }

        public override Color GetColor()
        {
            if (color != null)
                return color;

            //Расчитываем цвет блока
            color = CalcColor(wallUp);

            return color;

            Color CalcColor(Wall wall)
            {

                Vector3 colorVec = new Vector3(0.5f, 0.5f, 0.5f);
                int colorCount = 0;

                for (int x = 0; x < wall.texture.width; x++)
                {
                    for (int y = 0; y < wall.texture.height; y++)
                    {
                        Color color = wall.texture.GetPixel(x, y);
                        if (color.a < 1)
                            continue;

                        colorVec.x += color.r;
                        colorVec.y += color.g;
                        colorVec.z += color.b;

                        colorCount++;
                    }
                }


                if (colorCount < 1)
                {
                    colorCount = 1;
                }

                colorVec /= colorCount;

                return new Color(colorVec.x, colorVec.y, colorVec.z);
            }
        }

        public override Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down, Mesh meshResult = null)
        {
            meshResult ??= new Mesh();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            List<Vector3> listVert = new List<Vector3>();
            List<Vector2> listUV = new List<Vector2>();
            List<Vector2> listUV2 = new List<Vector2>();

            //int addNum = 0;

            if (face)
            {
                listVert.AddRange(wallFace.forms.vertices);
                listUV.AddRange(wallFace.forms.uv);
                listUV2.AddRange(wallFace.forms.uv);
            }
            if (back)
            {
                listVert.AddRange(wallBack.forms.vertices);
                listUV.AddRange(wallBack.forms.uv);
                listUV2.AddRange(wallBack.forms.uv);
            }
            if (right)
            {
                listVert.AddRange(wallRight.forms.vertices);
                listUV.AddRange(wallRight.forms.uv);
                listUV2.AddRange(wallRight.forms.uv);
            }
            if (left)
            {
                listVert.AddRange(wallLeft.forms.vertices);
                listUV.AddRange(wallLeft.forms.uv);
                listUV2.AddRange(wallLeft.forms.uv);
            }
            if (up)
            {
                listVert.AddRange(wallUp.forms.vertices);
                listUV.AddRange(wallUp.forms.uv);
                listUV2.AddRange(wallUp.forms.uv);
            }
            if (down)
            {
                listVert.AddRange(wallDown.forms.vertices);
                listUV.AddRange(wallDown.forms.uv);
                listUV2.AddRange(wallDown.forms.uv);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////

            meshResult.vertices = listVert.ToArray();
            meshResult.uv = listUV.ToArray();
            meshResult.uv2 = listUV2.ToArray();

            meshResult.subMeshCount = 6;

            int addNum = 0;
            if (face)
            {
                meshResult.SetTriangles(wallFace.forms.triangles, 0);
                addNum += wallFace.forms.triangles.Length;
            }

            if (back)
            {
                int[] trianglesBack = Calc.Array.changeEvery(wallBack.forms.triangles, addNum);
                meshResult.SetTriangles(trianglesBack, 1);
                addNum += wallBack.forms.triangles.Length;
            }

            if (right)
            {
                int[] trianglesRight = Calc.Array.changeEvery(wallRight.forms.triangles, addNum);
                meshResult.SetTriangles(trianglesRight, 2);
                addNum += wallRight.forms.triangles.Length;
            }

            if (left)
            {
                int[] trianglesLeft = Calc.Array.changeEvery(wallLeft.forms.triangles, addNum);
                meshResult.SetTriangles(trianglesLeft, 3);
                addNum += wallLeft.forms.triangles.Length;
            }

            if (up)
            {
                int[] trianglesUp = Calc.Array.changeEvery(wallUp.forms.triangles, addNum);
                meshResult.SetTriangles(trianglesUp, 4);
                addNum += wallUp.forms.triangles.Length;
            }

            if (down)
            {
                int[] trianglesDown = Calc.Array.changeEvery(wallDown.forms.triangles, addNum);
                meshResult.SetTriangles(trianglesDown, 5);
            }

            stopwatch.Stop();
            Debug.Log("Stopwatch processor: " + stopwatch.ElapsedMilliseconds);

            return meshResult;
        }

        public void saveBlock(string pathBlock)
        {
            saveBlockWall(pathBlock);
        }

        public void loadBlock(string path)
        {
            loadBlockWall(path);
        }

        void saveBlockWall(string pathBlock)
        {
            string pathWalls = $"{pathBlock}/{FOLDER_NAME_WALL}";

            wallFace.SaveTo(pathWalls);
            wallBack.SaveTo(pathWalls);
            wallLeft.SaveTo(pathWalls);
            wallRight.SaveTo(pathWalls);
            wallUp.SaveTo(pathWalls);
            wallDown.SaveTo(pathWalls);
        }
        void loadBlockWall(string path)
        {
            string pathWalls = $"{path}\\{FOLDER_NAME_WALL}";

            wallFace.LoadFrom(pathWalls);
            wallBack.LoadFrom(pathWalls);
            wallLeft.LoadFrom(pathWalls);
            wallRight.LoadFrom(pathWalls);
            wallUp.LoadFrom(pathWalls);
            wallDown.LoadFrom(pathWalls);

            wallFace.calcVertices();
            wallBack.calcVertices();
            wallRight.calcVertices();
            wallLeft.calcVertices();
            wallUp.calcVertices();
            wallDown.calcVertices();
        }

        public override void SaveData()
        {
            //Создаем путь к папке блоке
            string path = GetPathVariation();

            //Проверяем есть ли папка
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Сохраняем главные данные блока
            saveBlockMain(path);

            //Сохраняем стены
            saveBlock(path);

            //Сохраняем физику
            saveBlockPhysics(path);

            void saveBlockMain(string path)
            {
                string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

                //Сохранить надо в текстовый файл
                //создаем список того что надо запомнить
                List<string> dataList = new List<string>();

                string dataOne = "";
                //Запоминаем тип
                dataOne = StrC.type + StrC.SEPARATOR + StrC.TBlock;

                dataList.Add(dataOne);

                //Сохраняем в файл
                FileStream fileStream;
                //Если файла нет - создаем
                if (!File.Exists(pathMainStr))
                {
                    fileStream = File.Create(pathMainStr);
                    fileStream.Close();
                }
                File.WriteAllLines(pathMainStr, dataList.ToArray());
            }
            void saveBlockPhysics(string pathBlock)
            {
                string pathPhysics = pathBlock + "/" + StrC.physics;

                physics.saveColliderZone(pathPhysics);
            }

        }
    }
}
