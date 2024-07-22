using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeTutoPhase
{
    presentation,
    pushPaletClick,
    aigleClick,
    changeSpeed_Bounce,
    checkPoint,
    changeRotationSide,
    fin_tuto,
    empty,
}

[System.Serializable]
public struct tutoPhase
{
    //public string infoPhase_string;
    public typeTraduction type_trad;
    public typeTutoPhase type_phase;
    public GameObject associated_level; // si rien, pas de changement de level
    public GameObject associated_fleche;
}

public class TutoManager : MonoBehaviour
{
    [Header("Vrac")]
    public GameObject _appliManager;
    public GameObject trailTuto;
    public GameObject pieceGo;
    public Transform mainCanvas;
    public Animation menuBasAnim;
    public static bool menuBasOpen = false;
    private bool switchingPanelBas = false;
    private bool playerValuesInitialized = false;

    /*private static TutoManager instance;
    public static TutoManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("tutoManager").GetComponent<TutoManager>();
        return instance;
    }*/

    private void saveGaucherDroitier(bool droitier)
    {
        // par defaut MANETTE_SIDE_HAND = 1 ( droitier )
        //if (!droitier)
        //{
            SaveSystem.Save(typeSave.MANETTE_SIDE_HAND, droitier ? 1 : -1);
            paramManager.paramState[typeParam.handSide] = false;
        //}
    }


    public string[] nameAnimByOrder;
    public Animation animProgBarTuto;
    private int currentIndex = 0;

    private void progBarGoNextPhase()
    {
        animProgBarTuto.Play(nameAnimByOrder[currentIndex]);
        currentIndex++;
        StartCoroutine(animTextProgChange(currentIndex, true));
    }

    public Text progText, progText_ombre;
    public float timeMiddleAnim = 6 / 60f;
    private IEnumerator animTextProgChange(int progValue, bool anim)
    {
        if(anim)
            yield return new WaitForSeconds(timeMiddleAnim);
        progText.text = progValue + "<color=\"#BA7CB7\"><size=20>/4</size></color>";
        progText_ombre.text = progValue + "<size=20>/4</size>";
    }

    /*public bool isPlayerInSwitchPhase()
    {
        return currentPhase.type_phase == typeTutoPhase.changeRotationSide;
    }

    public bool canDisplayBtnB_mainAnim() // animation main
    {
        return currentPhase.type_phase == typeTutoPhase.aigleClick;
        //return currentPhase.type_phase != typeTutoPhase.pushPaletClick;
    }

    public bool canDisplayBtnB_manette() // animation main
    {
        return currentPhase.type_phase != typeTutoPhase.pushPaletClick;
        //return currentPhase.type_phase != typeTutoPhase.pushPaletClick;
    }*/

    [Header("Initialisations / Phases")]
    public Transform initPos;
    public Animation animUIMenu;
    public Animation transitionTutoToApp;
    public Transform niveauTuto;
    public tutoPhase[] phasesTuto;
    public static tutoPhase currentPhase;
    private int currentIndexPhase;
    //public static bool tutoOn = false;
    private GameObject currentFleche;
    public static bool aigleUnlocked, switchUnlocked;
    //public RectTransform manetteZoneClickTr, posManetteTr_Tuto, posManetteTr_Normal;
    public Vector2 anchoredPosVerticalManetteTuto;
    /*
     * void loadTuto()
     * IEnumerator openChoiceHand()
     * void closeChoixHand()
     * void btnChooseHand(int side)
     * IEnumerator loadPhase()
     * IEnumerator doubleAnimBtnOk()
     * IEnumerator loadNextPhase_cleantText()
     * void spawnUnspawnPlayer_menuInfoStateChanged(bool isOpen)
    */
    #region Initialisation tuto / Phases tuto
    public void loadTuto()
    {
        sfxGame.sonOn = true;
        //tutoOn = true;
        trailTuto.SetActive(true);

        aigleUnlocked = false;
        switchUnlocked = false;
        currentIndexPhase = 0;
        nbMonnaie = 0;
        setMonnaieCompteur();

        StartCoroutine(loadPhase());

        //manetteZoneClickTr.anchoredPosition = posManetteTr_Tuto.anchoredPosition;
    }

    public manetteManager manetteM;
    public Animation menuChooseHandAnim;
    private bool waitingChoiceHand = false;
    private int resultChoiceInt;

    private void openChoiceHand()
    {
        resultChoiceInt = -1;
        menuChooseHandAnim.Play("openGaucherDroitier");
    }

