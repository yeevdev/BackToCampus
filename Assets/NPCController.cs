using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    // --- 인스펙터 설정 변수 ---
    [Header("행동 타입")]
    public BehaviorType behavior;
    public enum BehaviorType { Fixed, Chaser }

    [Header("오브젝트 풀")]
    public PoolType poolType; // 알맞은 풀에 반환하기 위한 태그
    public enum PoolType { Male, Female }

    [Header("추적자(Chaser) 설정")]
    public float moveSpeed = 2f;         // 이동 속도
    public float detectionRadius = 5f;   // 플레이어 감지 반경
    public float randomMoveInterval = 2f; // 랜덤 이동 주기 (초)

    [Header("참조")]
    public GameObject questionMarkBubble; // 물음표 말풍선 오브젝트

    [Header("충돌 이벤트")]
    public UnityEvent onPlayerCollision;  // Inspector에서 이벤트 연결 가능

    [Header("생성된 열")]
    public float ColumnSpawnedIn; // 무슨 열에서 생성되었는지 저장

    [Header("회피 이벤트")]
    public float dashRadius = 2f;

    // --- 내부 변수 ---
    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.zero;
    private bool isVisible = false;
    private float randomMoveTimer;
    private bool isPaused = false;
    private bool isChaserLogicActivated = false; // Chaser 로직 활성화 스위치
    private string PoolTag => poolType.ToString();
    private SpriteRenderer spriteRenderer;
    private bool didDashAlreadySucceed = false;
    public int scoreWhenDashSucceed = 10;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 오브젝트 풀에서 재사용될 때마다 상태를 초기화합니다.
        isVisible = false;
        isPaused = false;
        isChaserLogicActivated = false; // 스위치 초기화
        moveDirection = Vector2.zero;
        randomMoveTimer = randomMoveInterval;
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

        // 1. 플레이어가 충분히 가까우면 어두워지지 않도록 Sorting Layer를 조절합니다.
        if (!GameManager.isPlayerDashing) // 대시 중에는 어두워지는 NPC가 뱐걍되지 않도록
        {
            DimmingLayer.SetDimmingRenderer(spriteRenderer, !isPlayerCloseEnough); // isPlayerCloseEnough가 true이면 '밝게' 유지합니다.
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
            SetMoveDirection(Vector2.zero); // 이동 방향 초기화
            UpdateAnimator(); // 애니메이션을 멈춤 상태로 업데이트

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
            case BehaviorType.Chaser:
                RunChaserLogic();
                break;
        }

        // 5. 계산된 이동 방향과 속도에 따라 실제로 위치를 이동시킵니다.
        float currentSpeed = (behavior == BehaviorType.Chaser) ? moveSpeed : 0;
        Vector3 displacement = (moveDirection * currentSpeed + Vector2.down * GameManager.currentScrollSpeed) * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + displacement);

        // 6. 이동 상태에 맞춰 애니메이터를 업데이트합니다.
        UpdateAnimator();
    }

    // 플레이어가 대시 성공!
    private void PlayerDashSuceeded()
    {
        // 점수 주기
        GameManager.score += scoreWhenDashSucceed;
        // NPC에 물음표 띄우기
        ShowQuestionMark();
    }

    void OnBecameVisible()
    {
        isVisible = true;
        // Chaser 타입일 경우에만, 화면에 보이는 순간 로직을 활성화시킵니다.
        if (behavior == BehaviorType.Chaser)
        {
            isChaserLogicActivated = true;
        }
    }

    void OnBecameInvisible()
    {
        isVisible = false;
        NPCSpawner.Instance.RemoveSpawnRestriction(ColumnSpawnedIn);
        ObjectPooler.Instance.ReturnToPool(gameObject, PoolTag);
    }

    private void RunFixedLogic()
    {
        // 고정형은 움직이지 않음
        // SetMoveDirection(Vector2.down);
    }

    private void RunChaserLogic()
    {
        // Chaser 로직이 활성화된 경우에만 추적/배회 로직 실행
        if (isChaserLogicActivated)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool isPlayerInSight = (distanceToPlayer < detectionRadius) && (player.position.y < transform.position.y);
            
            if (isPlayerInSight)
            {
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                SetMoveDirection(directionToPlayer);
            }
            else
            {
                randomMoveTimer -= Time.fixedDeltaTime;
                if (randomMoveTimer <= 0)
                {
                    if (Random.value < 0.8f)
                    {
                        Vector2 randomDirection = Random.insideUnitCircle.normalized;
                        SetMoveDirection(randomDirection);
                    }
                    else
                    {
                        SetMoveDirection(Vector2.zero);
                    }
                    randomMoveTimer = randomMoveInterval;
                }
            }
        }
        else // 추적/배회가 아닐 때는 움직이지 않음
        {
            // SetMoveDirection(Vector2.down);
        }
    }

    private void UpdateAnimator()
    {
        bool isMoving = moveDirection != Vector2.zero;
        anim.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            anim.SetFloat("moveX", moveDirection.x);
            anim.SetFloat("moveY", moveDirection.y);
        }
    }

    private void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
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