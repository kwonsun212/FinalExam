using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeathPopupController : MonoBehaviour
{
    [Tooltip("사망 시 띄울 패널")]
    public GameObject deathPanel;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    /// <summary>
    /// 플레이어가 사망했을 때 외부에서 호출
    /// </summary>
    public void ShowDeathPopup()
    {
        StartCoroutine(DelayedShow());
    }

    private IEnumerator DelayedShow()
    {
        // Time.timeScale 이 아직 1인 상태에서 0.4초 대기
        yield return new WaitForSecondsRealtime(0.4f);

        // 그 다음 패널 활성화 및 일시정지
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Button → OnClick() 에 연결
    /// </summary>
    public void OnReturnToMain()
    {
        Time.timeScale = 1f;                          // 일시정지 해제

        //사망 시 JSON을 리셋하되, 포션 개수는 그대로 유지
        var playerHpMp = FindObjectOfType<PlayerHpMp>();
        if (playerHpMp != null)
        {
            playerHpMp.HP = Mathf.Clamp(playerHpMp.HP + 1000f, 0f, playerHpMp.MaxHP);
            playerHpMp.MP = Mathf.Clamp(playerHpMp.MP + 1000f, 0f, playerHpMp.MaxMP);

            // 2) 변경된 수치 즉시 저장
            PlayerSaveManager.Save(playerHpMp);
        }

        // 3) Main 씬으로 이동
        SceneManager.LoadScene("Main");
    }

    public void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
