using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupController : MonoBehaviour
{
    // �����١� ��ư�� ����
    public void OnYes()
    {
        Time.timeScale = 1f;                      // �Ͻ����� ����
        SceneManager.LoadScene("Dungeon_1");      // �� ��ȯ
    }

    // ���� ���١� ��ư�� ����
    public void OnNo()
    {
        Time.timeScale = 1f;                      // �Ͻ����� ����
        gameObject.SetActive(false);              // �˾� �ݱ�
    }
}
