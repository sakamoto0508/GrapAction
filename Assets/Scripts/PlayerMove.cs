using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _groundDrag;
    private float _moveSpeed = 10f;
    private Vector2 _currentInput;

    [Header("GroundCheck")] 
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _isGround = true;

    [Header("Slope Handling")] 
    [SerializeField] private float _maxSlopeAngle;
    private bool _exitingSlope=false;
    private RaycastHit _slopeHit;

    [Header("Jumping")]
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _jumpCooldown = 2f;
    [SerializeField] private float _airMultiplier = 0.5f;
    private bool _readyToJump = false;
    private bool _isAir = false;

    [Header("Crouching")]
    [SerializeField] private float _crouchSpeed = 3f;
    [SerializeField] private float _crouchYScale;
    private float _startYScale;

    [Header("References")]
    [SerializeField] private Transform _playerCamera;
    private PlayerState _playerState;
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

    private void OnDestroy()
    {
        _inputBuffer.MoveAction.performed -= OnInputMove;
        _inputBuffer.MoveAction.canceled -= OnInputMove;
        _inputBuffer.JumpAction.started -= OnInputJump;
        _inputBuffer.SprintAction.started -= OnInputSprint;
        _inputBuffer.CrouchAction.started -= OnInputCrouch;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputBuffer = FindAnyObjectByType<InputBuffer>();
        _playerState = GetComponent<PlayerState>();
        RegisterInputAction();
        _rb.freezeRotation = true;
        _readyToJump = true;
        _startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        _isGround = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);
        StateHandler();
    }

    private void FixedUpdate()
    {
        if (OnSlope() && !_exitingSlope)
        {
            SlopeMovement();
        }
        else if (_isGround)
        {
            GroundMovement();
        }
        else if (!_isGround)
        {
            AirMovement();
        }
        _rb.useGravity = !OnSlope();
    }
    private void SlopeMovement()
    {
        //平面に垂直な法線ベクトルによって定義される平面上にベクトルを射影する。
        //第一引数平面に投影したいベクトル、第二引数投影s会の平面の法線ベクトル（垂直な方向）
        Vector3 slopeForward = Vector3.ProjectOnPlane(_playerCamera.forward, _slopeHit.normal).normalized;
        Vector3 slopeRight = Vector3.ProjectOnPlane(_playerCamera.right, _slopeHit.normal).normalized;
        // プレイヤーの入力を坂の方向に合わせて変換
        Vector3 inputOnSlope = slopeForward * _currentInput.y + slopeRight * _currentInput.x;
        Vector3 moveDir = inputOnSlope.normalized * _moveSpeed;
        float yVel = _rb.linearVelocity.y;
        _rb.linearVelocity = new Vector3(moveDir.x, yVel, moveDir.z);
        // 坂を上がるときに浮かないよう下向きの力を追加
        if (_rb.linearVelocity.y > 0 && !_exitingSlope)
        {
            _rb.AddForce(Vector3.down * 98f, ForceMode.Force);
        }
        _isAir = false;
    }
    private void GroundMovement()
    {
        Vector3 inputDir = _playerCamera.forward * _currentInput.y + _playerCamera.right * _currentInput.x;
        float yVel = _rb.linearVelocity.y;
        Vector3 moveXZ = inputDir.normalized * _moveSpeed;
        _rb.linearVelocity = new Vector3(moveXZ.x, yVel, moveXZ.z);
        _isAir = false;
    }

    private void AirMovement()
    {
        Vector3 inputDir = _playerCamera.forward * _currentInput.y + _playerCamera.right * _currentInput.x;
        float yVel = _rb.linearVelocity.y;
        Vector3 moveXZ = inputDir.normalized * _moveSpeed * _airMultiplier;
        _rb.linearVelocity = new Vector3(moveXZ.x, yVel, moveXZ.z);
        _isAir = true;
    }

    private bool OnSlope()
    {
        //プレイヤーから下方向にRaycastを飛ばしヒットしたオブジェクトの情報を保存するマックスマックス距離まで
        if (Physics.Raycast(transform.position, Vector3.down,
                out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            //傾斜の角度を計算
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            //角度が最大傾斜角度より小さく、ゼロでない場合はboolがtrue
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private void OnInputMove(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }

    private void OnInputSprint(InputAction.CallbackContext context)
    {
        if (_playerState.CurrentState == PlayerState.PlayerStateType.crouching)
            return;
        if (_playerState.CurrentState == PlayerState.PlayerStateType.walking)
        {
            _playerState.CurrentState = PlayerState.PlayerStateType.sprinting;
        }
        else
        {
            _playerState.CurrentState = PlayerState.PlayerStateType.walking;
        }
    }

    private void OnInputJump(InputAction.CallbackContext context)
    {
        if ((OnSlope() || _isGround) && _readyToJump)
        {
            _readyToJump = false;
            _exitingSlope = true;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);
            Invoke(nameof(ResetJump), _jumpCooldown);
        }
    }

    private void OnInputCrouch(InputAction.CallbackContext context)
    {
        if (_playerState.CurrentState != PlayerState.PlayerStateType.crouching && !_isAir)
        {
            _playerState.CurrentState = PlayerState.PlayerStateType.crouching;
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
        }
        else
        {
            _playerState.CurrentState = PlayerState.PlayerStateType.walking;
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        switch (_playerState.CurrentState)
        {
            case PlayerState.PlayerStateType.walking:
                _moveSpeed = _walkSpeed;
                break;
            case PlayerState.PlayerStateType.crouching:
                _moveSpeed = _crouchSpeed;
                break;
            case PlayerState.PlayerStateType.sprinting:
                _moveSpeed = _sprintSpeed;
                break;
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
        _exitingSlope = false;
    }
}