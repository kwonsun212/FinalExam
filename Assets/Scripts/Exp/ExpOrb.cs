using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int expAmount = 10;
    private Transform player;
    private bool canMove = false;
    public float moveSpeed = 15f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        Invoke(nameof(EnableMove), 0.1f);
    }

    private void Update()
    {
        if (canMove && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void EnableMove() => canMove = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStat stat = collision.GetComponent<PlayerStat>();
            if (stat != null)
            {
                stat.AddExp(expAmount);
            }
            Destroy(gameObject);
        }
    }
}
