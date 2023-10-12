using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeRarety
{
    legendary,
    epic,
    rare,
    common,
    none
}




public enum typeUpgrade
{
    // fusée 2  - bouclier 2  - superViseur 2 - tirelire 2 - pierreTombale 2
    // souris 1 - ressort 1 - ghost 1 - medic 1 - aimant 1 - ancre 1

    none,
    time_effect, // ressort - bouclier - fusée - pierre tombale
    time_regen, // ghost - ressort

    vitesse_max, // fusee
    vitesse_min, // tortue

    number_init, // bouclier - superViseur - fusée - medic
    number_yow_max, // tirelire

    distance_max, // ancre - superViseur - aimant
    distance_min,

    nb_bounce_after_death, // pierre tombale
    nb_bounce_min, // tirelire

    taille_min, // souris
}


public class itemBoost : MonoBehaviour
{
    // savoir si evolution croissant ou decroissante
    public static Dictionary<typeUpgrade, bool> isUpgradeAscending = new Dictionary<typeUpgrade, bool>()
    {
        { typeUpgrade.time_effect, true },
        { typeUpgrade.time_regen, false },
        { typeUpgrade.number_init, true },
        { typeUpgrade.number_yow_max, true },
        { typeUpgrade.distance_max, true },
        { typeUpgrade.distance_min, false },
        { typeUpgrade.nb_bounce_after_death, true },
        { typeUpgrade.nb_bounce_min, false },
        { typeUpgrade.taille_min, false },
    };
    public Transform imageBoost;
    public Transform imageProgBar;
    //public GameObject imageBrillerUpgrade;
    public static List<int> priceBoost = new List<int>() { 40, 90, 150, 240 };

    // legendaire : 2, 4, 10, 20
    // epique : 4, 10, 20, 50
    // rare : 10, 20, 50, 100
    // communes : 20, 50, 100, 200
    public Dictionary<typeRarety, int[]> nbCardUpgrade = new Dictionary<typeRarety, int[]>() {
        { typeRarety.legendary, new int[] { 2, 4, 10, 20} },
        { typeRarety.epic, new int[] { 4, 10, 20, 50} },
        { typeRarety.rare, new int[] { 10, 20, 50, 100} },
        { typeRarety.common, new int[]{ 20, 50, 100, 200} }
    };

    [Header("info boost")]
    public typeBoostGame typeBoost_name;
    public float[] progValueBoost_1;
    public float[] progValueBoost_2;
    public int[] upgradeAtLevel; // 1 - 2 - 3

    public typeTraduction_boost nomBoost;
    public typeTraduction_boost descriptionBoost;
    public typeTraduction_boost controllBoost;

    public List<int[]> priceUpgrade;
    public List<float[]> upgradeValues;

    public typeUpgrade upgrade1;
    public typeUpgrade upgrade2;

    public typeRarety type;
    public bool isSuperPower = false;
    public int idBoost = 0;

    [Header("Autres")]
    public Transform fondLock, /*btnInfoLock*/ btnUseLock;//, textLock;
    public Text niveauText;

    public Text useBoostText;
    //public Text infoBoostText;
    //public Text upgradeBoostText;
    public Text lockBoostText;
    private static string useString = "", infoString = "", upgradeString = "";// lockString = "";

    public bool unlocked = false;
    private bool isOpen = false; // btn info et use
    public static string max_string = "max";

    private void Start()
    {
        if (transform.childCount > 1)
            imageBoost = transform.GetChild(1);
        animUpgradeList.Add(upgradeImg.GetComponent<Animation>());
    }

    private static List<Animation> animUpgradeList = new List<Animation>();

    private void OnDisable()
    {
        if (isOpen)
            btnItem();
    }

    public string getNbCardRequirered(int niveau)
    {
        if (niveau == upgradeAtLevel.Length)
            return max_string;
        else
            return nbCardUpgrade[type][niveau - 1].ToString();
    }

