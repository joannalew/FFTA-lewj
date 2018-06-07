using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour  {

    public AudioSource source;

    // Sound effects
    public AudioClip cursorMove;
    public AudioClip characterHit;
    public AudioClip attackMiss;
    public AudioClip mainSound;
    public AudioClip moveInBattleMenu;
    public AudioClip characterDeath;
    public AudioClip mainMenuSelection;
    public AudioClip battleMenuSelection;
    public AudioClip newPlayerTurn;


    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }


    public void playCursorSound()
    {
        source.clip = cursorMove;
        source.Play();
    }

    public void playCharacterHitSound()
    {
        source.clip = characterHit;
        source.Play();
    }

    public void playAttackMissSound()
    {
        source.clip = attackMiss;
        source.Play();
    }

    public void playMainSound()
    {
        source.clip = mainSound;
        source.Play();
    }

    public void playNewPlayerTurnSound()
    {
        source.clip = newPlayerTurn;
        source.Play();
    }

    public void playDeathSound()
    {
        source.clip = characterDeath;
        source.Play();
    }

    public void playMainMenuSelectionSound()
    {
        source.clip = mainMenuSelection;
        source.Play();
    }

    public void playMoveInBattleMenuSound()
    {
        source.clip = moveInBattleMenu;
        source.Play();
    }

    public void playBattleMenuSelectionSound()
    {
        source.clip = battleMenuSelection;
        source.Play();
    }
}
