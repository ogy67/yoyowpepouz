using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum typeParam
{
    son,
    music,
    vibration,
    handSide
}
public class paramManager : MonoBehaviour
{
    void Start()
    {
        instance = GameObject.Find("newParam_test").GetComponent<paramManager>();
        if (!dataLoaded)
            loadData();

        //setText(versionJeuText,
        //    traductionManager.getInstance().getTradParam(typeTraduction_app.param_versionApp) + ' ' + launcher.versionJeu, true);
        sfxGame.sonOn = paramState[typeParam.son];
    }

    private static paramManager instance;
    public static paramManager getInstance()
    {
        if (instance == null)
        {
            instance = GameObject.Find("newParam_test").GetComponent<paramManager>();
        }
        return instance;
    }

    public static Dictionary<typeParam, bool> paramState = new Dictionary<typeParam, bool>()
    {
        // valeures par defaut
        { typeParam.music, true },
        { typeParam.son, true },
        { typeParam.vibration, true },
        { typeParam.handSide, true } // true = droitier
    };
    private static bool dataLoaded = false;
    /*
     * static void loadData()
     * static bool getParamState(typeParam type)
    */
    #region gestion data
    private static void loadData()
    {
        object data = SaveSystem.loadData(typeSave.PARAM_STATE);
        if (data != null)
        {
            paramState = (Dictionary<typeParam, bool>)data;
        }
        else
        {
            SaveSystem.Save(typeSave.PARAM_STATE, paramState);
        }
        dataLoaded = true;
    }

    public static bool getParamState(typeParam type)
    {
        if (!dataLoaded)
            loadData();
        return paramState[type];
    }

    #endregion

    private void setText(Text t, string s , bool childToo)
    {
        t.text = s;
        if (childToo)
        {
            t.transform.GetChild(0).GetComponent<Text>().text = s;
        }
    }

    public static bool menuParamOpen = false;
    private static bool firstLoadDone = false;
    private bool currentSideHand = true;// droite
    public Color btnOnColor, btnOffColor;
    public Image glowBtnSfx, glowBtnMusic, glowBtnVibrate;
    public Animation animBtnOpenParam;
    public void btnOpenParam()
    {
        if (notifDefiDone.isAnimatingNotif && notifStarUnlocked.isAnimating)
            return;

        GetComponent<Animation>().Play("openParamMenu");
        animBtnOpenParam.Play("btnParam");
        menuParamOpen = true;

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_infoUI);
        //performanceManager.instance.openSousMenuP();

