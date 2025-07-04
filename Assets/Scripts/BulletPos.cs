using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPos : MonoBehaviour
{
    public GameObject bulletPos;        // 총알 생성 위치 (빈 오브젝트)
    public GameObject preFabBullet;     // 총알 프리팹
    public float bulletSpeed = 10f;     // 발사 속도


    Camera cam;         //메인카메라

    private bool canShoot = true;


    [SerializeField] private PlayerHpMp player; // MP를 줄이기 위한 참조
    [SerializeField] private Player move; // 슬라이딩 상채 확인
    [SerializeField] private PlayerStat playerStat;  // 공격력 참조

    private void Start()
    {
        cam = Camera.main;

        if (player == null)
            player = FindObjectOfType<PlayerHpMp>();

        if (move == null)
            move = FindObjectOfType<Player>();

        //자동 연결
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
        // 1. 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // 2D 게임이므로 z=0 고정

        // 2. 발사 방향 계산
        Vector3 direction = (mouseWorldPos - bulletPos.transform.position).normalized;

        // 3. 총알 생성 및 발사
        GameObject bullet = Instantiate(preFabBullet, bulletPos.transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);

        // 4. 총알 스프라이트를 방향에 맞게 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // Z축 회전 (2D)
                                     
        //공격력 적용 
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null && playerStat != null)
            bulletScript.Init(playerStat.attackDamage, 0); // percent 0 또는 필요한 값
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
