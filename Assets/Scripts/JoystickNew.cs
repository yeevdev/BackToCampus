using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickNew : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 0.5f;
    [SerializeField] private Canvas canvas;
    private Vector2 inputVector;

    public Vector2 GetInputVector() => inputVector;

    public void OnPointerDown(PointerEventData data)
        => OnDrag(data);

    public void OnDrag(PointerEventData data)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, data.position, canvas.worldCamera, out localPoint))
        {
            Vector2 clamped = Vector2.ClampMagnitude(
                localPoint / (background.sizeDelta.x * 0.5f), handleRange);
            handle.anchoredPosition = clamped * (background.sizeDelta.x * 0.5f);
            inputVector = clamped;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }
}