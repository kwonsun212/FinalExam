using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLevelUI : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat; // 레벨 정보를 담은 스크립트
    [SerializeField] private TextMeshProUGUI levelText;

    private void Update()
    {
        if (playerStat != null && levelText != null)
        {
            levelText.text = $"Lv. {playerStat.level}";
        }
    }
}
