using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpPotion : MonoBehaviour
{
    public PotionItem potionData;        // ����� ���� ������
    public int MpPCount = 3; // ���� ���� ����

    [SerializeField] private PlayerHpMp player;
    [SerializeField] private GameObject healEffectPrefab;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PlayerHpMp>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2�� Ű �Է� ������");
            UsePotion();
        }

        //4��Ű ������ ���� 1�� ȸ��
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MpPCount++;
        }
    }
    public void UsePotion()
    {
        if (player == null || player.isDead) return;

        if (MpPCount <= 0)
        {
            Debug.Log("������ �����ϴ�!");
            return;
        }

        if (potionData.manaAmount > 0)
            player.MP += potionData.manaAmount;

        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, player.transform.position, Quaternion.identity);
            // ����Ʈ�� �÷��̾� �ڽ����� ����
            effect.transform.SetParent(player.transform);
            Destroy(effect, 2f); // 2�� �� ����
        }


        MpPCount--;
        Debug.Log($"{potionData.itemName} ���: MP+{potionData.manaAmount}");
    }
}
