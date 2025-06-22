using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPos : MonoBehaviour
{
    public GameObject bulletPos;        // �Ѿ� ���� ��ġ (�� ������Ʈ)
    public GameObject preFabBullet;     // �Ѿ� ������
    public float bulletSpeed = 10f;     // �߻� �ӵ�


    Camera cam;         //����ī�޶�

    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootSequence());
        }
    }


    IEnumerator ShootSequence()
    {
        yield return new WaitForSeconds(0.5f);
        Shoot();

        yield return new WaitForSeconds(0.4f); // 0.9�ʱ��� ��ٸ����� (0.5 + 0.4)
        Shoot();
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

    }
}
