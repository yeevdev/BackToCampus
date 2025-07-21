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

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
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
        if (!isVisible || isPaused)
        {
            SetMoveDirection(Vector2.zero);
            UpdateAnimator();
            if (!isVisible) // 보이지 않을 경우에는 맵 스크롤에 의해 내려가야 함
            {
                Vector3 downScroll = GameManager.currentScrollSpeed * Time.fixedDeltaTime * Vector2.down;
                rb.MovePosition(transform.position + downScroll);
            }
            return;
        }


        // 1. 행동 로직 실행
        switch (behavior)
        {
            case BehaviorType.Fixed:
                RunFixedLogic();
                break;
            case BehaviorType.Chaser:
                RunChaserLogic();
                break;
        }

        // 2. 실제 이동 처리
        float currentSpeed = (behavior == BehaviorType.Chaser) ? moveSpeed : 0;
        Vector3 positionChange = (moveDirection * currentSpeed + Vector2.down * GameManager.currentScrollSpeed) * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + positionChange);

        // 3. 애니메이터 업데이트
        UpdateAnimator();
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
                        Vector2 randomDirection = Random.insideUnitSphere.normalized;
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
        if (other.CompareTag("Player"))
        {
            // 물음표 표시
            ShowQuestionMark();

            // 이벤트 발생
            onPlayerCollision?.Invoke();
        }
    }
}