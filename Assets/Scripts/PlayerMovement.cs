using UnityEngine;

// 애니메이터 컴포넌트가 반드시 필요함을 명시합니다.
[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float speedCoef = 5f;

    [Header("이동 범위 제한")]
    [SerializeField] private float upBound = 4f;
    [SerializeField] private float downBound = -4f;
    [SerializeField] private float horizontalBound = 8f;

    [Header("맵 스크롤 설정")]
    [SerializeField] private Renderer mapRenderer;

    [Header("연결할 오브젝트")]
    [SerializeField] private JoystickController joystick;

    // --- 기존 변수 ---
    private Rigidbody2D rb;
    private Material mapMaterial;
    private Vector2 moveInput;
    private float mapWorldHeight;

    // ▼▼▼ 애니메이션용으로 추가된 변수 ▼▼▼
    private Animator animator;
    private Vector2 lastMoveDirection; // 마지막 이동 방향을 저장

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (mapRenderer != null)
        {
            mapMaterial = mapRenderer.material;
            mapWorldHeight = mapRenderer.bounds.size.y;
        }

        // ▼▼▼ 애니메이션용으로 추가된 부분 ▼▼▼
        // 시작할 때 애니메이터 컴포넌트를 찾아와 변수에 할당합니다.
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 조이스틱 입력을 받습니다.
        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        // ▼▼▼ 애니메이션용으로 추가된 부분 ▼▼▼
        // 매 프레임마다 입력 값에 따라 애니메이션 상태를 처리합니다.
        HandleAnimation();
    }

    // FixedUpdate의 기존 이동/스크롤 로직은 그대로 유지됩니다.
    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * speedCoef * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -horizontalBound, horizontalBound);

        bool isScrolling = rb.position.y >= upBound && moveInput.y > 0;

        if (mapMaterial != null && isScrolling && mapWorldHeight > 0)
        {
            // 이 부분은 기존 코드와 동일합니다.
            // GameManager.currentScrollSpeed = moveInput.y * speedCoef; // GameManager가 있다면 사용
            newPosition.y = upBound;
            
            float worldScrollDistance = (moveInput.y * speedCoef) * Time.fixedDeltaTime;
            float uvScrollAmount = worldScrollDistance / mapWorldHeight;
            mapMaterial.mainTextureOffset += new Vector2(0, uvScrollAmount);
        }
        else
        {
            // 이 부분은 기존 코드와 동일합니다.
            // GameManager.currentScrollSpeed = 0f; // GameManager가 있다면 사용
            newPosition.y = Mathf.Clamp(newPosition.y, downBound, upBound);
        }

        rb.MovePosition(newPosition);
    }
    
    // ▼▼▼ 애니메이션용으로 추가된 메서드 ▼▼▼
    /// <summary>
    /// 이동 입력 값에 따라 애니메이터의 파라미터를 설정하는 함수
    /// </summary>
    private void HandleAnimation()
    {
        // 조이스틱을 움직이고 있을 때
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
            lastMoveDirection = moveInput;
        }
        // 멈췄을 때
        else
        {
            animator.SetBool("isMoving", false);
            // 마지막으로 바라보던 방향을 계속 전달하여 Idle 방향을 유지합니다.
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }
    }
}