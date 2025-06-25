using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeathPopupController : MonoBehaviour
{
    [Tooltip("��� �� ��� �г�")]
    public GameObject deathPanel;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    /// <summary>
    /// �÷��̾ ������� �� �ܺο��� ȣ��
    /// </summary>
    public void ShowDeathPopup()
    {
        StartCoroutine(DelayedShow());
    }

    private IEnumerator DelayedShow()
    {
        // Time.timeScale �� ���� 1�� ���¿��� 0.4�� ���
        yield return new WaitForSecondsRealtime(0.4f);

        // �� ���� �г� Ȱ��ȭ �� �Ͻ�����
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Button �� OnClick() �� ����
    /// </summary>
    public void OnReturnToMain()
    {
        Time.timeScale = 1f;                          // �Ͻ����� ����

        //��� �� JSON�� �����ϵ�, ���� ������ �״�� ����
        var playerHpMp = FindObjectOfType<PlayerHpMp>();
        if (playerHpMp != null)
        {
            playerHpMp.HP = Mathf.Clamp(playerHpMp.HP + 1000f, 0f, playerHpMp.MaxHP);
            playerHpMp.MP = Mathf.Clamp(playerHpMp.MP + 1000f, 0f, playerHpMp.MaxMP);

            // 2) ����� ��ġ ��� ����
            PlayerSaveManager.Save(playerHpMp);
        }

        // 3) Main ������ �̵�
        SceneManager.LoadScene("Main");
    }

    public void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
