using UnityEngine;
public abstract class NPC : MonoBehaviour
{
    [SerializeField] private Transform gameMap;
    protected Transform _transform;
    private void Awake()
    {
        // transform 캐싱
        _transform = transform;
        // NPC들이 gameMap과 같이 움직이도록 
        _transform.SetParent(gameMap);
    }
    public void Init(Vector2 initPos)
    {   // 위치 초기화
        transform.position = initPos;
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
}