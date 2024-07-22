using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class devMenuPlayers : MonoBehaviour
{
    public developerBddManager devBddManager;
    public GameObject menuGo;
    public scrollBot scrollBotM;
    public scrollPlayersDev scrollPlayerM;
    public void btnOpenMenuPlayers()
    {
        menuGo.SetActive(true);
        scrollBotM.loadBots();
        scrollPlayerM.loadRanks(false);
    }

    public void btnCloseMenuPlayers()
    {
        menuGo.SetActive(false);
    }

    public void btnUpdateBotInBdd()
    {
        // on reccupère toute les infos des bots
        //List<infoPlayerTest> listInfo = scrollBotM.getListInfoBots();
        StartCoroutine(devBddManager.updateBotInBdd(scrollBotM.getListInfoBots_waitingUpdate(), scrollBotM.getListBotRemoved()));

        // fonction bdd : 
        // pour chaque info, check si player est dans bdd
        // oui, met à jour les valeures
        // non, créer un nouveau joueur
    }


    public Animation animSwitchMenu;
    private bool menuBotOn = true;
    public void btnBots()
    {
        if (!menuBotOn)
        {
            animSwitchMenu.Play("rankDevToBotDev");
            menuBotOn = true;
        }
    }

    public void btnRank()
    {
        if (menuBotOn)
        {
            animSwitchMenu.Play("botDevToRankDev");
            menuBotOn = false;
        }
    }



}
