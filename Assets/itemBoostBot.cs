using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemBoostBot : MonoBehaviour
{
    public botInfoCard myCard;

    private int boostIndex_ref = 0, levelIndex_ref = 0;

    public void resetBoostUpdate()
    {
        boostIndex_ref = prevIndexOpenned_boost;
        levelIndex_ref = prevIndexOpenned_level;
    }

    private void boostIndexChanged()
    {
        bool initState = prevIndexOpenned_boost == boostIndex_ref;
        myCard.itemBoostValueChanged(initState, GetComponent<itemBoostBot>());
    }
    private void levelIndexChanged()
    {
        bool initState = prevIndexOpenned_level == levelIndex_ref;
        myCard.itemBoostValueChanged(initState, GetComponent<itemBoostBot>());
    }


    private int prevIndexOpenned_boost = 0;
    private int maxIndex_boost = 11;
    public Transform boostContent;
    public void btnRight_boost()
    {
        if (prevIndexOpenned_boost < maxIndex_boost)
        {
            if (prevIndexOpenned_boost != -1)
                boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(false);
            prevIndexOpenned_boost++;
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(true);
            boostIndexChanged();
        }
    }

    public void btnLeft_boost()
    {
        if (prevIndexOpenned_boost > 0)
        {
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(false);
            prevIndexOpenned_boost--;
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(true);
            boostIndexChanged();
        }
        else if (prevIndexOpenned_boost == 0)
        {
            boostContent.GetChild(0).gameObject.SetActive(false);
            prevIndexOpenned_boost = -1;
            boostIndexChanged();
        }
    }

    private Dictionary<int, int> boostIdByIndex = new Dictionary<int, int>()
    {
        {-1, 0},
        {0, 1},
        {1, 2},
        {2, 3},
        {3, 11},
        {4, 12},
        {5, 13},
        {6, 21},
        {7, 22},
        {8, 23},
        {9, 31},
        {10, 32},
        {11, 33},
    };

    public int getIdBoost()
    {
        return boostIdByIndex[prevIndexOpenned_boost];
    }

    //private int currentLevelBoost = 1;
    private int maxLevelBoost = 5;
    private int prevIndexOpenned_level = 0;
    public Transform levelContent;
    public void btnRight_level()
    {
        if (prevIndexOpenned_level < maxLevelBoost - 1)
        {
            if (prevIndexOpenned_level != -1)
                levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(false);
            prevIndexOpenned_level++;
            levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(true);
            levelIndexChanged();
        }
    }

    public void btnLeft_level()
    {
        if (prevIndexOpenned_level > 0)
        {
            levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(false);
            prevIndexOpenned_level--;
            levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(true);
            levelIndexChanged();
        }
        else if (prevIndexOpenned_level == 0)
        {
            levelContent.GetChild(0).gameObject.SetActive(false);
            prevIndexOpenned_level = -1;
            levelIndexChanged();
        }
    }

    private Dictionary<int, int> levelByIndex = new Dictionary<int, int>()
    {
        {-1, 0},
        {0, 1},
        {1, 2},
        {2, 3},
        {3, 4},
        {4, 5}
    };
    public int getLevelBoost()
    {
        return levelByIndex[prevIndexOpenned_level];
    }


    public int getIdBoostUsed()
    {
        return boostIdByIndex[prevIndexOpenned_boost];
    }

    public string itemBoostToStringSaveLocal()
    {
        return prevIndexOpenned_boost + "," + prevIndexOpenned_level;
    }

    public void loadBoostWithSavedInfo(string info)
    {
        string[] tab = info.Split(',');

        boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(false);
        prevIndexOpenned_boost = int.Parse(tab[0]);
        //Debug.Log("Prev index : " + prevIndexOpenned_boost);
        if(prevIndexOpenned_boost != -1)
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(true);
        boostIndex_ref = prevIndexOpenned_boost;

        levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(false);
        prevIndexOpenned_level = int.Parse(tab[1]);
        if (prevIndexOpenned_level != -1)
            levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(true);
        levelIndex_ref = prevIndexOpenned_level;
    }

}
