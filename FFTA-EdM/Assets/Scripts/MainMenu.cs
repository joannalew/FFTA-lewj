using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static int level = 1;

    public void PlayLevel1()
    {
        level = 1;
        SceneManager.LoadScene("main");
    }

    public void PlayLevel2()
    {
        level = 2;
        SceneManager.LoadScene("main");
    }

    public void PlayLevel3()
    {
        level = 3;
        SceneManager.LoadScene("main");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
