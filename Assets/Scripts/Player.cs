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

    public bool IsSliding = false;           // ��� ������ ����
    public float SlidingSpeed = 20f;             // ��� �ӵ�
    public float SlideDuration = 0.4f;         // ��� �ð�
    private Vector2 SlideDirection;            // ��� ���� ����

    private bool Counter = false; // �����̵� �� Enemy�� ��Ҵ��� ����


    private bool AttackInSlide = true;

    CapsuleCollider2D CapCol;

    [SerializeField] private PlayerHpMp playerHpMp; // ��� ���� Ȯ��

    [SerializeField] private GameObject afterimagePrefab; // �ܻ�� ��������Ʈ ������
    [SerializeField] private float afterimageSpawnInterval = 0.05f; // �ܻ� ���� ���� (��)

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

        if (playerHpMp != null && playerHpMp.isDead)// ��� ������ ��� ���� �Ұ�
            return;

        // �ܻ� ���� Ÿ�̸� ó��
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

        if (IsSliding)// ��� ���̸� ���� �Ұ�
            return;

        // ���� ���� �ƴϸ� �Է��� �޾� �̵� ����
        if (!isAttacking)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputVec = Vector2.zero;    //  ���� ���� �� �Է� ���� 
        }

        // ���콺 ��Ŭ��: �⺻ ����
        if (Input.GetMouseButtonDown(0) &&
        !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking && (!IsSliding || AttackInSlide))
        {
            anim.SetTrigger("Attack");


            if (IsSliding)
            {
                StopCoroutine(SlideRoutine());  // �����̵� �ڷ�ƾ ����
                IsSliding = false;
                Counter = false;

                // ������ ����
                if (playerHpMp != null)
                    playerHpMp.SetInvincible(false);
            }

            StartCoroutine(AttackRoutine());
        }
        // ��Ŭ��: �����̵� �� ���� �浹�� ���¿��� Counter �ߵ�
        if (Input.GetMouseButtonDown(1))
        {
            if (IsSliding && Counter)
            {
                anim.SetTrigger("Counter");
                Counter = false;
            }
        }
        // �����̽���: ���
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
            sr.color = new Color(1f, 1f, 1f, 0.5f); // ������ ���� ����
            sr.sortingLayerID = spriter.sortingLayerID;
            sr.sortingOrder = spriter.sortingOrder - 1; // �÷��̾�� �ڿ� ǥ��
        }

        StartCoroutine(FadeAndDestroy(afterimage, 0.3f)); // 0.3�� �� �����
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

    //������ �̵� ������ ���� �ڷ�ƾ
    IEnumerator AttackRoutine()
    {
        if (playerHpMp.isDead)
        {
            yield break;        // �׾����� ���� ���
        }
            
        isAttacking = true;                         //�̵� ����
        yield return new WaitForSeconds(1.3f);      //1.3�� ���
        isAttacking = false;                        //�̵� ����
    }

    IEnumerator SlideRoutine()
    {
        IsSliding = true;
        SlideDirection = inputVec.normalized;  // �Է� ������ ��� �������� ����

        anim.SetTrigger("Slide");

        // 0.2�ʰ� ����
        if (playerHpMp != null)
            playerHpMp.SetInvincible(true);

        AttackInSlide = false; // �����̵� �ʱ� ���� ����


        // 0.4�� �Ŀ� ���� ����
        yield return new WaitForSeconds(0.4f);
        AttackInSlide = true;

        // ���� ���� (�� 0.2�� ��)
        if (playerHpMp != null)
            playerHpMp.SetInvincible(false);

        // ���� �����̵� �ð� ����
        yield return new WaitForSeconds(SlideDuration - 0.4f);

        IsSliding = false;
        Counter = false;
    }

    private void FixedUpdate()
    {
        if (playerHpMp != null && playerHpMp.isDead)
            return; // ��� �� �̵� ����
        if (IsSliding)
        {
            // ��� �߿� ���� �������� ������ �̵�
            rigid.MovePosition(rigid.position + SlideDirection * SlidingSpeed * Time.fixedDeltaTime);
        }
        else
        {
                //���� �̵� ó��
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }

            
    }

    private void LateUpdate()
    {
        if (playerHpMp != null && playerHpMp.isDead)
            return;

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

