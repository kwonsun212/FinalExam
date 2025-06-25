using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;       // PlayerStat 참조
    [SerializeField] private TextMeshProUGUI goldText;    // GoldText 텍스트 참조

    private void Awake()
    {
        if (playerStat == null)
            playerStat = FindObjectOfType<PlayerStat>();
    }

    private void Update()
    {
        if (playerStat != null && goldText != null)
        {
            goldText.text = $"Gold: {playerStat.gold}";
        }
    }
}
