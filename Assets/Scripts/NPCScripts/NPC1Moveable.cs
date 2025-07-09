using UnityEngine;

public class NPC1Moveable : NPC1
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private float maxInterpolationTime; // 부드럽게 움직이는 시간
    [SerializeField] private LayerMask forwardBoxes; // NPC1Moveable의 forward Collider(NPC 앞 쪽 4칸)가 있는 레이어
    [SerializeField] private LayerMask selfBoxes; // 각 NPC 자체 Collider들이 있는 레이어
    private bool isMoving; // isMoving: 이동하는 전체 과정 중 true
    private bool isInterpolating; // isInterpolating: 한 칸 이동하는 동안 true
    private float time; // 타이머
    private float interpolationTime; // 보간(부드럽게 이동)하는 동안 타이머
    private Vector3 displacement; // 변위; NPC가 이동하려는 거리 * 방향

    protected override void InitNPCMoveable()
    {
        // 오브젝트 풀에서 가져올 때 마다, Init 후에 실행. 
        InitMovement();
    }   

    private void OnBecameVisible()
    {
        // 화면에 보일 때
        isMoving = true;
    }

    private void InitMovement()
    {
        isMoving = false;
        time = maxTime;
        InitInterpolation();
    }

    private void InitInterpolation()
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
            if (isInterpolating) // 보간; 부드럽게 움직이기 위한 코드
            {
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
                    // 아래를 향하는 벡터
                    displacement = stepSize * Vector3.down;

                    // 이동 판정 시 사용되는 변수들
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
                        // 현재 NPC가 NPC1Moveable의 forward Collider에 겹쳐있거나,
                        if (Physics2D.OverlapBox(transform.position, colliderSize, 0f, forwardBoxes) != null
                            // 이동 시 NPC1Moveable의 forward Collider가 충돌하지 않을 때
                            || Physics2D.OverlapBox(forwardPos, colliderSize, 0f, forwardBoxes) == null)
                        {
                            isInterpolating = true; // 이 NPC 이동 시작
                        }
                    }

                    // 시간 초기화
                    time = maxTime;
                }
            }
        }
    }

    private void OnDrawGizmos()
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
    }
}