using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {

    // updates left UI
    public void updateLeftUI(Character player, Character target, bool mov)
    {
        transform.Find("Portrait").GetComponent<Image>().sprite = AssetHolder.Instance.CharPortraits[player.id];
        transform.Find("currHp").GetComponent<Text>().text = player.currhpStat.ToString();
        transform.Find("maxHp").GetComponent<Text>().text = player.maxhpStat.ToString();
        transform.Find("currMp").GetComponent<Text>().text = player.currmpStat.ToString();
        transform.Find("maxMp").GetComponent<Text>().text = player.maxmpStat.ToString();
        transform.Find("charName").GetComponent<Text>().text = player.charName.ToString();

        if (!mov)
        {
            transform.Find("hitDmg").GetComponent<Text>().text = player.calcDamage(target).ToString();
            transform.Find("percentHit").GetComponent<Text>().text = player.calcHit(target).ToString();
        }

        transform.Find("charName").GetComponent<Text>().enabled = mov;
        transform.Find("Hit").GetComponent<Text>().enabled = !mov;
        transform.Find("Dmg").GetComponent<Text>().enabled = !mov;
        transform.Find("Percent").GetComponent<Text>().enabled = !mov;
        transform.Find("hitDmg").GetComponent<Text>().enabled = !mov;
        transform.Find("percentHit").GetComponent<Text>().enabled = !mov;
    }

    public void updateRightUI(Character target)
    {
        transform.Find("currHp").GetComponent<Text>().text = target.currhpStat.ToString();
        transform.Find("maxHp").GetComponent<Text>().text = target.maxhpStat.ToString();
        transform.Find("currMp").GetComponent<Text>().text = target.currmpStat.ToString();
        transform.Find("maxMp").GetComponent<Text>().text = target.maxmpStat.ToString();
    }

    // damage popup animation
    public void showDamage(Character actor, int dmg)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(new Vector2(actor.transform.position.x, actor.transform.position.y));
        transform.GetChild(0).position = screenPos;

        if (dmg == 0)
            GetComponentInChildren<Text>().text = "Miss!";
        else
            GetComponentInChildren<Text>().text = dmg.ToString();

        GetComponentInChildren<Animator>().SetTrigger("damageText");
    }
}
