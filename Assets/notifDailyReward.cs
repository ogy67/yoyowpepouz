using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notifDailyReward : MonoBehaviour
{
    // à implementer
    // - click sur btn shop, si notifDailyReward_isOn = true, forcer ouvrir sur section dailyOffer, sur pos offer gratuit

    public static bool notifDailyOffer_isOn = false;
    public GameObject notifBtnShopGo; // par défaut est désactivé
    public GameObject notifOfferFreeGo; // par défaut est désactivé

    // Set notif ON
    // - Open app, si Offre gratuit pas prit, ouvrir
    // - DailyOfferTimeOut, update reward, ouvrir
    public void setNotifOn()
    {
        notifBtnShopGo.SetActive(true);
        notifOfferFreeGo.SetActive(true);
        notifDailyOffer_isOn = true;
    }

    // Set notif OFF
    // - Offre gratuit est réccupéré
    public void setNotifOff()
    {
        notifBtnShopGo.SetActive(false);
        notifOfferFreeGo.SetActive(false);
        notifDailyOffer_isOn = false;
    }
}
