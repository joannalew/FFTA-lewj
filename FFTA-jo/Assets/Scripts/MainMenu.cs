using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static int level = 1;
    private SoundEffects sfx_init;
    private Music music_init;

    private void Awake()
    {
        sfx_init = (SoundEffects)FindObjectOfType(typeof(SoundEffects));
        music_init = (Music)FindObjectOfType(typeof(Music));
    }

    private void Start()
    {
        music_init.playMainMenuMusic();
    }

    public void PlayLevel1()
    {
        level = 1;
        sfx_init.playMainMenuSelectionSound();
        SceneManager.LoadScene("main");
    }

    public void PlayLevel2()
    {
        level = 2;
        sfx_init.playMainMenuSelectionSound();
        SceneManager.LoadScene("main");
    }

    public void PlayLevel3()
    {
        level = 3;
        sfx_init.playMainMenuSelectionSound();
        SceneManager.LoadScene("main");
    }

    public void Quit()
    {
        sfx_init.playMainMenuSelectionSound();
        Application.Quit();
        Debug.Log("Quit!");
    }
}
