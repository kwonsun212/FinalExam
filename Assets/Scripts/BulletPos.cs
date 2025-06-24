using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPos : MonoBehaviour
{
    public GameObject bulletPos;        // �Ѿ� ���� ��ġ (�� ������Ʈ)
    public GameObject preFabBullet;     // �Ѿ� ������
    public float bulletSpeed = 10f;     // �߻� �ӵ�


    Camera cam;         //����ī�޶�

    private bool canShoot = true;


    [SerializeField] private PlayerHpMp player; // MP�� ���̱� ���� ����
    [SerializeField] private Player move; // �����̵� ��ä Ȯ��
    [SerializeField] private PlayerStat playerStat;  // ���ݷ� ����

    private void Start()
    {
        cam = Camera.main;

        if (player == null)
            player = FindObjectOfType<PlayerHpMp>();

        if (move == null)
            move = FindObjectOfType<Player>();

        //�ڵ� ����
        if (playerStat == null)
            playerStat = FindObjectOfType<PlayerStat>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && !player.isDead && !move.IsSliding)
        {
            StartCoroutine(ShootSequence());
        }
    }


    IEnumerator ShootSequence()
    {
        canShoot = false;

        yield return new WaitForSeconds(0.5f);
        if (player.isDead) yield break;

        if (player.MP >= 15)
        {
            Shoot();
            player.MP -= 15;
        }

        yield return new WaitForSeconds(0.4f);
        if (player.isDead) yield break;

        if (player.MP >= 15)
        {
            Shoot();
            player.MP -= 15;
        }

        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }

    void Shoot()
    {
        // 1. ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // 2D �����̹Ƿ� z=0 ����

        // 2. �߻� ���� ���
        Vector3 direction = (mouseWorldPos - bulletPos.transform.position).normalized;

        // 3. �Ѿ� ���� �� �߻�
        GameObject bullet = Instantiate(preFabBullet, bulletPos.transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

        // 4. �Ѿ� ��������Ʈ�� ���⿡ �°� ȸ��
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // Z�� ȸ�� (2D)
                                     
        //���ݷ� ���� 
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && playerStat != null)
            bulletScript.Init(playerStat.attackDamage, 0); // percent 0 �Ǵ� �ʿ��� ��
    }
    public void ShootCounterBullets()
    {
        if (player.MP >= 100)
        {
            player.MP -= 100;
            Vector2[] directions = new Vector2[]
            {
                  Vector2.up,
                  Vector2.down,
                  Vector2.left,
                  Vector2.right,
                  new Vector2(1,1).normalized,
                  new Vector2(-1,1).normalized,
                  new Vector2(1,-1).normalized,
                  new Vector2(-1,-1).normalized,
            };

            foreach (Vector2 dir in directions)
            {
                GameObject bullet = Instantiate(preFabBullet, bulletPos.transform.position, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null && playerStat != null)
                    bulletScript.Init(playerStat.attackDamage, 0);
            }
        }
    }
}
