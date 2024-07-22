using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum typeOffre
{
    yow,
    booster,
    boost,
    onlineTickets,
    diamond,
    none
}
public enum sousTypeOffre
{
    yow,
    booster_argent,
    booster_or,
    booster_Crystal,
    boost_common,
    boost_rare,
    boost_epic,
    boost_legendary,
    onlineTickets,
    diamond_ad,
    none
}
public enum payMode
{
    free,
    yow,
    ad,
    none
}

[System.Serializable]
public struct infoOffre
{
    public typeOffre type;
    public sousTypeOffre sousType;
    public payMode[] pay;
    public int[] quantity;
    public int yowPrice;
}

public class offreQuotidienne : MonoBehaviour
{
    public infoOffre[] _infos;
    public Text timerNewOffer;
    public static bool initDailyIsDone = false;

    // remplit en avance
    private readonly Dictionary<payMode, int> _indexPayMode = new Dictionary<payMode, int>()
    {
        {payMode.free, 0},
        {payMode.yow, 1 },
        {payMode.ad, 2 }
    };
    private readonly Dictionary<int, payMode> _indexPayMode_reverse = new Dictionary<int, payMode>()
    {
        {0, payMode.free},
        {1, payMode.yow},
        {2, payMode.ad}
    };
    private readonly Dictionary<sousTypeOffre, typeOffre> typeOffre_bySousType = new Dictionary<sousTypeOffre, typeOffre>()
    {
        {sousTypeOffre.booster_argent, typeOffre.booster},
        {sousTypeOffre.booster_or, typeOffre.booster},
        {sousTypeOffre.booster_Crystal, typeOffre.booster},
        {sousTypeOffre.boost_common, typeOffre.boost},
        {sousTypeOffre.boost_rare, typeOffre.boost},
        {sousTypeOffre.boost_epic, typeOffre.boost},
        {sousTypeOffre.boost_legendary, typeOffre.boost},
        {sousTypeOffre.yow, typeOffre.yow},
        {sousTypeOffre.onlineTickets, typeOffre.onlineTickets},
        {sousTypeOffre.diamond_ad, typeOffre.diamond},
    };

    // load data
    private static Dictionary<typeOffre, sousTypeOffre> previousDailyOffer_data = 
        new Dictionary<typeOffre, sousTypeOffre>();

    // se remplit seul
    private List<sousTypeOffre> previousDaily_sousOffer = new List<sousTypeOffre>();
    private Dictionary<typeOffre, sousTypeOffre> currentDailyOffer = new Dictionary<typeOffre, sousTypeOffre>();



