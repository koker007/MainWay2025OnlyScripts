using Game.Testing;
using System.Threading.Tasks;

public interface ILoadingManager
{
    public Task LoadSceneAsync(string targetSceneName);
}
