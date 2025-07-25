using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _jumpCooldown = 2f;
    [SerializeField] private float _ariMultiplier;
    [SerializeField] private Transform _orientation;
    private bool _isGround = true;
    private bool _readyToJump = false;
    private Vector2 _currentInput;
    private Rigidbody _rb;
    private InputBuffer _inputBuffer;
    private void RegisterInputAction()
    {
        _inputBuffer.MoveAction.performed += OnInputMove;
        _inputBuffer.MoveAction.canceled += OnInputMove;
        _inputBuffer.JumpAction.started += OnInputJump;
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

    private void FixedUpdate()
    {
        //Vector3 inputDir = new Vector3(_currentInput.x, 0, _currentInput.y);
        //Vector3 velocity = inputDir.normalized * _playerSpeed;

        //velocity.y = _rb.linearVelocity.y;
        //_rb.linearVelocity = velocity;
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
    private void OnInputJump(InputAction.CallbackContext context)
    {
        //if (_isGround)
        //{
        //    _rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);
        //    _isGround = false;
        //}
        if (_isGround && _readyToJump)
        {
            _readyToJump = false;
           
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(transform.up * _jumpPower, ForceMode.Impulse);

            Invoke(nameof(ResetJump), _jumpCooldown);
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

}
