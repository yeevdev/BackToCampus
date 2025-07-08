using UnityEngine;

public class MapScroll : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int scrollSpeed;

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 맵 스크롤링 로직
        float scrollOffset = scrollSpeed * Time.deltaTime;
        meshRenderer.material.mainTextureOffset += new Vector2(0, scrollOffset);
    }
}
