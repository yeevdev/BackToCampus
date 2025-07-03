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
    // 맵 텍스처가 스크롤될 속도
    [SerializeField] private float mapScrollSpeed = 0.1f;

    [Header("연결할 오브젝트")]
    [SerializeField] private JoystickController joystick;

    private Rigidbody2D rb;
    private Material mapMaterial; // 맵의 머티리얼을 저장할 변수
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // mapRenderer에서 머티리얼 인스턴스를 가져옵니다.
        // .material을 사용해야 원본 에셋이 아닌 개별 인스턴스가 수정됩니다.
        if (mapRenderer != null)
        {
            mapMaterial = mapRenderer.material;
        }
    }

    void Update()
    {
        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
    }

    void FixedUpdate()
    {
        // 플레이어 이동 및 위치 제한
        Vector2 newPosition = rb.position + moveInput * speedCoef * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -horizontalBound, horizontalBound);
        newPosition.y = Mathf.Clamp(newPosition.y, downBound, upBound);
        rb.MovePosition(newPosition);

        // 🎨 플레이어가 위쪽 경계에 닿았고, 계속 위로 가려고 할 때 맵 오프셋을 조절합니다.
        if (mapMaterial != null && rb.position.y >= upBound && moveInput.y > 0)
        {
            // 현재 오프셋 값을 가져와서 y값만 변경합니다.
            Vector2 currentOffset = mapMaterial.mainTextureOffset;
            float scrollAmount = moveInput.y * mapScrollSpeed * Time.fixedDeltaTime;
            currentOffset.y += scrollAmount;

            // 변경된 오프셋 값을 다시 머티리얼에 적용합니다.
            mapMaterial.mainTextureOffset = currentOffset;
        }
    }
}