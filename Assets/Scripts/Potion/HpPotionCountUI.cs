using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpPotionCountUI : MonoBehaviour
{
    [SerializeField] private HpPotion potionScript;
    [SerializeField] private TextMeshProUGUI countText;

    void Update()
    {
        if (potionScript != null && countText != null)
        {
            countText.text = $"x{potionScript.HpPCount}";
        }
    }
}
