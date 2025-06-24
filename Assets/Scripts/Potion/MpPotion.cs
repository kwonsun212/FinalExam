using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpPotion : MonoBehaviour
{
    public PotionItem potionData;        // 사용할 포션 데이터
    public int MpPCount = 3; // 현재 포션 개수

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
            Debug.Log("2번 키 입력 감지됨");
            UsePotion();
        }

        //4번키 누르면 포션 1개 회복
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
            Debug.Log("포션이 없습니다!");
            return;
        }

        if (potionData.manaAmount > 0)
            player.MP += potionData.manaAmount;

        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, player.transform.position, Quaternion.identity);
            // 이펙트를 플레이어 자식으로 설정
            effect.transform.SetParent(player.transform);
            Destroy(effect, 2f); // 2초 후 삭제
        }


        MpPCount--;
        Debug.Log($"{potionData.itemName} 사용: MP+{potionData.manaAmount}");
    }
}