    private static offreQuotidienne instance;
    public static offreQuotidienne getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<offreQuotidienne>();
        return instance;
    }

    // appelé quand boostManager a finit de load les boost dans leurs conteneurs
    public void canBeLoaded()
    {
        setMenuFromUpdateTimer = false;
        //Debug.Log("Init daily offer, app start");

        //long test_time = ElapsedTime();
        //long date_time = test_time - oneHourTime * 26;
        //Debug.Log("Test save prev date : " + date_time);
        // pour test on save une date precedente de 26h passé ( doit alors mettre current date à wait 22h )
        /*SaveSystem.Save(typeSave.SHOP_DAILY_DATE
            , date_time);*/

        initDailyOffer(false);
        StartCoroutine(updateCompteurTime());
        defiManager.getInstance().canBeLoaded();
    }

    private infoOffre getInfoOffre_bySousOffre(sousTypeOffre sTO)
    {
        foreach(infoOffre iO in _infos){
            if (iO.sousType == sTO) return iO;
        }
        return new infoOffre();
    }

    public notifDailyReward notifDailyRew;
    private static bool dailyOfferIsAlreadyLoaded = false;
    private long prevTime;
    private const long oneDayTime = 24 * 60 * 60 * 1000;
    private const long oneHourTime = 60 * 60 * 1000;
    private const long oneMinuteTime = 60 * 1000;
    /*
     * void initDailyOffer()
     * void setMenuOffers(bool newDay)
     * long getNewDate(long prevTime, long currentTime)
     * float getTimeSinceLastOffer()
     * int[] getTimeLeft_newOffer()
     * static long ElapsedTime()
     * IEnumerator updateCompteurTime()
    */
    #region initialisation / gestion timer

    //public bool test_forceReloadDaily = false;
    private void initDailyOffer(bool forceReload)
    {
        bool firstInit = (bool)SaveSystem.loadData(typeSave.SHOP_DAILY_DATE_INIT);

        object t = SaveSystem.loadData(typeSave.SHOP_DAILY_DATE);

        long dataTime = ElapsedTime();

        prevTime = firstInit ? dataTime : (long)t;

        //Debug.Log("Prev date retrieved : " + (long)t);
        //Debug.Log("Elapsed time : " + ElapsedTime() + ", prevTime : " + prevTime);
        //Debug.Log("Cur date - prev date = " + getTimeSinceLastOffer() + " , one day time : " + oneDayTime);

        if (!forceReload)
        {
            //Debug.Log("1");
            // si le temps depuis derniere offre est inférieur à 24h
            if (!firstInit 
                //&& !test_forceReloadDaily
                && getTimeSinceLastOffer() < oneDayTime)
            {
                //Debug.Log("Timer daily offer same day");
                // si on a pas encore load le menu, on le fait une seule fois par ouverture d'appli
                if (!dailyOfferIsAlreadyLoaded)
                {
                    
                    currentDailyOffer =
                        (Dictionary<typeOffre, sousTypeOffre>)SaveSystem.loadData(typeSave.SHOP_DAILY_OFFER);
                    // on load l'offre quotidienne depuis save ( déja calculée )
                    dailyOfferIsAlreadyLoaded = true;                  

                    if (currentDailyOffer == null
                        || currentDailyOffer.Count != 3)
                    {
                        forceSetDefaultDailyOfferList();
                        offerTaken = new Dictionary<payMode, bool>() { { payMode.free, false }, { payMode.yow, false }, { payMode.ad, false } };
                    }else
                    {
                        // on load offer taken depuis save
                        // il existe toujours une save de offreTaken si on est dans ce if
                        offerTaken = (Dictionary<payMode, bool>)SaveSystem.loadData(typeSave.SHOP_DAILY_OFFER_TAKEN);
                    }

                    setMenuOffers(false);
                    defiManager.getInstance().initDailyDefiTime(getTimeLeft_newOffer(), false);

                    // ici, vient juste d'ouvrir app, reccup prevData dailyDefi, temps pas écoulé
                    // si le defi quotidien n'est pas prit, activer notif
                    if(offerTaken[payMode.free] == false)
                    {
                        notifDailyRew.setNotifOn();
                    }
                }

                initDailyIsDone = true;
                // ne rien faire, joueur doit juste attendre
                return;
            }
        }
        // ---- nouvelle offre

        dailyOfferIsAlreadyLoaded = true;
        prevTime = !firstInit ? getNewDate((long)t, dataTime) : dataTime;
        SaveSystem.Save(typeSave.SHOP_DAILY_DATE
            , prevTime);
        if (firstInit)
            SaveSystem.Save(typeSave.SHOP_DAILY_DATE_INIT, false);
        // on reload offer taken
        offerTaken = new Dictionary<payMode, bool>(offerTaken_const);
        SaveSystem.Save(typeSave.SHOP_DAILY_OFFER_TAKEN, offerTaken_const);
        loadDailyOffer(firstInit);
        setMenuOffers(true);
        defiManager.getInstance().initDailyDefiTime(getTimeLeft_newOffer(), true);
        notifDailyRew.setNotifOn();

        initDailyIsDone = true;
    }

    public GameObject[] offersItem;
    public static int dailyBoost_id;
    private static bool setMenuFromUpdateTimer = false;
    private void setMenuOffers(bool newDay)
    {
        if (currentDailyOffer.ContainsKey(typeOffre.boost))
        {
            if (newDay)
            {
                sousTypeOffre sTO = currentDailyOffer[typeOffre.boost];
                typeRarety tR = sTO == sousTypeOffre.boost_common ?
                    typeRarety.common : sTO == sousTypeOffre.boost_rare ?
                    typeRarety.rare : sTO == sousTypeOffre.boost_epic ?
                    typeRarety.epic : typeRarety.legendary;
                // on génère un id de boost pour newDay, en fonction de sousTypeOffer
                dailyBoost_id = boostManager.getInstance().getRandomBoost_byRarety(tR);
                SaveSystem.Save(typeSave.SHOP_DAILY_ID_BOOST, dailyBoost_id);
            }
            else
            {
                // on reccupère l'id saved
                dailyBoost_id = (int)SaveSystem.loadData(typeSave.SHOP_DAILY_ID_BOOST);
            }
        }
        // on load menuOffer
        int i = 0;
        foreach(KeyValuePair<typeOffre, sousTypeOffre> k in currentDailyOffer)
        {
            StartCoroutine(offersItem[i].GetComponent<offerItem>().loadOffer(
                _indexPayMode_reverse[i]
                , getInfoOffre_bySousOffre(k.Value)
                , offerTaken[_indexPayMode_reverse[i]]
                , setMenuFromUpdateTimer));
            i++;
        }
    }

    // retourne la date actuelle mise a la même heure que prevTime
    // donne la date précédente la + proche de currentTime ( pour laisser - de 24h )
    private long getNewDate(long prevTime, long currentTime)
    {
        // oneDayTime = 8;
        // base 8 : 16 : 24 : 32 : 40 : 48 : 56

        // currentTime = 53
        // prevTime = 24

        // ecart = 53 - 24 -> 29
        long ecart = currentTime - prevTime;
        // ecart = 29 % 8 = 5
        ecart %= oneDayTime;
        // return 53 - 5 -> 48
        return currentTime - ecart;
    }

    public int delayTimerAdded_sec = 0; // test
    private long getTimeSinceLastOffer()
    {
        return ElapsedTime() - prevTime;

        /*long timeHourLong = ((ElapsedTime() - prevTime) + delayTimerAdded_sec * 1000) / 1000L // convertion en secondes
            / 60L // convertion en minutes
            / 60L // convertion en heure
            / 24L;
        /*float t = ((ElapsedTime() - prevTime) + delayTimerAdded_sec * 1000) / 1000f // convertion en secondes
            / 60f // convertion en minutes
            / 60f // convertion en heure
            / 24f;//
        Debug.Log("<color=blue> getTimeSinceLastOffer() : </color>" + timeHourLong + " (left; hour : "+ (timeHourLong * 24)+" ,min : "+ (timeHourLong * 24*60)+")");
        return timeHourLong;*/
    }
    public long[] getTimeLeft_newOffer()
    {
        // long est un entier

        long ecartLong = (ElapsedTime() - prevTime) + delayTimerAdded_sec * 1000;
        long nbHourPastLong = ecartLong / oneHourTime;
        long nbMinutePastLong = (ecartLong - ( nbHourPastLong * oneHourTime )) / oneMinuteTime;

        return new long[]
        {
            23 - nbHourPastLong,
            59 - nbMinutePastLong,
            //60 - (int)(ecart / 1000f),
        };
    }




    private void Start()
    {
        StartCoroutine(countTimeWait());
    }
    // on met la ref elapsedTime à jour quand init
    // et quand revient de pause ( jeu reouvert )
    private static long prevElapsedTimeRetrieved = -1L;
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //Debug.Log("Game is unPaused");
            currentIdWait++;

            DateTime t = DateTime.Now;
            prevElapsedTimeRetrieved = (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;
            /*if (Application.platform != RuntimePlatform.Android)
            {
                DateTime t = DateTime.Now;
                prevElapsedTimeRetrieved = (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;
            }
            else
            {
                AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
                // retourne des millisecondes
                prevElapsedTimeRetrieved = systemClock.CallStatic<long>("elapsedRealtime");
            }*/

            StartCoroutine(countTimeWait());
        }
    }

    private int currentIdWait = 0;
    private static float timeElapsed = 0f;
    private IEnumerator countTimeWait()
    {
        int idInit = currentIdWait;
        float initTime = Time.time;
        while(currentIdWait == idInit)
        {
            timeElapsed = Time.time - initTime;
            yield return null;
        }
    }

    public static long ElapsedTime()
    {
        if (prevElapsedTimeRetrieved == -1L)
        {
            DateTime t = DateTime.Now;
            prevElapsedTimeRetrieved = (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;

            /*if (Application.platform != RuntimePlatform.Android)
            {
                DateTime t = DateTime.Now;
                prevElapsedTimeRetrieved = (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;
            }
            else
            {
                AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
                // retourne des millisecondes
                prevElapsedTimeRetrieved = systemClock.CallStatic<long>("elapsedRealtime");
            }*/
        }
        
        return prevElapsedTimeRetrieved + (int)(timeElapsed * 1000); // en milli sec
            
        /*if (Application.platform != RuntimePlatform.Android)
        {
            DateTime t = DateTime.Now;
            return (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;
        }
        AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
        // retourne des millisecondes
        return systemClock.CallStatic<long>("elapsedRealtime");*/
    }

    private IEnumerator updateCompteurTime()
    {
        //yield return new WaitForSeconds(0.5f);
        long[] time;
        while (true)
        {
            time = getTimeLeft_newOffer();
            defiManager.getInstance().dailyOfferUpdateTime(time);

            timerNewOffer.text = traductionManager.getInstance().getTradShop(typeTraduction_shop.shop_pan_dailyOfferTime) 
                +" "+ time[0]
                + "<color=#B463C3>h</color> " + (time[1]+1L) + "<color=#B463C3>min</color>";
            // load la nouvelle offre du jour si timer ends
            if (getTimeSinceLastOffer() >= oneDayTime)
            {
                //Debug.Log("<color=blue>Compteur timer above 24h, reload daily</color>");
                SaveSystem.Save(typeSave.SHOP_DAILY_DATE, ElapsedTime());
                setMenuFromUpdateTimer = true;
                initDailyOffer(true);
                //StartCoroutine(loadLate());
                //loadDailyOffer();
            }
            yield return new WaitForSeconds(5f);
            
            if(packOffer.currentInstanceTryBuy != null)
            {
                packOffer.currentInstanceTryBuy.refreshTimer();
            }
        }
    }

    #endregion

    /*
     * void loadDailyOffer()
     * bool loadOffers()
     * typeOffre getPrevDayOffer_byTypePay(int index)
     * List<sousTypeOffre> getSousOfferAvailable(typeOffre avoidOnThisEmplacement, payMode pM)
     * void setOffer(typeOffre previousType, payMode pM)
    */
    #region creation de l'offre
    private int nbLoadDaily;

    private void loadDailyOffer(bool firstLoad_forceFreeCoin)
    {
        previousDailyOffer_data = (Dictionary<typeOffre, sousTypeOffre>)SaveSystem.loadData(typeSave.SHOP_DAILY_OFFER);
        previousDaily_sousOffer = new List<sousTypeOffre>();
        foreach (KeyValuePair<typeOffre, sousTypeOffre> k in previousDailyOffer_data)
            previousDaily_sousOffer.Add(k.Value);

        nbLoadDaily = 0;
        while (!loadOffers(firstLoad_forceFreeCoin) && nbLoadDaily < 100) nbLoadDaily++; // est mit a false si la generation de l'offre doit être refaite

        // lancé + de 15.000 générations, max atteind : 32, avec moyenne de 3
        if (nbLoadDaily == 100)
        {
            Debug.Log("<color=red>PROBLEME INIT DAILY OFFER, 100 générations</color>");
            // forcer générer une offre et la prendre quand même
            forceSetDefaultDailyOfferList();
        }
        else
        {
            Debug.Log("<color=green>INIT DAILY OFFER, " + nbLoadDaily + " générations</color>");
        }


        SaveSystem.Save(typeSave.SHOP_DAILY_OFFER, currentDailyOffer);
    }

    private void forceSetDefaultDailyOfferList()
    {
        currentDailyOffer.Clear();
        currentDailyOffer.Add(typeOffre.yow, sousTypeOffre.yow);
        currentDailyOffer.Add(typeOffre.booster, sousTypeOffre.booster_or);
        currentDailyOffer.Add(typeOffre.diamond, sousTypeOffre.diamond_ad);
    }

    private bool loadOffers(bool firstLoad_forceFreeCoin)
    {
        currentDailyOffer = new Dictionary<typeOffre, sousTypeOffre>();

        payMode[] pay;
        if (firstLoad_forceFreeCoin)
        {
            // on set payModeFree ici
            currentDailyOffer.Add(typeOffre.yow, sousTypeOffre.yow);
            pay = new payMode[] { payMode.yow, payMode.ad };
        }
        else
        {
            pay = new payMode[] { payMode.free, payMode.yow, payMode.ad };
        }

        foreach (payMode pM in pay)
        {
            typeOffre o = getPrevDayOffer_byTypePay(_indexPayMode[pM]);
            if (!setOffer(o, pM))
                return false;
        }
        return true;
    }

    private typeOffre getPrevDayOffer_byTypePay(int index)
    {
        int i_2 = 0;
        foreach(KeyValuePair<typeOffre, sousTypeOffre> k in previousDailyOffer_data)
        {
            if (i_2 == index)
                return k.Key;
            i_2++;
        }
        return typeOffre.none;
    }

    private bool setOffer(typeOffre previousType, payMode pM)
    {
        sousTypeOffre sO;
        List<sousTypeOffre> available = getSousOfferAvailable(previousType, pM);
        if (available.Count == 0)
            return false;
        sO = available[UnityEngine.Random.Range(0, available.Count)];
        //Debug.Log("Ajoute offre type "+ typeOffre_bySousType[sO]+" , sous type "+sO);
        currentDailyOffer.Add(typeOffre_bySousType[sO], sO);
        return true;
    }

    private List<sousTypeOffre> getSousOfferAvailable(typeOffre avoidOnThisEmplacement, payMode pM)
    {
        List<sousTypeOffre> ret = new List<sousTypeOffre>();
        bool avoidOnlineTickets = true;// lockInfinis.isUnlocked == false;

        foreach(infoOffre i in _infos)
        {
            bool avoidCheckSousOffre = i.type == typeOffre.yow || i.type == typeOffre.onlineTickets;
            if (
                // on ne propose pas ticket online si infinis pas unlock
                !(i.type == typeOffre.onlineTickets && avoidOnlineTickets)
                // on ne met pas le meme type d'offre sur le même emplacement
                && i.type != avoidOnThisEmplacement
                // on ne met pas la même offre 2 fois le meme jour
                && !currentDailyOffer.ContainsKey(i.type)
                // on ne met pas la meme sous offre que le jour precedent
                // si on ne parle pas d'offre yow et tickets
                && ( avoidCheckSousOffre || !previousDaily_sousOffer.Contains(i.sousType) )
                // on l'ajoute si peut se mettre sur cet emplacement
                && i.pay[_indexPayMode[pM]] != payMode.none
                ) 
                ret.Add(i.sousType);
        }

        return ret;
    }
    #endregion


    private Dictionary<payMode, bool> offerTaken_const = new Dictionary<payMode, bool>()
     {
             {payMode.free, false },
             {payMode.yow, false },
             {payMode.ad, false },
     };
    private Dictionary<payMode, bool> offerTaken;
    /*
     * void playerGotOffer(payMode pM)
    */
    #region reccup offre
    public void playerGotOffer(payMode pM)
    {
        offerTaken[pM] = true;
        SaveSystem.Save(typeSave.SHOP_DAILY_OFFER_TAKEN, offerTaken);
        if (pM == payMode.free)
            notifDailyRew.setNotifOff();
    }

    #endregion

    // appelé si un boost est amélioré ou monté de niveau
    // SI ce boost est dans l'offre quotidienne
    public void changeCurrentBoostValues()
    {
        // on prend l'item qui contient le boost
        // on upgrade l'item
        foreach(GameObject o in offersItem)
        {
            if (o.GetComponent<offerItem>()._iB != null)
                o.GetComponent<offerItem>().boostAsChangedValues();
        }
    }

    /*#region TEST
    public bool btnLoadOffer = false;
    public bool btnReloadOffer = false;
    private void Update()
    {
        if (btnLoadOffer)
        {
            btnLoadOffer = false;
            loadDailyOffer();
        }
        if (btnReloadOffer)
        {
            btnReloadOffer = false;
            setMenuFromUpdateTimer = true;
            initDailyOffer(true);
        }
    }
    #endregion*/

}
