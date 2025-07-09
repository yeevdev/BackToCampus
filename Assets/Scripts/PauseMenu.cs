using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PausePanel.SetActive(false); // ���� �� �Ͻ� ���� �г� ��Ȱ��ȭ
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Menu_Btn()
    {
        Time.timeScale = 0; // ���� �Ͻ� ����
        PausePanel.SetActive(true); // �Ͻ� ���� �г� Ȱ��ȭ
    }
    public void Resume_Btn()
    {
        Time.timeScale = 1f;          // ���� �ӵ� ���󺹱�
        PausePanel.SetActive(false);  // �г� �����
    }
}
