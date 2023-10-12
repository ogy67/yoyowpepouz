using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct offreYow
{
    public int priceGemme;
    public int nbYow;
    public Transform offreTr;
}
public class yowOfferManager : MonoBehaviour
{
    public offreYow[] _offreYow;

    private static yowOfferManager instance;
    public static yowOfferManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<yowOfferManager>();
        return instance;
    }

    private int currentRangOffer;
    public void btnGetOffer(int rang)
    {
        
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_getYowGemDaily);
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapOffer, _offreYow[rang].offreTr.GetComponent<RectTransform>());
        _offreYow[rang].offreTr.parent.GetComponent<Animation>().Play("offerClicked");

        if (_offreYow[rang].priceGemme <= gemmesManager.nbGemmes)
        {
            validAchat.getInstance().openMenuValidAchat(false, _offreYow[rang].priceGemme, typeAchat.getYowOffer);
            currentRangOffer = rang;
            //sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
        }
        else
        {
            sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
        }
    }

    public void validAchatYow()
    {
        popYowGemManager.getInstance().popRec_main(typeReward_game.yow, _offreYow[currentRangOffer].nbYow, _offreYow[currentRangOffer].offreTr.position);
    }
}
