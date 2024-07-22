using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using UnityEngine.Advertisements;
//using UnityEngine.UI;


//using GooglePlayGames;
//using GooglePlayGames.BasicApi;

/*public enum typeAchievment
{
    crown,
    infinite,
    star
}*/



public class launcher : MonoBehaviour/*
    ,IUnityAdsInitializationListener
    ,IUnityAdsLoadListener
    ,IUnityAdsShowListener*/
{
    public ParticleSystem pS;
    public GameObject mainCanvas;
    public GameObject animStart;
    public GameObject noConnexion;
    public GameObject monnaieRecGo;

    //public GameObject TUTO;
    public GameObject APP;
    public static string versionJeu = "b-6.0.0";

    void Start()
    {
        APP.SetActive(false);
        animStart.SetActive(true);
    }

    public void endCheckCloudAccount()
    {
        onlineRankingManager.getInstance().playerPassTestConnectUnityCloud();
        SaveSystem.loadData(typeSave.MISSED_ACHIEVMENTS);
        SaveSystem.record_infinis = (int)SaveSystem.loadData(typeSave.RECORD_INFINIS);

        StartCoroutine(initApp());
    }

    public static launcher getInstance()
    {
        return GameObject.Find("launcher").GetComponent<launcher>();
    }

    private IEnumerator playIntroSfx()
    {
        // 15
        yield return new WaitForSeconds(15 / 60f);
        sfxGame.getInstance().playSfx_listIntroApp(typeSfx_introApp.arrivee_pop);

        // 35
        yield return new WaitForSeconds((35 - 15) / 60f);
        sfxGame.getInstance().playSfx_listIntroApp(typeSfx_introApp.player_come);

        // 55
        yield return new WaitForSeconds((55 - 35) / 60f);
        sfxGame.getInstance().playSfx_listIntroApp(typeSfx_introApp.name_pop);

        // 210
        yield return new WaitForSeconds((210 - 55) / 60f);
        sfxGame.getInstance().playSfx_listIntroApp(typeSfx_introApp.close);
    }

    public float timeAnimInitApp = 261 / 60f;// 293
    public menuVisuLevel menuVisu;
    public expertMode expertM;
    IEnumerator initApp()
    {       
        animStart.GetComponent<Animation>().Play("loadScreen");
        StartCoroutine(playIntroSfx());
        expertM.tryOpenExpertOpenApp();

        //Translator.getInstance().setGameWithLanguage();

        //bool tutoDone = (bool)SaveSystem.loadData(typeSave.TUTO);
        //if (tutoDone)

        float delay_preload_ui = 0.5f;

        menuVisu.initApp();

        yield return new WaitForSeconds(timeAnimInitApp - delay_preload_ui);
        //animStart.SetActive(false);

        //if (tutoDone)
        //{
            playerMovment.comportement = typeBehaviour.aventure;
            APP.SetActive(true);
            APP.GetComponent<appliManager>().lancerApp(false);
            pS.gameObject.SetActive(true);
        /*}
        else
        {
            playerMovment.comportement = typeBehaviour.tuto;
            TUTO.SetActive(true);
            TUTO.transform.GetChild(0).GetComponent<TutoManager>().loadTuto();
            SaveSystem.Save(typeSave.TUTO, true);
            musicManager.getInstance().initMusic_tuto();
        }*/

        yield return new WaitForSeconds(1f);
        animStart.SetActive(false);
    }




    /*
     * region -------------------- Initialisation de UnityAds
     * region -------------------- Load les differentes ad
     * region -------------------- Test si une ad est prête à être Loaded
     * region -------------------- Montrer une publicité
     * void giveLifeRec(bool succed)
     * region -------------------- FONCTIONS de IUnityAdsListener non utilisées
    */
    #region gestion PUBLICITE (initialisation/load/show)

    /*public debugVisuAdvertisment debugVisu;
    private static float timeWaitReloadAd = 3f;
    private string store_id_Android = "3509376", store_id_IOS = "3509377";
    private string revive_ad = "revive";
    private string daily_offer_ad = "daily_offer";
    private string special_offer_ad = "special_offer_gem";

    public static Dictionary<typeAd, string> nameAd = new Dictionary<typeAd, string>()
    {
        {typeAd.daily_offer_ad, "daily_offer" },
        {typeAd.revive_ad, "revive" },
        {typeAd.special_offer_ad, "special_offer_gem"},
    };*/



    /*
     * IEnumerator tryAuthenticate()
     * void auth_to_UnityAd()
     * void OnInitializationComplete()
     * void OnInitializationFailed(UnityAdsInitializationError error, string message)
     * IEnumerator retryInitializeAd()
    */
    #region -------------------- Initialisation de UnityAds
    /*private IEnumerator tryAuthenticate()
    {
        debugVisu.connectedInternet_changeState(false);
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(2f);
        }
        debugVisu.connectedInternet_changeState(true);
        // de la co, essaie de se connecter à google
        auth_to_UnityAd();
    }
    private void auth_to_UnityAd()
    {
        waitInitialisation = true;
        StartCoroutine(waitAdInitialized());

        if (!Advertisement.isInitialized)
            Advertisement.Initialize(Application.platform == RuntimePlatform.Android ? store_id_Android : store_id_IOS, false, this);
        else
            LoadAd();
    }
    public void OnInitializationComplete()
    {
        if (waitInitialisation)
        {
            // on load toute les pub
            LoadAd();
        }
    }
    private bool waitInitialisation = false;
    public float timeWaitInitialisationMax = 1f;
    private IEnumerator waitAdInitialized()
    {
        // si attend trop longtemps, forcer init ici
        float initT = Time.time;
        while(waitInitialisation 
            && Time.time - initT < timeWaitInitialisationMax)
        {
            yield return null;
        }

        if (waitInitialisation) // bloqué, pas de reponse, forcer
        {
            debugVisu.forcePassInitialisation();
            LoadAd();
        }
    }



    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        if (waitInitialisation)
        {
            waitInitialisation = false;
            debugVisu.initializationAd_changeState(false);
            // on essaie de se reconnecter
            StartCoroutine(retryInitializeAd());
        }
    }
    private IEnumerator retryInitializeAd()
    {
        yield return new WaitForSeconds(3f);

        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(2f);
        }
        auth_to_UnityAd();
    }*/
    #endregion

    /*
     * void LoadAd()
     * void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
     * IEnumerator adShouldBeReseted(string idAd, float timeWait)
    */
    #region -------------------- Load les differentes ad
    /*private void LoadAd()
    {
        waitInitialisation = false;
        debugVisu.initializationAd_changeState(true);
        foreach (string s in nameAd.Values)
        {
            Advertisement.Load(s, this);
        }
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        debugVisu.loadAd_changeState(placementId, false);
        StartCoroutine(adShouldBeReseted(placementId, timeWaitReloadAd));
    }
    private IEnumerator adShouldBeReseted(string idAd, float timeWait)
    {
        if (timeWait != 0)
            yield return new WaitForSeconds(timeWaitReloadAd);
        tabAdReady[idAd] = false;
        Advertisement.Load(idAd, this);
    }*/
    #endregion

    /*
     * void OnUnityAdsReady(string placementId)
     * bool adIsLoaded(string ad)
    */
    #region -------------------- Test si une ad est prête à être Loaded
    /*private Dictionary<string, bool> tabAdReady = new Dictionary<string, bool>() {
        //{ "more_monney", false},
        { "revive", false},
        { "daily_offer", false},
        { "special_offer_gem", false},
    };
    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad ready");
        tabAdReady[placementId] = true;
        debugVisu.loadAd_changeState(placementId, true);
    }
    public void OnUnityAdsAdLoaded(string placementId) {
        Debug.Log("Ad loaded");
        tabAdReady[placementId] = true;
        debugVisu.loadAd_changeState(placementId, true);
    }

    public bool adIsLoaded(string ad)
    {
        //return true; // TEST
        debugVisu.checkIfAdIsAvailable(ad, tabAdReady[ad]);
        return tabAdReady[ad]; //  && Advertisement.isInitialized;
    }*/
    #endregion

    /*
     * void UserChoseToWatchAd(typeAd tA)
     * void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
     * void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
     * void local_OnUnityAdsDidFinish(string placementId, UnityEngine.Advertisements.ShowResult showResult)
    */
    #region -------------------- Montrer une publicité
    /*public void UserChoseToWatchAd(typeAd tA)
    {
        if (adIsLoaded(nameAd[tA]))
        {
            tabAdReady[nameAd[tA]] = false;
            Advertisement.Show(nameAd[tA], this);          
        }
        else
        {
            if (tA == typeAd.revive_ad)
                giveLifeRec(false);
            else if (tA == typeAd.daily_offer_ad)
                offerItem.offerAdWellOpenned = 0;
            else if (tA == typeAd.special_offer_ad)
                secretOffer.getInstance().resultGetOffer(false);
        }
    }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //throw new System.NotImplementedException();
        local_OnUnityAdsDidFinish(placementId, ShowResult.Finished);
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //throw new System.NotImplementedException();
        local_OnUnityAdsDidFinish(placementId, ShowResult.Failed);
    }
    public void local_OnUnityAdsDidFinish(string placementId, UnityEngine.Advertisements.ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            if (placementId == revive_ad)
                giveLifeRec(true);
            else if (placementId == daily_offer_ad)
                offerItem.offerAdWellOpenned = 1;
            else if (placementId == special_offer_ad)
                secretOffer.getInstance().resultGetOffer(true);
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped
            || showResult == ShowResult.Failed)
        {
            if (placementId == revive_ad)
                giveLifeRec(false);
            else if (placementId == special_offer_ad)
                secretOffer.getInstance().resultGetOffer(false);
            // Do not reward the user for skipping the ad.
        }
        StartCoroutine(adShouldBeReseted(placementId, 1));
    }*/
    #endregion

    /*public void giveLifeRec(bool succed)
    {
        menuGameOver.adLifeSucced = succed ? 1 : -1;
    }*/

    /*
     * void OnUnityAdsAdLoaded(string placementId)
     * void OnUnityAdsDidError(string message)
     * void OnUnityAdsDidStart(string placementId)
     * void OnUnityAdsShowStart(string placementId)
     * void OnUnityAdsShowClick(string placementId)
    */
    #region -------------------- FONCTIONS de IUnityAdsListener non utilisées

    /*public void OnUnityAdsDidError(string message) { }
    public void OnUnityAdsDidStart(string placementId) { }
    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }*/
    #endregion

    #endregion


    private static float prevTimeCheckedInternet = -1f;
    private static float timeRecheckInternet = 20f;
    private static bool prevStateInternet = false;
    public static IEnumerator checkInternetConnection(Action<bool> action)
    {
        if (prevTimeCheckedInternet == -1f || Time.time - prevTimeCheckedInternet > timeRecheckInternet)
        {
            UnityWebRequest www = new UnityWebRequest("http://google.com");
            yield return www;

            prevTimeCheckedInternet = Time.time;
            prevStateInternet = www.error != null ? false : true;
        }
        action(prevStateInternet);
    }




    // fonction pour video
    // créer une liste de 100 entiers au hasard
    // trier cette liste
    // afficher item au milieu
    // afficher la moyenne de la liste

    /*private void test_sortList()
    {
        List<int> list = new List<int>();
        int tot = 0;
        int rand;
        for(int i = 0;i < 100; i++)
        {
            rand = Random.Range(0, 100);
            list.Add(rand);
            tot += rand;
        }
        Debug.Log("Moyenne Liste : " + (tot / 100f));

        List<int> sorted = new List<int>();
        int max;
        int rangRemove;
        while(list.Count > 0)
        {
            max = list[0];
            rangRemove = 0;
            for(int i = 1; i < list.Count; i++)
            {
                if (list[i] > max)
                {
                    max = list[i];
                    rangRemove = i;
                }
            }
            sorted.Add(list[rangRemove]);
            list.RemoveAt(rangRemove);
            Debug.Log("Item ("+sorted.Count+") sorted equals : " + sorted[sorted.Count - 1]);
        }
    }*/
}