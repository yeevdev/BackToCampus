using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PausePanel.SetActive(false); // 시작 시 일시 정지 패널 비활성화
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Menu_Btn()
    {
        Time.timeScale = 0; // 게임 일시 정지
        PausePanel.SetActive(true); // 일시 정지 패널 활성화
    }
    public void Resume_Btn()
    {
        Time.timeScale = 1f;          // 게임 속도 원상복구
        PausePanel.SetActive(false);  // 패널 숨기기
    }
}
