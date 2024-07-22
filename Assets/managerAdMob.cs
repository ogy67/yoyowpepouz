using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

public enum typeAd
{
    revive_ad,
    double_monney_ad,
    special_offer_ad,
    more_tickets_ad,
    daily_offer_ad,
}

// appId = "ca-app-pub-2373151997898497~2323824861";

// NON appId_TEST_android = "ca-app-pub-3940256099942544/5224354917";
// NON appId_TEST_ios = "ca-app-pub-3940256099942544/1712485313";
public class managerAdMob : MonoBehaviour
{
    // /!\ quand tes mode on, configurer l'id de l'app dans Editor, avec appId_TEST_android
    public bool testModeOn = true;
    private Dictionary<typeAd, RewardedAd> RewardedAdInstance_byType = new Dictionary<typeAd, RewardedAd>()
    {
        {typeAd.daily_offer_ad, null },
        {typeAd.revive_ad, null },
        {typeAd.special_offer_ad, null },
    };

    #region Variables IDs adMob


#if UNITY_ANDROID
    private Dictionary<typeAd, string> idAdByType = new Dictionary<typeAd, string>() {
        { typeAd.daily_offer_ad, "ca-app-pub-2373151997898497/1399294305"},
        { typeAd.revive_ad, "ca-app-pub-2373151997898497/3766383444"},
        { typeAd.special_offer_ad, "ca-app-pub-2373151997898497/7625737401" },
    };
    string testId_rewardedAd = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
    private Dictionary<typeAd, string> idAdByType = new Dictionary<typeAd, string>() {
        { typeAd.daily_offer_ad, "ca-app-pub-2373151997898497/9715430576"},
        { typeAd.revive_ad, "ca-app-pub-2373151997898497/6817804490"},
        { typeAd.special_offer_ad, "ca-app-pub-2373151997898497/2413910658" },
    };
    string testId_rewardedAd = "ca-app-pub-3940256099942544/1712485313";
#else
    private Dictionary<typeAd, string> idAdByType = new Dictionary<typeAd, string>() {
        { typeAd.daily_offer_ad, "unexpected_platform"},
        { typeAd.revive_ad, "unexpected_platform"},
        { typeAd.special_offer_ad, "unexpected_platform" },
    };
    string testId_rewardedAd = "unexpected_platform";
#endif
    #endregion

