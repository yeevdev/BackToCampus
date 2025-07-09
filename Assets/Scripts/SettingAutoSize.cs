using UnityEngine;
/// ① SettingBox 크기 자동 조정
/// ② TitleText(상단) · ResumeButton(하단) 고정 배치
[RequireComponent(typeof(RectTransform))]
public class SettingAutoSize : MonoBehaviour
{
    [Header("필수 연결")]
    public RectTransform titleText;     // "SETTINGS" 텍스트
    public RectTransform resumeButton;  // "Resume" 또는 "닫기" 버튼

    [Header("Box 비율 (캔버스 기준)")]
    [Range(0.3f, 1f)] public float widthRatio = 0.8f; // 0~1
    [Range(0.3f, 1f)] public float heightRatio = 0.5f;

    [Header("여백·고정 크기")]
    public float topMargin = 30f; // 타이틀 위 여백
    public float bottomMargin = 30f; // 버튼 아래 여백
    public float titleHeight = 60f; // 타이틀 높이
    public float buttonHeight = 50f; // 버튼 높이

    RectTransform boxRT;

    void Start()
    {
        boxRT = GetComponent<RectTransform>();

        /* ─── 1) SettingBox 크기 조정 ────────────────────── */
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform cRT = canvas.GetComponent<RectTransform>();

        boxRT.anchorMin = boxRT.anchorMax = new Vector2(0.5f, 0.5f);
        boxRT.pivot = new Vector2(0.5f, 0.5f);

        float boxW = cRT.rect.width * widthRatio;
        float boxH = cRT.rect.height * heightRatio;

        boxRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boxW);
        boxRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boxH);
        boxRT.anchoredPosition = Vector2.zero;

        /* ─── 2) TitleText 상단 고정 ─────────────────────── */
        if (titleText)
        {
            SetupAnchor(titleText, new Vector2(0.5f, 1f));  // 상단 중앙
            SetSize(titleText, boxW, titleHeight);
            titleText.anchoredPosition = new Vector2(0, -topMargin);
        }

        /* ─── 3) ResumeButton 하단 고정 ─────────────────── */
        if (resumeButton)
        {
            SetupAnchor(resumeButton, new Vector2(0.5f, 0f)); // 하단 중앙
            SetSize(resumeButton, boxW, buttonHeight);
            resumeButton.anchoredPosition = new Vector2(0, bottomMargin);
        }
    }

    /* ──────────────── 유틸 메서드 ──────────────── */
    void SetupAnchor(RectTransform rt, Vector2 anchor)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
    }

    void SetSize(RectTransform rt, float w, float h)
    {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
}
