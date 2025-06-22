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

    CapsuleCollider2D CapCol;

    [SerializeField] private PlayerHpMp playerHpMp; // 사망 상태 확인


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

        if (playerHpMp != null && playerHpMp.isDead)
            return; // 사망 상태일 경우 조작 불가

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


        if (Input.GetKeyDown(KeyCode.Mouse0) &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking)
        {
            anim.SetTrigger("Attack");                  //애니메이션 트리거 설정
            StartCoroutine(AttackRoutine());            //공격 처리 코루틴 시작
        }
    }

    //공격중 이동 금지를 위한 코루틴
    IEnumerator AttackRoutine()
    {
        if (!playerHpMp.isDead)
        {
            isAttacking = false;
        }
            
        isAttacking = true;                         //이동 금지
        yield return new WaitForSeconds(1.3f);      //1.1초 대기
        isAttacking = false;                        //이동 가능
    }

    private void FixedUpdate()
    {
        if (playerHpMp != null && playerHpMp.isDead)
            return; // 사망 시 이동 금지

        //물리 이동 처리
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
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

