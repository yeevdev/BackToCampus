using UnityEngine;

public class NPC0Moveable : NPC0
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private float maxInterpolationTime; // 부드럽게 움직이는 시간 
    [SerializeField] private LayerMask forwardBoxes; // NPC1Moveable의 forward Collider(NPC 앞 쪽 4칸)가 있는 레이어
    [SerializeField] private LayerMask selfBoxes; // 각 NPC 자체 Collider들이 있는 레이어
    [SerializeField] private LayerMask boundaryBoxes; // 화면 양 옆 Collider들이 있는 레이어
    [SerializeField] private string playerTagName = "Player";
    [SerializeField] private float minDistance; // 플레이어 감지 후 플레이어에게 다가올 때 충분히 가까워서 정지할 거리
    private bool isMoving; // isMoving: 이동하는 전체 과정 중 true
    private bool isInterpolating; // isInterpolating: 한 칸 이동하는 동안 true
    private bool isChasing; // isChasing: 플레이어가 감지된 동안 true
    private float time; // 타이머
    private float interpolationTime; // 보간(부드럽게 이동)하는 동안 타이머
    private Vector3 displacement; // 변위; NPC가 이동하려는 거리 * 방향  
    private Transform playerTransform; // 플레이어의 transform; 플레이어 감지 및 쫓아가기 위해 필요  

    protected override void InitNPCMoveable()
    {
        // 오브젝트 풀에서 가져올 때 마다, Init 후에 실행. 
        playerTransform = GameObject.FindGameObjectWithTag(playerTagName).transform;   
        InitMovement();
    }

    private void OnBecameVisible()
    {
        // 화면에 보일 때
        isMoving = true;
    }

    public void InitMovement()
    {
        isMoving = false;
        isChasing = false;
        time = maxTime;
        InitInterpolation();
    }

    public void InitInterpolation()
    {
        isInterpolating = false;
        interpolationTime = 0;
    }

    protected override void FixedUpdate()
    {
        // NPC Moveable도 스크롤에 맞춰 내려가도록
        base.FixedUpdate();
        
        if (isMoving)
        {
            if (isInterpolating)
            {
                // 보간; 부드럽게 움직이기 위한 코드
                if (interpolationTime < maxInterpolationTime)
                {
                    interpolationTime += Time.fixedDeltaTime;
                    transform.position += Time.fixedDeltaTime / maxInterpolationTime * displacement;
                }
                else
                {
                    InitInterpolation();
                }
            }
            else
            {
                time -= Time.fixedDeltaTime;

                if (time <= 0)
                {
                    if (isChasing)
                    {
                        // 플레이어를 향하는 벡터
                        displacement = stepSize * (playerTransform.position - transform.position).normalized;

                        // 플레이어가 충분히 가깝다면 크기가 0인 벡터(멈춤)
                        if (Vector3.Distance(playerTransform.position, transform.position) < minDistance)
                        {
                            displacement = Vector3.zero;
                        }
                    }
                    else
                    {
                        // 360도 전방향 랜덤 벡터
                        displacement = stepSize * Random.insideUnitCircle.normalized;
                    }

                    // 이동 판정 시 사용되는 변수들
                    Vector2 currentPos = transform.position;
                    Vector2 forwardPos = transform.position + displacement;
                    Vector2 colliderSize = boxCollider.size * transform.localScale;
                    ContactFilter2D selfFilter = new();
                    selfFilter.SetLayerMask(selfBoxes);
                    Collider2D[] result = new Collider2D[2]; // 다음 줄 OverlapBox(이동한 이후의 Collider)와 충돌하는 Collider가 저장될 배열
                    int howManyCollisions = Physics2D.OverlapBox(forwardPos, colliderSize, 0f, selfFilter, result); // 충돌한 Collider 개수 반환

                    // 이동이 가능한지 판정
                    // 이동 시 어느 NPC와도 충돌하지 않거나 자기 자신과만 충돌할 때
                    if (howManyCollisions == 0 || (howManyCollisions == 1 && result[0] == boxCollider))
                    {
                        // 이동 시 화면 양 옆과 충돌하지 않을 때
                        if (Physics2D.OverlapBox(forwardPos, colliderSize, 0f, boundaryBoxes) == null)
                        {
                            // 현재 NPC가 NPC1Moveable의 forward Collider에 겹쳐있거나,
                            if (Physics2D.OverlapBox(currentPos, colliderSize, 0f, forwardBoxes) != null
                                // 이동 시 NPC1Moveable의 forward Collider가 충돌하지 않을 때
                                || Physics2D.OverlapBox(forwardPos, colliderSize, 0f, forwardBoxes) == null)
                            {
                                isInterpolating = true; // 이 NPC 이동 시작
                            }
                        }
                    }

                    // 시간 초기화
                    time = maxTime;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == playerTransform)
        {
            isChasing = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == playerTransform)
        {
            isChasing = false;
        }
    }

    private void OnDrawGizmos()
    {
        // 스크립트가 활성화되어 있고 BoxCollider2D가 할당되어 있을 때만 그림
        if (boxCollider == null)
        {
            return;
        }

        Gizmos.color = Color.yellow; // 검사하는 박스 색상

        // 현재 위치에서 검사하는 박스
        Vector2 currentColliderCenter = (Vector2)transform.position + boxCollider.offset;
        Gizmos.DrawWireCube(currentColliderCenter, boxCollider.size * transform.localScale);

        // 이동할 목적지에서 검사하는 박스
        if (displacement != Vector3.zero)  // displacement가 계산된 후에만 그리기
        {
            Gizmos.color = Color.red; // 목적지 박스 색상
            Vector2 nextColliderCenter = (Vector2)transform.position + (Vector2)displacement + boxCollider.offset;
            Gizmos.DrawWireCube(nextColliderCenter, boxCollider.size * transform.localScale);
        }

        Gizmos.color = Color.green; // 플레이어 감지 반원 색상

        // 플레이어가 가까이 있는지 검사하는 빈원
        Gizmos.DrawWireSphere(transform.position, 0.5f * transform.localScale.x);
    }
}