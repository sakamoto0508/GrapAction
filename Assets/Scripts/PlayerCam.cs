using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float _sensX;
    [SerializeField] private float _sensY;
    [SerializeField] private Transform _orientation;
    private float xRotation;
    private float yRotation;
    private InputBuffer _inputBuffer;
    private Vector2 _currentInput;

    private void RegisterInputAction()
    {
        if (_inputBuffer != null)
        {
            _inputBuffer.LookAction.performed += OnInputLook;
            _inputBuffer.LookAction.canceled += OnInputLook;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputBuffer = FindAnyObjectByType<InputBuffer>();
        RegisterInputAction();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = _currentInput.x * _sensX * Time.deltaTime;
        float mouseY = _currentInput.y * _sensY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        //YŽ²‰ñ“]‚ð90‚©‚ç-90‚ÉŒÅ’è
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        _orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
    private void OnInputLook(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }
}
