using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeRequestBdd
{
    none,
    openMenu_checkDate,
    openMenu_checkMaintenance,
    openMenu_changeAccount,
    openMenu_checkAccount,
    openMenu_updatePlayerData,
    openMenu_recheckDate,
    openMenu_getRankValues,

    checkStateMaintenance_menuMaint,
    createAccount_recheckMaintenance,
    createAccount_tryName,
    createAccount_tryCreateAcc,
}

public enum typeDisconnectInternet
{
    none,
    checkingMaintenance,
    inRankMenu,
}

public class managerRankInfinisV2 : MonoBehaviour
{
    public Text debug_lastRequestCalled_text;
    public Text debug_lastRequestEnded_text;
    public Text debug_lastRequestTimeOut_text;
    public bool test_showRequestStates = false;
    private void debug_requestCalled(string infos)
    {
        if(test_showRequestStates)
            debug_lastRequestCalled_text.text = "Dernière requête lancée : " + infos;
    }

    private void debug_requestEnded(string infos, float timeWait)
    {
        if (test_showRequestStates)
            debug_lastRequestEnded_text.text = "Dernière requête Ended : " + infos+ " ("+ timeWait + "s)";
    }

    private void debug_requestTimeOut(string infos, float timeWait)
    {
        if (test_showRequestStates)
            debug_lastRequestTimeOut_text.text = "Dernière requête timeOut : " + infos + " (" + timeWait + "s)";
    }

    public GameObject debuggerGo;
    public void btnShowDebugger()
    {
        debuggerGo.SetActive(!debuggerGo.activeSelf);
    }


    // Liste des requete qui, si echoue, a juste besoin de fermer la fenetre waitAnim
    private List<typeRequestBdd> listRequestError_closeSimple = new List<typeRequestBdd>() {
        typeRequestBdd.openMenu_checkDate,
        typeRequestBdd.openMenu_checkMaintenance,
        typeRequestBdd.openMenu_checkAccount,
        typeRequestBdd.openMenu_updatePlayerData,
        typeRequestBdd.openMenu_recheckDate,
        typeRequestBdd.openMenu_getRankValues,
        typeRequestBdd.createAccount_recheckMaintenance,
        typeRequestBdd.createAccount_tryCreateAcc,
        typeRequestBdd.openMenu_changeAccount,
    };

    private bool initLockAnimOn = false;
    public Animation initLockAnim;
    /*
     * void openInitLoadAnim()
     * void closeInitLoadAnim(bool errorQuery)
    */
    #region ecran wait entre 2 phase, en attente de reponse request
    public void openInitLoadAnim()
    {
        initLockAnimOn = true;
        initLockAnim.Play("initLockRankOpen");
    }

    public void closeInitLoadAnim()
    {
        if (initLockAnimOn)
        {
            initLockAnim.Play("initLockRankClose");
            initLockAnimOn = false;
            /*if (errorQuery)
            {
                // on affiche no co anim
                StartCoroutine(openNoConnection());
            }*/
        }
    }
    #endregion

    // activé quand open connec V
    // re-activé quand rankTimeOut V
    // desactivé quand error query V
    // desactivé quand end process connec V
    // desactivé quand menuCreateAcc/menuMaintenance userClose
    public static bool isInConnectionProcess = false;

    public maintenanceMenu maintenanceM;
    public onlineRankingManager onlineM;
    public createAccountMenu menuCreateAcc;
    public rankingInfinis rank;

    private float timeRefreshBdd = 60 * 10; // 10 minutes
    private float prevTimeRefreshDataBdd = -1f;
    private int currentIdMaintenanceChecked = 0; // 0: init, 1: afterAccount | savoir si a déjà checkMaintenance avant createAcc
    private string currentNameTested = "";

    public static bool accountValidated = false;

