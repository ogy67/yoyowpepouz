using CompleteProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct colorReward
{
    public Color color;
    public sousTypeOffre typeOffre;
}
public class offerItem : MonoBehaviour
{
    public Transform price_typeContent;
    public Transform diamondContent;
    public Transform imgOffer_content;
    public Transform booster_content;
    public Text nbGivedText;
    public Text priceText;
    public Text freeOfferText;

    public colorReward[] colorTextReward;


    private readonly Dictionary<payMode, int> indexTypeMode = new Dictionary<payMode, int>()
    {
        {payMode.free, 0 },
        {payMode.yow, 1 },
        {payMode.ad, 2 },
    };
    private readonly Dictionary<sousTypeOffre, string> nomOffer = new Dictionary<sousTypeOffre, string>()
    {
        {sousTypeOffre.booster_argent, "Booster Argent" },
        {sousTypeOffre.booster_or,"Booster Or" },
        {sousTypeOffre.booster_Crystal, "Booster Crystal" },
        {sousTypeOffre.yow, "Pile de Yow"},
        {sousTypeOffre.onlineTickets, "Tickets Online" },
        {sousTypeOffre.diamond_ad, "Gemmes" },
    };
    private readonly Dictionary<sousTypeOffre, int> rangBooster = new Dictionary<sousTypeOffre, int>()
    {
        {sousTypeOffre.booster_argent, 0 },
        {sousTypeOffre.booster_or, 1 },
        {sousTypeOffre.booster_Crystal, 2 },
    };
    private readonly Dictionary<typeOffre, int[]> indexImageOffer = new Dictionary<typeOffre, int[]>()
    {
        // free, yow, ad
        {typeOffre.yow, new int[]{0, -1, 1} },
        {typeOffre.onlineTickets, new int[]{2, -1, 3} },
        {typeOffre.booster, new int[]{4} },
        {typeOffre.boost, new int[]{5} },
        {typeOffre.diamond, new int[]{6} }
    };

    private void Start()
    {
        freeOfferText.text = traductionManager.getInstance().getTradShop(typeTraduction_shop.shop_free_offer);
    }

    public boostRewardAnim boostOfferItem;
    /*
     * IEnumerator loadOffer(payMode pM, infoOffre iO, bool offerAlreadyTaken, bool animNewDay)
     * void cleanOffer()
     * void boostAsChangedValues()
     * void setText(Text t, string s, bool childToo)
     * int getIndexImage(typeOffre tO, int indexPayMode)
    */
    #region Load Offer
    public IEnumerator loadOffer(payMode pM, infoOffre iO, bool offerAlreadyTaken, bool animNewDay)
    { 
        if (animNewDay && gameObject.activeInHierarchy) // si player est dans menu et attend nouvelle offre
        {
            GetComponent<Animation>().Play("offerItemUpdate");
            yield return new WaitForSeconds(2 / 6f);
        }
        _typePay = pM;
        _infoOffre = iO;
        cleanOffer();
        int index = indexTypeMode[pM];

        // on affiche l'image price
        offerIsAlreadyTaken = offerAlreadyTaken;
        if (!offerAlreadyTaken)
            price_typeContent.GetChild(index + 1).gameObject.SetActive(true);
        else
            offerTakenGo.SetActive(true);
        if (pM == payMode.yow)
            priceText.text = iO.yowPrice.ToString();

        // on affiche le nombre donné
        string nbGived = iO.type == typeOffre.booster ?
            "" :
            /*(iO.type == typeOffre.boost ? "+" : "")+*/ iO.quantity[index].ToString();

        Color c = Color.white;
        int i = 0;
        while (i < colorTextReward.Length)
        {
            if (colorTextReward[i].typeOffre == iO.sousType)
            {
                c = colorTextReward[i].color;
                break;
            }
            i++;
        }
        setText(nbGivedText, nbGived, true, c);

        // afficher l'image de l'offre
        Transform imgToShow = imgOffer_content.GetChild(getIndexImage(iO.type, index));
        imgToShow.gameObject.SetActive(true);
        if(iO.type == typeOffre.boost)
        {
            _iB = boostManager.getInstance().getBoostById(offreQuotidienne.dailyBoost_id);
            boostOfferItem.loadDailyOffer(_iB.typeBoost_name, offerIsAlreadyTaken);
        }
        else if(iO.type == typeOffre.booster)
        {
            // on affiche l'image du bon booster
            booster_content.GetChild(rangBooster[iO.sousType]).gameObject.SetActive(true);
        }
    }
    private void cleanOffer()
    {
        foreach (Transform t in new Transform[] { imgOffer_content, booster_content })
            foreach (Transform t_2 in t)
                t_2.gameObject.SetActive(false);
        for (int i = 1; i < price_typeContent.childCount; i++) // en 0 : bord bas
        {
            price_typeContent.GetChild(i).gameObject.SetActive(false);
        }
        _iB = null;
        offerIsAlreadyTaken = false;
    }
    public void boostAsChangedValues()
    {
        _iB = boostManager.getInstance().getBoostById(offreQuotidienne.dailyBoost_id).GetComponent<itemBoost>();
        boostOfferItem.loadDailyOffer(_iB.typeBoost_name, false);
    }
    private void setText(Text t, string s, bool childToo, Color textColor)
    {
        t.text = s;
        if (childToo)
        {
            t.transform.GetChild(0).GetComponent<Text>().text = s;
            t.transform.GetChild(0).GetComponent<Text>().color = textColor;
        }
    }
    private int getIndexImage(typeOffre tO, int indexPayMode)
    {
        int[] indexes = indexImageOffer[tO];
        if (indexPayMode > indexes.Length - 1)
            return indexes[0];
        return indexes[indexPayMode];
    }

