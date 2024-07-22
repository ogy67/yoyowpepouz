using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeAchat
{
    upgradeBoost,
    openBooster,
    buySkin,
    getDailyOffer,
    getYowOffer
}

public class validAchat : MonoBehaviour
{
    private static validAchat instance;
    public static validAchat getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("menuValidAchat").GetComponent<validAchat>();
        return instance;
    }

    public Animation animMenu;
    public GameObject coinGo, gemGo;
    public Text coinText, gemText;
    private typeAchat currentAchat;
    private int currentPrice;
    private bool currentIsCoin;
    public static offerItem currentDailyOffer;
    // appelé par fonction qui veut valider un achat
    public void openMenuValidAchat(bool coin, int price, typeAchat typeA)
    {
        currentAchat = typeA;
        currentPrice = price;
        currentIsCoin = coin;

        animMenu.Play("openValidAchat");
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.openSousMenu_validAchat);
        coinGo.gameObject.SetActive(coin);
        gemGo.gameObject.SetActive(!coin);
        if (coin)
        {
            coinText.text = price.ToString();
            coinText.transform.GetChild(0).GetComponent<Text>().text = price.ToString();
        }
        else
        {
            gemText.text = price.ToString();
            gemText.transform.GetChild(0).GetComponent<Text>().text = price.ToString();
        }
    }

    public void btnCloseValid()
    {
        animMenu.Play("closeValidAchat");
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.closeSousMenu_validAchat);
    }

    public void btnValidAchat()
    {
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.btnReward_validAchat);
        animMenu.Play("btnValidAchat");
        StartCoroutine(sendValidationToCaller());
    }

    public float timeAnimClose = 1 / 3f;
    private List<typeAchat> achatNoWaitClose_list = new List<typeAchat>() { typeAchat.getYowOffer };
    private IEnumerator sendValidationToCaller()
    {
        yield return new WaitForSeconds(timeAnimClose);//achatNoWaitClose_list.Contains(currentAchat)? 0.1f: timeAnimClose);

        switch (currentAchat)
        {
            case typeAchat.upgradeBoost:
                infoBoostManager.getInstance().validUpgradeBoost();
                break;
            case typeAchat.buySkin:
                skinManager.getInstance().validBuySkin();
                break;
            case typeAchat.getYowOffer:
                yowOfferManager.getInstance().validAchatYow();
                break;
            case typeAchat.getDailyOffer:
                currentDailyOffer.validAchatDailyOffer();
                break;
            case typeAchat.openBooster:
                 // pas son reward playerWinLevel
                menuBooster.getInstance().validAchatBooster();
                break;
            default:
                break;
        }
        if (!currentIsCoin)
            gemmesManager.buy(currentPrice);
        else
            StartCoroutine(gameManager.getInstance().debiteMonney(currentPrice));

        yield return new WaitForSeconds(0.25f);

        if(currentAchat != typeAchat.upgradeBoost)
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
    }
}
