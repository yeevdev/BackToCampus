using UnityEngine;
using UnityEngine.EventSystems;

public class TouchToShowJoystick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform joystickRoot; // 조이스틱 컨테이너
    [SerializeField] private Canvas canvas;             // UI Canvas

    private RectTransform touchAreaRect;

    void Awake()
    {
        touchAreaRect = GetComponent<RectTransform>();
        // 시작 시 조이스틱은 숨겨두기
        joystickRoot.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        Vector2 localPoint;
        // TouchArea RectTransform을 기준으로 스크린 좌표 → 로컬 좌표 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchAreaRect,
            data.position,
            canvas.worldCamera,
            out localPoint
        );

        // 변환된 로컬 좌표를 그대로 JoystickRoot의 anchoredPosition으로 사용
        joystickRoot.anchoredPosition = localPoint;
        joystickRoot.gameObject.SetActive(true);

        // 조이스틱 내부 OnPointerDown/OnDrag 로직 트리거
        ExecuteEvents.Execute(
            joystickRoot.gameObject,
            data,
            ExecuteEvents.pointerDownHandler
        );
    }

    public void OnPointerUp(PointerEventData data)
    {
        // 조이스틱 내부 OnPointerUp 로직 트리거
        ExecuteEvents.Execute(
            joystickRoot.gameObject,
            data,
            ExecuteEvents.pointerUpHandler
        );

        // 터치 해제 시 조이스틱 숨기기
        joystickRoot.gameObject.SetActive(false);
    }
}