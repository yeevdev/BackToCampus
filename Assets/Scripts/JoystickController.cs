using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 플로팅 조이스틱: 원 내부 = 이동, 원 밖 드래그 뒤 손 떼면 대시.
/// </summary>
public class JoystickController : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform joystickBg;
    [SerializeField] private RectTransform joystickHandle;

    public event Action<Vector2> OnDash;   // 대시 알림

    public float Horizontal => _input.x;
    public float Vertical   => _input.y;

    Vector2 _input;        // -1 ~ 1
    Vector2 _rawDrag;
    float   _radius;

    void Awake()
    {
        _radius = joystickBg.sizeDelta.x * 0.5f;
        joystickBg.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData e)
    {
        joystickBg.position   = e.position;
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
        if (_rawDrag.magnitude > _radius)          // 원 밖 → 대시
            OnDash?.Invoke(_rawDrag.normalized);

        _input = Vector2.zero;
        joystickBg.gameObject.SetActive(false);
    }
}