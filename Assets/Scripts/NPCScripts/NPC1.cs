using UnityEngine;

public class NPC1 : NPC
{
    [SerializeField] private SpriteRenderer spriteRenderer1;
    [SerializeField] private SpriteRenderer spriteRenderer2;
    // NPC를 구성하는 두 캐릭터에 스킨을 적용하기 위한 SpriteRenderer 둘 (inspector에서 설정해야 함)
    
    protected override void SetSkins(Sprite skin)
    {   // NPC1에는 정확히 두 스킨을 설정해야 하므로 오류
        Debug.LogAssertion("You must set exactly two skins for NPC1.");
    }

    protected override void SetSkins(Sprite skin1, Sprite skin2)
    {    // NPC를 구성하는 두 캐릭터의 스킨을 설정
        spriteRenderer1.sprite = skin1;
        spriteRenderer2.sprite = skin2;
    }

    protected override void OnBecameInvisible()
    {
        if (this is not NPC1Moveable)
            poolingManager.Pools[1].Release(gameObject);
        else
            poolingManager.Pools[3].Release(gameObject);
    }
}
