using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debugVisuAdvertisment : MonoBehaviour
{
    public bool testModeOn = false;
    public Color colorPassed, colorError, colorForcePass;


    public Image img_connectedInternet;
    public void connectedInternet_changeState(bool passed)
    {
        if (!testModeOn)
            return;
        img_connectedInternet.color = passed ? colorPassed : colorError;
    }

    public Image img_adInitialized;
    public void initializationAd_changeState(bool passed)
    {
        if (!testModeOn)
            return;
        img_adInitialized.color = passed ? colorPassed : colorError;
    }

    /*public Image img_forcePassInitialisation;
    public void forcePassInitialisation()
    {
         img_forcePassInitialisation.color = colorForcePass;
    }*/

    public Image[] imgList_ads;
    public static Dictionary<typeAd, int> indexImg_byAd = new Dictionary<typeAd, int>()
    {
        {typeAd.daily_offer_ad, 0 },
        {typeAd.revive_ad, 1 },
        {typeAd.special_offer_ad, 2},
    };
    public static Dictionary<string, typeAd> nameAd_toType = new Dictionary<string, typeAd>()
    {
        {"daily_offer", typeAd.daily_offer_ad },
        {"revive", typeAd.revive_ad },
        {"special_offer_gem", typeAd.special_offer_ad},
    };
    public void loadAd_changeState(typeAd type, bool passed)
    {
        if (!testModeOn)
            return;
        imgList_ads[indexImg_byAd[type]].color = passed ? colorPassed : colorError;
    }

    public Image[] imgList_ads_reponse;
    public void checkIfAdIsAvailable(typeAd type, bool passed)
    {
        if (!testModeOn)
            return;
        imgList_ads_reponse[indexImg_byAd[type]].color = passed ? colorPassed : colorError;
    }
}
