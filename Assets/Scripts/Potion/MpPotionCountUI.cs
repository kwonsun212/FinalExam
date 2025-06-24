using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MpPotionCountUI : MonoBehaviour
{
    [SerializeField] private MpPotion potionScript;
    [SerializeField] private TextMeshProUGUI countText;

    void Update()
    {
        if (potionScript != null && countText != null)
        {
            countText.text = $"x{potionScript.MpPCount}";
        }
    }
}
