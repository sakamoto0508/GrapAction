using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _groundDrag;
    private float _moveSpeed = 10f;

    [Header("GroundCheck")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _isGround = true;

    [Header("Jumping")]
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _jumpCooldown = 2f;
    [SerializeField] private float _ariMultiplier = 0.5f;
    private bool _readyToJump = false;
    private bool _isAir = false;

    [Header("Crouching")]
    [SerializeField] private float _couchSpeed = 3f;
    [SerializeField] private float _crouchYScale;
    private float _startYScale;

    [SerializeField] private Transform _playerCamera;
    private MovementState _state;
    private Vector2 _currentInput;
    private Rigidbody _rb;
    private InputBuffer _inputBuffer;
    private void RegisterInputAction()
    {
        _inputBuffer.MoveAction.performed += OnInputMove;
        _inputBuffer.MoveAction.canceled += OnInputMove;
        _inputBuffer.JumpAction.started += OnInputJump;
        _inputBuffer.SprintAction.started += OnInputSprint;
        _inputBuffer.CrouchAction.started += OnInputCrouch;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputBuffer = FindAnyObjectByType<InputBuffer>();
        RegisterInputAction();
        _rb.freezeRotation = true;
        _readyToJump = true;
        _startYScale = transform.localScale.y;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_state);
        }
        // ground check
        _isGround = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);
        StateHandler();

    }
    private void FixedUpdate()
    {
        if (_isGround)
        {
            Vector3 inputDirGround = _playerCamera.forward * _currentInput.y + _playerCamera.right * _currentInput.x;
            Vector3 velocity = inputDirGround.normalized * _moveSpeed;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
            _isAir = false;
        }
        else if (!_isGround)
        {
            Vector3 inputDirAir = _playerCamera.forward * _currentInput.y + _playerCamera.right * _currentInput.x;
            Vector3 velocity = inputDirAir.normalized * _moveSpeed * _ariMultiplier;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
            _isAir = true;
        }
    }
    private void OnInputMove(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }
    private void OnInputSprint(InputAction.CallbackContext context)
    {
        if (_state == MovementState.crouching)
            return;
        if (_state == MovementState.walking)
        {
            _state = MovementState.sprinting;
        }
        else
        {
            _state = MovementState.walking;
        }
    }
    private void OnInputJump(InputAction.CallbackContext context)
    {
        if (_isGround && _readyToJump)
        {
            _readyToJump = false;

            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);

            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }
    private void OnInputCrouch(InputAction.CallbackContext context)
    {
        if (_state != MovementState.crouching || _isAir == true)
        {
            _state = MovementState.crouching;
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);

        }
        else
        {
            _state = MovementState.walking;
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        switch (_state)
        {
            case MovementState.walking:
                _moveSpeed = _walkSpeed;
                break;
            case MovementState.crouching:
                _moveSpeed = _couchSpeed;
                break;
            case MovementState.sprinting:
                _moveSpeed = _sprintSpeed;
                break;
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }
}
