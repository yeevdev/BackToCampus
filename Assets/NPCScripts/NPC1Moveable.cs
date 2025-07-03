using UnityEngine;
public class NPC1Moveable : NPC1
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxStep; // 최대 스텝 수
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private int maxInterpolationTime; // 서브스텝 개수; 부드럽게 움직이기 위해 한 스텝의 길이를 몇 개로 쪼갤 것인지
    [SerializeField] private bool isMoving, isInterpolating;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    [SerializeField] private float time, interpolationTime;
    private Vector3 displacement;

    private CollisionBox collisionBoxForward = new(-0.5f, 0.5f, -3.5f, 0.5f);

      private void OnEnable()
    {   // 풀에서 가져오거나 생성되었을 때 초기화
        if (initalized)
        {
            collisionBoxForward.SetBoxPosition(transform.position);
            collisionManager.Add(collisionBoxForward);
            InitMovement();
        }
    }

    private void OnDisable()
    {   // 풀에 다시 넣을 때
        collisionManager.Remove(collisionBoxForward);
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
            if (isInterpolating)
            {
                if (interpolationTime < maxInterpolationTime)
                {
                    interpolationTime += Time.deltaTime;
                    transform.position += Time.deltaTime / maxInterpolationTime * displacement;
                }
                else InitInterpolation();
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    displacement = stepSize * Vector3.down;

                    // 이동한 후의 충돌 박스를 설정
                    collisionBoxForward.ChangeBoxPosition(displacement);

                    // 이동한 후의 충돌 박스가 다른 충돌 박스들과 충돌하는지 않으면 움직이기
                    if (!collisionManager.CheckCollision(collisionBoxForward))
                        isInterpolating = true;
  
                    // 시간 초기화
                    time = maxTime;
                }
                else InitMovement();
            }
        }
    }

    void OnBecameVisible()
    {
        isMoving = true;
    }
}