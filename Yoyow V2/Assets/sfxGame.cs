using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeSfx_coin
{
    none,
    pay,
    coin_getSingle,
    coin_getMultiple,
    coin_pickUpMap
}

public enum typeSfx_ingame
{
    none,
    manette_clickDownBtn,
    manette_switch,

    player_release,
    player_pivot,
    player_bounce,
    player_portal,
    player_superBoost,
    player_winLevel,

    player_hang,
    player_superB,

    player_pop,

    pause_open,
    pause_close,
    pause_quit,

    zoom_startGame,
    zoom_endGame,
    zoom_nextLevel,

    roadMap_open,
    roadMap_close,
    roadMap_openInfo,
    roadMap_closeInfo,
    roadMap_next,

    player_boostNormal,
    player_die,

    getLife_compteur,
}

public enum typeSfx_tuto
{
    none,
    tuto_btnOkHelp,
    tuto_btnLeftRight,
    tuto_firstLevelCome,

    ghostPlayer_bounce,
}

public enum typeSfx_mainMenu
{
    none,
    click_simpleBtn,
    click_closeSimpleBtn,
    click_openMenu,
    click_closeMenu,
    click_switchInfinis,

    click_btnGaucheD,
    click_btnBounds,
    click_btnStartGame,
    win_star,
    switch_star,




    unlock_infinis,
    unlock_level,
    btn_goUnlock,

    initApp_popLevel,

    crown_pop,
    crown_enterBar
}

public enum typeSfx_gameOver
{
    none,
    gameOver_open,
    gameOver_close,
    gameOver_switch,
    gameOver_getStar,
    btn_action,
    getLife,
}

// menu param, menu defi, menu rank
public enum typeSfx_paramMenu
{
    none,
    btn_switchState,
    btn_simple,
    btn_close,
    btn_getReward,
    switchToRate, // après get reward

    roueEndRotation,
}

public enum typeSfx_defiMenu
{
    none, 
    clickItem,
    clickGetReward,
    notifPop,
}

public enum typeSfx_shopMenu
{
    none,
    switch_section,
    click_item, // pas btn section
    openSousMenu,
    closeSousMenu,
    openSousMenu_validAchat,
    closeSousMenu_validAchat,
    btnReward_validAchat,
    animReward_validAchat,
    btn_useInfoItem,
    btn_preDeleteBoost,
    btn_deleteBoost,
}

public enum typeSfx_shopSousMenu
{
    none,
    infoBoost_btnInfo,
    btnBuyNotOk,
}

public enum typeSfx_rewardBoost
{
    none,

    boost_open,
    boost_close,
    boost_switch,
    boost_reveal,
    boost_blast,
    boost_full,

    booster_come,
    booster_skratch,
    //booster_open,

    itemBlast_end,
    //progBar_full,
}

public enum typeSfx_upgradeBoost
{
    none,
    upgrade_open,
    upgrade_close,
    click_pass,
    animUp_briller,
    animUp_star,
    animUp_item,
}

public enum typeSfx_rankOnline
{
    none,

    maintenance_open,
    maintenance_close,
    maintenance_end,

    createAccount_open,
    createAccount_close,
    createAccount_btnOk,
    createAccount_errorNameUsed,

    loadRank_open,
    loadRank_close,

    menuRank_open,
    menuRank_close,
    menuRank_switch,

    infoPlayer_open,
    infoPlayer_close,
    infoPlayer_deny,
    infoPlayer_clickEmpty,

    rewardWeek_open,
    rewardWeek_get,
    rewardWeek_close,

    btnUpDown,

    rewardWeek_anim,
}

public enum typeSfx_introApp
{
    none,
    arrivee_pop,
    player_come,
    name_pop,
    close,
}

public enum typeSfx_other
{
    none,
    click_switchInfinis_start,
}

[System.Serializable]
public struct sfxInfo
{
    public typeSfx_coin _typeSfx_coin;
    public typeSfx_ingame _typeSfx_ingame;
    public typeSfx_tuto _typeSfx_tuto;
    public typeSfx_mainMenu _typeSfx_mainMenu;
    public typeSfx_gameOver _typeSfx_gameOver;
    public typeSfx_paramMenu _typeSfx_paramMenu;
    public typeSfx_defiMenu _typeSfx_defiMenu;
    public typeSfx_shopMenu _typeSfx_shopMenu;
    public typeSfx_shopSousMenu _typeSfx_shopSousMenu;
    public typeSfx_rewardBoost _typeSfx_rewardBoost;
    public typeSfx_upgradeBoost _typeSfx_upgradeBoost;
    public typeSfx_rankOnline _typeSfx_rankOnline;
    public typeSfx_introApp _typeSfx_introApp;
    public typeSfx_other _typeSfx_other;
    public AudioClip _soundAudio;
    public float _volume;
}

public class sfxGame : MonoBehaviour
{
    public sfxInfo[] _listSfx_coin;
    public sfxInfo[] _listSfx_ingame;
    public sfxInfo[] _listSfx_tuto;
    public sfxInfo[] _listSfx_mainMenu;
    public sfxInfo[] _listSfx_gameOver;
    public sfxInfo[] _listSfx_paramMenu;
    public sfxInfo[] _listSfx_defiMenu;
    public sfxInfo[] _listSfx_shopMenu;
    public sfxInfo[] _listSfx_shopSousMenu;
    public sfxInfo[] _listSfx_rewardBoost;
    public sfxInfo[] _listSfx_upgradeBoost;
    public sfxInfo[] _listSfx_rankOnline;
    public sfxInfo[] _listSfx_introApp;
    public sfxInfo[] _listSfx_other;

