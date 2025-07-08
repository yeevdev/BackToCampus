using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // 버튼에서 이 함수를 호출하도록 연결
    public void LoadNextScene()
    {
        // 1) 씬을 바꾸기 전에 ‘불필요한’ 오디오 리스너 제거
        CleanExtraAudioListeners();

        // 2) 새 씬 로드 (Single 모드여서 이전 씬은 언로드됨)
        SceneManager.LoadScene("GameScene");
    }
    /// 씬 안에 AudioListener가 2개 이상이면 하나만 남기고 제거
    private void CleanExtraAudioListeners()
    {
        // **s** 가 붙은 FindObjectsOfType -> 배열 반환
        AudioListener[] Listeners = FindObjectsOfType<AudioListener>();

        if (Listeners.Length > 1)
        {
            // 첫 번째(0번)만 남기고 나머지 파괴
            for (int i = 1; i < Listeners.Length; i++)
            {
                Destroy(Listeners[i]);
            }
        }
    }
}
