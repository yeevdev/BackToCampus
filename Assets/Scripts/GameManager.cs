using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject dashCooldownBar;

    [Header("대쉬 시 어두워지는 것들 설정")]
    [Tooltip("대시할 때 함께 어두워질 스프라이트 렌더러 목록입니다.")]
    [SerializeField] private List<SpriteRenderer> dimmingSpritesSetting;

    [Tooltip("대시할 때 함께 어두워질 메쉬 렌더러 목록입니다. (예: 배경)")]
    [SerializeField] private List<MeshRenderer> dimmingMeshesSetting;

    [Tooltip("대시할 때 함께 어두워질 UI 그래픽 요소 목록입니다. (Image, Text, RawImage)")]
    [SerializeField] private List<Graphic> dimmingGraphicsSetting; 

    [Tooltip("얼마나 어두워질지 결정합니다. (0: 어두워지지 않음, 1: 가장 어두움)")]
    [Range(0, 1)] [SerializeField] private float dimnessSetting = 0.5f;

    // 스태틱 변수
    // 인스펙터에서 설정하는 고정 리스트
    private static List<SpriteRenderer> fixedDimmingSprites;
    private static List<MeshRenderer> fixedDimmingMeshes;
    private static List<Graphic> fixedDimmingGraphics;

    // NPC처럼 동적으로 추가/제거되는 스프라이트를 위한 리스트
    private static List<SpriteRenderer> dynamicDimmingSprites = new();

    private static float dimness;

    // 어떤 스크립트에서든 접근 가능한 '현재 월드 스크롤 속도' 변수
    // 0이면 멈춘 상태, 0보다 크면 스크롤 중인 상태입니다.
    public static float currentScrollSpeed = 0f;

    public static bool isPlayerDashing = false;

    void Awake() // Start 대신 Awake를 사용하여 실행 순서 문제를 방지합니다.
    {
        // static 변수는 인스턴스가 여러 개일 때 문제가 될 수 있으므로,
        // 싱글톤 패턴처럼 하나의 인스턴스만 값을 설정하도록 방어 코드를 추가하는 것이 좋습니다.
        if (fixedDimmingSprites == null) 
        {
            fixedDimmingSprites = dimmingSpritesSetting;
            fixedDimmingMeshes = dimmingMeshesSetting;
            fixedDimmingGraphics = dimmingGraphicsSetting;
            dimness = dimnessSetting;
        }
    }

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

    public static void AddDimmingSprites(SpriteRenderer renderer)
    {
        // 중복 추가를 방지합니다.
        if (renderer != null && !dynamicDimmingSprites.Contains(renderer))
        {
            dynamicDimmingSprites.Add(renderer);
        }
    }
    
    public static void RemoveDimmingSprites(SpriteRenderer renderer)
    {
        if (renderer != null)
        {
            dynamicDimmingSprites.Remove(renderer);   
        }
    }
    
    public static void DimSprites()
    {
        float brightness = 1 - dimness;
        Color dimColor = new(brightness, brightness, brightness);

        // 고정 스프라이트 어둡게
        foreach (SpriteRenderer renderer in fixedDimmingSprites)
        {
            if (renderer != null) renderer.color = dimColor;
        }
        // 동적 스프라이트(NPC 등) 어둡게
        foreach (SpriteRenderer renderer in dynamicDimmingSprites)
        {
            if (renderer != null) renderer.color = dimColor;
        }
        // 메쉬 어둡게 (배경화면)
        foreach (MeshRenderer renderer in fixedDimmingMeshes)
        {
            if (renderer != null) renderer.material.color = dimColor;
        }
        // UI 어둡게
        foreach (Graphic graphic in fixedDimmingGraphics)
        {
            if (graphic != null) graphic.color = dimColor;
        }
    } 
    
    public static void UndimSprites()
    {
        // 고정 스프라이트 다시 밝게
        foreach (SpriteRenderer renderer in fixedDimmingSprites)
        {
            if (renderer != null) renderer.color = Color.white;
        }
        // 동적 스프라이트(NPC 등) 다시 밝게
        foreach (SpriteRenderer renderer in dynamicDimmingSprites)
        {
            if (renderer != null) renderer.color = Color.white;
        }
        // 메쉬 다시 밝게 (배경화면)
        foreach (MeshRenderer renderer in fixedDimmingMeshes)
        {
            if (renderer != null) renderer.material.color = Color.white;
        }
        // UI 다시 밝게
        foreach (Graphic graphic in fixedDimmingGraphics)
        {
            if (graphic != null) graphic.color = Color.white;
        }
    } 
}