using UnityEngine;

[CreateAssetMenu(menuName = "Item/Potion")]
public class PotionItem : ScriptableObject
{
    public string itemName;        // ���� �̸�
    [TextArea]
    public string description;     // ����
    public Sprite icon;            // ������

    public int healAmount;         // ȸ����
    public int manaAmount;         // ���� ȸ����
}
