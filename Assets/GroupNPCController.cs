using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GroupNPCController : MonoBehaviour
{
    [Header("행동 타입")]
    public BehaviorType behavior;
    public enum BehaviorType { Fixed, Moving }

    [Header("오브젝트 풀")]
    public PoolType poolType;  // 알맞은 풀에 반환하기 위한 태그
    public enum PoolType { MaleMale, MaleFemale, FemaleMale, FemaleFemale }

    [Header("이동 설정")]
    public float moveSpeed = 3f;

    [Header("참조")]
    public GameObject questionMarkBubble; // 물음표 말풍선 오브젝트

    [Header("충돌 이벤트")]
    public UnityEvent onPlayerCollision;  // Inspector에서 이벤트 연결 가능

    [Header("생성된 열")]
    public float ColumnSpawnedIn; // 무슨 열에서 생성되었는지 저장

    [Header("회피 이벤트")]
    public float dashRadius = 2f;
    public int scoreWhenDashSucceed = 10;

    // --- 내부 변수 ---
    private Animator[] animators;
     private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.zero;
    private bool isVisible = false;
    private bool isPaused = false;
    private string PoolTag => poolType.ToString();
    private SpriteRenderer[] spriteRenderers;
    private bool didDashAlreadySucceed = false;


    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 오브젝트 풀에서 재사용될 때마다 상태를 초기화합니다.
        isVisible = false;
        isPaused = false;
        moveDirection = Vector2.zero;
        if (questionMarkBubble != null)
        {
            questionMarkBubble.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        // --- 프레임마다 항상 실행되어야 하는 로직 ---

        // 0. 플레이어와의 거리를 계산하여 대시 성공 판정에 사용합니다.
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isPlayerCloseEnough = distanceToPlayer < dashRadius;

        // 1. 플레이어가 충분히 가까우면 어두워지지 않도록 각 스프라이트의 Sorting Layer를 조절합니다.
        if (!GameManager.isPlayerDashing) // 대시 중에는 어두워지는 NPC가 변경되지 않도록
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                DimmingLayer.SetDimmingRenderer(spriteRenderer, !isPlayerCloseEnough); // isPlayerCloseEnough가 true이면 '밝게' 유지합니다.
            }
        }

        // 2. 플레이어가 가까이서 대시했을 때 점수를 추가하고 물음표를 표시합니다. (최초 한 번만)
        if (!didDashAlreadySucceed && isPlayerCloseEnough && GameManager.isPlayerDashing)
        {
            didDashAlreadySucceed = true; // 중복 점수 획득 방지
            PlayerDashSuceeded();
        }

        // --- NPC의 상태에 따라 분기되는 로직 ---

        // 3. 화면 밖이거나, 일시정지, 또는 플레이어가 대시 중일 때는 NPC의 행동을 멈춥니다.
        if (!isVisible || isPaused || GameManager.isPlayerDashing)
        {
            moveDirection = Vector2.zero; // 이동 방향 초기화
            UpdateAnimators(); // 애니메이션을 멈춤 상태로 업데이트

            // 게임이 일시정지 된 것이 아니라면 (즉, 플레이어 대시 또는 화면 밖) 맵 스크롤에 의해 아래로 내려갑니다.
            if (!isPaused) 
            {
                Vector3 downScroll = GameManager.currentScrollSpeed * Time.fixedDeltaTime * Vector2.down;
                rb.MovePosition(transform.position + downScroll);
            }
            return; // 아래의 행동 로직을 실행하지 않고 메서드를 종료합니다.
        }

        // --- NPC가 정상적으로 행동하는 로직 ---

        // 4. 행동 타입에 따라 로직을 실행합니다.
        switch (behavior)
        {
            case BehaviorType.Fixed:
                RunFixedLogic();
                break;
            case BehaviorType.Moving:
                RunMovingLogic();
                break;
        }

        // 5. 계산된 이동 방향과 속도에 따라 실제로 위치를 이동시킵니다.
        float currentSpeed = (behavior == BehaviorType.Moving) ? moveSpeed : 0;
        Vector3 displacement = (moveDirection * currentSpeed + Vector2.down * GameManager.currentScrollSpeed) * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + displacement);

        // 6. 이동 상태에 맞춰 애니메이터를 업데이트합니다.
        UpdateAnimators();
    }

    // 플레이어가 대시 성공!
    private void PlayerDashSuceeded()
    {
        // 점수 주기
        GameManager.score += scoreWhenDashSucceed;
        // NPC에 물음표 띄우기
        ShowQuestionMark();
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    void OnBecameInvisible()
    {
        isVisible = false;
        NPCSpawner.Instance.RemoveSpawnRestriction(ColumnSpawnedIn);
        ObjectPooler.Instance.ReturnToPool(gameObject, PoolTag);
    }

    private void RunFixedLogic()
    {
        // 고정
    }

    private void RunMovingLogic()
    {
        // 이동 방향 설정 (아래쪽)
        moveDirection = Vector2.down;
    }

    private void UpdateAnimators()
    {
        bool isMoving = moveDirection != Vector2.zero;

        foreach (Animator anim in animators)
        {
            anim.SetBool("isWalking", isMoving);
            if (isMoving)
            {
                anim.SetFloat("moveX", moveDirection.x);
                anim.SetFloat("moveY", moveDirection.y);
            }
        }
    }


    public void ShowQuestionMark()
    {
        if (questionMarkBubble != null)
        {
            StartCoroutine(ShowBubbleCoroutine());
        }
    }

    private IEnumerator ShowBubbleCoroutine()
    {
        questionMarkBubble.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        questionMarkBubble.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.isPlayerDashing && other.CompareTag("Player"))
        {
            // 이벤트 발생
            onPlayerCollision?.Invoke();
        }
    }
}

