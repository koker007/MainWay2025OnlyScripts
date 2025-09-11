using Game.Testing;
using UnityEditor;
using UnityEngine;

namespace Game.Services
{
    public interface IEnvironmentService: ITestingSystem
    {
        public bool isInitialized { get; }
        public RuntimeMode runtimeMode { get; }
    }

    public enum RuntimeMode
    {
        None = 0,
        PC,
        VR
    }
}
