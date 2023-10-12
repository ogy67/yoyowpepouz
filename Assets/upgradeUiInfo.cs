using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct upInfo
{
    public typeUpgrade uPInfo;
    public int index;
}


[System.Serializable]
public struct colorByTypeText
{
    public colorText cT;
    public string color;
}
public enum colorText
{
    red,
    yellow,
    finished,
    none
}

public class upgradeUiInfo : MonoBehaviour
{
    public Text nomText;
    public Text currentUpText;
    public Text upgradeValueText;
    public Transform imageUpContent;

    public colorByTypeText[] colorByType;
    public upInfo[] info;

    private int getRangUpgrade(typeUpgrade tU)
    {
        foreach(upInfo uI in info)
        {
            if (uI.uPInfo == tU)
                return uI.index;
        }
        return 0;
    }

    public GameObject itemOn, itemOff;
    public void setItemState(bool isOn)
    {
        itemOn.SetActive(isOn);
        itemOff.SetActive(!isOn);
    }

    public typeUpgrade myType;
    public float currVal, addedVal;
    public void setUpgrade(typeUpgrade tU, string nom, string upCurrent, string upNext, colorText cT
        , int levelMax, int currentLevel, bool willUpgrade
        , float upCurrentVal, float upAddedVal)
    {
        currVal = upCurrentVal;
        addedVal = upAddedVal;// upAddedVal;
        myType = tU;

        foreach (Transform t in imageUpContent)
            t.gameObject.SetActive(false);


        nomText.text = nom;
        /*string s_next = cT != colorText.finished ? "<size=18><color=" + getColorByType(cT)
            + ">" + upNext + "</color></size>" : "";*/
        string s_next = cT != colorText.finished ? "<color=" + getColorByType(cT)
            + ">" + upNext + "</color>" : "";

        currentUpText.text = upCurrent;// + "\n" + s_next;
        upgradeValueText.text = s_next;
        imageUpContent.GetChild(getRangUpgrade(tU)).gameObject.SetActive(true);

        setProgBarUpgrades(levelMax, currentLevel, willUpgrade);
    }

    private string getColorByType(colorText cT)
    {
        foreach (colorByTypeText cBTT in colorByType)
            if (cBTT.cT == cT)
                return cBTT.color;
        return "";
    }


    public Image[] progBarImages;
    public Color itemProgB_on, itemProgB_off, itemProgB_passed, itemProgB_invisible;
    public GameObject fondUpOn, fondUpOff;
    // willUpgrade : si la caractéristique va s'améliorer
    private void setProgBarUpgrades(int levelProgMax, int currentLevel, bool willUpgrade)
    {
        //Debug.Log("LevelProgMax : " + levelProgMax + " , currentLevel : " + currentLevel);
        // /!\ PB : paramètres doivent être
        // nbUpgradesMax
        // currentNbUpgrades


        // si est level 1 , illuminer item index 0, reste off
        // si est level 5 , illuminer tout les items

        // afficher autant d'item que levelProgMax - 1
        // -> lvl 5 max, afficher 4 items

        

        bool boostIsMax = currentLevel == levelProgMax - 1;

        bool fondOn = boostIsMax || willUpgrade;
        fondUpOn.SetActive(fondOn);
        fondUpOff.SetActive(!fondOn);

        Animation a;
        for (int i = 0; i < 5; i++)
        {
            a = progBarImages[i].GetComponent<Animation>();
            if (a.isPlaying)
                a.Stop();
        }

        for (int i = 0; i < 5; i++)
        {
            progBarImages[i].gameObject.SetActive(i < levelProgMax);
            if (boostIsMax)
                progBarImages[i].color = itemProgB_on;
            else
            {
                if (currentLevel > i)
                    progBarImages[i].color = itemProgB_passed;
                else if (currentLevel == i)
                {
                    progBarImages[i].color = itemProgB_passed;
                    if (willUpgrade)
                        progBarImages[i + 1].GetComponent<Animation>().Play("progBarItemOn");
                    // prev
                    //progBarImages[i].color = willUpgrade ? itemProgB_on : itemProgB_passed;
                }
                else
                    progBarImages[i].color = itemProgB_off;
            }
        }
    }
}
