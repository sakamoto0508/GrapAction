using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _jumpCooldown = 2f;
    [SerializeField] private float _ariMultiplier;
    [SerializeField] private Transform _orientation;
    private MovementState _state;
    private bool _isGround = true;
    private bool _isSprint = false;
    private bool _readyToJump = false;
    private Vector2 _currentInput;
    private Rigidbody _rb;
    private InputBuffer _inputBuffer;
    private void RegisterInputAction()
    {
        _inputBuffer.MoveAction.performed += OnInputMove;
        _inputBuffer.MoveAction.canceled += OnInputMove;
        _inputBuffer.JumpAction.started += OnInputJump;
        _inputBuffer.SprintAction.started += OnInputSprint;
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputBuffer = FindAnyObjectByType<InputBuffer>();
        RegisterInputAction();
        _rb.freezeRotation = true;
        _readyToJump = true;
    }
    private void Update()
    {
        StateHandler();
    }
    private void FixedUpdate()
    {
        
        if (_isGround)
        {
            Vector3 inputDirGround = _orientation.forward * _currentInput.y + _orientation.right * _currentInput.x;
            Vector3 velocity = inputDirGround.normalized * _moveSpeed;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
        else if (!_isGround)
        {
            Vector3 inputDirAir = _orientation.forward * _currentInput.y + _orientation.right * _currentInput.x;
            Vector3 velocity = inputDirAir.normalized * _moveSpeed * _ariMultiplier;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
    }
    private void OnInputMove(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }
    private void OnInputSprint(InputAction.CallbackContext context)
    {
        if (!_isSprint)
        {
            _isSprint = true;
        }
        else
        {
            _isSprint = false;
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

    private void StateHandler()
    {
        if (_isGround && _isSprint)
        {
            _state=MovementState.sprinting;
            _moveSpeed=_sprintSpeed;
        }
        else if (_isGround)
        {
            _state = MovementState.walking;
            _moveSpeed = _walkSpeed;
        }
        else
        {
            _state = MovementState.air;
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            _isGround = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            _isGround = false;
        }
    }
    private enum MovementState
    {
        walking,
        sprinting,
        air
    }
}
