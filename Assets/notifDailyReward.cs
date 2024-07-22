using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notifDailyReward : MonoBehaviour
{
    // � implementer
    // - click sur btn shop, si notifDailyReward_isOn = true, forcer ouvrir sur section dailyOffer, sur pos offer gratuit

    public static bool notifDailyOffer_isOn = false;
    public GameObject notifBtnShopGo; // par d�faut est d�sactiv�
    public GameObject notifOfferFreeGo; // par d�faut est d�sactiv�

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
    // - Offre gratuit est r�ccup�r�
    public void setNotifOff()
    {
        notifBtnShopGo.SetActive(false);
        notifOfferFreeGo.SetActive(false);
        notifDailyOffer_isOn = false;
    }
}
