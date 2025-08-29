using Game.Testing;
using UnityEngine;
using Steamworks;
using Zenject;

namespace Game.Services
{
    public class SteamService : MonoBehaviour, IDistributionService
    {
        readonly TestResult testResult = new TestResult(nameof(SteamService));

        [SerializeField] private SteamManager _steamManager;

        public bool IsUserLoggedIn => SteamManager.Initialized && SteamUser.BLoggedOn();

        public float TestCoefficientReady => 0;

        public string TestingSystemMessage => nameof(SteamService);

        public bool IsAsync => true;

        public ulong GetUserId()
        {
            CSteamID cSteamID = SteamUser.GetSteamID();
            return cSteamID.m_SteamID;
        }

        public string GetUserName()
        {
            throw new System.NotImplementedException();
        }

        public TestResult TestIt()
        {
            if (!SteamManager.Initialized)
            {
                testResult.AddProblem($"This service is not Initialized", TypeProblem.Error);
            }
            return testResult;
        }
    }
}
