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

    [SerializeField] private Slider expSlider; // 경험치 UI
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private GameObject levelUpTextPrefab;

    [SerializeField] private GameObject levelUpEffectPrefab;

    public delegate void OnLevelUpDelegate();
    public event OnLevelUpDelegate OnLevelUp;

    public float attackDamage = 50f;

    [Header("Currency")]
    public int gold = 0;        // 보유 골드

    // 골드 획득
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"골드 +{amount} (현재 골드: {gold})");
    }

    private void Start()
    {
        UpdateExpUI(); // 시작 시 UI 초기화
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 +{amount} (현재 경험치: {currentExp}/{expToNextLevel})");

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
        expToNextLevel += 50; // 난이도 상승

        Debug.Log($"레벨업! 현재 레벨: {level}");
        // 여기서 체력 증가, 스탯 증가 등도 추가 가능


        // 레벨업 이펙트
        if (levelUpEffectPrefab != null)
        {
            GameObject effect = Instantiate(levelUpEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 레벨업 텍스트
        if (levelUpTextPrefab != null)
        {
            GameObject textObj = Instantiate(levelUpTextPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            textObj.transform.SetParent(null); // 캔버스는 World Space이므로 따로 붙이지 않음
        }

        //이벤트 호출
        OnLevelUp?.Invoke();

        // 경험치 UI 갱신
        UpdateExpUI();
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
