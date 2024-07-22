using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class maintenanceMenu : MonoBehaviour
{
    private bool menuOpen = false;
    public Animation animMenu;

    private bool phaseWait = false;
    private bool phaseSwitch = false;

    public void openMaintenanceMenu()
    {
        // appelé à ouverture de menu si la base est en maintenance
        animMenu.Play("openMaintenance");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.maintenance_open);
        phaseWait = true;
        phaseSwitch = false;
        menuOpen = true;
        StartCoroutine(waitMaintenanceEnd());
    }

    public void btnCloseMaintenanceMenu()
    {
        if (phaseWait)
        {
            // appelé quand ferme volontairement le menu
            animMenu.Play("closeMaintenance");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.maintenance_close);
            phaseWait = false;
            managerRankV2.playerForceCloseMenuMaintenance();
        }
        else if(phaseSwitch)
        {
            // appelé quand ferme volontairement après maintenance finit
            animMenu.Play("closeMaintenanceToRanking");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.maintenance_close);
            phaseSwitch = false;
        }
        menuOpen = false;
    }


    public managerRankInfinisV2 managerRankV2;
    public float timeWaitCheckMaintenanceEnd = 3f;
    private float currentTimeLoop = 0f; // on reprend timer à cette valeure
    private IEnumerator waitMaintenanceEnd()
    {
        // tant que player attend dans menu, verifier si maintenance finit

        //float timeRef = Time.time;

        bool waitCheckInternet;
        bool resultInternet = false;
        yield return null;
        while (menuOpen && onlineRankingManager.maintenanceOn)
        {
            currentTimeLoop += Time.deltaTime;
            if (currentTimeLoop >= timeWaitCheckMaintenanceEnd)
            {
                currentTimeLoop = 0f;

                //onlineRankingManager.getInstance().refreshMaintenanceState(false);. REMPLACE par managerRankV2.loadParallelRequest
                waitCheckInternet = true;
                StartCoroutine(launcher.checkInternetConnection((isConnected) =>
                {
                    waitCheckInternet = false;
                    resultInternet = isConnected;
                }));
                while (waitCheckInternet)
                    yield return null;
                if (!resultInternet)
                {
                    animMenu.Play("closeMaintenance");
                    phaseWait = false;
                    menuOpen = false;
                    managerRankV2.playerDisconnecterFromInternet(typeDisconnectInternet.checkingMaintenance);
                }
                else
                    managerRankV2.loadParallelRequest(typeRequestBdd.checkStateMaintenance_menuMaint, null);

                /*Debug.Log("Tik wait maintenance end");
                if (!onlineRankingManager.getInstance().isConnectedToInternet())
                {
                    animMenu.Play("closeMaintenance");
                    phaseWait = false;
                    menuOpen = false;
                    managerRankV2.playerDisconnecterFromInternet(typeDisconnectInternet.checkingMaintenance);
                    //managerRank.playerDisconnectedFromInternet_fromMaintenance();. // remplacer
                }
                else
                {
                    managerRankV2.loadParallelRequest(typeRequestBdd.checkStateMaintenance_menuMaint, null);
                }*/
            }
            yield return null;
        }
        if (menuOpen && !onlineRankingManager.maintenanceOn)
        {
            // maintenance finit
            maintenanceEnd();
        }
    }



    public void maintenanceEnd()
    {
        phaseWait = false;
        phaseSwitch = true;
        StartCoroutine(animMaintenanceEnd());
    }

    public float timeValidToClose = 1f;
    //public managerRankInfinis managerRank;
    private IEnumerator animMaintenanceEnd()
    {
        // anim wait -> valid
        animMenu.Play("switchMaintenance");
        Debug.Log("Anim switch maintenance");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.maintenance_end);

        // attend x seconde
        yield return new WaitForSeconds(timeValidToClose);

        // si pas click close
        if (phaseSwitch)
        {
            animMenu.Play("closeMaintenanceToRanking");
            Debug.Log("Anim close maintenance");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.maintenance_close);
            phaseSwitch = false;
        }

        menuOpen = false;


        // si a deja check compte existe, passer cash a ouverture menu

        // doit appeler V2
        managerRankV2.maintenanceEnded();
        /* passe a la phase check account
        if (managerRankInfinis.waitingMaintenance_init)
            managerRank.playerPassedMaintenancePhase(true);
        // passe a la phase ouverture rankInfinis
        else if (managerRankInfinis.waitingMaintenance_afterAccount)
            managerRank.playerPassedMaintenancePhase_afterCreateAccount();*/

    }

    public void errorWhileRecheckMaintenanceON()
    {
        // appelé par managerV2, quand erreur dans requête update maintenanceState
        // fermer menu et reset
        onlineRankingManager.maintenanceOn = false;
        animMenu.Play("closeMaintenance");
        phaseWait = false;
        menuOpen = false;
    }
}