    private static managerAdMob instance;
    public static managerAdMob getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("launcher").GetComponent<managerAdMob>();
        return instance;
    }

    public debugVisuAdvertisment debugVisu;

    /*
     * void Start()
     * IEnumerator waitInitialization()
     * void LoadAllRewardedAd()
     * IEnumerator LoadRewardedAdUnit(typeAd type)
     * string getIdAdRewardByType(typeAd type) // renvoit testId si testMode ON
    */
    #region Initialisation
    private void Start()
    {
        StartCoroutine(waitInitialization());
    }

    private IEnumerator waitInitialization()
    {
        debugVisu.connectedInternet_changeState(false);

        bool hasInternet = false;
        bool wait_ping;
        while (!hasInternet)
        {
            wait_ping = true;
            StartCoroutine(launcher.checkInternetConnection((isConnected) =>
            {
                hasInternet = isConnected;
                wait_ping = false;
            }));
            while (wait_ping) yield return null;

            if(!hasInternet)
                yield return new WaitForSeconds(3f);
        }

        /*while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(2f);
        }*/
        debugVisu.connectedInternet_changeState(true);

        bool requestEnded = false;
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            requestEnded = true;
            Debug.Log("Ads is Initialized !!");
        });
        while (!requestEnded)
            yield return new WaitForSeconds(1f);
        LoadAllRewardedAd();
    }

    private void LoadAllRewardedAd()
    {
        debugVisu.initializationAd_changeState(true);
        foreach (typeAd type in new List<typeAd>() { typeAd.daily_offer_ad, typeAd.revive_ad, typeAd.special_offer_ad })
        {
            StartCoroutine(LoadRewardedAdUnit(type));
        }
    }

    private IEnumerator LoadRewardedAdUnit(typeAd type)
    {
        if(RewardedAdInstance_byType[type] != null)
        {
            RewardedAdInstance_byType[type].Destroy();
            RewardedAdInstance_byType[type] = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        string adUnitId = getIdAdRewardByType(type);

        bool requestEnded = false;
        bool requestSucced = false;
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            requestEnded = true;
            if (error != null && ad != null)
            {
                Debug.Log("Reward failed to load " + error);
                return;
            }
            Debug.Log("Reward Loaded !!");
            requestSucced = true;
            RewardedAdInstance_byType[type] = ad;
        });

        while (!requestEnded)
            yield return new WaitForSeconds(1f);
        if (requestSucced)
        {
            debugVisu.loadAd_changeState(type, true);
            StartCoroutine(trackAdLifeTime(type));
        }
        else
        {
            debugVisu.loadAd_changeState(type, false);
            // reload
            // PRENDRE EN COMPTE
            // Warning: Attempting to load a new ad from the ad request completion block
            // when an ad failed to load is strongly discouraged. If you must load an ad from the ad request completion block,
            // limit ad load retries to avoid continuous failed ad requests in situations such as limited network connectivity.
            bool hasInternet = false;
            bool wait_ping;
            while(!hasInternet)
            {
                wait_ping = true;
                StartCoroutine(launcher.checkInternetConnection((isConnected) =>
                {
                    wait_ping = false;
                    hasInternet = isConnected;
                }));
                while (wait_ping) yield return null;

                if (!hasInternet) yield return new WaitForSeconds(3f);
            }
            // quand a de l'internet , retester de load ad
            // -> reccursion
            yield return new WaitForSeconds(2f);
            StartCoroutine(LoadRewardedAdUnit(type));      
        }
    }

    private string getIdAdRewardByType(typeAd type)
    {
        if (testModeOn)
            return testId_rewardedAd;
        return idAdByType[type];
    }
    #endregion

    /*
     * IEnumerator trackAdLifeTime(typeAd type)
     * void resetTimeAdWasLoaded(typeAd type)
     */
    #region Gestion expiration Ad timeOut

    // TUTO : Since ads expire after an hour, you should clear this cache and reload with new ads every hour.
    private Dictionary<typeAd, float> timeWhenTypeAdWasLoaded = new Dictionary<typeAd, float>() {
        {typeAd.daily_offer_ad, 0f},
        {typeAd.revive_ad, 0f},
        {typeAd.special_offer_ad, 0f},
    };
    private float oneHour = 59 * 60f; // 59 min, pour être sûr
    private float timeWaitRecheckUpdate = 10f;
    // appelé quand une ad est loaded
    private IEnumerator trackAdLifeTime(typeAd type)
    {
        float initTimeTrack = Time.time;
        timeWhenTypeAdWasLoaded[type] = Time.time;

        while (timeWhenTypeAdWasLoaded[type] == initTimeTrack // vérifier que n'a pas été reset
            && (Time.time - initTimeTrack < oneHour))
            yield return new WaitForSeconds(timeWaitRecheckUpdate);

        if (Time.time - initTimeTrack >= oneHour)
            // temps écoulé, on doit reload l'ad, a éxpiré
            StartCoroutine(LoadRewardedAdUnit(type));
    }

    private void resetTimeAdWasLoaded(typeAd type)
    {
        timeWhenTypeAdWasLoaded[type] = 0f; // stop les coroutines
    }
    #endregion

    /*
     * bool AdIsReadyToShow(typeAd type)
     * void ShowRewardedAd(typeAd type)
     * void showAdResult(typeAd type, bool succed)
    */
    #region Gestion affichage ad
    public bool AdIsReadyToShow(typeAd type)
    {
        bool canShow = RewardedAdInstance_byType[type] != null && RewardedAdInstance_byType[type].CanShowAd();
        debugVisu.checkIfAdIsAvailable(type, canShow);
        return canShow;
    }

    public void ShowRewardedAd(typeAd type)
    {
        RewardedAdInstance_byType[type].OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            showAdResult(type, false);
        };

        RewardedAdInstance_byType[type].Show((Reward reward) =>
        {
            Debug.Log("End Show");
            // on part du principe que si arrive ici, pub a complété cycle sans pb
            showAdResult(type, true);
        });
    }

    private void showAdResult(typeAd type, bool succed)
    {
        Debug.Log("(Time:" + Time.time + ") Resultat showAd : " + succed);

        // When you are finished with a RewardedAd, make sure to call the Destroy() method before dropping your reference to it
        if (RewardedAdInstance_byType[type] != null)
        {
            RewardedAdInstance_byType[type].Destroy();
            RewardedAdInstance_byType[type] = null;
        }
        // This notifies the plugin that the object is no longer used and the memory it occupies can be reclaimed
        //. Failure to call this method results in memory leaks

        resetTimeAdWasLoaded(type);
        // Reload the ad so that we can show another as soon as possible.
        StartCoroutine(LoadRewardedAdUnit(type)); // callback pas bien set ?

        // FAIRE LES CALLBACKS -> donner récompenses
        switch (type){
            case typeAd.revive_ad:
                menuGameOver.adLifeSucced = succed ? 1 : -1;
                break;
            case typeAd.daily_offer_ad:
                offerItem.offerAdWellOpenned = succed ? 1 : 0;
                break;
            case typeAd.special_offer_ad:
                secretOffer.getInstance().resultGetOffer(succed ? true : false);
                break;
        }
    }
    #endregion
}


/*public void RewardedAdEvents(RewardedAd ad, typeAd type)
{
    // Raised when the ad is estimated to have earned money.
    ad.OnAdPaid += (AdValue adValue) =>
    {
        Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
            adValue.Value,
            adValue.CurrencyCode));
    };
    // Raised when an impression is recorded for an ad.
    ad.OnAdImpressionRecorded += () =>
    {
        Debug.Log("Rewarded ad recorded an impression.");
    };
    // Raised when a click is recorded for an ad.
    ad.OnAdClicked += () =>
    {
        Debug.Log("Rewarded ad was clicked.");
    };
    // Raised when an ad opened full screen content.
    ad.OnAdFullScreenContentOpened += () =>
    {
        Debug.Log("Rewarded ad full screen content opened.");
    };
    // Raised when the ad closed full screen content.
    ad.OnAdFullScreenContentClosed += () =>
    {
        Debug.Log("Rewarded ad full screen content closed.");
    };
    // Raised when the ad failed to open full screen content.
    ad.OnAdFullScreenContentFailed += (AdError error) =>
    {
        Debug.LogError("Rewarded ad failed to open full screen content " +
                       "with error : " + error);
    };
}*/
