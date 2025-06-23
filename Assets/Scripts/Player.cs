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

    public bool IsSliding = false;           // 슬라이딩 중인지 여부
    public float SlidingSpeed = 20f;             // 슬라이딩 속도
    public float SlideDuration = 0.4f;         // 슬라이딩 시간
    private Vector2 SlideDirection;            // 슬라이딩 방향 고정

    private bool AttackInSlide = true;

    CapsuleCollider2D CapCol;

    [SerializeField] private PlayerHpMp playerHpMp; // 사망 상태 확인

    [SerializeField] private GameObject afterimagePrefab; // 잔상용 스프라이트 프리팹
    [SerializeField] private float afterimageSpawnInterval = 0.05f; // 잔상 생성 간격 (초)

    private float afterimageTimer = 0f; // 잔상 생성용 타이머

    [SerializeField] private SpriteRenderer shadowSpriter; // 플레이어 그림자

    [SerializeField] private BulletPos bulletPosScript;  // BulletPos 스크립트 참조

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CapCol = GetComponent<CapsuleCollider2D>();

        if (playerHpMp == null)
            playerHpMp = GetComponent<PlayerHpMp>();

        if (bulletPosScript == null)
            bulletPosScript = FindObjectOfType<BulletPos>();
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
                SpawnAfterimage();  // 잔상 생성
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


                // 무적 해제
                if (playerHpMp != null)
                    playerHpMp.SetInvincible(false);
            }

            StartCoroutine(LeftAttackRoutine());   // 공격 중 이동 금지 처리
        }
        // Counter 공격 (우클릭)
        if (Input.GetMouseButtonDown(1) && !isAttacking && (!IsSliding || AttackInSlide))
        {
            StartCoroutine(CounterAttackRoutine());
        }

        // 스페이스바: 대시
        if (Input.GetKeyDown(KeyCode.Space) && inputVec != Vector2.zero)
        {
            StartCoroutine(SlideRoutine());
        }
    }





    void SpawnAfterimage()
    {
        GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);

        SpriteRenderer sr = afterimage.GetComponent<SpriteRenderer>();
        if (sr != null && spriter != null)
        {
            sr.sprite = spriter.sprite;     // 현재 플레이어 스프라이트 복사
            sr.flipX = spriter.flipX;       // 방향 복사
            sr.color = new Color(1f, 1f, 1f, 0.5f); // 반투명 조절 가능
            sr.sortingLayerID = spriter.sortingLayerID;
            sr.sortingOrder = spriter.sortingOrder - 1; // 플레이어보다 뒤에 표시
        }

        StartCoroutine(FadeAndDestroy(afterimage, 0.3f)); // 0.3초 후 사라짐
    }

    IEnumerator CounterAttackRoutine()
    {
        anim.SetTrigger("Counter");   // 카운터 애니메이션 트리거
        StartCoroutine(RightAttackRoutine());        // 공격 중 이동 금지 코루틴 실행
        yield return new WaitForSeconds(0.4f);  // 0.2초 대기
        bulletPosScript?.ShootCounterBullets(); // 8방향 검기 발사

    }


    IEnumerator FadeAndDestroy(GameObject obj, float duration)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float timer = 0f;
        Color initialColor = sr.color;

        // 점점 투명하게 만들어 사라지게 함
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, timer / duration);
            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        Destroy(obj);   // 잔상 오브젝트 삭제
    }

    //공격중 이동 금지를 위한 코루틴
    IEnumerator LeftAttackRoutine()
    {
        if (playerHpMp.isDead)
        {
            yield break;        // 죽었으면 공격 취소
        }
            
        isAttacking = true;                         //이동 금지
        yield return new WaitForSeconds(1.3f);      //1.3초 대기
        isAttacking = false;                        //이동 가능
    }

    IEnumerator RightAttackRoutine()
    {
        if (playerHpMp.isDead)
        {
            yield break;        // 죽었으면 공격 취소
        }

        isAttacking = true;                         //이동 금지
        yield return new WaitForSeconds(0.7f);      //0.7초 대기
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

