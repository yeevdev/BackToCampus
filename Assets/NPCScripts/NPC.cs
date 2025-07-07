using UnityEngine;
public abstract class NPC : MonoBehaviour
{
    protected ObjectPoolingManager poolingManager;
    protected BoxCollider2D boxCollider;

    // NPC 생성시 한 번만 초기화
    public virtual void InitInstantiated(Transform gameMap, ObjectPoolingManager _poolingManager)
    {
        // NPC들이 gameMap과 같이 움직이도록
        transform.SetParent(gameMap);
        // 풀링 매니저 초기화
        poolingManager = _poolingManager;
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void Init(Vector2 initPos)
    {   // 위치 초기화
        transform.position = initPos;
        if (this is NPC0Moveable || this is NPC1Moveable)
            InitNPCMoveable();
    }
    public void Init(Vector2 initPos, Sprite skin)
    {   // NPC0을 위한 초기화 메서드
        Init(initPos);
        SetSkins(skin);
    }
    public void Init(Vector2 initPos, Sprite skin1, Sprite skin2)
    {   // NPC1을 위한 초기화 메서드
        Init(initPos);
        SetSkins(skin1, skin2);
    }

    protected abstract void SetSkins(Sprite skin);
    protected abstract void SetSkins(Sprite skin1, Sprite skin2);
    protected abstract void OnBecameInvisible();
    protected virtual void InitNPCMoveable() { Debug.LogAssertion("InitNPCMoveable()은 Moveable NPC에서만 실행할 수 있습니다."); }
    void OnDrawGizmos()
    {
        // 스크립트가 활성화되어 있고 BoxCollider2D가 할당되어 있을 때만 그림
        if (boxCollider == null) return;

        Gizmos.color = Color.yellow; // 검사하는 박스 색상

        // 현재 위치에서 검사하는 박스
        Vector2 currentColliderCenter = (Vector2)transform.position + boxCollider.offset;
        Gizmos.DrawWireCube(currentColliderCenter, boxCollider.size * 3.5f);
    }
}