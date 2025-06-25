using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntry : MonoBehaviour
{
    [Tooltip("팝업 패널을 드래그하세요")]
    public GameObject popupPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player 태그가 부딪히면
        if (collision.CompareTag("Player"))
        {
            popupPanel.SetActive(true);
            // 게임 일시정지(선택)
            Time.timeScale = 0f;
            // 한 번만 트리거하고 싶다면:
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
