using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

//using GooglePlayGames;
//using GooglePlayGames.BasicApi;

public enum typeAchievment
{
    crown,
    infinite,
    star
}

public enum typeAd
{
    //more_monney_ad,
    revive_ad,
    double_monney_ad,
    special_offer_ad,
    more_tickets_ad,
    daily_offer_ad,
}

public class launcher : MonoBehaviour
    ,IUnityAdsInitializationListener
    ,IUnityAdsLoadListener
    ,IUnityAdsShowListener
     //IUnityAdsListener
{
    public ParticleSystem pS;
    public GameObject mainCanvas;
    public GameObject animStart;
    public GameObject noConnexion;
    public GameObject monnaieRecGo;
    //public Transform monnaieSpawnPos;
    public GameObject TUTO;
    public GameObject APP;

    private string store_id = "3509376";
    //private string monney_ad = "more_monney";
    private string revive_ad = "revive";
    //private string double_monney_ad = "double_monney";
    //private string special_offer_ad = "special_offer_monney";
    //private string more_tickets_ad = "more_tickets";
    private string daily_offer_ad = "daily_offer";
    private string special_offer_ad = "special_offer_gem";

    public static Dictionary<typeAd, string> nameAd = new Dictionary<typeAd, string>()
    {
        {typeAd.daily_offer_ad, "daily_offer" },
        //{typeAd.more_monney_ad, "more_monney" },
        {typeAd.revive_ad, "revive" },
        {typeAd.special_offer_ad, "special_offer_gem"},
        //{typeAd.double_monney_ad, "double_monney" },
        //{typeAd.special_offer_monney_ad, "special_offer_monney" },
        //{typeAd.more_tickets_ad, "more_tickets" },
    };


    //public Text versionJeuText;
    public static string versionJeu = "b-5.0.1";

    //private bool noCoOn = false; // si affichage de non connexion ouvert

    void Start()
    {

        //SaveSystem.Save(typeSave.MONNEY, 20000);
        //versionJeuText.text = versionJeu;
        APP.SetActive(false);
        TUTO.SetActive(false);


        //SaveSystem.checkDataSavedNotCorrupted();

        SaveSystem.loadData(typeSave.MISSED_ACHIEVMENTS);
        SaveSystem.record_infinis = (int)SaveSystem.loadData(typeSave.RECORD_INFINIS);

        //menuShop.fondUnlocked = (object[])SaveSystem.loadData(typeSave.FOND_BUYED);

        //Screen.SetResolution(1280, 720, true, 60);

        StartCoroutine(initApp());
        StartCoroutine(tryAuthenticate());
    }

    private IEnumerator tryAuthenticate()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(2f);
        }
        // de la co, essaie de se connecter à google
        auth_to_UnityAd();
    }

    private void auth_to_UnityAd()
    {
        Advertisement.Initialize(store_id, false, this);

        //  ADD THIS CODE BETWEEN THESE COMMENTS

        /* Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        // END THE CODE TO PASTE INTO START

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);*/
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
    IEnumerator initApp()
    {
        animStart.SetActive(true);
        animStart.GetComponent<Animation>().Play("loadScreen");
        StartCoroutine(playIntroSfx());

        //Translator.getInstance().setGameWithLanguage();

        bool tutoDone = (bool)SaveSystem.loadData(typeSave.TUTO);
        //if (tutoDone)

        float delay_preload_ui = 0.5f;

        menuVisu.initApp();

        yield return new WaitForSeconds(timeAnimInitApp - delay_preload_ui);
        animStart.SetActive(false);

        if (tutoDone)
        {
            playerMovment.comportement = typeBehaviour.aventure;
            APP.SetActive(true);
            APP.GetComponent<appliManager>().lancerApp(false);
            pS.gameObject.SetActive(true);
        }
        else
        {
            playerMovment.comportement = typeBehaviour.tuto;
            TUTO.SetActive(true);
            TUTO.transform.GetChild(0).GetComponent<TutoManager>().loadTuto();
            SaveSystem.Save(typeSave.TUTO, true);
            musicManager.getInstance().initMusic_tuto();
        }

        yield return new WaitForSeconds(1f);

        //updateAchievment(typeAchievment.infinite);
    }
    /*public void redoTuto()
    {
        APP.SetActive(false);
        mainCanvas.SetActive(false);
        pS.gameObject.SetActive(false);
        playerMovment.comportement = typeBehaviour.tuto;
        TUTO.SetActive(true);
        TUTO.GetComponent<tutorielManager>().initTuto();
        //SaveSystem.Save(typeSave.TUTO, true);
    }*/

    /*
     * void SignIn()
     * void SignInCallback(bool success)
     * region Achievments
     * region LeaderBoard
    */
    #region PLAY STORE , off , etait pour leaderboard et achievment
    /*public void SignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }

    }*/
    /*public void SignInCallback(bool success)
    {
        if (success)
        {
            //updateAllAchievment();

            Debug.Log("(Lollygagger) Signed in!");
            //updateLeaderRecordInfinis()

            //if ((bool)dataMissedValue[3] == false) // si des maj ont été manqués
            //{
            //    updateMissedValues();
            //}

            //updateAllAchievments();

            if (signForLeaderBoard)
            {
                signForLeaderBoard = false;
                btnOpenLeaderBoards_off();
            }
            else if (signForAchievment)
            {
                signForAchievment = false;
                ShowAchievements();
            }
            // Change sign-in button text
            //signInButtonText.text = "Sign out";

            // Show the user's name
            //authStatus.text = "Signed in as: " + Social.localUser.userName;
        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...");
            // Show failure message
            //signInButtonText.text = "Sign in";
            //authStatus.text = "Sign-in failed";
        }
    }*/
    /*
     * void updateAllAchievments()
     * void updateAchievment(typeAchievment type, int newValue)
     * void ShowAchievements()
     * IEnumerator afficherVoyantNoCoAchievments()
    */
    #region ACHIEVMENTS

    /*private void updateMissedValues()
    {
        if ((int)dataMissedValue[0] > 0)
            updateAchievment(typeAchievment.crown, (int)dataMissedValue[0], true);
        if ((int)dataMissedValue[1] > 0)
            updateAchievment(typeAchievment.infinite, (int)dataMissedValue[1], true);
        if ((int)dataMissedValue[2] > 0)
            updateAchievment(typeAchievment.star, (int)dataMissedValue[2], true);

        // dataMissedValue est mit a jour pendant que valeures sont traitées
        SaveSystem.missedGooglePlayValues = dataMissedValue;
        SaveSystem.Save(typeSave.MISSED_ACHIEVMENTS, dataMissedValue);
    } // met a jour les valeures manquées, et réinitialise save -> miss

    private void updateSucced(typeAchievment type, bool success, int addingValue, bool missedMode)
    {
        Debug.Log("La maj du defi -" + type + "- a réussit -" + success + "- avec ajout de -"+addingValue+"- unité(s)");

        int index = type == typeAchievment.crown ? 0 : type == typeAchievment.infinite ? 1 : 2;
        if (!success)
        {
            // si incremente, retiens le miss
            // sinon si juste maj de miss, et que fail, garder sa valeure. ( sinon miss est doublé )
            if (!missedMode)
            {
                dataMissedValue[index] = (int)dataMissedValue[index] + addingValue;       
            }
            // le prochain login fera alors la maj
            dataMissedValue[3] = false;

            // enregistrer
            SaveSystem.missedGooglePlayValues = dataMissedValue;
            SaveSystem.Save(typeSave.MISSED_ACHIEVMENTS, dataMissedValue);
        }
        else if(missedMode) // si la maj a fonctionné : maj val missed -> remise a zero
        {
            dataMissedValue[index] = 0;
        }
    }*/

    //private List<object> dataMissedValue;

    // appelé quand se connecte au PS

    /*bool signForAchievment = false;
    //public GameObject voyantNoCoAchievment;

    private void updateAllAchievments_off()
    {
        updateAchievment_off(typeAchievment.crown, SaveSystem.nb_couronne);
        updateAchievment_off(typeAchievment.infinite, SaveSystem.record_infinis);
        updateAchievment_off(typeAchievment.star, SaveSystem.getNbStarObtain());
    }

    public void updateAchievment_off(typeAchievment type, int newValue/*, bool missMode)
    {
        //dataMissedValue = SaveSystem.missedGooglePlayValues;
        
        // Only do achievements if the user is signed in
        if (Social.localUser.authenticated)
        {
            switch (type)
            {
                case typeAchievment.crown:
                    PlayGamesPlatform.Instance.ReportProgress(
                        GPGSIds.achievement_poignet_de_couronnes__dbutant,
                            newValue / 10f >= 1 ? 100f : 0f 
                            , (bool success) =>
                            {
                                //updateSucced(type, success, addingValue, missMode);
                            });
                    PlayGamesPlatform.Instance.ReportProgress(
                            GPGSIds.achievement_sac_de_couronnes__intermdiaire,
                                newValue / 50f >= 1 ? 100f : 0f
                                , (bool success) =>
                                {
                                    //updateSucced(type, success, addingValue, missMode);
                                });
                    PlayGamesPlatform.Instance.ReportProgress(
                            GPGSIds.achievement_flope_de_couronnes__confirm,
                                newValue / 100f >= 1 ? 100f : 0f, (bool success) =>
                                {
                                    //updateSucced(type, success, addingValue, missMode);
                                });
                    break;

                case typeAchievment.infinite:
                    PlayGamesPlatform.Instance.ReportProgress(
                       GPGSIds.achievement_brindille_dinfinis,
                           newValue / 10f >= 1 ? 100f : 0f, (bool success) =>
                           {
                               //updateSucced(type, success, addingValue, missMode);
                           });
                    PlayGamesPlatform.Instance.ReportProgress(
                      GPGSIds.achievement_voyageur_expriment,
                          newValue / 50f >= 1 ? 100f : 0f, (bool success) =>
                          {
                              //updateSucced(type, success, addingValue, missMode);
                          });
                    break;

                case typeAchievment.star:
                    /*Social.ReportProgress(
                       GPGSIds.achievement_une_toile__lhorizon,
                           100.0f, (bool success) =>
                           {
                               updateSucced(type, success, 100);
                           });*
                    PlayGamesPlatform.Instance.ReportProgress(
                       GPGSIds.achievement_ciel_toil,
                            newValue / 10f >= 1 ? 100f : 0f, (bool success) =>
                            {
                                //updateSucced(type, success, addingValue, missMode);
                            });
                    PlayGamesPlatform.Instance.ReportProgress(
                       GPGSIds.achievement_voyageur_spatiale,
                           newValue / 30f >= 1 ? 100f : 0f, (bool success) =>
                           {
                               //updateSucced(type, success, addingValue, missMode);
                           });
                    break;
            }
        } // end of isAuthenticated

        //else // enregistrer la valeure non prises en comptes
        //    updateSucced(type, false, addingValue, missMode);
     
    }*/

    /*public void ShowAchievements()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable) {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                //updateAllAchievment();
                PlayGamesPlatform.Instance.ShowAchievementsUI();
            }
            else
            {
                signForAchievment = true;
                SignIn();
                //Debug.Log("Cannot show Achievements, not logged in");
            }
        }
        /*else // afficher le voyant noCo
        {
            StartCoroutine(afficherVoyantNoCoAchievments());
        }
    }*/

    /*private IEnumerator afficherVoyantNoCoAchievments()
    {
        voyantNoCoAchievment.SetActive(true);
        yield return new WaitForSeconds(1f);
        voyantNoCoAchievment.SetActive(false);
    }*/

    #endregion

    /*
     * void btnOpenLeaderBoards()
     * void updateLeaderRecordInfinis()
     * IEnumerator afficherVoyantNoCoLeaderBoard()
    */
    #region LEADER BOARD

    /*bool signForLeaderBoard = false;
    //public GameObject voyantNoCoLeaderBoard;
    //bool lastUpdateRecordSuccessed = false;
    public void btnOpenLeaderBoards_off()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {
                // Sign in with Play Game Services, showing the consent dialog
                // by setting the second parameter to isSilent=false.
                signForLeaderBoard = true;
                SignIn();
            }
            else
            {
                //if (!lastUpdateRecordSuccessed)
                updateLeaderRecordInfinis_off();
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            }
        }
        /*else
        {
            StartCoroutine(afficherVoyantNoCoLeaderBoard());
        }
    }
    public void updateLeaderRecordInfinis_off()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Note: make sure to add 'using GooglePlayGames'
            PlayGamesPlatform.Instance.ReportScore(SaveSystem.record_infinis,
                GPGSIds.leaderboard_record_infinis,
                (bool success) =>
                {
                    Debug.Log("(Lollygagger) Leaderboard update success: " + success);
                    //lastUpdateRecordSuccessed = success;
                });
                
        }
    }   */
    /*private IEnumerator afficherVoyantNoCoLeaderBoard()
    {
        voyantNoCoLeaderBoard.SetActive(true);
        yield return new WaitForSeconds(1f);
        voyantNoCoLeaderBoard.SetActive(false);
    }*/



    #endregion

    #endregion

    /*
     * region RECOMPENSE AD
     * region INTERACTION AD
    */
    #region PUBLICITE
    /*
     * void giveLifeRec(bool succed)
     * IEnumerator giveMonneyRec()
    */
    #region RECOMPENSE AD
    public void giveLifeRec(bool succed)
    {
        menuGameOver.adLifeSucced = succed ? 1 : -1;
        //gameOverManager.adLifeSucced = succed ? 1 : -1;
    }
    /*public IEnumerator giveMonneyRec(int nbCoin, Vector2 posRef, int nbPerFrame, int showCoinEvery)
    {
        // nbCoin : ( ad : 30 ) , ( buy : 500 )
        // nbCoin : ( posRef : monnaieSpawnPos ) , ( buy :  )

        for (int i = 0; i < nbCoin; i++)
        {
            if(showCoinEvery == -1 || i % showCoinEvery == 0)
            {
                GameObject p = Instantiate(monnaieRecGo, GameObject.Find("mainCanvas").transform);
                p.transform.localScale = monnaieRecGo.transform.localScale;
                p.GetComponent<recompenseDefi>().spawn(posRef
                    + new Vector2(UnityEngine.Random.Range(-130, 130), UnityEngine.Random.Range(-130, 130)));
            }
            // ne pas montrer le coin
            else
            {
                StartCoroutine(giveLateMonney());
            }

            if (i % nbPerFrame == 0)
                yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f); // on sauvegarde la recompense
        SaveSystem.Save(typeSave.MONNEY, gameManager.monnaieVal);
    }*/

    /*private IEnumerator giveLateMonney()
    {
        yield return new WaitForSeconds(0.2f);
        gameManager.getInstance().giveRecompense(recompense.ARGENT);
    }*/

    /*public void give_buyMoreMonney_rec(Vector2 posSpawn)
    {
        StartCoroutine(giveMonneyRec(500, posSpawn, 10, 10));
    }*/

    #endregion

    /*
     * void UserChoseToWatchAd(typeAd tA)
     * bool adIsLoaded(string ad)
     * void OnUnityAdsDidFinish(string placementId, UnityEngine.Advertisements.ShowResult showResult)
    */
    #region INTERACTION AD

    private Dictionary<string, bool> tabAdReady = new Dictionary<string, bool>() {
        //{ "more_monney", false},
        { "revive", false},
        { "daily_offer", false},
        { "special_offer_gem", false},
    };

    public void OnUnityAdsAdLoaded(string placementId)
    {
        //throw new System.NotImplementedException();
        tabAdReady[placementId] = true;
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Initialisation Ad complete");
        //throw new System.NotImplementedException();
        // on load toute les pub
        foreach (string s in nameAd.Values)
        {
            if (!tabAdReady[s])
                Advertisement.Load(s, this);
        }
    }

    private static float timeWaitReloadAd = 3f;
    private IEnumerator adShouldBeReseted(string idAd, float timeWait)
    {
        if (timeWait != 0)
            yield return new WaitForSeconds(timeWaitReloadAd);
        tabAdReady[idAd] = false;
        Advertisement.Load(idAd, this);
    }



    public bool adIsLoaded(string ad)
    {
        return Advertisement.isInitialized && tabAdReady[ad];
        // PB , retourne toujours true meme si ad pas loaded
        //return Advertisement.isInitialized;
    }
    public void UserChoseToWatchAd(typeAd tA)
    {
        if (adIsLoaded(nameAd[tA]))
        {
            // normalement déjà loaded sinon ne peut passer dans le if
            //Advertisement.Load(nameAd[tA], this);
            Advertisement.Show(nameAd[tA], this);
        }
        else
        {
            /*if (tA == typeAd.double_monney_ad)
                gameManager.getInstance().doublePieceAdLoadSucced(false);
            else */
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
        OnUnityAdsDidFinish(placementId, ShowResult.Finished);
    }
    public void OnUnityAdsDidFinish(string placementId, UnityEngine.Advertisements.ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            /*if (placementId == monney_ad)
                gemmesManager.getInstance().adGemmeFinishedSucced();
            else */if (placementId == revive_ad)
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
    }

    #endregion


    #region FONCTIONS de IUnityAdsListener non utilisées
    public void OnUnityAdsReady(string placementId) { }
    public void OnUnityAdsDidError(string message) { }
    public void OnUnityAdsDidStart(string placementId) { }



    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Initialisation Ad null");
        //throw new System.NotImplementedException();

        // on essaie de se reconnecter
        StartCoroutine(retryInitializeAd());
    }

    private IEnumerator retryInitializeAd()
    {
        yield return new WaitForSeconds(3f);

        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForSeconds(2f);
        }
        auth_to_UnityAd();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        //OnUnityAdsDidFinish(placementId, ShowResult.Failed);
        //throw new System.NotImplementedException();
        StartCoroutine(adShouldBeReseted(placementId, timeWaitReloadAd));
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //throw new System.NotImplementedException();
        OnUnityAdsDidFinish(placementId, ShowResult.Failed);
        StartCoroutine(adShouldBeReseted(placementId, 1));
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    #endregion

    #endregion


}