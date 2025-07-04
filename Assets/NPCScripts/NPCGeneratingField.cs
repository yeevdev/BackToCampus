using System.Collections.Generic;
using System.Linq;
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
        Vector2[] coords = GetRandomCoords(5, 3, 2, 2, numberOfNPCs);

        // 4종류의 NPC Prefab 중 랜덤으로 선택
        List<int> types = new();
        for (int i = 0; i < numberOfNPCs; i++)
        {
             
            types.Add(Random.Range(0, 4));
        }

        for (int i = 0; i < numberOfNPCs; i++)
        {
            
            int type = Random.Range(0, 4);

            // if (/*same column exists*/) {
            //     if (/*type is 0*/) {
            //         /*the other cell in the same column should be type 0, 1, or 2 */
            //         // type 3 is forbidden
            //     }
            //     else if (/*type is 1*/) {
            //         /*the other cell in the same column should be type 0, 1, 2 or 3*/
            //         // nothing is forbidden
            //     }
            //     else if (/*type is 2*/) {
            //         /*the other cell in the same column should be type 0, 1, or 2 */
            //         // type 3 is forbidden
            //     }
            //     else if (/*type is 3*/) {
            //         /*the other cell in the same column should only be type 1 or 3*/
            //         // type 0 or 2 is forbidden
            //     }
            // }

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
    
    private Vector2[] GetRandomCoords(int rows, int columns, int subrows, int subcolumns, int numberOfNPCs)
    {
        float cellWidth = Screen.width / columns;
        float cellHeight = Screen.height / rows;

        List<int> cells = new();
        Vector2[] positions = new Vector2[numberOfNPCs];

        for (int i = 0; i < rows * columns; i++) cells.Add(i);

        for (int i = 0; i < numberOfNPCs; i++)
        {
            // 선택되지 않은 큰 구역 중 한 곳을 정함
            int chosenIndex = Random.Range(0, cells.Count);
            int chosenCell = cells[chosenIndex];

            // 선택된 구역을 더 잘게 나누는 subcell을 정함
            int chosenSubcell = Random.Range(0, subrows * subcolumns - 1);
            // 한 subcell의 너비와 높이
            float subcellWidth = cellWidth / subcolumns;
            float subcellHeight = cellHeight / subrows;

            // NPC가 생성될 좌표
            Vector2 position = new(
                cellWidth * (chosenCell % columns) + subcellWidth * (chosenSubcell % subcolumns + .5f),
                Screen.height + cellHeight * (chosenCell / columns) + subcellHeight * (chosenSubcell / subcolumns + .5f)
            );

            positions[i] = mainCamera.ScreenToWorldPoint(position);

            // 선택된 큰 구역의 숫자를 제거
            cells.RemoveAt(chosenIndex);
        }

        var sorted = from coord in positions orderby coord.x select coord;
        positions = sorted.ToArray();
        return positions;
    }
}