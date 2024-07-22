using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class difficultyBar : MonoBehaviour
{
    public Transform difficultyImg;
    public static int prev_level_difficulty = -1;
    //public Transform typeImg;

    private typeLevel type = typeLevel.precision;
    private GameObject currentSelected;

    private Dictionary<typeLevel, int> indexImgType = new Dictionary<typeLevel, int>()
    {
        {typeLevel.agilite, 2 },
        {typeLevel.endurance, 1 },
        {typeLevel.precision, 0 }
    };

    public void initBar(int _levelDifficulty, typeLevel _typeLevel, int numLevel)
    {
        StartCoroutine(animChangeLevel(_levelDifficulty, _typeLevel, numLevel));
    }

    private int idChange = 0; // +1 a chaque clique
    public void setInfos(int _levelDifficulty, typeLevel _typeLevel, int numLevel)
    {
        StartCoroutine(animChangeLevel(_levelDifficulty, _typeLevel, numLevel));
    }

    private IEnumerator animChangeLevel(int _levelDifficulty, typeLevel _typeLevel, int numLevel)
    {
        idChange++;

        /*if (anim)
        {
            difficultyGo.GetComponent<Animation>().Play("typeLevelPop");
            typeGo.gameObject.GetComponent<Animation>().Play("typeLevelPop");

            yield return new WaitForSeconds(1 / 6f);
        }*/
        yield return new WaitForSeconds(1 / 6f);

        if (_levelDifficulty - 1 != prev_level_difficulty)
        {
            if(prev_level_difficulty != -1)
                difficultyImg.GetChild(prev_level_difficulty + 1).gameObject.SetActive(false);
            prev_level_difficulty = _levelDifficulty - 1;// - 1;
            difficultyImg.GetChild(prev_level_difficulty + 1).gameObject.SetActive(true);
        }

        if (type != _typeLevel)
        {
            //typeImg.GetChild(indexImgType[type]).gameObject.SetActive(false);
            type = _typeLevel;
            //typeImg.GetChild(indexImgType[type]).gameObject.SetActive(true);
        }

        difficultyImg.GetChild(prev_level_difficulty + 1).GetChild(2) // progB img
    .GetComponent<Image>().fillAmount = SaveSystem.getProgLevel(numLevel, _levelDifficulty);
    }

    public GameObject difficultyGo, typeGo;
    public GameObject contourDifficulty, contourType;
    public Text infoDifficultyText, infoTypeText;
    private bool infoDifficultyOpen = false, infoTypeOpen = false;
    private Dictionary<int, string> infoLevel = new Dictionary<int, string>()
    {
        {0, "Facile" },
        {1, "Moyen" },
        {2, "Difficile" },
        {3, "Impossible" }
    };
    private Dictionary<typeLevel, string> infoType = new Dictionary<typeLevel, string>()
    {
        {typeLevel.agilite, "Agilité" },
        {typeLevel.endurance, "Endurance" },
        {typeLevel.precision, "Précision" }
    };

    public void btnInfoDifficulty()
    {
        if (!infoDifficultyOpen)
        {
            btnInfo(true);
        }
    }
    public void btnInfoType()
    {
        if (!infoTypeOpen)
        {
            btnInfo(false);
        }
    }

    private void btnInfo(bool difficulty)
    {
        Text infoText = difficulty ? infoDifficultyText : infoTypeText;
        //infoText.GetComponent<Animation>().Play("infoFade");
        (difficulty ? contourDifficulty : contourType).GetComponent<Animation>().Play("infoFade");

        if (difficulty)
            infoDifficultyOpen = true;
        else
            infoTypeOpen = true;

        //infoText.gameObject.SetActive(true);
        infoText.text = difficulty ? infoLevel[prev_level_difficulty] : infoType[type];
        StartCoroutine(closeInfoLate(difficulty, difficulty ? contourDifficulty : contourType));
    }

    private IEnumerator closeInfoLate(bool difficulty, GameObject toClose)
    {
        int id = idChange;
        float t = 0;
        while ( t < 2f && id == idChange)
        {
            yield return null;
            t += Time.deltaTime;
        }
        if(t < 105f/60f) // force close ( 11F / 12f : temps de l'anim ou se ferme automatiquement ) 
        {
            toClose.GetComponent<Animation>().Play("infoForceFade");
            yield return new WaitForSeconds(1f / 6f);
        }
        //yield return new WaitForSeconds(2f);

        //toClose.SetActive(false);
        if (difficulty)
            infoDifficultyOpen = false;
        else
            infoTypeOpen = false;
    }
}
