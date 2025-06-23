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

    private void Start()
    {
        cam = Camera.main;

        if (player == null)
            player = FindObjectOfType<PlayerHpMp>();

        if (move == null)
            move = FindObjectOfType<Player>();
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
    }
}
