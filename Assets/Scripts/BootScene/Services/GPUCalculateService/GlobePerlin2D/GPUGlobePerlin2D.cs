using Game.Testing;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using Zenject;

namespace Game.Services.GPU
{
    public class GPUGlobePerlin2D : MonoBehaviour, ITestingSystem, IGPUGlobePerlin2D
    {
        const short TEST_COUNT_MAX = 100;

        const string RESULT_TEST_GPU_GLOBE_PERLIN_2D_FAIL_SEAMLESS_HORIZONTAL = "Fail globe perlin test seamless horizontal";
        const string RESULT_TEST_GPU_GLOBE_PERLIN_2D_FAIL_SEAMLESS_POLES = "Fail globe perlin test seamless poles";
        const float SEAM_TEXTURE_THRESHOLD = 0.1f;

        [PreviewField(120, ObjectFieldAlignment.Center)] [ShowInInspector]
        private Texture2D _textureGlobePerlin2D;

        [Required][SerializeField] private ComputeShader _shaderGlobePerlin2D;

        private int _kernelIndexPerlin2D = 0;
        private float _testCoefficientReady = 0.0f;

        uint _lengthX = 0;
        uint _lengthY = 0;
        uint _lengthZ = 0;

        readonly private TestResult _testResult = new TestResult(nameof(GPUGlobePerlin2D));

        public float TestCoefficientReady => _testCoefficientReady;
        public string TestingSystemMessage => nameof(GPUGlobePerlin2D);
        public bool IsAsync => false;

        private int TestNum = 0;

        void Start() 
        {
            InitializePerlin2D();
        }

        public TestResult TestIt()
        {
            TestGlobePerlin2D();

            TestNum++;
            _testCoefficientReady = ((float)TestNum) / (TEST_COUNT_MAX - 1);

            return _testResult;
        }

        [Button]
        public bool TestGlobePerlin2D() 
        {
            bool isProblem = false;

            float scaleX = GlobePerlin2D.SIZE / 2 * Random.Range(0.8f, 1.2f);
            float scaleY = GlobePerlin2D.SIZE / 2 * Random.Range(0.8f, 1.2f);
            float scaleZ = GlobePerlin2D.SIZE / 2 * Random.Range(0.8f, 1.2f);
            float frequency = Random.Range(0.8f, 1.2f);
            float offsetX = Random.Range(0f,1024f);
            float offsetY = Random.Range(0f, 1024f);
            float offsetZ = Random.Range(0f, 1024f);
            int octaves = Random.Range(1, 2);
            int repeatX = GlobePerlin2D.SIZE;
            int repeatY = GlobePerlin2D.SIZE;
            int regionX = 0;
            int regionY = 0;
            GlobePerlin2D globePerlin2D = new GlobePerlin2D(scaleX, scaleY, scaleZ, frequency, offsetX, offsetY, offsetZ, octaves, repeatX, repeatY, regionX, regionY);
            globePerlin2D.Calculate();

            _textureGlobePerlin2D = new Texture2D(GlobePerlin2D.SIZE, GlobePerlin2D.SIZE);
            for (int x = 0; x < GlobePerlin2D.SIZE; x++) 
            {
                for (int y = 0; y < GlobePerlin2D.SIZE; y++) 
                {
                    float intensive = globePerlin2D.result[x, y, 0];
                    _textureGlobePerlin2D.SetPixel(x,y, new Color(intensive, intensive, intensive));
                }
            }
            _textureGlobePerlin2D.filterMode = FilterMode.Point;
            _textureGlobePerlin2D.Apply();

            bool checkFailHorizontal = false;
            bool checkFailPoles = false;

            float absSizeHorizontal = 0;
            float absSizeMinY = 0;
            float absSizeMaxY = 0;

            //Проверка бесшовности по высоте
            for (short y = 0; y < GlobePerlin2D.SIZE; y++) 
            {
                absSizeHorizontal = System.Math.Abs(globePerlin2D.result[0, y, 0] - globePerlin2D.result[GlobePerlin2D.SIZE - 1, y, 0]);
                if (absSizeHorizontal > SEAM_TEXTURE_THRESHOLD) 
                {
                    checkFailHorizontal = true;
                    break;
                }
            }

            //Проверка беcшовности по полюсам
            for (short x = 0; x < GlobePerlin2D.SIZE; x++) 
            {
                short xHalfOffset = (short)(x - (GlobePerlin2D.SIZE / 2));
                if (xHalfOffset < 0) xHalfOffset += GlobePerlin2D.SIZE;

                short maxY = GlobePerlin2D.SIZE - 1;

                absSizeMinY = System.Math.Abs(globePerlin2D.result[x, 0, 0] - globePerlin2D.result[xHalfOffset, 0, 0]);
                absSizeMaxY = System.Math.Abs(globePerlin2D.result[x, maxY, 0] - globePerlin2D.result[xHalfOffset, maxY, 0]);

                if (absSizeMinY > SEAM_TEXTURE_THRESHOLD ||
                    absSizeMaxY > SEAM_TEXTURE_THRESHOLD) 
                {
                    checkFailPoles = true;
                    break;
                }
            }

            if (checkFailHorizontal)
            {
                _testResult.AddProblem($"{RESULT_TEST_GPU_GLOBE_PERLIN_2D_FAIL_SEAMLESS_HORIZONTAL} {nameof(absSizeHorizontal)} {absSizeHorizontal.ToString().Substring(0, 3)}", TypeProblem.Error);
                isProblem = true;
            }
            if (checkFailPoles)
            {
                _testResult.AddProblem($"{RESULT_TEST_GPU_GLOBE_PERLIN_2D_FAIL_SEAMLESS_POLES} {nameof(absSizeMinY)} {absSizeMinY.ToString().Substring(0, 3)} {nameof(absSizeMaxY)} {absSizeMaxY.ToString().Substring(0, 3)}", TypeProblem.Error);
                isProblem = true;
            }
            return isProblem;
        }

