using Game.Services.GPU;
using Game.Testing;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Services.GPU
{
    public class GPUBiomePerlin3D: MonoBehaviour, ITestingSystem, IGPUBiomePerlin3D
    {
        const short TEST_COUNT_MAX = 10;

        [PreviewField(120, ObjectFieldAlignment.Center)]
        [ShowInInspector]
        private Texture2D _textureBiomePerlin3D;

        [Required][SerializeField] private ComputeShader _shaderGlobePerlin3D;

        private int _kernelIndexPerlin3D = 0;
        private float _testCoefficientReady = 0.0f;

        uint _lengthX = 0;
        uint _lengthY = 0;
        uint _lengthZ = 0;

        readonly private TestResult _testResult = new TestResult(nameof(GPUGlobePerlin2D));

        public float TestCoefficientReady => _testCoefficientReady;

        public string TestingSystemMessage => nameof(GPUBiomePerlin3D);

        public bool IsAsync => false;

        public TestResult TestIt()
        {
            //���� ������� �������� ������� ��� ������� �� ����������� ����������
            for (short num = 0; num < TEST_COUNT_MAX; num++)
            {
                _testCoefficientReady = ((float) num) / TEST_COUNT_MAX;
                if (TestBiomePerlin3D())
                    break;
            }

            return _testResult;
        }

        [Button]
        public bool TestBiomePerlin3D()
        {
            bool isProblem = false;
            
            return isProblem;
        }

        public void InitializePerlin3D()
        {
            BiomePerlin3D.SetGPUCalculateService(this);

            _kernelIndexPerlin3D = _shaderGlobePerlin3D.FindKernel("CSMain");

            //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
            _shaderGlobePerlin3D.GetKernelThreadGroupSizes(_kernelIndexPerlin3D, out _lengthX, out _lengthY, out _lengthZ);
        }

        public void Calculate(BiomePerlin3D chenkPerlin3D)
        {
            int dataPosSize = sizeof(float);
            int sizeOneData = dataPosSize;

            //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
            ComputeBuffer bufferResult = new ComputeBuffer(chenkPerlin3D.result.Length, sizeOneData);
            bufferResult.SetData(chenkPerlin3D.result);
            //�������� ����� ������ � ������
            _shaderGlobePerlin3D.SetBuffer(_kernelIndexPerlin3D, "_datas", bufferResult);

            throw new NotImplementedException();
            //ComputeBuffer bufferRules = new ComputeBuffer(chenkPerlin3D.genRules.Length, BiomeData.GenRule.SizeOf);
            //bufferRules.SetData(chenkPerlin3D.genRules);
            //�������� ����� ������ � ������
            //_shaderGlobePerlin3D.SetBuffer(_kernelIndexPerlin3D, "_rules", bufferRules);

            //_shaderGlobePerlin3D.SetInt("_RulesMax", chenkPerlin3D.genRules.Length);

            _shaderGlobePerlin3D.SetFloat("_factor", BiomePerlin3D.factor);

            _shaderGlobePerlin3D.SetInt("_repeatX", chenkPerlin3D.repeatX);
            _shaderGlobePerlin3D.SetInt("_repeatY", chenkPerlin3D.repeatY);
            _shaderGlobePerlin3D.SetInt("_repeatZ", chenkPerlin3D.repeatZ);

            _shaderGlobePerlin3D.SetFloat("_regionX", chenkPerlin3D.regionX);
            _shaderGlobePerlin3D.SetFloat("_regionY", chenkPerlin3D.regionY);
            _shaderGlobePerlin3D.SetFloat("_regionZ", chenkPerlin3D.regionZ);

            //������ ���������� �������
            _shaderGlobePerlin3D.Dispatch(_kernelIndexPerlin3D, 1, 1, 1);

            //�������� ������ �� �������
            bufferResult.GetData(chenkPerlin3D.result);

            //������� ��� ������ ��������� ���������� � ������ � ������
            chenkPerlin3D.calculated = true;

            //����������� ����� ������
            bufferResult.Dispose();
            throw new NotImplementedException();
            //bufferRules.Dispose();
        }
    }
}
