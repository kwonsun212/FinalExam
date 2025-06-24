using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class PlayerSaveManager
{

    // ���� ������ ���
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_data.json");


    // �÷��̾� �����͸� JSON �������� ����
    public static void Save(PlayerHpMp player)
    {
        HpPotion hpPotion = GameObject.FindObjectOfType<HpPotion>();
        MpPotion mpPotion = GameObject.FindObjectOfType<MpPotion>();

        // ������ ������ ����
        PlayerData data = new PlayerData
        {
            hp = player.HP,
            mp = player.MP,
            hpPotionCount = hpPotion != null ? hpPotion.HpPCount : 0,
            mpPotionCount = mpPotion != null ? mpPotion.MpPCount : 0
        };


        // JSON ���ڿ��� ��ȯ (true = ���ڰ� ������)
        string json = JsonUtility.ToJson(data, true);

        // ���Ϸ� ����
        File.WriteAllText(SavePath, json);
    }

    public static void Load(PlayerHpMp player)
    {
        //���� ���� ������ ����
        if (!File.Exists(SavePath)) return;

        // JSON ���� �б�
        string json = File.ReadAllText(SavePath);
        // JSON �� PlayerData ��ü�� ��ȯ
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        // �ҷ��� ���� �÷��̾ ����
        player.HP = data.hp;
        player.MP = data.mp;

        HpPotion hpPotion = GameObject.FindObjectOfType<HpPotion>();
        MpPotion mpPotion = GameObject.FindObjectOfType<MpPotion>();

        if (hpPotion != null)
            hpPotion.HpPCount = data.hpPotionCount;

        if (mpPotion != null)
            mpPotion.MpPCount = data.mpPotionCount;
    }


    //���� ���� ���� ����
    public static bool HasSaveData()
    {
        return File.Exists(SavePath);
    }
}
