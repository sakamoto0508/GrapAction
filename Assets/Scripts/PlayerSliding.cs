using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
    private Transform _playerCamera;
    private Transform _playerObj;
    private Rigidbody _rb;
    private PlayerMove _playerMove;
    private InputBuffer _inputBuffer;
    private PlayerState _playerState;

    [Header("Sliding")]
    private float _maxSlidingTime;
    private float _slidingForce;
    private float _slidingTimer;
    private float _slidingYScale;
    private float _startYScale;

    private bool _isSliding;
    private float _speed;
    private void RegisterInputAction()
    {
        _inputBuffer.SlidingAction.started += OnInputSliding;
    }
    private void OnDestroy()
    {
        _inputBuffer.SlidingAction.started -= OnInputSliding;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerMove = GetComponent<PlayerMove>();
        _playerState = GetComponent<PlayerState>();
        _startYScale = transform.localScale.y;
        RegisterInputAction();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _speed = horizontalVelocity.magnitude;
    }
    private void OnInputSliding(InputAction.CallbackContext context)
    {
        if (_playerState.CurrentState == PlayerState.PlayerStateType.Sliding)
        {
            StopSliding();
        }
        if (_speed != 0)
        {
            StartSliding();
            _playerState.CurrentState = PlayerState.PlayerStateType.Sliding;
        }
    }

    private void StartSliding()
    {

    }

    private void StopSliding()
    {

    }

}