    private void closeChoixHand()
    {
        waitingChoiceHand = false;
        // on ferme le menu
        menuChooseHandAnim.Play("closeGaucherDroitier");

        //menuInfoTuto.setSideHand(resultChoiceInt != 1);
        //aideVisuelleTuto.getInstance().setSideHand(resultChoiceInt != 1); // 1 -> gaucher

        if (resultChoiceInt == 1)
        {
            manetteM.setSideButton(false);


            /* on change sens main de tutoriel si gaucher
            Vector2 scal = handParent.transform.localScale;
            handParent.transform.localScale = new Vector2(-scal.x, scal.y);
            // on doit rechanger le scale de btnB ( est dans le sens inverse )
            Vector2 scal2 = btnAManetteAnimTuto.transform.localScale;
            btnAManetteAnimTuto.transform.localScale = new Vector2(-scal2.x, scal2.y);*/
        }
        // on enregistre dans la mémoire le côté choisit par player 
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
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.tuto_ok_help);
        closeChoixHand();
    }

    public Image btnOkScreen;
    private bool loadingNewPhase = false;
    public float timeWaitOpenBtnOk_closeChooseHand = 1f;
    public float timeWaitReopenAnim = 0.8f;
    public menuTutoAventure menuInfoTuto;

    private IEnumerator loadPhase()
    {
        loadingNewPhase = true;
        currentPhase = phasesTuto[currentIndexPhase];
        typeTutoPhase type = currentPhase.type_phase;
        currentIndexPhase++;

        if (currentIndexPhase == phasesTuto.Length)
        {
            manetteM.savePosBtnManette();
            //manetteZoneClickTr.anchoredPosition = posManetteTr_Normal.anchoredPosition;
            // animation de fin de tuto
            btnOkScreen.raycastTarget = false;
            gameManager.getInstance().tutoEnd_giveMonney(nbMonnaie);
            niveauTuto.GetComponent<Animation>().Play("niveauTutoOut");
            animUIMenu.Play("tutoClose");
            fondJeu.GetComponent<Animation>().Play("fondOut");
            transitionTutoToApp.Play("fadeTutoToApp");
            yield return new WaitForSeconds(25/60f);

            niveauTuto.gameObject.SetActive(false);
            _appliManager.gameObject.SetActive(true);
            _appliManager.GetComponent<appliManager>().lancerApp(true);
            transform.parent.gameObject.SetActive(false);
            //tutoOn = false;
            playerMovment.comportement = typeBehaviour.aventure;
            yield break;
        }

        //string s;// traduction info
        switch (type)
        {
            case typeTutoPhase.presentation:
                animUIMenu.Play("tutoOpen");
                niveauTuto.gameObject.SetActive(true);
                niveauTuto.GetComponent<Animation>().Play("niveauTutoCome");
                sfxGame.getInstance().playSfx_listTuto(typeSfx_tuto.tuto_firstLevelCome);
                yield return new WaitForSeconds(15/60f);// 15 / 60f);                
                writeText(traductionManager.getInstance().getTradTuto(currentPhase.type_trad));
                break;
            case typeTutoPhase.pushPaletClick:
                // d'abord , on demande si est gaucher ou droitier
                waitingChoiceHand = true;
                openChoiceHand();
                yield return new WaitUntil(predicate: () => waitingChoiceHand == false);
                writeTextAnim.getInstance().deleteText();

                yield return new WaitForSeconds(0.5f);

                //animFond3DGame.getInstance().gameOpen();

                progBarGoNextPhase();
                switchingPanelBas = true;

                //yield return new WaitForSeconds(2 / 6f);// temps close menu choose hand
                StartCoroutine(switchState(false, false, false));
                yield return new WaitForSeconds(timeWaitOpenBtnOk_closeChooseHand);

                //menuInfoTuto.openTuto_tuto(typeTuto_tuto.push, true);
                /*s = traductionManager.getInstance().getTradTuto(typeTraduction.tuto_pushPalet);
                infoText.text = s;
                infoText_ombre.text = s;
                animPanInfoText.Play("openInfoTextTuto");*/
                //animUIMenu.Play("tutoPanelBasOpenChooseHand");

                break;
            case typeTutoPhase.aigleClick:
                progBarGoNextPhase();
                aigleUnlocked = true;
                StartCoroutine(switchState(false, true, true));

                /*s = traductionManager.getInstance().getTradTuto(typeTraduction.tuto_tortueClick);
                infoText.text = s;
                infoText_ombre.text = s;
                animPanInfoText.Play("openInfoTextTuto");*/
                yield return new WaitForSeconds(timeWaitReopenAnim);

                //menuInfoTuto.openTuto_tuto(typeTuto_tuto.slow, true);
                niveauTuto.GetComponent<packNiveau>().lvlClean();
                break;
            case typeTutoPhase.changeSpeed_Bounce:
                progBarGoNextPhase();
                //ghostAide.main_2_anim.gameObject.SetActive(false);
                StartCoroutine(switchState(false, true, true));

                yield return new WaitForSeconds(timeWaitReopenAnim);
                niveauTuto.GetComponent<packNiveau>().lvlClean();

                //menuInfoTuto.openTuto_tuto(typeTuto_tuto.slow, true);
                /*s = traductionManager.getInstance().getTradTuto(typeTraduction.tuto_bounce);
                infoText.text = s;
                infoText_ombre.text = s;
                animPanInfoText.Play("openInfoTextTuto");*/
                break;
            case typeTutoPhase.checkPoint:
                progBarGoNextPhase();
                StartCoroutine(switchState(false, true, true));

                yield return new WaitForSeconds(timeWaitReopenAnim);
                //menuInfoTuto.openTuto_tuto(typeTuto_tuto.checkP, true);
                niveauTuto.GetComponent<packNiveau>().lvlClean();
                /*s = traductionManager.getInstance().getTradTuto(typeTraduction.tuto_checkP);
                infoText.text = s;
                infoText_ombre.text = s;
                animPanInfoText.Play("openInfoTextTuto");*/
                break;
            case typeTutoPhase.changeRotationSide:
                progBarGoNextPhase();
                switchUnlocked = true;
                StartCoroutine(switchState(false, true, true));


                yield return new WaitForSeconds(timeWaitReopenAnim);
                //menuInfoTuto.openTuto_tuto(typeTuto_tuto.switchR, true);
                niveauTuto.GetComponent<packNiveau>().lvlClean();
                /*s = traductionManager.getInstance().getTradTuto(typeTraduction.tuto_switchRotation);
                infoText.text = s;
                infoText_ombre.text = s;
                animPanInfoText.Play("openInfoTextTuto");*/
                break;
            case typeTutoPhase.fin_tuto:
                progBarGoNextPhase();
                niveauTuto.GetComponent<Animation>().Play("niveauZoomOut");
                StartCoroutine(doubleAnimBtnOk());
                // on dit que le menu bas est ouvert, mais ne l'est pas
                spawnUnspawnPlayer_menuInfoStateChanged(true);
                writeText(traductionManager.getInstance().getTradTuto(currentPhase.type_trad));
                // animation de btn '?' en 'OK'
                yield return new WaitForSeconds(2f);
                break;
        }
        loadingNewPhase = false;
    }

    // enchainer activation et changement de text, par 2 Go.Animation differents
    private IEnumerator doubleAnimBtnOk()
    {
        btnOkHelpAnim.Play("btnOkEnabled");
        while (btnOkHelpAnim.isPlaying)
        {
            yield return null;
        }
        btnOkHelpEnabled = true;
        btnOkScreen.raycastTarget = true;
        animUIMenu.Play("btnAidTutoToOk");

        
        // si pas dans derniere phase
        // anim text info pop
        // animPanInfoText.Play("openInfoTextTuto"); 
    }

    // Clean text tuto, lance nextPhase 
    private IEnumerator loadNextPhase_cleantText(bool openChooseHand)
    {
        switchingPanelBas = true;
        //if(cleanText)
        writeTextAnim.getInstance().animateTextOff();
        if(openChooseHand)
            animUIMenu.Play("tutoPanelBasCloseChooseHand");

        //yield return new WaitForSeconds(1 / 6f);

        StartCoroutine(loadPhase());
        yield return new WaitForSeconds(0.5f);

        switchingPanelBas = false;
    }

    private void spawnUnspawnPlayer_menuInfoStateChanged(bool isOpen)
    {
        if (!isOpen)
        {
            player.GetComponent<playerMovment>().activate();
        }
        else
        {
            player.GetComponent<playerMovment>().stop();
        }
    }
    #endregion


    //public Animation animPanInfoText;
    //public Text infoText_ombre, infoText;
    /*
     * void btnOkHelp()
     * IEnumerator switchState(bool setGhostPhase)
    */
    #region btn OK / HELP
    public void btnOkHelp()
    {
        if (!btnOkHelpEnabled)
            return;

        sfxGame.getInstance().playSfx_listTuto(typeSfx_tuto.tuto_btnOkHelp);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.tuto_ok_help);

        if (switchingPanelBas || loadingNewPhase
            || winning || dying || (!playerMovment.finish && playerMovment.firstCheckP == false))
        {
            return;
        }

        // D'abord , on regarde si on est dans une phase d'animation visuelle
        // Si ne l'est pas ( présentation , fin tuto ) : juste passer à l'étape suivante
        if (currentPhase.type_phase == typeTutoPhase.presentation
            || currentPhase.type_phase == typeTutoPhase.fin_tuto)
        {
            StartCoroutine(loadNextPhase_cleantText(currentPhase.type_phase == typeTutoPhase.presentation ? true : false));
        }
        else
        {
            //menuInfoTuto.openTuto_tuto(menuInfoTuto.currentType, false);
            StartCoroutine(switchState(true, true, false));
        }
    }

    public void btnOkMenuAnim()
    {
        StartCoroutine(switchState(true, true, false));
    }

    //public Animation progBarWaitAnim;
    //public Image progBarWaitImg;
    public float[] timeAnimsPhase = new float[] { 271 / 60f, 317 / 60f, 207 / 60f, 409 / 60f, 479 / 60f };
    public float timePopPlayerAfterCloseMenu = 0.4f;

    private IEnumerator switchState(bool newAnimAlreadyLoaded, bool changeMenuState, bool playerWin)
    {
         // switchingPanelBas && btnOkHelpEnabled bon si player win ?
        switchingPanelBas = true;
        // si btnOkHelp pas enable -> veut dire que player died / won
        if (!btnOkHelpEnabled)
        {
            // si player win, simple anim vers btnOk
            if (!playerWin)
            {
                btnOkHelpAnim.Play("btnOkEnabled");
                Debug.Log("<color=purple>BtnOkEnable</color>");
            }
            btnOkHelpEnabled = true;
        }
        if (changeMenuState)
        {
            // /!\
            // quand player win, ne pas reactiver btn ok tt de suite
            if (playerWin)
            {
                Debug.Log("Player win, menu bas open ? " + menuBasOpen);
                menuBasAnim.Play("tutoPanelBasClose_win");
            }
            else
            {
                // seulement appeler ok anim 
                // btn come : init tuto, end tuto

                Debug.Log("Change state btn help, isOn : " + menuBasOpen);
                menuBasAnim.Play(menuBasOpen ? "tutoPanelBasOut" : "tutoPanelBasCome");
                // activer desactiver info text
                
                //animPanInfoText.Play(menuBasOpen ? "closeInfoTextTuto": "openInfoTextTuto");
            }

            niveauTuto.GetComponent<Animation>().Play("niveauZoom" + (!menuBasOpen ? "Out" : "In"));

            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.zoom_nextLevel);
            //sfxManager.getInstance().playSound_ingame(typeSound_ingame.zoom_level);
        }

        menuBasOpen = !menuBasOpen;

        if (menuBasOpen)
        {
            spawnUnspawnPlayer_menuInfoStateChanged(menuBasOpen);

            Debug.Log("(2) Anim main A on");
            // main pop si clique sur btn ok et ouvre le menu
            yield return new WaitForSeconds(0.5f);//newAnimAlreadyLoaded ? 0.5f : 0.65f);// 0.5f);
            //aideVisuelleTuto.getInstance().openAideVisuelle(currentPhase.type_phase);

            //menuBasAnim.Play("tutoPanelBasCome");

            //menuInfoTuto.openTuto_tuto(menuInfoTuto.currentType, false);

            if (!newAnimAlreadyLoaded)
            {
                //menuInfoTuto.openTuto_tuto(menuInfoTuto.currentType, false);
                /*progBarWaitAnim.Play("openProgBarEndAnim");
                float timeAnim = timeAnimsPhase[currentIndex - 1];//; 5f;
                float initT = Time.time;
                progBarWaitImg.fillAmount = 1f;
                while (Time.time < initT + timeAnim)
                {
                    yield return null;
                    progBarWaitImg.fillAmount = 1 - (Time.time - initT) / timeAnim;
                }
                progBarWaitAnim.Play("closeProgBarEndAnim");

                // on reactive btn ok à la fin wait anim
                menuBasAnim.Play("tutoPanelBasOpenChooseHand");*/
            }
        }
        else
        {
            //aideVisuelleTuto.getInstance().closeAnimVisuelle();

            yield return new WaitForSeconds(timePopPlayerAfterCloseMenu);
            // initialisation des valeures de player
            if (!playerValuesInitialized)
            {
                playerMovment.initCenter = initPos.transform;
                playerMovment.initPos = initPos.transform.GetChild(0).position;
                playerValuesInitialized = true;
                playerMovment.sens = -1;
            }

            spawnUnspawnPlayer_menuInfoStateChanged(menuBasOpen);
        }
        
        
        switchingPanelBas = false;
    }

    #endregion

    public Animation btnOkHelpAnim;
    private bool btnOkHelpEnabled = true;
    // appelé quand player pose doigt sur btnA ET est sur premier checkP
    public void playerFirstClick()
    {
        // désactiver Click btn OK/HELP
        btnOkHelpAnim.Play("btnOkDisabled");
        btnOkHelpEnabled = false;
    }

    private void writeText(string s)
    {
        writeTextAnim.getInstance().writeText(s);
    }


    [Header("Ancien Tuto")]
    public Transform player;
    public GameObject fondJeu;
    private bool winning = false, dying = false;
    public float timeUpdateCompteurCoin = 32 / 60f;
    /*
     * IEnumerator lockWin()
     * void winLevel()
     * void yowReachedUI()
     * IEnumerator waitCloseYowUI()
     * void died()
     * IEnumerator waitDeath()
    */
    #region FONCTIONS ANCIEN TUTO
    // rarement : double win, fait bug phases et map
    // supposition : double entrée dans arriveeCheckP
    private IEnumerator lockWin()
    {
        winning = true;

        yield return new WaitForSeconds(1f);
        monnaieAnim.Play("monneyComeTuto");

        yield return new WaitForSeconds(timeUpdateCompteurCoin);
        nbMonnaie++;
        setMonnaieCompteur();
        sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.get_yow);

        // si sur dernière phase, doit raccourcir temps pour etre phasé avec close coin
        bool lastPhase = currentPhase.type_phase == typeTutoPhase.changeRotationSide;
        Debug.Log("Is in last phase ? " + lastPhase);
        yield return new WaitForSeconds(1.5f - (lastPhase ? timeUpdateCompteurCoin + 0.15f : timeUpdateCompteurCoin));

        winning = false;
        StartCoroutine(loadNextPhase_cleantText(false));
    }
    public void winLevel()
    {
        if (!winning)
        {
            loadingNewPhase = true;
            StartCoroutine(lockWin());
            //sfxManager.getInstance().playSound_ingame(typeSound_ingame.win);
            fondJeu.GetComponent<Animation>().Play("tutoFondWin");
            player.GetComponent<playerMovment>().arivee();
        }
    }

    /*public void yowReachedUI()
    {
        // on pop 1 seul yow par win dans tuto, on peut cash fermer au premier appel
        StartCoroutine(waitCloseYowUI());
    }

    private IEnumerator waitCloseYowUI()
    {
        
        //monnaieAnim.Play("monnaieIncremente");
        nbMonnaie++;
        yield return new WaitForSeconds(7 / 60f);
        setMonnaieCompteur();
        sfxManager.getInstance().playSound_menuP(typeSound_menuP.get_yow);
        yield return new WaitForSeconds(0.7f);
        //monnaieAnim.Play("monneyOutTuto");
    }*/
    public void died()
    {
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_die);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.death);
        fondJeu.GetComponent<Animation>().Play("tutoFondLose");
        StartCoroutine(waitDeath());
        playerMovment.sens = -1;
    }
    public float timePlayerRepopDeathTuto = 1f;
    private IEnumerator waitDeath()
    {
        dying = true;
        yield return new WaitForSeconds(timePlayerRepopDeathTuto);
        player.GetComponent<playerMovment>().activate();
        btnOkHelpAnim.Play("btnOkEnabled");
        btnOkHelpEnabled = true;
        dying = false;
    }

    #endregion

    [Header("Gestion argent")]
    int nbMonnaie = 0;
    public GameObject monnaieCompteur;
    public Animation monnaieAnim;
    public Text nbYowText;

    private void setMonnaieCompteur()
    {
        int nbZero = 5 - nbMonnaie.ToString().Length;
        string ret = "";
        while (nbZero > 0)
        {
            ret += "0";
            nbZero--;
        }
        nbYowText.text = ret + nbMonnaie;
        nbYowText.transform.GetChild(0).GetComponent<Text>().text = "<color=\"#91895A\">" + ret + "</color>" + nbMonnaie.ToString();
    }

}

// 755.l -> 510.l
