using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIManager : MonoBehaviour
{
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Button atkButton;
    [SerializeField] private Button hpButton;
    [SerializeField] private Button mpButton;
    [SerializeField] private Button speedButton;

    private PlayerStat playerStat;
    private PlayerHpMp playerHpMp;
    private Player player;


    private void Awake()
    {
        levelUpPanel.SetActive(false);

        atkButton.onClick.AddListener(() => SelectStat("atk"));
        hpButton.onClick.AddListener(() => SelectStat("hpBonus"));
        mpButton.onClick.AddListener(() => SelectStat("mpBonus"));
        speedButton.onClick.AddListener(() => SelectStat("speed"));
    }
    private void Start()
    {
        // 자동 연결
        playerStat = FindObjectOfType<PlayerStat>();
        playerHpMp = FindObjectOfType<PlayerHpMp>();
        player = FindObjectOfType<Player>();

        if (playerStat != null)
            playerStat.OnLevelUp += () => StartCoroutine(ShowLevelUpUI());
    }

    IEnumerator ShowLevelUpUI()
    {
        yield return new WaitForSecondsRealtime(0.4f); // 0.4초 기다림
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);
    }

    void SelectStat(string type)
    {
        switch (type)
        {
            case "atk":
                playerStat.attackDamage += 50f;
                break;
            case "hpBonus":
                playerHpMp.hpBonus += 100;
                break;
            case "mpBonus":
                playerHpMp.mpBonus += 50;
                break;
            case "speed":
                player.speed += 1f;
                break;
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
