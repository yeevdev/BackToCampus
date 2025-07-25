using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Slider hpBar;
    public float maxHp = 100f;
    public float currentHp;
    public float damagePerSecond = 10f;
    public bool isTakingDamage = false;
    private Coroutine damageCoroutine;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp; 
        UpdateHpBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isTakingDamage)
            {
                isTakingDamage = true;
                damageCoroutine = StartCoroutine(DamageOverTime());

            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 적과의 충돌이 끝나면 체력 감소 코루틴 중지
            if (collision.gameObject.CompareTag("Enemy"))
            {
                StopTakingDamage();

            }
        }
    }
    IEnumerator DamageOverTime()
    {
        isTakingDamage = true;
        while (currentHp > 0 && isTakingDamage)
        {
            currentHp -= damagePerSecond * Time.deltaTime;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp); // 체력이 0 이하로 내려가지 않도록 클램프
            UpdateHpBar();
            // 다음 프레임까지 기다림
            yield return null;
        }
        isTakingDamage = false;
    }
    void StopTakingDamage()
    {
        isTakingDamage = false;
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
    void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.value = currentHp / maxHp;
        }
    }
}
