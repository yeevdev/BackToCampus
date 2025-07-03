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
        newPosition.y = Mathf.Clamp(newPosition.y, downBound, upBound);
        rb.MovePosition(newPosition);

        // ▼▼▼ 변경/추가된 부분 ▼▼▼
        // 맵 스크롤 로직 수정
        if (mapMaterial != null && rb.position.y >= upBound && moveInput.y > 0 && mapWorldHeight > 0)
        {
            // 1. 플레이어가 이번 프레임에 이동하려던 '월드 거리'를 계산합니다.
            float worldScrollDistance = moveInput.y * speedCoef * Time.fixedDeltaTime;

            // 2. '월드 거리'를 'UV 오프셋' 값으로 변환합니다. (이동 거리 / 맵 전체 높이)
            float uvScrollAmount = worldScrollDistance / mapWorldHeight;

            // 3. 변환된 값을 현재 오프셋에 더해줍니다.
            Vector2 currentOffset = mapMaterial.mainTextureOffset;
            currentOffset.y += uvScrollAmount;
            mapMaterial.mainTextureOffset = currentOffset;
        }
    }
}