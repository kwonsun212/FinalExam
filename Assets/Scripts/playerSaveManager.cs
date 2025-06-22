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
        // ������ ������ ����
        PlayerData data = new PlayerData
        {
            hp = player.HP,
            mp = player.MP
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
    }


    //���� ���� ���� ����
    public static bool HasSaveData()
    {
        return File.Exists(SavePath);
    }
}
