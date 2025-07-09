using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BackgroundAspectRatioScaler : MonoBehaviour
{
    [Header("픽셀 설정")]
    [Tooltip("텍스처 임포트 설정의 Pixels Per Unit과 동일한 값으로 설정하세요.")]
    public float pixelsPerUnit = 100f;

    void Start()
    {
        ScaleToFitScreen();
    }

    void ScaleToFitScreen()
    {
        var camera = Camera.main;
        if (camera == null || !camera.orthographic)
        {
            Debug.LogError("Orthographic 타입의 메인 카메라가 필요합니다.");
            return;
        }

        var rend = GetComponent<Renderer>();
        var texture = rend.material.mainTexture;
        if (texture == null)
        {
            Debug.LogError("배경 Material에 텍스처가 설정되지 않았습니다.");
            return;
        }

        // 1. 화면의 월드 크기를 계산합니다.
        float screenHeight = camera.orthographicSize * 2;
        float screenWidth = screenHeight * camera.aspect;

        // 2. 텍스처의 픽셀과 PPU를 이용해, 텍스처 자체의 월드 크기(비율)를 계산합니다.
        float textureWorldHeight = (float)texture.height / pixelsPerUnit;
        float textureWorldWidth = (float)texture.width / pixelsPerUnit;

        // 3. 화면을 채우기 위해 필요한 확대 비율(가로/세로 중 더 큰 값)을 찾습니다.
        float widthScaleFactor = screenWidth / textureWorldWidth;
        float heightScaleFactor = screenHeight / textureWorldHeight;
        float finalScaleFactor = Mathf.Max(widthScaleFactor, heightScaleFactor);

        // 4. 텍스처의 원래 월드 크기에 최종 확대 비율을 곱하여, 최종적으로 필요한 너비와 높이를 계산합니다.
        float finalWidth = textureWorldWidth * finalScaleFactor;
        float finalHeight = textureWorldHeight * finalScaleFactor;

        // 5. 이 최종 너비와 높이를 Quad의 localScale에 직접 적용합니다.
        transform.localScale = new Vector3(finalWidth, finalHeight, 1f);
    }
}