using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";
    private const string LOOK_ACTION = "Look";
    private const string JUMP_ACTION = "Jump";
    private const string SPRINT_ACTION = "Sprint";
    private const string CROUCH_ACTION = "Crouch";

    public InputAction MoveAction => _moveAction;
    public InputAction LookAction => _lookAction;
    public InputAction JumpAction => _jumpAction;
    public InputAction SprintAction => _sprintAction;
    public InputAction CrouchAction => _crouchAction;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _crouchAction;

    private void Awake()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            _moveAction = playerInput.actions[MOVE_ACTION];
            _lookAction = playerInput.actions[LOOK_ACTION];
            _jumpAction = playerInput.actions[JUMP_ACTION];
            _sprintAction = playerInput.actions[SPRINT_ACTION];
            _crouchAction = playerInput.actions[CROUCH_ACTION];
        }
    }
}
