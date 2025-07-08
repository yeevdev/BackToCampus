using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // ��ư���� �� �Լ��� ȣ���ϵ��� ����
    public void LoadNextScene()
    {
        // 1) ���� �ٲٱ� ���� �����ʿ��ѡ� ����� ������ ����
        CleanExtraAudioListeners();

        // 2) �� �� �ε� (Single ��忩�� ���� ���� ��ε��)
        SceneManager.LoadScene("GameScene");
    }
    /// �� �ȿ� AudioListener�� 2�� �̻��̸� �ϳ��� ����� ����
    private void CleanExtraAudioListeners()
    {
        // **s** �� ���� FindObjectsOfType -> �迭 ��ȯ
        AudioListener[] Listeners = FindObjectsOfType<AudioListener>();

        if (Listeners.Length > 1)
        {
            // ù ��°(0��)�� ����� ������ �ı�
            for (int i = 1; i < Listeners.Length; i++)
            {
                Destroy(Listeners[i]);
            }
        }
    }
}
