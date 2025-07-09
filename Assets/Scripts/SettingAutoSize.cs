using UnityEngine;
/// �� SettingBox ũ�� �ڵ� ����
/// �� TitleText(���) �� ResumeButton(�ϴ�) ���� ��ġ
[RequireComponent(typeof(RectTransform))]
public class SettingAutoSize : MonoBehaviour
{
    [Header("�ʼ� ����")]
    public RectTransform titleText;     // "SETTINGS" �ؽ�Ʈ
    public RectTransform resumeButton;  // "Resume" �Ǵ� "�ݱ�" ��ư

    [Header("Box ���� (ĵ���� ����)")]
    [Range(0.3f, 1f)] public float widthRatio = 0.8f; // 0~1
    [Range(0.3f, 1f)] public float heightRatio = 0.5f;

    [Header("���顤���� ũ��")]
    public float topMargin = 30f; // Ÿ��Ʋ �� ����
    public float bottomMargin = 30f; // ��ư �Ʒ� ����
    public float titleHeight = 60f; // Ÿ��Ʋ ����
    public float buttonHeight = 50f; // ��ư ����

    RectTransform boxRT;

    void Start()
    {
        boxRT = GetComponent<RectTransform>();

        /* ������ 1) SettingBox ũ�� ���� �������������������������������������������� */
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform cRT = canvas.GetComponent<RectTransform>();

        boxRT.anchorMin = boxRT.anchorMax = new Vector2(0.5f, 0.5f);
        boxRT.pivot = new Vector2(0.5f, 0.5f);

        float boxW = cRT.rect.width * widthRatio;
        float boxH = cRT.rect.height * heightRatio;

        boxRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boxW);
        boxRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boxH);
        boxRT.anchoredPosition = Vector2.zero;

        /* ������ 2) TitleText ��� ���� ���������������������������������������������� */
        if (titleText)
        {
            SetupAnchor(titleText, new Vector2(0.5f, 1f));  // ��� �߾�
            SetSize(titleText, boxW, titleHeight);
            titleText.anchoredPosition = new Vector2(0, -topMargin);
        }

        /* ������ 3) ResumeButton �ϴ� ���� �������������������������������������� */
        if (resumeButton)
        {
            SetupAnchor(resumeButton, new Vector2(0.5f, 0f)); // �ϴ� �߾�
            SetSize(resumeButton, boxW, buttonHeight);
            resumeButton.anchoredPosition = new Vector2(0, bottomMargin);
        }
    }

    /* �������������������������������� ��ƿ �޼��� �������������������������������� */
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
