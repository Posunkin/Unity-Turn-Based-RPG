using UnityEngine;
using Zenject;

public class TacticalGridInstaller : MonoInstaller
{
    [SerializeField] private TacticalGrid _grid;

    public override void InstallBindings()
    {
        Container.Bind<TacticalGrid>().FromInstance(_grid).AsSingle();
    }
}
