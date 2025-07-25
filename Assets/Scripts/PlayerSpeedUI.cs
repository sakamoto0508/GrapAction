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
            // プレイヤーの現在の速度を取得
            Rigidbody rb = playerMove.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 水平速度のみを計算（Y軸を除く）
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float speed = horizontalVelocity.magnitude;

                // 速度を表示（小数点1桁まで）
                _playerSpeedText.text = $"速度: {speed:F1} m/s";
            }
        }
    }
}
