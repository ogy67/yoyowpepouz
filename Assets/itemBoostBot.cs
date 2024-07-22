using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemBoostBot : MonoBehaviour
{
    public botInfoCard myCard;

    private int boostIndex_ref = -1, levelIndex_ref = -1;

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


    private int prevIndexOpenned_boost = -1;
    private int maxIndex_boost = 11;
    public Transform boostContent;
    public GameObject btnRight_boost_go, btnLeft_boost_go;
    public GameObject btnRight_level_go, btnLeft_level_go;
    public void btnRight_boost()
    {
        if (prevIndexOpenned_boost < maxIndex_boost)
        {
            if (prevIndexOpenned_boost != -1)
                boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(false);
            prevIndexOpenned_boost++;
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(true);
            boostIndexChanged();
            if (!btnLeft_boost_go.activeSelf)
                btnLeft_boost_go.SetActive(true);
            if(prevIndexOpenned_boost == maxIndex_boost)
                btnRight_boost_go.SetActive(false);
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
            if (!btnRight_boost_go.activeSelf)
                btnRight_boost_go.SetActive(true);
        }
        else if (prevIndexOpenned_boost == 0)
        {
            boostContent.GetChild(0).gameObject.SetActive(false);
            prevIndexOpenned_boost = -1;
            boostIndexChanged();
            btnLeft_boost_go.SetActive(false);
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

    private Dictionary<int, int> indexByBoostId = new Dictionary<int, int>()
    {
        {0, -1},
        {1, 0},
        {2, 1},
        {3, 2},
        {11, 3},
        {12, 4},
        {13, 5},
        {21, 6},
        {22, 7},
        {23, 8},
        {31, 9},
        {32, 10},
        {33, 11},
    };

    public int getIdBoost()
    {
        return boostIdByIndex[prevIndexOpenned_boost];
    }

    //private int currentLevelBoost = 1;
    private int maxLevelBoost = 5;
    private int prevIndexOpenned_level = -1;
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

            if (!btnLeft_level_go.activeSelf)
                btnLeft_level_go.SetActive(true);
            if(prevIndexOpenned_level == maxLevelBoost - 1)
                btnRight_level_go.SetActive(false);
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
            if (!btnRight_level_go.activeSelf)
                btnRight_level_go.SetActive(true);
        }
        else if (prevIndexOpenned_level == 0)
        {
            levelContent.GetChild(0).gameObject.SetActive(false);
            prevIndexOpenned_level = -1;
            levelIndexChanged();
            btnLeft_level_go.SetActive(false);
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

    private Dictionary<int, int> indexByLevel = new Dictionary<int, int>()
    {
        { 0, -1},
        { 1, 0 },
        { 2, 1 },
        { 3, 2 },
        { 4, 3 },
        { 5, 4 }
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

    public void loadBoostWithSavedInfo(string info, bool bdd)
    {
        string[] tab = info.Split(',');

        //if(prevIndexOpenned_level != -1)
        //    boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(false);
        prevIndexOpenned_boost = bdd ? indexByBoostId[int.Parse(tab[0])] : int.Parse(tab[0]);
        if(prevIndexOpenned_boost != -1)
            boostContent.GetChild(prevIndexOpenned_boost).gameObject.SetActive(true);
        boostIndex_ref = prevIndexOpenned_boost;

        //levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(false);
        prevIndexOpenned_level = bdd ? indexByLevel[int.Parse(tab[1])] : int.Parse(tab[1]);
        if (prevIndexOpenned_level != -1)
            levelContent.GetChild(prevIndexOpenned_level).gameObject.SetActive(true);
        levelIndex_ref = prevIndexOpenned_level;

        // on doit check quels boutons droite et gauche sont activés
        btnRight_boost_go.SetActive(prevIndexOpenned_boost < maxIndex_boost);
        btnLeft_boost_go.SetActive(prevIndexOpenned_boost > -1);
        btnRight_level_go.SetActive(prevIndexOpenned_level < maxLevelBoost - 1);
        btnLeft_level_go.SetActive(prevIndexOpenned_level > 0);

    }



}
