using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    // --- 인스펙터 설정 변수 ---
    [Header("행동 타입")]
    public BehaviorType behavior;
    public enum BehaviorType { Fixed, Chaser }

    [Header("맵 스크롤 속도")]
    public float mapScrollSpeed = 3f; // 플레이어의 전진(맵 스크롤) 속도와 맞춰야 합니다.

    [Header("추적자(Chaser) 설정")]
    public float moveSpeed = 2f;         // 이동 속도
    public float detectionRadius = 5f;   // 플레이어 감지 반경
    public float randomMoveInterval = 2f; // 랜덤 이동 주기 (초)

    [Header("참조")]
    public GameObject questionMarkBubble; // 물음표 말풍선 오브젝트

    [Header("충돌 이벤트")]
    public UnityEvent onPlayerCollision;  // Inspector에서 이벤트 연결 가능

    // --- 내부 변수 ---
    private Animator anim;
    private Transform player;
    private Vector2 moveDirection = Vector2.zero;
    private bool isVisible = false;
    private float randomMoveTimer;
    private bool isPaused = false;
    private bool isChaserLogicActivated = false; // Chaser 로직 활성화 스위치

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

    void Update()
    {
        if (!isVisible || isPaused)
        {
            SetMoveDirection(Vector2.zero);
            UpdateAnimator();
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
        float currentSpeed = (behavior == BehaviorType.Chaser) ? moveSpeed : mapScrollSpeed;
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime);

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
        gameObject.SetActive(false); // 오브젝트 풀로 반환
    }

    private void RunFixedLogic()
    {
        // 고정형 NPC는 항상 화면 아래 방향으로 이동하도록 설정합니다.
        SetMoveDirection(Vector2.down);
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
                randomMoveTimer -= Time.deltaTime;
                if (randomMoveTimer <= 0)
                {
                    if (Random.value < 0.8f)
                    {
                        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
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
        else // 아직 활성화되지 않았다면 고정형처럼 아래로 내려가기만 함
        {
            SetMoveDirection(Vector2.down);
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