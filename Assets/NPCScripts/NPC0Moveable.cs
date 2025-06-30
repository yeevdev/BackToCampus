using Unity.Collections;
using UnityEngine;
public class NPC0Moveable : NPC0
{
    [SerializeField] private float stepSize; // 한 스텝의 길이
    [SerializeField] private float maxTime; // 움직이는데 걸리는 시간
    [SerializeField] private float maxInterpolationTime; // 부드럽게 움직이는 시간 
    private bool isMoving, isInterpolating;
    // isMoving: 이동하는 전체 과정 중 true
    // isInterpolating: 한 칸 이동하는 동안 true
    private float time, interpolationTime;
    private Vector3[] direction = {
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.left,
    };
    private Vector3 displacement;


    private void OnEnable()
    {   // 풀에서 가져오거나 생성되었을 때 초기화
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
                    Vector3 chosenDirection = direction[Random.Range(0, 4)]
                    chosenDirection 
                    displacement = stepSize * c;
                    time = maxTime;
                    isInterpolating = true;
                }
            }
        }
    }
    void OnBecameVisible()
    {
        isMoving = true;
    }
}