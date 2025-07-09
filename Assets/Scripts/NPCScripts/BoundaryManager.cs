using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    [SerializeField] private GameObject leftBoundary;
    [SerializeField] private GameObject rightBoundary;
    [SerializeField] private float boundaryThickness = 0.5f; // 벽 두께
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; // 메인 카메라 참조 가져오기

        if (mainCamera == null)
        {
            Debug.LogError("'MainCamera'로 태그된 카메라를 찾을 수 없습니다.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        SetupBoundaries();
    }

    private void SetupBoundaries()
    {
        // 카메라의 시야 범위 (월드 좌표) 계산
        Vector2 screenMinWorld = mainCamera.ScreenToWorldPoint(Vector2.zero); // 화면 좌하단
        Vector2 screenMaxWorld = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)); // 화면 우상단

        // 화면 중앙 계산
        float screenCenterY = (screenMinWorld.y + screenMaxWorld.y) / 2f;

        // 화면 전체 높이
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

    // 에디터에서 Scene 뷰에서 Collider가 잘 보이도록 Gizmo 그리기
    private void OnDrawGizmos()
    {
        if (leftBoundary != null) DrawBoundaryGizmo(leftBoundary);
        if (rightBoundary != null) DrawBoundaryGizmo(rightBoundary);
    }

    private void DrawBoundaryGizmo(GameObject boundaryObj)
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