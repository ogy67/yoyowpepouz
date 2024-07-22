using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct typeRaretyToGameObj
{
    public typeRarety type;
    public GameObject itemAssociated;
    public GameObject hiddenImg;
}
public class boostRewardAnim : MonoBehaviour
{
    public bool test_btnLoadDaily = false;
    public bool test_btnGetDailyReward = false;
    public bool test_btnCloseDailyReward = false;
    public bool test_isAlreadyTaken = false;
    public typeBoostGame test_typeBoostDaily;
    // Update is called once per frame
    void Update()
    {
        if (test_btnLoadDaily)
        {
            test_btnLoadDaily = false;
            loadDailyOffer(test_typeBoostDaily, test_isAlreadyTaken);
        }
        if (test_btnGetDailyReward)
        {
            test_btnGetDailyReward = false;
            modeMenuDaily_close();
        }
        if (test_btnCloseDailyReward)
        {
            test_btnCloseDailyReward = false;
            modeMenuDaily_reopen();
        }
    }

    public Animation animDaily;
    // seulement pour item dans affichage daily
    public void loadDailyOffer(typeBoostGame type, bool alreadyTaken)
    {
        setInfosAnim(type, true);
        if (alreadyTaken)
        {
            currentRaretyHidden.gameObject.SetActive(false);
            currentBoostItemOpen.gameObject.SetActive(true);
        }
    }
    // fermer '?' quand openRecompense
    public void modeMenuDaily_close()
    {
        // anim '?' disparaît
        animDaily.Play("boostDailyClose");
    }
    // ré-ouvrir avec item affiché quand closeRecompense
    public void modeMenuDaily_reopen()
    {
        // '?' -> imgBoostReward
        currentRaretyHidden.gameObject.SetActive(false);
        currentBoostItemOpen.gameObject.SetActive(true);
        // anim boostReward apparaît
        animDaily.Play("boostDailyReopen");
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }



    private bool booster = false;

    private GameObject currentBoostItemOpen;
    private GameObject currentRaretyOpen;
    private GameObject currentRaretyHidden;
    public Animation automaticAnim; // anim bounce mystery
    public void popDaily(typeBoostGame type)
    {
        booster = false;
        setInfosAnim(type, true);
        automaticAnim.playAutomatically = true;
    }


    private void setInfosAnim(typeBoostGame type, bool daily)
    {
        // init
        typeRarety tR = typeBoostToTypeRarety[type];
        if (currentBoostItemOpen != null)
        {
            currentBoostItemOpen.SetActive(false);
            currentRaretyOpen.SetActive(false);
            currentRaretyHidden.SetActive(false);
        }
        // on les ouvre au moment de anim reveal
        menuGetBoost.getInstance().revealNewText();
        currentBoostItemOpen = getBoostImgAssociated(type);
        currentRaretyOpen = getRaretyImgAssociated(tR);
        currentRaretyHidden = getRaretyHiddenAssociated(tR);

        currentRaretyOpen.SetActive(true);
        currentRaretyHidden.SetActive(daily);
        currentBoostItemOpen.SetActive(!daily);
    }

    public Animation myAnim;
    public void switchDailyToReward()
    {
        StartCoroutine(animSwitch());
    }

    public void switchToNextBoost()
    {
        myAnim.Play("boostRewardReveal");
        //StartCoroutine(animSwitch(false));
    }
    private IEnumerator animSwitch()
    {
        yield return new WaitForSeconds(13 / 60f);
        currentRaretyHidden.gameObject.SetActive(false);
        currentBoostItemOpen.gameObject.SetActive(true);
    }



    public typeRarety getTypeRaretyByTypeBoost(typeBoostGame tB)
    {
        return typeBoostToTypeRarety[tB];
    }

    // l'img boost est cash affiché, pas '?'
    public void popBooster(typeBoostGame type)
    {
        booster = true;
        setInfosAnim(type, false);
        automaticAnim.playAutomatically = false;
    }

    public void nextBoostItem_booster(typeBoostGame type)
    {
        StartCoroutine(animSwitchToNextBoost(type));
    }

    public float timeWaitChangeBoost = 15 / 60f;
    private IEnumerator animSwitchToNextBoost(typeBoostGame type)
    {
        yield return new WaitForSeconds(timeWaitChangeBoost);
        setInfosAnim(type, false);
    }





    private Dictionary<typeBoostGame, typeRarety> typeBoostToTypeRarety = new Dictionary<typeBoostGame, typeRarety>()
    {
        { typeBoostGame.aimant, typeRarety.common},
        { typeBoostGame.ancre, typeRarety.common},
        { typeBoostGame.medic, typeRarety.common},
        { typeBoostGame.ghost, typeRarety.rare},
        { typeBoostGame.fusee, typeRarety.rare},
        { typeBoostGame.ressort, typeRarety.rare},
        { typeBoostGame.souris, typeRarety.epic},
        { typeBoostGame.pierre_tombale, typeRarety.epic},
        { typeBoostGame.tirelire, typeRarety.epic},
        { typeBoostGame.bouclier, typeRarety.legendary},
        { typeBoostGame.bombe, typeRarety.legendary},
        { typeBoostGame.super_viseur, typeRarety.legendary}
    };

    public List<rewardBoost_typeByGameObj> typeBoostToGameO;
    public List<typeRaretyToGameObj> typeRaretyToGameO;
    private GameObject getBoostImgAssociated(typeBoostGame type)
    {
        foreach (rewardBoost_typeByGameObj r in typeBoostToGameO)
        {
            if (r.type == type)
                return r.itemAssociated;
        }
        return null;
    }
    private GameObject getRaretyImgAssociated(typeRarety type)
    {
        foreach (typeRaretyToGameObj r in typeRaretyToGameO)
        {
            if (r.type == type)
                return r.itemAssociated;
        }
        return null;
    }
    private GameObject getRaretyHiddenAssociated(typeRarety type)
    {
        foreach (typeRaretyToGameObj r in typeRaretyToGameO)
        {
            if (r.type == type)
                return r.hiddenImg;
        }
        return null;
    }





    public void screenClicked()
    {
        if (booster)
            btnNextBoost();
        else
            btnGetReward_daily();
    }

    public void btnNextBoost()
    {

    }

    public void btnGetReward_daily()
    {

    }
}
