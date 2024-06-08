using UnityEngine;
using Zenject;

public class CombatControllerInstaller : MonoInstaller
{
    [SerializeField] private CombatController _combatController;
    
    public override void InstallBindings()
    {
        Container.Bind<CombatController>().FromInstance(_combatController).AsSingle();
    }
}