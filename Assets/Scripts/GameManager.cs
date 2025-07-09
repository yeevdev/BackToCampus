// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어떤 스크립트에서든 접근 가능한 '현재 월드 스크롤 속도' 변수
    // 0이면 멈춘 상태, 0보다 크면 스크롤 중인 상태입니다.
    public static float currentScrollSpeed = 0f;
}