using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntry : MonoBehaviour
{
    [Tooltip("�˾� �г��� �巡���ϼ���")]
    public GameObject popupPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player �±װ� �ε�����
        if (collision.CompareTag("Player"))
        {
            popupPanel.SetActive(true);
            // ���� �Ͻ�����(����)
            Time.timeScale = 0f;
            // �� ���� Ʈ�����ϰ� �ʹٸ�:
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
