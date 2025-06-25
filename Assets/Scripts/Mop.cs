using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mop : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public float maxHP = 100f;

    [Header("AI Settings")]
    [Tooltip("�� �ݰ� �ȿ� �÷��̾ ������ ���� ����")]
    public float chaseRadius = 5f;

    [Header("References")]
    public Rigidbody2D target;    // �÷��̾� Rigidbody2D �Ҵ�

    private float currentHP;
    public bool isLive = true;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;

    [SerializeField] private GameObject expOrbPrefab;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
    }

    void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        // �÷��̾���� �Ÿ� ���
        Vector2 dirVec = (Vector2)target.position - rigid.position;
        float sqrDist = dirVec.sqrMagnitude;

        // chaseRadius �̳���� �̵�
        if (sqrDist <= chaseRadius * chaseRadius)
        {
            Vector2 moveVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + moveVec);
        }
        // else: �ƹ� ���� �� �� (idle)

        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!isLive) return;
        // �÷��̾� ���⿡ ���� ��������Ʈ ������
        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� ������ ó��
        if (!collision.CompareTag("Bullet") || !isLive) return;

        currentHP -= collision.GetComponent<Bullet>().damage;
        if (currentHP > 0)
            anim.SetTrigger("Hit");
        else
            Dead();
    }

    private void Dead()
    {
        isLive = false;
        anim.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 10f);

        // ���豸�� ���
        if (expOrbPrefab != null)
        {
            int orbCount = Random.Range(1, 6);
            for (int i = 0; i < orbCount; i++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.3f, 0.3f), 0);
                Instantiate(expOrbPrefab, transform.position + offset, Quaternion.identity);
            }
        }
        // ��� �� ��� ���
        int goldDrop = Random.Range(1, 6);
        PlayerStat stat = GameObject.FindObjectOfType<PlayerStat>();
        if (stat != null)
        {
            stat.AddGold(goldDrop);
        }

        //����� ������ ������ ��� ����
        PlayerHpMp player = GameObject.FindObjectOfType<PlayerHpMp>();
        if (player != null)
        {
            PlayerSaveManager.Save(player);
        }
    }

    // �����Ϳ��� chaseRadius�� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
