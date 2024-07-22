using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class managerNiveau : MonoBehaviour
{
    public difficultyBar _difficultyBar;

    public crownProgBar cProgBarMenuNiveau;
    public GameObject star;
    public GameObject starImgY;

    public Text numeroNivText;
    public GameObject notifLevelUnlocked;
    public Text nbCrownUnlockText;
    public Text couronneText;
    public Transform[] lesNiveaux;
    public static int currentNiveau = -1;
    public static GameObject currentNiveauGo;
    public static int[] records;

    private int numeroNiveau = 1;

    public void eventHappened_animGlow(Color _colorEffect)
    {
        currentNiveauGo.GetComponent<packNiveau>().glowEffectIngame(_colorEffect);
    }

    public static managerNiveau instance;
    public static managerNiveau getInstance()
    {
        return instance;
    }

    public ScrollRectSnap_CS scrollManager;
    //private static bool useScrollV2 = true;

   



    float temps = 0;
    //bool waiting = false;
    // utilisé par shopManager quand swipe
    public static float dragDistance;  //minimum distance for a swipe to be registered
                                       /*
                                        * void Start()
                                        * IEnumerator waitBeforeStart()
                                       */
    #region initialisation


    //private bool managerInitializedOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        //if(!managerInitializedOnStart)
            StartCoroutine(StartRoutine());
    }

    public float timeWaitInit_backUpReward = 1f;

    private IEnumerator StartRoutine()
    {
        //Debug.Log("<color=red>StartRoutine start</color>");

        dragDistance = Screen.height * 15 / 100;

        instance = GameObject.Find("managerNiveau").GetComponent<managerNiveau>();
        setCompteurCouronne();

        while (lockInfinis.getInstance() == null)
            yield return null;

        foreach (Transform t in lesNiveaux)
        {
            t.GetComponent<packNiveau>().initNiveauGame();
        }

        lockInfinis.getInstance().checkVerouillage(SaveSystem.nb_couronne, true);
        lockStar.getInstance().checkVerouillage(SaveSystem.nb_couronne, true);
        /*lockInfinis.getInstance_online().checkVerouillage(
            (int)SaveSystem.loadData(typeSave.RECORD_INFINIS), true);*/

        starObtain = (bool[])SaveSystem.loadData(typeSave.STAR);


        /* TEST
        
        starObtain = new bool[78];
        for (int i = 0; i < 78; i++)
        {
            starObtain[i] = i < 75 ? true : false;
        }
        SaveSystem.Save(typeSave.STAR, starObtain);
        */

        int nbStarObtain = SaveSystem.getNbStarObtain();
        setCompteurStar();
        int nivActuel = 0;
      
            // si pas zero couronnes
            if (SaveSystem.nb_couronne != 0)
            {
                // on recherche quel est le niveau actuel
                nivActuel = loadNewLevel(-1, nbStarObtain) + 1;
            }
            // sinon 
            else
            {
                // niveau courant est le premier
                nivActuel = 1;
                numeroNiveau = 1;
                numeroNivText.text = "1";
                currentNiveauGo = lesNiveaux[0].gameObject;
            }

            //checkLock_V2();

            scrollManager.initGame_void(nivActuel);
            convertRangLevel_toNextDizaine(true, nivActuel);
            // on dit à scrollView de load le level init
            //StartCoroutine(scrollManager.initGame_enum(nivActuel));
            levelIsCentered(nivActuel);
        
        /*else
        {
            if (SaveSystem.nb_couronne != 0)
            {
                loadNewLevel(-1);
            }
            else
            {
                btnFirst.SetActive(false);

                numeroNiveau = 1;
                numeroNivText.text = "1";

                currentNiveauGo = lesNiveaux[0].gameObject;
                currentNiveauGo.SetActive(true);
            }

            updateTextLast_First();
            currentNiveauGo.GetComponent<Animation>().Play("switchLightLevel");
        }*/

        _difficultyBar.initBar(
            lesNiveaux[currentNiveau].GetComponent<packNiveau>().levelDifficulty
            ,lesNiveaux[currentNiveau].GetComponent<packNiveau>().type
            ,nivActuel);

        yield return new WaitForSeconds(timeWaitInit_backUpReward);
        ensurePurchaseReward.getInstance().backUpReward();
        //Debug.Log("<color=red>StartRoutine end</color>");
    }

    public IEnumerator waitBeforeStart()
    {
        //waiting = true;
        temps = 0.3f;
        while (temps >= 0)
        {
            temps -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //waiting = false;
    }
    #endregion

    private bool firstB, lastB; // enregistrer leurs etat avant de passer au mode infinis
    public static bool gameOn = false;
    public bool[] starObtain;
    public GameObject monneyGo;
    /*
     * void openFromInfiniteMode()
     * void closeForInfiniteMode()
     * IEnumerator lauchMenu(bool loadApp, bool fromTuto)
     * IEnumerator gameStart(bool goInfinite)
     * bool obtainStar()
     * IEnumerator waitAnim()
     * void setCompteurCouronne()
     * void setCrownCompteurText(int value)
    */
    #region gestion menu / lancement jeu
    public void openFromInfiniteMode()
    {
        btnFirst.SetActive(firstB);
        btnLast.SetActive(lastB);
        lauchMenu(true, false, false, true);
        cProgBarMenuNiveau.reOpenBar();
        scrollManager.openLevelsFromInfinite();
    }
    public void closeForInfiniteMode()
    {
        firstB = btnFirst.activeSelf;
        lastB = btnLast.activeSelf;

        StartCoroutine(cProgBarMenuNiveau.closeBar());
        StartCoroutine(gameStart(true)); // nettoyer les ui
        scrollManager.closeLevelsToInfinite();
    }

    public void gameEnd_reopenLevels()
    {
        scrollManager.gameEnd_reopenLevels();
    }

    public void gameEnd_allRewardGiven()
    {
        scrollManager.gameEnd_allRewardGiven();
        lesNiveaux[currentNiveau].GetComponent<Animation>().Play("switchLightLevel");
    }

    public void lauchMenu(bool loadApp, bool fromTuto, bool fromGame, bool fromInfinis)
    {
        //Debug.Log("<color=red>Launch menu</color>");
        //if (useScrollV2)
        //{
            // ET que ne vient pas du mode infinis
            if (loadApp && !fromInfinis)
            {
                checkLock_V2();
                updateTextLast_First();
            }
            if (records == null)
                records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);
            /* initialiser records quand start
            if (records == null)
                records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);*/
            if (cProgBarMenuNiveau.transform.childCount == 0) // sinon,  gameManager affiche la precedente
                cProgBarMenuNiveau.loadBar(
                        records[currentNiveau]);

            /*if (fromTuto)
            {
                infoMenuManager.playerHasDoneTuto(typeOpenInfo.tuto_aventure, 0.4f);
            }*/

            // PB : quand loadNextLevel après victoire et finit niveau
            // left et right pop après reccup crown et etoile, MAIS load appelé en meme temps : pas d'anim pop
            // faire pop avant de donner rec ( sans activer btn gauche et droit ), cash à fermeture mode jeu
            //if (fromGame && !infinisManager.infinis_Mode_On)
            //    scrollManager.gameEnd_reopenLevels();

            if (goNextLevel)
            {
                scrollManager.levelFinished_goNext();
                goNextLevel = false;
            }
        //}
    }

    public IEnumerator gameStart(bool goInfinite)
    {
        //if (useScrollV2)
        //{
            if (!goInfinite)
            {
                lesNiveaux[currentNiveau].GetComponent<Animation>().Play("switchLightGameStart");
                scrollManager.btnStartClicked();
            }
            if (notifLevelOn)
            {
                notifLevelOn = false;
                notifLevelUnlocked.GetComponent<Animation>().Play("notifNewLevelClose");
            }

            yield return new WaitForSeconds(1f / 6f);

            if (!goInfinite)
                StartCoroutine(cProgBarMenuNiveau.closeBar());

            //if(!goInfinite)
            //    sfxManager.getInstance().playSound_ingame(typeSound_ingame.zoom_level);
        //}
    }
    public bool obtainStar()
    {
        bool ret = false;
        if (starObtain[currentNiveau] != true)
        {
            starImgY.SetActive(true);
            starObtain[currentNiveau] = true;
            SaveSystem.playerGetStar();

            SaveSystem.Save(typeSave.STAR, starObtain);
            star.GetComponent<Animation>().Play("starObtain");

            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.win_star);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.obtain_star);
            //managerAudio.playSound(typeSon.STAR);

            ret = true;
            StartCoroutine(waitAnim());
            //checkLock_V2();
            
        }
        return ret;
    }

    public Transform menuPCanvas;
    public Transform posSpawnRecStar;
    private IEnumerator waitAnim()
    {
        yield return new WaitForSeconds(getStarMenuP.timeAnimGetStar - 22 / 60f);
        defiManager.getInstance().defiIsIncremented(typeDefi.star, 1);
        setCompteurStar();
        //yield return new WaitForSeconds(22 / 60f);

        
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, 5, posSpawnRecStar.position);
        
    }
    private void setCompteurCouronne()
    {
        records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);
        if (records != null)
        {
            int tot = 0;
            for (int i = 0; i < records.Length; i++)
            {
                tot += records[i];
            }

            setCrownCompteurText(tot);
            SaveSystem.nb_couronne = tot;
        }
        else
        {
            setCrownCompteurText(0);
            SaveSystem.nb_couronne = 0;
        }

    }
    private void setCrownCompteurText(int value)
    {
        instance.couronneText.text = appliManager.intTo4DigitString(value, 3);
    }

    public Text compteurStar_ombre, compteurStar;
    public RectTransform compteurRectTr, compteurModeOver100;
    private bool has100StarOrMore = false;
    private void setCompteurStar()
    {
        int nbStar_int = SaveSystem.getNbStarObtain();
        string nbStar = nbStar_int.ToString();
        compteurStar.text = nbStar;
        compteurStar_ombre.text = nbStar;

        if(nbStar_int >= 100 && !has100StarOrMore)
        {
            has100StarOrMore = true;
            compteurRectTr.localScale = compteurModeOver100.localScale;
            compteurRectTr.localPosition = compteurModeOver100.localPosition;
        }
    }


    #endregion


    public Animation animSwapNumLevel;
    public GameObject btnFirst;
    public GameObject btnLast;
    public GameObject textNumLevel;
    public Text textLast, textFirst;
    public static bool goingBound = false; // eviter clickShot
    private int prevCrown = -1;
    private int prevStar = -1;
    private int maxi_Level = 80; // 72 - 1

    /*
     * REGION bouton Droite et Gauche
     * REGION update value niv after switch
     * REGION go bounded levels
    */
    #region Change Level

    /*
     * void btnD()
     * void btnG()
     * IEnumerator btn(bool droite)
     * IEnumerator closeLvl(GameObject toClose, bool checkLok)
    */
    #region Bouton droite et gauche
    public void btnD()
    {
        btn_V2(true);
    }

    public void btnG()
    {
        btn_V2(false);
    }

    public void btn_V2(bool droite)
    {
        int i = droite ? 1 : -1;
        swapInfoLevel(currentNiveau + i);
    }

    // appelé pendant scroll quand level va zone milieu
    public void levelIsCentered(int numLevel)
    {
        swapInfoLevel(numLevel - 1);
    }

    private void swapInfoLevel(int indexLevel)
    {
        // eviter double call
        if (currentNiveau == indexLevel)
            return;

        //if (indexLevel < 0) indexLevel = 0;
        // Debug.Log("Level centré : " + indexLevel+" (num:"+ (indexLevel + 1)+")");

        currentNiveauGo = lesNiveaux[indexLevel].gameObject;

        cProgBarMenuNiveau.changeBarValues(
            records[indexLevel]);

        if (starObtain[indexLevel] == true)
            starImgY.SetActive(true);
        else if (starImgY.activeSelf) starImgY.SetActive(false);

        numeroNivText.text = (indexLevel + 1).ToString();

        changeDifficulty(lesNiveaux[indexLevel].gameObject, indexLevel + 1);

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_btnGaucheD);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);

        currentNiveau = indexLevel;

        //Debug.Log("<color=blue>Btn last activated : " + (currentNiveau < maxi_Level - 3) + "</color> (maxi_level:"+maxi_Level+")");
        //Debug.Log("current num level : " + indexLevel);
        btnFirst.SetActive(currentNiveau > 2);
        btnLast.SetActive(currentNiveau < maxi_Level - 3);

        updateTextLast_First();
    }

    // appelé par scrollItem quand close
    // appelé par static scrollItem quand switch
    public void closeLvl_V2(GameObject toClose)
    {
        if (toClose.GetComponent<packNiveau>().currentLevel != 0)
        {
            toClose.GetComponent<packNiveau>().partieFinit();
        }
    }
    #endregion

    /*
     * void changeDifficulty(GameObject level)
     * void updateTextLast_First()
     * int loadNewLevel(int index)
     * IEnumerator checkLock_V2()
     */
    #region update value niv after switch
    private void changeDifficulty(GameObject level, int numLevel)
    {
        int d = level.GetComponent<packNiveau>().levelDifficulty;
        typeLevel t = level.GetComponent<packNiveau>().type;
        _difficultyBar.setInfos(d, t, numLevel);
    }

    
    // /!\ doit aussi être appelé à l'initialisation quand set num currentLevel
    public void updateTextLast_First()
    {
        //Debug.Log("<color=yellow>Set set btn first and last</color>");
        int i = getBoundRangLevel(false);
        textLast.text = (1 + i).ToString();

        i = getBoundRangLevel(true);
        //Debug.Log("Change text first : " + (1 + i));
        textFirst.text = (1 + i).ToString();
    }

    public int loadNewLevel(int index, int nbStarObtain) // met a jour le nouveau niveau     // appelé au lancement du jeu
    {
        
        if (index == -1) // si pas clique sur notif new level : aller dernier lvl
        {
            //Debug.Log("currentNiveau : " + currentNiveau);
            if (currentNiveau == -1)
                currentNiveau = 0;
            while (currentNiveau < lesNiveaux.Length
                && lesNiveaux[currentNiveau].GetComponent<packNiveau>().crownToUnlock <= SaveSystem.nb_couronne
                && lesNiveaux[currentNiveau].GetComponent<packNiveau>().starToUnlock <= nbStarObtain)
                currentNiveau++;
            currentNiveau -= 1;

            btnLast.SetActive(false);
        }
        else
        {
            currentNiveau = index;
        }

        numeroNiveau = currentNiveau + 1;
        numeroNivText.text = numeroNiveau.ToString();

        if (starObtain[currentNiveau]) starImgY.SetActive(true);
        else if (starImgY.activeSelf) starImgY.SetActive(false);
        currentNiveauGo = lesNiveaux[currentNiveau].gameObject;

        //if (!useScrollV2)
        //    currentNiveauGo.SetActive(true);


        return currentNiveau;
    }

    public static int nbCrownRequired_unlockLevel;
    public static int nbStarRequired_unlockLevel;
    public void checkLock_V2()
    {
        
        int myCrownTotal = SaveSystem.nb_couronne;
        int index = 0;
        int indexLimit = 0; 
        Transform lvlLimitTest = lesNiveaux[index];
        packNiveau p = lvlLimitTest.GetComponent<packNiveau>();
        while (index < lesNiveaux.Length 
            && p.crownToUnlock <= myCrownTotal
            && p.starToUnlock <= SaveSystem.getNbStarObtain())
        {
            index++;
            if (index < lesNiveaux.Length)
            {
                lvlLimitTest = lesNiveaux[index];
                p = lvlLimitTest.GetComponent<packNiveau>();
                indexLimit++;
            }
        }
        if (index == lesNiveaux.Length)
        {
            indexLimit = 1000;
            nbCrownRequired_unlockLevel = 1000;
            nbStarRequired_unlockLevel = 0;
        }
        else
        {
            nbCrownRequired_unlockLevel = lvlLimitTest.GetComponent<packNiveau>().crownToUnlock;
            nbStarRequired_unlockLevel = lvlLimitTest.GetComponent<packNiveau>().starToUnlock;
        }

        // BUG , si est executé fin de game, avant notif lock
        // => level unlocked devient new locked et click notif emmene sur limite
        scrollManager.setLockNum(indexLimit + 1);
        //scrollManager.setLockNum(indexLimit + 1);
    }

    #endregion

    /*
     * void btnLastLvl_V2()
     * void btnFirstLvl_V2()
     * int getBoundRangLevel(bool down)
     * int convertRangLevel_toNextDizaine(bool down, int rangLevel)
     * IEnumerator void goBoundedLevel_V2(int index)
    */
    #region go Bounded Level

    public void btnLastLvl_V2()
    {
        if (!ScrollRectSnap_CS.isSwitching)
        {
            int indexBound = getBoundRangLevel(false);
            goBoundedLevel_V2(indexBound);
            scrollManager.goToBoundedLevel(indexBound + 1);
        }
    }
    public void btnFirstLvl_V2()
    {
        if (!ScrollRectSnap_CS.isSwitching)
        {
            int indexBound = getBoundRangLevel(true);
            goBoundedLevel_V2(indexBound);
            scrollManager.goToBoundedLevel(indexBound + 1);
        }
    }

    public void menuVisu_forceGoLevel(int numLevel)
    {
        goBoundedLevel_V2(numLevel - 1);
        scrollManager.goToBoundedLevel(numLevel);
    }

    private void goBoundedLevel_V2(int index)
    {
        btnFirst.SetActive(false);
        btnLast.SetActive(false);

        //Debug.Log("Call sound swipe, go bounded level");
        //sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_btnBounds);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);

        swapInfoLevel(index);

        if (index == 0)
        {
            btnLast.SetActive(true);
        }
        else if (index == lesNiveaux.Length - 1) // last
        {
            btnFirst.SetActive(true);
        }
        else
        {
            btnFirst.SetActive(true);

            //Debug.Log("index level desired : " + index + " maxi_level : "+maxi_Level);
            if (index < maxi_Level - 3)
            {
                btnLast.SetActive(true);
            }
        }
    }

    private int getBoundRangLevel(bool down)
    {
        return convertRangLevel_toNextDizaine(down, currentNiveau);
    }

    private int convertRangLevel_toNextDizaine(bool down, int rangLevel)
    {
        // recalculer max last
        if (SaveSystem.nb_couronne != prevCrown
            || SaveSystem.getNbStarObtain() != prevStar)
        {
            maxi_Level = 0;
            while (maxi_Level < lesNiveaux.Length
               && lesNiveaux[maxi_Level].GetComponent<packNiveau>().crownToUnlock <= SaveSystem.nb_couronne
               && lesNiveaux[maxi_Level].GetComponent<packNiveau>().starToUnlock <= SaveSystem.getNbStarObtain())
                maxi_Level++;

           prevCrown = SaveSystem.nb_couronne;
        }

        int retour = rangLevel + 1; // on commence à 1

        // on se rapproche de la dizaine la plus proche
        if (down)
        {
            do
            {
                retour--;
            }
            while (retour > 1 && retour % 10 != 0);
        }
        else
        {
            do
            {
                retour++;
            }
            while (retour < maxi_Level && retour % 10 != 0);
        }

        return retour -= 1; // on remet au format de base ( commence à 0 )
    }
  
    #endregion

    #endregion


    public bool click = false;
    private int newLevelIndex;
    /*
     * void clickNotifNewLvl()
     * void checkNewLevel(int newC, int prevC, bool lvlClean)
     * IEnumerator openNotifNewLevels()
    */
    #region notification nouveaux niveaux
    public void clickNotifNewLvl()
    {
        //if (useScrollV2)
        //{
            scrollManager.btnGoToUnlockedLevel();
            notifLevelOn = false;
            notifLevelUnlocked.GetComponent<Animation>().Play("notifNewLevelClose");
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.btn_goUnlock);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_infoUI);
            goBoundedLevel_V2(newLevelIndex);
        //}  
    }

    public void checkNewLevel(int newC, int prevC, bool lvlClean
        , int newS, int prevS) // après avoir reccupéré toute les couronnes et etoile
    {
        newLevelIndex = 0;
        foreach (Transform pack in lesNiveaux)
        {
            int packC = pack.GetComponent<packNiveau>().crownToUnlock;
            int packS = pack.GetComponent<packNiveau>().starToUnlock;
            //int nbStar = SaveSystem.getNbStarObtain();

            bool hasNecessaryCrown = packC <= newC;
            bool hasNecessaryStar = packS <= newS;
            bool justPassedNecessaryCrown = hasNecessaryCrown && packC > prevC;
            bool justPassedNecessaryStar = hasNecessaryStar && packS > prevS;

            bool aLevelIsUnlocked = justPassedNecessaryCrown && hasNecessaryStar
                || justPassedNecessaryStar && hasNecessaryCrown;


            if (aLevelIsUnlocked)
            {
                sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.unlock_level);
                //sfxManager.getInstance().playSound_menuP(typeSound_menuP.new_level_unlocked);
                StartCoroutine(openNotifNewLevels());

                // si vient de debloquer un level qui se lock par star, ouvrir notif menuVisu
                if (packS > 0)
                    menuVisuLevel.getInstance().menuPLevelStarUnlocked();


                scrollManager.levelIsUnlocked(newLevelIndex + 1);
                checkLock_V2();
                scrollManager.resetTextLock();


                updateTextLast_First();

                // si avant btn last pas activé, devrait être activé
                if(!btnLast.activeSelf)
                    btnLast.SetActive(true);

                return;
            }
            else if(justPassedNecessaryCrown || justPassedNecessaryStar)
            {
                // si level n'est pas unlock MAIS
                // - locked par star ET crown
                // - update voyant valid de star et crown directement
                scrollManager.updateTextLock();
            }
            newLevelIndex++;
        }
        if (lvlClean && newC != prevC/*&& btnDroite.activeSelf*/)
            goNextLevel = true;
    }

    private static bool notifLevelOn = false;
    private float timeNotif = 4.5f;
    private IEnumerator openNotifNewLevels()
    {
        notifLevelOn = true;
        notifLevelUnlocked.GetComponent<Animation>().Play("notifNewLevelOpen");
        yield return new WaitForSeconds(timeNotif);
        if (notifLevelOn)
        {
            notifLevelUnlocked.GetComponent<Animation>().Play("notifNewLevelClose");
            notifLevelOn = false;
        }
    }
    #endregion


    bool goNextLevel = false; // quand les recompense de game on été donné, aller lvl suivant si goNextLevel
    /*
     * void incrementeCrown()
     * IEnumerator animCrown()
    */
    #region gestion affichage couronne / get couronne
    public void incrementeCrown()
    {
        SaveSystem.nb_couronne ++;
        lockInfinis.getInstance().checkVerouillage(SaveSystem.nb_couronne, false);
        lockStar.getInstance().checkVerouillage(SaveSystem.nb_couronne, false);
        StartCoroutine(animCrown());
    }

    public IEnumerator animCrown()
    {
        couronneText.transform.parent.GetComponent<Animation>().Play("getCrown");
        yield return new WaitForSeconds(7f / 60f);
        setCrownCompteurText(SaveSystem.nb_couronne);
    }
    #endregion
}
