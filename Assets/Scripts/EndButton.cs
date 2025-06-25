using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EndButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 에디터 플레이 모드 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드된 어플리케이션 종료
        Application.Quit();
#endif
    }
}
