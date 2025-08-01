using UnityEngine;
using UnityEngine.UI;

public class DashCooldownBar : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private JoystickController joystick;
    [SerializeField] private Vector2 displayingOffset;

    private Slider bar; 

    void Awake()
    {
        bar = GetComponent<Slider>();
    }

    private void LateUpdate() // LateUpdate VS Update 테스트 해보기
    {
        transform.position = Camera.main.WorldToScreenPoint(player.position + new Vector3(displayingOffset.x, displayingOffset.y));
        bar.value = joystick.DashTimer / joystick.dashCooldown;
    }
}