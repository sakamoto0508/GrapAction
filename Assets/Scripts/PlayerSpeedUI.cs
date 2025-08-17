using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeedUI : MonoBehaviour
{
    [SerializeField] private Text _playerSpeedText;
    private PlayerMove playerMove;
    private Rigidbody _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMove = FindAnyObjectByType<PlayerMove>();
        _rb = playerMove.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove != null && _playerSpeedText != null)
        {
            if (_rb != null)
            {
                // �������x�݂̂��v�Z�iY���������j
                Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
                float speed = horizontalVelocity.magnitude;

                // ���x��\���i�����_1���܂Łj
                _playerSpeedText.text = $"���x: {speed:F1} m/s";
            }
        }
    }
}
