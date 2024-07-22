using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voyantBoostRanking : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public GameObject noBoostGo;
    public infoBoostGame[] boostGoByType;
    public Transform levelStarContent;

    private GameObject prevGoOpenned;
    private int prevLevelOpenned = -1;

    public void setInfoVoyant(typeBoostGame type, int levelBoost)
    {
        if (prevGoOpenned != null)
            prevGoOpenned.SetActive(false);
        if (prevLevelOpenned != -1)
            levelStarContent.GetChild(prevLevelOpenned - 1).gameObject.SetActive(false);

        if (type == typeBoostGame.none)
        {
            prevGoOpenned = null;
            prevLevelOpenned = -1;
        }
        else
        {           
            foreach (infoBoostGame info in boostGoByType)
            {
                if (info.type_boost == type)
                {
                    prevGoOpenned = info.boost_object;
                    prevGoOpenned.SetActive(true);
                    break;
                }
            }

            prevLevelOpenned = levelBoost;
            levelStarContent.GetChild(prevLevelOpenned - 1).gameObject.SetActive(true);
        }
    }
}
