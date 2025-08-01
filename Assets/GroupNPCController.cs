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

    // --- 내부 변수 ---
    private Animator[] animators;
    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.zero;
    private bool isVisible = false;
    private bool isPaused = false;
    private string PoolTag => poolType.ToString();

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (!isVisible || isPaused)
        {
            moveDirection = Vector2.zero;
            UpdateAnimators();
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
            case BehaviorType.Moving:
                RunMovingLogic();
                break;
        }

        // 2. 실제 이동 처리
        float currentSpeed = (behavior == BehaviorType.Moving) ? moveSpeed : 0;
        Vector3 displacement = (moveDirection * currentSpeed + Vector2.down * GameManager.currentScrollSpeed) * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + displacement);

        // 3. 애니메이터 업데이트
        UpdateAnimators();
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
        if (other.CompareTag("Player"))
        {
            // 물음표 표시
            ShowQuestionMark();

            // 이벤트 발생
            onPlayerCollision?.Invoke();
        }
    }
}

