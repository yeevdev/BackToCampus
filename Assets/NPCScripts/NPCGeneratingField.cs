using System.Collections.Generic;
using UnityEngine;
public class NPCGeneratingField : MonoBehaviour
{
    [SerializeField] private ObjectPoolingManager poolingManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Sprite> skins;
    [SerializeField] private float NPC0Freq; // NPC0이 생성될 확률
    [SerializeField] private float NPC1Freq; // NPC1이 생성될 확률
    [SerializeField] private float NPC0MoveableFreq; // NPC0Moveable이 생성될 확률
    [SerializeField] private float NPC1MoveableInColumnFreq; // 각 열마다 NPC1Moveable이 생성될 확률
    [SerializeField] private float NPC0MoveableFreq2; // 각 열마다 NPC1Moveable이 생성될 확률
    [SerializeField] private float NPC1MoveableFreq2; // 각 열마다 NPC1Moveable이 생성될 확률
    private float allFreq;
    private float allFreq2;

    // NPC type
    // 0: NPC0
    // 1: NPC1
    // 2: NPC0Moveable
    // 3: NPC1Moveable

    private void Awake()
    {
        if (NPC0Freq < 0 || NPC1Freq < 0 || NPC0MoveableFreq < 0
        || NPC1MoveableInColumnFreq < 0 || NPC0MoveableFreq2 < 0 || NPC1MoveableFreq2 < 0)
        {
            Debug.LogAssertion("어떤 Freq 변수도 음수가 될 수 없습니다.");
        }
        else if (NPC1MoveableInColumnFreq > 1)
        {
            Debug.LogAssertion("NPC1MoveableInColumnFreq는 0 이상 1 이하의 수입니다.");
        }

        allFreq = NPC0Freq + NPC1Freq + NPC0MoveableFreq;
        allFreq2 = NPC0MoveableFreq2 + NPC1MoveableFreq2;

        if (allFreq == 0)
        {
            Debug.LogAssertion("NPC0Freq, NPC1Freq, NPC0MoveableFreq의 합은 0일 수 없습니다.");
        }
        if (allFreq2 == 0)
        {
            Debug.LogAssertion("NPC0MoveableFreq2와 NPC1MoveableFreq2의 합은 0일 수 없습니다.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) SpawnNPCs(4);
    }

    private void SpawnNPCs(int numberOfNPCs)
    {
        // 열별로 정리된 모든 좌표를 GetRandomCoords()에서 먼저 가져옴
        // 각 열은 일정 확률로 수직이동형 NPC 생성
        // 수직이동형 NPC가 생성되는 열에서는 수직이동형 NPC와 랜덤추적형 NPC만 생성
        // 수직이동형 NPC가 생성되지 않는 열에서는 나머지 NPC로 채워 넣음
        List<Vector2>[] coords = GetRandomCoords(5, 3, 2, 2, numberOfNPCs);

        // 큰 구역의 열을 이터레이팅
        for (int i = 0; i < coords.Length; i++)
        {
            if (coords[i].Count == 0)
            {
                continue;
            }

            // 랜덤 결과 NPC1Moveable을 생성하기로 결정되었다면
            if (Random.Range(0f, 1f) <= NPC1MoveableInColumnFreq)
            {
                // 선택된 열의 첫번째 좌표는 NPC1Moveable로 생성
                poolingManager.pools[3].Get().GetComponent<NPC>()
                .Init(coords[i][0], skins[Random.Range(0, skins.Count)], skins[Random.Range(0, skins.Count)]);

                // 선택된 열의 다른 좌표는 NPC0Moveable 또는 NPC1Moveable 중 하나로 랜덤으로 생성
                for (int j = 1; j < coords[i].Count; j++)
                {
                    // type2(NPC0Moveable)와 type3(NPC1Moveable) 중 랜덤으로 결정
                    float random = Random.Range(0f, 1f);
                    int type;
                    if (random < NPC0MoveableFreq2 / allFreq2)
                    {
                        type = 2;
                    }
                    else
                    {
                        type = 3;
                    }

                    // NPC 생성 및 초기화
                    NPC npc = poolingManager.pools[type].Get().GetComponent<NPC>();
                    if (type == 2)
                    {
                        npc.Init(coords[i][j], skins[Random.Range(0, skins.Count)]);
                    }
                    else if (type == 3)
                    {
                        npc.Init(coords[i][j], skins[Random.Range(0, skins.Count)], skins[Random.Range(0, skins.Count)]);
                    }
                }
            }
            else // 랜덤 결과 NPC1Moveable을 생성하지 않기로 결정되었다면
            {
                for (int j = 0; j < coords[i].Count; j++)
                {
                    // type0(NPC0), type1(NPC1), type2(NPC0Moveable) 중 랜덤으로 결정
                    float random = Random.Range(0f, 1f);
                    int type;
                    if (random < NPC0Freq / allFreq)
                    {
                        type = 0;
                    }
                    else if (random < (NPC0Freq + NPC1Freq) / allFreq)
                    {
                        type = 1;
                    }
                    else
                    {
                        type = 2;
                    }

                    // NPC 생성 및 초기화
                    NPC npc = poolingManager.pools[type].Get().GetComponent<NPC>();
                    if (type == 0 || type == 2)
                    {
                        npc.Init(coords[i][j], skins[Random.Range(0, skins.Count)]);
                    }
                    else if (type == 1)
                    {
                        npc.Init(coords[i][j], skins[Random.Range(0, skins.Count)], skins[Random.Range(0, skins.Count)]);
                    }
                }
            }
        }
    }
    
    private List<Vector2>[] GetRandomCoords(int rows, int columns, int subrows, int subcolumns, int numberOfNPCs)
    {
        List<Vector2>[] result = new List<Vector2>[columns];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new();
        }
        // NPC 생성 좌표를 큰 구역의 각 열마다 분리해 저장
        // 열이 3개이고 생성하는 NPC가 4명일 때 result의 임의의 구조 예시)
        // [
        //  [(,),(,),(,)],  // 첫번째 열
        //  [],             // 두번째 열
        //  [(,)]           // 세번째 열
        // ]

        // 한 cell의 너비와 높이
        float cellWidth = Screen.width / columns;
        float cellHeight = Screen.height / rows;
        // 한 subcell의 너비와 높이
        float subcellWidth = cellWidth / subcolumns;
        float subcellHeight = cellHeight / subrows;

        List<int> cells = new();
        for (int i = 0; i < rows * columns; i++)
        {
            cells.Add(i);
        }

        for (int i = 0; i < numberOfNPCs; i++)
        {
            // 선택되지 않은 큰 구역 중 한 곳을 정함
            int chosenIndex = Random.Range(0, cells.Count);
            int chosenCell = cells[chosenIndex];
            cells.RemoveAt(chosenIndex);

            // 정해진 구역이 어느 열인지 계산
            int chosenColumn = chosenCell % columns;

            // 선택된 구역을 더 잘게 나누는 subcell을 정함
            int chosenSubcell = Random.Range(0, subrows * subcolumns);

            // NPC가 생성될 좌표
            Vector2 position = new(
                cellWidth * (chosenCell % columns) + subcellWidth * (chosenSubcell % subcolumns + .5f),
                Screen.height + cellHeight * (chosenCell / columns) + subcellHeight * (chosenSubcell / subcolumns + .5f)
            );

            // 좌표 저장
            result[chosenColumn].Add(mainCamera.ScreenToWorldPoint(position));
        }
        return result;
    }
}