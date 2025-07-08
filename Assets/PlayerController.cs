using UnityEngine;

// 필요한 컴포넌트들을 명시하여 실수를 방지합니다.
[RequireComponent(typeof(Animator), typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("연결 필수")]
    [Tooltip("씬에 있는 조이스틱 컨트롤러 오브젝트를 연결해주세요.")]
    [SerializeField] private JoystickController joystick;

    [Header("플레이어 능력치")]
    [Tooltip("플레이어의 이동 속도를 조절합니다.")]
    [SerializeField] private float speed = 3f;

    // 자주 사용하는 컴포넌트는 미리 변수에 담아두어 성능을 향상시킵니다.
    private Rigidbody2D rb;
    private Animator animator;

    // 이동 관련 변수
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;

    // 스크립트가 시작될 때 한 번 호출됩니다.
    void Start()
    {
        // 필요한 컴포넌트들을 자동으로 찾아와 변수에 할당합니다.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 물리 효과로 인한 회전이나 중력 영향을 받지 않도록 설정합니다.
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    // 매 프레임마다 호출됩니다. (주로 입력 처리, 시각적 업데이트에 사용)
    void Update()
    {
        // 1. 조이스틱으로부터 입력 값을 받아옵니다.
        // JoystickController가 이미 정규화된(-1~1) 값을 제공하므로, 추가적인 .normalized 처리는 필요 없습니다.
        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        // 2. 입력 값에 따라 애니메이션을 제어합니다.
        HandleAnimation();
    }

    // 고정된 시간 간격으로 호출됩니다. (물리 기반 이동 처리에 사용)
    void FixedUpdate()
    {
        // 3. 물리 엔진을 통해 캐릭터를 부드럽게 이동시킵니다.
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// 이동 입력 값에 따라 애니메이터의 파라미터를 설정하는 함수
    /// </summary>
    private void HandleAnimation()
    {
        // 조이스틱을 움직이고 있을 때 (벡터의 길이가 0.1보다 클 때)
        if (moveInput.magnitude > 0.1f)
        {
            // 'isMoving' 파라미터를 true로 바꿔 Movement 블렌드 트리로 전환시킵니다.
            animator.SetBool("isMoving", true);

            // 'moveX', 'moveY' 파라미터에 현재 방향을 전달하여 올바른 걷기 애니메이션을 재생합니다.
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);

            // 멈췄을 때 마지막 방향을 기억하기 위해 현재 방향을 저장합니다.
            lastMoveDirection = moveInput;
        }
        // 멈췄을 때
        else
        {
            // 'isMoving' 파라미터를 false로 바꿔 Idle 블렌드 트리로 전환시킵니다.
            animator.SetBool("isMoving", false);

            // 마지막으로 바라보던 방향을 계속 전달하여 올바른 정지 자세를 유지합니다.
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }
    }
}