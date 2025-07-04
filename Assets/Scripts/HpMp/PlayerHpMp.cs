using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpMp : Entity
{
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private bool Invincible = false; // 무적 상태

    public bool isDead { get; private set; } = false;

    public float hpBonus = 0f;  // 추가 HP
    public float mpBonus = 0f;  // 추가 MP

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();  // Animator 컴포넌트 가져오기

        // Entity에 정의되어 있는 Setup() 메소드 호출
        base.Setup(); // 체력, 마나 기본 세팅

        // 저장된 데이터가 있다면 불러오기
        if (PlayerSaveManager.HasSaveData())
        {
            PlayerSaveManager.Load(this); // 저장된 데이터 불러오기
        }
    }

    private void OnDestroy()
    {
        // 씬이 바뀌기 전 데이터 저장
        PlayerSaveManager.Save(this); // 씬 이동 전 데이터 저장
    }

    private void Update()
    {
        
    }

    // 기본 체력 + 스탯 보너스 + 버프 등과 같이 계산
    public override float MaxHP => MaxHPBasic + MaxHPAttrBonus + hpBonus;
    // 100 + 현재레벨 * 30
    public float MaxHPBasic => 100 + 1 * 900;
    // 힘 * 10
    public float MaxHPAttrBonus => 0;

    public override float HPRecovery => 0;     //초당 Hp회복
    public override float MaxMP => 200 + mpBonus;
    public override float MPRecovery => 10;     //초당 Mp회복

    public override void TakeDamage(float damage)
    {
        if (Invincible) return;

        HP -= damage;

        if (HP > 0)
        {
            // 피해 입었을 때
            if (anim != null)
                anim.SetTrigger("Hurt"); //Hurt트리거 실행

            StartCoroutine(HitAnimation());
            StartCoroutine(InvincibleCoroutine());// 0.5초 무적
        }
        else
        {
            // 죽었을 때
            if (anim != null)
            {
              anim.SetTrigger("Death");   // HP 0이면 Death 애니메이션 실행
            }
  
                isDead = true;
            
            var deathUI = FindObjectOfType<DeathPopupController>();
            if (deathUI != null)
            {
                deathUI.ShowDeathPopup();  // 이제 0.4초 뒤에 패널이 뜹니다.
            }
        }
    }

    public void SetInvincible(bool value)
    {
        Invincible = value;
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

        // 원하는 무적 시간 (0.5초)
        yield return new WaitForSeconds(0.5f);

        Invincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)     //Enemy에게 닿으면 피해입음  
    {
        if (isDead) return;//사망 확인

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Mop mop = collision.gameObject.GetComponent<Mop>();
            if (mop != null && mop.isLive == false)
            {
                // 죽은 적이면 데미지 무시
                return;
            }

            // 고정 피해 10
            TakeDamage(10);
        }
    }
}
