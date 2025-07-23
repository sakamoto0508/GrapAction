using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";
    private const string LOOK_ACTION = "Look";
    private const string JUMP_ACTION = "Jump";

    public InputAction MoveAction => _moveAction;
    public InputAction LookAction => _lookAction;
    public InputAction JumpAction => _jumpAction;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        if(TryGetComponent<PlayerInput>(out var playerInput))
        {
            _moveAction = playerInput.actions[MOVE_ACTION];
            _lookAction = playerInput.actions[LOOK_ACTION];
            _jumpAction = playerInput.actions[JUMP_ACTION];
        }
    }
}
