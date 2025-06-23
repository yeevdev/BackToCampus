using UnityEngine;
public abstract class NPC : MonoBehaviour
{
    public void Init(Vector2 initPos, Transform gameMap)
    {   // 위치 초기화
        transform.position = initPos;
        // NPC들이 gameMap과 같이 움직이도록 ;
        transform.SetParent(gameMap);
    }
    public void Init(Vector2 initPos, Transform gameMap, Sprite skin)
    {   // NPC0을 위한 초기화 메서드
        Init(initPos, gameMap);
        SetSkins(skin);
    }
    public void Init(Vector2 initPos, Transform gameMap, Sprite skin1, Sprite skin2)
    {   // NPC1을 위한 초기화 메서드
        Init(initPos, gameMap);
        SetSkins(skin1, skin2);
    }

    protected abstract void SetSkins(Sprite skin);
    protected abstract void SetSkins(Sprite skin1, Sprite skin2);
}