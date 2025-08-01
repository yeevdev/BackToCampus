// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject dashCooldownBar;
    // 어떤 스크립트에서든 접근 가능한 '현재 월드 스크롤 속도' 변수
    // 0이면 멈춘 상태, 0보다 크면 스크롤 중인 상태입니다.
    public static float currentScrollSpeed = 0f;

    void Start()
    {
        ReadyToStart();
    }

    void ReadyToStart()
    {
        Time.timeScale = 0f;

        startUI.SetActive(true);
    }

    public void GameStart()
    {
        startUI.SetActive(false);

        Time.timeScale = 1f;

        joystick.SetActive(true);
        dashCooldownBar.SetActive(true);
    }
}