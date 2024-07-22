using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct colorBoostByRarety
{
    public typeRarety typeRarety;
    public Color colorRarety;
}
public class menuGetBoost : MonoBehaviour
{
    public int test_addedNbBoost = 10;
    public int test_currentNbBoost = 10;
    public int test_maxNbBoost = 30;
    public typeBoostGame test_typeBoostReward;
    public bool test_btnOpenRewardBoostMenu = false;

    /* Update is called once per frame
    void Update()
    {
        if (test_btnOpenRewardBoostMenu)
        {
            test_btnOpenRewardBoostMenu = false;
            openRewardBoost_daily(test_typeBoostReward, test_addedNbBoost, test_currentNbBoost, test_maxNbBoost);
        }
    }*/

    public static offerItem currentDailyOfferItem;
    private static menuGetBoost instance;
    public static menuGetBoost getInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("menuGetBoost").GetComponent<menuGetBoost>();
        }
        return instance;
    }

    public boostRewardAnim boostRewardItem;
    private int nbBoostProg_init, nbBoostProg_max, nbBoostProg_added, levelBoostOpenned;
    public Text nbCardAddedText;
    private typeBoostGame typeBoost_init;
    public void openRewardBoost_daily(typeBoostGame type, int nbBoostReward, int nbBoostInit, int nbBoostMax,
        int levelBoost)
    {
        rewardM.resetItems();

        dailyReward = true;
        phaseClick = 0;

        newBoostTextGo.SetActive(false);
        setCurrentBoostValues(type, nbBoostReward, nbBoostInit, nbBoostMax, levelBoost);

        boostRewardItem.popDaily(type);
        setColorFond(boostRewardItem.getTypeRaretyByTypeBoost(type));
        // anim open
        myAnim.Play("openMenuGetBoost");
        sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_open);

        // on attend que click sur ecran
    }



    // 0 init
    // 1 clickMystery
    // 2 clickClose
    private int phaseClick;
    public rewardBoostManager rewardM;

    private void closeMenuGetBoost()
    {
        myAnim.Play("closeMenuGetBoost");
        currentDailyOfferItem.menuGetBoostOfferClosed();
    }



    public Image progBarAdded, progBarCurrent, progBarSpace;
    private float spaceSize;
    private bool spaceSet = false;
    private void initProgBar()
    {
        if (!spaceSet)
        {
            spaceSet = true;
            spaceSize = progBarSpace.fillAmount - progBarCurrent.fillAmount;
        }
        float currentProg = nbBoostProg_init / (float)nbBoostProg_max;
        progBarCurrent.fillAmount = currentProg;
        progBarSpace.fillAmount = currentProg + spaceSize;

        float maxProg = (nbBoostProg_init + nbBoostProg_added) / (float)nbBoostProg_max;       
        progBarAdded.fillAmount = maxProg;
    }

    public float timeWaitBeforeAnim = 1f;
    public float timeAnimFillProgBar = 0.8f;
    public Animation progBarAnim;
    private static int currentAnimId = 0; 
    private IEnumerator animProgBar()
    {
        currentAnimId ++;
        int idCopy = currentAnimId;
        progBarAnim.Play("progBarRewardBoostReset");

        float initProg = nbBoostProg_init / (float)nbBoostProg_max;
        if (initProg < 1f)
        {
            float goalProg = (nbBoostProg_init + nbBoostProg_added) / (float)nbBoostProg_max;
            goalProg = goalProg > 1f ? 1f : goalProg;
            yield return new WaitForSeconds(timeWaitBeforeAnim);
            progBarAnim.Play("progBarRewardBoostFill");

            float timer = Time.time;
            float progTime;
            float progVal;

            while (Time.time < timer + timeAnimFillProgBar
            && idCopy == currentAnimId)
            {
                progTime = (Time.time - timer) / timeAnimFillProgBar;
                progVal = initProg + (goalProg - initProg) * progTime;
                progBarCurrent.fillAmount = progVal;
                progBarSpace.fillAmount = progVal + spaceSize;
                yield return null;
            }

            if (goalProg == 1f && idCopy == currentAnimId)
            {
                progBarAnim.Play("progBarRewardBoostFinish");
                //sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_full);
            }
        }
        else
        {
            yield return new WaitForSeconds(timeWaitBeforeAnim);
            //progBarAnim.Play("progBarRewardBoostFill");
            //Debug.Log("Anim bar finish, go.active : " + progBarAnim.gameObject.activeInHierarchy);
            progBarAnim.Play("progBarRewardBoostFinish");
            //sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_full);
            yield return new WaitForSeconds(timeAnimFillProgBar);
        }
        //Debug.Log("Reactivate btn click");
        //btnClickGo.SetActive(true);
    }

    // appelé par anim boosterNextBoost
    public Image imgBarFinishGo;
    public void closeAnimBarFinished()
    {
        imgBarFinishGo.fillAmount = 0f;
        /*Color c = imgBarFinishGo.color;
        imgBarFinishGo.color = new Color(c.r, c.g, c.b, 0f);*/
    }


    public Image[] imgChangeColor;
    public List<colorBoostByRarety> colorByRarety;
   
    private void setColorFond(typeRarety tR)
    {
        Color c = getColorByTypeRarety(tR);
        foreach(Image i in imgChangeColor)
        {
            i.color = new Color(c.r, c.g, c.b, i.color.r);
        }
    }

    public Animation myAnim;








    public Animation animBoosterWait;
    // appelé par animation openMenuOpenBooster
    public void animBoosterOpenLoaded()
    {
        animBoosterWait.Play("boosterWaitOpen");
    }

    private bool dailyReward;

    [Header("Booster")]
    // PARTIE BOOSTER
    public float timeWaitPop_first = 0.3f;
    public float timeWaitPop_next = 0.3f;
    public Color colorBoost_argent, colorBoost_or, colorBoost_crystal;
    public GameObject item_argent, item_or, item_crystal;
    public Image[] imgColorBooster;
    public Image[] imgChangeColor_booster;
    public Animation animOpenBooster;

    public openBoosterManager boosterGenerator;

    private GameObject currentItemOpen;
    private List<int[]> currentInfoContentBooster;
    public void openMenuBooster(typeBooster type)
    {
        rewardM.resetItems();

        dailyReward = false;

        // donner bonne couleure au booster
        Color c = type == typeBooster.boosterArgent ? colorBoost_argent : type == typeBooster.boosterOr ? colorBoost_or : colorBoost_crystal;
        foreach (Image i in imgColorBooster)
            i.color = c;
        // donner bonne couleure au fond
        foreach (Image i_2 in imgChangeColor_booster)
            i_2.color = new Color(c.r, c.g, c.b, i_2.color.a);

        // activer la bonne imgBooster
        if (currentItemOpen != null)
            currentItemOpen.SetActive(false);
        currentItemOpen = type == typeBooster.boosterArgent ? item_argent : type == typeBooster.boosterOr ? item_or : item_crystal;
        currentItemOpen.SetActive(true);

        // anim ouverture menu openBooster
        animOpenBooster.Play("openMenuGetBooster");
        //sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.booster_come);

        currentInfoContentBooster = boosterGenerator.createContentBooster(type);
        currentIndexBoostItem = 0;

        // on attend un click sur ecran pour lancer anim openBooster
    }
    private void closeMenuBooster()
    {
        animOpenBooster.Play("boosterClose");
        //sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.booster_open);
    }

    private int currentIndexBoostItem;
    private bool isLastBoostFromBooster()
    {
        return currentIndexBoostItem == currentInfoContentBooster.Count;
    }

    public float timeWaitSfxOpenBoost = 0.2f;
    private IEnumerator playSoundBoostOpen_late()
    {
        // on attend anim boost pop commence ( temps dans anim openPackBooster )
        yield return new WaitForSeconds(timeWaitSfxOpenBoost);
        sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_open);
    }
    // appelé pendant ouverture booster, quand ouvre boost
    private void goNextBoostItem()
    {
        // currentInfoContentBooster, cell : {id , nombre}

        // le retenir pour le mettre à jour une fois finit
        itemBoost iB = boostManager.getInstance().getBoostById(
            currentInfoContentBooster[currentIndexBoostItem][0]);
        string nbBoostMax = iB.getNbCardRequirered(iB.niveauBoost);
        int max = nbBoostMax == itemBoost.max_string ? iB.currentNbCards : int.Parse(nbBoostMax);

        setCurrentBoostValues(
            iB.typeBoost_name
            , currentInfoContentBooster[currentIndexBoostItem][1]
            , iB.currentNbCards
            , max
            , iB.niveauBoost);

        // animOpen boost / go next boost
        if (currentIndexBoostItem == 0)
        {
            boostRewardItem.popBooster(iB.typeBoost_name);
            animOpenBooster.Play("openPackBooster");
            sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.booster_skratch);
            StartCoroutine(playSoundBoostOpen_late());
            

            //animOpenBooster.Play("boosterFirstBoost");
            StartCoroutine(animBoostPop(timeWaitPop_first));
            StartCoroutine(updateDataBoostFromBooster(iB, currentInfoContentBooster[currentIndexBoostItem][1], max));

            animChangeColorFond(getColorByTypeRarety(iB.type), timeWaitInit);
        }
        else
        {
            boostRewardItem.nextBoostItem_booster(iB.typeBoost_name);
            boostRewardItem.switchToNextBoost();
            StartCoroutine(animBoostPop(timeWaitPop_next));
            StartCoroutine(updateDataBoostFromBooster(iB, currentInfoContentBooster[currentIndexBoostItem][1], max));

            animOpenBooster.Play("boosterNextBoost");
            sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_switch);
            //Debug.Log("GO next boost");
            animChangeColorFond(getColorByTypeRarety(iB.type), 0f);
        }

        currentIndexBoostItem++;
    }


    public float delayUpdateRealItem = 0.5f;// temps à  partir de '
    // appelé par anim quand next et close
    private IEnumerator updateDataBoostFromBooster(itemBoost iB, int nbAdded, int max)
    {
        yield return new WaitForSeconds(delayUpdateRealItem);
        iB.saveBoosterRewardData(nbAdded, max);
    }


    public float timeChangeColor = 0.8f;
    public float timeWaitInit = 0.3f;
    // public Image[] imgChangeColor_booster; ( + haut )
    //private Color initColorFond;
    public void animChangeColorFond(Color toGo, float initTimeWait)
    {
        // va de currentColor à colorRaretyBoost
        StartCoroutine(switchColor(imgChangeColor_booster[0].color, toGo, timeChangeColor, initTimeWait));
    }

    IEnumerator switchColor(Color initColor, Color endColor, float timeAnim, float initTimeWait)
    {
        if (initTimeWait != 0f)
            yield return new WaitForSeconds(initTimeWait);

        float t = 0, p;
        Color c;

        while (t < timeAnim)
        {
            yield return null;
            t += Time.deltaTime;

            p = t / timeAnim;
            p = p > 1 ? 1 : p;
            c = new Color(
               (initColor.r + (endColor.r - initColor.r) * p),
               (initColor.g + (endColor.g - initColor.g) * p),
               (initColor.b + (endColor.b - initColor.b) * p), 0f
               );
            foreach (Image im in imgChangeColor_booster)
                im.color = new Color(c.r, c.g, c.b, im.color.a);
        }
    }




    // FONCTIONS COMMUNES A DAILY ET BOOSTER
    public Transform positionPopTr;
    public Animation animItemMystery;
    public float timeWaitPop_daily = 0.3f;
    private IEnumerator animBoostPop(float timeWait)
    {
        yield return new WaitForSeconds(timeWait);

        //Debug.Log("Sfx boost blast"); // pas celui la a ouverture booster
        sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_blast);
        // pop reward
        rewardM.giveBoostReward(typeBoost_init, nbBoostProg_added, positionPopTr.transform);

        // init progBar
        initProgBar();
        // anim fill progBar
        StartCoroutine(animProgBar());
    }


    public void revealNewText()
    {
        // afficher " new " si debloqué
        newBoostTextGo.SetActive(boostIsUnlocked);
    }

    private bool boostIsUnlocked;
    public GameObject newBoostTextGo;
    private void setCurrentBoostValues(typeBoostGame type, int nbBoostReward, int nbBoostInit, int nbBoostMax
        , int levelBoost)
    {
        typeBoost_init = type;
        nbBoostProg_init = nbBoostInit;
        nbBoostProg_max = nbBoostMax;
        nbBoostProg_added = nbBoostReward;
        levelBoostOpenned = levelBoost;
        boostIsUnlocked = levelBoost == 1 && nbBoostInit == 0;



        nbCardAddedText.text = '+' + nbBoostReward.ToString();
    }
    public void clickOnScreen()
    {
        //Debug.Log("CLick on screen");
        phaseClick++;
        if (dailyReward)
        {
            // si dans phase mysteryOpen
            // -> anim click boost
            if (phaseClick == 1)
            {
                myAnim.Play("clickMysteryBoost");
                sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_reveal);
                boostRewardItem.switchDailyToReward();
                StartCoroutine(animBoostPop(timeWaitPop_daily));

            }
            else if (phaseClick == 2)
            {
                // fermer le menuRewardBoost
                closeMenuGetBoost();
                sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_close);
            }
        }
        else
        {
            if (!isLastBoostFromBooster())
            {
                goNextBoostItem();
                // sur de faire son ?
                //sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_switch);
            }
            else
            {
                closeMenuBooster();
                sfxGame.getInstance().playSfx_listRewardBoost(typeSfx_rewardBoost.boost_close);
                if (openOfferPack.isWaitingItemPackGiven)
                    StartCoroutine(waitBoosterClosed());
            }
        }
    }

    private IEnumerator waitBoosterClosed()
    {
        yield return new WaitForSeconds(0.35f);
        openOfferPack.getInstance().itemPackIsGiven();
    }

    private Color getColorByTypeRarety(typeRarety tR)
    {
        foreach (colorBoostByRarety cB in colorByRarety)
        {
            if (cB.typeRarety == tR)
            {
                return cB.colorRarety;
            }
        }
        return Color.white;
    }
}
