using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum typeInfoMenu
{
    compteurCrown,
    progressLevel,
    starLevel,
    difficultyLevel,
    compteurInfinis,
    multiplayer,
    none
}

public enum typeOpenInfo
{
    aventure,
    infinis,
}

[System.Serializable]
public struct infoMenu
{
    public typeInfoMenu type;
    public typeOpenInfo type_menu;
    public Transform ui_object;
    public Image briller_object;
    public int order_in_info; // savoir quel UI afficher quand clique sur précédent ou suivant
    public typeTraduction info_ui;
}
public class infoMenuManager : MonoBehaviour
{
    private static infoMenuManager instance;
    public static infoMenuManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("infoMenuManager").GetComponent<infoMenuManager>();
        return instance;
    }


    public infoMenu[] infos_menu_aventure;
    public infoMenu[] infos_menu_infinis;
    private infoMenu[] current_info_menu_list;

    public GameObject menu_info_tr;
    public Transform ui_content;
    private infoMenu current_info_menu;
    private typeInfoMenu current_type_info_open;
    private Transform current_ui_tr;
    private Transform current_ui_parent;
    private int current_ui_siblingIndex;


    public Text descriptionInfo_text;
    public Text difficultyInfo_text;
    public Text totCrownInfo_text;
    public Text progLevelInfo_text;
    //public GameObject starInfoGo, othersInfoGo;
    public GameObject panSwitchExpertMode;
    private void fillInfos()
    {
        /*bool starBtn = current_type_info_open == typeInfoMenu.starLevel;
        bool difficultyBtn = current_type_info_open == typeInfoMenu.difficultyLevel;
        bool totCrownBtn = current_type_info_open == typeInfoMenu.compteurCrown;
        bool recordInfinisBtn = current_type_info_open == typeInfoMenu.compteurInfinis;
        bool progLevelBtn = current_type_info_open == typeInfoMenu.progressLevel;*/

        /*starInfoGo.SetActive(starBtn);
        othersInfoGo.SetActive(!starBtn);*/

        // si etoile, pan info etoile ON, info text OFF
        if (current_type_info_open == typeInfoMenu.starLevel)
        {
            // activer panBtnSwitchMode
            panSwitchExpertMode.SetActive(true);
        }
        else if (current_type_info_open == typeInfoMenu.difficultyLevel)
        {
            difficultyInfo_text.text = traductionManager.getInstance().getTradTutoDifficulty(difficultyBar.prev_level_difficulty + 1);
        }
        else if (current_type_info_open == typeInfoMenu.compteurCrown)
        {
            totCrownInfo_text.text = SaveSystem.nb_couronne +
                "<size=23><color=\"#00A49D\">/" + (SaveSystem.nbNiveauAventure*3) + "</color></size>";
        }
        else if (current_type_info_open == typeInfoMenu.compteurInfinis)
        {
            // ne rien faire
        }
        else if(current_type_info_open == typeInfoMenu.progressLevel)
        {
            int currentProg = crownProgBar.maxi;
            progLevelInfo_text.text = currentProg+ "<size=23><color=\"#00A49D\">/3</color></size>";
        }
        /*else
        {
            setText(traductionManager.getInstance().getTradTutoUI(current_info_menu.info_ui)
                , descriptionInfo_text, false);
            panSwitchExpertMode.SetActive(false);
        }*/
        current_info_menu.briller_object.color = new Color(1f, 1f, 1f, 1f);
    }

    private void setText(string s, Text t, bool childToo)
    {
        string s_2 = s.Replace("_", "\n");
        t.text = s_2;
        if(childToo)
            t.transform.GetChild(0).GetComponent<Text>().text = s_2;
    }

    //private static bool posUiInfoInitialized = false;
    public Transform posInfo_difficultyTr, posInfo_totCrownTr, posInfo_recordInfinisTr, posInfo_progLevel;
    public Transform uiInfo_difficulty, uiInfo_totCrown, uiInfo_recordInfinis, uiInfo_progLevel;

    private Dictionary<typeInfoMenu, string> nameAnimOpenByType = new Dictionary<typeInfoMenu, string>() {
        { typeInfoMenu.difficultyLevel, "openInfo_difficulty"},
        { typeInfoMenu.compteurCrown, "openInfo_totCrown" },
        { typeInfoMenu.compteurInfinis, "openInfo_recordInfinis"},
        { typeInfoMenu.progressLevel, "openInfo_progLevel"},
        { typeInfoMenu.multiplayer, "openInfo_multiplayer"},
    };

    private Dictionary<typeInfoMenu, string> nameAnimCloseByType = new Dictionary<typeInfoMenu, string>() {
        { typeInfoMenu.difficultyLevel, "closeInfo_difficulty"},
        { typeInfoMenu.compteurCrown, "closeInfo_totCrown" },
        { typeInfoMenu.compteurInfinis, "closeInfo_recordInfinis"},
        { typeInfoMenu.progressLevel, "closeInfo_progLevel"},
        { typeInfoMenu.multiplayer, "closeInfo_multiplayer"},
    };

    // appelé après premier niveau pop
    public void initializePosOnAppOpen()
    {
        //posUiInfoInitialized = true;
        uiInfo_difficulty.position = new Vector2(posInfo_difficultyTr.position.x, uiInfo_difficulty.position.y);
        uiInfo_totCrown.position = new Vector2(posInfo_totCrownTr.position.x, uiInfo_totCrown.position.y);
        uiInfo_recordInfinis.position = new Vector2(posInfo_recordInfinisTr.position.x, uiInfo_recordInfinis.position.y);
    }

    public static bool menuInfoOpen = false;
    public void openInfoUI(bool animate)
    {
        //menu_info_tr.gameObject.SetActive(true);
        if (animate)
        {
            string anim = nameAnimOpenByType.ContainsKey(current_type_info_open) ?
                nameAnimOpenByType[current_type_info_open] : "openInfoMenu";
            GetComponent<Animation>().Play(anim);
        }



        /*if (!posUiInfoInitialized)
        {
            posUiInfoInitialized = true;
            uiInfo_difficulty.position = new Vector2(posInfo_difficultyTr.position.x, uiInfo_difficulty.position.y);
            uiInfo_totCrown.position = new Vector2(posInfo_totCrownTr.position.x, uiInfo_totCrown.position.y);
            uiInfo_recordInfinis.position = new Vector2(posInfo_recordInfinisTr.position.x, uiInfo_recordInfinis.position.y);
        }*/

        menuInfoOpen = true;

        current_ui_tr = getUI_byType(current_type_info_open);
        current_ui_parent = current_ui_tr.parent;
        current_ui_siblingIndex = current_ui_tr.GetSiblingIndex();

        // on le met en premier plan
        current_ui_tr.SetParent(ui_content);
        fillInfos();
    }

    public void closeInfoUI()
    {
        /*if (expertMode.animSwitch)
            return;*/
        string anim = nameAnimCloseByType.ContainsKey(current_type_info_open) ?
            nameAnimCloseByType[current_type_info_open] : "closeInfoMenu";
        GetComponent<Animation>().Play(anim);

        if (current_type_info_open == typeInfoMenu.compteurCrown)
            menuVisu.btnCloseMenu();
        //Debug.Log("CLose menu info type : " + current_type_info_open);

        /*GetComponent<Animation>().Play(current_type_info_open == typeInfoMenu.difficultyLevel ? "closeInfo_difficulty"
            : current_type_info_open == typeInfoMenu.compteurCrown ? "closeInfo_totCrown" 
            : "closeInfoMenu");*/
        //if (current_type_info_open == typeInfoMenu.starLevel)
        //panSwitchExpertMode.SetActive(false);

        if (current_type_info_open == typeInfoMenu.compteurCrown)
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        else
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeSimpleBtn);

        current_type_info_open = typeInfoMenu.none;
        //menu_info_tr.gameObject.SetActive(false);
        current_ui_tr.SetParent(current_ui_parent);
        current_ui_tr.SetSiblingIndex(current_ui_siblingIndex);
        current_info_menu.briller_object.color = new Color(1f, 1f, 1f, 0f);

        




        menuInfoOpen = false;
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_close);
    }

    private Transform getUI_byType(typeInfoMenu type)
    {
        foreach (infoMenu iM in current_info_menu_list)
            if (iM.type == type)
            {
                current_info_menu = iM;
                return iM.ui_object;
            }
        return null;
    }



    public void openMenuType(typeOpenInfo type_menu, typeInfoMenu type_info_requested)
    {
        // éviter de cliquer sur l'ui si est open dans le menu
        if (menu_info_tr.activeSelf)
            return;

        if(type_info_requested == typeInfoMenu.compteurCrown)
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
        else
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_infoUI);

        current_type_info_open = type_info_requested;

        switch (type_menu)
        {
            case typeOpenInfo.aventure:
                current_info_menu_list = infos_menu_aventure;
                openInfoUI(true);
                break;
            case typeOpenInfo.infinis:
                current_info_menu_list = infos_menu_infinis;
                openInfoUI(true);
                break;
        }
    }

    public menuVisuLevel menuVisu;
    public ScrollRectSnap_CS scrollLevels;
    public Animation animClick_compteur, animClick_prog, animClick_difficulty, animClick_star;
    public Animation animClick_multiplayer, animClick_record, animClick_rank;
    public void btnInfo_crownCompteur()
    {
        if (managerNiveau.gameOn || notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating || scrollLevels.isScrollingList())
            return;

        if (!menuInfoOpen)
            menuVisu.openMenu(false);

        openMenuType(typeOpenInfo.aventure, typeInfoMenu.compteurCrown);
        animClick_compteur.Play("itemUIClicked");
    }

    public void btnInfo_crownProgress()
    {
        if (managerNiveau.gameOn || notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating || scrollLevels.isScrollingList())
            return;

        openMenuType(typeOpenInfo.aventure, typeInfoMenu.progressLevel);
        animClick_prog.Play("itemUIClicked");
    }

    public void btnInfo_difficulty()
    {
        if (managerNiveau.gameOn || notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating || scrollLevels.isScrollingList())
            return;

        openMenuType(typeOpenInfo.aventure, typeInfoMenu.difficultyLevel);
        animClick_difficulty.Play("itemUIClicked");
    }

    public expertMode expertM;
    public void btnInfo_star()
    {
        if (managerNiveau.gameOn || notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating)
            return;

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
        if (lockStar.isUnlocked)
        {
            expertM.gameObject.SetActive(true);
            expertM.btnVoyantStarClicked();
            //openMenuType(typeOpenInfo.aventure, typeInfoMenu.starLevel);
            animClick_star.Play("itemUIClicked");
            current_type_info_open = typeInfoMenu.starLevel;
        }
        else
        {
            lockStar.getInstance().btnInfoLock();
        }
    }


    public void btnInfo_multiplayer()
    {
        if (notifDefiDone.isAnimatingNotif)
            return;
        openMenuType(typeOpenInfo.infinis, typeInfoMenu.multiplayer);
        current_type_info_open = typeInfoMenu.multiplayer;
        animClick_multiplayer.Play("itemUIClicked");
    }

    public void btnInfo_recordInfinis()
    {
        if (notifDefiDone.isAnimatingNotif)
            return;
        openMenuType(typeOpenInfo.infinis, typeInfoMenu.compteurInfinis);
        current_type_info_open = typeInfoMenu.compteurInfinis;
        animClick_record.Play("itemUIClicked");
    }

    // appelé par manager Ranking
    public void btnInfo_rank()
    {
        if (notifDefiDone.isAnimatingNotif)
            return;
        animClick_rank.Play("itemUIClicked");
    }
}

// 369 l -> 257 l -> 175 l
