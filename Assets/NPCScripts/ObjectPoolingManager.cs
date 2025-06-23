using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
public class ObjectPoolingManager : MonoBehaviour
{
    public Dictionary<int, ObjectPool<GameObject>> pools;
    [SerializeField] private GameObject[] prefabs;

    private void Awake()
    {
        pools = new()
        {
            {
                0,
                new ObjectPool<GameObject>(
            () => Instantiate(prefabs[0])
        )
            },
            {
                1,
                new ObjectPool<GameObject>(
            () => Instantiate(prefabs[1])
        )
            },
            {
                2,
                new ObjectPool<GameObject>(
            () => Instantiate(prefabs[2])
        )
            },
            {
                3,
                new ObjectPool<GameObject>(
            () => Instantiate(prefabs[3])
        )
            }
        };
    }
}