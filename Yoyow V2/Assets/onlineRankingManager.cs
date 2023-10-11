using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;

public class onlineRankingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        testConnectToFirestore();
    }

    private static onlineRankingManager instance;
    public static bool maintenanceOn = false;
    public static onlineRankingManager getInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("rankingInfinisCanvas").GetComponent<onlineRankingManager>();
        }
        return instance;
    }


    /*
     * void playerScoreInfinis(int score, string dateScore, string boostUsed, string levelBoostUsed)
     * bool canTestScore(double timePastLocal, double timeLeftBdd)
     * List<string> getBestScoreThisWeek(double timeLeftSecBdd)
     * bool isConnectedToInternet()
     * void refreshMaintenanceState(bool callForOpenMenu)
     * void timerEnds_setMaintenance()
     */
    #region Côté player ( transforme data Firestore pour être utilisé )    
    List<List<string>> currentListScoreSaved = new List<List<string>>();
    public void playerScoreInfinis(int score, string dateScore, string boostUsed, string levelBoostUsed)
    {
        // on reccupere la liste de tout les scores faits depuis derniere connection base
        if (currentListScoreSaved.Count == 0)
        {
            currentListScoreSaved = (List<List<string>>)SaveSystem.loadData(typeSave.SCORES_INFINIS_ONLINE);
        }

        // ---------- PARTIE SCORE WEEK
        // on save le score associé à la date

         // save comme ca
        List<string> saveScore = new List<string>() { score.ToString(), dateScore, boostUsed, levelBoostUsed};

        currentListScoreSaved.Add(saveScore);
        SaveSystem.Save(typeSave.SCORES_INFINIS_ONLINE, currentListScoreSaved);

        // ---------- PARTIE SCORE RECORD
        // on reccupère le dernier record fait
        if(prevRecordRetrieved == -1)
        {
            List<string> bestScoreInfo = (List<string>)SaveSystem.loadData(typeSave.RECORD_INFINIS_ONLINE);
            if(bestScoreInfo.Count == 0
                || score > int.Parse(bestScoreInfo[0]))
            {
                // on sauvegarde le nouveau scoreRecord
                bestScoreInfo = new List<string>() { score.ToString(), boostUsed, levelBoostUsed };
                SaveSystem.Save(typeSave.RECORD_INFINIS_ONLINE, bestScoreInfo);
            }
        }
    }

    private int prevRecordRetrieved = -1;


    private double oneWeekInSecond = 60 * 60 * 24 * 7;
    private double delayTimeScoreAllowed = 60 * 10; // 10 minutes
    // renvoit true si timeLocalFromScore + timeRestantBdd < 7j
    private bool canTestScore(double timePastLocal, double timeLeftBdd)
    {
        double timeScoreIfDonneInBdd = timeLeftBdd + timePastLocal;
        return timeScoreIfDonneInBdd <= oneWeekInSecond + delayTimeScoreAllowed;
    }

    // regarder dans scores fais en local
    public List<string> getBestScoreThisWeek(double timeLeftSecBdd)
    {
        // on reccupere la liste de tout les scores faits depuis derniere connection base
        if (currentListScoreSaved.Count == 0)
        {
            currentListScoreSaved = (List<List<string>>)SaveSystem.loadData(typeSave.SCORES_INFINIS_ONLINE);
        }

        List<string> bestScoreDoneThisWeek = new List<string>();
        double timePastScoreToNow;
        foreach (List<string> l in currentListScoreSaved)
        {
            if (bestScoreDoneThisWeek.Count == 0)
            {
                bestScoreDoneThisWeek = l;
            }
            else
            {
                timePastScoreToNow = (Convert.ToDateTime(DateTime.Now.ToString()) - Convert.ToDateTime(l[1])).TotalSeconds;
                if (canTestScore(timePastScoreToNow, timeLeftSecBdd))
                {
                    if (int.Parse(l[0]) > int.Parse(bestScoreDoneThisWeek[0]))
                        bestScoreDoneThisWeek = l;
                }
            }
        }

        return bestScoreDoneThisWeek;

        
    }

    public rankingInfinis rank;


    //public bool test_connectionOn = true;
    public bool isConnectedToInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable && firestoreEnabled; 
    }

    public void refreshMaintenanceState(bool callForOpenMenu)
    {
        // on regarde d'abord si temps date fin base est inférieur au temps actuel de la base
        // mode maintenance fictif activé ( ne l'est pas dans la base

        // si update date, refresh fonctionne pas, timeNext nest pas refresh
        // testLoadTimeNextWeek()
        double currentTimeBdd = getTimeFirestoreBdd();
        double nextTimeBdd = timeNextWeek;

        //Debug.Log("Check si week time out");
        // timeNextWeek - getTimeFirestoreBdd();
        if (nextTimeBdd < currentTimeBdd)
        {
            //Debug.Log("TIME OUT");
            maintenanceOn = true;

            if (callForOpenMenu)
            {
                if (managerRankInfinis.waitingMaintenance_init)
                {
                    managerRankInfinis.getInstance().resultMaintenanceInitMenu(true);
                }
                else if (managerRankInfinis.waitingMaintenance_afterAccount)
                {
                    managerRankInfinis.getInstance().resultMaintenanceAfterCreateAccount(true);
                }
            }

            testLoadTimeNextWeek(true);
            return;
        }

        // -> cash ouvrir maintenance

        // Debug.Log("Check si maintenance on");
        // fait requête pour mettre à jour maintenance
        // fin de requête, rafraichir mode maintenance de maintenanceMenu
        Query query_updateMaintenance = bddInfos.WhereEqualTo(_fieldIdInfos, _docModeMaintenance);
        query_updateMaintenance.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                managerRankInfinis.getInstance().errorWhileInitQuery();
            }
            else
            {
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    //Debug.Log("RESULT MAINTENANCE");
                    Dictionary<string, object> details = documentSnapshot.ToDictionary();
                    maintenanceOn = Convert.ToBoolean(details[_fieldModeMaintenance]);

                    if (callForOpenMenu)
                    {
                        if (managerRankInfinis.waitingMaintenance_init)
                        {
                            //Debug.Log("(1)");
                            managerRankInfinis.getInstance().resultMaintenanceInitMenu(maintenanceOn);
                        }
                        else if (managerRankInfinis.waitingMaintenance_afterAccount)
                        {
                            //Debug.Log("(2)");
                            managerRankInfinis.getInstance().resultMaintenanceAfterCreateAccount(maintenanceOn);
                        }
                    }
                    break;
                }
            }
        });
    }

    public void timerEnds_setMaintenance()
    {
        maintenanceOn = true;
    }

    #endregion



    // 2 ------------------------ GESTION FIRESTORE

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

            userData.Add(_fieldValueScoreInfos, this.infoScore == "" ? "0,0,0|0,0,0|0|0,0,0|0,0,0|0":this.infoScore);

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

    private fsUsers _fsUsers;
    private fsUsers_prevWeek _fsUsers_prevWeek;


    private FirebaseFirestore fsDB;
    private CollectionReference users;
    private CollectionReference users_prevWeek;
    private CollectionReference bddInfos;

    [Header("Firebase")]
    public DependencyStatus depedencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    /*
     * void testConnectToFirestore()
     * void setUpFirebase()
     * void AuthStateChanged(object sender, System.EventArgs eventArgs)
     * void testSignInAnonym()
    */
    #region Initialisation automatique
    // normalement dans awake, se connecter directement au lancement de l'app
    public void testConnectToFirestore()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            depedencyStatus = task.Result;
            if (depedencyStatus == DependencyStatus.Available)
            {
                setUpFirebase();
                firestoreEnabled = true;
            }
            else
            {
                Debug.LogError("Could not resolve all firebase depedencies " + depedencyStatus);
                firestoreEnabled = false;
            }
        });
    }

    private void setUpFirebase()
    {
        fsDB = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        users = fsDB.Collection(_collectionUsers);
        users_prevWeek = fsDB.Collection(_collectionUsers_prevWeek);
        bddInfos = fsDB.Collection(_collectionBddInfos);
        testSignInAnonym();
    }

    public static bool initAutoLogSucced = false;
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log("<color=red>Init log succed, player already exists</color>");
            initAutoLogSucced = true;
            User = auth.CurrentUser;
        }
        else
        {
            Debug.Log("<color=red>NO USER FOUND</color>");
            //initAutoLogSucced = false;
        }
    }

    public static bool firestoreEnabled = false;
    //private bool playerIsConnected = false;
    private string localPlayerIdInBdd;
    public void testSignInAnonym()
    {
        // on regarde d'abord si on est déja co ou non
        if (!initAutoLogSucced)
        {
            Debug.Log("Try signIn");
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.Log("<color=red>BUG : sign anonymously failed</color>");
                    //fbConnec.registerConnecError_2();
                    //fbConnec.waitConnection(false);
                }
                else
                {
                    User = task.Result.User;
                    localPlayerIdInBdd = User.UserId;
                    Debug.Log("<color=green>Sign anonymously succed (id:</color>" + localPlayerIdInBdd + ')');
                    //StartCoroutine(initPlayerData(nameChoose));
                    //playerIsConnected = true;
                }
            });
        }
        else
        {
            Debug.Log("Already connected, check data same as base");
            //StartCoroutine(initPlayerData(nameChoose));
            //playerIsConnected = true;
        }
    }
    #endregion

    /*
     * void createNewAccount(string namePlayer)
     * void fsAddNewUser(fsUsers userInfos, bool addingLocalPlayer)
     * void checkIfNamePlayerAlreadyUsed(string nameTested)
    */
    #region Creation de compte
    private void createNewAccount(string namePlayer)
    {
        Debug.Log("Create new account");
        fsUsers localInfos = getInfoLocalUser(namePlayer);
        localInfos.name = namePlayer;

        Debug.Log("2 Create new account");
        fsAddNewUser(localInfos, true);
    }
    // devrait être appelé quand player menu sign in et essaie de créer un compte
    private void fsAddNewUser(fsUsers userInfos, bool addingLocalPlayer)
    {
        Query query = users.WhereEqualTo(_fieldUserId, userInfos.userId);

        query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (!querySnapshotTask.IsCompleted)
            {
                Debug.Log("Erreur creation compte");
                //Debug.Log("<color=red>Task failed : </color>");
            }
            else
            {
                Debug.Log("Continue create account");
                int nbP = 0;
                if (querySnapshotTask.Result.Documents != null)
                    foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                        nbP++;

                // on créer un utilisateur
                if (nbP == 0)
                {
                    Dictionary<string, object> userData = userInfos.toDictionnary();
                    DocumentReference docRef = users.Document(userInfos.userId);
                    Debug.Log("2 Continue create account");
                    docRef.SetAsync(userData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.Log("(error) Continue create account");
                            managerRankInfinis.waitingAccountCreated = false;
                            managerRankInfinis.error_waitingAccountCreated = true;
                            //Debug.Log("<color=red>Task User registration failed</color>");
                        }
                        else
                        {
                            Debug.Log("<color=green>User registered</color>");
                            managerRankInfinis.waitingAccountCreated = false;
                            //managerRankInfinis.getInstance().playerPassedCreateAccountPhase(true);
                            //hasCreatedPlayer = true;
                        }
                        //isDoingARequest = false;
                    });

                    if (addingLocalPlayer)
                    {
                        prevPlayerInfosRetrieve = userInfos;
                        _fsUsers = userInfos;
                        //alreadyLoadedLocalPlayerData = true;
                    }
                }
                else
                {
                    Debug.Log("<color=red>Already registered</color>");
                }
            }
        });
    }

    public createAccountMenu accountMenu;
    public void checkIfNamePlayerAlreadyUsed(string nameTested)
    {
        Debug.Log("Test name exists");
        Query query = users.WhereEqualTo(_fieldUserName, nameTested);
        query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (!querySnapshotTask.IsCompleted)
            {
                Debug.Log("(name exists) Test name exists");
                accountMenu.StartCoroutine(accountMenu.waitCreateAccount_resultNameUsed(true));
            }
            else
            {
                Debug.Log("(name is free) Test name exists");
                int nbP = 0;
                if (querySnapshotTask.Result.Documents != null)
                    foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                        nbP++;
                accountMenu.StartCoroutine(accountMenu.waitCreateAccount_resultNameUsed(nbP != 0));
                if (nbP == 0)
                {
                    managerRankInfinis.waitingAccountCreated = true;
                    createNewAccount(nameTested);
                }
            }
        });
    }
    #endregion

    /*
     * void loadAllDatas()
     * void loadAllDatas_suite()
     * void aDataIsLoaded()
     * void testLoadRankings(bool forceResetPrevWeek, bool resetWeek, bool resetRecord)
     * bool localTimeSavedEqualsBddTime()
     * 
     * List<infoPlayerTest> reOrderListPlayersWeek_byTimeScore(List<infoPlayerTest> list)
     * List<infoPlayerTest> reOrderListPlayersRecord_byTimeScore(List<infoPlayerTest> list)
     * List<infoPlayerPrevWeek> reOrderListPlayersPrevWeek_byTimeScore(List<infoPlayerPrevWeek> list)
    */
    #region Load data from Firestore
    // appelé si account déja créer, juste reccup info base
    //private bool alreadyLoadedLocalPlayerData = false;
    private fsUsers prevPlayerInfosRetrieve;

    // on a besoin de d'abord réccupérer le timer next week de bdd
    // si est le même que saved local, réutiliser le ranking saved en local
    public void loadAllDatas(bool reloadRankings)
    {
        reloadRankingsAtOpen = reloadRankings;
        //Debug.Log("Load all data");
        currentNbDataLoaded = 0;
        testLoadTimeNextWeek(false);
    }

    private bool reloadRankingsAtOpen;
    public static bool anyRanksHasChanged_lastOpenMenu;
    public void loadAllDatas_suite()
    {
        // on doit aussi reload si la date dans la base n'est pas la même que en local
        bool savedTimeBddEqualsTimeBdd = localTimeSavedEqualsBddTime();

        if (reloadRankingsAtOpen
            || !savedTimeBddEqualsTimeBdd)
        {
            anyRanksHasChanged_lastOpenMenu = true;
            if (!savedTimeBddEqualsTimeBdd && !reloadRankingsAtOpen)
                managerRankInfinis.getInstance().weekChanged();

            // IL faut reload le rank qui est impacté par les changements de player
            bool resetWeek = false;
            bool resetRecord = false;

            if (!savedTimeBddEqualsTimeBdd)
            {
                //Debug.Log("Time bdd not same local, update ranks");
                resetWeek = true;
                resetRecord = true;
            }
            // reloadRankingsAtOppen = true SI infos de player ont changés OU timer past
            else if (reloadRankingsAtOpen)
            {
                if (lastUpdate_playerScoreWeekChanged
                    || lastUpdate_playerScoreRecordChanged) {
                    resetWeek = lastUpdate_playerScoreWeekChanged;
                    resetRecord = lastUpdate_playerScoreRecordChanged;
                    //Debug.Log("Score player changed, update ranks");
                }
                else // c'est timer qui a changé
                {
                    resetWeek = true;
                    resetRecord = true;
                    //Debug.Log("Timer update bdd out, update ranks");
                }
            }

            //Debug.Log("Player reloadRankingWhenReopen");
            testLoadRankings(!savedTimeBddEqualsTimeBdd, resetWeek, resetRecord);
        }
        else
        {
            anyRanksHasChanged_lastOpenMenu = false;
            //Debug.Log("Player reuse All prevRank");
            // on ne load pas les rankings
            aDataIsLoaded(); // pour rank week
            aDataIsLoaded(); // pour rank record
            aDataIsLoaded(); // pour rank prevWeek
        }
    }


    private const int nbDataToBeLoaded = 4;
    private int currentNbDataLoaded;
    private void aDataIsLoaded()
    {
        currentNbDataLoaded++;
        if(currentNbDataLoaded == nbDataToBeLoaded)
        {
            //Debug.Log("All datas loaded");
            // on met à jour ranking infinis;
            rank.liste_week = currentRankWeek;
            rank.liste_record = currentRankRecord;
            rank.setPrevWeekList(currentRankPrevWeek);
            rank.localPlayerInfos = fsUser_toInfoPlayer(prevPlayerInfosRetrieve);
            rank.openMenuRankingInfinis_suiteDataLoaded(anyRanksHasChanged_lastOpenMenu);
            managerRankInfinis.getInstance().closeInitLoadAnim(false);
        }
    }


    private List<infoPlayerTest> currentRankWeek = new List<infoPlayerTest>();
    private List<infoPlayerTest> currentRankRecord = new List<infoPlayerTest>();
    private List<infoPlayerPrevWeek> currentRankPrevWeek = new List<infoPlayerPrevWeek>();
    public int offSetPlayers = 10; // en reccupere x de plus, trie tout par score -> time, puis enlève x player

    private List<object> savedRankPrevWeek;

    public void testLoadRankings(bool forceResetPrevWeek, bool resetWeek, bool resetRecord)
    {
        if (resetWeek)
        {
            Query query_rankWeek = users.OrderByDescending(_fieldValueScoreWeek)
                //.OrderBy(_fieldTimeScoreWeek)
                .WhereGreaterThan(_fieldValueScoreWeek, 0)
                .Limit(rank.nbPlayerLimitWeek + offSetPlayers);
            query_rankWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    Debug.Log("BUG : tesk failed");
                    managerRankInfinis.getInstance().errorWhileInitQuery();
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
                    while (currentRankWeek.Count > rank.nbPlayerLimitWeek)
                        currentRankWeek.RemoveAt(currentRankWeek.Count - 1);
                    aDataIsLoaded();
                }
            });
        }
        else
        {
            aDataIsLoaded();
        }

        if (resetRecord)
        {
            Query query_rankRecord = users.OrderByDescending(_fieldValueScoreRecord)
                //.OrderBy(_fieldTimeScoreRecord)
                .WhereGreaterThan(_fieldValueScoreRecord, 0)
                .Limit(rank.nbPlayerLimitRecord + offSetPlayers);
            query_rankRecord.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    Debug.Log("BUG : tesk failed");
                    managerRankInfinis.getInstance().errorWhileInitQuery();
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
                    while (currentRankRecord.Count > rank.nbPlayerLimitRecord)
                        currentRankRecord.RemoveAt(currentRankRecord.Count - 1);
                    aDataIsLoaded();
                }
            });
        }
        else
        {
            aDataIsLoaded();
        }


        // si pas de save rank // timer next week pas le même, reload prevWeekRanking
        // si on change de date de tournois, on doit forcer la mise à jour interne du rank précédent
        bool rankAlreadySet = true;

        if (!forceResetPrevWeek)
        {
            savedRankPrevWeek = (List<object>)SaveSystem.loadData(typeSave.FIRESTORE_PREV_WEEK_RANK);
            if (savedRankPrevWeek.Count == 0)
            {
                rankAlreadySet = false;
            }
            else
            {
                double prevTimeSaved = Convert.ToDouble(savedRankPrevWeek[0]);
                if (prevTimeSaved != timeNextWeek)
                    rankAlreadySet = false;
            }
        }
        if (!rankAlreadySet
            || forceResetPrevWeek)
        {
            //Debug.Log("Recreate ranking week saved in local");
            Query query_rankPrevWeek = users_prevWeek.OrderByDescending(_fieldValueScorePrevWeek);
            //.Limit(rank.nbPlayerLimitWeek);
            query_rankPrevWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    //Debug.Log("BUG : tesk failed");
                    managerRankInfinis.getInstance().errorWhileInitQuery();
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

                        if (infoPlayer.idPlayer == User.UserId)
                        {
                            //Debug.Log("Local player retrieved from prev rank");
                            _fsUsers_prevWeek = user;
                        }
                        i++;
                    }
                    //Debug.Log("Nombre de players ajoutés au rank : " + i);
                    currentRankPrevWeek = reOrderListPlayersPrevWeek_byTimeScore(currentRankPrevWeek);


                    List<string> infosPlayersSavedLocal = new List<string>();
                    // avec la liste trié, on créer la liste save
                    foreach (infoPlayerPrevWeek info in currentRankPrevWeek)
                    {
                        infosPlayersSavedLocal.Add(infoPrevWeek_toFsUserPrevWeek(info).userToString());
                    }

                    List<object> newSave = new List<object>()
                    {
                        timeNextWeek,
                        infosPlayersSavedLocal
                    };
                    savedRankPrevWeek = newSave;
                    SaveSystem.Save(typeSave.FIRESTORE_PREV_WEEK_RANK, newSave);


                    aDataIsLoaded();
                }
            });
        }
        else
        {
            //Debug.Log("Re-use ranking week saved in local");
            // on réutilise les data saved pour prevWeekRank
            currentRankPrevWeek = new List<infoPlayerPrevWeek>();
            List<string> infoPlayers = (List<string>)savedRankPrevWeek[1];
            fsUsers_prevWeek fU = new fsUsers_prevWeek();
            foreach(string s in infoPlayers)
            {
                fU.stringToUser(s);
                currentRankPrevWeek.Add(fsUserPrevWeek_toInfoPlayerPrevWeek(fU));
                if(fU.userId == User.UserId)
                    _fsUsers_prevWeek = fU;
            }

            aDataIsLoaded();
        }
    }

    private bool localTimeSavedEqualsBddTime()
    {
        if(savedRankPrevWeek == null)
            savedRankPrevWeek = (List<object>)SaveSystem.loadData(typeSave.FIRESTORE_PREV_WEEK_RANK);

        if (savedRankPrevWeek.Count == 0)
            return false;
        else
        {
            double prevTimeSaved = Convert.ToDouble(savedRankPrevWeek[0]);
            return prevTimeSaved == timeNextWeek;
        }
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
                while(i < list.Count - 1 && list[i].scoreWeek == list[i + 1].scoreWeek)
                {
                    sameScore.Add(list[i + 1]);
                    i++;
                }

                if(sameScore.Count == 1)
                    retour.Add(list[i]);
                else
                {
                    // on classe les joueurs ayant le même Score
                    while(sameScore.Count > 0)
                    {
                        int index = 0;
                        double minTime = sameScore[index].timeScoreWeek;
                        for(int e = 1; e < sameScore.Count; e++)
                        {
                            if(sameScore[e].timeScoreWeek < minTime)
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
                while (i < list.Count - 1 && list[i].scoreRecord== list[i + 1].scoreRecord)
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

    #endregion

    private string infoScore_toString(string idBoostUsed_week, string levelBoostUsed_week, string timeScore_week
        , string idBoostUsed_record, string levelBoostUsed_record, string timeScore_record)
    {
        return idBoostUsed_week + "|" + levelBoostUsed_week + "|" + timeScore_week + "|"
            + idBoostUsed_record + "|" + levelBoostUsed_record + "|" + timeScore_record;
    }

    private string[] infoScore_toList(string s)
    {
        string s_2;
        //Debug.Log("Enter get info score");
        if (s == null)
        {
            //Debug.Log("String null");
            s_2 = "0,0,0|0,0,0|0|0,0,0|0,0,0|0";
        }
        else
        {
            //Debug.Log("s : " + s + " , length : " + s.Length);
            s_2 = s == "" ? "0,0,0|0,0,0|0|0,0,0|0,0,0|0" : s;
        }

        //Debug.Log("Split info score : " + s_2);
        return s_2.Split('|');
    }

    public bool localPlayerHasTakenRewardPrevWeek()
    {
        return _fsUsers_prevWeek.rewardTaken;
    }
    public void playerTryGetRewardRankPrevWeek()
    {
        _fsUsers_prevWeek.rewardTaken = true;

        // pas besoin d'internet pour reccuperer reward
        List<string> infosPlayersSavedLocal = new List<string>();
        fsUsers_prevWeek user;
        // on créer la liste save
        foreach (infoPlayerPrevWeek info in currentRankPrevWeek)
        {
            user = infoPrevWeek_toFsUserPrevWeek(info);
            if (user.userId == User.UserId)
                user.rewardTaken = true;
            infosPlayersSavedLocal.Add(user.userToString());
        }

        List<object> newSave = new List<object>()
                    {
                        timeNextWeek,
                        infosPlayersSavedLocal
                    };
        SaveSystem.Save(typeSave.FIRESTORE_PREV_WEEK_RANK, newSave);

        rank.resultGetRewardPrevWeek(true);
    }

    public static bool accountAlreadyExists = false;
    //private Dictionary<string, object> localPlayerDetails = new Dictionary<string, object>();
    public void fsTestLocalPlayerHasAnAccount() // bool calledAfterMaintenance)
    {
        // si le joueur a déjà lancé le menu 1 fois, et que a déjà check si account existe
        // deja check se met à true SI , créer compte succed OU reccuperer compte de base succed
        // -> ne pas recheck, partir du principe que C'EST BON
        if (accountAlreadyExists)
        {
            managerRankInfinis.getInstance().playerPassedCheckAccountExistPhase(true);
        }
        else
        {
            Query query = users.WhereEqualTo(_fieldUserId, localPlayerIdInBdd);
            query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    managerRankInfinis.getInstance().errorWhileInitQuery();
                }
                else
                {
                    int nbP = 0;
                // ne doit que renvoyer 1 data, pas possible que 2 players ont le même id
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                    {
                        prevPlayerInfosRetrieve = dataUserBdd_tofsUsers(documentSnapshot.ToDictionary());
                        _fsUsers = prevPlayerInfosRetrieve;
                        rank.localPlayerInfos = fsUser_toInfoPlayer(_fsUsers);
                        nbP++;
                    }
                /*if (calledAfterMaintenance)
                {
                    fsUpdateLocalPlayerValues_2();
                }
                else
                {*/
                    if (nbP != 0)
                    {
                    // est appelé avant que _fsUsers est retrieved de la base
                    //Debug.Log("(checkPlayer registered) Player already has an account");
                    managerRankInfinis.getInstance().playerPassedCheckAccountExistPhase(true);
                        accountAlreadyExists = true;
                    }
                    else
                    {
                    //Debug.Log("(checkPlayer registered) Player has NO account");
                    managerRankInfinis.getInstance().playerPassedCheckAccountExistPhase(false);
                    // accountAlreadyExists sera mit à true une fois le compte bien crée
                }
                //}
            }
            });
        }
    }


    private bool lastUpdate_playerScoreWeekChanged;
    private bool lastUpdate_playerScoreRecordChanged;
    // update valeure base si pas les mêmes

    public void fsUpdateLocalPlayerValues_2()
    {
        //Debug.Log("-1");
        double timeLeftBdd = getTimeLeft_toNextWeek();
        bool valueChanged = false;
        lastUpdate_playerScoreWeekChanged = false;
        lastUpdate_playerScoreRecordChanged = false;
        //Debug.Log("-2");
        double currentTime = getTimeFirestoreBdd();

        //Debug.Log("-3");
        string[] prevInfo = infoScore_toList(_fsUsers.infoScore);
        //Debug.Log("Nombre d'items dans liste info score : " + prevInfo.Length);
        //foreach (string s in prevInfo)
          //  Debug.Log("i : " + s);
        string s_1 = prevInfo[0], s_2 = prevInfo[1], s_3 = prevInfo[2]
            , s_4 = prevInfo[3], s_5 = prevInfo[4], s_6 = prevInfo[5];

        //Debug.Log("-4");
        List<string> bestScoreWeek = getBestScoreThisWeek(timeLeftBdd);

        //Debug.Log("Check score changed");
        if (bestScoreWeek.Count != 0)
        {
            int bestScoreThisWeek = int.Parse(bestScoreWeek[0]);
            if (bestScoreThisWeek != 0 && bestScoreThisWeek > _fsUsers.scoreWeek)
            {
                valueChanged = true;
                lastUpdate_playerScoreWeekChanged = true;

                _fsUsers.scoreWeek = bestScoreThisWeek;
                
                // score , time , boost used , level used
                s_1 = bestScoreWeek[2];
                s_2 = bestScoreWeek[3];
                s_3 = currentTime.ToString();
            }
        }
        //Debug.Log("-5");
        List<string> bestScoreInfo = (List<string>)SaveSystem.loadData(typeSave.RECORD_INFINIS_ONLINE);
        if (bestScoreInfo.Count != 0)
        {
            int record = int.Parse(bestScoreInfo[0]);
            if (record > _fsUsers.scoreRecord)
            {
                valueChanged = true;
                lastUpdate_playerScoreRecordChanged = true;

                _fsUsers.scoreRecord = record;

                // score , boost used , level used
                s_4 = bestScoreInfo[1];
                s_5 = bestScoreInfo[2];
                s_6 = currentTime.ToString();
            }
        }
       // Debug.Log("-6");

        _fsUsers.infoScore = infoScore_toString(s_1, s_2, s_3, s_4, s_5, s_6);
        int nbStar = SaveSystem.getNbStarObtain();
        if(nbStar > _fsUsers.nbStar)
        {
            valueChanged = true;
            _fsUsers.nbStar = nbStar;
        }

        // Debug.Log("Try update player values in bdd");

        if (valueChanged)
        {
            //Debug.Log("UPDATE VALUES");
            prevPlayerInfosRetrieve = _fsUsers;
            rank.localPlayerInfos = fsUser_toInfoPlayer(_fsUsers);

            // on met à jour la base avec les infos de player
            users.Document(User.UserId).UpdateAsync(prevPlayerInfosRetrieve.toDictionnary()).ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    //Debug.Log("-1");
                    managerRankInfinis.getInstance().errorWhileInitQuery();
                }
                else
                {
                    //Debug.Log("-2");
                    if (lastUpdate_playerScoreWeekChanged)
                    {
                        // Une fois que la base est à jour, on supprime data
                        currentListScoreSaved = new List<List<string>>();
                        SaveSystem.Save(typeSave.SCORES_INFINIS_ONLINE, currentListScoreSaved);
                    }
                    bool playerRankValueChanged = lastUpdate_playerScoreWeekChanged || lastUpdate_playerScoreRecordChanged;
                    managerRankInfinis.getInstance().resultUpdatePlayerValuesInBdd(playerRankValueChanged);
                }
            });
        }
        else
        {
            //Debug.Log("Values in bdd already updated");
            // ne pas reload les ranking
            managerRankInfinis.getInstance().resultUpdatePlayerValuesInBdd(false);
        }
    }



    public static double timeNextWeek = -1;
    
    public void testLoadTimeNextWeek(bool toCheckMaintenance)
    {
        Query query_timeNextWeek = bddInfos.WhereEqualTo(_fieldIdInfos, _docDateWeekTournament);

        query_timeNextWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                //Debug.Log("BUG : no time found in database");
                managerRankInfinis.getInstance().errorWhileInitQuery();
            }
            else
            {
                //Debug.Log("TIME BDD NEXT WEEK RETRIEVED");
                int nbData = 0;
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    Dictionary<string, object> details = documentSnapshot.ToDictionary();
                    timeNextWeek = Convert.ToDouble(details[_fieldNextTimeSecTournament]);
                    if (!toCheckMaintenance)
                    {
                        aDataIsLoaded();
                        loadAllDatas_suite();
                    }
                    nbData++;
                    break;
                }
                //Debug.Log("Number of data in query : " + nbData);
            }
        });
    }
    private double getTimeFirestoreBdd()
    {
        //Firebase.Firestore.Timestamp.
        Timestamp timeBdd = Timestamp.GetCurrentTimestamp();
        DateTime timeBdd_toDate = timeBdd.ToDateTime();
        long elapsedTicks = timeBdd_toDate.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        double seconds = elapsedSpan.TotalSeconds;
        //Debug.Log("Current time bdd in seconds : " + seconds);
        return seconds;
    }
    public double getTimeLeft_toNextWeek()
    {
        return timeNextWeek - getTimeFirestoreBdd();
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

    private infoPlayerPrevWeek fsUserPrevWeek_toInfoPlayerPrevWeek(fsUsers_prevWeek user)
    {
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

    public string getIdLocalPlayer()
    {
        return User.UserId;
    }
    private fsUsers getInfoLocalUser(string name)
    {
        fsUsers ret = new fsUsers();
        ret.userId = User.UserId;
        ret.name = name;
        ret.nbStar = 0; SaveSystem.getNbStarObtain();// 0;
        //.
        ret.scoreWeek = 0;
        ret.scoreRecord = 0;
        ret.infoScore = "0,0,0|0,0,0|0|0,0,0|0,0,0|0";

        return ret;
    }


    /*private fsUsers getTestUser()
    {
        fsUsers ret = new fsUsers();
        ret.userId = User.UserId;
        ret.name = User.UserId;
        ret.nbStar = 12;

        ret.scoreWeek = 10;
        ret.scoreRecord = 25;
        ret.infoScore = "1,11,21|1,2,2|10000|1,11,21|1,2,2|9000";

        return ret;
    }


    private void fsUpdateDateBdd_toNextDate()
    {
        Query query_timeNextWeek = bddInfos.WhereEqualTo(_fieldIdInfos, _docDateWeekTournament);

        // on reccupère d'abord le temps actuel de base
        query_timeNextWeek.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                Debug.Log("BUG : no time found in database");
            }
            else
            {
                Debug.Log("TIME BDD NEXT WEEK RETRIEVED");
                int nbData = 0;
                foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                {
                    Dictionary<string, object> details = documentSnapshot.ToDictionary();
                    timeNextWeek = Convert.ToDouble(details[_fieldNextTimeSecTournament]);
                    // puis on créer la nouvelle date
                    Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldNextTimeSecTournament, getNextWeekTime() } };
                    //, { _fieldIdInfos, _fieldNextTimeSecTournament} };

                    //_fsUsers_prevWeek.rewardTaken = true;
                    bddInfos.Document(_docDateWeekTournament).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask_2) =>
                    {
                        if (querySnapshotTask_2.IsFaulted)
                        {
                            Debug.Log("BUG : dateNextWeek not modified");
                        }
                        else
                        {
                            Debug.Log("dateNextWeek modified");
                        }
                    });
                    break;
                }
                Debug.Log("Number of data in query : " + nbData);
            }
        });


    }
    private void testCreateNextDateWeek()
    {
        double currentTimeBdd = getTimeFirestoreBdd();
        DateTime currentDate = DateTime.Now;
        DateTime nextDate = DateTime.Parse("17/03/2023 20:00:00");
        double nbSecDiff = (nextDate - currentDate).TotalSeconds;

        Dictionary<string, object> newData = new Dictionary<string, object>()
            { {_fieldNextTimeSecTournament, currentTimeBdd + nbSecDiff } };
        //, { _fieldIdInfos, _fieldNextTimeSecTournament} };

        //_fsUsers_prevWeek.rewardTaken = true;
        bddInfos.Document(_docDateWeekTournament).UpdateAsync(newData).ContinueWithOnMainThread((querySnapshotTask) =>
        {
            if (querySnapshotTask.IsFaulted)
            {
                Debug.Log("BUG : dateNextWeek not modified");
            }
            else
            {
                Debug.Log("dateNextWeek modified");
            }
        });
    }*/
    public double getNextWeekTime()
    {
        //Debug.Log("Time next week : " + timeNextWeek + " , oneWeekInseconds : " + oneWeekInSecond + " , Addition : " + (timeNextWeek + oneWeekInSecond));
        return timeNextWeek + oneWeekInSecond;
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
}

// 851l -> 657l
// 1071
// 2003l -> 1373l
