using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speedCoef, upBound, downBound, horizontalBound;
    // 속도와 플레이어가 움직일 수 있는 경계를 직접 설정해주세요
    [SerializeField] private Joystick joystick;  // Inspector에서 조이스틱을 직접 설정해주세요
    [SerializeField] private GameObject gameMap; // Inspector에서 맵을 직접 설정해주세요 
    private Transform _transform, _gameMapTransform;

    private void Start()
    {
        // caching
        _transform = transform;
        _gameMapTransform = gameMap.transform;
    }

    private void Update()
    {
        Vector3 velocity = joystick.GetInputVector() * speedCoef; // 조이스틱 조작에 따른 플레이어 속도

        _transform.position += velocity;

        _transform.position = new Vector3( // 플레이어 이동 범위 제한
            Mathf.Clamp(_transform.position.x, -horizontalBound, horizontalBound),
            Mathf.Clamp(_transform.position.y, downBound, upBound)
        );

        if (_transform.position.y >= upBound && velocity.y > 0) // 플레이어가 upBound보다 위에 있고 위를 향하고 있다면
            _gameMapTransform.position -= Vector3.up * velocity.y; // 맵을 아래로 이동
    }
}
