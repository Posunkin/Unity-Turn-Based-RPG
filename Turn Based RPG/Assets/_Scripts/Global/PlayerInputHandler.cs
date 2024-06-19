using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _input;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Enable();
    }
}
