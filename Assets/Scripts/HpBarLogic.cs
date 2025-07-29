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
            // ������ �浹�� ������ ü�� ���� �ڷ�ƾ ����
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
            currentHp = Mathf.Clamp(currentHp, 0, maxHp); // ü���� 0 ���Ϸ� �������� �ʵ��� Ŭ����
            UpdateHpBar();
            // ���� �����ӱ��� ��ٸ�
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
