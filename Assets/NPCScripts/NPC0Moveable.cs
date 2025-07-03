using UnityEngine;
using UnityEngine.PlayerLoop;
public class NPC0Moveable : NPC0
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private float maxInterpolationTime; // 부드럽게 움직이는 시간 
    [SerializeField] private bool isMoving, isInterpolating;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    [SerializeField] private float time, interpolationTime;
    private Vector3[] direction = {
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.left,
    };
    private Vector3 displacement;

    private CollisionBox collisionBoxForward = new(-0.5f, 0.5f, -0.5f, 0.5f);

    private void OnEnable()
    {   // 풀에서 가져올 때 초기화
        if (initalized)
        {
        collisionBoxForward.SetBoxPosition(transform.position);
        // collisionManager.Add(collisionBoxForward);
        InitMovement();
        }
    }

    private void OnDisable()
    {   // 풀에 다시 넣을 때
        // collisionManager.Remove(collisionBoxForward);
    }

    public void InitMovement()
    {
        isMoving = false;
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
                    collisionBox.ChangeBoxPosition(displacement); // NPC 자체 충돌 박스를 이동
                    InitInterpolation();
                }
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    displacement = stepSize * direction[Random.Range(0, 4)];

                    // 이동한 후의 충돌 박스를 설정
                    collisionBoxForward.ChangeBoxPosition(displacement);

                    // 이동한 후의 충돌 박스가 다른 충돌 박스들과 충돌하는지 않으면 움직이기
                    // if (!collisionManager.CheckCollision(collisionBoxForward))
                        isInterpolating = true;
                    
                    // 시간 초기화
                    time = maxTime;
                }
            }
        }
    }
    void OnBecameVisible()
    {
        isMoving = true;
    }
}