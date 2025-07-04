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
    [SerializeField] private Vector2 boxPos;
    [SerializeField] private Vector3 displacement;

    private CollisionBox collisionBoxForward = new(-0.4f, 0.4f, -0.4f, 0.4f);
    // NPC1Moveable이 다른 NPC들의 충돌 박스와 충돌하는지 판단할 때 사용하는 충돌 박스
    private CollisionBox collisionBoxForwardUpTo4Steps = new(-0.4f, 0.4f, -3.6f, -0.4f);
    // 다른 NPC들이 NPC1Moveable의 충돌 박스와 충돌하는지 판단할 때 사용하는 충돌 박스
    // 다른 NPC들이 NPC1Moveable 아래로 4칸까지 이동할 수 없게 함 

    protected override void InitNPCMoveable()
    {   // 오브젝트 풀에서 가져올 때 마다 실행해야 함. 그런데, Init 후에 실행되어야 함. 
        collisionBoxForward.SetBoxPosition(transform.position);
        collisionManager.Add(collisionBoxForward);
        collisionBoxForwardUpTo4Steps.SetBoxPosition(transform.position);
        collisionManager.Add(collisionBoxForwardUpTo4Steps);
        InitMovement();
    }

    private void OnDisable()
    {   // 풀에 다시 넣을 때
        collisionManager.Remove(collisionBoxForward);
        collisionManager.Remove(collisionBoxForwardUpTo4Steps);
        collisionManager.Remove(collisionBox);
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
                    collisionBox.ChangeBoxPosition(displacement);
                    collisionBoxForwardUpTo4Steps.ChangeBoxPosition(displacement);
                    InitInterpolation();
                }
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    displacement = stepSize * Vector3.down;

                    // 이동한 후의 충돌 박스를 설정
                    collisionBoxForward.SetBoxPosition(collisionBox.GetBoxPosition() + (Vector2)displacement);

                    // 이동한 후의 충돌 박스가 다른 충돌 박스들과 충돌하는지 않으면 움직이기
                    if (!collisionManager.CheckCollision(collisionBoxForward, collisionBoxForwardUpTo4Steps))
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