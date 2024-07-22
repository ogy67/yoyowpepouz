using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
//using System.Linq;
//using UnityEditor.Search;

public class developerBddManager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        //Firebase.FirebaseApp.Create();
        initUI();
        loadSaveParams();
        connectToBdd();
    }

    /*public bool test_btnListValueTranches = false;
    public bool test_btnDeleteAllPlayersFromBdd = false;
    // Update is called once per frame
    void Update()
    {
        if (test_btnListValueTranches)
        {
            test_btnListValueTranches = false;
            listValuesTranche();
        }
        if (test_btnDeleteAllPlayersFromBdd)
        {
            test_btnDeleteAllPlayersFromBdd = false;
            testDeleteAllDataPlayersFromBdd();
        }
    }*/


    public Image voyant_connectionInternet;
    public Image voyant_connectionFirestore;
    public Image voyant_maintenance;
    public Image voyant_phase1, voyant_phase2, voyant_phase3, voyant_phase4;
    public Color color_voyantOn, color_voyantOff, color_voyantWait;

    private void initUI()
    {
        //debugMessage("Init UI", false);
        voyant_connectionInternet.color = color_voyantOff;
        voyant_connectionFirestore.color = color_voyantOff;
        voyant_maintenance.color = color_voyantOff;

        voyant_phase1.color = color_voyantOff;
        voyant_phase2.color = color_voyantOff;
        voyant_phase3.color = color_voyantOff;
        voyant_phase4.color = color_voyantOff;

        openWaitInitValues();
        nbDataLoaded = 0;
    }


    // Gestion date
    public InputField dateInputField;  

    #region GESTION SCROLL TRANCHE
    public GameObject itemTranche;
    public Transform contentScrollTranche;
    private List<itemTranche> listeItemTranche = new List<itemTranche>();
    public void btnAddNewTranche()
    {
        GameObject o = Instantiate(itemTranche, contentScrollTranche);
        itemTranche i = o.GetComponent<itemTranche>();
        o.SetActive(true);
        i.itemInstantiated(-1);
        listeItemTranche.Add(o.GetComponent<itemTranche>());
    }

    private void addNewTrancheFromSave(int valueTranche)
    {
        GameObject o = Instantiate(itemTranche, contentScrollTranche);
        itemTranche i = o.GetComponent<itemTranche>();
        o.SetActive(true);
        i.itemInstantiated(valueTranche);
        listeItemTranche.Add(o.GetComponent<itemTranche>());
    }
    
    public void itemTrancheDeleted(itemTranche i)
    {
        listeItemTranche.Remove(i);
        Destroy(i.gameObject);
    }

    private string listValuesTranche()
    {
        string s = "";
        foreach(itemTranche i in listeItemTranche)
        {
            if (s != "")
                s += "|";
            s += i.getValueTranche();       
        }
        Debug.Log("Tranche : " + s);
        return s;
    }

    private List<int> listValuesTrancheInt()
    {
        List<int> retour = new List<int>();
        foreach (itemTranche i in listeItemTranche)
        {
            retour.Add(i.getValueTranche());
        }
        return retour;
    }

    private bool trancheUsed = true;
    public GameObject imgUseTranche, imgNotUseTranche;
    public void btnUseTranche()
    {
        trancheUsed = !trancheUsed;
        imgUseTranche.SetActive(trancheUsed);
        imgNotUseTranche.SetActive(!trancheUsed);
    }
    #endregion


    public Animation animBtnSave;
    #region GESTION SAVE
    public void btnSaveParamsInLocal()
    {
        // on sauvegarde :
        // - liste tranche
        string list = listValuesTranche();
        // - use tranche
        bool useTranche = trancheUsed;
        // - string date
        string date = dateInputField.text;

        List<object> dataSaved = new List<object>() { list, useTranche, date };
        SaveSystem.Save(typeSave.FIRESTORE_DEV, dataSaved);

        debugMessage("Paramètres de maintenance sauvegardés ", false);
        animBtnSave.Play("btnSaveMaintenanceApp");
    }

    private void loadSaveParams()
    {
        List<object> dataSaved = (List<object>)SaveSystem.loadData(typeSave.FIRESTORE_DEV);
        if (dataSaved.Count != 0)
        {
            string list = (string)dataSaved[0];
            bool useTranche = (bool)dataSaved[1];
            string date = (string)dataSaved[2];

            // on remplit liste tranche
            if (list != "")
            {
                string[] split = list.Split('|');
                foreach (string s in split)
                {
                    if (s != "")
                    {
                        addNewTrancheFromSave(int.Parse(s));
                    }
                }
            }
            // on met voyant utiliser tranche à jour
            trancheUsed = useTranche;
            imgUseTranche.SetActive(trancheUsed);
            imgNotUseTranche.SetActive(!trancheUsed);
            // on met la prochaine date à jour
            dateInputField.text = date;
        }
    }
    #endregion

    #region DEBUGGER
    public GameObject menuDebugger;
    public GameObject itemDebug;
    public Transform contentItemDebug;
    public GameObject notifError, notifNormal;
    public void btnDebugger()
    {
        menuDebugger.gameObject.SetActive(true);
        notifError.SetActive(false);
        notifNormal.SetActive(false);
    }
    public void btnCloseDebugger()
    {
        menuDebugger.gameObject.SetActive(false);
    }

    public void debugMessage(string message, bool errorMessage)
    {
        GameObject o = Instantiate(itemDebug, contentItemDebug);
        //o.transform.SetParent(contentItemDebug);
        if (errorMessage)
        {
            message = "<color=red>Error: </color>" + message;
            notifError.SetActive(true);
        }
        else
        {
            notifNormal.SetActive(true);
        }
        o.transform.GetChild(0).GetComponent<Text>().text = message;
        o.SetActive(true);
    }
    #endregion

    // pan attente load
    #region GESTION message attente
    private string waitInitConnecStr = "Attente de connexion à la base ...";
    private string waitMaintenanceStr = "Attente de maintenance ...";
    private string initMaintenanceStr = "Initialisation de maintenance ...";
    private string maintenanceOnStr = "Maintenance en cours ...";
    private string endMaintenanceStr = "Maintenance finit";

    public Text waitText;
    private void openWaitInitValues()
    {
        waitText.text = waitInitConnecStr;
    }

    private void wait_InitValuesFinished()
    {
        waitText.text = waitMaintenanceStr;
    }

    private void wait_MaintenanceStart()
    {
        waitText.text = initMaintenanceStr;
    }

    private void wait_MaintenanceEnd()
    {
        waitText.text = maintenanceOnStr;
    }

    private void wait_MaintenanceHasEnded()
    {
        waitText.text = endMaintenanceStr;
    }
    #endregion






    public DependencyStatus depedencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    private FirebaseFirestore fsDB;
    private CollectionReference users;
    private CollectionReference users_prevWeek;
    private CollectionReference bddInfos;

    #region Variable champs Bdd
    private const string _collectionUsers = "users";
    private const string _fieldUserId = "idPlayer";
    private const string _fieldUserName = "namePlayer";
    private const string _fieldUserNbStar = "nbStar";

    private const string _fieldValueScoreWeek = "scoreWeek";
    private const string _fieldValueScoreRecord = "scoreRecord";

    private const string _fieldValueScoreInfos = "scoreInfos";

    private const string _collectionUsers_prevWeek = "users_prevWeek";
    private const string _fieldValueScorePrevWeek = "scorePrevWeek";
    private const string _fieldIdBoostUsedPrevWeek = "idBoostUsedPrevWeek";
    private const string _fieldLevelBoostUsedPrevWeek = "levelBoostUsedPrevWeek";
    private const string _fieldTimeScorePrevWeek = "timeScorePrevWeek";


    private const string _collectionBddInfos = "bddInfos";
    private const string _docDateWeekTournament = "dateWeekTournament";
    private const string _fieldNextTimeSecTournament = "nextTimeSecond";
    private const string _docModeMaintenance = "modeMaintenance";
    private const string _fieldModeMaintenance = "maintenanceOn";
    private const string _fieldIdInfos = "idInfo";
    #endregion

    // ---------------------------------------- gestion connection à la base

    #region GESTION init connection à la base

    int testConnecInternetCount = 0;
    // 1 connection internet
    private void connectToBdd()
    {
        if(testConnecInternetCount == 0)
        {
            debugMessage("Va essayer de se connecter à internet", false);
            voyant_connectionInternet.color = color_voyantWait;
        }
        testConnecInternetCount++;

        //StartCoroutine(waitConnectionInternet());
        StartCoroutine(checkInternetConnection((isConnected) => {
            if (isConnected)
            {
                voyant_connectionInternet.color = color_voyantOn;
                debugMessage("Connexion internet trouvée", false);
                testConnectToFirestore();
            }
            else
            {
                debugMessage("Pas de connexion à internet", true);
                voyant_connectionInternet.color = color_voyantOff;
            }
        }));
    }

    /*private IEnumerator waitConnectionInternet()
    {
        yield return new WaitForSeconds(0.5f);
        while (Application.internetReachability == NetworkReachability.NotReachable)
            yield return new WaitForSeconds(0.5f);
        testConnectToFirestore();
        voyant_connectionInternet.color = color_voyantOn;
    }*/

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }




    // 2 connection à firestore
    public void testConnectToFirestore()
    {
        //fbConnec = GetComponent<firebaseConnection>();
        //debugMessage("Essaie de se connecter à Firestore", false);
        isWaitingConnectFirestore = true;
        voyant_connectionFirestore.color = color_voyantWait;
        StartCoroutine(waitConnectToFirestore());
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            depedencyStatus = task.Result;
            isWaitingConnectFirestore = false;
            if (depedencyStatus == DependencyStatus.Available)
            {
                resultConnectFirestore = 1;               
            }
            else
            {
                resultConnectFirestore = -1;
                //Debug.LogError("Could not resolve all firebase depedencies " + depedencyStatus);
            }
        });
    }

    private bool isWaitingConnectFirestore = false;
    private int resultConnectFirestore = 0; // -1 pas reussit, 1 reussit
    private IEnumerator waitConnectToFirestore()
    {
        while (isWaitingConnectFirestore)
        {
            yield return null;
        }
        if (resultConnectFirestore == 1)
        {
            debugMessage("Connexion à Firestore réussit", false);
            setUpFirebase();
        }
        else if (resultConnectFirestore == -1)
            debugMessage("N'a pas pu se connecter à Firestore malgré la connection à internet", true);
    }

    private void setUpFirebase()
    {
        debugMessage("... Attend authentification ...", false);
        fsDB = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        users = fsDB.Collection(_collectionUsers);
        users_prevWeek = fsDB.Collection(_collectionUsers_prevWeek);
        lesBots = fsDB.Collection(_collectionBddBot);
        bddInfos = fsDB.Collection(_collectionBddInfos);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        // On s'en fout que user soit identifié ou non dans firestore
        // on passe cash à la suite ( on attend juste d'avoir une reponse de bdd meme si pas identifie )

        // on arrive dans cette fonction parce que bdd envoit un signal
        voyant_connectionFirestore.color = color_voyantOn;
        initConnectionSucced();
        debugMessage("Authentification réussit", false);
        // dans jeu normal, besoin de verifier si est log, ici fait requête sans être log


        /*if (auth.CurrentUser != null)
        {
            Debug.Log("<color=red>Init log succed, player already exists</color>");
            User = auth.CurrentUser;
            voyant_connectionFirestore.color = color_voyantOn;
            initConnectionSucced();
        }
        else
        {
            debugMessage("N'a pas réussit à reccuperer le compte (mode anonym)", true);
            //Debug.Log("<color=red>NO USER FOUND</color>");
        }*/
    }

    #endregion

    private void initConnectionSucced()
    {
        loadTimeNextWeek();
        loadMaintenanceState();
    }

    public Text nextWeekTimeText;
    public Text compteurTimeText;
    private double timeNextWeek;
    private bool maintenanceOn;

    #region GESTION init retrieve data

    // reccupère next week time
    // -> met à jour next date && lance timer
    private void loadTimeNextWeek()
    {
        Query query_timeNextWeek = bddInfos.WhereEqualTo(_fieldIdInfos, _docDateWeekTournament);

        query_timeNextWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                debugMessage("N'a pas reussit à réccupérer la date end week dans bdd", true);
            }
            else
            {
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    Dictionary<string, object> details = documentSnapshot.ToDictionary();
                    timeNextWeek = Convert.ToDouble(details[_fieldNextTimeSecTournament]);
                    nextWeekTimeText.text = Math.Truncate(timeNextWeek).ToString();
                    StartCoroutine(compteurTimeEnum());
                    aDataIsLoaded_initMenu();
                    break;
                }
            }
        });
    }

    // reccupère état maintenance
    // -> met à jour voyant maintenance
    public void loadMaintenanceState()
    {
        // fait requête pour mettre à jour maintenance
        // fin de requête, rafraichir mode maintenance de maintenanceMenu
        Query query_updateMaintenance = bddInfos.WhereEqualTo(_fieldIdInfos, _docModeMaintenance);

        query_updateMaintenance.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                Debug.Log("N'a pas reussit à réccupérer l'état de maintenance dans bdd");
            }
            else
            {
                //Debug.Log("Maintenance state refresh, init menu check ? : " + initMenuCheck);
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    //Debug.Log("1");
                    Dictionary<string, object> details = documentSnapshot.ToDictionary();
                    maintenanceOn = Convert.ToBoolean(details[_fieldModeMaintenance]);
                    voyant_maintenance.color = maintenanceOn ? color_voyantOn : color_voyantOff;
                    aDataIsLoaded_initMenu();
                }
            }
        });
    }
    #endregion

    // lancer après reccup next date time de bdd
    #region GESTION timer avant maintenance
    private IEnumerator compteurTimeEnum()
    {
        double currentTimeBdd = getTimeFirestoreBdd();
        bool reachedTimeStartMaintenance = false;
        while (timeNextWeek - currentTimeBdd > 0)
        {
            if(timeNextWeek - currentTimeBdd < timeEnableMaintenance
                && !reachedTimeStartMaintenance)
            {
                reachedTimeStartMaintenance = true;
                timerReachTimeEnableMaintenance();
            }
            compteurTimeText.text = roundDecimal(timeNextWeek - currentTimeBdd, 1).ToString();
            yield return null;
            currentTimeBdd = getTimeFirestoreBdd();
        }
        compteurTimeText.text = "0";
    }

    public double getTimeFirestoreBdd()
    {
        //Firebase.Firestore.Timestamp.
        Timestamp timeBdd = Timestamp.GetCurrentTimestamp();
        DateTime timeBdd_toDate = timeBdd.ToDateTime();
        long elapsedTicks = timeBdd_toDate.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        double seconds = elapsedSpan.TotalSeconds;
        return seconds;
    }

    private double roundDecimal(double number, int nbDecimal)
    {
        int n = (int)(number * Mathf.Pow(10, nbDecimal));
        double n2 = (double)n / Mathf.Pow(10, nbDecimal);
        return n2;
    }

    #endregion

    private int nbDataLoaded;
    private const int nbDataToBeFullyLoaded = 2;
    private void aDataIsLoaded_initMenu()
    {
        nbDataLoaded++;
        if (nbDataLoaded == nbDataToBeFullyLoaded)
        {
            wait_InitValuesFinished();
        }
    }

    // fin ------------------------------------------- gestion connection à la base

        // appelé par btn update currentDate
        // appelé par routine maintenance
    public void updateCurrentDateBdd(bool callInMaintenance)
    {
        double currentTimeBdd = getTimeFirestoreBdd();
        DateTime currentDate = DateTime.Now;
        DateTime nextDate = DateTime.Parse(dateInputField.text);
        double nbSecDiff = (nextDate - currentDate).TotalSeconds;

        Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldNextTimeSecTournament, currentTimeBdd + nbSecDiff } };
        bddInfos.Document(_docDateWeekTournament).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                debugMessage("N'a pas reussit à modifier la date actuelle de la base", true);
            }
            else
            {
                debugMessage("A modifié la date end week dans bdd", false);
                double prevTime = timeNextWeek;
                timeNextWeek = currentTimeBdd + nbSecDiff;
                nextWeekTimeText.text = Math.Truncate(timeNextWeek).ToString();
                if (callInMaintenance)
                    end_etape4();
                if (compteurTimeText.text == "0")
                    StartCoroutine(compteurTimeEnum());
                currentTimeText_maintenance.text = prevTime.ToString();
                nextTimeText_maintenance.text = timeNextWeek.ToString();
            }
        });
    }

    public menuChangeMaintenance menuForceChangeMaintenanceState;
    public GameObject menuForceChangeMaintenanceGo;
    public void btnOpenForceMaintenance()
    {
        menuForceChangeMaintenanceGo.SetActive(true);
    }

    public void btnCloseForceMaintenance()
    {
        menuForceChangeMaintenanceGo.SetActive(false);
    }

    
    // appelé depuis menu force switch
    public IEnumerator forceSwitchMaintenanceState()
    {
        queryOn = true;
        // 1 : on met bdd en mode maintenance
        Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldModeMaintenance, !maintenanceOn} };
        bddInfos.Document(_docModeMaintenance).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                querryError = true;
                queryOn = false;
                errorWhileMaintenance();
                //debugMessage("N'a pas reussit à forcer le switch de maintenance", true);
            }
            else
            {
                queryOn = false;
                //debugMessage("A reussit à forcer le switch de maintenance", false);
                maintenanceOn = !maintenanceOn;
                
            }
        });
        while (queryOn)
            yield return null;
        debugMessage(querryError ? "N'a pas reussit à forcer le switch de maintenance"
            : "A reussit à forcer le switch de maintenance"
            , querryError);
        menuForceChangeMaintenanceState.endChangeState();
        voyant_maintenance.color = maintenanceOn ? color_voyantOn : color_voyantOff;


    }



    private static bool queryOn = false;
    private static bool querryError = false;


    // ------------------------ phase maintenance
    #region phase maintenance

    private void errorWhileMaintenance()
    {
        // mettre tt les waitImg sur off
    }

    public void forceReloadEtape(int numEtape)
    {
        switch (numEtape)
        {
            case 1:
                // vider le ranking prevWeek
                StartCoroutine(resetPrevWeekRankRankEnum());
                break;
            case 2:
                // transferer rank week à rank prevWeek
                StartCoroutine(transferWeekToPrevWeek());
                break;
            case 3:
                // on reset tout les scores week des joueurs dans bdd
                fsResetAllPlayersScoreWeek();
                break;
            case 4:
                // set la nouvelle date fin week de bdd
                updateCurrentDateBdd(maintenanceAuto ? true : false);
                break;
        }
    }

    private bool maintenanceAuto = false;

    public GameObject waitImg_phase1, waitImg_phase2, waitImg_phase3, waitImg_phase4;
    public GameObject panContinue_phase2, panContinue_phase3, panContinue_phase4, panContinue_phaseEnd;
    public Text nbPlayerRankedText, nbPlayerResetedText, currentTimeText_maintenance, nextTimeText_maintenance;
    public Text nbPlayersDeletedPrevWeekText;

    public GameObject menuStartMaintenance;
    private double timeEnableMaintenance = 5f;
    private void timerReachTimeEnableMaintenance()
    {
        waitImg_phase1.SetActive(true);
        waitImg_phase2.SetActive(true);
        waitImg_phase3.SetActive(true);
        waitImg_phase4.SetActive(true);
        StartCoroutine(waitTimeLeft());
    }

    private IEnumerator waitTimeLeft()
    {
        wait_MaintenanceStart();
        bool wait_query = true;
        bool error_query = false;
        // 1 : on met bdd en mode maintenance
        Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldModeMaintenance, true} };
        bddInfos.Document(_docModeMaintenance).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                errorWhileMaintenance();
                //debugMessage("N'a pas reussit à activer maintenance dans bdd", true);
                wait_query = false;
                error_query = true;
            }
            else
            {
                wait_query = false;               
            }
        });
        while (wait_query)
            yield return null;
        debugMessage(error_query ? "N'a pas reussit à activer maintenance dans bdd":
            "A reussit à activer maintenance dans bdd", error_query);
        if (error_query)
            yield break;
        else
            voyant_maintenance.color = color_voyantOn;

        // attend timer atteind 0s
        // on attend que timer bdd atteind 0
        double currentTimeBdd = getTimeFirestoreBdd();
        while (timeNextWeek - currentTimeBdd > 0)
        {
            compteurTimeText.text = roundDecimal(timeNextWeek - currentTimeBdd, 1).ToString();
            yield return null;
            currentTimeBdd = getTimeFirestoreBdd();
        }
        wait_MaintenanceEnd();
        // à la fin attente
        menuStartMaintenance.SetActive(true);
        maintenanceAuto = true;
    }

    public void btnStartMaintenance()
    {
        menuStartMaintenance.SetActive(false);
        // vider le ranking prevWeek
        StartCoroutine(resetPrevWeekRankRankEnum());
    }

    // une fois le ranking prevWeek vidé
    private void end_etape1()
    {
        waitImg_phase1.SetActive(false);
        // ouvrir btn next step
        panContinue_phase2.gameObject.SetActive(true);
        voyant_phase1.color = color_voyantOn;
    }

    // on attend click sur btn next step
    public void maintenance_etape2()
    {
        panContinue_phase2.gameObject.SetActive(false);
        // transferer rank week à rank prevWeek
        StartCoroutine(transferWeekToPrevWeek());
    }

    // une fois le ranking prevWeek crée
    private void end_etape2()
    {
        waitImg_phase2.SetActive(false);
        // ouvrir btn next step
        panContinue_phase3.gameObject.SetActive(true);
        voyant_phase2.color = color_voyantOn;
    }

    // on attend click sur btn next step
    public void maintenance_etape3()
    {
        panContinue_phase3.gameObject.SetActive(false);
        // on reset tout les scores week des joueurs dans bdd
        fsResetAllPlayersScoreWeek();
    }

    // une fois les scores week reset
    private void end_etape3()
    {       
        waitImg_phase3.SetActive(false);
        // ouvrir btn next step    
        panContinue_phase4.gameObject.SetActive(true);
        voyant_phase3.color = color_voyantOn;
    }

    // on attend click sur btn next step
    public void maintenance_etape4()
    {
        panContinue_phase4.gameObject.SetActive(false);
        // set la nouvelle date fin week de bdd
        updateCurrentDateBdd(true);
    }

    // une fois les scores week reset
    private void end_etape4()
    {
        waitImg_phase4.SetActive(false);
       
        // ouvrir btn end maintenance       
        panContinue_phaseEnd.gameObject.SetActive(true);
        voyant_phase4.color = color_voyantOn;
    }

    public void btnEndMaintenance()
    {
        maintenanceAuto = false;
        panContinue_phaseEnd.gameObject.SetActive(false);
        StartCoroutine(disableMaintenanceStateLate());
    }

    private IEnumerator disableMaintenanceStateLate()
    {
        queryOn = true;

        // mettre maintenance en off
        Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldModeMaintenance, false} };
        bddInfos.Document(_docModeMaintenance).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                querryError = true;
                queryOn = false;
                errorWhileMaintenance();
                debugMessage("N'a pas reussit à desactiver maintenance dans bdd", true);
            }
            else
            {
                queryOn = false;
                voyant_maintenance.color = color_voyantOff;
                wait_MaintenanceHasEnded();
            }
        });
        while (queryOn)
            yield return null;
        debugMessage(querryError ? "N'a pas reussit à desactiver maintenance dans bdd"
            : "A reussit à desactiver maintenance dans bdd", querryError);
        if (!querryError)
            voyant_maintenance.color = color_voyantOff;
        querryError = false;
    }
    #endregion



    #region Transfer week to prevWeek

    //public int offSetPlayers = 10; // en reccupere x de plus, trie tout par score -> time, puis enlève x player
    public int nbPlayerLimitWeek = 100;//, nbPlayerLimitRecord = 200;
    public int offSetPlayers_prevWeek = 15; // en reccupere x de plus, trie tout par score -> time, puis enlève x player
    private IEnumerator resetPrevWeekRankRankEnum()
    {
        WriteBatch batch = fsDB.StartBatch();
        Query query_rankPrevWeek = users_prevWeek;
        queryOn = true;
        int nbPlayersDeleted = 0;
        query_rankPrevWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                //debugMessage("N'a pas reussit reset prevRank", true);
                errorWhileMaintenance();
                querryError = true;
                queryOn = false;
            }
            else
            {
                //int nbBatch = 0;
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    DocumentReference dR = documentSnapshot.Reference;
                    batch.Delete(dR);
                    //nbBatch++;
                    nbPlayersDeleted++;
                }

                if (nbPlayersDeleted > 0)
                {
                    batch.CommitAsync().ContinueWith(task =>
                    {
                        queryOn = false;
                        if (task.IsFaulted)
                            querryError = true;

                    });
                }
                else
                {
                    queryOn = false;
                }
            }
        });
        while (queryOn)
            yield return null;
        debugMessage(querryError ? 
            "N'a pas reussit reset prevRank" : "A reussit reset prevRank", querryError);
        nbPlayersDeletedPrevWeekText.text = nbPlayersDeleted.ToString();
        querryError = false;
        if(!querryError && maintenanceAuto)
            end_etape1();
    }
    private IEnumerator transferWeekToPrevWeek()
    {
        Query query_rankWeek = users.OrderByDescending(_fieldValueScoreWeek)
            .WhereGreaterThan(_fieldValueScoreWeek, 0)
            .Limit(nbPlayerLimitWeek + offSetPlayers_prevWeek);
        queryOn = true;
        string messageDebug = "";
        List<infoPlayerPrevWeek> listPlayer = new List<infoPlayerPrevWeek>();

        query_rankWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                queryOn = false;
                querryError = true;
                messageDebug = "N'a pas reussit transfer week to prevWeek (getListFromBdd)";
                errorWhileMaintenance();
            }
            else
            {
                Dictionary<string, object> details;
                fsUsers_prevWeek user;
                DocumentReference docRef;
                
                // 1 , on reccupère la liste et on la trie avec timeScore
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    details = documentSnapshot.ToDictionary();
                    user = fsUser_tofsUserPrevWeek(
                        dataUserBdd_tofsUsers(details));
                    listPlayer.Add(fsUserPrevWeek_toInfoPlayerPrevWeek(user));
                }

                if (listPlayer.Count > 0)
                {
                    listPlayer = reOrderListPlayersPrevWeek_byTimeScore(listPlayer);
                    // on enlève le surplus de player
                    while (listPlayer.Count > nbPlayerLimitWeek)
                        listPlayer.RemoveAt(listPlayer.Count - 1);


                    int nbPlayerCreatedQueue = 0;
                    // 2 , on créer un item dans liste bdd pour chaque player
                    foreach (infoPlayerPrevWeek info in listPlayer)
                    {
                        nbPlayerCreatedQueue++;
                        user = infoPrevWeek_toFsUserPrevWeek(info);

                        docRef = users_prevWeek.Document(user.userId);
                        docRef.SetAsync(user.toDictionnary(), SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsFaulted)
                            {
                                queryOn = false;
                                querryError = true;
                                messageDebug = "N'a pas reussit transfer week to prevWeek (createItemPlayer)";
                                errorWhileMaintenance();
                            }
                            else
                            {
                                //Debug.Log("<color=green>User registered</color>");
                                nbPlayerCreatedQueue--;
                                if (nbPlayerCreatedQueue == 0)
                                {
                                    queryOn = false;
                                    messageDebug = "A reussit transfer week to prevWeek";
                                }
                            }
                        });
                    }
                }
                else
                {
                    queryOn = false;
                    messageDebug = "A reussit transfer week to prevWeek";
                }
            }
        });
        while (queryOn)
            yield return null;
        debugMessage(messageDebug, querryError);
        if (!querryError && maintenanceAuto)
            end_etape2();
        nbPlayerRankedText.text = listPlayer.Count.ToString();

        querryError = false;
    }


    private List<infoPlayerPrevWeek> reOrderListPlayersPrevWeek_byTimeScore(List<infoPlayerPrevWeek> list)
    {
        // les joueurs sont déjà classés par scores, on les reclasse par timeScore
        List<infoPlayerPrevWeek> retour = new List<infoPlayerPrevWeek>();
        List<infoPlayerPrevWeek> sameScore;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
                retour.Add(list[i]);
            else
            {
                sameScore = new List<infoPlayerPrevWeek>() { list[i] };
                // -1 parce que check index suivant
                while (i < list.Count - 1 && list[i].scorePrevWeek == list[i + 1].scorePrevWeek)
                {
                    sameScore.Add(list[i + 1]);
                    i++;
                }

                if (sameScore.Count == 1)
                    retour.Add(list[i]);
                else
                {
                    // on classe les joueurs ayant le même Score
                    while (sameScore.Count > 0)
                    {
                        int index = 0;
                        double minTime = sameScore[index].timeScore;
                        for (int e = 1; e < sameScore.Count; e++)
                        {
                            if (sameScore[e].timeScore < minTime)
                            {
                                minTime = sameScore[e].timeScore;
                                index = e;
                            }
                        }
                        retour.Add(sameScore[index]);
                        sameScore.RemoveAt(index);
                    }
                }
            }
        }
        return retour;
    }

    private infoPlayerPrevWeek fsUserPrevWeek_toInfoPlayerPrevWeek(fsUsers_prevWeek user)
    {
        Debug.Log("fsUserPrevWeek_toInfoPlayerPrevWeek()");
        infoPlayerPrevWeek retour = new infoPlayerPrevWeek()
        {
            nomPlayer = user.name,
            idPlayer = user.userId,
            nbStar = user.nbStar,

            scorePrevWeek = user.scorePrevWeek,
            idsBoostUsed = user.idBoostUsedPrevWeek,
            levelBoostUsed = user.levelBoostUsedPrevWeek,
            timeScore = user.timeScore,

            rewardTaken = user.rewardTaken,
        };
        return retour;
    }

    private fsUsers_prevWeek fsUser_tofsUserPrevWeek(fsUsers user)
    {
        string[] infosScore = infoScore_toList(user.infoScore);
        return new fsUsers_prevWeek()
        {
            name = user.name,
            userId = user.userId,
            nbStar = user.nbStar,

            scorePrevWeek = user.scoreWeek,
            idBoostUsedPrevWeek = infosScore[0],
            levelBoostUsedPrevWeek = infosScore[1],
            timeScore = Convert.ToDouble(infosScore[2]),

            rewardTaken = false,
        };
    }

    private fsUsers_prevWeek infoPrevWeek_toFsUserPrevWeek(infoPlayerPrevWeek info)
    {
        return new fsUsers_prevWeek()
        {
            name = info.nomPlayer,
            userId = info.idPlayer,
            nbStar = info.nbStar,

            scorePrevWeek = info.scorePrevWeek,
            timeScore = info.timeScore,
            idBoostUsedPrevWeek = info.idsBoostUsed,
            levelBoostUsedPrevWeek = info.levelBoostUsed,
        };
    }
    #endregion

    public float timeWaitBatchEnd = 0.2f;
    private const int nbOperationBatch = 500;
    /*
     * void fsResetAllPlayersScoreWeek()
     * IEnumerator resetNormalEnum()
     * IEnumerator resetByTrancheEnum()
     * void failedResetingPrevWeekScore()
    */
    #region RESET all scoreWeek 
    public void fsResetAllPlayersScoreWeek()
    {
        nbPlayerReset = 0;

        if (trancheUsed && listeItemTranche.Count == 0)
        {
            debugMessage("\"Tranche Batch\" était activé MAIS liste vide -> fait un reset sans batch", false);
            trancheUsed = false;
        }

        if (!trancheUsed)
        {
            StartCoroutine(resetNormalEnum());
        }
        else
        {
            StartCoroutine(resetByTrancheEnum());
        }
    }

    private IEnumerator resetNormalEnum()
    {
        StartCoroutine(batchResetPlayers(0));
        while (waitingRetBatch)
            yield return null;
        //yield return new WaitForSeconds(timeWaitBatchEnd);
        if (!errorWhileBatch)
        {
            nbPlayerResetedText.text = nbPlayerReset.ToString();
            if (maintenanceAuto)
                end_etape3();
            debugMessage("A reussit reset score week de tout les player de bdd ("+nbPlayerReset+" joueurs)", false);
        }
    }

    // new rank prev week created
    private IEnumerator resetByTrancheEnum()
    {
        List<int> trancheScore = listValuesTrancheInt();
        foreach (int tranche in trancheScore)
        {
            StartCoroutine(batchResetPlayers(tranche));
            while (waitingRetBatch)
                yield return null;
            if (errorWhileBatch)
                break;
            else
                nbPlayerResetedText.text = nbPlayerReset.ToString();
           
            //yield return new WaitForSeconds(timeWaitBatchEnd);
        }
        if (!errorWhileBatch)
        {
            if (maintenanceAuto)
                end_etape3();
            nbPlayerResetedText.text = nbPlayerReset.ToString();
            debugMessage("A reussit reset score week de tout les player de bdd", false);
        }
    }


    private bool waitingRetBatch = false;
    private bool errorWhileBatch = false;
    private int nbPlayerReset;
    private int queueBatchWaiting;

    private IEnumerator batchResetPlayers(int _trancheScore)
    {
        int initNbPlayer = nbPlayerReset;
        waitingRetBatch = true;

        CollectionReference collec = users;
        Dictionary<string, object> data = new Dictionary<string, object>() { { _fieldValueScoreWeek, 0 } };
        int currentNbBatch = 0;

        List<WriteBatch> listBatch = new List<WriteBatch>();
        WriteBatch currentBatch = fsDB.StartBatch();
        bool allBatchCreated = false;

        Query q = users.WhereGreaterThan(_fieldValueScoreWeek, _trancheScore);

        //Debug.Log("Try reset all scoreWeek");
        q.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                debugMessage("N'a pas reussit a reccup data players bdd (reset all scores)", true);
                errorWhileMaintenance();
                //Debug.Log("Failed getting players from base");
                //failedResetingPrevWeekScore();
                errorWhileBatch = true;
                waitingRetBatch = false;
            }
            else
            {
                //Debug.Log("Getting players from base");
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    currentBatch.Update(documentSnapshot.Reference, data);
                    currentNbBatch++;
                    nbPlayerReset++;

                    if (currentNbBatch == nbOperationBatch)
                    {
                        //Debug.Log("Add batch to list batch, create new batch");
                        currentNbBatch = 0;
                        listBatch.Add(currentBatch);
                        currentBatch = fsDB.StartBatch();
                    }
                }
                if (currentNbBatch != 0)
                {
                    //Debug.Log("Add end batch to list batch");
                    //queueBatchWaiting++;
                    listBatch.Add(currentBatch);
                }
                allBatchCreated = true;
                //Debug.Log("Tranche above : " + _trancheScore);
                //Debug.Log("Nb players modified in one request : " + (nbPlayerReset - initNbPlayer));
            }
        });

        while (!allBatchCreated && !errorWhileBatch)
            yield return null;
        if (errorWhileBatch)
            yield break;

        bool batchEnd;
        // on envoit tout les batch, un à la suite, en attendant que chacuns se termine avant de lancer le suivant
        foreach (WriteBatch batch in listBatch)
        {
            batchEnd = false;
            batch.CommitAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    errorWhileBatch = true;
                }
                else
                {
                    batchEnd = true;
                    Debug.Log("One batch fully commited");
                }
            });
            while (!batchEnd && !errorWhileBatch)
                yield return null;
            if (errorWhileBatch)
                break;
        }

        waitingRetBatch = false;
        if (errorWhileBatch)
        {
            debugMessage("Un des batch n'a pas reussit à aboutir, tranche de score superieure à : "
                + _trancheScore + " , nombre de joueurs dans tranche : "+ (nbPlayerReset - initNbPlayer), true);
            errorWhileMaintenance();
        }
        else
        {
            debugMessage("Sur tranche de score superieure à : "
                + _trancheScore + " , a reset "+ (nbPlayerReset - initNbPlayer) + " players", false);
        }

    }
    #endregion


    internal struct fsUsers
    {
        internal string userId;
        internal string name;
        internal int nbStar;

        internal int scoreWeek;
        internal int scoreRecord;
        internal string infoScore;

        internal Dictionary<string, object> toDictionnary()
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData.Add(_fieldUserId, this.userId);
            userData.Add(_fieldUserName, this.name);
            userData.Add(_fieldUserNbStar, this.nbStar);

            userData.Add(_fieldValueScoreWeek, this.scoreWeek);
            userData.Add(_fieldValueScoreRecord, this.scoreRecord);

            userData.Add(_fieldValueScoreInfos, this.infoScore);

            return userData;
        }
    }

    internal struct fsUsers_prevWeek
    {
        internal string userId;
        internal string name;
        internal int nbStar;

        internal int scorePrevWeek;
        internal double timeScore;

        internal string idBoostUsedPrevWeek;
        internal string levelBoostUsedPrevWeek;

        internal bool rewardTaken;

        internal Dictionary<string, object> toDictionnary()
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData.Add(_fieldUserId, this.userId);
            userData.Add(_fieldUserName, this.name);
            userData.Add(_fieldUserNbStar, this.nbStar);

            userData.Add(_fieldValueScorePrevWeek, this.scorePrevWeek);
            userData.Add(_fieldIdBoostUsedPrevWeek, this.idBoostUsedPrevWeek);
            userData.Add(_fieldLevelBoostUsedPrevWeek, this.levelBoostUsedPrevWeek);
            userData.Add(_fieldTimeScorePrevWeek, this.timeScore);

            //userData.Add(_fieldRewardPrevWeekTaken, this.rewardTaken);

            return userData;
        }

        internal string userToString()
        {
            return this.userId + ':' + this.name + ':' + this.nbStar + ':' + this.scorePrevWeek + ':' + this.idBoostUsedPrevWeek
                + ':' + this.levelBoostUsedPrevWeek + ':' + this.timeScore + ':' + this.rewardTaken;
        }

        internal void stringToUser(string data)
        {
            string[] s_tab = data.Split(':');
            this.userId = s_tab[0];
            this.name = s_tab[1];
            this.nbStar = int.Parse(s_tab[2]);
            this.scorePrevWeek = int.Parse(s_tab[3]);
            this.idBoostUsedPrevWeek = s_tab[4];
            this.levelBoostUsedPrevWeek = s_tab[5];
            this.timeScore = Convert.ToDouble(s_tab[6]);
            this.rewardTaken = Convert.ToBoolean(s_tab[7]);
        }
    }

    private fsUsers dataUserBdd_tofsUsers(Dictionary<string, object> details)
    {
        fsUsers retour = new fsUsers()
        {
            name = Convert.ToString(details[_fieldUserName]),
            userId = Convert.ToString(details[_fieldUserId]),
            nbStar = Convert.ToInt32(details[_fieldUserNbStar]),

            scoreWeek = Convert.ToInt32(details[_fieldValueScoreWeek]),
            scoreRecord = Convert.ToInt32(details[_fieldValueScoreRecord]),

            infoScore = Convert.ToString(details[_fieldValueScoreInfos])
        };
        return retour;
    }

    private string[] infoScore_toList(string s)
    {
        return s.Split('|');
    }



    // -------------------------------------------- BOTS
    private const string _collectionBddBot = "botInfos";
    private const string _fieldBotId = "idBot";
    public btnUpdateBot updateBot;
    private CollectionReference lesBots;

    private void botFailedUpdate()
    {
        debugMessage("Erreur pendant met à jour les bots", true);
    }
    private void botEndUpdate()
    {
        //if (error)
        //    debugMessage("Erreur pendant met à jour les bots", true);

        /*if (start)
        {
            nbBotUpdated++;
        }
        else
        {*/
            nbBotUpdated--;
            if(nbBotUpdated == 0)
            {
            debugMessage("Mise à jour des bots réussit ( sans phase suppression bot )", false);
            updateBot.endChangeState();
            }
        //}
    }

    public scrollBot _scrollBots;
    private static int nbBotUpdated;
    public IEnumerator updateBotInBdd(List<infoPlayerTest> listInfo_update, List<infoPlayerTest> listInfo_remove)
    {
        fsUsers botUser;

        nbBotUpdated = listInfo_update.Count;


        // pour chaque joueur dans listitemsBot, créer / update bot dans bdd
        foreach (infoPlayerTest info in listInfo_update)
        {
            queryOn = true;
            querryError = false;

            Debug.Log("Traite update bot id : "+info.idPlayer);

            string id = info.idPlayer;
            Query query = users.WhereEqualTo(_fieldUserId, id);
            botUser = infoUserToFsUser(info);


            query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (!querySnapshotTask.IsCompleted)
                {
                    queryOn = false;
                    querryError = true;
                }
                else
                {
                    queryOn = false;
                    int nbP = 0;
                    if (querySnapshotTask.Result.Documents != null)
                        foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                            nbP++;

                    

                    // on créer un utilisateur
                    if (nbP == 0)
                    {
                        Debug.Log("Create bot "+ info.idPlayer);
                        Dictionary<string, object> userData = botUser.toDictionnary();
                        DocumentReference docRef = users.Document(botUser.userId);
                        docRef.SetAsync(userData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsFaulted)
                            {
                                queryOn = false;
                                querryError = true;
                                Debug.Log("Create bot 1");
                            }
                            else
                            {
                                queryOn = false;
                                Debug.Log("Create bot 2");
                            }
                        });
                        // on ajoute aussi l'id du bot a la nouvelle liste bot
                        Dictionary<string, object> userData_2 = new Dictionary<string, object>
                        {
                            { _fieldBotId, info.idPlayer }
                        };
                        DocumentReference docRef_2 = lesBots.Document(botUser.userId);
                        docRef_2.SetAsync(userData_2, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsFaulted)
                            {
                                queryOn = false;
                                querryError = true;
                                Debug.Log("Create bot 1 (Add in parraleleList)");
                            }
                            else
                            {
                                queryOn = false;
                                Debug.Log("Create bot 2 (Add in parraleleList)");
                            }
                        });
                    }
                    else
                    {
                        Debug.Log("Update bot "+ info.idPlayer);
                        // on met à jour les valeures
                        users.Document(botUser.userId).UpdateAsync(botUser.toDictionnary()).ContinueWithOnMainThread((querySnapshotTask2) =>
                        {
                            if (querySnapshotTask2.IsFaulted)
                            {
                                queryOn = false;
                                querryError = true;
                                Debug.Log("Update bot 1");
                            }
                            else
                            {
                                queryOn = false;
                                Debug.Log("Update bot 2");
                            }
                        });
                    }
                }
            });

            while (queryOn)
                yield return null;
            if (querryError)
            {
                querryError = false;
                botFailedUpdate();
                yield break;
            }
            
            botEndUpdate();
        }

        // pour chaque joueur à supprimer de bdd, on le supprime
        if (listInfo_remove.Count > 0)
        {
            WriteBatch batch;
            batch = fsDB.StartBatch();
            Query quer, quer2;

            queryOn = true;
            querryError = false;
            int i = 0;
            int max_i = listInfo_remove.Count - 1;

            foreach (infoPlayerTest info in listInfo_remove)
            {
                Debug.Log("Delete bot id : " + info.idPlayer);
                int my_i = i;
                quer = users.WhereEqualTo(_fieldUserId, info.idPlayer);
                quer.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
                {
                    if (querySnapshotTask.IsFaulted)
                    {
                        querryError = true;
                        if (my_i == max_i)
                            queryOn = false;
                        Debug.Log("BUG : delete bots failed");
                    }
                    else
                    {
                        foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                        {
                            DocumentReference dR = documentSnapshot.Reference;
                            batch.Delete(dR);
                            // aussi supprimer de 2e liste
                            DocumentReference dR_2 = lesBots.Document(info.idPlayer);
                            batch.Delete(dR_2);
                            // un seul doc normalement
                            break;
                        }
                        if (my_i == max_i)
                            queryOn = false;
                    }
                });
                i++;
            }
            while (queryOn)
                yield return null;
            if (querryError)
            {
                botFailedUpdate();
                querryError = false;
                yield break;
            }

            batch.CommitAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Batch delete players failed");
                }
                else
                {
                    Debug.Log("Batch delete players succed");
                }
            });
        }
        // remet à zero la wait bar si un échapé
        // n'est pas désactive si fait juste une supression de bot
        yield return null;
        updateBot.endChangeState();

        debugMessage("Routine update bot bdd terminée", false);
        _scrollBots.btnSaveListBot(true);
    }


    public Animation anim_btnLoadBddData;
    public GameObject waitGo_btnLoadBddData;
    // met à jour la liste des bots enregistré en local, en prenant data dans bdd
    public void btnRefreshLocalList()
    {
        anim_btnLoadBddData.Play("btnReloadBotBdd");
        StartCoroutine(loadBotBdd());
    }

    private IEnumerator loadBotBdd()
    {
        yield return new WaitForSeconds(20 / 60f);
        List<string> lesIdsList = new List<string>();
        Query allIdBotQuery = fsDB.Collection(_collectionBddBot);

        queryOn = true;
        querryError = false;
        allIdBotQuery.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (!querySnapshotTask.IsCompleted)
            {
                queryOn = false;
                querryError = true;
            }
            else
            {
                queryOn = false;
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    lesIdsList.Add(documentSnapshot.Id);
                }                   
            }
        });

        while (queryOn)
            yield return null;

        if (querryError)
        {
            querryError = false;
            debugMessage("ERROR : Retirer liste des ids des bots de la base", true);
            waitGo_btnLoadBddData.SetActive(false);
            yield break;
        }

        //Debug.Log("A retrieve "+lesIdsList.Count+" id de bots");


        List<infoPlayerTest> infoBotsList = new List<infoPlayerTest>();
        // Ensuite, pour chaque id retiré
        // On cherche ses infos dans 'users'
        foreach (string id in lesIdsList)
        {
            queryOn = true;
            querryError = false;

            Query query = users.WhereEqualTo(_fieldUserId, id);
            query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (!querySnapshotTask.IsCompleted)
                {
                    queryOn = false;
                    querryError = true;
                }
                else
                {
                    queryOn = false;
                    if (querySnapshotTask.Result.Documents != null)
                        foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                        {
                            infoBotsList.Add(
                                fsUser_toInfoPlayer(
                                dataUserBdd_tofsUsers(
                                    documentSnapshot.ToDictionary())));
                            break;
                        }
                }
            });

            while (queryOn)
                yield return null;

            if (querryError)
            {
                querryError = false;
                debugMessage("ERROR : Chercher id bot dans 'users'", true);
                waitGo_btnLoadBddData.SetActive(false);
                yield break;
            }
        }
        // On garde ces infos en local
        // Si n'existe pas encore dans la liste, le créer
        // Sinon remplacer ses infos
        debugMessage("Liste infos bots de bdd bien retrieve ( "+infoBotsList.Count+" bots )", false);
        _scrollBots.loadFirestoreListBot(infoBotsList, GetComponent<developerBddManager>());
        waitGo_btnLoadBddData.SetActive(false);
    }




    //btnUpdateBot.endChangeState() : -> enlever anim wait

    private fsUsers infoUserToFsUser(infoPlayerTest info)
    {
        return new fsUsers()
        {
            userId = info.idPlayer,
            name = info.nomPlayer,
            nbStar = info.nbStar,

            scoreWeek = info.scoreWeek,
            scoreRecord = info.scoreRecord,
            infoScore = info.idsBoostUsed + '|' + info.levelBoostUsed + "|" + info.timeScoreWeek
            + '|' + info.idsBoostUsed_record + '|' + info.levelBoostUsed_record + "|" + info.timeScoreRecord
        };
    }




    public scrollPlayersDev scrollRank;
    private int offSetPlayers = 10;
    List<infoPlayerTest> currentRankWeek;
    List<infoPlayerTest> currentRankRecord;
    List<infoPlayerPrevWeek> currentRankPrevWeek;
    public void getWeekList()
    {
        Query query_rankWeek = users.OrderByDescending(_fieldValueScoreWeek)
                //.OrderBy(_fieldTimeScoreWeek)
                .WhereGreaterThan(_fieldValueScoreWeek, 0)
                .Limit(100 + offSetPlayers);

        query_rankWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                Debug.Log("BUG : tesk failed");
                //managerRankInfinis.getInstance().errorWhileInitQuery();
            }
            else
            {
                Dictionary<string, object> details;
                fsUsers user;
                infoPlayerTest infoPlayer;

                currentRankWeek = new List<infoPlayerTest>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    details = documentSnapshot.ToDictionary();
                    user = dataUserBdd_tofsUsers(details);
                    infoPlayer = fsUser_toInfoPlayer(user);
                    currentRankWeek.Add(infoPlayer);
                }
                currentRankWeek = reOrderListPlayersWeek_byTimeScore(currentRankWeek);
                while (currentRankWeek.Count > 100)
                    currentRankWeek.RemoveAt(currentRankWeek.Count - 1);
                //aDataIsLoaded();

                scrollRank.rankWeekLoaded(currentRankWeek);
            }
        });
    }

    public void getRecordList()
    {
        Query query_rankRecord = users.OrderByDescending(_fieldValueScoreRecord)
                //.OrderBy(_fieldTimeScoreRecord)
                .WhereGreaterThan(_fieldValueScoreRecord, 0)
                .Limit(200 + offSetPlayers);
        query_rankRecord.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                Debug.Log("BUG : tesk failed");
                //managerRankInfinis.getInstance().errorWhileInitQuery();
            }
            else
            {
                Dictionary<string, object> details;
                fsUsers user;
                infoPlayerTest infoPlayer;

                currentRankRecord = new List<infoPlayerTest>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    details = documentSnapshot.ToDictionary();
                    user = dataUserBdd_tofsUsers(details);
                    infoPlayer = fsUser_toInfoPlayer(user);
                    currentRankRecord.Add(infoPlayer);
                }
                currentRankRecord = reOrderListPlayersRecord_byTimeScore(currentRankRecord);
                while (currentRankRecord.Count > 200)
                    currentRankRecord.RemoveAt(currentRankRecord.Count - 1);
                //aDataIsLoaded();

                scrollRank.rankRecordLoaded(currentRankRecord);
            }
        });
    }

    public void getPrevWeekList()
    {
        Query query_rankPrevWeek = users_prevWeek.OrderByDescending(_fieldValueScorePrevWeek);
        //.Limit(rank.nbPlayerLimitWeek);
        query_rankPrevWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                //Debug.Log("BUG : tesk failed");
                debugMessage("ERROR : cannot get prevWeekList", true);
                //managerRankInfinis.getInstance().errorWhileInitQuery();
            }
            else
            {
                Dictionary<string, object> details;
                fsUsers_prevWeek user;
                infoPlayerPrevWeek infoPlayer;

                currentRankPrevWeek = new List<infoPlayerPrevWeek>();

                int i = 0;
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    details = documentSnapshot.ToDictionary();
                    user = dataUserBddPrevWeek_tofsUserPrevWeek(details);
                    infoPlayer = fsUserPrevWeek_toInfoPlayerPrevWeek(user);

                    currentRankPrevWeek.Add(infoPlayer);

                    i++;
                }
                currentRankPrevWeek = reOrderListPlayersPrevWeek_byTimeScore(currentRankPrevWeek);

                scrollRank.rankPrevWeekLoaded(currentRankPrevWeek);
            }
        });
    }

    private fsUsers_prevWeek dataUserBddPrevWeek_tofsUserPrevWeek(Dictionary<string, object> details)
    {
        fsUsers_prevWeek retour = new fsUsers_prevWeek()
        {
            name = Convert.ToString(details[_fieldUserName]),
            userId = Convert.ToString(details[_fieldUserId]),
            nbStar = Convert.ToInt32(details[_fieldUserNbStar]),

            scorePrevWeek = Convert.ToInt32(details[_fieldValueScorePrevWeek]),
            idBoostUsedPrevWeek = Convert.ToString(details[_fieldIdBoostUsedPrevWeek]),
            levelBoostUsedPrevWeek = Convert.ToString(details[_fieldLevelBoostUsedPrevWeek]),
            timeScore = Convert.ToDouble(details[_fieldTimeScorePrevWeek]),

            rewardTaken = false//Convert.ToBoolean(details[_fieldRewardPrevWeekTaken]),
        };
        return retour;
    }
    private infoPlayerTest fsUser_toInfoPlayer(fsUsers user)
    {
        string[] infosScore = infoScore_toList(user.infoScore);

        infoPlayerTest retour = new infoPlayerTest()
        {
            nomPlayer = user.name,
            idPlayer = user.userId,
            nbStar = user.nbStar,

            scoreWeek = user.scoreWeek,
            scoreRecord = user.scoreRecord,

            idsBoostUsed = infosScore[0],
            idsBoostUsed_record = infosScore[3],

            levelBoostUsed = infosScore[1],
            levelBoostUsed_record = infosScore[4],

            timeScoreWeek = Convert.ToDouble(infosScore[2]),
            timeScoreRecord = Convert.ToDouble(infosScore[5]),
        };
        return retour;
    }

    private List<infoPlayerTest> reOrderListPlayersWeek_byTimeScore(List<infoPlayerTest> list)
    {
        // les joueurs sont déjà classés par scores, on les reclasse par timeScore
        List<infoPlayerTest> retour = new List<infoPlayerTest>();
        List<infoPlayerTest> sameScore;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
                retour.Add(list[i]);
            else
            {
                sameScore = new List<infoPlayerTest>() { list[i] };
                while (i < list.Count - 1 && list[i].scoreWeek == list[i + 1].scoreWeek)
                {
                    sameScore.Add(list[i + 1]);
                    i++;
                }

                if (sameScore.Count == 1)
                    retour.Add(list[i]);
                else
                {
                    // on classe les joueurs ayant le même Score
                    while (sameScore.Count > 0)
                    {
                        int index = 0;
                        double minTime = sameScore[index].timeScoreWeek;
                        for (int e = 1; e < sameScore.Count; e++)
                        {
                            if (sameScore[e].timeScoreWeek < minTime)
                            {
                                minTime = sameScore[e].timeScoreWeek;
                                index = e;
                            }
                        }
                        retour.Add(sameScore[index]);
                        sameScore.RemoveAt(index);
                    }
                }
            }
        }
        return retour;
    }

    private List<infoPlayerTest> reOrderListPlayersRecord_byTimeScore(List<infoPlayerTest> list)
    {
        // les joueurs sont déjà classés par scores, on les reclasse par timeScore
        List<infoPlayerTest> retour = new List<infoPlayerTest>();
        List<infoPlayerTest> sameScore;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
                retour.Add(list[i]);
            else
            {
                sameScore = new List<infoPlayerTest>() { list[i] };
                while (i < list.Count - 1 && list[i].scoreRecord == list[i + 1].scoreRecord)
                {
                    sameScore.Add(list[i + 1]);
                    i++;
                }

                if (sameScore.Count == 1)
                    retour.Add(list[i]);
                else
                {
                    // on classe les joueurs ayant le même Score
                    while (sameScore.Count > 0)
                    {
                        int index = 0;
                        double minTime = sameScore[index].timeScoreRecord;
                        for (int e = 1; e < sameScore.Count; e++)
                        {
                            if (sameScore[e].timeScoreRecord < minTime)
                            {
                                minTime = sameScore[e].timeScoreRecord;
                                index = e;
                            }
                        }
                        retour.Add(sameScore[index]);
                        sameScore.RemoveAt(index);
                    }
                }
            }
        }
        return retour;
    }



    // fonction test reccup dans onlineRankingManager


    // à supprimer, juste pour eviter manip de supp sur base
    /*private void testDeleteAllDataPlayersFromBdd()
    {
        // Ne depasse jamais limit, rank prev week peut aller jusqu'à 100 player max
        WriteBatch batch;// = fsDB.StartBatch();

        //bool querySucced = false;
        //bool queryFailed = false;
        int nbBatch = 0;
        // 1 On vide rank prevWeek bdd (avec batch)
        Debug.Log("Try delete ranking");
        foreach (Query q in new List<Query>() { users, users_prevWeek })
        {

            q.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    Debug.Log("BUG : delete all players failed");
                }
                else
                {
                    batch = fsDB.StartBatch();
                    foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                    {
                        DocumentReference dR = documentSnapshot.Reference;
                        batch.Delete(dR);
                        nbBatch++;
                        if (nbBatch == nbOperationBatch)
                        {
                            nbBatch = 0;
                            batch.CommitAsync();
                            batch = fsDB.StartBatch();
                        }
                    }
                    if (nbBatch > 0)
                        batch.CommitAsync();
                }
            });
        }
    }*/


}
