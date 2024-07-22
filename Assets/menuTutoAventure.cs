using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum typeTutoAventure
{
    none,
    crown,
    bounce,
    pivot,
    portal,

    push,
    slow,
    checkP,
    switchR,
    reset
}

public class menuTutoAventure : MonoBehaviour
{
    private Dictionary<typeTutoAventure, string> nameAnim = new Dictionary<typeTutoAventure, string>() {
        { typeTutoAventure.bounce, "aideAventureBounce" },
        { typeTutoAventure.pivot, "aideAventurePivot" },
        { typeTutoAventure.portal, "aideAventurePortail" },
        { typeTutoAventure.push, "aideTutoPush" },
        { typeTutoAventure.slow, "aideTutoSlow" },
        { typeTutoAventure.checkP, "aideTutoCheckP" },
        { typeTutoAventure.switchR, "aideTutoSwitch" },
        { typeTutoAventure.reset, "aideTutoReset" },
    };
    // si a + de couronnes, alors ne même pas essayer montrer tuto
    private Dictionary<typeTutoAventure, int> crownMaxForTuto = new Dictionary<typeTutoAventure, int>() {
        { typeTutoAventure.bounce, 9 },
        { typeTutoAventure.pivot, 17 },
        { typeTutoAventure.portal, 25 },
        { typeTutoAventure.push,3 },
        { typeTutoAventure.slow, 3 },
        { typeTutoAventure.checkP, 9 },
        { typeTutoAventure.switchR, 17 },
    };
    // permet de savoir quel est l'index DANS la save du typeTuto
    private Dictionary<typeTutoAventure, int> indexSaveTuto = new Dictionary<typeTutoAventure, int>() {
        { typeTutoAventure.bounce, 3 },
        { typeTutoAventure.pivot, 5 },
        { typeTutoAventure.portal, 6 },
        { typeTutoAventure.push,0 },
        { typeTutoAventure.slow, 1 },
        { typeTutoAventure.checkP, 2 },
        { typeTutoAventure.switchR, 4 },
    };
    private Dictionary<int, typeTutoAventure> indexSaveTuto_reverse = new Dictionary<int, typeTutoAventure>() {
        { 3, typeTutoAventure.bounce },
        { 5, typeTutoAventure.pivot },
        { 6, typeTutoAventure.portal },
        { 0, typeTutoAventure.push },
        { 1, typeTutoAventure.slow },
        { 2, typeTutoAventure.checkP },
        { 4, typeTutoAventure.switchR },
    };
    private List<typeTutoAventure> listTutoLockAnim = new List<typeTutoAventure>() {
          typeTutoAventure.push
        , typeTutoAventure.slow
        , typeTutoAventure.checkP
        , typeTutoAventure.switchR
    };
    private Dictionary<typeTutoAventure, int> timeAnim_tuto = new Dictionary<typeTutoAventure, int>() {
        { typeTutoAventure.push, 302 },
        { typeTutoAventure.slow, 395 },
        { typeTutoAventure.checkP, 315 },
        { typeTutoAventure.switchR, 378 },
    };

    public int nbLevelTutoAventure = 6;
    public int nbLifeInitLevelTuto = 4;
    public float timeBetweenZoomOut_openTuto = 0.3f;

    public TutoManager tutoM;
    public gameManager gameM;
    public menuPause pauseM;
    public Animation aideAnim;
    public GameObject btnInfoTutoGo;
    public Animation menuAnim;
    public Text infoText;
    public detectorClickWhileTutoAnim detectorClickOnAnimGo;

    private List<bool> listTutoAlreadySeen = new List<bool>();
    private typeTutoAventure currentTuto;

    public void playerReplay()
    {
        // on doit load tuto en fonction lvl 1
        packNiveau p = managerNiveau.currentNiveauGo.GetComponent<packNiveau>();

        if (p.isALevelTuto)
        {
            currentTuto = p.getTypeTutoByLevel();
            // si est bien sur un level ( du niveau ) qui a un tuto
            btnInfoTutoGo.SetActive(currentTuto != typeTutoAventure.none);
            //tutoIsBetweenLevel = late;
        }
    }

    // revoir variable, ne doit pas dire si cest un tuto entre level MAIS si le menu est ouvert en mode 'entre 2 level'
    //private bool menuOpennedBetweenLevel;


