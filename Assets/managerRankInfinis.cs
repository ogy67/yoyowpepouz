using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managerRankInfinis : MonoBehaviour
{
    /*public static bool menuOpen = false;

    private static managerRankInfinis instance;
    public static managerRankInfinis getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("rankingInfinisCanvas").GetComponent<managerRankInfinis>();
        return instance;
    }

    public createAccountMenu menuCreateAcc;
    public Animation initLockAnim;
    private bool initLockAnimOn = false;

    public rankingInfinis rank;
    public maintenanceMenu maintenanceM;*/


    /* depuis ranking
    public void playerDisconnectedForMaintenance()
    {
        StartCoroutine(switchRankToMaintenance());
    }

    private IEnumerator switchRankToMaintenance()
    {
        while (rank.animSwitchMenu.isPlaying)
            yield return null;
        onlineRankingManager.getInstance().timerEnds_setMaintenance();
        maintenanceM.openMaintenanceMenu();
        rank.animCloseMenu();
    }*/





    /* ATTEND RESULTAT : met à jour la base avec infos locales
    public void weekChanged() // appelé par onlineRankingManager
    {
        prevTimeRefreshDataBdd = Time.time;
    }*/

    /*public void loadBarOpen()
    {
        if (initLockAnimOn)
        {
            initLockAnimOn = false;
            initLockAnim.Play("initLockRankClose");
        }
    }*/





    /* appelé par onLineRankingManager
    public void errorWhileInitQuery()
    {
        closeInitLoadAnim(true);
        menuOpen = false;
    }
    public void closeInitLoadAnim(bool errorQuery)
    {
        if (initLockAnimOn)
        {
            initLockAnim.Play("initLockRankClose");
            initLockAnimOn = false;
            if (errorQuery)
            {
                // on affiche no co anim
                StartCoroutine(openNoConnection());
            }
        }
    }*/

    //public bool testModeOn = true;
    //public bool test_ConnecOn = true;

    //private bool playerHasAnAccount = false;

    /*public void btnOpenRankingInfinis()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);

        maintenanceHasBeenOpenned = false;
        waitingMaintenance_afterAccount = false;
        waitingMaintenance_init = false;

        infoMenuManager.getInstance().btnInfo_rank();

        if (onlineRankingManager.getInstance().isConnectedToInternet())
        {
            initLockAnimOn = true;
            initLockAnim.Play("initLockRankOpen");
            // va en même temps load les infos de local player de la base
            StartCoroutine(btnOpenRankingInfinis_waitDate());

            //onlineRankingManager.getInstance().fsTestLocalPlayerHasAnAccount(false);
            /*if (!playerHasAnAccount)
                onlineRankingManager.getInstance().fsTestLocalPlayerHasAnAccount();
            else
                playerPassedCheckAccountExistPhase(true);

        }
        else
        {
            StartCoroutine(openNoConnection());
        }
    }*/

    /*public IEnumerator btnOpenRankingInfinis_waitDate()
    {
        maintenanceHasBeenOpenned = false;
        waitingMaintenance_init = true;
        onlineRankingManager.timeNextWeek = -1;
        onlineRankingManager.getInstance().testLoadTimeNextWeek(true);
        while (onlineRankingManager.timeNextWeek == -1 && initLockAnim) 
            yield return null;
        // si pas d'erreur continuer
        if (initLockAnim)
            onlineRankingManager.getInstance().refreshMaintenanceState(true);
    }*/

    /*public void btnCloseRankingInfinis()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        rank.animCloseMenu();
    }*/


    // avant de créer compte, check si maintenance en court



    // ----- ------------------------------- ATTEND resultat


    //private bool maintenanceHasBeenOpenned;
    /*public void resultMaintenanceInitMenu(bool maintenanceOn)
    {
        if (maintenanceOn)
        {
            if (!maintenanceHasBeenOpenned)
            {
                maintenanceHasBeenOpenned = true;
                maintenanceM.openMaintenanceMenu();
                closeInitLoadAnim(false);
            }           
        }
        else
        {
            maintenanceHasBeenOpenned = false;
            playerPassedMaintenancePhase(false);
            waitingMaintenance_init = false;
        }
    }*/

    /* appelé quand maintenance finit / pas de maintenance
    public void playerPassedMaintenancePhase(bool fromMaintenance)
    {
        waitingMaintenance_init = false;
        if (fromMaintenance)
        {
            initLockAnimOn = true;
            initLockAnim.Play("initLockRankOpen");
        }

        // on regarde si player a un compte

        //Debug.Log("Update player datas in bdd, from maintenance : " + fromMaintenance);
        // on met à jour les données de player avec la base

        //Debug.Log("1");
        // false : veut dire que player ne vient pas de la maintenance
        // on peut venir de la maintenance, mais si met true 
        // -> update value player dans base ( ce n'est pas ici qu'on veut le faire, c'est après check account exist )
        //onlineRankingManager.getInstance().fsTestLocalPlayerHasAnAccount();


        //onlineRankingManager.getInstance().fsUpdateLocalPlayerValues_1(fromMaintenance);

            // on lance l'ouverture du ranking
            // rank.openMenuRankingInfinis();
    }*/

    /*private float timeRefreshBdd = 60 * 10; // 10 minutes
    private float prevTimeRefreshDataBdd = -1f;
    private bool shouldRefreshBdd_timeOut()
    {
        /*Debug.Log("Refresh time out ?");
        Debug.Log("prevTimeRefreshDataBdd : " + prevTimeRefreshDataBdd
            + ", Time.time : "+ Time.time 
            + ", prevTimeRefreshDataBdd : "+ prevTimeRefreshDataBdd
            + ", timeRefreshBdd : " + timeRefreshBdd);/
        if (prevTimeRefreshDataBdd == -1
            || Time.time - prevTimeRefreshDataBdd >= timeRefreshBdd)
        {
            return true;
        }
        return false;
    }*/





    /* --- ------------------------------- ATTEND RESULTAT

    //private static bool alreadyCheckedPlayerHasAccount = false;
    public void playerPassedCheckAccountExistPhase(bool alreadyHasAnAccount)
    {
        if (alreadyHasAnAccount)
        {
            //Debug.Log("3.1");
            //alreadyCheckedPlayerHasAccount = true;
            playerPassedCreateAccountPhase(false);
        }
        else
        {
            // ouvrir menu create account
            //Debug.Log("3.2");
            menuCreateAcc.initMenu();
            closeInitLoadAnim(false);
        }
    }*/


    // appelé par manager
    // appelé par menu createAccount quand finit
    /*public void playerPassedCreateAccountPhase(bool fromCreateAccount)
    {
        if (fromCreateAccount)
        {
            initLockAnimOn = true;
            initLockAnim.Play("initLockRankOpen");
            StartCoroutine(waitAccountCreated());
        }
        else
        {
            //Debug.Log("4");
            onlineRankingManager.getInstance().fsUpdateLocalPlayerValues_2();//true);// updateDataPlayer);
        }
    }*/

    /*public static bool waitingAccountCreated = false; // mit a true par online rankmanager après check name
    public static bool error_waitingAccountCreated = false;
    private IEnumerator waitAccountCreated()
    {
        //Debug.Log("Attend compte crée");
        while (waitingAccountCreated)
            yield return null;
        if (error_waitingAccountCreated)
        {
            //Debug.Log("(error) Fin attente compte crée");
            errorWhileInitQuery();
        }
        else
        {
            //Debug.Log("(pass) Fin attente compte crée");
            //alreadyCheckedPlayerHasAccount = true;
            //onlineRankingManager.accountAlreadyExists = true;
            StartCoroutine(waitCheckMaintenance_afterCreateAccount());
        }
    }*/

    /*public static bool waitingMaintenance_init = false;
    public static bool waitingMaintenance_afterAccount = false;
    // après création compte, on re-check si maintenance
    private IEnumerator waitCheckMaintenance_afterCreateAccount()
    {
        // Debug.Log("Check maintenance after create account, check time");
        maintenanceHasBeenOpenned = false;
        waitingMaintenance_afterAccount = true;
        
        onlineRankingManager.timeNextWeek = -1;
        onlineRankingManager.getInstance().testLoadTimeNextWeek(true);
        while (onlineRankingManager.timeNextWeek == -1 && initLockAnim)
            yield return null;
        // si pas d'erreur pendant set timer continuer
        //Debug.Log("Check no error, initLockAnim (on):" + initLockAnimOn);
        if (initLockAnimOn)
            onlineRankingManager.getInstance().refreshMaintenanceState(true);
    }*/

    /*public void resultMaintenanceAfterCreateAccount(bool maintenanceOn)
    {
        //Debug.Log("(3)");
        if (maintenanceOn)
        {
            if (!maintenanceHasBeenOpenned)
            {
                //Debug.Log("(3.1)");
                maintenanceHasBeenOpenned = true;
                maintenanceM.openMaintenanceMenu();
                closeInitLoadAnim(false);
            }
        }
        else
        {
            //Debug.Log("3.2");
            maintenanceHasBeenOpenned = false;
            waitingMaintenance_afterAccount = false;
            playerPassedMaintenancePhase_afterCreateAccount();            
        }
    }*/

    /*public void playerPassedMaintenancePhase_afterCreateAccount()
    {
        //initLockAnimOn = true;
        //initLockAnim.Play("initLockRankOpen");
        if (!initLockAnimOn)
        {
            initLockAnimOn = true;
            initLockAnim.Play("initLockRankOpen");
        }

        waitingMaintenance_afterAccount = false;

        //Debug.Log("UPDATE player values in bdd");
        onlineRankingManager.getInstance().fsUpdateLocalPlayerValues_2();//true);// updateDataPlayer);

        // on regarde si player a un compte

        //Debug.Log("Update player datas in bdd, from maintenance : " + fromMaintenance);
        // on met à jour les données de player avec la base


        //onlineRankingManager.getInstance().fsUpdateLocalPlayerValues_1(fromMaintenance);

        // on lance l'ouverture du ranking
        // rank.openMenuRankingInfinis();
    }*/







    // -------------------------------



    /*public void resultUpdatePlayerValuesInBdd( bool playerValuesChanged)
    {
        Debug.Log("5");
        //if (succedUpdating)
        //{
        bool reloadRank = playerValuesChanged || shouldRefreshBdd_timeOut();
        if (reloadRank)
            prevTimeRefreshDataBdd = Time.time;
        // on lance l'ouverture du ranking
        Debug.Log("Load rank");
        rank.openMenuRankingInfinis(reloadRank);

        // /!\ fermer seulement une fois datarank reccup
        //.
        //closeInitLoadAnim(false);

        /*}
        else
        {
            // sinon, on ouvre noCo
            openNoConnection();
            if (initLockAnimOn)
            {
                closeInitLoadAnim(true);
            }
        }*/
    //}


    /*public Animation noConnecAnim;
    private IEnumerator openNoConnection()
    {
        noConnecAnim.Play("noCoRankInfinis");
        yield return new WaitForSeconds(1f);
        menuOpen = false;
    }*/







    /* depuis maintenance
    public void playerDisconnectedFromInternet_fromMaintenance()
    {
        StartCoroutine(openNoConnection());
    }

    public void playerDisconnectedFromInternet_fromRankMenu()
    {
        StartCoroutine(openNoConnection());
        rank.animCloseMenu();
    }*/
}
