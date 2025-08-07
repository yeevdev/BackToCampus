using System.Collections;
using UnityEngine;

/// <summary>
/// 잔상 1장. Spawn()으로 활성화되고, fadeTime 후 자동 Return().
/// </summary>
public class DashGhost : MonoBehaviour
{
    [SerializeField] float fadeTime = 0.25f;

    SpriteRenderer _sr;
    Rigidbody2D _rb;
    System.Action<DashGhost> _onReturn;   // 풀로 되돌릴 콜백

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Spawn(Sprite sprite, Color tint,
                      System.Action<DashGhost> returnCallback)
    {
        _sr.sprite = sprite;
        _sr.color  = tint;
        _onReturn  = returnCallback;

        gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = _sr.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0.6f, 0f, t / fadeTime);
            _sr.color = c;
            yield return null;
        }

        gameObject.SetActive(false);
        _onReturn?.Invoke(this);          // 풀에 반환
    }

    void FixedUpdate()
    {   // 스크롤에 맞춰 움직임
        _rb.MovePosition(_rb.position + GameManager.currentScrollSpeed * Time.fixedDeltaTime * Vector2.down);
    }
}