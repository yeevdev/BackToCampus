using UnityEngine;

public class NPC0 : NPC
{
    private SpriteRenderer spriteRenderer;
    // NPC에 스킨을 적용하기 위한 SpriteRenderer (자동 설정)

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void SetSkins(Sprite skin)
    {   // NPC의 스킨 설정
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = skin;
    }
    
    protected override void SetSkins(Sprite skin1, Sprite skin2)
    {   // NPC0에는 정확히 한 스킨을 설정해야 하므로 오류
        Debug.LogAssertion("You must set exactly one skin for NPC0.");
    }

    protected override void OnBecameInvisible()
    {
        if (this is not NPC0Moveable)
            poolingManager.Pools[0].Release(gameObject);
        else
            poolingManager.Pools[2].Release(gameObject);
    }
}