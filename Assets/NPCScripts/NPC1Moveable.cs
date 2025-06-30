using UnityEngine;
public class NPC1Moveable : NPC1
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxStep; // 최대 스텝 수
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private int maxInterpolationTime; // 서브스텝 개수; 부드럽게 움직이기 위해 한 스텝의 길이를 몇 개로 쪼갤 것인지
    private bool isMoving, isInterpolating;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    private float time, interpolationTime;
    private int stepCount = 0; // 현재 스텝 수
    private Vector3 displacement;


      private void OnEnable()
    {   // 풀에서 가져오거나 생성되었을 때 초기화
        stepCount = 0;
        InitMovement();
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
                    if (stepCount < maxStep)
                    {
                        displacement = stepSize * Vector3.down;
                        time = maxTime;
                        isInterpolating = true;
                        stepCount++;
                    }
                    else InitMovement();
                }
            }
        }
    }

    void OnBecameVisible()
    {
        isMoving = true;
    }
}