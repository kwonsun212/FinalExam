using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EndButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        // ������ �÷��� ��� ����
        EditorApplication.isPlaying = false;
#else
        // ����� ���ø����̼� ����
        Application.Quit();
#endif
    }
}
