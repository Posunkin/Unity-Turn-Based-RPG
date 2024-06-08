using UnityEngine;
using Zenject;

public class CombatUIInstaller : MonoInstaller
{
    [SerializeField] private CombatUI _combatUI;

    public override void InstallBindings()
    {
        Container.Bind<CombatUI>().FromInstance(_combatUI).AsSingle();
    }
}
