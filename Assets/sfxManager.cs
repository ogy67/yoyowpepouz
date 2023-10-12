using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeSound_online
{
    none,
    btn_close,
    btn_worldLocal,
    btn_logInSignIn,
    btn_ranking,
}
public enum typeSound_menuP
{
    none,
    switch_level,
    btn_firstLast,
    btn_infoUI,
    btn_shopDefiInfinis,
    obtain_star,
    get_yow,
    get_gem,
    get_crown,
    use_yowGem,
    new_level_unlocked,
    // Menu infos UI
    menuInfo_switch,
    btn_close,
    // Menu param
    param_onOffBtn,
    btn_yT_socialMedia,
    // Menu quit
    quitApp_cancel,
}

public enum typeSound_menuShopDefi
{
    none,
    btn_section,
    btn_voyantBoost,
    btn_itemBoost,
    btn_useInfos,
    btn_itemBooster,
    menu_infosBtnSwipe,
    btn_close,
    btn_buyUpgrade,
    upgradeBoost,
    btn_itemSkin,
    btn_getYowGemDaily,
    // defi menu
    btn_getDefiReward,
    // openBooster menu
    ouverturePacket,
    click_phase,
    // reward menu
    btn_getReward,

    dechirement_packet,
}

public enum typeSound_ingame
{
    none,
    btnStart,
    btnQuit,
    bounce,
    pivot,
    portal,
    death,
    win,
    superPower,
    catchBonus,
    getYow,
    getLife,
    gameOverGetLife,
    gameOverClose,
    zoom_level,
    checkP_hang,
    checkP_release,
    // defi
    tuto_ok_help,

    gameOverWin,

    boostNormal,
}

[System.Serializable]
public struct soundItem
{
    public typeSound_menuP _typeSound_menuP;
    public typeSound_ingame _typeSound_ingame;
    public typeSound_menuShopDefi _typeSound_shopDefi;
    public typeSound_online _typeSound_online;
    public AudioClip _soundAudio;
    public float _volume;
}


public class sfxManager : MonoBehaviour
{
    /*public soundItem[] _sound_MenuP_Param_Infos;
    public soundItem[] _sound_Shop_Defi;
    public soundItem[] _sound_Ingame;
    public soundItem[] _sound_Online;

    private static sfxManager instance;
    //public static bool sonOn = true;

    public static sfxManager getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("SFX").GetComponent<sfxManager>();
        }
        return instance;
    }

    public void playSound_ingame(typeSound_ingame tS_ingame)
    {
        //if (!sonOn)
        //    return;
        // on cherche le son dans la liste
        int i = 0;
        while (_sound_Ingame[i]._typeSound_ingame != tS_ingame && i < _sound_Ingame.Length)
            i++;
        // si le son a été trouvé
        if(i < _sound_Ingame.Length)
        {
            playSound(_sound_Ingame[i]);
        }
    }

    private float lastCurrencySoundTime = -1f, intervalTimeCurrencySound = 0.2f;
    private List<typeSound_menuP> currencyTypes = new List<typeSound_menuP>() {
        typeSound_menuP.get_yow, typeSound_menuP.get_gem};
    public void playSound_menuP(typeSound_menuP tS_menuP)
    {
        //if (!sonOn)
        //    return;

        if (currencyTypes.Contains(tS_menuP))
        {
            if (lastCurrencySoundTime != -1f
                && Time.time - lastCurrencySoundTime < intervalTimeCurrencySound)
                return;
            lastCurrencySoundTime = Time.time;
        }

        // on cherche le son dans la liste
        int i = 0;
        while (_sound_MenuP_Param_Infos[i]._typeSound_menuP != tS_menuP && i < _sound_MenuP_Param_Infos.Length)
            i++;
        // si le son a été trouvé
        if (i < _sound_MenuP_Param_Infos.Length)
        {
            playSound(_sound_MenuP_Param_Infos[i]);
        }
    }

    public void playSound_shopDefi(typeSound_menuShopDefi tS_shopDefi)
    {
        //if (!sonOn)
        //    return;
        // on cherche le son dans la liste
        int i = 0;
        while (_sound_Shop_Defi[i]._typeSound_shopDefi != tS_shopDefi && i < _sound_Shop_Defi.Length)
            i++;
        // si le son a été trouvé
        if (i < _sound_Shop_Defi.Length)
        {
            playSound(_sound_Shop_Defi[i]);
        }
    }

    private void playSound(soundItem _itemSound)
    {
        createInstanceSound(_itemSound._soundAudio, _itemSound._volume);
    }

    public GameObject instanceSound;

    private void createInstanceSound(AudioClip s, float vol)
    {
        GameObject p = Instantiate(instanceSound);
        p.transform.SetParent(instance.transform);
        p.SetActive(true);

        AudioSource player = p.GetComponent<AudioSource>();
        p.GetComponent<itemSound>().player = player;
        player.volume = vol;
        player.clip = s;
        player.Play();
    }*/
}