    public void modeInfinisStarted()
    {
        if (btnInfoTutoGo.activeSelf)
        {
            btnInfoTutoGo.SetActive(false);
        }
    }

    /*
     * bool tryOpenTuto(int nbCrown, GameObject currentLevelGo, bool late)
     * IEnumerator tryOpenTuto_late(typeTutoAventure typeT)
     * void openTutoAventure(typeTutoAventure type, bool calledByInfoButton)
     * void btnClose()
    */
    #region GESTION ouverture / fermeture du menuTuto
    // si 'late=true', ca veut dire qu'on ouvre le tuto entre 2 level du meme niveau
    public bool tryOpenTuto(int nbCrown, GameObject currentLevelGo, bool late)
    {
        // cette fonction est appelé quand
        // - ouvre level
        // - gagne niveau du level

        // on doit regarder si ce niveau spécifique est un niveau tuto 
        // si oui : enable btn '?' pour voir le tuto du level
        packNiveau p = currentLevelGo.GetComponent<packNiveau>();
        //menuOpennedBetweenLevel = false;

        if (p.isALevelTuto)
        {
            currentTuto = p.getTypeTutoByLevel();
            // si est bien sur un level ( du niveau ) qui a un tuto
            btnInfoTutoGo.SetActive(currentTuto != typeTutoAventure.none);
            //tutoIsBetweenLevel = late;
        }else if (btnInfoTutoGo.activeSelf)
        {
            btnInfoTutoGo.SetActive(false);
        }

        if (listTutoAlreadySeen.Count == 0)
        {
            // on load save
            listTutoAlreadySeen = (List<bool>)SaveSystem.loadData(typeSave.TUTO_MENUP);

            // si la save est corompu, ajouter les tutos manquants dans le fichier save
            while(listTutoAlreadySeen.Count != 7)
            {
                typeTutoAventure type = indexSaveTuto_reverse[listTutoAlreadySeen.Count];
                listTutoAlreadySeen.Add(nbCrown > crownMaxForTuto[type]);
            }
        }
        if (listTutoAlreadySeen == new List<bool>() { true, true, true, true, true, true, true })
            return false;

        if (p.isALevelTuto) {
            // on regarde si les animations sont disponibles ( pas trop de crown )
            if (currentTuto != typeTutoAventure.none
                && nbCrown < crownMaxForTuto[currentTuto])
            {
                // si a déjà fait le tuto, ne rien faire
                if (listTutoAlreadySeen[indexSaveTuto[currentTuto]] == true)
                    return false;

                if (late)
                {
                    //menuOpennedBetweenLevel = true;
                    StartCoroutine(tryOpenTuto_late(currentTuto));
                }
                else
                {
                    StartCoroutine(openTutoAventure(currentTuto, false));
                    listTutoAlreadySeen[indexSaveTuto[currentTuto]] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                }
                return true;
            }
            else
            {
                // ne rien faire
                return false;
            }
        }
        return false;
    }

    // appelé seulement si tuto entre 2 levels
    private IEnumerator tryOpenTuto_late(typeTutoAventure typeT)
    {
        yield return new WaitForSeconds(timeBetweenZoomOut_openTuto);
        StartCoroutine(openTutoAventure(typeT, false));
        listTutoAlreadySeen[indexSaveTuto[typeT]] = true;
        SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
    }

    private bool isInRightMode = true;
    public GameObject progBarWaitGo;
    public float timeOpenToStartAnim = 1f;
    private IEnumerator openTutoAventure(typeTutoAventure type, bool calledByInfoButton)
    {
        //bool auto = !calledByInfoButton && listTutoLockAnim.Contains(type);
        //menuAnim.Play(auto ? "tutoAventureOpen_auto" : "tutoAventureOpen");        
        infoText.text = traductionManager.getInstance().getTradInfoTutoAventure(type);
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);

        // /!\ pas si appelé entre 2 levels ( géré tt seul )
        //if(!menuOpennedBetweenLevel)
        //    animFond3DGame.getInstance().menuPauseOpen();