        public void InitializePerlin2D()
        {
            GlobePerlin2D.SetGPUCalculateService(this);
            _textureGlobePerlin2D = new Texture2D(GlobePerlin2D.SIZE, GlobePerlin2D.SIZE);
            //Ищем в шейдере программу по расчету перлина
            _kernelIndexPerlin2D = _shaderGlobePerlin2D.FindKernel("CSMain");
            //Получаем информацию из шейдера какая возможная длина за один раcчет
            _shaderGlobePerlin2D.GetKernelThreadGroupSizes(_kernelIndexPerlin2D, out _lengthX, out _lengthY, out _lengthZ);
        }

        public void Calculate(GlobePerlin2D repeatPerlin2D)
        {

            int dataPosSize = sizeof(float);
            int sizeOneData = dataPosSize;

            //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
            ComputeBuffer bufferResult = new ComputeBuffer(repeatPerlin2D.result.Length, sizeOneData);
            bufferResult.SetData(repeatPerlin2D.result);

            //Помещаем буфер данных в шейдер
            _shaderGlobePerlin2D.SetBuffer(_kernelIndexPerlin2D, "datas", bufferResult);
            _shaderGlobePerlin2D.SetFloat("_offsetX", repeatPerlin2D.offsetX);
            _shaderGlobePerlin2D.SetFloat("_offsetY", repeatPerlin2D.offsetY);
            _shaderGlobePerlin2D.SetFloat("_offsetZ", repeatPerlin2D.offsetZ);

            float factor = GlobePerlin2D.factor / repeatPerlin2D.scale;
            float factorX = GlobePerlin2D.factor / repeatPerlin2D.scaleX;
            float factorY = GlobePerlin2D.factor / repeatPerlin2D.scaleY;
            float factorZ = GlobePerlin2D.factor / repeatPerlin2D.scaleZ;
            _shaderGlobePerlin2D.SetFloat("_factor", factor);
            _shaderGlobePerlin2D.SetFloat("_factorX", factorX);
            _shaderGlobePerlin2D.SetFloat("_factorY", factorY);
            _shaderGlobePerlin2D.SetFloat("_factorZ", factorZ);


            _shaderGlobePerlin2D.SetFloat("_frequency", repeatPerlin2D.frequency);
            _shaderGlobePerlin2D.SetFloat("_octaves", repeatPerlin2D.octaves);

            _shaderGlobePerlin2D.SetInt("_repeatX", repeatPerlin2D.repeatX);
            _shaderGlobePerlin2D.SetInt("_repeatY", repeatPerlin2D.repeatY);

            _shaderGlobePerlin2D.SetFloat("_regionX", repeatPerlin2D.regionX);
            _shaderGlobePerlin2D.SetFloat("_regionY", repeatPerlin2D.regionY);

            //Начать вычисления шейдера
            _shaderGlobePerlin2D.Dispatch(_kernelIndexPerlin2D, 1, 1, 1);

            //Вытащить данные из шейдера
            bufferResult.GetData(repeatPerlin2D.result);

            //Сказать что данные закончили вычисления и готовы к работе
            repeatPerlin2D.calculated = true;

            //Высвободить видео память
            bufferResult.Dispose();
        }
    }
}
