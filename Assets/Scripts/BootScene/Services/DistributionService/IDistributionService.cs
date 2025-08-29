using Game.Testing;
using UnityEngine;

namespace Game.Services
{
    public interface IDistributionService: ITestingSystem
    {
        public bool IsUserLoggedIn { get; }
        public string GetUserName();
        public ulong GetUserId();
    }
}
