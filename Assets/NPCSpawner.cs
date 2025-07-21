using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;

    [Header("스폰 설정")]
    public Transform gridStartPosition;
    public Transform background;
    public int columns = 3;
    public int rows = 5;
    public float waveInterval = 60f;
    public int spawnCountPerWave = 4;

    private float zoneWidth = 10f;
    private float zoneHeight = 5f;
    private List<Vector2> calculatedSpawnZones;
    private string[] genderTags = { "Male", "Female" };
    private string[] groupTags = { "MaleMale", "MaleFemale", "FemaleMale", "FemaleFemale" };

    void Awake()
    {
        // 스폰 존 면적 자동 계산 
        zoneHeight = background.localScale.y / rows;
        zoneWidth = background.localScale.x / columns;

        // 스폰 존 위치 자동 계산
        calculatedSpawnZones = new List<Vector2>();
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                float x = gridStartPosition.position.x + (c * zoneWidth);
                float y = gridStartPosition.position.y + (r * zoneHeight);
                calculatedSpawnZones.Add(new Vector2(x, y));
            }
        }
    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        InvokeRepeating(nameof(SpawnWave), 3f, waveInterval);
    }

    void SpawnWave()
    {
        // 1. 스폰할 구역 무작위 선택
        List<Vector2> selectedZones = new List<Vector2>();
        List<int> zoneIndices = new List<int>();
        for (int i = 0; i < calculatedSpawnZones.Count; i++) zoneIndices.Add(i);

        for (int i = 0; i < spawnCountPerWave; i++)
        {
            if (zoneIndices.Count == 0) break;
            int randIndex = Random.Range(0, zoneIndices.Count);
            selectedZones.Add(calculatedSpawnZones[randIndex]);
            zoneIndices.RemoveAt(randIndex);
        }

        // 2. 선택된 각 위치에 NPC 스폰 및 행동 설정
        foreach (Vector2 zoneCenter in selectedZones)
        {
            string npcTag;
            bool isSingleSpawning = Random.value > 0.5f;

            if (isSingleSpawning)
            {
                // 성별 랜덤 결정
                npcTag = genderTags[Random.Range(0, genderTags.Length)];
            }
            else
            {
                // 성별 조합 랜덤 결정
                npcTag = groupTags[Random.Range(0, groupTags.Length)];
            }

            // 위치 랜덤 설정
            Vector2 spawnPos = zoneCenter + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            
            // 풀에서 NPC 스폰
            GameObject npc = objectPooler.SpawnFromPool(npcTag, spawnPos, Quaternion.identity);
            // 행동 타입 랜덤 결정 후 코드로 설정
            if (isSingleSpawning)
            {
                if (Random.value > 0.5f)
                {
                    npc.GetComponent<NPCController>().behavior = NPCController.BehaviorType.Fixed;
                }
                else
                {
                    npc.GetComponent<NPCController>().behavior = NPCController.BehaviorType.Chaser;
                }
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    npc.GetComponent<GroupNPCController>().behavior = GroupNPCController.BehaviorType.Fixed;
                }
                else
                {
                    npc.GetComponent<GroupNPCController>().behavior = GroupNPCController.BehaviorType.Moving;
                }
            }
        }
    }
}