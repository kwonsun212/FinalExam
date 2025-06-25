using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupController : MonoBehaviour
{
    // “간다” 버튼에 연결
    public void OnYes()
    {
        Time.timeScale = 1f;                      // 일시정지 해제
        SceneManager.LoadScene("Dungeon_1");      // 씬 전환
    }

    // “안 간다” 버튼에 연결
    public void OnNo()
    {
        Time.timeScale = 1f;                      // 일시정지 해제
        gameObject.SetActive(false);              // 팝업 닫기
    }
}
