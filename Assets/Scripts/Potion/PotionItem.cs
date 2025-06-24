using UnityEngine;

[CreateAssetMenu(menuName = "Item/Potion")]
public class PotionItem : ScriptableObject
{
    public string itemName;        // 포션 이름
    [TextArea]
    public string description;     // 설명
    public Sprite icon;            // 아이콘

    public int healAmount;         // 회복량
    public int manaAmount;         // 마나 회복량
}
