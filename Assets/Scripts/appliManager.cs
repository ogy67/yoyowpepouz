using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class appliManager : MonoBehaviour
{
    public GameObject APPcanvas;
    public GameObject nivManager;

    public static appliManager instance;

    public ParticleSystem fondMenu;
    //public GameObject btnStart;

    //public lockInfinis lI_online;

    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        instance = gameObject.GetComponent<appliManager>();
        //lockInfinis.instance_lockOnline = lI_online;
    }

    public void lancerApp(bool fromTuto)
    {
        StartCoroutine(init(fromTuto));       
    }

    public bool _lockLevelWithCrownNumbers = false;
    IEnumerator init(bool fromTuto)
    {
        SaveSystem.loadApp_loadBestScores();

        if (_lockLevelWithCrownNumbers)
            SaveSystem.loadCrownToUnlock();

        if (fromTuto)
        {
            fondMenu.gameObject.SetActive(true);
            musicManager.getInstance().switchMusicType(typeMusic.menuP);
            //fondMenu.Clear();
        }
        else
        {
            musicManager.getInstance().initMusic_menuP();
        }

        /*if (tutorielManager.reDoTuto)
        {
            APPcanvas.SetActive(true);
            APPcanvas.GetComponent<Animation>().Play("lancerJeu");
            yield return new WaitForSeconds(1f);
            managerNiveau.instance.openFromInfiniteMode();
            tutorielManager.reDoTuto = false;
        }
        else
        {*/
        APPcanvas.SetActive(true);
        nivManager.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        safeArea.getInstance().appIsOpenned();
        mN.lauchMenu(true, fromTuto, false, false);

        menuVisuLevel.getInstance().initOpenMenuP();

        yield return new WaitForSeconds(0.3f);
        //Debug.Log("<color=red>INIT POS ui infos</color>");
        infoMenuManager.getInstance().initializePosOnAppOpen();
        //}
    }

    public managerNiveau mN;


    /*
     * void reactiveFondMenu()
     * void btnStartGame()
     * void quitGame()
    */
    #region Lancement partie , fin partie

    // réactiver les 'bulles' de fond
    public void reactiveFondMenu()
    {
        setEmissionRate(7);
    }

    private void setEmissionRate(float value)
    {
        var em = fondMenu.emission;
        var rate = new ParticleSystem.MinMaxCurve();
        rate.constantMax = value;
        em.rateOverTime = rate;
    }

    //public trailManager tM;
    public void btnStartGame()
    {
        // si dans menu infinis, vient de finir game, 
        // - bloquer btn start ( tte UI ) tant que menu pas full open
        // - bloquer btn start tant que pas finit donner recompense infinis

        /*if (INFINIS_ON // si est dans menuP->infinis , check ne revient pas de game
            && modeInfinisOn) // si est dans menuP->infinis , check ne revient pas de game
            return; // on annule le click start tant que n'est pas 100% revenus game infinis*/
        

        if (!managerNiveau.gameOn /*&& !managerNiveau.isDragging()*/ && gameManager.decompteCrown == 0)
        {
            if (notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating)
                return;

            setEmissionRate(0);
            bool inf = false;
            contentTrailGame.getInstance().new_game_started();
            //tM.new_game_started();

            gameManager.getInstance().btnStart();
            if (INFINIS_ON)
            {
                //Debug.Log("LANCE MODE INFINIS");
                inf = true;
                StartCoroutine(startInfinis());
            }
            cameraGame.changeColorGame(true, inf);
        }
    }

    public void btnStartOnlineGame()
    {
        setEmissionRate(0);
        contentTrailGame.getInstance().new_game_started();

        gameManager.getInstance().btnStartOnline();
        cameraGame.changeColorGame(true, true);
        StartCoroutine(loadOnlineUi());
    }

    private IEnumerator loadOnlineUi()
    {
        yield return new WaitForSeconds(0.5f);
        playAnim("lancerPartie");
    }

    public void quitOnlineGame()
    {
        // faire les anim de jeu -> menuP
        reactiveFondMenu();
        playAnim("fermerPartie");
        cameraGame.changeColorGame(false, true);
    }

    public void quitGame()
    {
        if ((gameManager.gameOn && !INFINIS_ON) || (infinisManager.game_On && INFINIS_ON))
        {
            /*if (OnlineManager.ONLINE_mode_on && !infinisManager.quitOnlineValidated)
            {
                infinisManager.getInstance().btnQuit_online();
            }
            else
            {*/
                gameManager.getInstance().btnQuit();
                if (INFINIS_ON)
                {
                    quitInfiniteMode(false);
                }
            //}
        }
    }

    /*public void quitGame_online()
    {
        quitInfiniteMode(false, true);
        StartCoroutine(gameManager.getInstance().closeGame(false, false, false, false));
        gameManager.getInstance().player.GetComponent<playerMovment>().stop();

    }*/

    #endregion


    public Animation animBtnInfinis;
    public GameObject btnRoadMap_menuPause;
    private bool switching = false;
    public static bool INFINIS_ON = false;

    /*
     * void btnModeInfinis()
     * IEnumerator switchToInfinite()
     * IEnumerator switchToNormal()
     * IEnumerator closeInfinisForTuto()
    */
    #region Changement de mode de jeu

    public menuGameOver menuGameO;
    public GameObject fondScoreIngame_aventure, fondScoreIngame_infinis;
    public void btnModeInfinis()
    {
        if (!managerNiveau.gameOn // donne encore des recompenses
            && !managerNiveau.goingBound
            && !switching)
            //&& !modeInfinisOn)
        {
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_switchInfinis);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_shopDefiInfinis);
            switching = true;
            if (!INFINIS_ON)
                StartCoroutine(switchToInfinite());
            else
                StartCoroutine(switchToNormal());
            INFINIS_ON = !INFINIS_ON;
            btnRoadMap_menuPause.gameObject.SetActive(INFINIS_ON);
            menuGameO.typeGameSwitched();
        }
    }
    IEnumerator switchToInfinite()
    {
        fondScoreIngame_aventure.SetActive(false);
        fondScoreIngame_infinis.SetActive(true);
        playerMovment.comportement = typeBehaviour.infinis;
        cameraGame.getInstance().switchColor(true);
        updateRecord(false);
        animBtnInfinis.Play("btnToInfinis");
        APPcanvas.GetComponent<Animation>().Play("openInfiniteMode");
        managerNiveau.getInstance().closeForInfiniteMode();
        yield return new WaitForSeconds(1 / 3f);
        sfxGame.getInstance().playSfx_listOther(typeSfx_other.click_switchInfinis_start);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);
        MODE_INFINIS.SetActive(true);
        MODE_INFINIS.GetComponent<infinisManager>().openMenu();
        yield return new WaitForSeconds(0.15f);
        //btnStart.SetActive(true);
        //Debug.Log("Open tuto infinis");
        //infoMenuManager.playerHasDoneTuto(typeOpenInfo.tuto_infinis, -1);

        //yield return new WaitForSeconds(0.2f);
        switching = false;
    }
    IEnumerator switchToNormal()
    {
        fondScoreIngame_aventure.SetActive(true);
        fondScoreIngame_infinis.SetActive(false);
        playerMovment.comportement = typeBehaviour.aventure;
        cameraGame.getInstance().switchColor(false);
        //btnStart.SetActive(false);
        animBtnInfinis.Play("btnToAventure");
        APPcanvas.GetComponent<Animation>().Play("exitInfiniteMode");
        MODE_INFINIS.GetComponent<infinisManager>().closeMenu();
        yield return new WaitForSeconds(1 / 3f);
        sfxGame.getInstance().playSfx_listOther(typeSfx_other.click_switchInfinis_start);
        //Debug.Log("<color=red>SOUND SHOULD NOT BE CALLED</color>");
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);
        MODE_INFINIS.SetActive(false);
        managerNiveau.getInstance().openFromInfiniteMode();

        yield return new WaitForSeconds(0.15f);
        switching = false;
    }

    #endregion

    private Animation myAnim;
    public void playAnim(string anim)
    {
        if (myAnim == null)
            myAnim = GetComponent<Animation>();
        myAnim.Play(anim);
    }

    /*
     * IEnumerator startInfinis()
     *  void startInfiniteMode()
     * void quitInfiniteMode(bool playerDied) 
     *  IEnumerator closeInf(bool pDied)
     * void updateRecord(bool save)
    */
    #region MODE INFINIS

    public GameObject MODE_INFINIS;
    public Text txt_record;
    // n'est que utilisé pour bloquer UI quand retour de game infinis -> menuP
    //public static bool modeInfinisOn = false; // distinguer phase debut jeu -> fin recompense // fin re-ouverture menu infinis 

    IEnumerator startInfinis()
    {
        bool changeFirstLevel = infinisManager.getInstance().checkBeforeInit();
        //Debug.Log("Change first level ?" + !changeFirstLevel);
        if (!changeFirstLevel)
            yield return new WaitForSeconds(4f / 6f);
        //gameManager.getInstance().btnStart();
        startInfiniteMode();
    }
    public void startInfiniteMode()
    {

        playAnim("lancerPartie");
        gameManager.getInstance().safeA.animVoyantHautGameStart();
        //APPcanvas.GetComponent<Animation>().Play("loadInfiniteMode");

        //GetComponent<Animation>().Play("fondOn");
        infinisManager.getInstance().startInfinis();
        //modeInfinisOn = true;
    }



    public void quitInfiniteMode(bool playerDied) // sortie du mode jeu
    {
        StartCoroutine(closeInf(playerDied));
    }

    public GameObject lockUIendInfinisGo;
    public GameObject recompenseInfinis;
    public Transform compteurInfinis;
    public static List<GameObject> itemInstantiated = new List<GameObject>();

    public static void itemInfinisDestroyed(GameObject o)
    {
        itemInstantiated.Remove(o);
    }

    public void infinisBestIncremented()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.crown_enterBar);
        SaveSystem.record_infinis++;
        // /!\ lancer anim score upgrade
        if (txt_record.GetComponent<Animation>().isPlaying)
        {
            updateRecord(false);
            txt_record.GetComponent<Animation>().Stop();
        }
        txt_record.GetComponent<Animation>().Play("newRecordInfinis");
        StartCoroutine(updateTextLate());
    }

    private static int idItem = 0;
    public float timeBeforePopInfinisScore = 1.4f;
    public float timeBeforeBtnStartRenabled = 1f;
    private IEnumerator updateTextLate()
    {
        idItem++;
        int idCopy = idItem;
        yield return new WaitForSeconds(8 / 60f);
        if(idCopy == idItem)
            updateRecord(false);
    }

    public notifDefiDone notifAnim;
    IEnumerator closeInf(bool pDied)
    {
        lockUIendInfinisGo.SetActive(true);
        if (pDied)
            yield return new WaitForSeconds(1f);

        /* REMPLACER
        StartCoroutine(gameManager.getInstance().closeGame(true, false, false));
        */
        //if (onlineMode)
        //    StartCoroutine(infinisManager.getInstance().endInfinis_online());

        yield return new WaitForSeconds(0.25f);
        playAnim("fermerPartie");
        gameManager.getInstance().safeA.animVoyantHautGameEnd();
        //APPcanvas.GetComponent<Animation>().Play("closeInfiniteMode");

        yield return new WaitForSeconds(0.15f);
        //GetComponent<Animation>().Play("fondOff");
        /*if (cameraGame.bonusOn)
            cameraGame.getInstance().fromBonusToInfiniteMenu();
        else*/
        cameraGame.changeColorGame(false, true);
        yield return new WaitForSeconds(0.15f);

        infinisManager.getInstance().endInfinis();
        //StartCoroutine(infinisManager.getInstance().endInfinis());

        int s = infinisManager.SCORE;
        List<int> idBoostUsed = boostManager.getInstance().getBoostsUsedId();
        List<int> levelBoostUsed = boostManager.getInstance().getBoostUsedLevel();
        string idStr = idBoostUsed[0] + "," + idBoostUsed[1] + "," + idBoostUsed[2];
        string levelStr = levelBoostUsed[0] + "," + levelBoostUsed[1] + "," + levelBoostUsed[2];

        onlineRankingManager.getInstance().playerScoreInfinis(s, DateTime.Now.ToString(), idStr, levelStr);

        //offreSpeciale.updateNbLevelsDone(false, s);

        //Vector2 compteurPos = Camera.main.WorldToScreenPoint(compteurInfinis.position);
        // on pop recompense infinis
        if (s > SaveSystem.record_infinis)
        {
            yield return new WaitForSeconds(timeBeforePopInfinisScore);
            int nbRewardToPop = s - SaveSystem.record_infinis;
            for(int i = 0; i < nbRewardToPop; i++)
            {
                GameObject o = Instantiate(recompenseInfinis);
                o.GetComponent<recompenseLvl>().spawn(
                    new Vector2(0 - UnityEngine.Random.Range(-1f, 1f), 0 - UnityEngine.Random.Range(-1f, 1f))
                    , compteurInfinis.position);
                o.SetActive(true);
                itemInstantiated.Add(o);
            }

            // on attend que tout les items aient finis
            yield return new WaitUntil(() => itemInstantiated.Count == 0);
            SaveSystem.record_infinis = s;
            updateRecord(true);
            //launcher.getInstance().updateAchievment(typeAchievment.infinite, s);

            /*yield return new WaitForSeconds(1.8f);
            txt_record.GetComponent<Animation>().Play("newRecordInfinis");
            yield return new WaitForSeconds(20f / 60f);*/
            
            //launcher.getInstance().updateLeaderRecordInfinis();
        }
        else
        {
            yield return new WaitForSeconds(timeBeforeBtnStartRenabled);
        }
        //modeInfinisOn = false;
        lockUIendInfinisGo.SetActive(false);

        notifAnim.showAllNotif();
        notifInfinisUnlocked.getInstance().endPhaseGame();

        //offreSpeciale.getInstance().checkIfProposeSpecialOffer();
    }

    public void updateRecord(bool save)
    {
        int r = SaveSystem.record_infinis;
        //lockInfinis.getInstance_online().checkVerouillage(r, false);

        int r2 = r;
        int nbDigit = 0;
        while (r2 / 10 >= 1)
        {
            r2 /= 10;
            nbDigit++;
        }
        int nbZero = 2 - nbDigit; // taille de 4 digit, les zero remplissent les 'trous' : 0042

        string digit = "";
        //int nbZero = r < 10 ? 2 : r < 100 ? 1 : 0;
        if(nbZero > 0)
        {
            digit += "<color=grey><size=26>";
            while (nbZero > 0)
            {
                digit += '0';
                nbZero--;
            };
            digit += "</size></color>";
        }
        digit += "<color=white>" + r + "</color>";

        txt_record.text = digit; // Translator.getText(keyText.MENU_I_Record)+ ": " + ;
        if(save)
            SaveSystem.Save(typeSave.RECORD_INFINIS, r);
    }

    #endregion

    public static string intTo4DigitString(int value, int nbDigit)
    {
        int nbDValue = 0; // 11 -> /10 = 1 -> /10 = 0 ( 2 digit, incremente 1 seule foi )
        int v2 = value;

        do
        {
            nbDValue++;
            v2 /= 10;
        } while (v2 > 0);

        int nbZero = nbDigit - nbDValue;

        string retour = "";
        if (nbZero > 0)
        {
            retour += "<color=\"#6D604A\">";
            while (nbZero > 0)
            {
                retour += '0';
                nbZero--;
            }
            retour += "</color>";
        }
        retour += value.ToString();

        return retour;
    }

}
