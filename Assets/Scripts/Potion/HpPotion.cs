using UnityEngine;

public class HpPotion : MonoBehaviour
{
    public PotionItem potionData;        // ����� ���� ������
    public int HpPCount = 3; // ���� ���� ����

    [SerializeField] private PlayerHpMp player;
    [SerializeField] private GameObject healEffectPrefab;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PlayerHpMp>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1�� Ű �Է� ������");
            UsePotion();
        }
        //4��Ű ������ ���� 1�� ȸ��
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HpPCount ++;
        }
    }
    public void UsePotion()
    {
        if (player == null || player.isDead) return;

        if (HpPCount <= 0)
        {
            Debug.Log("������ �����ϴ�!");
            return;
        }

        if (potionData.healAmount > 0)
            player.HP += potionData.healAmount;

        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, player.transform.position, Quaternion.identity);
            // ����Ʈ�� �÷��̾� �ڽ����� ����
            effect.transform.SetParent(player.transform);
            Destroy(effect, 2f); // 2�� �� ����
        }

        HpPCount--;
        // **����� ���� ���� ��� ����**
        PlayerSaveManager.Save(player);
        Debug.Log($"{potionData.itemName} ���: HP+{potionData.healAmount}");
    }
}
