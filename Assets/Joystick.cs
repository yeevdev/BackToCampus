using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float maxRadius = 1f;
    [SerializeField] private Camera cam; // Inspector에서 카메라를 직접 설정해주세요
    private Transform _transform, _handleTransform;
    private GameObject handle; // handle은 "Handle"이라는 이름의 자식 오브젝트이어야 함
    private Vector2 inputVector; // 조이스틱 입력: 크기가 0~1인 벡터

    private void Start()
    {
        _transform = transform; // caching
        handle = _transform.Find("Handle").gameObject;
        _handleTransform = handle.transform; // caching
    }

    private Vector2 GetHandleOffset(Vector2 screenCoords)
    {
        Vector2 worldCoords = cam.ScreenToWorldPoint(screenCoords, Camera.MonoOrStereoscopicEye.Mono); 
        return worldCoords - new Vector2(_transform.position.x, _transform.position.y);
    }


    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    // 주의: 이하 메서드들은 다음 조건을 만족시켜야 작동합니다.
    // 1) 조이스틱에 EventSystem이 자식으로 있고,
    // 2) 조이스틱에 Collider 2D 컴포넌트가 있고,
    // 3) 카메라에 Physics 2D Raycaster 컴포넌트가 있다.
    public void OnPointerDown(PointerEventData data)
    {
        OnDrag(data);
    }
 
    public void OnDrag(PointerEventData data)
    {
        Vector2 handleOffset = Vector2.ClampMagnitude(GetHandleOffset(data.position), maxRadius);
        _handleTransform.localPosition = handleOffset;
        inputVector = handleOffset / maxRadius; // inputVector의 크기를 0~1로 조정
    }
    
    public void OnPointerUp(PointerEventData data)
    {
        _handleTransform.localPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }
}