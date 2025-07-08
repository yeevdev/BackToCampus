using UnityEngine;
using UnityEngine.UI;

public class SettingBoxLayout : MonoBehaviour
{
    public RectTransform settingsBox;
    public RectTransform titleText;
    public RectTransform resumeButton;

    void Start()
    {
        PositionTop(titleText, -30f, 60f);         // ��ܿ��� 30px �Ʒ�
        PositionBottom(resumeButton, 30f, 60f);    // �ϴܿ��� 30px ��
    }

    void PositionTop(RectTransform rt, float topMargin, float height)
    {
        rt.SetParent(settingsBox);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f); // ��� �߾�
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(settingsBox.rect.width * 0.8f, height);
        rt.anchoredPosition = new Vector2(0f, -topMargin);
    }

    void PositionBottom(RectTransform rt, float bottomMargin, float height)
    {
        rt.SetParent(settingsBox);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f); // �ϴ� �߾�
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(settingsBox.rect.width * 0.8f, height);
        rt.anchoredPosition = new Vector2(0f, bottomMargin);
    }
}
