using Game.Testing;
using UnityEngine;

namespace Game.Services
{
    public interface IOpenXRService: ITestingSystem
    {
        public bool IsActive { get; }
    }
}