    public int getPrice()
    {
        if (niveauBoost == upgradeAtLevel.Length)
            return -1;
        return priceBoost[niveauBoost - 1];
    }

    // on part du principe que cette fonction n'est appelé seulement par les boost regen
    public float getTimeRegen()
    {
        int index_upgrade = upgrade1 == typeUpgrade.time_regen ? 0 : 1;
        float t = getUpgradeValue()[index_upgrade][0];
        return t;
    }
    public int getNbBoost()
    {
        // legendaire : upgrade 2
        // life : upgrade 1
        if (type == typeRarety.legendary)
        {
            return (int)getUpgradeValue()[1][0];
        }
        else if (typeBoost_name == typeBoostGame.medic)
        {
            return (int)getUpgradeValue()[0][0];
        }
        return 0;

    }

    public int[] getTirelireInfos()
    {
        List<float[]> l = getUpgradeValue();

        return new int[] { (int)l[0][0], (int)l[1][0] };
    }

    public float getTimeEffect()
    {
        // legendaire : upgrade 2
        // life : upgrade 1
        if (type == typeRarety.legendary
            || typeBoost_name == typeBoostGame.fusee)
        {
            return getUpgradeValue()[0][0];
        }
        // sinon si pierre tombale
        return 0;
    }



    private static itemBoost currentOpen;

    public GameObject btnInfoGo, btnUpgradeGo;
    /*
     * void btnItem()
     * IEnumerator closeInfoLate()
     * void btnInfo()
     * void btnUse()
    */
    #region gestion ouverture fermeture / btn info et use

    public void btnItem()
    {
        //Debug.Log("CLique sur boost item, isOpen ? " + isOpen+ "( Unlocked : "+unlocked+" )");
        isOpen = !isOpen;
        if (isOpen)
        {
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
            //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_itemBoost);
            snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapBoost, GetComponent<RectTransform>());
            GetComponent<Animation>().Play("itemBoostClicked");
            //boostManager.getInstance().SnapTo_2(GetComponent<RectTransform>());
            if (currentOpen != null)
                currentOpen.btnItem();
            currentOpen = transform.GetComponent<itemBoost>();
        }
        else if (currentOpen == transform.GetComponent<itemBoost>())
        {
            currentOpen = null;
            btnInfoGo.SetActive(false);
            btnUseLock.gameObject.SetActive(false);
        }

