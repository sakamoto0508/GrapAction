using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public PlayerStateType CurrentState= PlayerStateType.walking;
    public enum PlayerStateType
    {
        walking,
        sprinting,
        crouching,
        air,
        Sliding
    }
}
