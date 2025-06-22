using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public Vector2 inputVec;        //�÷��̾� �Է� ����
    public float speed;             //�̵� �ӵ�

    Rigidbody2D rigid;              
    SpriteRenderer spriter;
    Animator anim;

    bool isAttacking = false;       //���� ������ ����(�������̸� �̵� �Ұ�)

    CapsuleCollider2D CapCol;


    [SerializeField] private SpriteRenderer shadowSpriter;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CapCol = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // ���� ���� �ƴϸ� �Է��� �޾� �̵� ����
        if(!isAttacking)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputVec = Vector2.zero;    //  ���� ���� �� �Է� ���� 
        }


        if (Input.GetKeyDown(KeyCode.Mouse0) &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking)
        {
            anim.SetTrigger("Attack");                  //�ִϸ��̼� Ʈ���� ����
            StartCoroutine(AttackRoutine());            //���� ó�� �ڷ�ƾ ����
        }
    }

    //������ �̵� ������ ���� �ڷ�ƾ
    IEnumerator AttackRoutine()
    {
        isAttacking = true;                         //�̵� ����
        yield return new WaitForSeconds(1.3f);      //1.1�� ���
        isAttacking = false;                        //�̵� ����
    }

    private void FixedUpdate()
    {
        //���� �̵� ó��
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        //�ִϸ��̼� �ӵ� �Ķ���� ����(�����ӿ� ����)
        anim.SetFloat("Speed",inputVec.magnitude);

        //�̵� ���⿡ ���� ��������Ʈ ����
        if (inputVec.x != 0)
        {
            bool isLeft = inputVec.x < 0;
            spriter.flipX = isLeft;

            //�׸��ڵ� ���� �������� ����
            if (shadowSpriter != null)
                shadowSpriter.flipX = isLeft;


            // �ݶ��̴��� offset.x ����
            Vector2 offset = CapCol.offset;
            offset.x = Mathf.Abs(offset.x) * (isLeft ? 1 : -1);
            CapCol.offset = offset;
        }
    }
}

