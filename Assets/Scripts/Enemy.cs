using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float healtht;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLive)
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

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        healtht = maxHealth;
    }

    public void Init(SpawnData data)
    {
       anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        healtht = data.health;
    }
}
