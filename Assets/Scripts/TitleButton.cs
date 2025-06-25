using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }
}
