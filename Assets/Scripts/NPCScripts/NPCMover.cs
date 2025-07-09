using UnityEngine;

public class NPCMover : MonoBehaviour
{
    // 맵의 스크롤 속도와 맞출 변수입니다.
    // public으로 선언해서 유니티 인스펙터 창에서 값을 쉽게 조절할 수 있습니다.
    public float scrollSpeed = 2f;

    void Update()
    {
        // 매 프레임마다 아래 방향(Vector3.down)으로 scrollSpeed 만큼 이동시킵니다.
        // Time.deltaTime을 곱해 컴퓨터 성능과 상관없이 일정한 속도를 유지합니다.
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
    }
}