using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mop : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public float maxHP = 100f;

    [Header("AI Settings")]
    [Tooltip("이 반경 안에 플레이어가 들어오면 추적 시작")]
    public float chaseRadius = 5f;

    [Header("References")]
    public Rigidbody2D target;    // 플레이어 Rigidbody2D 할당

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

        // 플레이어와의 거리 계산
        Vector2 dirVec = (Vector2)target.position - rigid.position;
        float sqrDist = dirVec.sqrMagnitude;

        // chaseRadius 이내라면 이동
        if (sqrDist <= chaseRadius * chaseRadius)
        {
            Vector2 moveVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + moveVec);
        }
        // else: 아무 동작 안 함 (idle)

        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!isLive) return;
        // 플레이어 방향에 맞춰 스프라이트 뒤집기
        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 기존 데미지 처리
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

        // 경험구슬 드랍
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
        // 사망 시 골드 드롭
        int goldDrop = Random.Range(1, 6);
        PlayerStat stat = GameObject.FindObjectOfType<PlayerStat>();
        if (stat != null)
        {
            stat.AddGold(goldDrop);
        }

        //변경된 골드까지 포함해 즉시 저장
        PlayerHpMp player = GameObject.FindObjectOfType<PlayerHpMp>();
        if (player != null)
        {
            PlayerSaveManager.Save(player);
        }
    }

    // 에디터에서 chaseRadius를 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