        if(!calledByInfoButton)
        {
            aideAnim.Play(nameAnim[type]);
            aideAnim[nameAnim[type]].speed = 0;

            menuAnim.Play("tutoAventureOpen_auto");

            bool waitEndAnim = listTutoLockAnim.Contains(type);
            progBarWaitGo.SetActive(waitEndAnim);
            panLockAnimGo.SetActive(waitEndAnim);
            if (waitEndAnim)
            {
                waitAnim_anim.Play("waitAnimEndOpen");
                progBar.fillAmount = 1 - minBarNoAnim;
            }

            yield return new WaitForSeconds(timeOpenToStartAnim);

            if (waitEndAnim)
                StartCoroutine(lockAnim(type));

            aideAnim[nameAnim[type]].speed = 1;
        }
        else
        {
            aideAnim.Play(nameAnim[type]);
            menuAnim.Play("tutoAventureOpen");
            progBarWaitGo.SetActive(false);
        }

        /*if (auto)
        {
            progBarWaitGo.SetActive(true);
            yield return new WaitForSeconds(timeOpenToStartAnim);
            StartCoroutine(lockAnim(type));
        }
        else
        {
            progBarWaitGo.SetActive(false);
        }*/



        // set gaucher droitier
        if (manetteManager.manetteModeRightHandled != isInRightMode)
        {
            isInRightMode = !isInRightMode;
            contentSideAnim.localScale *= new Vector2( -1, 1);
            btnBImg.localScale *= new Vector2( -1, 1);
        }
    }

    public void btnClose()
    {
        if (animLocked)
            return;

        menuAnim.Play("tutoAventureClose");
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);

        // si pas entre 2 levels ( géré tt seul )
        //if (!menuOpennedBetweenLevel)
        //    animFond3DGame.getInstance().menuPauseClose();
        //menuOpennedBetweenLevel = false;

        if (modeInfoOpenned)
        {
            btnInfoTuto_close();
        }
        else
        {
            if (gameManager.isWaitingTutoBetweenLevel) gameManager.isWaitingTutoBetweenLevel = false;
            else
                gameM.StartCoroutine(gameM.loadGame_passTuto(true));
        }
    }
    #endregion

    public bool canDisplayBtnB()
    {
        if (SaveSystem.nb_couronne > crownMaxForTuto[typeTutoAventure.slow])
            return true;

        if (listTutoAlreadySeen.Count == 0)
        {
            listTutoAlreadySeen = (List<bool>)SaveSystem.loadData(typeSave.TUTO_MENUP);
        }
        return listTutoAlreadySeen[indexSaveTuto[typeTutoAventure.slow]];
    }

    private bool switchUnlocked = false;
    public bool canDisplaySwitchBtnA()
    {
        if (switchUnlocked) return true;
        else
        {
            if (SaveSystem.nb_couronne > crownMaxForTuto[typeTutoAventure.switchR])
            {
                switchUnlocked = true;
                //Debug.Log("Too much crown, enable automaticaly switch A");
                return true;
            }

            if (listTutoAlreadySeen.Count == 0)
            {
                listTutoAlreadySeen = (List<bool>)SaveSystem.loadData(typeSave.TUTO_MENUP);
            }
            //Debug.Log("Tuto switchRot deja fait ? : " + listTutoAlreadySeen[indexSaveTuto[typeTutoAventure.switchR]]);
            switchUnlocked = listTutoAlreadySeen[indexSaveTuto[typeTutoAventure.switchR]];
            return switchUnlocked;
        }
    }

    /*
     * IEnumerator lockAnim(typeTutoAventure type)
    */
    #region GESTION anim progBarWaitFinAnim
    public Animation waitAnim_anim;
    public GameObject panLockAnimGo;
    public Image progBar;
    public int offsetTimeAnim = 10;
    private bool animLocked = false;
    public float minBarNoAnim = 0.15f; // jusqu'à prog atteind 0.15, progBar reste bloqué sur prog 0.15 ( sorte de temps de pause visuel)
    public float delayCloseLockBtnOk = 0.15f;
    private IEnumerator lockAnim(typeTutoAventure type)
    {
        //panLockAnimGo.SetActive(true);
        //waitAnim_anim.Play("waitAnimEndOpen");
        animLocked = true;
        detectorClickOnAnimGo.contentWarningSetOn();

        float timeAnim = (timeAnim_tuto[type] + offsetTimeAnim)/ 60f;
        float initTime = Time.time;
        bool lockIsClosed = false;

        progBar.fillAmount = 1 - minBarNoAnim;
        float curProg;
        do
        {
            yield return null;
            curProg = 1 - ((Time.time - initTime) / timeAnim);
            if(curProg <= 1 - minBarNoAnim)
                progBar.fillAmount = 1 - ( (Time.time - initTime) / timeAnim );

            if((initTime + timeAnim) - Time.time < delayCloseLockBtnOk && !lockIsClosed)
            {
                lockIsClosed = true;
                // jouer l'anim en dessous env 0.2 sec avant prog bar atteint fin
                waitAnim_anim.Play("waitAnimEndClose");
                animLocked = false;
            }

        } while (Time.time - initTime < timeAnim);

        detectorClickOnAnimGo.contentWarningSetOff();

        yield return new WaitForSeconds(30f / 60f);

        panLockAnimGo.SetActive(false);
    }
    #endregion

    /*
     * void btnInfoTuto()
     * void btnInfoTuto_close()
     */
    #region GESTION mode info ( btn '?' qui reouvre le tuto )
    private bool modeInfoOpenned = false;
    public void btnInfoTuto()
    {
        // ICI SEULEMENT APPELE pour ouverture
        modeInfoOpenned = true;

        // ouvrir le menu tuto
        // -> appeler fonction ICI : openTutoAventure(typeTutoAventure type) MAIS forcer sans timer
        StartCoroutine(openTutoAventure(currentTuto, true));

        // mettre le jeu en mode pause ( reprit de menuPause.btnPause() )
        manetteM.manetteDisabled();
        animFond3DGame.getInstance().menuPauseOpen();

        // anim speciale btn pause
        // -> reutiliser les anims 'menuPauseOff' et 'menuPauseOn' du menu pause
        // menuPause.menuPauseIsEnabled() menuPause.menuPauseDisabled()
        pauseM.menuPauseDisabled();

        // reset tps anim
        aideAnim[nameAnim[currentTuto]].time = 0;
        aideAnim.Play(nameAnim[currentTuto]);
    }

    private void btnInfoTuto_close()
    {
        modeInfoOpenned = false;

        // mettre le jeu en mode play ( reprit de menuPause.btnPlay() )
        manetteM.manetteEnabled();
        animFond3DGame.getInstance().menuPauseClose();

        pauseM.menuPauseIsEnabled();
    }
    #endregion


    /*
     * void setSideHand(bool right)
     * void openChoiceHand()
     * void closeChoixHand()
     * void btnChooseHand(int side)
     * void saveGaucherDroitier(bool droitier)
     */
    #region GESTION side Hand
    public Transform contentSideAnim, btnBImg;

    // des que menu est appelé pour ouverture, check la valeure de gauche droite SAVED
    // set menu en fonction

    // - menu choix hand
    // => PB , si a choisit main gauche avant ouvrir level, choose hand pas ouvert, side hand pas set
    // doit aussi être appelé à 
    // - init app, meme si choix hand avait deja été fait
    // - param menu : change side
    /*public void setSideHand(bool right)
    {
        if (!right)
        {
            . // supprimer la fonction, se fait seul dans ouverture
            contentSideAnim.localScale *= new Vector2(-1, 1);
            btnBImg.localScale *= new Vector2(-1, 1);
        }
    }*/

    public manetteManager manetteM;
    public Animation menuChooseHandAnim;
    public int resultChoiceInt;

    public void openChoiceHand()
    {
        resultChoiceInt = -1;
        menuChooseHandAnim.Play("openGaucherDroitier");
    }

    private void closeChoixHand()
    {
        // on ferme le menu
        menuChooseHandAnim.Play("closeGaucherDroitier");
        //setSideHand(resultChoiceInt != 1);

        if (resultChoiceInt == 1)
        {
            manetteM.setSideButton(false);
        }
    }

    public Animation btnLeftAnim, btnRightAnim;
    public void btnChooseHand(int side) // 1 : gauche , 2 : droite
    {
        if (resultChoiceInt != -1)
            return; // double clique rapide

        if (side == 2)
            btnRightAnim.Play("btnChooseHandClicked");
        else
            btnLeftAnim.Play("btnChooseHandClicked");

        saveGaucherDroitier(side == 2 ? true : false);
        resultChoiceInt = side;
        sfxGame.getInstance().playSfx_listTuto(typeSfx_tuto.tuto_btnLeftRight);
        closeChoixHand();
    }

    private void saveGaucherDroitier(bool droitier)
    {
        SaveSystem.Save(typeSave.MANETTE_SIDE_HAND, droitier ? 1 : -1);
        paramManager.paramState[typeParam.handSide] = false;
    }

    #endregion
}
