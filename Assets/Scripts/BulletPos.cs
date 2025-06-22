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

    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(ShootSequence());
        }
    }


    IEnumerator ShootSequence()
    {

        canShoot = false;

        yield return new WaitForSeconds(0.5f);
        if (!player.isDead && player.MP >= 15)
        {
            Shoot();
            player.MP -= 15;
        }

        yield return new WaitForSeconds(0.4f); // 0.9초까지 기다리도록 (0.5 + 0.4)
        if (!player.isDead && player.MP >= 15)
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
    }
}
