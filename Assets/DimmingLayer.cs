using UnityEngine;

public class DimmingLayer : MonoBehaviour
{
    [Range(0, 1)][SerializeField] private float dimness = 0.7f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 0, 0, dimness);

        transform.position = Vector3.zero;

        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        transform.localScale = new Vector2(screenWidth, screenHeight);
    }

    // 어두워지기
    public void Dim()
    {
        sr.enabled = true;
    }

    // 다시 밝아지기
    public void Undim()
    {
        sr.enabled = false;
    }

    // 특정 renderer가 어두워져야 하는지 아닌지를 설정합니다. 
    public static void SetDimmingRenderer(SpriteRenderer renderer, bool isDimming)
    {
        if (isDimming)
        {
            renderer.sortingLayerName = "Dimming";
        }
        else
        {
            renderer.sortingLayerName = "NoDimming";
        }
    }
}