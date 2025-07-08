using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float speedCoef = 5f;

    [Header("이동 범위 제한")]
    [SerializeField] private float upBound = 4f;
    [SerializeField] private float downBound = -4f;
    [SerializeField] private float horizontalBound = 8f;

    [Header("맵 스크롤 설정")]
    // 맵의 Sprite Renderer 또는 Mesh Renderer를 연결해주세요.
    [SerializeField] private Renderer mapRenderer;
    // 맵 스크롤 속도 변수는 삭제합니다. speedCoef를 기준으로 계산됩니다.

    [Header("연결할 오브젝트")]
    [SerializeField] private JoystickController joystick;

    private Rigidbody2D rb;
    private Material mapMaterial;
    private Vector2 moveInput;

    // ▼▼▼ 변경/추가된 부분 ▼▼▼
    private float mapWorldHeight; // 맵의 실제 월드 높이를 저장할 변수

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (mapRenderer != null)
        {
            mapMaterial = mapRenderer.material;
            // ▼▼▼ 변경/추가된 부분 ▼▼▼
            // 맵의 실제 월드 유닛 높이를 한번만 계산해서 저장해둡니다.
            mapWorldHeight = mapRenderer.bounds.size.y;
        }
    }

    void Update()
    {
        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * speedCoef * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -horizontalBound, horizontalBound);
        // Y축 이동은 스크롤 로직과 연관되므로 Clamp를 아래에서 처리합니다.

        // 맵 스크롤 조건 확인
        bool isScrolling = rb.position.y >= upBound && moveInput.y > 0;

        if (mapMaterial != null && isScrolling && mapWorldHeight > 0)
        {
            // ▼▼▼ 핵심 추가 부분 1 ▼▼▼
            // 현재 스크롤 속도를 GameManager에 기록합니다.
            GameManager.currentScrollSpeed = moveInput.y * speedCoef;

            // 플레이어는 더 이상 위로 올라가지 않도록 위치를 고정합니다.
            newPosition.y = upBound;

            // --- 기존 맵 스크롤링 코드 ---
            float worldScrollDistance = GameManager.currentScrollSpeed * Time.fixedDeltaTime;
            float uvScrollAmount = worldScrollDistance / mapWorldHeight;
            Vector2 currentOffset = mapMaterial.mainTextureOffset;
            currentOffset.y += uvScrollAmount;
            mapMaterial.mainTextureOffset = currentOffset;
        }
        else
        {
            // ▼▼▼ 핵심 추가 부분 2 ▼▼▼
            // 스크롤 조건이 아닐 때는 속도를 0으로 설정하여 멈춥니다.
            GameManager.currentScrollSpeed = 0f;
            
            // 플레이어는 아래쪽 경계까지만 자유롭게 움직입니다.
            newPosition.y = Mathf.Clamp(newPosition.y, downBound, upBound);
        }

        rb.MovePosition(newPosition);
    }
}