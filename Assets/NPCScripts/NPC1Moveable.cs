using UnityEngine;
public class NPC1Moveable : NPC1
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private int maxInterpolationTime; // 서브스텝 개수; 부드럽게 움직이기 위해 한 스텝의 길이를 몇 개로 쪼갤 것인지
    [SerializeField] private bool isMoving, isInterpolating;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    [SerializeField] private float time, interpolationTime;
    private BoxCollider2D boxCollider;
    [SerializeField] private Vector3 displacement;
    [SerializeField] private LayerMask forwardBoxes;
    [SerializeField] private LayerMask selfBoxes;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void InitNPCMoveable()
    {   // 오브젝트 풀에서 가져올 때 마다 실행해야 함. 그런데, Init 후에 실행되어야 함. 
        InitMovement();
    }

    private void OnBecameVisible()
    {   // 화면에 보일 때
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
                    // NPC 자체 충돌 박스와 앞 4칸 박스를 이동
                    InitInterpolation();
                }
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    displacement = stepSize * Vector3.down;
                    
                    // 현재 NPC1Moveable의 forward충돌박스 속에 있는데,
                    if (Physics2D.OverlapBox(transform.position, boxCollider.size, 0f, forwardBoxes) != null)
                    {// 이동한 후의 충돌 박스가 self충돌박스와 겹치지 않을 때
                        if (Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f, selfBoxes) == null)
                        {
                            isInterpolating = true;
                        }
                    }
                    // 현재 forward충돌박스 밖에 있으며, 이동한 후의 충돌 박스가 어떤 충돌박스와도 겹치지 않을 때
                    else if (Physics2D.OverlapBox(transform.position + displacement, boxCollider.size, 0f) == null)
                    {
                        isInterpolating = true;
                    }

                    // 시간 초기화
                    time = maxTime;
                }
            }
        }
    }
}