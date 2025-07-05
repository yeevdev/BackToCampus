using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
public class ObjectPoolingManager : MonoBehaviour
{
    public Dictionary<int, ObjectPool<GameObject>> pools;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform gameMap;
    [SerializeField] private CollisionManager collisionManager;


    // NPC type
    // 0: NPC0
    // 1: NPC1
    // 2: NPC0Moveable
    // 3: NPC1Moveable
    
    private void Awake()
    {
        pools = new();
        for (int i = 0; i < 4; i++)
        {
            int tempIndex = i;
            pools[i] = new ObjectPool<GameObject>(
                    () =>
                    {
                        GameObject npc = Instantiate(prefabs[tempIndex]);
                        npc.GetComponent<NPC>().InitInstantiated(gameMap, this, collisionManager);
                        return npc;
                    },
                    /* tempIndex로 i값을 복사 후 전달해야 이 delegate에
                    reference가 아니라 copy로 전달됨 */
                    (npc) => npc.SetActive(true),
                    (npc) => npc.SetActive(false),
                    (npc) => Destroy(npc),
                    true,
                    2,
                    4
                );
        }
    }
}