using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Testing
{
    public interface ITestingSystem
    {
        public bool IsAsync { get; }
        public float TestCoefficientReady { get; }
        public string TestingSystemMessage { get; }
        public TestResult TestIt();
    }

    public class TestResult
    {
        private bool _allOk;
        private readonly List<string> _problemsList;
        private readonly List<TypeProblem> _typeProblems;
        private string _testedModuleName;

        public bool AllOk => _allOk;
        public List<string> ProblemsList => _problemsList;
        public List<TypeProblem> TypeProblems => _typeProblems;
        public string TestedModuleName => _testedModuleName;

        public TestResult(string TestedModuleName)
        {
            _allOk = true;
            _problemsList = new List<string>();
            _typeProblems = new List<TypeProblem>();
            _testedModuleName = TestedModuleName;
        }

        public void AddProblem(string description, TypeProblem typeProblem)
        {
            _allOk = false;

            if (typeProblem == TypeProblem.Warning)
                Debug.LogWarning($"{TestedModuleName}: {description}");
            else
                Debug.LogError($"{TestedModuleName}: {description}");

            _problemsList.Add(description);
            _typeProblems.Add(typeProblem);
        }
    }

    public enum TypeProblem 
    {
        Error = 0,
        Warning = 1,
    }
}

