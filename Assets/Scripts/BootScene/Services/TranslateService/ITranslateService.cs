using Game.Testing;
using UnityEngine;

public interface ITranslateService: ITestingSystem
{
    public string GetTextFromKey(string key, string defaultText);
    public string GetTextFromKey(string key);
    public bool SetTextFromKey(string key, string textNew);
}
