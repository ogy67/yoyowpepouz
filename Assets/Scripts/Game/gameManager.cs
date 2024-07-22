using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public enum typeEffectColor
{
    win,
    lose,
    bounce,
    pivot,
    portal
}

[Serializable]
public struct effectColorInfos
{
    public typeEffectColor _typeEffect;
    public Color _color;
}


public class gameManager : MonoBehaviour
{
    public GameObject progBarNiv;

    [Header("Objets")]
    
    public GameObject fondjeu;
    public GameObject animMenu;
    public GameObject crownGo;
    public GameObject monnaieTextGo;
    public Text monnaieText;
    public Text nbVieText;
    public GameObject menuChoixNiveau;
    public Transform player;
    public playerMovment pM;
    public Transform currentNiveau;


    public static int decompteCrown = 0; // rien faire tant que > 0
    private packNiveau currentPack;
    public static int VIE = 2;
    public static bool viePerdue;
    public static int nbVies;
    private int bestLevel;
    public static int monnaieVal;
    private int[] records;
    public static bool gameOn = false;
    public static int SCORE = 0;



    private static gameManager instance;
    public static gameManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("GameManager").GetComponent<gameManager>();
        return instance;
    }
    private void Start()
    { 
        monnaieVal = (int)SaveSystem.loadData(typeSave.MONNEY);
        updateMonneyText(monnaieVal);
    }

    public void tutoEnd_giveMonney(int nbYow)
    {
        monnaieVal = nbYow;
        updateMonneyText(monnaieVal);
        SaveSystem.Save(typeSave.MONNEY, monnaieVal);
    }

    public static KeyCode quitKey = KeyCode.Q;
    public static KeyCode hangKey = KeyCode.Space;
    public static KeyCode slowCheckPKey = KeyCode.W;

    // appelé par btnManette.cs
    public void btnBClicked()
    {
        // activer desactiver super visee
        if (!tortue_on)
        {
            tortue_on = true;
            StartCoroutine(tortue());
        }
        else
        {
            tortue_on = false;
        }
    }

    private void playerDied_checkBtnBOn()
    {
        // si btn b On, desactiver tortue de force
        if (tortue_on)
            tortue_on = false;
    }



    /*void Update()
    {
        if (TutoManager.tutoOn && playerMovment.finish)
            return;
        else if (!TutoManager.tutoOn && !gameOn && !infinisManager.game_On)
            return;

        if (Input.GetKeyDown(quitKey))
        {
            appliManager.instance.quitGame();
            return;
        }
    }*/

    public bool _singleClick()
    {
        bool phone = Application.platform == RuntimePlatform.Android;
        return !phone && !Input.GetKey(slowCheckPKey)
            || phone && Input.touchCount == 1;
    }

    //public Animation glowTortueAnim;
    private float tortue_decce_time = 0.3f;
    private float tortue_acce_time = 0.3f;
    private bool tortue_on = false;
    private static int tortueSpeed = 3;
    private static int initSpeedValue = 11;
    public static int speedValue = 11;
    /*
     * IEnumerator tortue(bool phone)
    */
    #region Contrôle tortue

    private IEnumerator tortue()
    {
        // une fois activé, attend soit reclique pour stopper
        // soit 0 input.touchCount -> release checkP
        float startSpeed = speedValue;
        float endSpeed = tortueSpeed;
        float diff_speed = startSpeed - endSpeed;
        float currentSpeed = speedValue;

        tortue_on = true;

        //glowTortueAnim.Play("glowBtnBOn");
        cameraGame.getInstance().activateAigle();

        float t = 0f;

        //Debug.Log("<color=red>Start tortue, player has released ? </color>" + playerMovment.hasReleased);
        
        // PB , si preshot , player pas hang au moment ou tortue appelé
        // pb que de pc ?

        // tant que hang CHECKP et n'a pas desactivé manuellement la tortue
        while (!playerMovment.hasReleased && tortue_on)
        {
            yield return null;
            t += Time.deltaTime;

            if(t < tortue_decce_time)
            {
                // deccelerer
                currentSpeed = startSpeed - diff_speed * (t / tortue_decce_time);
                playerMovment.changeSpeedRotation(
                    (currentSpeed  /* + 1 augmente la vitesse de base*/ ) / 10f);
            }
            else
            {
                // vitesse constante
                if(currentSpeed != endSpeed)
                {
                    currentSpeed = endSpeed;
                    playerMovment.changeSpeedRotation(
                        (currentSpeed  /* + 1 augmente la vitesse de base*/ ) / 10f);
                }
            }
        }
        //glowTortueAnim.Play("glowBtnBOff");
        cameraGame.getInstance().desactivateAigle();

        // a désactivé manuellement tortue
        if (tortue_on == false)
        {
            // accelerer
            t = 0f;
            while (t < tortue_acce_time)
            {
                yield return null;
                t += Time.deltaTime;
                currentSpeed = endSpeed + diff_speed * (t / tortue_acce_time);
                playerMovment.changeSpeedRotation(
                    (currentSpeed  /* + 1 augmente la vitesse de base*/ ) / 10f);
            }
            playerMovment.changeSpeedRotation(
                    (startSpeed  /* + 1 augmente la vitesse de base*/ ) / 10f);
        }
        else // toucheCount == 0, a release checkP, reinitialiser
        {
            playerMovment.changeSpeedRotation(
                    (startSpeed  /* + 1 augmente la vitesse de base*/ ) / 10f);
            tortue_on = false;
        }
    }
    #endregion


    public Animation gemmeCompteurAnim;
    public float timeCompteurAnimOn = 1.5f;
    public crownProgBar cProgBarMenuNiveau;
    /*
     * void giveRecompense(recompense type)
     * Ienumerator giveCrown(int number)
     * void debiteMonney(int val)
     */
    #region LES RECOMPENSES , ARGENT - GEMMES

    /*
     * void giveMonneyInfinis()
     * IEnumerator waitPickMonneyEnd()
     */
    #region Monnaie mode infinis

    private static float timeOut = 2f;
    public void pickUpYowInfinis(bool onMap) // appelé quand player ramasse YOW sur level, mode infinis
    {   
        if(onMap)
        {
            // new 
            monnaieVal++;
            updateMonneyText(monnaieVal);
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
        }
        else
        {
            StartCoroutine(giveMonney(true, 1, false, true));
            popYowGemManager.popCoinGame(); // incremente compteur wait
            popYowGemManager.getInstance().openCompteurYow_inGame(); // ouvre compteur
            StartCoroutine(waitPickMonneyEnd()); // reinitialise compteur
        }
    }
    private IEnumerator waitPickMonneyEnd()
    {
        yield return new WaitForSeconds(timeOut);
        //popYowGemManager.getInstance().closeCompteurYow_inGame();
        popYowGemManager.coinEndAnimGame();
    }

    //private bool lockAnimOut = false;

    #endregion

    public void rewardYowGem_giveYow(int nbYow)
    {
        StartCoroutine(giveMonney(false, nbYow, false, false));
    }

    // pickOnMap : true quand piece reccup sur level -> fermeture compteur géré tte seule
    private IEnumerator giveMonney(bool fromGame, int nbAdd, bool arriveeReward, bool pickOnMap)
    {
        
        if (fromGame)
        {
            if(!arriveeReward)
                yield return new WaitForSeconds(0.5f);

            monnaieTextGo.GetComponent<Animation>().Play("monnaieIncremente");
            yield return new WaitForSeconds(5f / 60f);
            monnaieVal++;
            updateMonneyText(monnaieVal);
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.get_yow);

            // on essaie de close le compteur
            // est appelé dès qu'une piece atteint le compteur, la première est celle qui lance la fermeture
            if (!pickOnMap)
            {
                yield return new WaitForSeconds(timeCompteurAnimOn);
                popYowGemManager.coinEndAnimGame();
                // BUG : lockAnimOut bloque autres blasts ( qui appellent tous individuellement .closeCompteurYow_inGame();
                /*if (lockAnimOut)
                    yield break;
                lockAnimOut = true;
                yield return new WaitForSeconds(timeCompteurAnimOn);
                lockAnimOut = false;
                popYowGemManager.getInstance().closeCompteurYow_inGame();*/
            }
        }
        else
        {
            monnaieTextGo.GetComponent<Animation>().Play("monnaieIncremente");
            yield return new WaitForSeconds(5f / 60f);
            monnaieVal += nbAdd;
            updateMonneyText(monnaieVal);
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.get_yow);
        }
    }

    public static void updateMonneyText(int value)
    {
        // on veut 5 digit
        int lengthValue = value.ToString().Length;
        int nbZero = 5 - lengthValue;

        /*int nbZero = value < 10 ?
            3 : value < 100 ?
            2 : value < 1000 ?
            1 : 0;*/

        string zeroTxt = "";

        if(nbZero > 0)
        {
            while(nbZero > 0)
            {
                zeroTxt += '0';
                nbZero--;
            }
        }

        getInstance().monnaieText.transform.parent.GetComponent<Text>().text = zeroTxt + value.ToString(); 
        getInstance().monnaieText.text = "<color=\"#91895A\">" +zeroTxt+ "</color>" + value.ToString();
    }

    public void updateLife()
    {
        nbVieText.text = nbVies.ToString();
    }

    public IEnumerator giveMoneyArrivee(float timeWait) // séparé, ne fait aucune animation ( pas de compteur qui s'ouvre ), juste incrémenter
    {
        monnaieVal += 1;
        updateMonneyText(monnaieVal);
        yield return new WaitForSeconds(timeWait);
        sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
    }

    public void giveRecompense(recompense type, bool arriveeReward)
    {
        
        switch (type)
        {
            case recompense.ARGENT:
                StartCoroutine(giveMonney(managerNiveau.gameOn ? true : false, 1, arriveeReward, false));
                break;
            case recompense.GEMME:
                if(arriveeReward) // si est pickUp
                    gemmesManager.getInstance().addGemme(true);
                else // sinon appelé si player stuck
                    StartCoroutine(giveGemme_playerStuck());
                break;
            case recompense.VIE:
                nbVies++;
                updateLife();
                nbVieText.GetComponent<Animation>().Play("perdVie");
                sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.getLife_compteur);
                break;
            case recompense.CROWN:
                //sfxManager.getInstance().playSound_menuP(typeSound_menuP.get_crown);
                managerNiveau.instance.incrementeCrown();
                decompteCrown--;
                break;
            case recompense.BOOST:
                break;
        }       
    }

    private IEnumerator giveGemme_playerStuck()
    {
        // UNIQUEMENT appelé quand player stuck ( si regarde d'ou est appelé )

        gemmeCompteurAnim.Play("diamondComeGame");
        yield return new WaitForSeconds(3/6f);
        // anim gemme incremente
        // gemmesManager.getInstance().addGemme(false); // fait l'animation d'incrémentation

        float t = 0f;
        while(t < timeCompteurAnimOn) //&& animGemmeOn)// tant que l'anim n'est pas coupée par compteur piece
        {
            yield return null;
            t += Time.deltaTime;
        }

        gemmeCompteurAnim.Play("diamondOutGame");
        gemmesManager.saveNbGemmes();
    }

    IEnumerator giveCrown(int number)
    {
        Debug.Log("<color=red>---- Player get reward crown ----</color>");
        decompteCrown = number;
        yield return new WaitForSeconds(0.5f);
        List<Transform> freePlace = cProgBarMenuNiveau.getFreeStatment();
        Debug.Log("- nombre place disponible dans progBar : " + freePlace.Count);
        Debug.Log("- nombre crown given : " + number);

        for (int i = 0; i < number; i++)
        {
            Vector2 pos = new Vector2(0 - UnityEngine.Random.Range(-1f, 1f), 0 - UnityEngine.Random.Range(-1f, 1f));
            GameObject g = Instantiate(crownGo);
            g.GetComponent<recompenseLvl>().spawn(pos, freePlace[i].position);
            g.SetActive(true);

            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.crown_pop);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator debiteMonney(int val)
    {
        monnaieTextGo.GetComponent<Animation>().Play("monnaieDebite");
        sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.pay);

        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.use_yowGem);
        yield return new WaitForSeconds(7f / 60f);
        monnaieVal -= val;
        updateMonneyText(monnaieVal);
        SaveSystem.Save(typeSave.MONNEY, monnaieVal);
    }

    #endregion

    public Animation btnQuitAnim;
    /*
     * void btnStart()
     * void btnQuit()
     */
    #region Les BOUTONS

    public void btnStart()
    {
        menuGameOver.getInstance().gameStart();
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_btnStartGame);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.btnStart);
        if (currentPack != null && currentPack.currentLevel != 0)
            currentPack.partieFinit();

        managerNiveau.gameOn = true;
        menuGameOver.hasRevived = false;

        if (infinisManager.infinis_Mode_On == false)
        {
            StartCoroutine(managerNiveau.getInstance().gameStart(false));

            currentNiveau = managerNiveau.currentNiveauGo.transform;
            records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);
            bestLevel = records[managerNiveau.currentNiveau];

            StartCoroutine(loadGame());
        }

        // infinis doit lui aussi appeler : ferme bar star
        progBarStar.loadBar();

        viePerdue = false;
        musicManager.getInstance().switchMusicType(typeMusic.ingame);
    }

    public void btnStartOnline()
    {
        managerNiveau.gameOn = true;
        menuGameOver.hasRevived = false;
        viePerdue = false;
        musicManager.getInstance().switchMusicType(typeMusic.ingame);
    }

    public void btnQuit()
    {
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.btnQuit);

        //btnQuitAnim.Play("btnQuitClose");
        gameOn = false;
        StartCoroutine(closeGame(true, true, false, false));
    }

    #endregion



    public void btnReplay_1()
    {
        cameraGame.getInstance().playerReplay(); // avant setInitvalues qui va reset la couleure du fond

        setInitValues();
        progBarNiv.GetComponent<progBarGame>().initBar(currentPack.lesNiveaux.Length);
        progBarStar.playerReplay();

        currentPack.playerReplay(); // remet level à 0
        boostIngameManager.getInstance().playerReplay(); // reload les boosts  

        menuTuto.playerReplay();
    }

    public void btnReplay_2()
    {  
        animFond3DGame.getInstance().zoomIn();

        StartCoroutine(restart(true)); // fait juste repop player
        managerNiveau.gameOn = true;
        menuGameOver.hasRevived = false;

    }

    public manetteManager manetteM;
    /*
     * void setInitValues()
     * IEnumerator loadGamel()
     * IEnumerator closeGame(bool quick, bool died, bool fromReviveMenu)
     * void initGame()
     * IEnumerator restart(bool fromMenuRevive)
     * IEnumerator finishedLevel()
     * void checkRecord(bool died)
     */
    #region GESTION AFFICHAGE

    public void setInitValues()
    {
        SCORE = 0;
        speedValue = initSpeedValue;
        playerMovment.changeSpeedRotation(speedValue / 10f);
        cameraGame.getInstance().adventureGameMode_activated();

        nbVies = expertMode.modeExpertOn && !infinisManager.infinis_Mode_On ? 1 :
            menuTuto.nbLevelTutoAventure > managerNiveau.currentNiveau ? menuTuto.nbLifeInitLevelTuto : VIE;
        updateLife();
    }

    private Animation _APPLIAnimation;
    private Animation getAPPLIAnimation()
    {
        if (_APPLIAnimation == null)
            _APPLIAnimation = GameObject.Find("APPLI").GetComponent<Animation>();
        return _APPLIAnimation;
    }

    public Transform parentAnimLevelScale;
    public Animation animLevelScale;
    private Transform parentLevelBeforeAnim;
    public Animation bordSombreGameAnim;
    public getStarMenuP managerStarMenuP;
    public safeArea safeA;

    //public float timePlayerPop_progBarSet = 0.2f; // animation tuto
    public progBarStarIngame progBarStar;
    //appelé depuis le menu principal 

    public IEnumerator loadGame()
    {
        setInitValues();
        yield return new WaitForSeconds(1f / 6f);
        getAPPLIAnimation().Play("lancerPartie");
        safeA.animVoyantHautGameStart();        

        yield return new WaitForSeconds(0.2f);
        parentLevelBeforeAnim = currentNiveau.parent;
        currentNiveau.SetParent(parentAnimLevelScale);
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_startGame);
        animLevelScale.Play("niveauGame");
        bordSombreGameAnim.Play("bordSombreGameOn");
        //managerAnimFond3D.getInstance().gameOpen();
        animFond3DGame.getInstance().gameOpen();

        yield return new WaitForSeconds(0.55f);

        //Debug.Log("Open level, num : " + managerNiveau.currentNiveau+", saveGaucherDroitier : "+ SaveSystem.loadData(typeSave.MANETTE_SIDE_HAND));
        int sideHand = (int)SaveSystem.loadData(typeSave.MANETTE_SIDE_HAND);
        bool openTuto;
        // d'abord test : si first level, et choisit entre G/D ( aussi modif menu param )
        if ((sideHand == -1|| sideHand == -1000) && managerNiveau.currentNiveau == 0)
        {
            menuTuto.openChoiceHand();
            // on attend alors la réponse pour passer à la suite
            while (menuTuto.resultChoiceInt == -1)
                yield return null;

            yield return new WaitForSeconds(0.65f);
            openTuto = menuTuto.tryOpenTuto(SaveSystem.nb_couronne, managerNiveau.currentNiveauGo, false);
            yield return new WaitForSeconds(0.65f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            openTuto = menuTuto.tryOpenTuto(SaveSystem.nb_couronne, managerNiveau.currentNiveauGo, false);
            if(openTuto)
                yield return new WaitForSeconds(0.35f);
        }
 
        currentPack = currentNiveau.GetComponent<packNiveau>();
        progBarNiv.gameObject.SetActive(true);
        progBarNiv.GetComponent<progBarGame>().initBar(currentPack.lesNiveaux.Length);


        if(!openTuto)
        {
            StartCoroutine(loadGame_passTuto(false));
        }
        // sinon on attend que tuto appelle fin ouverture game

    }

    public menuTutoAventure menuTuto;
    public IEnumerator loadGame_passTuto(bool fromTuto)
    {
        if (fromTuto)
            yield return new WaitForSeconds(0.3f);
        //yield return new WaitForSeconds(timePlayerPop_progBarSet);
        currentPack.startGame();
        menuChoixNiveau.SetActive(false);
        initGame();

        yield return new WaitForSeconds(1f / 2f); // temps que player spawn
        gameOn = true;
    }

    public notifDefiDone notifAnim;
    public static bool waitingEndAnimUnlockStar = false;
    public IEnumerator closeGame(bool quick, bool died, bool fromReviveMenu, bool fromGameOver)
    {
        SaveSystem.Save(typeSave.MONNEY, monnaieVal);
        //SaveSystem.Save(typeSave.SPEED_ROT, speedValue);
        manetteM.savePosBtnManette();
        musicManager.getInstance().switchMusicType(typeMusic.menuP);
        secretOffer.getInstance().playerEndGame();
        
        if (quick)
        {   
            // tout de suite l'arreter
            player.GetComponent<playerMovment>().stop();
        }
        else if (!fromGameOver)
        {
            yield return new WaitForSeconds(0.8f); // temps de l'anim de valide
            menuChoixNiveau.SetActive(true);

            yield return new WaitForSeconds(1.2f);
            player.GetComponent<playerMovment>().stop();
        }

        GhostBoostManager.getInstance().gameEnd(false);

        if (infinisManager.infinis_Mode_On)
            infinisManager.bonusPhase = false;

        if (!infinisManager.infinis_Mode_On)
        {
            progBarNiv.GetComponent<progBarGame>().animBarStop();

            if(!fromReviveMenu)
                yield return new WaitForSeconds(0.1f);

            progBarNiv.gameObject.SetActive(false);
            player.gameObject.SetActive(false);

            menuChoixNiveau.SetActive(true);
            currentNiveau.GetComponent<packNiveau>().gameEnd();

            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_endGame);
            bordSombreGameAnim.Play("bordSombreGameOff");
            animLevelScale.Play("niveauGameEnd");
            // on doit attendre que l'anim soit finit avant de changer parent
            StartCoroutine(waitAnimEnd_changeLevelParent());
            getAPPLIAnimation().Play("fermerPartie");
            safeA.animVoyantHautGameEnd(); // moment ou UI menuP commence à revenir

            appliManager.instance.reactiveFondMenu();
            cameraGame.getInstance().adventureGameMode_desactivated();

            // réinitialiser pos de compteur or et gemmes
            // si ferme trop vite gameOver, n'est pas réinitialisé
            //yield return new WaitForSeconds(1f);
            //safeA.forceCloseClean();

            yield return new WaitForSeconds(1.4f);//1.4f //1.75f);

            cProgBarMenuNiveau.reOpenBar();
            managerNiveau.getInstance().gameEnd_reopenLevels();
            
            //if (!quick || (quick && fromReviveMenu))
            //{
                // donne les couronnes
                checkRecord(died);
            //}

            int prevC = SaveSystem.nb_couronne;
            while (decompteCrown != 0)
                yield return new WaitForSeconds(Time.deltaTime);

            // se bloque ici, pcq animNotif jamais appelé, est appelé pcq suite code appelé
            // on regarde si est en train d'animer notif star -> attendre fin
            if (prevC < lockStar.crownRequired && SaveSystem.nb_couronne >= lockStar.crownRequired) {
                // /!\ n'est pas encore en etat 'animating'
                waitingEndAnimUnlockStar = true;
                notifStarUnlocked.getInstance().endPhaseGame();

                while (waitingEndAnimUnlockStar)
                    yield return new WaitForSeconds(Time.deltaTime);
            }

            int newC = SaveSystem.nb_couronne;
            int nextProgC = defiManager.getInstance().getNextProgCrown();
            int nbCrownWon = newC - prevC;
            if (prevC < nextProgC && prevC + nbCrownWon >= nextProgC)
                notifAnim.newDefiIsUnlocked(typeDefi.crown);

            defiManager.getInstance().defiIsIncremented(typeDefi.crown, 0); // n'a pas besoin de savoir cb reçu

            int prevS = SaveSystem.getNbStarObtain();
            int nextProgS = defiManager.getInstance().getNextProgStar();
            // check si recoit l'etoile
            if (expertMode.modeExpertOn
                && !quick && !died && !viePerdue /*&& !hasUsedABoost()*/
                && managerNiveau.getInstance().obtainStar()) // true si reçoit l'etoile, sinon l'a deja
            {
                recompenseDefi.forStar = true;
                managerStarMenuP.playerGetStar();

                //Debug.Log("Player get star, prevS : " + prevS + " , nextProgS : " + nextProgS);
                if (prevS + 1 == nextProgS)
                    notifAnim.newDefiIsUnlocked(typeDefi.star);
                yield return new WaitForSeconds(getStarMenuP.timeAnimGetStar);
                recompenseDefi.forStar = false;
            }

            managerNiveau.getInstance().gameEnd_allRewardGiven();

            managerNiveau.records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);
            managerNiveau.instance.checkNewLevel(newC, prevC, currentPack.niveauClean()
                , SaveSystem.getNbStarObtain(), prevS); // lance niv suivant si finit niv
            managerNiveau.gameOn = false;

            managerNiveau.getInstance().lauchMenu(false, false, true, false);

            defiManager.getInstance().gameEnd();
            notifAnim.showAllNotif();
            notifInfinisUnlocked.getInstance().endPhaseGame();
        }
        else
        {
            defiManager.getInstance().gameEnd();
        }

        managerNiveau.gameOn = false;
        // notification si defi finit

        //StartCoroutine(waitMenuReopenToNotif());
    }

    /*private IEnumerator waitMenuReopenToNotif()
    {
        while (!notifAnim.gameObject.activeInHierarchy)
            yield return null;

        notifAnim.showAllNotif();

        if (!infinisManager.infinis_Mode_On)
        {
            notifInfinisUnlocked.getInstance().endPhaseGame();
        }
    }*/

    private IEnumerator waitAnimEnd_changeLevelParent()
    {
        yield return new WaitUntil(() => animLevelScale.isPlaying == false);
        currentNiveau.SetParent(parentLevelBeforeAnim);
    }
    public void initGame()
    {
        player.GetComponent<playerMovment>().activate();
        //Debug.Log("Activate player");
    }

    public float timePlayerRepop_aventure = 1.7f;
    // appelé quand meurt mais a encore des vies
    public IEnumerator restart(bool fromMenuRevive)
    {
        gameOn = false;

        if (!fromMenuRevive)
            yield return new WaitForSeconds(timePlayerRepop_aventure);

        if (!infinisManager.infinis_Mode_On)
        {
            initGame();

            yield return new WaitForSeconds(1f / 2f); // temps que player spawn
            gameOn = true;
        }
    }

    public float time_bonusPop_toZoomOut = 2f; // 2.5f
    public float time_zoomOut_toUpdateProgBar = 0.8f;
    public float time_cleanLevel_toZoomIn = 1f;
    public float time_cleanLevel_toGameOver = 1f;//1.5f
    public float time_playerRepop_nextLevel = 0.8f;
    public static bool isWaitingTutoBetweenLevel = false;
    // appelé quand clean lvl et passe au suivant
    public IEnumerator finishedLevel()
    {
        if (!currentPack.niveauClean())
        {
            progBarNiv.GetComponent<progBarGame>().passNextLvl(true);

            yield return new WaitForSeconds(time_bonusPop_toZoomOut); // temps bonus pop

            animFond3DGame.getInstance().zoomOut();
            //managerAnimFond3D.getInstance().dezoom_level();

            cameraGame.getInstance().zoomBetweenNormalLvl(true);
            cameraGame.getInstance().playerWin_changeDifficultyColor(SCORE);
            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_nextLevel);
            //sfxManager.getInstance().playSound_ingame(typeSound_ingame.zoom_level);

            yield return new WaitForSeconds(time_zoomOut_toUpdateProgBar);

            // progBarNiv.GetComponent<progBarGame>().passNextLvl(true);
            progBarStar.playerWinLevel();
            currentPack.lvlClean();


            // si un tuto intermediaire
            if (menuTuto.tryOpenTuto(SaveSystem.nb_couronne, managerNiveau.currentNiveauGo, true))
            {
                // attendre un peu
                yield return new WaitForSeconds(menuTuto.timeBetweenZoomOut_openTuto);
                // tuto s'ouvre seul
                isWaitingTutoBetweenLevel = true;

                // attendre jusqu'à tuto soit refermé
                while (isWaitingTutoBetweenLevel)
                    yield return null;
            }
            else
                yield return new WaitForSeconds(time_cleanLevel_toZoomIn);

            pM.changeLevel();

            animFond3DGame.getInstance().zoomIn();
            //managerAnimFond3D.getInstance().zoom_level();

            cameraGame.getInstance().zoomBetweenNormalLvl(false);
            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_nextLevel);

            yield return new WaitForSeconds(time_playerRepop_nextLevel);

            initGame();

            yield return new WaitForSeconds(1f / 2f); // temps que player spawn
            gameOn = true;
        }
        else
        {
            yield return new WaitForSeconds(time_cleanLevel_toGameOver);
            progBarNiv.GetComponent<progBarGame>().passNextLvl(true);
            progBarStar.playerWinLevel();

            menuGameOver.getInstance().openMenu(false, SCORE, false);
            //StartCoroutine(_gameOverManager.openGameOverMenu(true, false, SCORE, currentPack));
        }
    }

    // appelé en fin de partie
    public void checkRecord(bool died)
    {
        int curLvl = died ? currentPack.currentLevel : currentPack.currentLevel + 1;

        int replayLvl = menuGameOver.getBestScoreGame();
        if(replayLvl != -1)
        {
            curLvl = replayLvl > curLvl ? replayLvl : curLvl;
        }

        if (curLvl > bestLevel)
        {
            StartCoroutine(giveCrown(curLvl - bestLevel));
            records[managerNiveau.currentNiveau] = curLvl;
            SaveSystem.Save(typeSave.BEST_SCORE, records);
            bestLevel = curLvl;
        }
    }
    #endregion

    //public gameOverManager _gameOverManager;
    /*
     * IEnumerator openGameOverMenu(bool win, bool fromRevive)
     */
    #region GAME OVER && REVIVE

    /*public void openGameOverMenu(bool win, bool fromRevive)
    {
        menuGameOver.getInstance().openMenu(false, SCORE, !win);
        //StartCoroutine(_gameOverManager.openGameOverMenu(win, fromRevive, SCORE, currentPack));
    }*/

    #endregion



    public effectColorInfos[] effectInfos;
    public ParticleSystem ps_win, ps_loose;

    public void eventIngame(typeEffectColor typeEffect)
    {
        if (typeEffect == typeEffectColor.win)
            ps_win.Play();
        if (typeEffect == typeEffectColor.lose)
            ps_loose.Play();
        if (!infinisManager.infinis_Mode_On)
        {
            managerNiveau.getInstance().eventHappened_animGlow(getColorEffect_byEffect(typeEffect));
        }
    }

    public Color getColorEffect_byEffect(typeEffectColor t)
    {
        int i = 0;
        while (i < effectInfos.Length && effectInfos[i]._typeEffect != t)
            i++;
        if (i >= effectInfos.Length)
        {
            return Color.white;
        }
        else
            return effectInfos[i]._color;
    }


    public void win()
    {
        SCORE++;
        getAPPLIAnimation().Play("fondGagne");
        player.GetComponent<playerMovment>().arivee();


        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.win);

        if (!infinisManager.infinis_Mode_On)
        {
            gameOn = false;
            StartCoroutine(finishedLevel());
        }
        defiManager.getInstance().playerEndsLevel(true);
        eventIngame(typeEffectColor.win);
    }

    public void died()
    {
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_die);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.death);
        nbVieText.GetComponent<Animation>().Play("perdVie");
        getAPPLIAnimation().Play("fondLose");
        animFond3DGame.getInstance().playerDied();

        playerDied_checkBtnBOn();

        updateLife();

        if (!infinisManager.infinis_Mode_On && playerMovment.comportement != typeBehaviour.online)
        {
            viePerdue = true;
            gameOn = false;
            

            if (nbVies > 0)
                StartCoroutine(restart(false));
            else
            {
                if (!menuGameOver.hasRevived && currentPack.proposerVieSupp()
                    && managerAdMob.getInstance().AdIsReadyToShow(typeAd.revive_ad))
                    menuGameOver.getInstance().openMenu(true, SCORE, true);
                //StartCoroutine(_gameOverManager.proposerVie(SCORE, currentPack));
                else
                    menuGameOver.getInstance().openMenu(false, SCORE, true);
                //StartCoroutine(_gameOverManager.openGameOverMenu(false, false, SCORE, currentPack));
            }
        }
        else
            StartCoroutine(restart(false));

        defiManager.getInstance().playerEndsLevel(false);
        eventIngame(typeEffectColor.lose);
    }

    // appelé par trackerPlayerStuck
    public void died_stuck()
    {
        playerDied_checkBtnBOn();

        //StartCoroutine(restart(false));
        defiManager.getInstance().playerEndsLevel(false);
    }

    public void unstuckPlayer()
    {
        StartCoroutine(restart(false));
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

// 1053.l -> 793.l