using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//using TMPro;


[System.Serializable]
public struct boostMenu_infos
{
    public typeBoostGame typeB;
    public string animAssociated_name;
    // panTop, imgBoost et nameRarety doivent être associé
    public Transform imgBoostTr_associated;
    public Transform typeRaretyTr_associated;
    public GameObject[] closeObjects_endAnim;
}

public class infoBoostManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        currentBtnUpgradeOpen = btnUpgradeOn;
    }

    private static infoBoostManager instance;

    public static infoBoostManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<infoBoostManager>();
        return instance;
    }

    /*
     * void btnOpenInfoBoost(Transform caller)
     * void btnCloseInfoBoost()
     * void boostIsOpenned(Transform caller)
     * void btnInfoText()
    */
    #region  gestion ouverture fermeture menu info boost

    public Animation boostMenuAnim;
    public GameObject boostMenuGo;
    private Transform currentItemBoost;
    private bool menuAlreadyLoadedBefore = false;

    public void btnOpenInfoBoost(Transform caller)
    {
        if(menuAlreadyLoadedBefore)
        {
            // on reset l'anim precedente
            foreach (GameObject o in currentInfos.closeObjects_endAnim)
                o.SetActive(false);
            aideVisuelleAnim.Stop();

            currentInfos.imgBoostTr_associated.gameObject.SetActive(false);
            currentInfos.typeRaretyTr_associated.gameObject.SetActive(false);

            levelStar[prevLevelStarOpenned].SetActive(false);
        }
        else
        {
            menuAlreadyLoadedBefore = true;
        }
        boostMenuGo.SetActive(true);
        boostMenuAnim.Play("openMenuInfoBoost");
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.openSousMenu);
        if (infoTextPan_activated)
        {
            infoTextPan_activated = false;
            infoTextPanGo.SetActive(false);
        }
        boostIsOpenned(caller);
        currentItemBoost = caller;
    }

    public void btnCloseInfoBoost()
    {
        boostMenuAnim.Play("closeMenuInfoBoost");
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.closeSousMenu);
    }




    private void setColorProgBar(bool isFull, bool arrowOn)
    {
        Color c = isFull ? canBeUpColor : normalColor;
        foreach (Image i in progBarImgs)
        {
            i.color = c;
        }

        canBeUpImg.SetActive(arrowOn);
    }

    public Animation aideVisuelleAnim;
    public boostMenu_infos[] infosBoosts;
    //public Image progCardImg;
    public Text nameBoostText;///*, levelBoostText*/, progText;
    public Text infoControlText, infoText;
    public Text priceOnText, priceOffText;
    public GameObject btnUpgradeOn, btnUpgradeOff, btnUpgradeMax;
    public upgradeUiInfo info_up_1, info_up_2;
    public string colorSecond_progFinished;

    public Image[] progBarImgs;
    public Color normalColor, canBeUpColor;
    public GameObject canBeUpImg;
    public Text progBarText;
    public Text progBarText_shadow;
    public Image progBarImg;

    //public Text levelStarText;
    public GameObject[] levelStar;
    private int prevLevelStarOpenned = -1;

    private GameObject currentBtnUpgradeOpen;
    private boostMenu_infos currentInfos;
    private Dictionary<typeRarety, typeTraduction_shop> rareteString = new Dictionary<typeRarety, typeTraduction_shop>()
    {
        {typeRarety.common, typeTraduction_shop.booster_common },
        {typeRarety.rare, typeTraduction_shop.booster_rare},
        {typeRarety.epic, typeTraduction_shop.booster_epic },
        {typeRarety.legendary, typeTraduction_shop.booster_legendary },
    };
    private Dictionary<typeUpgrade, typeTraduction_shop> nomUpgrade = new Dictionary<typeUpgrade, typeTraduction_shop>()
    {
        {typeUpgrade.distance_max, typeTraduction_shop.boost_am_distance },
        {typeUpgrade.distance_min, typeTraduction_shop.boost_am_distance },
        {typeUpgrade.nb_bounce_after_death, typeTraduction_shop.boost_am_timeEffect },
        {typeUpgrade.nb_bounce_min, typeTraduction_shop.boost_am_nbBounce },
        {typeUpgrade.number_init, typeTraduction_shop.boost_am_nombre },
        {typeUpgrade.number_yow_max, typeTraduction_shop.boost_am_nombreYow},
        {typeUpgrade.taille_min, typeTraduction_shop.boost_am_tailleMin },
        {typeUpgrade.time_effect, typeTraduction_shop.boost_am_timeEffect },
        {typeUpgrade.time_regen, typeTraduction_shop.boost_am_regenerationTime },
        {typeUpgrade.vitesse_max, typeTraduction_shop.boost_am_speedMax }
    };
    private Dictionary<typeUpgrade, string> typeValue = new Dictionary<typeUpgrade, string>()
    {
        {typeUpgrade.distance_max, "" },
        {typeUpgrade.distance_min, "" },
        {typeUpgrade.nb_bounce_after_death, "s" },
        {typeUpgrade.nb_bounce_min, "" },
        {typeUpgrade.number_init, "" },
        {typeUpgrade.number_yow_max, "" },
        {typeUpgrade.taille_min, "" },
        {typeUpgrade.time_effect, "s" },
        {typeUpgrade.time_regen, "s" },
        {typeUpgrade.vitesse_max, "" },
    };


    // remplir les infos du menu
    private void boostIsOpenned(Transform caller)
    {
        itemBoost iB = caller.GetComponent<itemBoost>();
        typeBoostGame typeB = iB.typeBoost_name;
        int e = 0;
        while (e < infosBoosts.Length && infosBoosts[e].typeB != typeB)
            e++;
        if (e == infosBoosts.Length)
            Debug.Log("<color=red> BUG, aucune infosBoost associé au boost demandé</color>");
        currentInfos = infosBoosts[e];

        // on commence par lancer l'animation associer au boost
        aideVisuelleAnim.Play(currentInfos.animAssociated_name);

        // on active imgBoost/rarety du boost ouvert
        currentInfos.imgBoostTr_associated.gameObject.SetActive(true);
        currentInfos.typeRaretyTr_associated.gameObject.SetActive(true);

        // prog
        progBarText.text = iB.progText.text;
        progBarText_shadow.text = iB.progText_ombre.text;
        //Debug.Log("Text ombre : " + iB.progText_ombre.text);
        progBarImg.fillAmount = iB.progBarImg.fillAmount;

        // set level Text
        if(prevLevelStarOpenned != -1) // désactiver precedent ouvert meme si le meme
            levelStar[prevLevelStarOpenned].SetActive(false);
        prevLevelStarOpenned = iB.niveauBoost - 1;
        if(iB.unlocked)
            levelStar[prevLevelStarOpenned].SetActive(true);

        string s;
        // set name Text
        s = traductionManager.getInstance().getTradBoost(iB.nomBoost);
        nameBoostText.text = s;
        nameBoostText.transform.GetChild(0).GetComponent<Text>().text = s;
        // set descriptionControle & descriptionBoost Text
        s = traductionManager.getInstance().getTradBoost(iB.descriptionBoost);
        infoText.text = s;
        s = traductionManager.getInstance().getTradBoost(iB.controllBoost);
        infoControlText.text = s;
        // traduire textRarete
        s = traductionManager.getInstance().getTradShop(rareteString[iB.type]);
        Transform tr = currentInfos.typeRaretyTr_associated.GetChild(0);
        tr.GetComponent<Text>().text = s;
        tr.GetChild(0).GetComponent<Text>().text = s;
        //switch

        // set nbCard progImg
        //int maxCard_int = -5;
        //string maxCard_string = iB.getNbCardRequirered(iB.niveauBoost);
        //progText.text = iB.currentNbCards + "<size=20>/" +
        //    (Int32.TryParse(maxCard_string, out maxCard_int) ? maxCard_int.ToString() : maxCard_string) + "</size>";
        //progCardImg.fillAmount = maxCard_int <= 0 ? 0 : (float)iB.currentNbCards / maxCard_int;

        nbUpgradeOpen = 0;
        List<float[]> upgradeValues = iB.getUpgradeValue();
        foreach (typeUpgrade tU in new typeUpgrade[] { iB.upgrade1, iB.upgrade2 })
        {
            // mettre item upgrade mode off SI vide
            (tU == iB.upgrade1 ? info_up_1 : info_up_2).setItemState(tU != typeUpgrade.none);

            string nomUp = tU != typeUpgrade.none ?
                traductionManager.getInstance().getTradShop(nomUpgrade[tU]) : "";

            string s_current = "", s_next = "";
            //float f_added = 0f, f_current = 0f;
            colorText cT = colorText.none; // couleure du text info upgrade, ex : 1 +0.5s
            bool willUpgrade = true;

            if (tU != typeUpgrade.none)
            {
             
                int i = tU == iB.upgrade1 ? 0 : 1;

                s_current = upgradeValues[i][0].ToString();
                //f_current = upgradeValues[i][0];

                if (iB.niveauBoost == iB.upgradeAtLevel.Length)
                {
                    cT = colorText.finished;
                    s_current += typeValue[tU] != "" ? typeValue[tU]
                        //"<color="+colorSecond_progFinished+">"+ typeValue[tU]+"</color>"
                        : "";
                }
                else
                {
                    if (upgradeValues[i][1] == itemBoost.noUpgradeFloat
                        || upgradeValues[i][1] == 0)
                    {
                        cT = colorText.red;
                        s_next = (itemBoost.isUpgradeAscending[tU] ? "+" : "-")
                            + "0" + typeValue[tU];
                        willUpgrade = false;
                        //f_added = 0f;
                    }
                    else
                    {
                        nbUpgradeOpen++;
                        cT = colorText.yellow;
                        float diff = upgradeValues[i][1];
                        s_next = (diff > 0 ? "+" : "") + diff + typeValue[tU];
                        //f_added = diff;
                    }
                }
            }

            if (tU == iB.upgrade1)
                info_up_1.setUpgrade(tU, nomUp, s_current, s_next, cT, (int)upgradeValues[0][2], (int)upgradeValues[0][3], willUpgrade
                    , upgradeValues[0][0], upgradeValues[0][1]);
            else if (tU != typeUpgrade.none)
                info_up_2.setUpgrade(tU, nomUp, s_current, s_next, cT, (int)upgradeValues[1][2], (int)upgradeValues[1][3], willUpgrade
                    , upgradeValues[1][0], upgradeValues[1][1]);
        }

        // desactiver bouton precedement ouvert
        if (currentBtnUpgradeOpen != null)
            currentBtnUpgradeOpen.SetActive(false);

        int price = iB.getPrice();
        if (price != -1)
        {
            priceOnText.text = price.ToString();
            priceOffText.text = price.ToString();

            setColorProgBar(iB.canBeUpgraded(), iB.canBeUpgraded());

            if (iB.canBeUpgraded() && price <= gameManager.monnaieVal)
            {
                // activer le bouton canBeUpgraded
                btnUpgradeOn.SetActive(true);
                currentBtnUpgradeOpen = btnUpgradeOn;
            }
            else
            {
                btnUpgradeOff.SetActive(true);
                currentBtnUpgradeOpen = btnUpgradeOff;
            }
        }
        else
        {
            // activate 'btn max'
            btnUpgradeMax.SetActive(true);
            currentBtnUpgradeOpen = btnUpgradeMax;
            setColorProgBar(true, false);
        }
    }


    public GameObject infoTextPanGo;
    private bool infoTextPan_activated = false;

    public void btnInfoText()
    {
        sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.infoBoost_btnInfo);
        infoTextPan_activated = !infoTextPan_activated;
        infoTextPanGo.SetActive(infoTextPan_activated);
    }
    #endregion


    [Header("Upgrade boost")]
    public Animation upgradeAnim;
    public Transform itemExample;
    //private bool closeUpgrade = false;
    public static bool upgrading = false;
    /*
     * void btnUpgrade(bool isBtnOn)
     * void btnCloseUpgrading()
     * IEnumerator animUpgrade()
     */
        #region Upgrade boost
    public void btnUpgrade(bool isBtnOn)
    {
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_buyUpgrade);
        if (isBtnOn)
        {
            validAchat.getInstance().openMenuValidAchat(true, currentItemBoost.GetComponent<itemBoost>().getPrice(), typeAchat.upgradeBoost);
        }
        else
        {
            sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
        }
    }

    public void validUpgradeBoost()
    {
        // on upgrade le boost
        upgrading = true;
        animUpgrade_2();
        //StartCoroutine(animUpgrade());
        //StartCoroutine(gameManager.getInstance().debiteMonney(currentItemBoost.GetComponent<itemBoost>().getPrice()));
    }

    /*public void btnCloseUpgrading()
    {
        if (!upgrading && !closeUpgrade)
        {
            Debug.Log("<color=red>SOUND SHOULD NOT BE CALLED</color>");
            closeUpgrade = true;
            sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_close);
        }
    }*/



    public menuUpgradeBoost menuUpgrade;
    private int nbUpgradeOpen;

    private void animUpgrade_2()
    {
        itemBoost iB = currentItemBoost.GetComponent<itemBoost>();
        int levelBoost = iB.niveauBoost; // typeBoost_name
        typeBoostGame typeB = iB.typeBoost_name;
        menuUpgrade.openMenuUpgradeFromShop(levelBoost, typeB, nbUpgradeOpen == 1);//, false);

        if (nbUpgradeOpen == 2)
        {
            //Debug.Log("(two) OPEN UPGRADE. Current =" + info_up_1.currVal + ", Added = " + info_up_1.addedVal);
            //typeValue[iB.upgrade1]
            menuUpgrade.openTwoUpgrade(
                info_up_1.myType, info_up_1.currVal, info_up_1.addedVal, typeValue[iB.upgrade1],
                info_up_2.myType, info_up_2.currVal, info_up_2.addedVal, typeValue[iB.upgrade2]);
        }
        else
        {
            //Debug.Log("(one) OPEN UPGRADE. Current =" + info_up_1.currVal + ", Added = " + info_up_1.addedVal + ", Type = "+ info_up_1.myType);
            if(info_up_1.addedVal != 0)
                menuUpgrade.openOneUpgrade(false, info_up_1.myType, info_up_1.currVal, info_up_1.addedVal, typeValue[iB.upgrade1]);
            else // Tirelire, upgrade nb coin ( 2e upgrade )
                menuUpgrade.openOneUpgrade(false, info_up_2.myType, info_up_2.currVal, info_up_2.addedVal, typeValue[iB.upgrade2]);
        }

        menuUpgrade.endOpen();
    }

    public void animEnded_updateMenuInfo()
    {
        currentItemBoost.GetComponent<itemBoost>().upgradeBoost();
        boostIsOpenned(currentItemBoost);
    }

    /*
     * 
        currentItemBoost.GetComponent<itemBoost>().upgradeBoost();
        boostIsOpenned(currentItemBoost);*/

    /*private IEnumerator animUpgrade()
    {
        GameObject o = Instantiate(currentItemBoost.gameObject, itemExample.parent);
        itemBoost iB = o.GetComponent<itemBoost>();
        o.transform.localScale = itemExample.localScale;
        o.transform.localPosition = itemExample.localPosition;
        o.transform.SetParent(itemExample);

        upgradeAnim.Play("upgradeBoost");

        yield return new WaitForSeconds(45 / 60f);
        upgradeAnim["upgradeBoost"].speed = 0f;
        sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.upgradeBoost);

        yield return new WaitForSeconds(1 / 6f);
        //iB.upgradeBoost(true);
        // on attend que l'anim du boost soit finit
        yield return new WaitUntil(predicate: () => iB.isUpgrading == false);

        upgrading = false;
        currentItemBoost.GetComponent<itemBoost>().upgradeBoost();
        boostIsOpenned(currentItemBoost);

        // on attend que player clique sur ecran pour fermer le menu
        yield return new WaitUntil(predicate: () => closeUpgrade == true);
        upgradeAnim["upgradeBoost"].speed = 1f;

        yield return new WaitForSeconds(45 / 60f);
        Destroy(o);
        closeUpgrade = false;
    }*/
    #endregion
}

// 353 l -> 323 l
