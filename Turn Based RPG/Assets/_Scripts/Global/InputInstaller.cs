using Zenject;

public class InputInstaller : MonoInstaller
{
    private PlayerInput _input;

    public override void InstallBindings()
    {
        _input = new PlayerInput();
        _input.Enable();
        Container.Bind<PlayerInput>().FromInstance(_input).AsSingle().NonLazy();
    }
}