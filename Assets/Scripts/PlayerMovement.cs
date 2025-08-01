using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("기본 이동")]
    [SerializeField] float speedCoef = 5f;
    [SerializeField] float upBound = 4f, downBound = -4f, horizontalBound = 8f;
    [SerializeField] Renderer mapRenderer;
    [SerializeField] JoystickController joystick;

    [Header("대시")]
    [SerializeField] float dashDistance = 3f;
    [SerializeField] float dashTime = 0.15f;

    [Header("대시 스프라이트 (▲▼◀▶)")]
    [SerializeField] Sprite dashUp, dashDown, dashLeft, dashRight;

    [Header("잔상 풀")]
    [SerializeField] DashGhost ghostPrefab;
    [SerializeField] int poolSize = 10;
    [SerializeField] float ghostInterval = 0.05f;
    [SerializeField] Color ghostTint = new Color(1f, 1f, 1f, 0.6f);

    // ─── 내부 ───
    Rigidbody2D     rb;
    Animator        anim;
    SpriteRenderer  sr;

    Vector2         moveInput;
    bool            isDashing;

    // 맵 스크롤
    Material mapMat;
    float    mapWorldH;

    // 잔상 풀
    readonly Queue<DashGhost> pool = new Queue<DashGhost>();

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        sr   = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (mapRenderer)
        {
            mapMat     = mapRenderer.material;
            mapWorldH  = mapRenderer.bounds.size.y;
        }

        // 풀 미리 채우기
        for (int i = 0; i < poolSize; i++)
            pool.Enqueue(Instantiate(ghostPrefab, transform.parent));
    }

    void OnEnable()  => joystick.OnDash += StartDash;
    void OnDisable() => joystick.OnDash -= StartDash;

    void Update()
    {
        if (isDashing) return;

        moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Vector2 next = rb.position + moveInput * speedCoef * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, -horizontalBound, horizontalBound);

        bool scrolling = rb.position.y >= upBound && moveInput.y > 0;

        if (scrolling && mapMat && mapWorldH > 0)
        {
            next.y = upBound;

            float worldMove = moveInput.y * speedCoef * Time.fixedDeltaTime;
            GameManager.currentScrollSpeed = moveInput.y * speedCoef;
            mapMat.mainTextureOffset += new Vector2(0, worldMove / mapWorldH);
        }
        else
        {
            next.y = Mathf.Clamp(next.y, downBound, upBound);
            GameManager.currentScrollSpeed = 0f;  // ★ 스크롤 멈춤
        }

        rb.MovePosition(next);
    }

    // ── 대시 시작 ──
    void StartDash(Vector2 dir)
    {
        if (!isDashing)
            StartCoroutine(DashRoutine(dir));
    }

    // ── 대시 코루틴 ──
    IEnumerator DashRoutine(Vector2 dir)
    {
        isDashing = true;

        anim.enabled = false;
        Sprite orgSpr = sr.sprite;
        Sprite dashSpr = DirToSprite(dir);
        sr.sprite = dashSpr;

        Vector2 start   = rb.position;
        Vector2 target  = start + dir * dashDistance;

        float elapsed   = 0f;
        float ghostTm   = 0f;
        float prevRawY  = start.y;
        float scrollAcc = 0f;     // ★ 누적 스크롤량(월드 단위)

        while (elapsed < dashTime)
        {
            elapsed += Time.fixedDeltaTime;
            float t  = elapsed / dashTime;

            Vector2 rawNext = Vector2.Lerp(start, target, t);
            Vector2 next    = rawNext;

            if (rawNext.y > upBound)
            {
                next.y = upBound;

                // Δ overshoot
                float ovNow   = rawNext.y - upBound;
                float ovPrev  = Mathf.Max(0, prevRawY - upBound);
                float deltaOv = ovNow - ovPrev;

                // 맵 스크롤
                if (mapMat && mapWorldH > 0)
                {
                    GameManager.currentScrollSpeed = deltaOv / Time.fixedDeltaTime;
                    mapMat.mainTextureOffset += new Vector2(0, deltaOv / mapWorldH);
                }

                scrollAcc += deltaOv;               // ★ 누적 스크롤량 갱신
            }
            else
            {
                GameManager.currentScrollSpeed = 0f;  // ★ 스크롤 멈춤
            }

            next.x = Mathf.Clamp(next.x, -horizontalBound, horizontalBound);
            next.y = Mathf.Clamp(next.y,  downBound,        upBound);
            rb.MovePosition(next);

            // ─── 잔상 ───
            ghostTm += Time.fixedDeltaTime;
            if (ghostTm >= ghostInterval)
            {
                ghostTm = 0f;

                // ① 플레이어 현재 위치 next
                // ② 누적 스크롤만큼 아래로 오프셋
                Vector2 ghostPos = next;
                ghostPos.y -= scrollAcc;            // ★ 바로 이 한 줄!
                SpawnGhost(dashSpr, ghostPos);
            }

            prevRawY = rawNext.y;
            yield return new WaitForFixedUpdate();
        }

        sr.sprite = orgSpr;
        anim.enabled = true;
        isDashing = false;
    }

    // ── 잔상 풀 사용 ──
    void SpawnGhost(Sprite spr, Vector2 pos)
    {
        DashGhost g = pool.Count > 0 ? pool.Dequeue()
                                    : Instantiate(ghostPrefab, transform.parent);

        g.transform.position = pos;               // rawNext 좌표
        g.transform.localScale = sr.transform.localScale;
        g.Spawn(spr, ghostTint, ReturnGhost);
    }
    void ReturnGhost(DashGhost g) => pool.Enqueue(g);

    // ── 방향 → 스프라이트 ──
    Sprite DirToSprite(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? dashRight : dashLeft;
        else
            return dir.y > 0 ? dashUp    : dashDown;
    }

    // ── 걷기 애니메이션 유지 ──
    Vector2 lastDir;
    void HandleAnimation()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("moveX", moveInput.x);
            anim.SetFloat("moveY", moveInput.y);
            lastDir = moveInput;
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.SetFloat("moveX", lastDir.x);
            anim.SetFloat("moveY", lastDir.y);
        }
    }
}