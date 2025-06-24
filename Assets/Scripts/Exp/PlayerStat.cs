using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [SerializeField] private Slider expSlider; // ����ġ UI
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private GameObject levelUpTextPrefab;

    [SerializeField] private GameObject levelUpEffectPrefab;

    private void Start()
    {
        UpdateExpUI(); // ���� �� UI �ʱ�ȭ
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"����ġ +{amount} (���� ����ġ: {currentExp}/{expToNextLevel})");

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UpdateExpUI();
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel += 50; // ���̵� ���

        Debug.Log($"������! ���� ����: {level}");
        // ���⼭ ü�� ����, ���� ���� � �߰� ����


        // ������ ����Ʈ
        if (levelUpEffectPrefab != null)
        {
            GameObject effect = Instantiate(levelUpEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // ������ �ؽ�Ʈ
        if (levelUpTextPrefab != null)
        {
            GameObject textObj = Instantiate(levelUpTextPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            textObj.transform.SetParent(null); // ĵ������ World Space�̹Ƿ� ���� ������ ����
        }
    }

    void UpdateExpUI()
    {
        if (expSlider != null)
        {
            expSlider.value = (float)currentExp / expToNextLevel;
        }
        if (expText != null)
        {
            expText.text = $"{currentExp} / {expToNextLevel}";
        }
    }
}