    private bool shouldReloadTimeNextWeek; // si a ouvert la maintenance OU creerAcc, recheckTimeNext week avant open ranks

    
    /*
     * void btnOpenMenuRanking()
     * void btnCloseRankingInfinis()
     * void aRequestEnded(int idReq, bool succed, object result) : lance les events en fonction reponses
     * void loadParallelRequest(typeRequestBdd type, object data)
     * int getNextIdRequestBdd()
    */
    #region gestion process connection; initialisation, requêtes BDD
    // appelé depuis menu mode infinis, quand clique sur le bouton rank infinis
    public void btnOpenMenuRanking()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);
        infoMenuManager.getInstance().btnInfo_rank(); // anim click btn si pas d'anim notif defi en court

        if (onlineRankingManager.getInstance().isFireStoreInitialized())
        {
            StartCoroutine(launcher.checkInternetConnection((isConnected) =>
            {
                if(isConnected)
                {
                    isInConnectionProcess = true;
                    shouldReloadTimeNextWeek = false;
                    openInitLoadAnim();
                    int idReq = getNextIdRequestBdd();
                    StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_checkDate));
                    StartCoroutine(onlineM.testLoadTimeNextWeek_openMenu(idReq));
                }
                else
                {
                    Debug.Log("openNoConnection() : btnOpenMenuRanking()");
                    StartCoroutine(openNoConnection());
                }
            }));
        }
        else
        {
            Debug.Log("openNoConnection() : btnOpenMenuRanking()");
            StartCoroutine(openNoConnection());
        }


        /*if (onlineRankingManager.getInstance().isConnectedToInternet())
        {
            
        }
        else
        {
            Debug.Log("openNoConnection() : btnOpenMenuRanking()");
            StartCoroutine(openNoConnection());
        }*/
    }
    public void btnCloseRankingInfinis()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        rank.animCloseMenu();
    }

    private bool playerValuesChanged_sinceLastUp = false;
    private int idReqForced = -10; // quand on veut lancer aRequestEnded() sans avoir lancé de request
    public void aRequestEnded(int idReq, bool succed, object result)
    {
        if (listRequestWaitingForEnd.ContainsKey(idReq))
        {
            typeRequestBdd typeReq = listRequestWaitingForEnd[idReq];
            debug_requestEnded(typeReq.ToString(), Time.time - initTimeWait);
            Debug.Log("<color=blue>- Remove idRequest : " + idReq + " (type:" + typeReq + ")</color>");
            listRequestWaitingForEnd.Remove(idReq);

            // si la requete a fonctionné
            if (succed)
            {            
                // en fonction du type de la requête, on effectue la suite du processus de connection
                switch (typeReq)
                {
                    case typeRequestBdd.openMenu_checkDate:
                        idReq = getNextIdRequestBdd();
                        StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_checkMaintenance));

                        currentIdMaintenanceChecked = 0; // 0: init, 1: afterAccount
                        StartCoroutine(onlineM.checkMaintenanceIsOn_openMenu(idReq, false));
                        break;
                    case typeRequestBdd.openMenu_checkMaintenance:
                        if ((bool)result)
                        {
                            shouldReloadTimeNextWeek = true;
                            maintenanceM.openMaintenanceMenu();
                            closeInitLoadAnim();
                        }
                        else
                        {
                            idReq = getNextIdRequestBdd();
                            if (accountValidated) // a déjà 'check account' plus tôt dans la session, et l'avait validé
                            {
                                // sauter phase check account
                                // atterit phase update playerData
                                StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_updatePlayerData));
                                StartCoroutine(onlineM.updatePlayerData_openMenu(idReq));
                            }
                            else
                            {
                                listRequestWaitingForEnd.Add(idReq, typeRequestBdd.openMenu_changeAccount);
                                onlineM.checkIfReplacePlayerId(idReq);                                
                            }
                        }
                        break;
                    case typeRequestBdd.openMenu_changeAccount:
                        // result : bool. True -> a du changer de compte
                        bool hasChangeAccount = (bool)result;
                        if (hasChangeAccount)
                        {
                            // sauter phase checkAccount
                            // sauter phase updatePlayerData
                            // direct load les ranks
                            accountValidated = true;
                            shouldReloadTimeNextWeek = true;
                            listRequestWaitingForEnd.Add(idReqForced, typeRequestBdd.openMenu_updatePlayerData);
                            aRequestEnded(idReqForced, true, true);
                            dataInCloudManager.playerPassedAccountPhase(true);
                        }
                        // on fait process normal de check compte
                        else
                        {
                            idReq = getNextIdRequestBdd();
                            // check account
                            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_checkAccount));
                            StartCoroutine(onlineM.checkLocalPlayerHasAnAccount(idReq));
                        }                        
                        break;
                    case typeRequestBdd.openMenu_checkAccount:
                        bool hasAnAccount = (bool)result;
                        if (!hasAnAccount)
                        {
                            shouldReloadTimeNextWeek = true;
                            menuCreateAcc.initMenu();
                            closeInitLoadAnim();
                        }
                        else
                        {
                            accountValidated = true;
                            dataInCloudManager.playerPassedAccountPhase(false);
                            // update player data
                            idReq = getNextIdRequestBdd();
                            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_updatePlayerData));
                            StartCoroutine(onlineM.updatePlayerData_openMenu(idReq));
                        }
                        break;
                    case typeRequestBdd.createAccount_recheckMaintenance:
                        // appelé après create account, quand menu est close (-> que appelé si createAcc a été ouvert
                        // si maintenance, ouvrir
                        if ((bool)result)
                        {
                            shouldReloadTimeNextWeek = true;
                            maintenanceM.openMaintenanceMenu();
                            closeInitLoadAnim();
                        }
                        // sinon update playerData ( si il n'y avait pas d'info dans bdd pour player, va les créer )
                        else
                        {
                            // update player data
                            idReq = getNextIdRequestBdd();
                            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_updatePlayerData));
                            StartCoroutine(onlineM.updatePlayerData_openMenu(idReq));
                        }
                        break;
                    case typeRequestBdd.openMenu_updatePlayerData:
                        // on pourrait aussi check si une autre valeure a changé
                        // mais seulement si sa place dans le rank change nous interesse
                        playerValuesChanged_sinceLastUp = (bool)result;
                        
                        // on doit re-check date next week, SI pas direct open->ici
                        // shouldReloadTimeNextWeek mit à true quand : passe par menu maintenance / createAcc / change compte
                        if (shouldReloadTimeNextWeek)
                        {
                            idReq = getNextIdRequestBdd();
                            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_recheckDate));
                            StartCoroutine(onlineM.testLoadTimeNextWeek_openMenu(idReq));
                        }
                        else
                        {
                            bool timeOut = prevTimeRefreshDataBdd == -1 || Time.time - prevTimeRefreshDataBdd >= timeRefreshBdd;
                            // suite : request load ranks (fait la même que : case:typeRequestBdd.openMenu_recheckDate
                            idReq = getNextIdRequestBdd();
                            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_getRankValues));
                            onlineM.loadAllDatas_dateIsOk(playerValuesChanged_sinceLastUp || timeOut, idReq, prevTimeRefreshDataBdd == -1);
                        }
                        break;
                    case typeRequestBdd.openMenu_recheckDate:
                        // atterit ici, recheckDate succed ( appel de case typeRequestBdd.openMenu_updatePlayerData SI shouldReloadTimeNextWeek = true
                        // on essaie de réccupérer les ranks de bdd
                        idReq = getNextIdRequestBdd();
                        bool timeOut_2 = prevTimeRefreshDataBdd == -1 || Time.time - prevTimeRefreshDataBdd >= timeRefreshBdd;
                        StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_getRankValues));
                        onlineM.loadAllDatas_dateIsOk(playerValuesChanged_sinceLastUp || timeOut_2, idReq, prevTimeRefreshDataBdd == -1);
                        break;
                    case typeRequestBdd.openMenu_getRankValues:
                        bool weekOrRecordRequested = (bool)result;
                        if (weekOrRecordRequested)
                            prevTimeRefreshDataBdd = Time.time;
                        // réussit : lancer anim loadRanks
                        onlineM.allDatasFinishLoad();
                        closeInitLoadAnim();
                        isInConnectionProcess = false;
                        break;

                    // ----------------------------------------------------------------------------parallel
                    case typeRequestBdd.checkStateMaintenance_menuMaint:
                        // ici on a juste appelé l'update de maintenance ET reçu res, on n'en FAIT RIEN si succed
                        // ( maintenanceMenu s'update, et regarde si maintenance est ON de lui même, en lisant var de onlineRankingManager )
                        break;
                    case typeRequestBdd.createAccount_tryName:
                        // si le nom est valide (pas encore utilisé)
                        bool nameFree = (bool)result;
                        menuCreateAcc.waitCreateAccount_resultNameUsed(!nameFree);
                        // si nameFree, menuCreateAcc va rappeler finishedToCheckNameAccount() à la fin de sa fermeture
                        // sinon attend nouvelle proposition de nom
                        break;
                    case typeRequestBdd.createAccount_tryCreateAcc:
                        dataInCloudManager.playerPassedAccountPhase(false);
                        accountValidated = true;
                        // creation reussit si appelle ici
                        // doit re-check si maintenance | pas besoin de créer une autre fonction de checkMaintenance
                        idReq = getNextIdRequestBdd();                        
                        currentIdMaintenanceChecked = 1; // 0: init, 1: afterAccount
                        StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.createAccount_recheckMaintenance));
                        StartCoroutine(onlineM.checkMaintenanceIsOn_openMenu(idReq, true));
                        break;
                }
            }
            // sinon, fermer le menu
            else
            {
                Debug.Log("Request response = error , typeErreur : " + listRequestWaitingForEnd[idReq]);
                forceCloseMenu_noAnswer(listRequestWaitingForEnd[idReq]);
            }
        }
        // SINON, le menu s'est déjà désactivé de lui même, est déjà passé dans le if{}
        // ne rien faire de plus
    }

    // requêtes non automatiques
    public void loadParallelRequest(typeRequestBdd type, object data)
    {
        int idReq;
        switch (type)
        {
            case typeRequestBdd.checkStateMaintenance_menuMaint:
                idReq = getNextIdRequestBdd();
                StartCoroutine(waitForRequestToEnd(idReq, type));

                // _openMenu fait ce qu'on attendrait de _menuMaintenance ( donc 1 seule fonction pour les 2 )
                StartCoroutine(onlineM.checkMaintenanceIsOn_openMenu(idReq, true));
                break;
            case typeRequestBdd.createAccount_tryName:
                idReq = getNextIdRequestBdd();
                StartCoroutine(waitForRequestToEnd(idReq, type));
                currentNameTested = (string)data;

                // on demande à onlineRankingManager de tester un nom
                StartCoroutine(onlineM.testNameIsFreeToUse(idReq, currentNameTested));
                break;
            case typeRequestBdd.createAccount_tryCreateAcc:
                        
                Debug.Log("(1) Create Account");
                idReq = getNextIdRequestBdd();
                StartCoroutine(waitForRequestToEnd(idReq, type));
                Debug.Log("(2) Create Account");
                // on demande à onlineRankingManager de créer un compte
                onlineM.createNewAccountWithName(idReq, currentNameTested);
                break;
        }
    }

    private static int idRequestBDD = 0;
    private int getNextIdRequestBdd()
    {
        idRequestBDD++;
        return idRequestBDD;
    }
    #endregion

    /*
     * void maintenanceEnded()
     * void finishedToCheckNameAccount()
    */
    // appelé par autres menus
    #region preprise du process de connexion
    // appelé quand la maintenance est finit
    // par menuMaintenance, quand menuMaintenance est open
    public void maintenanceEnded()
    {
        // on doit regarde si
        // - fin de maintenance avant checkAccount -> check account exists
        // - fin de maintenance après checkAccount -> update player dans bdd
        bool maintenanceAfterCreateAccount = currentIdMaintenanceChecked == 1;      
        int idReq = getNextIdRequestBdd();

        // reprendre le process de connection au rank
        if (accountValidated
            || maintenanceAfterCreateAccount)
        {
            // update les valeures de player dans la base
            // ( case typeRequestBdd.createAccount_recheckMaintenance: )
            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_updatePlayerData));
            StartCoroutine(onlineM.updatePlayerData_openMenu(idReq));
        }
        else
        {
            // regarder si player a un compte
            // ( case typeRequestBdd.openMenu_checkMaintenance: )  
            StartCoroutine(waitForRequestToEnd(idReq, typeRequestBdd.openMenu_checkAccount));
            // checkLocalPlayerHasAnAccount ne fait pas de requête si sait déjà si un compte est dans bdd
            StartCoroutine(onlineM.checkLocalPlayerHasAnAccount(idReq));
        }
        openInitLoadAnim();
    }

    // appelé par le menuCreateAccount quand finit processus de création de compte, et que s'est bien fermé
    public void finishedToCheckNameAccount()
    {
        // menuCreateAcc s'est fermé tt seul avant d'appeler ici
        // on lance request creation de compte
        openInitLoadAnim();// ré-ouvrir anim wait ici
        // ? se trouve ici dans un query ?
        loadParallelRequest(typeRequestBdd.createAccount_tryCreateAcc, null);      
    }
    #endregion

    /*
     * bool resetTimeWaitRequest(int idReq) 
     * IEnumerator waitForRequestToEnd(int idReq, typeRequestBdd typeReq)
     * void forceCloseMenu_noAnswer(typeRequestBdd typeReq)
     * void playerDisconnecterFromInternet(typeDisconnectInternet type)
     * IEnumerator openNoConnection()
    */
    #region gestion des erreurs

    // temps d'attente permit pour une requête
    // si on atteind le temps, alors on ferme automatiquement le menu
    public float timeWaitRequest_toError = 3f;
    private float initTimeWait;
    private Dictionary<int, typeRequestBdd> listRequestWaitingForEnd = new Dictionary<int, typeRequestBdd>();

    // renvoit 'true' si la request est toujours en recherche
    public bool resetTimeWaitRequest(int idReq) 
    {
        initTimeWait = Time.time;
        return listRequestWaitingForEnd.ContainsKey(idReq);
    }
    private IEnumerator waitForRequestToEnd(int idReq, typeRequestBdd typeReq)
    {
        debug_requestCalled(typeReq.ToString());

        listRequestWaitingForEnd.Add(idReq, typeReq);
        Debug.Log("<color=blue>+ Add idRequest : " + idReq + " (type:" + typeReq + ")</color>");
        initTimeWait = Time.time;

        while (Time.time - initTimeWait < timeWaitRequest_toError
            && listRequestWaitingForEnd.ContainsKey(idReq))
        {
            yield return null;
        }

        if (listRequestWaitingForEnd.ContainsKey(idReq))
        {
            debug_requestTimeOut(typeReq.ToString(), Time.time - initTimeWait);
            Debug.Log("Request response = timeOUT , typeErreur : " + listRequestWaitingForEnd[idReq]);
            // temps dépassé, on force la fermeture du menu
            forceCloseMenu_noAnswer(typeReq);
            listRequestWaitingForEnd.Remove(idReq);
            Debug.Log("<color=blue>- Remove idRequest : " + idReq + " (type:" + typeReq + ")</color>");
        }
    }

 

    // appelé si aucune reponse a une requête, OU alors si la requête n'a pas réussit ( 1 des 2, jamais 2 en mm temps
    private void forceCloseMenu_noAnswer(typeRequestBdd typeReq)
    {
        isInConnectionProcess = false;

        // si erreur pas dans menu parallel ( menuMaintenance / createAccount
        // -> juste fermer loadAnim
        if (listRequestError_closeSimple.Contains(typeReq))
        {
            // quand reponse request de la liste closeSimple, animLoad est TJR déjà ouverte
            closeInitLoadAnim();
        }
        // si erreur checkStateMaintenance_menuMaint
        else if(typeReq == typeRequestBdd.checkStateMaintenance_menuMaint)
        {
            // fermer menu maintenance
            maintenanceM.errorWhileRecheckMaintenanceON();
        }
        // si erreur createAccount_tryName OU createAccount_tryCreateAcc
        else if(typeReq == typeRequestBdd.createAccount_tryName)
            //|| typeReq == typeRequestBdd.createAccount_tryCreateAcc)
        {
            // fermer menuCreateAcc
            StartCoroutine(menuCreateAcc.errorWhileTryingName());
        }

        Debug.Log("openNoConnection() : forceCloseMenu_noAnswer() , typeError : "+typeReq);
        StartCoroutine(openNoConnection());
    }

    // quelle 'request' peut être en court pendant ce type de deconnexion
    // exemple, menuMaintenance -> request : checkStateMaintenance (possible)
    private Dictionary<typeDisconnectInternet, typeRequestBdd> possibleRequestON_whileDisconnected
        = new Dictionary<typeDisconnectInternet, typeRequestBdd>()
        {
            {typeDisconnectInternet.checkingMaintenance, typeRequestBdd.checkStateMaintenance_menuMaint},
        };

    public void playerDisconnecterFromInternet(typeDisconnectInternet type)
    {
        // On check si on coupe une 'request' en court
        // ferme le menu en simulant une erreur de requête
        if (possibleRequestON_whileDisconnected.ContainsKey(type)
            && listRequestWaitingForEnd.ContainsValue(possibleRequestON_whileDisconnected[type]))
        {
            int idReq = 0; // trouve toujours quelque chose
            foreach (KeyValuePair<int, typeRequestBdd> k in listRequestWaitingForEnd)
            {
                if (k.Value == possibleRequestON_whileDisconnected[type])
                {
                    idReq = k.Key;
                }
            }
            aRequestEnded(idReq, false, null); // simule erreur -> ferme le menu       
        }
        // Si on ne coupe pas de requête
        // On ferme le menu qui est ouvert
        else
        {
            switch (type)
            {
                case typeDisconnectInternet.checkingMaintenance:
                    // appelé par maintenanceMenu, quand check internet
                    // -> ferme de lui même le menu maintenance                   
                    break;
                case typeDisconnectInternet.inRankMenu:
                    // rankingInfinis, quand essaie de changer de rank, si pas de connexion, se ferme
                    rank.animCloseMenu();
                    break;
            }
        }
        Debug.Log("openNoConnection() : playerDisconnecterFromInternet(");
        StartCoroutine(openNoConnection());
    }

    public static bool menuOpen = false;
    public Animation noConnecAnim;
    private IEnumerator openNoConnection()
    {
        noConnecAnim.Play("noCoRankInfinis");
        yield return new WaitForSeconds(1f);
        menuOpen = false;
    }
    #endregion


    // gestion timeOut, depuis menu rank, switch vers maintenance
    public void rankTimeOut_GoMaintenance()
    {
        isInConnectionProcess = true;
        StartCoroutine(reopenMaintenance_rankTimeOut());
    }

    private IEnumerator reopenMaintenance_rankTimeOut()
    {
        while (rank.animSwitchMenu.isPlaying) // attendre fin anim switchRank si était en court
            yield return null;
        //onlineRankingManager.getInstance().timerEnds_setMaintenance(); // revient a faire mm chose qu'en dessous
        onlineRankingManager.maintenanceOn = true;
        rank.animCloseMenu();

        // re-rentrer process à partir de "case typeRequestBdd.openMenu_updatePlayerData:

        yield return new WaitForSeconds(15 / 60f); // time animCloseRank = 20/60
        // quand maintenance se finira, appellera managerRankV2.maintenanceEnded();
        // managerRankV2.maintenanceEnded() lance suite process en fonction currentIdMaintenanceChecked
        // si currentIdMaintenanceChecked == 1 -> updatePlayerData, SINON test account
        currentIdMaintenanceChecked = 1;
        shouldReloadTimeNextWeek = true; // va changer après maintenance
        maintenanceM.openMaintenanceMenu();
    }



    public void playerForceCloseMenuMaintenance()
    {
        // si player dans menu, a reussit check maintenanceON
        // cet appel ne peut que couper la requête re-check : "typeRequestBdd.checkStateMaintenance_menuMaint"
        if (listRequestWaitingForEnd.ContainsValue(typeRequestBdd.checkStateMaintenance_menuMaint)) {
            int idReq = 0; // trouve toujours quelque chose
            foreach (KeyValuePair<int, typeRequestBdd> k in listRequestWaitingForEnd)
            {
                if (k.Value == typeRequestBdd.checkStateMaintenance_menuMaint)
                {
                    idReq = k.Key;
                }
            }
            Debug.Log("<color=blue>- Remove idRequest : " + idReq + " (type:" + listRequestWaitingForEnd[idReq] + ")</color>");
            // stopper la requête, ne rien fermer de + ici, se fait seul par menuMaintenance
            listRequestWaitingForEnd.Remove(idReq);
        }

        isInConnectionProcess = false;
    }

    public void playerForceCloseMenuCreateAccount()
    {
        // n'est pas appelé pendant requête (requête bloque le menu createAccount en mode wait)
        isInConnectionProcess = false;
    }
}