    public static bool sonOn = true;

    private static sfxGame instance;
    public static sfxGame getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("SFX V2").GetComponent<sfxGame>();
        return instance;
    }

    public Transform parentSound;
    public GameObject instanceSound;
    /*
     * void playSound(sfxInfo _itemInfo)
     * void createInstanceSound(AudioClip s, float vol)
    */
    #region Lecture SFX
    private void playSound(sfxInfo _itemInfo)
    {
        nbSfxSimultaneous++;
        //Debug.Log("Nombre de son simmultanés : "+nbSfxSimultaneous);
        createInstanceSound(_itemInfo._soundAudio, _itemInfo._volume);
    }

    private void createInstanceSound(AudioClip s, float vol)
    {
        GameObject p = Instantiate(instanceSound);
        p.transform.SetParent(parentSound);// instance.transform);
        p.SetActive(true);

        AudioSource player = p.GetComponent<AudioSource>();
        p.GetComponent<itemSound>().player = player;
        player.volume = vol;
        player.clip = s;
        player.Play();
    }

    public int nbSfxSimultaneous = 0;
    public void endSfx()
    {
        nbSfxSimultaneous--;
    }
    #endregion



    public void playSfx_listCoin(typeSfx_coin typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_coin[i]._typeSfx_coin != typeSfx && i < _listSfx_coin.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_coin.Length)
        {
            playSound(_listSfx_coin[i]);
        }
    }

    public void playSfx_listIngame(typeSfx_ingame typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_ingame[i]._typeSfx_ingame != typeSfx && i < _listSfx_ingame.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_ingame.Length)
        {
            playSound(_listSfx_ingame[i]);
        }
    }

    public void playSfx_listTuto(typeSfx_tuto typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_tuto[i]._typeSfx_tuto != typeSfx && i < _listSfx_tuto.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_tuto.Length)
        {
            playSound(_listSfx_tuto[i]);
        }
    }

    public void playSfx_listMainMenu(typeSfx_mainMenu typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_mainMenu[i]._typeSfx_mainMenu != typeSfx && i < _listSfx_mainMenu.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_mainMenu.Length)
        {
            playSound(_listSfx_mainMenu[i]);
        }
    }

    public void playSfx_listgameOver(typeSfx_gameOver typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_gameOver[i]._typeSfx_gameOver != typeSfx && i < _listSfx_gameOver.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_gameOver.Length)
        {
            playSound(_listSfx_gameOver[i]);
        }
    }

    public void playSfx_listParamMenu(typeSfx_paramMenu typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_paramMenu[i]._typeSfx_paramMenu != typeSfx && i < _listSfx_paramMenu.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_paramMenu.Length)
        {
            playSound(_listSfx_paramMenu[i]);
        }
    }

    public void playSfx_listDefiMenu(typeSfx_defiMenu typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_defiMenu[i]._typeSfx_defiMenu != typeSfx && i < _listSfx_defiMenu.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_defiMenu.Length)
        {
            playSound(_listSfx_defiMenu[i]);
        }
    }

    public void playSfx_listShopMenu(typeSfx_shopMenu typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_shopMenu[i]._typeSfx_shopMenu != typeSfx && i < _listSfx_shopMenu.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_shopMenu.Length)
        {
            playSound(_listSfx_shopMenu[i]);
        }
    }

    public void playSfx_listShopSousMenu(typeSfx_shopSousMenu typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_shopSousMenu[i]._typeSfx_shopSousMenu != typeSfx && i < _listSfx_shopSousMenu.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_shopSousMenu.Length)
        {
            playSound(_listSfx_shopSousMenu[i]);
        }
    }

    public void playSfx_listRewardBoost(typeSfx_rewardBoost typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_rewardBoost[i]._typeSfx_rewardBoost != typeSfx && i < _listSfx_rewardBoost.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_rewardBoost.Length)
        {
            playSound(_listSfx_rewardBoost[i]);
        }
    }

    public void playSfx_listUpgradeBoost(typeSfx_upgradeBoost typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_upgradeBoost[i]._typeSfx_upgradeBoost != typeSfx && i < _listSfx_upgradeBoost.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_upgradeBoost.Length)
        {
            playSound(_listSfx_upgradeBoost[i]);
        }
    }

    public void playSfx_listRankOnline(typeSfx_rankOnline typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_rankOnline[i]._typeSfx_rankOnline != typeSfx && i < _listSfx_rankOnline.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_rankOnline.Length)
        {
            playSound(_listSfx_rankOnline[i]);
        }
                
    }

    public void playSfx_listIntroApp(typeSfx_introApp typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_introApp[i]._typeSfx_introApp != typeSfx && i < _listSfx_introApp.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_introApp.Length)
        {
            playSound(_listSfx_introApp[i]);
        }
    }

    public void playSfx_listOther(typeSfx_other typeSfx)
    {
        if (!sonOn) return;

        // on cherche le son dans la liste
        int i = 0;
        while (_listSfx_other[i]._typeSfx_other != typeSfx && i < _listSfx_other.Length)
            i++;
        // si le son a été trouvé
        if (i < _listSfx_other.Length)
        {
            playSound(_listSfx_other[i]);
        }
    }
}
