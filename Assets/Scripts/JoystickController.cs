using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 플로팅 조이스틱: 원 내부 = 이동, 원 밖 드래그 뒤 손 떼면 대시.
/// </summary>
public class JoystickController : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform joystickBg; // 조이스틱 베이스
    [SerializeField] private RectTransform joystickHandle;

    [Header("회피 쿨타임")]
    public float dashCooldown = 3f;

    public event Action<Vector2> OnDash;   // 대시 알림

    public float Horizontal => _input.x;
    public float Vertical   => _input.y;
    public float DashTimer => _dashTimer;

    Vector2 _input;        // -1 ~ 1
    Vector2 _rawDrag;
    float   _radius;
    float   _dashTimer;

    void Awake()
    {
        // 1. 베이스 각 코너의 좌표 구하기 
        // 2. 두 코너의 좌표로부터 베이스의 반지름 추출
        Vector3[] corners = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        joystickBg.GetWorldCorners(corners);
        _radius = (corners[2] - corners[0]).x / 2;

        _dashTimer = dashCooldown;

        joystickBg.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_dashTimer < dashCooldown)
        {
            _dashTimer += Time.deltaTime;
        }
        else
        {
            _dashTimer = dashCooldown;
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        joystickBg.position = e.position;
        joystickHandle.position = e.position;
        joystickBg.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData e)
    {
        _rawDrag = e.position - (Vector2)joystickBg.position;
        Vector2 clamped = Vector2.ClampMagnitude(_rawDrag, _radius);

        joystickHandle.position = (Vector2)joystickBg.position + clamped;
        _input = clamped / _radius;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (_rawDrag.magnitude > _radius && _dashTimer == dashCooldown)          // 원 밖 → 대시
        {
            OnDash?.Invoke(_rawDrag.normalized);
            _dashTimer = 0f;
        }

        _input = Vector2.zero;
        joystickBg.gameObject.SetActive(false);
    }
}