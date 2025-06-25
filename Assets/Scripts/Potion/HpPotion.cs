using UnityEngine;

public class HpPotion : MonoBehaviour
{
    public PotionItem potionData;        // 사용할 포션 데이터
    public int HpPCount = 3; // 현재 포션 개수

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
            Debug.Log("1번 키 입력 감지됨");
            UsePotion();
        }
        //4번키 누르면 포션 1개 회복
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
            Debug.Log("포션이 없습니다!");
            return;
        }

        if (potionData.healAmount > 0)
            player.HP += potionData.healAmount;

        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, player.transform.position, Quaternion.identity);
            // 이펙트를 플레이어 자식으로 설정
            effect.transform.SetParent(player.transform);
            Destroy(effect, 2f); // 2초 후 삭제
        }

        HpPCount--;
        // **변경된 포션 개수 즉시 저장**
        PlayerSaveManager.Save(player);
        Debug.Log($"{potionData.itemName} 사용: HP+{potionData.healAmount}");
    }
}
