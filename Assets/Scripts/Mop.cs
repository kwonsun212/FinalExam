using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Mop : MonoBehaviour
{
    public float speed;
    
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    
    public bool isLive = true;
       
    private float currentHP;
    public float maxHP = 100;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
    }
    private void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") )
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    private void LateUpdate()
    {
        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
            return;

        currentHP -= collision.GetComponent<Bullet>().damage;

        if (currentHP > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            Dead();
        }


    }
    private void Dead()
    {
        //gameObject.SetActive(false);
        isLive = false;
        anim.SetTrigger("Dead");

    }
}
