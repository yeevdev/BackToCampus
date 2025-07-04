using UnityEngine;
public abstract class NPC : MonoBehaviour
{
    protected ObjectPoolingManager poolingManager;
    protected CollisionManager collisionManager; // NPC 충돌 매니저
    protected CollisionBox collisionBox = new(-0.4f, 0.4f, -0.4f, 0.4f);

    // NPC 생성시 한 번만 초기화
    public virtual void InitInstantiated(Transform gameMap, ObjectPoolingManager _poolingManager, CollisionManager _collisionManager)
    {
        // NPC들이 gameMap과 같이 움직이도록
        transform.SetParent(gameMap);
        // 풀링 매니저 초기화
        poolingManager = _poolingManager;
        // 충돌 매니저 초기화
        collisionManager = _collisionManager;
    }
    public void Init(Vector2 initPos)
    {   // 위치 초기화
        transform.position = initPos;
        // 충돌 매니저에 NPC 충돌 박스 등록
        collisionManager.Add(collisionBox);
        // 충돌 박스 위치 초기화
        collisionBox.SetBoxPosition(initPos);
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
 
    private void OnDisable()
    {
        collisionManager.Remove(collisionBox);
    }
}