    #endregion


    public GameObject noConnexionGo;
    public GameObject offerTakenGo;
    private payMode _typePay;
    private infoOffre _infoOffre;
    public itemBoost _iB;
    private bool offerIsAlreadyTaken = false;
    public void btnGetOffer()
    {      
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_getYowGemDaily);
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapOffer, GetComponent<RectTransform>());
        transform.parent.GetComponent<Animation>().Play("offerClicked");

        if (offerIsAlreadyTaken
            || _typePay == payMode.yow && gameManager.monnaieVal < _infoOffre.yowPrice)
        {
            sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
            return;
        }

        if (_typePay == payMode.ad)
        {
            StartCoroutine(launcher.checkInternetConnection((isConnected) => {
                if(!isConnected || (isConnected && !managerAdMob.getInstance().AdIsReadyToShow(typeAd.daily_offer_ad)))
                {
                    sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                    StartCoroutine(noConnexion());
                    return;
                }
                btnGetOffer_give();
            }));
        }
        else
        {
            btnGetOffer_give();
        }

        /*if (_typePay == payMode.ad 
            && 
            (Application.internetReachability == NetworkReachability.NotReachable
            || !managerAdMob.getInstance().AdIsReadyToShow(typeAd.daily_offer_ad)))
        {
            sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
            StartCoroutine(noConnexion());
            return;
        }
        //sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.infoBoost_btnInfo);
        switch (_typePay)
        {
            case (payMode.free):
                sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
                StartCoroutine(giveOffer(_typePay));
                break;
            case (payMode.yow):
                validAchat.currentDailyOffer = GetComponent<offerItem>();
                validAchat.getInstance().openMenuValidAchat(true, _infoOffre.yowPrice, typeAchat.getDailyOffer);
                break;
            case (payMode.ad):
                managerAdMob.getInstance().ShowRewardedAd(typeAd.daily_offer_ad);
                //launcher.getInstance().UserChoseToWatchAd(typeAd.daily_offer_ad);
                StartCoroutine(giveOffer(_typePay));
                break;
        }*/

    }

    private void btnGetOffer_give()
    {
        switch (_typePay)
        {
            case (payMode.free):
                sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
                StartCoroutine(giveOffer(_typePay));
                break;
            case (payMode.yow):
                validAchat.currentDailyOffer = GetComponent<offerItem>();
                validAchat.getInstance().openMenuValidAchat(true, _infoOffre.yowPrice, typeAchat.getDailyOffer);
                break;
            case (payMode.ad):
                managerAdMob.getInstance().ShowRewardedAd(typeAd.daily_offer_ad);
                //launcher.getInstance().UserChoseToWatchAd(typeAd.daily_offer_ad);
                StartCoroutine(giveOffer(_typePay));
                break;
        }
    }

    public void validAchatDailyOffer() // seulement pour COIN
    {
        StartCoroutine(giveOffer(_typePay));
    }

    public static int offerAdWellOpenned;
    private IEnumerator giveOffer(payMode pM)
    {
        if(pM == payMode.ad)
        {
            offerAdWellOpenned = -1;
            yield return new WaitUntil(predicate: () => offerAdWellOpenned != -1);
            if (offerAdWellOpenned != 1) // publicité pas bien load
            {
                offerAdWellOpenned = -1;
                yield break;
            }
            offerAdWellOpenned = -1;
        }

        // get offer succed
        offreQuotidienne.getInstance().playerGotOffer(_typePay);
        for (int i = 1; i < price_typeContent.childCount; i++) // en 0 : bord bas
        {
            price_typeContent.GetChild(i).gameObject.SetActive(false);
        }
        offerTakenGo.SetActive(true);
        offerIsAlreadyTaken = true;

        int nbToGive = _infoOffre.quantity[indexTypeMode[_typePay]];
        // donner la recompense
        switch (_infoOffre.type)
        {
            case typeOffre.diamond:
                popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbToGive, transform.position);
                break;
            case typeOffre.yow:
                popYowGemManager.getInstance().popRec_main(typeReward_game.yow, nbToGive, transform.position);
                break;
            //case typeOffre.onlineTickets:
            //    rewardMenu.getInstance().giveReward(typeReward.tickets_normal, new object[] { nbToGive });
                //break;
            case typeOffre.boost:
                boostOfferItem.modeMenuDaily_close();
                int nbBoostCurrent = _iB.currentNbCards;
                string sMax = _iB.getNbCardRequirered(_iB.niveauBoost);
                int nbBoostMax = sMax == itemBoost.max_string ? nbBoostCurrent : int.Parse(sMax);
                int levelBoost = _iB.niveauBoost;
                menuGetBoost.getInstance().openRewardBoost_daily(_iB.typeBoost_name, nbToGive, nbBoostCurrent, nbBoostMax, levelBoost);
                menuGetBoost.currentDailyOfferItem = GetComponent<offerItem>();
                _iB.saveDailyBoostRewardData(nbToGive, nbBoostMax);
                break;
            case typeOffre.booster:
                typeBooster tB = _infoOffre.sousType == sousTypeOffre.booster_argent ? typeBooster.boosterArgent :
                    _infoOffre.sousType == sousTypeOffre.booster_or ? typeBooster.boosterOr : typeBooster.boosterEpique;
                int r = tB == typeBooster.boosterArgent ? 0 : tB == typeBooster.boosterOr ? 1 : 2;

                // doit être géré par menuBooster
                
                menuGetBoost.getInstance().openMenuBooster(tB);
                //openBoosterManager.getInstance().buyBooster(r, true, tB);
                break;
        }
    }

    public void menuGetBoostOfferClosed()
    {
        boostOfferItem.modeMenuDaily_reopen();
    }

    private IEnumerator noConnexion()
    {
        noConnexionGo.SetActive(true);
        yield return new WaitForSeconds(1f);
        noConnexionGo.SetActive(false);
    }

    private void OnDisable()
    {
        if (noConnexionGo.activeSelf)
            noConnexionGo.SetActive(false);
    }
} 

// 315.L -> 277.L
