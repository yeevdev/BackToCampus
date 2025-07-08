using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 화면의 어느 곳이든 터치하면 해당 위치에 나타나고, 드래그하여 조작하는 플로팅 조이스틱 스크립트.
/// </summary>
public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // [SerializeField]를 사용하면 private 변수여도 Inspector 창에서 값을 할당할 수 있습니다.
    [SerializeField] private RectTransform joystickBackground; // 조이스틱의 배경 (컨테이너 역할)
    [SerializeField] private RectTransform joystickHandle;   // 조이스틱의 핸들 (실제 움직이는 부분)

    // 조이스틱의 입력 방향과 크기를 저장하는 변수 (플레이어 이동에 사용)
    private Vector2 inputVector;

    // 외부 스크립트에서 조이스틱의 입력 값을 쉽게 가져갈 수 있도록 프로퍼티를 제공합니다.
    // Horizontal은 x축 (좌우) 입력, Vertical은 y축 (상하) 입력을 나타냅니다.
    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;

    // 스크립트가 처음 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // 게임 시작 시에는 조이스틱이 보이지 않도록 비활성화합니다.
        joystickBackground.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 터치된 위치에 조이스틱 배경을 이동시킵니다.
        joystickBackground.position = eventData.position;
        // 조이스틱을 화면에 보이도록 활성화합니다.
        joystickBackground.gameObject.SetActive(true);
        // OnDrag에서 핸들의 위치를 계산하기 위해 OnPointerDown이 호출된 순간의 핸들 위치를 저장합니다.
        // 이 스크립트에서는 배경의 위치와 핸들의 시작 위치를 동일하게 설정합니다.
        joystickHandle.position = eventData.position;
    }

    /// <summary>
    /// 사용자가 화면을 터치한 상태로 드래그할 때 계속해서 호출됩니다.
    /// </summary>
    /// <param name="eventData">드래그 이벤트에 대한 정보</param>
    public void OnDrag(PointerEventData eventData)
    {
        // 조이스틱 배경의 반지름을 계산합니다. (배경 크기의 절반)
        float radius = joystickBackground.sizeDelta.x / 2;

        // 터치된 위치에서 조이스틱 배경의 중심까지의 벡터를 계산합니다.
        Vector2 dragVec = eventData.position - (Vector2)joystickBackground.position;

        // 벡터의 크기가 반지름보다 작도록 제한합니다.
        // 이렇게 하면 핸들이 조이스틱 배경 밖으로 벗어나지 않습니다.
        Vector2 clampedVec = Vector2.ClampMagnitude(dragVec, radius);

        // 제한된 벡터를 기반으로 핸들의 위치를 업데이트합니다.
        joystickHandle.position = (Vector2)joystickBackground.position + clampedVec;

        // 플레이어 이동에 사용할 정규화된 입력 벡터를 계산합니다.
        // 정규화(normalized)는 벡터의 크기를 1로 만들어 방향만을 나타나게 합니다.
        inputVector = clampedVec / radius;
    }

    /// <summary>
    /// 사용자가 화면에서 손을 뗐을 때 호출됩니다.
    /// </summary>
    /// <param name="eventData">터치 종료 이벤트에 대한 정보</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // 조이스틱을 다시 보이지 않도록 비활성화합니다.
        joystickBackground.gameObject.SetActive(false);

        // 입력 벡터를 0으로 초기화하여 플레이어가 멈추도록 합니다.
        inputVector = Vector2.zero;
    }
}