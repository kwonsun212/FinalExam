using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public Vector2 inputVec;        //플레이어 입력 방향
    public float speed;             //이동 속도

    Rigidbody2D rigid;              
    SpriteRenderer spriter;
    Animator anim;

    bool isAttacking = false;       //공격 중인지 여부(공격중이면 이동 불가)

    public bool IsSliding = false;           // 대시 중인지 여부
    public float SlidingSpeed = 20f;             // 대시 속도
    public float SlideDuration = 0.4f;         // 대시 시간
    private Vector2 SlideDirection;            // 대시 방향 고정

    private bool Counter = false; // 슬라이드 중 Enemy와 닿았는지 여부


    private bool AttackInSlide = true;

    CapsuleCollider2D CapCol;

    [SerializeField] private PlayerHpMp playerHpMp; // 사망 상태 확인

    [SerializeField] private GameObject afterimagePrefab; // 잔상용 스프라이트 프리팹
    [SerializeField] private float afterimageSpawnInterval = 0.05f; // 잔상 생성 간격 (초)

    private float afterimageTimer = 0f;


    [SerializeField] private SpriteRenderer shadowSpriter;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CapCol = GetComponent<CapsuleCollider2D>();

        if (playerHpMp == null)
            playerHpMp = GetComponent<PlayerHpMp>();
    }

    private void Update()
    {

        if (playerHpMp != null && playerHpMp.isDead)// 사망 상태일 경우 조작 불가
            return;

        // 잔상 생성 타이머 처리
        if (IsSliding)
        {
            afterimageTimer += Time.deltaTime;
            if (afterimageTimer >= afterimageSpawnInterval)
            {
                SpawnAfterimage();
                afterimageTimer = 0f;
            }
        }
        else
        {
            afterimageTimer = 0f;
        }

        if (IsSliding)// 대시 중이면 조작 불가
            return;

        // 공격 중이 아니면 입력을 받아 이동 가능
        if (!isAttacking)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputVec = Vector2.zero;    //  공격 중일 땐 입력 무시 
        }

        // 마우스 좌클릭: 기본 공격
        if (Input.GetMouseButtonDown(0) &&
        !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking && (!IsSliding || AttackInSlide))
        {
            anim.SetTrigger("Attack");


            if (IsSliding)
            {
                StopCoroutine(SlideRoutine());  // 슬라이드 코루틴 종료
                IsSliding = false;
                Counter = false;

                // 무적도 해제
                if (playerHpMp != null)
                    playerHpMp.SetInvincible(false);
            }

            StartCoroutine(AttackRoutine());
        }
        // 우클릭: 슬라이딩 중 적과 충돌한 상태에서 Counter 발동
        if (Input.GetMouseButtonDown(1))
        {
            if (IsSliding && Counter)
            {
                anim.SetTrigger("Counter");
                Counter = false;
            }
        }
        // 스페이스바: 대시
        if (Input.GetKeyDown(KeyCode.Space) && inputVec != Vector2.zero)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsSliding && !playerHpMp.isDead && collision.collider.CompareTag("Enemy"))
        {
            Counter = true;
        }
    }

    void SpawnAfterimage()
    {
        GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);

        SpriteRenderer sr = afterimage.GetComponent<SpriteRenderer>();
        if (sr != null && spriter != null)
        {
            sr.sprite = spriter.sprite;
            sr.flipX = spriter.flipX;
            sr.color = new Color(1f, 1f, 1f, 0.5f); // 반투명 조절 가능
            sr.sortingLayerID = spriter.sortingLayerID;
            sr.sortingOrder = spriter.sortingOrder - 1; // 플레이어보다 뒤에 표시
        }

        StartCoroutine(FadeAndDestroy(afterimage, 0.3f)); // 0.3초 후 사라짐
    }

    IEnumerator FadeAndDestroy(GameObject obj, float duration)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float timer = 0f;
        Color initialColor = sr.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, timer / duration);
            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }

    //공격중 이동 금지를 위한 코루틴
    IEnumerator AttackRoutine()
    {
        if (playerHpMp.isDead)
        {
            yield break;        // 죽었으면 공격 취소
        }
            
        isAttacking = true;                         //이동 금지
        yield return new WaitForSeconds(1.3f);      //1.3초 대기
        isAttacking = false;                        //이동 가능
    }

    IEnumerator SlideRoutine()
    {
        IsSliding = true;
        SlideDirection = inputVec.normalized;  // 입력 방향을 대시 방향으로 고정

        anim.SetTrigger("Slide");

        // 0.2초간 무적
        if (playerHpMp != null)
            playerHpMp.SetInvincible(true);

        AttackInSlide = false; // 슬라이드 초기 공격 금지


        // 0.4초 후에 공격 가능
        yield return new WaitForSeconds(0.4f);
        AttackInSlide = true;

        // 무적 해제 (총 0.2초 후)
        if (playerHpMp != null)
            playerHpMp.SetInvincible(false);

        // 남은 슬라이드 시간 유지
        yield return new WaitForSeconds(SlideDuration - 0.4f);

        IsSliding = false;
        Counter = false;
    }

    private void FixedUpdate()
    {
        if (playerHpMp != null && playerHpMp.isDead)
            return; // 사망 시 이동 금지
        if (IsSliding)
        {
            // 대시 중엔 고정 방향으로 빠르게 이동
            rigid.MovePosition(rigid.position + SlideDirection * SlidingSpeed * Time.fixedDeltaTime);
        }
        else
        {
                //물리 이동 처리
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }

            
    }

    private void LateUpdate()
    {
        if (playerHpMp != null && playerHpMp.isDead)
            return;

        //애니메이션 속도 파라미터 설정(움직임에 따라)
        anim.SetFloat("Speed",inputVec.magnitude);

        //이동 방향에 따라 스프라이트 반전
        if (inputVec.x != 0)
        {
            bool isLeft = inputVec.x < 0;
            spriter.flipX = isLeft;

            //그림자도 같은 방향으로 반전
            if (shadowSpriter != null)
                shadowSpriter.flipX = isLeft;


            // 콜라이더의 offset.x 반전
            Vector2 offset = CapCol.offset;
            offset.x = Mathf.Abs(offset.x) * (isLeft ? 1 : -1);
            CapCol.offset = offset;
        }
    }
}

