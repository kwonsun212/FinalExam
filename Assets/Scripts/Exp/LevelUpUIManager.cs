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
        hpButton.onClick.AddListener(() => SelectStat("hp"));
        mpButton.onClick.AddListener(() => SelectStat("mp"));
        speedButton.onClick.AddListener(() => SelectStat("speed"));
    }
    private void Start()
    {
        // 자동 연결
        playerStat = FindObjectOfType<PlayerStat>();
        playerHpMp = FindObjectOfType<PlayerHpMp>();
        player = FindObjectOfType<Player>();

        if (playerStat != null)
            playerStat.OnLevelUp += ShowLevelUpUI;
    }

    void ShowLevelUpUI()
    {
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
            case "hp":
                playerHpMp.HP += 100;
                break;
            case "mp":
                playerHpMp.MP += 50;
                break;
            case "speed":
                player.speed += 1f;
                break;
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
