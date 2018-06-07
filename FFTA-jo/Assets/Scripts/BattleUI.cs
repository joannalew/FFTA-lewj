﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {
    public int actMenuSelected = 0;             // current selected option
    private Text[] actOptions;                  // Text options
    private int numOptions = 4;                 // number of options
    private bool[] actTriggers;                 // selected previously or not (if can't select twice)
    private SoundEffects sfx_init;

    void Awake ()
    {
        actMenuSelected = 0;
        sfx_init = (SoundEffects)FindObjectOfType(typeof(SoundEffects));
    }

    void Start ()
    {
        actOptions = GetComponentsInChildren<Text>(true);
        actTriggers = new bool[numOptions];
        highlightOption(actMenuSelected);
    }

    public void selectNext()
    {
 //       sfx_init.playMainMenuSelectionSound();
        unhighlightOption(actMenuSelected);

        do
        {
            actMenuSelected++;
            if (actMenuSelected == numOptions)
                actMenuSelected = 0;
        } while (actTriggers[actMenuSelected]);

        highlightOption(actMenuSelected);
    }

    public void selectPrev()
    {
 //       sfx_init.playMainMenuSelectionSound();
        unhighlightOption(actMenuSelected);

        actMenuSelected--;
        if (actMenuSelected == -1)
            actMenuSelected = numOptions - 1;

        highlightOption(actMenuSelected);
    }

    public void selectOption()
    {
//        sfx_init.playBattleMenuSelectionSound();
        actTriggers[actMenuSelected] = true;
        actOptions[actMenuSelected].color = Color.grey;
        actOptions[actMenuSelected].GetComponent<Outline>().effectColor = Color.black;
    }

    public void highlightOption(int optionID)
    {
        sfx_init.playMoveInBattleMenuSound();
        if (!actTriggers[optionID])
        {
            actOptions[optionID].color = Color.yellow;
            actOptions[optionID].GetComponent<Outline>().effectColor = Color.magenta;
        }
    }

    public void unhighlightOption(int optionID)
    {
        if (!actTriggers[optionID])
        {
            actOptions[optionID].color = Color.white;
            actOptions[optionID].GetComponent<Outline>().effectColor = Color.black;
        }
    }

    public void resetMenu()
    {
        unhighlightOption(actMenuSelected);
        actMenuSelected = 0;
        
        while (actTriggers[actMenuSelected])
            actMenuSelected++;

        highlightOption(actMenuSelected);
    }

    public void resetOptions()
    {
        for (int i = 0; i < numOptions; i++)
        {
            actTriggers[i] = false;
            unhighlightOption(i);
        }
    }

    public void setNumOptions(int num)
    {
        numOptions = num;
    }
}
