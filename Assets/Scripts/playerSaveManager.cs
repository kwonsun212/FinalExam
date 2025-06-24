using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class PlayerSaveManager
{

    // 저장 파일의 경로
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_data.json");


    // 플레이어 데이터를 JSON 형식으로 저장
    public static void Save(PlayerHpMp player)
    {
        HpPotion hpPotion = GameObject.FindObjectOfType<HpPotion>();
        MpPotion mpPotion = GameObject.FindObjectOfType<MpPotion>();

        // 저장할 데이터 생성
        PlayerData data = new PlayerData
        {
            hp = player.HP,
            mp = player.MP,
            hpPotionCount = hpPotion != null ? hpPotion.HpPCount : 0,
            mpPotionCount = mpPotion != null ? mpPotion.MpPCount : 0
        };


        // JSON 문자열로 변환 (true = 예쁘게 포맷팅)
        string json = JsonUtility.ToJson(data, true);

        // 파일로 저장
        File.WriteAllText(SavePath, json);
    }

    public static void Load(PlayerHpMp player)
    {
        //저장 파일 없으면 리턴
        if (!File.Exists(SavePath)) return;

        // JSON 파일 읽기
        string json = File.ReadAllText(SavePath);
        // JSON → PlayerData 객체로 변환
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        // 불러온 값을 플레이어에 적용
        player.HP = data.hp;
        player.MP = data.mp;

        HpPotion hpPotion = GameObject.FindObjectOfType<HpPotion>();
        MpPotion mpPotion = GameObject.FindObjectOfType<MpPotion>();

        if (hpPotion != null)
            hpPotion.HpPCount = data.hpPotionCount;

        if (mpPotion != null)
            mpPotion.MpPCount = data.mpPotionCount;
    }


    //저장 파일 존재 여부
    public static bool HasSaveData()
    {
        return File.Exists(SavePath);
    }
}
