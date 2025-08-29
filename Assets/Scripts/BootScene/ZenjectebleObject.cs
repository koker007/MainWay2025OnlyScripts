using Game.Services.Managers;
using Game.UI.Menu;
using UnityEngine;
using Zenject;

public class ZenjectebleObject: MonoBehaviour
{
    private DiContainer _container;

    protected DiContainer Container => _container;

    [Inject]
    private void Construct(DiContainer container)
    {
        _container = container;
    }

    private void Awake()
    {
        _container.BindInstance(this).AsTransient();
    }
}
