using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CompleteProject;

public enum offerGemmesType
{
    none,
    offer_90,
    offer_500,
    offer_1200,
}


[System.Serializable]
public struct offerGemmesInfo
{
    public GameObject offer;
    public GameObject noConnexion;
    public offerGemmesType type;
    public int nbGemmes;
}

public class gemmesManager : MonoBehaviour
{
    public offerGemmesInfo[] infoOffers;
    private offerGemmesInfo currentOffer;


    public static int nbGemmes = 0;
    public Text nbGemmesText;
    public Animation animGemmes;

    private static gemmesManager instance;
    public static gemmesManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<gemmesManager>();
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        nbGemmes = (int)SaveSystem.loadData(typeSave.GEMMES);
        setTextGemmes(nbGemmes);
    }

    /*
     * static bool validAchat(int priceGemmes)
     * void changeGemmeValue(bool up, int nbG)
     * IEnumerator animChangeNbGemmes(bool up, int nbG)
     * void setTextGemmes(int value)
    */
    #region gestion add / retrieve / affichage gemmes

    public void addGemme(bool pickUp)
    {
        //
        if (pickUp)
        {
            nbGemmes += 1;
            setTextGemmes(nbGemmes);
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);

            saveNbGemmes();
        }
        else
        {
            changeGemmeValue(true, 1);
        }
    }

    public static bool validAchat(int priceGemmes)
    {
        if (priceGemmes <= nbGemmes)
        {
            getInstance().changeGemmeValue(false, priceGemmes);
            SaveSystem.Save(typeSave.GEMMES, nbGemmes - priceGemmes );
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.use_yowGem);
        }
        return priceGemmes <= nbGemmes;
    }

    public static void buy(int priceGemmes)
    {
        getInstance().changeGemmeValue(false, priceGemmes);
        SaveSystem.Save(typeSave.GEMMES, nbGemmes - priceGemmes);
        // supp : remplacer ?
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.use_yowGem);
    }

    public static void saveNbGemmes()
    {
        SaveSystem.Save(typeSave.GEMMES, nbGemmes);
    }

    public void changeGemmeValue(bool up, int nbG)
    {
        StartCoroutine(animChangeNbGemmes(up, nbG));
    }

    private IEnumerator animChangeNbGemmes(bool up, int nbG)
    {
        if(!up)
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.pay);
        animGemmes.Play("monnaie" + (up ? "Incremente" : "Debite"));
        yield return new WaitForSeconds(up ? 7 / 60f : 17 / 60f);

        int value = nbGemmes + (up ? nbG : -nbG);
        nbGemmes = value;

        setTextGemmes(value);
    }

    private void setTextGemmes(int value)
    {
        // on veut 5 digit
        int lengthValue = value.ToString().Length;
        int nbZero = 5 - lengthValue;
        /*int nbZero = value < 10 ?
            3 : value < 100 ?
            2 : value < 1000 ?
        1 : 0;*/

        string zeroTxt = "";

        if (nbZero > 0)
        {
            while (nbZero > 0)
            {
                zeroTxt += '0';
                nbZero--;
            }
        }

        nbGemmesText.transform.parent.GetComponent<Text>().text = zeroTxt + value.ToString();
        nbGemmesText.text = "<color=\"#689369\">"+zeroTxt+ "</color>" + value.ToString();
    }

    #endregion

    /*
     * offerGemmesInfo getType(GameObject o)
     * void btnGetOffer(GameObject caller)
     * IEnumerator noConnexion(GameObject noConnecGo)
     * void btnBuyMoreMonney(GameObject noConnecGo, offerGemmesType oY)
     * IEnumerator waitPurchase_moreGemmes()
     * void moreGemmesPurchaseResult(bool succed)
    */
    #region Achat de gemmes
    private offerGemmesInfo getType(GameObject o)
    {
        foreach (offerGemmesInfo oY in infoOffers)
        {
            if (oY.offer == o)
                return oY;
        }
        return new offerGemmesInfo();
    }

    public void btnGetOffer(GameObject caller)
    {
        
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_getYowGemDaily);
        currentOffer = getType(caller);
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapOffer, currentOffer.offer.GetComponent<RectTransform>());
        currentOffer.offer.transform.parent.GetComponent<Animation>().Play("offerClicked");

        // pas de pub pour gem
        /*if (currentOffer.type == offerGemmesType.offer_10)
        {
            // publicité
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (!currentOffer.noConnexion.activeSelf)
                {
                    sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                    StartCoroutine(noConnexion(currentOffer.noConnexion));
                }
            }
            else
            {
                sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.infoBoost_btnInfo);
                launcher.getInstance().UserChoseToWatchAd(typeAd.more_monney_ad);
            }
        }
        else
        {*/
            // argent
            btnBuyMoreMonney(currentOffer.noConnexion, currentOffer.type);
        //}
    }

    private IEnumerator noConnexion(GameObject noConnecGo)
    {
        noConnecGo.SetActive(true);
        yield return new WaitForSeconds(1f);
        noConnecGo.SetActive(false);
    }

    private static bool purchaseEnd = false, purchaseSucced = false, isGivingRec = false;
    public void btnBuyMoreMonney(GameObject noConnecGo, offerGemmesType oY)
    {
        StartCoroutine(launcher.checkInternetConnection((isConnected) => {
            if (isConnected)
            {
                if(!isGivingRec)
                {
                    sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
                    purchaseEnd = false;
                    purchaseSucced = false;
                    StartCoroutine(waitPurchase_moreGemmes());
                    GetComponent<Purchaser>().BuyMoreMonney(currentOffer.type);
                }
            }
            else
            {
                if (!noConnecGo.activeSelf)
                {
                    sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                    StartCoroutine(noConnexion(noConnecGo));
                }
            }
        }));
        /*if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!noConnecGo.activeSelf)
            {
                sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                StartCoroutine(noConnexion(noConnecGo));
            }
        }
        else if (!isGivingRec)
        {
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
            purchaseEnd = false;
            purchaseSucced = false;
            StartCoroutine(waitPurchase_moreGemmes());
            GetComponent<Purchaser>().BuyMoreMonney(currentOffer.type);
        }*/
    }
    private IEnumerator waitPurchase_moreGemmes()
    {
        isGivingRec = true;
        while (!purchaseEnd)
            yield return null;

        purchaseEnd = false; // remet valeure par defaut
        typeRewardBuyed b = currentOffer.type == offerGemmesType.offer_90 ? typeRewardBuyed.gem_1
                : currentOffer.type == offerGemmesType.offer_500 ? typeRewardBuyed.gem_2
                : typeRewardBuyed.gem_3;

        if (purchaseSucced)// Si achat fonctionne
        {
            popYowGemManager.getInstance().popRec_main(typeReward_game.gem, currentOffer.nbGemmes, currentOffer.offer.transform.position);
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
            ensurePurchaseReward.getInstance().playerBuyedListReward(
                new List<typeRewardBuyed>() { b });
            //.
            //rewardMenu.getInstance().giveReward(typeReward.gemmes, new object[] { currentOffer.nbGemmes });
            //launcher.getInstance().give_buyMoreMonney_rec(posSpawnBuyMoreMonney.position);
        }
        yield return new WaitForSeconds(1f);
        isGivingRec = false;
        yield return new WaitForSeconds(0.3f);
        if (purchaseSucced)
        {
            Debug.Log("<color=red>End give reward gem ( buy )</color>");
            ensurePurchaseReward.getInstance().playerGetBuyedReward(b);
        }
    }

    public static void moreGemmesPurchaseResult(bool succed)
    {
        purchaseEnd = true;
        purchaseSucced = succed;
    }

    public void adGemmeFinishedSucced()
    {
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, currentOffer.nbGemmes, currentOffer.offer.transform.position);
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
    }

    #endregion
}
