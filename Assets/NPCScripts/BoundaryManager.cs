using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    // 경계가 될 오브젝트 (Left, Right, Top, Bottom 등)
    // 인스펙터에서 BoxCollider2D를 가진 빈 GameObject들을 드래그하여 할당합니다.
    [SerializeField] private GameObject leftBoundary;
    [SerializeField] private GameObject rightBoundary;
    [SerializeField] private float boundaryThickness = 0.5f; // 경계 콜라이더의 두께 (월드 유닛)
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main; // 메인 카메라 참조 가져오기

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Please ensure your camera is tagged as 'MainCamera'.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        SetupBoundaries();
    }

    void SetupBoundaries()
    {
        // 카메라의 시야 범위 (월드 좌표) 계산
        Vector2 screenMinWorld = mainCamera.ScreenToWorldPoint(Vector2.zero); // 화면 좌하단
        Vector2 screenMaxWorld = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)); // 화면 우상단

        // 화면 중앙 계산
        float screenCenterX = (screenMinWorld.x + screenMaxWorld.x) / 2f;
        float screenCenterY = (screenMinWorld.y + screenMaxWorld.y) / 2f;

        // 화면 전체 폭과 높이
        float screenWidthWorld = screenMaxWorld.x - screenMinWorld.x;
        float screenHeightWorld = screenMaxWorld.y - screenMinWorld.y;

        // 좌측 경계 설정
        if (leftBoundary != null)
        {
            leftBoundary.transform.position = new Vector3(screenMinWorld.x - boundaryThickness / 2f, screenCenterY);
            BoxCollider2D collider = leftBoundary.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = leftBoundary.AddComponent<BoxCollider2D>();
            }
            collider.size = new Vector2(boundaryThickness, screenHeightWorld);
        }

        // 우측 경계 설정
        if (rightBoundary != null)
        {
            rightBoundary.transform.position = new Vector3(screenMaxWorld.x + boundaryThickness / 2f, screenCenterY);
            BoxCollider2D collider = rightBoundary.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = rightBoundary.AddComponent<BoxCollider2D>();
            }
            collider.size = new Vector2(boundaryThickness, screenHeightWorld);
        }
    }

    // 화면 크기가 변경될 때 (예: 해상도 변경, 창 크기 변경) 경계를 다시 설정
    // Editor에서 테스트할 때 유용하며, 빌드된 게임에서는 주로 Awake/Start에서 한 번만 호출됨.
    // 만약 런타임에 해상도 변경을 지원해야 한다면 Update나 Coroutine으로 체크하여 호출
    void OnEnable()
    {
        // 해상도 변경 감지를 위한 이벤트 구독 (Unity 2021.2 이상)
        // Application.onResolutionChange += OnResolutionChange;
    }

    void OnDisable()
    {
        // Application.onResolutionChange -= OnResolutionChange;
    }

    // private void OnResolutionChange(Vector2 newResolution)
    // {
    //     SetupBoundaries();
    // }

    // 에디터에서 Scene 뷰에서 Collider가 잘 보이도록 Gizmo 그리기
    void OnDrawGizmos()
    {
        if (leftBoundary != null) DrawBoundaryGizmo(leftBoundary);
        if (rightBoundary != null) DrawBoundaryGizmo(rightBoundary);
    }

    void DrawBoundaryGizmo(GameObject boundaryObj)
    {
        BoxCollider2D collider = boundaryObj.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.cyan;
            // 월드 좌표에서의 충돌체 크기와 중심을 정확히 반영
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}