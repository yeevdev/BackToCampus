using System.Collections.Generic;
using UnityEngine;
public class NPCGeneratingField : MonoBehaviour
{
    [SerializeField] private ObjectPoolingManager poolingManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Sprite> skins;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G)) SpawnNPCs(4);
    }

    private void SpawnNPCs(int numberOfNPCs)
    {
        // 4종류의 NPC Prefab 중 랜덤으로 선택
        List<int> types = new();
        for (int i = 0; i < numberOfNPCs; i++)
        {
            types.Add(Random.Range(0, 4));
        }

        Vector2[] coords = GetRandomCoords(5, 3, numberOfNPCs);

        for (int i = 0; i < numberOfNPCs; i++)
        {
            int type = types[i];
            NPC newNPC = poolingManager.pools[type].Get().GetComponent<NPC>();
            if (type % 2 == 0) // NPC0과 NPC0Moveable
            {
                newNPC.Init(coords[i], skins[Random.Range(0, skins.Count)]);
            }
            else if (type % 2 == 1) // NPC1과 NPC1Moveable
            {
                newNPC.Init(coords[i], skins[Random.Range(0, skins.Count)], skins[Random.Range(0, skins.Count)]);
            }
        }
    }
    
    private Vector2[] GetRandomCoords(int rows, int columns, int numberOfNPCs)
    {
        float cellWidth = Screen.width / columns;
        float cellHeight = Screen.height / rows;

        List<int> cells = new();
        Vector2[] positions = new Vector2[numberOfNPCs];

        for (int i = 0; i < rows * columns; i++) cells.Add(i);

        for (int i = 0; i < numberOfNPCs; i++)
        {
            int chosenIndex = Random.Range(0, rows * columns - i);
            int cellNum = cells[chosenIndex];

            Vector3 position = new(
                cellWidth * (cellNum % columns) + Random.Range(0f, cellWidth),
                Screen.height + cellHeight * (cellNum / columns) + Random.Range(0f, cellHeight),
                0f
                );

            positions[i] = mainCamera.ScreenToWorldPoint(position);

            cells.RemoveAt(chosenIndex);
        }

        return positions;
    }
}