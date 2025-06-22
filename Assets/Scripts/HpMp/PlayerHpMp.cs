using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpMp : Entity
{
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private bool Invincible = false; // ���� ����

    public bool isDead { get; private set; } = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();  // Animator ������Ʈ ��������

        // Entity�� ���ǵǾ� �ִ� Setup() �޼ҵ� ȣ��
        base.Setup(); // ü��, ���� �⺻ ����

        // ����� �����Ͱ� �ִٸ� �ҷ�����
        if (PlayerSaveManager.HasSaveData())
        {
            PlayerSaveManager.Load(this); // ����� ������ �ҷ�����
        }
    }

    private void OnDestroy()
    {
        // ���� �ٲ�� �� ������ ����
        PlayerSaveManager.Save(this); // �� �̵� �� ������ ����
    }

    private void Update()
    {
        // �⺻ ����
        if (Input.GetKeyDown("1"))
        {
            HP += 100;
            target.TakeDamage(20);
        }
        // ��ų ����
        else if (Input.GetKeyDown("2"))
        {
            MP -= 100;
            target.TakeDamage(55);
        }
    }

    // �⺻ ü�� + ���� ���ʽ� + ���� ��� ���� ���
    public override float MaxHP => MaxHPBasic + MaxHPAttrBonus;
    // 100 + ���緹�� * 30
    public float MaxHPBasic => 100 + 1 * 30;
    // �� * 10
    public float MaxHPAttrBonus => 10 * 10;

    public override float HPRecovery => 0;     //�ʴ� Hpȸ��
    public override float MaxMP => 200;
    public override float MPRecovery => 10;     //�ʴ� Mpȸ��

    public override void TakeDamage(float damage)
    {
        if (Invincible) return;

        HP -= damage;

        if (HP > 0)
        {
            // ���� �Ծ��� ��
            if (anim != null)
                anim.SetTrigger("Hurt"); //HurtƮ���� ����

            StartCoroutine(HitAnimation());
            StartCoroutine(InvincibleCoroutine());// 0.5�� ����
        }
        else
        {
            // �׾��� ��
            if (anim != null)
            {
              anim.SetTrigger("Death");   // HP 0�̸� Death �ִϸ��̼� ����
            }
  
                isDead = true;
        }
    }

    private IEnumerator HitAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.1f);

        color.a = 1;
        spriteRenderer.color = color;
    }

    private IEnumerator InvincibleCoroutine()
    {
        Invincible = true;

        // ���ϴ� ���� �ð� (0.5��)
        yield return new WaitForSeconds(0.5f);

        Invincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)     //Enemy���� ������ ��������  
    {
        if (isDead) return;//��� Ȯ��

        if (collision.gameObject.CompareTag("Enemy"))
        {
            //���� ���� 10
            TakeDamage(10);
        }
    }
}