        Text t;
        // si lock, juste afficher btn info
        if (!unlocked)
        {
            t = btnInfoGo.transform.GetChild(1).GetComponent<Text>();
            t.text = infoString;
            t.transform.GetChild(0).GetComponent<Text>().text = infoString;
            btnInfoGo.SetActive(isOpen);

            //btnInfoLock.gameObject.SetActive(isOpen);
        }
        // si pas lock, afficher fond, btn info, btnUse
        else
        {
            //Debug.Log("Change état boutons : "+(isOpen ? "ACTIVATE" : "DESACTIVATE"));
            //fondLock.gameObject.SetActive(isOpen);
            //btnInfoLock.gameObject.SetActive(isOpen);

            string toHave = getNbCardRequirered(niveauBoost);


            // Modifier
            // si canBeUpgrade
            //  - activer btn upgrade
            //  - traduire text ( si pas encore fait )
            // sinon 
            //  - activer btn infos
            //  - traduire text ( si pas encore fait )
            bool upgrade = toHave != max_string && int.Parse(toHave) <= currentNbCards;
            string textBtnString = upgrade ? upgradeString : infoString;
            GameObject btnUsed = upgrade ? btnUpgradeGo : btnInfoGo;

            if (isOpen)
            {
                t = btnUsed.transform.GetChild(1).GetComponent<Text>();
                t.text = textBtnString;
                t.transform.GetChild(0).GetComponent<Text>().text = textBtnString;
            }

            btnUseLock.gameObject.SetActive(isOpen);
            btnUsed.SetActive(isOpen);

            /*if (upgrade)
            {
                if (isOpen)
                {
                    t = btnUpgradeGo.transform.GetChild(1).GetComponent<Text>();
                    t.text = textBtnString;
                    t.transform.GetChild(0).GetComponent<Text>().text = textBtnString;
                }

                btnUpgradeGo.SetActive(isOpen);
            }
            else
            {
                if (isOpen)
                {
                    t = btnInfoGo.transform.GetChild(1).GetComponent<Text>();
                    t.text = textBtnString;
                    t.transform.GetChild(0).GetComponent<Text>().text = textBtnString;
                }

                btnInfoGo.SetActive(isOpen);
            }*/

            /*set_text(true
                , toHave != max_string && int.Parse(toHave) <= currentNbCards ? upgradeString : infoString
                , btnInfoLock.transform.GetChild(1).GetComponent<Text>());*/
            //btnInfoLock.transform.GetChild(1).GetComponent<Text>().text =
            //  toHave != max_string && int.Parse(toHave) <= currentNbCards ? "Améliorer" : "Infos";

        }
        if (isOpen)
        {
            _cliqueId++;
            StartCoroutine(closeInfoLate(_cliqueId));
        }
    }

    private void set_text(bool childToo, string s, Text t)
    {
        t.text = s;
        if (childToo)
            t.transform.GetChild(0).GetComponent<Text>().text = s;
    }

    private int _cliqueId = 0;
    private IEnumerator closeInfoLate(int clique_id)
    {
        yield return new WaitForSeconds(3f);
        if (isOpen && clique_id == _cliqueId)
            btnItem();
    }

    public void btnInfo() // btn ameliorer
    {
        //sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.btn_useInfoItem);
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_useInfos);
        infoBoostManager.getInstance().btnOpenInfoBoost(transform);
        //menuInfoBoost.getInstance().openMenu(transform);
        btnItem(); // fermer info
    }

    public void btnUse()
    {
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.btn_useInfoItem);
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_useInfos);
        boostManager.getInstance().btnUseItem(transform);
        btnItem(); // fermer info
        boostChangeSelectionState(true);
    }
    #endregion

    /*
     * List<float[]> getUpgradeValue()
     * float[] getValueUpgrade_byNiv(int boost, float[] upValue)
     * float roundFloat(float t)
     * int getIndexVal(int boost, int niveau)
    */
    #region Remplissage des valeure de menu infos boost

    public List<float[]> getUpgradeValue()
    {
        List<float[]> retour = new List<float[]>();
        retour.Add(getValueUpgrade_byNiv(1, progValueBoost_1));
        if (upgrade2 != typeUpgrade.none)
            retour.Add(getValueUpgrade_byNiv(2, progValueBoost_2));
        return retour;
    }

    public static float noUpgradeFloat = 1.1111f;
    private float[] getValueUpgrade_byNiv(int boost, float[] upValue)
    {
        int curValIndex = getIndexVal(boost, niveauBoost - 1);

        int nextValIndex = curValIndex + 1 < upValue.Length ? getIndexVal(boost, niveauBoost) : -1;

        return new float[] {
            nextValIndex == -1 ? upValue[upValue.Length - 1] : upValue[curValIndex]
            , nextValIndex == -1 ? noUpgradeFloat : roundFloat(upValue[nextValIndex] - upValue[curValIndex])
            , upValue.Length
            , curValIndex};

        //Debug.Log("Niveau : "+niveauBoost+" , curValIndex : " + curValIndex + " , next val : " + nextValIndex + ", size upValue : " + upValue.Length);
        /*if (nextValIndex == -1)
            return new float[] {
                upValue[upValue.Length - 1]
                , noUpgradeFloat
                , upValue.Length
                , curValIndex};

        return new float[] {
            upValue[curValIndex]
            , roundFloat(upValue[nextValIndex] - upValue[curValIndex])
            , upValue.Length
            , curValIndex};*/

        //nextValIndex - 1 == curValIndex ?
        //roundFloat(upValue[nextValIndex] - upValue[curValIndex]) : noUpgradeFloat };
    }


    private float roundFloat(float t) // a la dizaine centaine
    {
        return Mathf.Round(t * 100) / 100f;
    }

    private int getIndexVal(int boost, int niveau)
    {
        List<int> indexUp = new List<int>() { boost, 3 };
        int indexCurrentValue = 0;

        for (int i = 0; i < niveau; i++)
        {
            if (indexUp.Contains(upgradeAtLevel[i]))
                indexCurrentValue++;
        }

        return indexCurrentValue;
    }
    #endregion

    public string colorStringLevel = "";
    private static string levelString = "";
    private string getLevelString(bool shadowString)
    {
        if (levelString == "")
        {
            levelString = traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_niveau);
        }
        //Debug.Log("Leve string : " + levelString);
        return !shadowString ?
            "<color=\"" + colorStringLevel + "\">" + levelString + "</color>"
            : levelString;
    }

    public Text nbCardAdded_text;

    public int currentNbCards;
    public int niveauBoost;
    public Text progText, progText_ombre;
    public Image progBarImg;
    public Image bordProgBarImg;
    public GameObject upgradeImg;
    public GameObject voyantLevel;
    public Transform progStarBoostTr;
    public Color32 colorBarFull, colorBarNotFull, colorAnimProgress;
    public Color32 colorBarFull_bord, colorBarNotFull_bord;

    private Transform prevStarBarOpen;
    // 62EEFF

    /*
     * void setInitValues(bool isUnlocked, int niveau, int nbCards)
     * bool canBeUpgraded()
     * void setProgBarBoost(int nbCards, int niveau)
    */
    #region gestion de la progression du niveau et nombre de carte
    public void setInitValues(bool isUnlocked, int niveau, int nbCards)
    {
        /*useString = useString == "" ?
    traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_use) : useString;
        set_text(true, useString, useBoostText);

        infoString = infoString == "" ?
            traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_infos) : infoString;
        set_text(true, infoString, infoBoostText);*/

        //Debug.Log("Set init values, nbCards : " + nbCards + ", niveau " + niveau);
        unlocked = isUnlocked;
        niveauBoost = niveau;

        voyantLevel.SetActive(isUnlocked);
        fondLock.gameObject.SetActive(!isUnlocked);
        //textLock.gameObject.SetActive(!isUnlocked);
        progText_ombre.gameObject.SetActive(isUnlocked);

        if(prevStarBarOpen != null)
        {
            prevStarBarOpen.gameObject.SetActive(false);
        }
        prevStarBarOpen = progStarBoostTr.GetChild(niveau - 1);
        prevStarBarOpen.gameObject.SetActive(true);

        if (isUnlocked)
        {
            string levelString = /*" " + */niveau.ToString();
            //Debug.Log("<color=red>(2) LevelString : '" + levelString + "'</color>");
            niveauText.text = /*getLevelString(true) +*/ levelString;
            niveauText.transform.GetChild(0).GetComponent<Text>().text = /*getLevelString(false) +*/ levelString;
        }
        //set_text(true, getLevelString(false)+ " " + niveau.ToString(), niveauText.GetComponent<Text>());

        //niveauText.GetComponent<Text>().text = "Niveau " + niveau.ToString();

        currentNbCards = nbCards;
        setProgBarBoost(nbCards, niveau);


        infoString = infoString == "" ?
            traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_infos) : infoString;

        upgradeString = upgradeString == "" ?
            traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_upgrade) : upgradeString;

        useString = useString == "" ?
            traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_use) : useString;
        set_text(true, useString, useBoostText);

        /*lockString = lockString == "" ?
            traductionManager.getInstance().getTradShop(typeTraduction_shop.boost_lock) : lockString;
        set_text(false, lockString, lockBoostText);*/

    }

    public bool canBeUpgraded()
    {
        return getNbCardRequirered(niveauBoost) != max_string
            && currentNbCards >= nbCardUpgrade[type][niveauBoost - 1];
    }

    private void setProgBarBoost(int nbCards, int niveau)
    {
        string s = getNbCardRequirered(niveau);
        progText.text = nbCards + "<color=\"#99AEAA\"><size=20>/"
            + getNbCardRequirered(niveau)
            + "</size></color>";

        progText_ombre.text = nbCards + "<size=20>/"
            + getNbCardRequirered(niveau)
            + "</size>";

        progBarImg.fillAmount = s == max_string ?
            0f : (float)nbCards / nbCardUpgrade[type][niveau - 1];
        progBarImg.color = progBarImg.fillAmount < 1f ? colorBarNotFull : colorBarFull;
        bordProgBarImg.color = progBarImg.fillAmount < 1f ? colorBarNotFull_bord : colorBarFull_bord;

        bool changeStateUpgrade_toOn = progBarImg.fillAmount == 1f && !upgradeImg.gameObject.activeSelf;
        bool changeStateUpgrade_toOff = progBarImg.fillAmount < 1f && upgradeImg.gameObject.activeSelf;
        if (changeStateUpgrade_toOn)
        {
            upgradeImg.gameObject.SetActive(true);
            // si on met l'image upgrade active, on reset toutes les anims pour quelles soient coordonnées
            foreach (Animation a in animUpgradeList)
                a["canUpgrade"].time = 0f;
        }
        else if (changeStateUpgrade_toOff)
        {
            upgradeImg.gameObject.SetActive(false);
        }
        //upgradeImg.gameObject.SetActive(progBarImg.fillAmount == 1f);

        //imageBrillerUpgrade.SetActive(progBarImg.fillAmount == 1f);
    }

    #endregion


    public bool isUpgrading = false;
    public GameObject[] close_modeDailyOffer;
    public GameObject btnSelect;
    /*
     * void setModeOpenBooster()
     * void setModeDailyOffer()
     * 
     * void loadItemBooster(int nbCards)
     * IEnumerator animCardAdded_dailyOffer(int nbCardAdded)
     * 
     * void upgradeBoost(bool isItemFromAnim)
     * IEnumerator upgradeAnim(int nbCard)
    */
    #region MODE d'affichages ( booster - dailyOffer - upgrade )
    public void setModeOpenBooster()
    {
        if (currentNbCards == 0 && niveauBoost == 1)
        {
            niveauText.text = /*getLevelString(true) + " " + */traductionManager.getInstance().getTradDefi(typeTraduction_app.defi_newPalet);
            niveauText.transform.GetChild(0).GetComponent<Text>().text = /*getLevelString(false) + " " +*/ traductionManager.getInstance().getTradDefi(typeTraduction_app.defi_newPalet);
            //.
            //set_text(true, traductionManager.getInstance().getTradDefi(typeTraduction_app.defi_newPalet), niveauText);
            niveauText.GetComponent<Animation>().Play("newBoost");
        }
    }

    public void loadItemBooster(int nbCards)
    {
        // si affichage locked, unlock
        if (!unlocked)
        {
            fondLock.gameObject.SetActive(false);
            //textLock.gameObject.SetActive(false);
            // ajouter le nouveau boost aux boost own
            boostManager.getInstance().boostIsUnlocked(idBoost);
        }
        nbCardAdded_text.text = "+" + nbCards;
        nbCardAdded_text.transform.parent.gameObject.SetActive(true);
        // aficher nb card gived
        setProgBarBoost(currentNbCards + nbCards, niveauBoost);
        // update object dont on a fait la copie
        boostManager.getInstance().getBoostById(idBoost).setInitValues(true, niveauBoost, currentNbCards + nbCards);
        boostManager.getInstance().updateBoostSavedValues(idBoost, currentNbCards + nbCards, niveauBoost);
        if (idBoost == offreQuotidienne.dailyBoost_id)
            offreQuotidienne.getInstance().changeCurrentBoostValues();
    }



    private void saveNewData(int nbCardAdded, int max)
    {
        int initNbCard = currentNbCards;
        bool nowUnlocked = initNbCard == 0 && niveauBoost == 1;
        // on envoit les données au boost d'origine
        //boostManager.getInstance().getBoostById(idBoost).
        setInitValues(true, niveauBoost, initNbCard + nbCardAdded);
        boostManager.getInstance().updateBoostSavedValues(idBoost, initNbCard + nbCardAdded, niveauBoost);

        if(nowUnlocked)
            boostManager.getInstance().boostIsUnlocked(idBoost);
        /*string s = getNbCardRequirered(niveauBoost);
        if (s != max_string)
        {
            if (initNbCard + nbCardAdded >= int.Parse(s) && niveauBoost == 1)
            {
                boostManager.getInstance().boostIsUnlocked(idBoost);
            }
        }*/
    }
    public void saveBoosterRewardData(int nbCardAdded, int max)
    {
        saveNewData(nbCardAdded, max);
    }
    public void saveDailyBoostRewardData(int nbCardAdded, int max)
    {
        saveNewData(nbCardAdded, max);
    }

    // SUPPRIMER
    public IEnumerator animCardAdded_dailyOffer(int nbCardAdded)
    {
        float t = 0f;
        int initNbCard = currentNbCards;
        bool noProgBAnim = niveauBoost == upgradeAtLevel.Length;
        //== nbCardUpgrade[type].Length;

        // bug, indexOutOfBounds
        //Debug.Log("nbCardUpgrade[" + type + "].length = " + nbCardUpgrade[type].Length+ ", niveauBoost "+niveauBoost);
        int nbCardRequirered = noProgBAnim ? 0 : nbCardUpgrade[type][niveauBoost - 1];

        float initProgBarValue = progBarImg.fillAmount;
        float endProgBarValue = noProgBAnim ? initProgBarValue
            : (initNbCard + nbCardAdded) / (float)nbCardRequirered;
        float rangeProgB = endProgBarValue - initProgBarValue;

        float wait = nbCardAdded == 1 ? 0.5f : 1f;
        float p;
        while (t < wait)
        {
            p = t / wait;
            int addedCard = (int)(nbCardAdded * p);
            progText.text = (initNbCard + addedCard) + "/"
                + (noProgBAnim ? "max" : nbCardRequirered.ToString());
            if (!noProgBAnim)
                progBarImg.fillAmount = initProgBarValue + rangeProgB * p;

            yield return null;
            t += Time.deltaTime;
        }

        progText.text = (initNbCard + nbCardAdded) + "/"
            + (noProgBAnim ? "max" : nbCardRequirered.ToString());
        if (!noProgBAnim)
        {
            setProgBarBoost(initNbCard + nbCardAdded, niveauBoost);
        }

        // on envoit les données au boost d'origine
        boostManager.getInstance().getBoostById(idBoost).setInitValues(true, niveauBoost, initNbCard + nbCardAdded);
        boostManager.getInstance().updateBoostSavedValues(idBoost, initNbCard + nbCardAdded, niveauBoost);

        if (initNbCard == 0 && niveauBoost == 1)
        {
            boostManager.getInstance().boostIsUnlocked(idBoost);
        }
    }




    public void upgradeBoost(/*bool isItemFromAnim*/)
    {
        /* isItemFromAnim : diférencier boost du menu et celui du booster
        isUpgrading = true;
        if (isItemFromAnim)
        {
            StartCoroutine(upgradeAnim(currentNbCards));
            btnSelect.gameObject.SetActive(false);
        }
        else
        {*/
            currentNbCards -= nbCardUpgrade[type][niveauBoost - 1];
            niveauBoost++;
            setInitValues(true, niveauBoost, currentNbCards);
            boostManager.getInstance().updateBoostSavedValues(idBoost, currentNbCards, niveauBoost);
            if (idBoost == offreQuotidienne.dailyBoost_id)
            {
                offreQuotidienne.getInstance().changeCurrentBoostValues();
            }
        //}
    }

    /*private IEnumerator upgradeAnim(int nbCard)
    {
        // descendre progbar
        float t = 0, t_2 = 1.5f;
        //Text prog_text = progBarImg.transform.GetChild(1).GetComponent<Text>();
        int nbCardRequirered = nbCardUpgrade[type][niveauBoost - 1];
        niveauBoost++;
        while (t < t_2)
        {
            yield return null;
            t += Time.deltaTime;
            float prog = (t / t_2);
            progBarImg.fillAmount = 1 - prog;
            progText.text = (int)(nbCard - nbCardRequirered * prog) + "/" + nbCardRequirered;
        }
        progText.text = (nbCard - nbCardRequirered) + "/" + getNbCardRequirered(niveauBoost);
        upgradeImg.SetActive(false);

        // anim text niveau 
        string levelString = /*" " + niveauBoost.ToString();

        niveauText.GetComponent<Animation>().Play("boostUpgradeLevel");
        niveauText.transform.GetChild(1).GetComponent<Text>().text = getLevelString(true) + levelString;
        yield return new WaitForSeconds(25 / 60f);

        Debug.Log("<color=red>(1) LevelString : '" + levelString + "'</color>");
        niveauText.text = getLevelString(true) + levelString;
        niveauText.transform.GetChild(0).GetComponent<Text>().text = getLevelString(false) + levelString;
        //set_text(true,getLevelString(false)+ " " + niveauBoost, niveauText);
        //.
        //niveauText.text = "Niveau " + niveauBoost;
        //niveauText.transform.GetChild(1).GetComponent<Text>().text = getLevelString(true) + " " + niveauBoost;
        yield return new WaitForSeconds(35 / 60f);

        isUpgrading = false;
    }*/

    #endregion

    [Header("Daily Mode")]
    // Mode affichage dailyOffer
    public GameObject voyantNiveau;
    public GameObject ombreNumberBoostReceived;
    public itemBoostModeDaily_changingChild[] transformChaningInDailyMode;
    public GameObject normalBackground;
    public GameObject dailyBackground;
    private bool modeDailyOfferOn = false;
    public void setDailyOfferMode()
    {
        modeDailyOfferOn = true;
        voyantNiveau.gameObject.SetActive(false);
        ombreNumberBoostReceived.SetActive(true);
        normalBackground.SetActive(false);
        dailyBackground.SetActive(true);
        foreach (itemBoostModeDaily_changingChild iB in transformChaningInDailyMode)
        {
            iB.changingChild.localPosition = iB.newPosition;
            //iB.changingChild.localScale = iB.newScale;
        }
    }








    // Dans section boost, anim glow si selectionné

    private bool isSelected = false;
    //private int nbLeader = 0;
    private static List<itemBoost> listBoostUsed = new List<itemBoost>();
    public void boostChangeSelectionState(bool _isSelected)
    {
        isSelected = _isSelected;
        if (isSelected)
        {
            if (!leaderSet)
            {
                leaderSet = true;
                isLeader = true;
                //nbLeader++;
            }
            if (gameObject.activeInHierarchy)
                StartCoroutine(animItemBoostSelected());
            listBoostUsed.Add(GetComponent<itemBoost>());
        }
        else
        {
            itemBoostDeletedFromUsed();
            /*if (isLeader)
            {
                isLeader = false;
                leaderSet = false;
                //nbLeader--;
            }
            listBoostUsed.Remove(GetComponent<itemBoost>());*/
        }
    }
    private void OnEnable()
    {
        if (!modeDailyOfferOn && isSelected)
            StartCoroutine(animItemBoostSelected());
    }

    //public bool isStaticItem = false;

    public Image fondImgGlow_1, fondImgGlow_2;
    public float timeAnimGlow = 0.5f;
    public float colorNormal_alpha = 0.5f;
    public float colorGlow_alpha = 1f;
    public float colorInit_alpha = 0f;
    //public Color colorNormal, colorGlow;

    // 2 états possibles
    // -> gère l'anim générale
    // -> suit l'anim générale
    private static bool leaderSet = false;
    public bool isLeader = false;
    private static float currentAnimColor_static;
    private static float t = 0f;
    private static bool toColorGlow = true, loopInitialyzed = false;
    private static float currentColor_alpha, goalColor_alpha;
    private static float diffColor;// = new float[4];
    private IEnumerator animItemBoostSelected()//bool isLeader)
    {
        //float[] diffColor = new float[4];
        float prog;
        if (isLeader)
        {
            if (!loopInitialyzed)
            {
                loopInitialyzed = true;
                currentColor_alpha = colorNormal_alpha;
                goalColor_alpha = colorGlow_alpha;

                diffColor = currentColor_alpha - goalColor_alpha;
            }
        }
        while (isSelected)
        {
            if (isLeader)
            {
                yield return null;
                t += Time.deltaTime;
                prog = t / timeAnimGlow;
                prog = prog > 1f ? 1f : prog;

                currentAnimColor_static = currentColor_alpha - (diffColor * prog);

                fondImgGlow_1.color = new Color(fondImgGlow_1.color.r, fondImgGlow_1.color.g, fondImgGlow_1.color.b, currentAnimColor_static);
                fondImgGlow_2.color = new Color(fondImgGlow_2.color.r, fondImgGlow_2.color.g, fondImgGlow_2.color.b, currentAnimColor_static);

                if (t >= timeAnimGlow)
                {
                    t = 0f;
                    toColorGlow = !toColorGlow;
                    goalColor_alpha = toColorGlow ? colorGlow_alpha : colorNormal_alpha;
                    currentColor_alpha = !toColorGlow ? colorGlow_alpha : colorNormal_alpha;

                    diffColor = currentColor_alpha - goalColor_alpha;
                }

            }
            else
            {
                yield return null;
                if (loopInitialyzed)
                {
                    fondImgGlow_1.color = new Color(fondImgGlow_1.color.r, fondImgGlow_1.color.g, fondImgGlow_1.color.b, currentAnimColor_static);
                    fondImgGlow_2.color = new Color(fondImgGlow_2.color.r, fondImgGlow_2.color.g, fondImgGlow_2.color.b, currentAnimColor_static);
                }
            }
        }
        fondImgGlow_1.color = new Color(fondImgGlow_1.color.r, fondImgGlow_1.color.g, fondImgGlow_1.color.b, colorInit_alpha);
        fondImgGlow_2.color = new Color(fondImgGlow_2.color.r, fondImgGlow_2.color.g, fondImgGlow_2.color.b, colorInit_alpha);
    }
    


    public void itemBoostDeletedFromUsed()
    {
        isSelected = false;
        listBoostUsed.Remove(GetComponent<itemBoost>());
        if (isLeader)
        {
            Debug.Log("Leader removed from used");
            isLeader = false;

            if (listBoostUsed.Count > 0)
            {
                // le premier item de la list devient le leader
                listBoostUsed[0].isLeader = true;
                Debug.Log("Another boost is set leader");
            }
            else
                leaderSet = false;
        }
    }
}

[System.Serializable]
public struct itemBoostModeDaily_changingChild
{
    public Transform changingChild;
    public Vector2 newPosition;
    //public Vector2 newScale;
}