        if (!firstLoadDone)
        {
            firstLoadDone = true;
            if (paramState[typeParam.son])
            {
                glowBtnSfx.color = new Color(btnOnColor.r, btnOnColor.g, btnOnColor.b, glowBtnSfx.color.a);
                btnSoundAnim.Play("btnOffToOn");
                sonText.text = paramState[typeParam.son] ? "0N" : "OFF";
            }
            if (vibrationManager.getVibeVal())
            {
                glowBtnVibrate.color = new Color(btnOnColor.r, btnOnColor.g, btnOnColor.b, glowBtnVibrate.color.a);
                btnVibrationAnim.Play("btnOffToOn");
                vibrationText.text = paramState[typeParam.vibration] ? "0N" : "OFF";
            }
            if (paramState[typeParam.music])
            {
                glowBtnMusic.color = new Color(btnOnColor.r, btnOnColor.g, btnOnColor.b, glowBtnMusic.color.a);
                btnMusicAnim.Play("btnOffToOn");
                musicText.text = paramState[typeParam.music] ? "ON" : "OFF";
            }

            int saveSide = (int)SaveSystem.loadData(typeSave.MANETTE_SIDE_HAND);
            paramState[typeParam.handSide] =  saveSide == 1 || saveSide == -1000;

            sideHandText.text = traductionManager.getInstance().getTradSideHandParamMenu(paramState[typeParam.handSide]);

            //Debug.Log("Load param, side hand, droitier ? : " + paramState[typeParam.handSide]);
            if (!paramState[typeParam.handSide])// est sur droite de base
            {              
                // on change side img
                Vector2 scal = sideHandImg.localScale;
                sideHandImg.localScale = new Vector2(-scal.x, scal.y);
                currentSideHand = !currentSideHand;
            }
        }
    }

    public void btnCloseParam()
    {
        menuParamOpen = false;
        GetComponent<Animation>().Play("closeParamMenu");
        //StartCoroutine(closeLateParam());
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_close);
        //performanceManager.instance.closeSousMenuP();
    }


    public manetteManager manetteM;
    public Animation btnVibrationAnim;
    public Animation btnSoundAnim;
    public Animation btnMusicAnim;
    public Text sideHandText;
    public Transform sideHandImg;
    public Text sonText, vibrationText, musicText;
    //public Text versionJeuText;
    public Animation animGlowSfx, animGlowMusic, animGlowVibrate;
    public Animation animSideHand;

    // /!\ sideHand a déja un fichier save
    // est mit à jour dans le tuto
    // -> tuto, modifier enregistrement de side hand ( utiliser mm file que ici )
    public void btnSideHand()
    {


        // on change side img
        /*Vector2 scal = sideHandImg.localScale;
        sideHandImg.localScale = new Vector2(-scal.x, scal.y);*/
        animSideHand.Play(paramState[typeParam.handSide] ? "btnSideToGaucher" : "btnSideToDroitier");
        paramState[typeParam.handSide] = !paramState[typeParam.handSide];
        // on change le text 
        sideHandText.text = traductionManager.getInstance().getTradSideHandParamMenu(paramState[typeParam.handSide]);

        // on change le side dans manetteManager
        manetteM.setSideButton(paramState[typeParam.handSide]);

        SaveSystem.Save(typeSave.PARAM_STATE, paramState);
        SaveSystem.Save(typeSave.MANETTE_SIDE_HAND, paramState[typeParam.handSide] ? 1 : 0);
        //SaveSystem.Save(typeSave.MANETTE_SIDE_HAND, paramState[typeParam.handSide] == true ? 1 : 0);
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_switchState);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }
    public void btnMusic()
    {
        paramState[typeParam.music] = !paramState[typeParam.music];
        animSwitchBtn(typeParam.music, btnMusicAnim, animGlowMusic, musicText, paramState[typeParam.music]);
        musicManager.musicOn = paramState[typeParam.music];
        musicManager.getInstance().musicStateChanged();

    }

    public void btnSon()
    {
        paramState[typeParam.son] = !paramState[typeParam.son];
        sfxGame.sonOn = paramState[typeParam.son];
        animSwitchBtn(typeParam.son, btnSoundAnim, animGlowSfx, sonText, paramState[typeParam.son]);
        
        //sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
    }

    public void btnVibration()
    {
        paramState[typeParam.vibration] = !paramState[typeParam.vibration];
        animSwitchBtn(typeParam.vibration, btnVibrationAnim, animGlowVibrate, vibrationText, vibrationManager.vibeValChanged());
        //sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
    }

    // pour sfx, music, vibrate
    private void animSwitchBtn(typeParam typeP, Animation animBtn, Animation animGlow, Text onOffText, bool isOn)
    {
        animBtn.Play(isOn ? "btnOffToOn" : "btnOnToOff");
        animGlow.Play(isOn ? "glowParamToOn" : "glowParamToOff");
        onOffText.text = paramState[typeP] ? "ON" : "OFF";
        SaveSystem.Save(typeSave.PARAM_STATE, paramState);
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_switchState);
        //sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }

    public Animation animMenuTwitter;
    public void btnOpenMenuTwitter()
    {
        animMenuTwitter.Play("openMenuTwitter");
        btnTwitterAnim.Play("btnParamClicked");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
    }

    public void btnCloseMenuTwitter()
    {
        animMenuTwitter.Play("closeMenuTwitter");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_close);
    }

    public Animation btnTwitterAnim;
    public Animation btnTwitterAnim_menuTwitter;
    public void btnTwitter()
    {
        Application.OpenURL("https://www.tiktok.com/@ogystik");
        btnTwitterAnim_menuTwitter.Play("btnParamClicked_2");

        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_yT_socialMedia);
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
    }

    public void btnPlayStore()
    {
        if (Application.platform == RuntimePlatform.Android)
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.Yoyow");
        else// if (Application.platform == RuntimePlatform.IPhonePlayer)
            Application.OpenURL("itms-apps://itunes.apple.com/app/com.pepouz.yoyow");
        //else
        //    Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.Yoyow");

        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_yT_socialMedia);
    }

    /*
    public void btnYoutube()
    {
        sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }*/
}
