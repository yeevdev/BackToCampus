using UnityEngine;
public class NPC0Moveable : NPC0
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private float maxInterpolationTime; // 부드럽게 움직이는 시간 
    [SerializeField] private bool isMoving, isInterpolating, isChasing;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    [SerializeField] private float time, interpolationTime;
    private Vector3 displacement;
    private Transform playerTransform;
    [SerializeField] private LayerMask forwardBoxes;
    [SerializeField] private LayerMask selfBoxes;
    [SerializeField] private LayerMask boundaryBoxes;
    [SerializeField] private CircleCollider2D detector;
    [SerializeField] private float minDistance;

    protected override void InitNPCMoveable()
    {   // 오브젝트 풀에서 가져올 때 마다 실행해야 함. 그런데, Init 후에 실행되어야 함.
        InitMovement();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;   
    }

    void OnBecameVisible()
    {   // 화면에 보일 때
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

    private void Update()
    {
        if (isMoving)
        {
            if (isInterpolating) // 보간; 부드럽게 움직이기 위한 코드
            {
                if (interpolationTime < maxInterpolationTime)
                {
                    interpolationTime += Time.deltaTime;
                    transform.position += Time.deltaTime / maxInterpolationTime * displacement;
                }
                else
                {
                    InitInterpolation();
                }
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    if (isChasing)
                    {
                        displacement = stepSize * (playerTransform.position - transform.position).normalized;
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

                    // NPC?Moveable은 어떤 충돌박스 내에 이미 있으면 밖으로 탈출하도록 움직일 수 있어야 하므로 Forward충돌박스와는 충돌하지 않고,
                    // NPC?Moveable이 어떠한 충돌박스에도 겹치지 않으면 Forward충돌박스와 충돌하여야 한다.
                    // NPC0Moveable은 모든 충돌박스와 충돌하되,
                    // Forward충돌박스 안에 있을 때에는 Forward충돌박스와 충돌하지 않는다.
                    // Forward충돌박스 안에 있지 않을 때에는 Forward충돌박스와 충돌한다.

                    // 이동한 후의 충돌 박스가 다른 충돌 박스들과 충돌하는지 않으면 움직이기

                    // 현재 NPC1Moveable의 forward충돌박스 속에 있는데,
                    if (Physics2D.OverlapBox(transform.position, boxCollider.size, 0f, forwardBoxes) != null)
                    {// 이동한 후의 충돌 박스가 self충돌박스 또는 boundary충돌박스와 겹치지 않을 때
                        if (Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, selfBoxes) == null
                        && Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, boundaryBoxes) == null)
                        {
                            isInterpolating = true;
                        }
                    }
                    // 현재 forward충돌박스 밖에 있으며, 이동한 후의 충돌 박스가 playerBox를 제외한 어떤 충돌박스와도 겹치지 않을 때
                    else if (Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, selfBoxes) == null
                    && Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, forwardBoxes) == null
                    && Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, boundaryBoxes) == null)
                    {
                        isInterpolating = true;
                    }
                    // 시간 초기화
                    time = maxTime;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == playerTransform)
        {
            isChasing = true;
        } 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == playerTransform)
        {
            isChasing = false;
        }
    }
    void OnDrawGizmos()
    {
        // 스크립트가 활성화되어 있고 BoxCollider2D가 할당되어 있을 때만 그림
        if (boxCollider == null) return;

        Gizmos.color = Color.yellow; // 검사하는 박스 색상

        // 현재 위치에서 검사하는 박스
        Vector2 currentColliderCenter = (Vector2)transform.position + boxCollider.offset;
        Gizmos.DrawWireCube(currentColliderCenter, boxCollider.size * transform.localScale);

        // 이동할 목적지에서 검사하는 박스
        if (displacement != Vector3.zero) // displacement가 계산된 후에만 그리기
        {
            Gizmos.color = Color.red; // 목적지 박스 색상
            Vector2 nextColliderCenter = (Vector2)transform.position + (Vector2)displacement + boxCollider.offset;
            Gizmos.DrawWireCube(nextColliderCenter, boxCollider.size * transform.localScale);
        }

        Gizmos.color = Color.green; // 플레이어 감지 원 색상

        // 플레이어가 가까이 있는지 검사하는 원
        Gizmos.DrawWireSphere(transform.position, detector.radius * transform.localScale.x);

    }
}