using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class infinisManager : MonoBehaviour
{
    #region VARIABLES
    private static infinisManager instance;

    public static int SCORE;
    public GameObject scoreText;

    public GameObject player;
    public playerMovment pM;
    public GameObject test_lvlDepart;
    public GameObject[] test_lesPatern;

    public GameObject animLevelUp;
    public Text levelUpText;

    private List<GameObject> prevPaterns;
    public static GameObject currentPatern;
    private static GameObject prev_level;

    public static bool infinis_Mode_On = false;
    public static bool game_On = false;

    /*
    * VARIABLES DE PARTIE
    */
    private static int nbPaternToBeReusable = 14; // avant 9 pour 18, mtn 14 pour 24
    // difficulté
    private static int lvlDifficulty = 1;
    public static List<int> rangUpDifficulty = new List<int>() { 10, 25, 45, 70 }; // { 15, 30, 50, 70 } // { 2, 4, 6, 70 };
    private static List<int> rangNoBonus = new List<int>();
    public static List<int> getLevelWithoutBonus()
    {
        if(rangNoBonus.Count == 0)
        {
            foreach(int i in rangUpDifficulty)
            {
                rangNoBonus.Add(i - 1);
                rangNoBonus.Add(i);
                rangNoBonus.Add(i + 1);
            }
        }
        return rangNoBonus;
    }
    // les bonus
    public GameObject[] test_lesPaternBonus;
    //private float probLvlBonus = 1f / 3f; // 1
    public static bool bonusPhase = false;
    private int prevLvlBonusApparition = 0, rangeMinBetweenLvlBonus = 5; // 4
    #endregion

    void Start()
    {
        instance = GameObject.Find("MODE INFINIS").GetComponent<infinisManager>();
    }

    public static infinisManager getInstance()
    {
        return instance;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    /*
     * void preloadNextLevel()
     * void loadNextLevel()
     * void chooseNewPatern()
     * void startInfinis()
     * 
     * IEnumerator initGame()
     * void initGameSuite()
     * static void winLevel()
     * void setScore(int _s)
     * 
     * IEnumerator nextLevel()
     * IEnumerator nextLevel_suite()
     * void openLevelUp()
     * void levelUpEnd()
     * 
     * IEnumerator setPlayerParentLate()
     * void died()
     * void died_stuck()
     * void endInfinis()
     * 
     * static float getTimeRepopInfinis()
     * IEnumerator reactiveBtnChangeRot(bool newLevel, bool fromRevive)
     */
    #region Gestion phase jeu

    private bool prevIsCut;
    private bool prevIsLong;
    // seulement appelé par initGame
    public Transform preloadNextLevel()
    {
        prevIsCut = false;
        prevIsLong = false;

        loadNextLevel();
        return preloadPatern.transform;
    }

    private bool preloadBonus = false;
    private GameObject preloadPatern;
    public int minScore_levelEnabled = 2;
    private void loadNextLevel(/*bool _bonusPhase*/)
    {
        /*preloadBonus = _bonusPhase;

        GameObject[] l = _bonusPhase == true ? test_lesPaternBonus : test_lesPatern;*/

        int i = Random.Range(0, test_lesPatern.Length);
        bool minScorePassed = SCORE >= minScore_levelEnabled;
        packNiveau_infinis p = test_lesPatern[i].GetComponent<packNiveau_infinis>();
        // tant que
        // level dans liste used
        // level pas encore disponible
        int testCounter = 0;
        while (prevPaterns.Contains(test_lesPatern[i])
            || (p.notInFirstLevels && !minScorePassed)
            || ((prevIsCut || prevIsLong) && (p.levelIsCut || p.levelIsLong))
            || !p.checkIfCombiAvailable()// on check ici aussi si une combi est disponible
            )
        {
            i = Random.Range(0, test_lesPatern.Length);
            p = test_lesPatern[i].GetComponent<packNiveau_infinis>();
            testCounter++;
            //Debug.Log("Nb iteration find level : " + testCounter);
        }
        //Debug.Log("Nb iteration find level : " + testCounter);

        prevIsCut = p.levelIsCut;
        prevIsLong = p.levelIsLong;

        if (prevPaterns.Count == nbPaternToBeReusable)
            prevPaterns.RemoveAt(0);

        preloadPatern = test_lesPatern[i];
        prevPaterns.Add(preloadPatern);

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            preloadPatern.GetComponent<packNiveau_infinis>().mirror();
        }

        //packNiveau_infinis p = preloadPatern.GetComponent<packNiveau_infinis>();

        int levelDifficulty_2 = rangUpDifficulty.Contains(SCORE + 1) ? lvlDifficulty + 1 : lvlDifficulty;

        p.launchPack(levelDifficulty_2,
            SCORE > 0 && SCORE + 1 == SaveSystem.record_infinis ? true : false, -1);

        // savoir si bonus phase
        bool rangCanBeBonus = getLevelWithoutBonus().Contains(SCORE);
        bool bonusON = rangCanBeBonus && SCORE - prevLvlBonusApparition > rangeMinBetweenLvlBonus
            && p.currentCombiCs.combiCanBeBonus;
        if (bonusON)
        {
            Debug.Log("<color=red>NEXT WILL BE BONUS</color>");
            preloadBonus = true;
            prevLvlBonusApparition = SCORE + 1;
        }
        else if (preloadBonus)
        {
            preloadBonus = false;
            bonusPhase = true;
            cameraGame.getInstance().modeInfinisBonus(true);
        }
        p.lanchPack_late(preloadBonus);
    }

    private void chooseNewPatern()
    {
        // on regarde si le prochain level sera dans phase bonus
        /*if (SCORE - prevLvlBonusApparition > rangeMinBetweenLvlBonus
            && probLvlBonus >= Random.Range(0f, 1f))
        {
            preloadBonus = true;
            prevLvlBonusApparition = SCORE + 1;
        }
        // s'il ne l'est pas
        else if(preloadBonus == true)
        {
            preloadBonus = false;
            bonusPhase = true;
        }*/

        currentPatern = preloadPatern;
        loadNextLevel();
    }

    public menuTutoAventure tutoManager;
    public void startInfinis()
    {
        tutoManager.modeInfinisStarted();
        StartCoroutine(initGame());
    }

    public float timeOpenInitRoadMap = 0.5f;
    public Animation bordSombreGameAnim;
    private IEnumerator initGame()
    {
        //packNiveau_infinis.prevLevelPopedSuperPower = false;

        packNiveau_infinis.resetBonusVariables_initGame();
        //menuLevelUp.gameStart();

        lvlDifficulty = 1;
        prevPaterns = new List<GameObject>();
        gameManager.getInstance().setInitValues();
        SCORE = 0;
        setScore(0);
        bool currentIsAlreadyStart = currentPatern == test_lvlDepart;
        currentPatern = test_lvlDepart;

        //scrollManager.scroll.resetInitGame();

        yield return new WaitForSeconds(0.25f);

        // on doit cash donner level suivant au scroll ( avec bon mirror, bon level difficulté ), 
        // pop boost 1sec + tard sur level quand centré
        Transform nextLevel = preloadNextLevel();
        scrollManager.startGame(nextLevel);

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_startGame);

        if (currentIsAlreadyStart)
        {
            while (!test_lvlDepart.activeSelf) yield return null;
        }

        //test_lvlDepart.GetComponent<packNiveau_infinis>().launchFirstPack();
        bordSombreGameAnim.Play("bordSombreGameOn");
        animFond3DGame.getInstance().gameOpen();
        //managerAnimFond3D.getInstance().gameOpen();

        yield return new WaitForSeconds(timeOpenInitRoadMap);
        roadMap.openMenuRoadMap(false, true);

        //Debug.Log("Force set nbLevelPassed : 2");
        //scrollManager.scroll.nbLevelPassed = 2;
        //yield return new WaitForSeconds(1f);

        //game_On = true;
    }

    public void initGameSuite()
    {
        test_lvlDepart.GetComponent<packNiveau_infinis>().launchFirstPack();
        game_On = true;
    }

    public static void winLevel()
    {
        game_On = false;     
        instance.StartCoroutine(instance.nextLevel());
        defiManager.getInstance().playerEndsLevel(true);
    }

    private void setScore(int _s)
    {
        string s = "";
        int nbZ = 3 - _s.ToString().Length;
        while (nbZ > 0) { s += '0'; nbZ--; };
        scoreText.GetComponent<Text>().text = "<color=\"#4A6D63\">" + s + "</color>" + _s;
        //Debug.Log("DEBUG SCORE - score ui : " + scoreText.GetComponent<Text>().text);
    }

    public Animation animTextScoreInfinis;
    private GameObject currentPatern_copy;
    public float timeWait_animScore = 1.2f;
    private float timeWait_scoreRefresh = 0.25f; // en plein milieu de anim text, changer le text
    private IEnumerator nextLevel()
    {
        //Debug.Log("TEMPS " + Time.time + ", nextLevel , SCORE " + SCORE);
        player.transform.SetParent(currentPatern.transform);
        
        
        //player.transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(timeWait_animScore);// 1.5f
        animTextScoreInfinis.Play("infinisLevelUp");
        //.scoreText.GetComponent<Animation>().Play("scoreInfinisIncremente");

        yield return new WaitForSeconds(timeWait_scoreRefresh);
        SCORE++;
        Debug.Log("DEBUG SCORE - score change ( script ), new : " + SCORE);
        setScore(SCORE);

        if (rangUpDifficulty.Contains(SCORE))
            lvlDifficulty++;

        pM.changeLevel();

        if (rangUpDifficulty.Contains(SCORE))
            openLevelUp();
        else
            StartCoroutine(nextLevel_suite());
    }

    private IEnumerator nextLevel_suite()
    {
        if(cameraGame.modeBonusInfinisOn)
            cameraGame.getInstance().modeInfinisBonus(false);

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_nextLevel);

        currentPatern_copy = currentPatern;//, prev_level_2 = prev_level;
        chooseNewPatern();
        scrollManager.goNextPhase(preloadPatern.transform);
        StartCoroutine(setPlayerParentLate());

        yield return new WaitForSeconds(0.3f);
        currentPatern_copy.GetComponent<packNiveau_infinis>().levelPassed();
        prev_level = currentPatern_copy;
        if (bonusPhase)
        {
            //cameraGame.getInstance().modeInfinisBonus(false);
            bonusPhase = false;
        }
        StartCoroutine(reactiveBtnChangeRot(true, false));

        yield return new WaitForSeconds(0.4f);
        currentPatern.GetComponent<packNiveau_infinis>().popBoostLate();
    }

    //public menuLevelUpInfinis menuLevelUp;
    public roadMapInfinis roadMap;
    private void openLevelUp()
    {
        roadMap.openMenuRoadMap(true, false);
        //menuLevelUp.levelUp();
    }

    public void levelUpEnd()
    {
        StartCoroutine(nextLevel_suite());
    }



    private IEnumerator setPlayerParentLate()
    {
        yield return new WaitForSeconds(scrollManager.scroll.timeAnimScroll);
        player.transform.SetParent(currentPatern.transform);
        //player.transform.localScale = new Vector3(1, 1, 1);
    }

    public static void died()
    {
        game_On = false;

        if (gameManager.nbVies > 0)
        {
            instance.StartCoroutine(instance.reactiveBtnChangeRot(false, false));
        }
        else
        {
            if (instance.peutProposerVie() 
                && managerAdMob.getInstance().AdIsReadyToShow(typeAd.revive_ad))
            {
                menuGameOver.getInstance().openMenu(true, SCORE, true);
            }
            else
                menuGameOver.getInstance().openMenu(false, SCORE, true);
        }

        defiManager.getInstance().playerEndsLevel(false);
    }

    public static void died_stuck()
    {
        game_On = false;
        defiManager.getInstance().playerEndsLevel(false);
    }

    public void endInfinis()
    {
        if(game_On) // quick quit
            game_On = false;

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_endGame);
        scrollManager.endGame();
        player.transform.SetParent(null);
        player.transform.localScale = new Vector3(1, 1, 1);

        prevLvlBonusApparition = 0;

        appliManager.instance.reactiveFondMenu();
        scoreText.GetComponent<Text>().text = "";

        if(SCORE != 0)
        {
            defiManager.getInstance().defiIsIncremented(typeDefi.infinis, SCORE);
        }
        defiManager.getInstance().gameEnd();

        bordSombreGameAnim.Play("bordSombreGameOff");
        if (cameraGame.modeBonusInfinisOn)
            cameraGame.getInstance().modeInfinisBonus(false);

        roadMap.resetMenuRoadMap();
    }

    public float timeBeforeRepopAfterDeath = 0.5f;
    public float timeBeforeRepopAfterRevive = 0.5f;
    public float timeBeforeRepopChangeLevel = 0.8f;
    public static float getTimeRepopInfinis()
    {
        return instance.timeBeforeRepopChangeLevel;
    }
    public IEnumerator reactiveBtnChangeRot(bool newLevel, bool fromRevive)
    {
        if (!newLevel)
            currentPatern.GetComponent<packNiveau_infinis>().reset_level(fromRevive ? timeBeforeRepopAfterRevive : timeBeforeRepopAfterDeath);
        yield return new WaitForSeconds(1f);
        game_On = true;
    }
    #endregion


    public scrollInfinisManager scrollManager;
    /*
     * void openMenu()
     * void closeMenu()
     *  IEnumerator close()
     * bool checkBeforeInit()
     *  IEnumerator changeInitLevel()
     */
    #region Gestion menu
    public void openMenu()
    {
        currentPatern = test_lvlDepart;
        scrollManager.openMenuInfinis(true);
        infinis_Mode_On = true;
    }

    public void closeMenu()
    {
        scrollManager.closeMenuInfinis();
        StartCoroutine(close());
        infinis_Mode_On = false;
    }

    private IEnumerator close()
    {
        yield return new WaitForSeconds(1 / 4f);

        GameObject toClose = currentPatern != null ? currentPatern : test_lvlDepart;
        toClose.GetComponent<packNiveau_infinis>().resetLevel();
        currentPatern = test_lvlDepart;
    }

    // changer le premier niveau si un autre est activé
    public bool checkBeforeInit() // true si pas de pb
    {
        if (currentPatern != test_lvlDepart && currentPatern != null)
            changeInitLevel();
        return currentPatern == test_lvlDepart;
    }

    void changeInitLevel()
    {
        currentPatern = test_lvlDepart;
        initGame();
    }
    #endregion

    /*
     * bool peutProposerVie()
    */
    #region VIE SUP / Ad
    /* propose une vie si
     *  1 - fait + que lvlMin
     *  2 - a deja proposé une vie ET ecart de Y avec X
     *  3 - si est a son record et ecart de Y avec precedent
     */
    private int nbProposition = 0; // max 4
    private static int lvlMin = 8, rangeBetweenProp = 20, prevPropRang = 0;
    public bool peutProposerVie()
    {
        bool ret = false;
        if (SCORE >= lvlMin)
        {
            if (nbProposition == 0)
                ret = true;
            else if (nbProposition <= 4)
            {
                if (SCORE - prevPropRang >= rangeBetweenProp)
                    ret = true;
            }
            /*switch (nbProposition)
            {
                case 0:
                    if (SCORE >= lvlMin)
                        ret = true;
                    break;
                case 1:
                    if (SCORE - prevPropRang >= rangeBetweenProp)
                        ret = true;
                    break;
                case 2:
                    if (SCORE - prevPropRang >= rangeBetweenProp
                        && SCORE > (int)SaveSystem.loadData(typeSave.BEST_SCORE))
                        ret = true;
                    break;
                default:
                    ret = false;
                    break;
            }*/
            if (ret)
            {
                prevPropRang = SCORE;
                nbProposition++;
            }
        }
        return ret;
    }
    #endregion


    // 573 l. -> 390 l.





    [Header("Mode Online")]
    public GameObject lifePan;
    public GameObject progBarOnline;
    public GameObject eliminatedPan_online, disconnectedPan_online;
    public Text textDisconnected;

    int /*rangeMinBetweenLvlBonus_online = 6,*/ nbPaternToBeReusable_online = 7;
    public static int nbLevelInfinis_online = 2;// 12
    List<int> rangUpDifficulty_online = new List<int>() { 5 };// 4, 8

    public List<List<int>> listNiveaux_online = new List<List<int>>();
    public static bool switchingLevel_online = false;

    // region quit game
    public GameObject menuValidQuitOnlineGame;
    public static bool quitOnlineValidated = false;

    /*
     * void chooseNewPatern_onlineMode()
     * IEnumerator nextLevel_online()
     * IEnumerator initGame_online()
     * IEnumerator endInfinis_online()
     * IEnumerator localPlayerIsTheLast(bool playerDisconnected)
     * IEnumerator localPlayerIsDisconnected()
     * 
     * REGION creation liste niveau partagés
     * REGION quit game online
    */
    #region Mode ONLINE

    // renvoit l'index de la combi à utiliser
    /*private int chooseNewPatern_onlineMode()
    {
        // int bonusPhase , int childIndex, int mirror, int combiIndex
        List<int> infos = listNiveaux_online[SCORE];
        currentPatern = infos[0] == 1 ? test_lesPaternBonus[infos[1]] : test_lesPatern[infos[1]];

        // on regarde si le niveau est deja mirror
        // dans ce cas ne rien faire sinon aura niv inversé par rapport aux autres joueurs
        int isMirrored = currentPatern.GetComponent<packNiveau_infinis>().packIsMirrored ? 1 : 0;
        if (infos[2] != isMirrored) 
        {
            currentPatern.GetComponent<packNiveau_infinis>().mirror();
        }
        return infos[3];
    }

    private IEnumerator nextLevel_online()
    {
        switchingLevel_online = true;
        bool GAME_END = SCORE + 1 == nbLevelInfinis_online;

        if (!GAME_END)
            player.transform.SetParent(currentPatern.transform);
        else // sseulement quand partie se finit
        {
            // si on n'est pas connecté
            // on attend soit d'être viré
            // soit d'être reconnecté
            while(Application.internetReachability == NetworkReachability.NotReachable
                && OnlineManager.ONLINE_mode_on)// et on est dans le mode jeu
            {
                yield return null;
            }
            if (!OnlineManager.ONLINE_mode_on)  // ON SARRETE ici si a été déconnecté
            {
                switchingLevel_online = false;
                yield break;
            }

            ghostManager.getPlayerOnlineManager().localPlayerFinishedGame(false);
        }

        yield return new WaitForSeconds(1.5f);
        progBarModeOnline.getInstance().localPlayerWin();

        yield return new WaitForSeconds(0.25f);
        sfxManager.getInstance().playSound_ingame(typeSound_ingame.zoom_level);
        cameraGame.getInstance().zoomLevel(true, 3f);
        SCORE++;
        yield return new WaitForSeconds(0.75f);
        //ghostManager.getInstance().updateGhostVisibility_localPlayerWin(true);

        // si la partie continue
        if (!GAME_END)
        {
            // anim difficulté augmente
            if (rangUpDifficulty_online.Contains(SCORE))
            {
                animLevelUp.SetActive(true);
                lvlDifficulty++;
                string t = "Level " + lvlDifficulty;
                levelUpText.text = t;
                animLevelUp.GetComponent<Animation>().Play("levelUpInfinis");
                yield return new WaitForSeconds(160f / 60f); // eclair

                int i = 0, sens = 1;
                float tempsEclair = 0;
                do
                {
                    if (t[i] != ' ')
                    {
                        if (i == 0)
                        {
                            levelUpText.text = "<color=white>" + t[0] + "</color>" + t.Substring(1);
                        }
                        else if (i == t.Length - 1)
                        {
                            levelUpText.text = t.Substring(0, t.Length - 1) + "<color=white>" + t[t.Length - 1] + "</color>";
                        }
                        else
                        {
                            levelUpText.text = t.Substring(0, i) + "<color=white>" + t[i] + "</color>" + t.Substring(i + 1);
                        }
                    }

                    if (sens == 1 && i == t.Length - 1)
                    {
                        sens = -1;
                    }
                    i += sens;
                    yield return new WaitForSeconds(0.06f);
                    tempsEclair += 0.05f;

                } while (i != -1);
                levelUpText.text = t;
                yield return new WaitForSeconds((300f - 160f) / 60f - tempsEclair); // attendre la fin de l'animation
                animLevelUp.SetActive(false);
            }

            currentPatern.GetComponent<packNiveau_infinis>().levelPassed();
            if (currentPatern != test_lvlDepart)
                prev_level.GetComponent<packNiveau_infinis>().turnOffLevel(true);

            yield return new WaitForSeconds(0.5f);

            player.transform.SetParent(null);
            prev_level = currentPatern;

            int combiIndex = chooseNewPatern_onlineMode();
            currentPatern.SetActive(true);
            currentPatern.GetComponent<packNiveau_infinis>().launchPack(lvlDifficulty, bonusPhase,
                SCORE > 0 && SCORE == SaveSystem.record_infinis ? true : false, combiIndex);

            if (bonusPhase)
            {
                //cameraGame.getInstance().switchBonusMode();
                bonusPhase = false;
            }
            //else if (cameraGame.bonusOn)
            //    cameraGame.getInstance().switchBonusMode();
            StartCoroutine(reactiveBtnChangeRot(true, false));
        }
        switchingLevel_online = false;
    }

    private IEnumerator initGame_online()
    {
        //btnQuitOnline.GetComponent<Button>().enabled = false;
        //btnQuitOnline.transform.GetChild(0).GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        
        // On met les vies en mode infinis
        lifePan.transform.GetChild(1).gameObject.SetActive(false);
        lifePan.transform.GetChild(3).gameObject.SetActive(true);

        lvlDifficulty = 1;
        prevPaterns = new List<GameObject>();
        gameManager.getInstance().setInitValues();
        SCORE = 0;
        scoreText.GetComponent<Text>().text = "";
        currentPatern = test_lvlDepart;

        yield return new WaitForSeconds(0.25f);

        test_lvlDepart.GetComponent<packNiveau_infinis>().launchFirstPack();
        //managerAudio.playSound(typeSon.GAME_START);

        yield return new WaitForSeconds(0.5f);
        progBarModeOnline.getInstance().initProgBar(ghostManager.getPlayerOnlineManager().getOthersID());

        yield return new WaitForSeconds(0.5f);

        //btnChangeRot.SetActive(true);
        //btnChangeRot.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(
          // currentPatern.GetComponent<packNiveau_infinis>().initGameCheckP.transform.position);
        game_On = true;
    }

    public IEnumerator endInfinis_online() // reinitialiser les ui
    {   
        StartCoroutine(progBarModeOnline.getInstance().animCleanBar());
        yield return new WaitForSeconds(0.5f);
        //btnQuitOnline.GetComponent<Button>().enabled = false;
        //btnQuitOnline.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        lifePan.transform.GetChild(1).gameObject.SetActive(true);
        lifePan.transform.GetChild(3).gameObject.SetActive(false);
    }

    // event qui coupent la partie
    public IEnumerator localPlayerIsTheLast(bool playerDisconnected)
    {
        // localPlayer est totalement arreté
        player.GetComponent<playerMovment>().stop();
        //btnChangeRot.SetActive(false);

        if (!playerDisconnected)
        {
            warningPanGameOnline.getInstance().openVoyantWarning(typeWarning.isLastPlayer, 0f);
            /* afficher l'anim éliminé
            eliminatedPan_online.SetActive(true);

            int loose = menuRankingGameOnline.getInstance().getNbTrLooseIfQuit();
            textDisconnected.text = "Déconnecté : "+ loose;
            tropheOnlineManager.getInstance().changeNbTrophe(loose, true);/

            yield return new WaitForSeconds(1f);
            eliminatedPan_online.SetActive(false);
            // dezommer
            sfxManager.getInstance().playSound_ingame(typeSound_ingame.zoom_level);
            cameraGame.getInstance().zoomLevel(true, 3f);
        }
        else
        {
            warningPanGameOnline.getInstance().openVoyantWarning(typeWarning.isDisconnected, 0f);
            //disconnectedPan_online.SetActive(true);
            yield return new WaitForSeconds(1f);
            //disconnectedPan_online.SetActive(false);
        }
        //btnChangeRot.SetActive(false);
    }

    public IEnumerator localPlayerIsDisconnected()
    {
        if (OnlineManager.ONLINE_mode_on) // seulement si on est dans le mode jeu
        {
            OnlineManager.ONLINE_mode_on = false; // debloquer attente de co de nextlevel_online

            while (switchingLevel_online)
                yield return null;
            // animation disconnected
            StartCoroutine(localPlayerIsTheLast(true));

            //quitOnlineValidated = true;
            menuRankingGameOnline.stayInRoom = false;
            appliManager.instance.quitGame_online();

            //quitOnlineValidated = false;
            OnlineManager.ONLINE_mode_on = false;
            tropheOnlineManager.getInstance().playerEndAGameOnline();
        }
    }*/

    /*
     * void btnQuit_online()
     * IEnumerator closeMenuQuitTimer()
     * void closeMenuQuit(bool quitGame)
     * IEnumerator closeDelay()
     * void playerLeavesTheGame()
     * void confirmQuit_online()
     * void denyQuit_online()
    */
    #region quit game online

    /*public Text quitInfo;
    public void btnQuit_online() // ouvrir le menu quit 
    {
        //nbTrLooseIfQuit = menuRankingGameOnline.getInstance().getNbTrLooseIfQuit();
        // ouverture du menu quit
        quitInfo.text = menuRankingGameOnline.getInstance().getNbTrLooseIfQuit().ToString();
        menuValidQuitOnlineGame.SetActive(true);
        StartCoroutine(closeMenuQuitTimer());
    }
    private IEnumerator closeMenuQuitTimer()// fermer menu tout seul au bout de x sec

    {
        float t = 0f;
        while (menuValidQuitOnlineGame.activeSelf
            && t < 5f)
        {
            yield return null;
            t += Time.deltaTime;
        }
        if(menuValidQuitOnlineGame.activeSelf)
            closeMenuQuit(false);
    }
    private void closeMenuQuit(bool quitGame)
    {
        // fermeture du menu quit
        StartCoroutine(closeDelay());
        quitOnlineValidated = quitGame;
        if (quitGame)
        {
            playerLeavesTheGame();
        }
    }
    private IEnumerator closeDelay() // eviter miss clique sur map
    {
        yield return new WaitForSeconds(0.1f);
        menuValidQuitOnlineGame.SetActive(false);
    }
    public void playerLeavesTheGame() // pour ne pas être actif OU choisit quitter
    {
        checkPlayerIsPlaying.getInstance().end_CheckingPlayerIsPlaying();
        quitOnlineValidated = true;
        menuRankingGameOnline.stayInRoom = false;
        appliManager.instance.quitGame_online();
        // avertir les autres joueurs
        CustomMatchmackingRoomController.quitRoom();
        quitOnlineValidated = false;
        //btnChangeRot.SetActive(false);
        tropheOnlineManager.getInstance().changeNbTrophe(
            menuRankingGameOnline.getInstance().getNbTrLooseIfQuit(), true);
    }
    public void confirmQuit_online()
    {
        closeMenuQuit(true);
    }
    public void defyQuit_online()// deny
    {
        closeMenuQuit(false);
    }*/

    #endregion

    /*
     * List<List<int>> generateGameLevels()
     * List<int> chooseNewPatern_initOnlineList(int lvl)
    */
    #region creation de la liste partagée des niveaux

    List<GameObject> prevPaterns_online = new List<GameObject>();
    public List<List<int>> generateGameLevels() // appelé par le clientMaster
    {
        GameObject currentPatern_copy = currentPatern;

        prevLvlBonusApparition = 0;
        prevPaterns_online = new List<GameObject>();
        usedPatern = new List<int[]>();

        List<List<int>> retour = new List<List<int>>();

        for (int i = 0; i < nbLevelInfinis_online; i++)
        {
            retour.Add(chooseNewPatern_initOnlineList(i));
        }

        prevPaterns_online = null;
        currentPatern = currentPatern_copy;
        prevLvlBonusApparition = 0;
        bonusPhase = false;
        usedPatern = null;

        //Debug.Log("Creation de la liste : succed");
        return retour;

    }

    // bonus - index patern - index combi
    private List<int[]> usedPatern = new List<int[]>(); // seulement utilisé dans func
    private List<int> chooseNewPatern_initOnlineList(int curScore)
    {
        // int bonusPhase , int indexInParent, int mirror, int paternChildIndex
        List<int> ret = new List<int>();

        /* regarder si phase de bonus
        if (curScore - prevLvlBonusApparition > rangeMinBetweenLvlBonus_online
            && probLvlBonus >= Random.Range(0f, 1f))
        {
            prevLvlBonusApparition = curScore;
            bonusPhase = true;
        }
        else
            bonusPhase = false;*/

        bonusPhase = false; // jamais en mode online

        int boostMode = bonusPhase ? 1 : 0;
        ret.Add(boostMode);

        GameObject[] l = bonusPhase == true ? test_lesPaternBonus : test_lesPatern;

        int i = Random.Range(0, l.Length);

        while (l[i].GetComponent<packNiveau_infinis>().useInOnlineMode == false
            || prevPaterns_online.Contains(l[i]))
            i = Random.Range(0, l.Length);

        ret.Add(i);

        if (prevPaterns_online.Count == nbPaternToBeReusable_online)
            prevPaterns_online.RemoveAt(0);

        currentPatern = l[i];
        prevPaterns_online.Add(currentPatern);

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            //mirroirNiveau(currentPatern.transform);
            ret.Add(1);
        }
        else
        {
            ret.Add(0);
        }

        GameObject[] lesCombi = currentPatern.GetComponent<packNiveau_infinis>().lesCombi;
        int paternChildIndex = Random.Range(0, lesCombi.Length);
        int newCI = paternChildIndex;
        if (!canUseThisCombiIndex(boostMode, i, paternChildIndex))
        {
            newCI = Random.Range(0, lesCombi.Length);
            while (newCI == paternChildIndex)
                newCI = Random.Range(0, lesCombi.Length);
        }
        ret.Add(newCI);
        usedPatern.Add(new int[] { bonusPhase ? 1 : 0, i, paternChildIndex });

        return ret;
    }

    private bool canUseThisCombiIndex(int boostMode, int paternIndex, int testCombiIndex)
    {
        int i = 0;
        foreach (int[] l in usedPatern)
        {
            if (l[0] == boostMode && l[1] == paternIndex)
            {
                if (l[2] == testCombiIndex)
                    return false;
                else
                {
                    usedPatern[i][2] = testCombiIndex;
                    return true;
                }
            }
            i++;
        }
        return true;
    }
    #endregion

    #endregion
}
