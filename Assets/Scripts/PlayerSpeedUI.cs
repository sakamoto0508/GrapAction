using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeedUI : MonoBehaviour
{
    [SerializeField] private Text _playerSpeedText;
    private PlayerMove playerMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMove = FindAnyObjectByType<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove != null && _playerSpeedText != null)
        {
            // �v���C���[�̌��݂̑��x���擾
            Rigidbody rb = playerMove.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // �������x�݂̂��v�Z�iY���������j
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float speed = horizontalVelocity.magnitude;

                // ���x��\���i�����_1���܂Łj
                _playerSpeedText.text = $"���x: {speed:F1} m/s";
            }
        }
    }
}
