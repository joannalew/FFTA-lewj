using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {
    public int actMenuSelected = 0;
    private Text[] actOptions;
    private int numOptions = 4;

    // Use this for initialization
    void Start () {
        actMenuSelected = 0;
        actOptions = GetComponentsInChildren<Text>(true);
        highlightOption(actMenuSelected);
    }

    public void selectNext()
    {
        unhighlightOption(actMenuSelected);

        actMenuSelected++;
        if (actMenuSelected == numOptions)
            actMenuSelected = 0;

        highlightOption(actMenuSelected);
    }

    public void selectPrev()
    {
        unhighlightOption(actMenuSelected);

        actMenuSelected--;
        if (actMenuSelected == -1)
            actMenuSelected = numOptions - 1;

        highlightOption(actMenuSelected);
    }

    public void highlightOption(int optionID)
    {
        actOptions[optionID].color = Color.yellow;
        actOptions[optionID].GetComponent<Outline>().effectColor = Color.magenta;
    }

    public void unhighlightOption(int optionID)
    {
        actOptions[optionID].color = Color.white;
        actOptions[optionID].GetComponent<Outline>().effectColor = Color.black;
    }

    public void resetMenu()
    {
        if (actMenuSelected != 0)
        {
            unhighlightOption(actMenuSelected);
            actMenuSelected = 0;
            highlightOption(actMenuSelected);
        }
    }

    public void setNumOptions(int num)
    {
        numOptions = num;
    }
}
