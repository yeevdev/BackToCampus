using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// 오브젝트 풀에서 오브젝트를 가져오거나 생성합니다.
    /// </summary>
    /// <param name="tag">풀의 식별 태그</param>
    /// <param name="position">생성할 위치</param>
    /// <param name="rotation">생성할 회전값</param>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn;
        
        // 풀이 비어있으면 새로운 오브젝트 생성
        if (poolDictionary[tag].Count == 0)
        {
            Pool poolSettings = pools.Find(p => p.tag == tag);
            objectToSpawn = Instantiate(poolSettings.prefab);
        }
        else
        {
            // 풀에서 비활성화된 오브젝트 재사용
            objectToSpawn = poolDictionary[tag].Dequeue();
        }
        
        // 오브젝트 활성화 및 위치/회전 설정
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        return objectToSpawn;
    }

    /// <summary>
    /// 사용이 끝난 오브젝트를 풀로 반환하거나 파괴합니다.
    /// </summary>
    /// <param name="obj">반환할 게임오브젝트</param>
    /// <param name="tag">해당 오브젝트의 풀 태그</param>
    public void ReturnToPool(GameObject obj, string tag)
    {
        // 해당 태그의 풀이 존재하지 않으면 오브젝트 파괴
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            Destroy(obj);
            return;
        }

        // 풀의 크기 제한 확인
        Pool poolSettings = pools.Find(p => p.tag == tag);
        if (poolDictionary[tag].Count >= poolSettings.size)
        {
            Debug.Log($"Pool '{tag}' is full. Destroying object instead of returning to pool.");
            Destroy(obj);  // 풀이 가득 찼으면 오브젝트 파괴
            return;
        }

        // 오브젝트를 비활성화하고 풀에 반환
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}