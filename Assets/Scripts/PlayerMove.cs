using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private Transform _orientation;
    private Vector2 _currentInput;
    private Rigidbody _rb;
    private InputBuffer _inputBuffer;
    private void RegisterInputAction()
    {
        _inputBuffer.MoveAction.performed += OnInputMove;
        _inputBuffer.MoveAction.canceled += OnInputMove;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb=GetComponent<Rigidbody>();
        _inputBuffer = FindAnyObjectByType<InputBuffer>();
        RegisterInputAction();
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        //Vector3 inputDir = new Vector3(_currentInput.x, 0, _currentInput.y);
        //Vector3 velocity = inputDir.normalized * _playerSpeed;

        //velocity.y = _rb.linearVelocity.y;
        //_rb.linearVelocity = velocity;

        Vector3 inputDir = _orientation.forward * _currentInput.y + _orientation.right * _currentInput.x;
        Vector3 velocity = inputDir.normalized * _moveSpeed;
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;
    }
    private void OnInputMove(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }
}
