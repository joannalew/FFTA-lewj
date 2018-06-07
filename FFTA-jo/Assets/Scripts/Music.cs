using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioSource source;

    // Music
    public AudioClip herbPickingBattleMusic;
    public AudioClip snowInLutiaBattleMusic;
    public AudioClip lutia2BattleMusic;
    public AudioClip mainMenuMusic;


    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void playL1Music()
    {
        source.loop = true;
        source.clip = herbPickingBattleMusic;
        source.Play();
    }

    public void playL2Music()
    {
        source.loop = true;
        source.clip = snowInLutiaBattleMusic;
        source.Play();
    }

    public void playL3Music()
    {
        source.loop = true;
        source.clip = lutia2BattleMusic;
        source.Play();
    }

    public void playMainMenuMusic()
    {
        source.loop = true;
        source.clip = mainMenuMusic;
        source.Play();
    }

    public void stopMainMenuMusic()
    {
        source.Stop();
    }
}
