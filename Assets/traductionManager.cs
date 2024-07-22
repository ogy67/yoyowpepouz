using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeTraduction_ranking
{
    none,
    score_player,
    score_player_2,
    current_week,
    prev_week,
    reward_daily,
    top_score,
    namePlayer_createAccount,
    nameUsed_createAccount,
    maintenance,
    maintenance_info,
}
public enum typeTraduction_online
{
    none,
    connec_titre,
    connec_name,
    connec_mail,
    connec_mdp,
    connec_logIn,
    connec_newPlayer,
    connec_succedConnec,
    connec_errorMailNotFound,
    connec_errorPlayerNameShort,
    connec_errorAccountAlreadyExists,
    connec_errorWrongPassword,
    connec_errorNameAlreadyTaken,
    connec_noUserFound,

    infinis_rankingWorld,
    infinis_rankingLocal,
    infinis_errorRanking,

    connec_unknownError
}
public enum typeTraduction_boost
{
    none,

    boost_bouclierNom,
    boost_bouclierInfos,
    boost_superViseeNom,
    boost_superViseeInfos,
    boost_bombeNom,
    boost_bombeInfos,

    boost_pierreTNom,
    boost_pierreTInfos,
    boost_sourisNom,
    boost_sourisInfos,
    boost_tirelireNom,
    boost_tirelireInfos,

    boost_fuseeNom,
    boost_fuseeInfos,
    boost_ressortNom,
    boost_ressortInfos,
    boost_ghostNom,
    boost_ghostInfos,

    boost_medicNom,
    boost_medicInfos,
    boost_ancreNom,
    boost_ancreInfos,
    boost_aimantNom,
    boost_aimantInfos,

    use_doubleClick,
    use_longClick,
    use_activateAutomatic,
    use_alwaysActive,
}
public enum typeTraduction_shop
{
    none,

    shop_titre,
    shop_pocessed,
    shop_notPocessed,
    shop_pan_boostPocessed,
    shop_pan_boostPocessedNumber,
    shop_pan_palet,
    shop_pan_trail,
    shop_pan_fond,
    shop_pan_dailyOffer,
    shop_pan_dailyOfferTime,
    shop_pan_yowOffer,
    shop_pan_gemmeOffer,

    shop_free_offer,
    shop_price_100_gems,
    shop_price_600_gems,

    boost_use,
    boost_infos,
    boost_niveau,
    boost_upgrade,
    boost_lock,

    booster_contientAuMoin,
    booster_packArgent,
    booster_packOr,
    booster_packCrystal,
    booster_common,
    booster_rare,
    booster_epic,
    booster_legendary,

    boost_rarety,
    boost_am_distance,
    boost_am_timeEffect,
    boost_am_nbBounce,
    boost_am_nombre,
    boost_am_nombreYow,
    boost_am_tailleMin,
    boost_am_regenerationTime,
    boost_am_speedMax,

    validAchat_title,

    shop_price_1200_gems,

    shop_pack_debutant_price,
    shop_pack_debutant_titre,
    shop_pack_debutant_value,
    shop_pack_debutant_timeOut,

    shop_pack_tiktok_price,
    shop_pack_tiktok_titre,
    shop_pack_tiktok_timeOut,
}
public enum typeTraduction_app
{
    none,
    // Menu gameOver / revive
    game_over_win,
    game_over_loose,
    game_over_revive,
    // Menu paramètre
    param_titre,
    param_YTTitle,
    param_soonVideo,
    param_versionApp,
    param_YT_boost,
    param_YT_star,
    param_YT_infinite,
    // Menu défi
    defi_titre,
    defi_quotidienTitre,
    defi_paletTitre,
    defi_boosterTitre,
    defi_normalTitre,
    defi_item_crown,
    defi_item_star,
    defi_item_infinis,
    defi_item_bounce,
    defi_item_checkP,
    defi_item_portal,
    defi_item_pivot,
    defi_item_boostNormal,
    defi_item_boostSuperPower,

    defi_newPalet,
    // Menu shop

    notif_newLevel,
    titre_openPack,
    titre_rewardMenu,
    lock_infinis,

    menu_quitTitre,

    online_previewTitre,
    online_previewInfos,
    online_previewBientot,

    new_rewardMenu,

    param_privacyMoreInfo,
    param_privacyMailInfo,
    param_privacyTextCopied,
    param_privacyPrivacyInfo,
    param_privacyPrivacyButton,

    infinis_mode_titre,
    game_over_reviveExpert,
    param_freeGem,
    param_rateGame,
    param_letsGo,

    infinis_survive_mode,

    bugStuck_title,
    bugStuck_info_1,
    bugStuck_info_2,

    game_over_replay,

    reload_account_yes,
    reload_account_no,
    reload_account_validNo,
    reload_account_title,
    reload_account_infoNo,

    delete_account_supp,
    delete_account_validSup,
    delete_account_denySup,
    delete_account_infoSup,
    wait_delete_accountUnity,
    wait_delete_dataUnity,
    wait_delete_accountFirestore,
    wait_delete_dataFirestore,

    delete_account_alreadyDone,
}
public enum typeTraduction // tutoriels
{
    // Tutoriel
    tuto_presentation,
    tuto_pushPalet,
    tuto_tortueClick,
    tuto_bounce,
    tuto_checkP,
    tuto_switchRotation,
    tuto_end,
    tuto_leftHand,
    tuto_rightHand,
    // Tutoriel menu P
    //tuto_UI_param_name,
    //tuto_UI_param_infos,
    //tuto_UI_totCrown_name,
    tuto_UI_totCrown_infos,
    //tuto_UI_progLevel_name,
    tuto_UI_progLevel_infos,
    //tuto_UI_difficulty_name,
    tuto_UI_difficulty_infos,
    //tuto_UI_star_name,
    //tuto_UI_star_infos,
    //tuto_UI_shop_name,
    //tuto_UI_shop_infos,
    //tuto_UI_infinis_name,
    //tuto_UI_infinis_infos,
    //tuto_UI_challenge_name,
    //tuto_UI_challenge_infos,
    // Tutoriel Infinis
    //tuto_UI_online_name,
    //tuto_UI_online_infos,
    tuto_UI_bestInfinis_name,
    tuto_UI_bestInfinis_infos,

    tuto_UI_expertMode_life,
    tuto_UI_expertMode_boost,

    none,

    tuto_ui_multiplayer,
    tuto_ui_comingSoon,
    tuto_ui_recordInfinis,

    tuto_ui_iAm,
    //tuto_UI_rankingInfinis_name,
    //tuto_UI_rankingInfinis_infos,
    // Tutoriel shop
    //tuto_UI_shopVoyant_name,
    //tuto_UI_shopVoyant_infos,
    //tuto_UI_shopBoost_name,
    //tuto_UI_shopBoost_infos,
    //tuto_UI_shopCustom_name,
    //tuto_UI_shopCustom_infos,
    //tuto_UI_offer_name,
    //tuto_UI_offer_infos,
    ui_info_modeExpert,

    tuto_warningWaitAnim,
}

public enum typeLanguage
{
    french,
    english
}

[System.Serializable]
public struct traducValues
{
    public typeLanguage _typeLanguage;
    public string _textValue;
}

[System.Serializable]
public struct traducItem
{
    // true : Text associé ne changera pas de valeure
    // true : on met la valeure initiale et on ne touche plus jamais
    public bool staticTrad;
    public typeTraduction typeTrad;
    public typeTraduction_app typeTradApp;
    public typeTraduction_shop typeTradShop;
    public typeTraduction_boost typeTradBoost;
    public typeTraduction_online typeTradOnline;
    public typeTraduction_ranking typeTradRank;
    public traducValues[] tradValues;
    public Text staticText;
    public bool childToo;
}


public class traductionManager : MonoBehaviour
{
    public traducItem[] tutorielTrad;
    public traducItem[] tutorielUIMenu;
    public traducItem[] gameOverTrad;
    public traducItem[] paramTrad;
    public traducItem[] defiTrad;
    public traducItem[] shopTrad;
    public traducItem[] boostTrad;
    public traducItem[] boostInfosTrad;
    public traducItem[] otherTrad;
    public traducItem[] connectTrad;
    public traducItem[] rankTrad;

    public Text[] shopUnlockText, shopLockText;//, rewardNewText;


    public typeLanguage defautLanguage = typeLanguage.english;
    public typeLanguage currentLanguage;

    private static traductionManager instance;
    public static traductionManager getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("TRADUCTION V2").GetComponent<traductionManager>();
        }
        return instance;
    }

    private void Awake()
    {
        setLanguage();
    }
    private void Start()
    {       
        setStaticText();
        setNewText();
    }

    // modifie la langue en fonction de la langue de l'appareil
    private void setLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.French:
                currentLanguage = typeLanguage.french;
                break;
            default:
                currentLanguage = defautLanguage;
                break;
        }
        //currentLanguage = defautLanguage;
    }

    // A L'INITIALISATION
    // les statics text sont tous de base en français, les reload quand même ?
    private void setStaticText()
    {
        string s;
        foreach (traducItem[] tradArray in 
            new List<traducItem[]>() { tutorielTrad, tutorielUIMenu, gameOverTrad
            , paramTrad, defiTrad, shopTrad, boostTrad, otherTrad, connectTrad, rankTrad })
        {
            foreach (traducItem tI in tradArray)
            {
                if (tI.staticTrad)
                {
                    s = tI.typeTrad != typeTraduction.none ? getTrad(tI.typeTrad, tradArray)
                        : tI.typeTradApp != typeTraduction_app.none ? getTrad_app(tI.typeTradApp, tradArray)
                        : tI.typeTradShop != typeTraduction_shop.none ? getTrad_shop(tI.typeTradShop) 
                        : tI.typeTradRank != typeTraduction_ranking.none ? getTradRank(tI.typeTradRank)
                        : getTrad_online(tI.typeTradOnline);
                    setText(tI.staticText, s, tI.childToo);
                }
            }
        }
        foreach(Text[] t_list in new List<Text[]> { shopUnlockText, shopLockText })
        {
            bool locked = t_list == shopLockText;
            foreach(Text t in t_list)
            {
                t.text = getTradShop(locked ? typeTraduction_shop.shop_notPocessed : typeTraduction_shop.shop_pocessed);
            }
        }
        string _new = getTrad_app(typeTraduction_app.new_rewardMenu, otherTrad);
        _new = _new.Replace("_", "\n");
        foreach (Text t in allTextNew)
        {
            if(t!=null)
                t.text = _new;
        }
    }

    public Text[] allTextNew;
    private string nouveau_fr = "NOUVEAU", nouveau_en = "NEW";
    private void setNewText() // traduit tt les text "NOUVEAU"
    {
        foreach(Text t in allTextNew)
        {
            t.text = currentLanguage == defautLanguage ? nouveau_en : nouveau_fr;
        }
    }

    

    // traduction française occupe tout les index 0 , english 1 ...
    private Dictionary<typeLanguage, int> indexLanguage = new Dictionary<typeLanguage, int>() {
        { typeLanguage.french, 0},
        { typeLanguage.english, 1},
    };

    public string getTradConnec(typeTraduction_online tT_online)
    {
        return getTrad_online(tT_online);
    }
    public string getTradTuto(typeTraduction tT)
    {
        return getTrad(tT, tutorielTrad);
    }

    public string getTradTutoUI(typeTraduction tT)
    {
        return getTrad(tT, tutorielUIMenu);
    }







    public string getTradParam(typeTraduction_app tT_app)
    {
        return getTrad_app(tT_app, paramTrad);
    }

    public string getTradDefi(typeTraduction_app tT_app)
    {
        return getTrad_app(tT_app, defiTrad);
    }

    public string getTradShop(typeTraduction_shop tT_shop)
    {
        return getTrad_shop(tT_shop);
    }

    public string getTradBoost(typeTraduction_boost tT_boost)
    {
        return getTrad_boost(tT_boost, boostInfosTrad);
    }

    public string getTradRank(typeTraduction_ranking tT_rank)
    {
        return getTrad_rank(tT_rank);
    }

    private string getTrad(typeTraduction tT, traducItem[] list)
    {
        foreach (traducItem tI in list)
        {
            if (tI.typeTrad == tT)
            {
                return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
            }
        }
        return "";
    }

    private string getTrad_app(typeTraduction_app tT_app, traducItem[] list)
    {
        foreach (traducItem tI in list)
        {
            if (tI.typeTradApp == tT_app)
            {
                return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
            }
        }
        return "";
    }

    private string getTrad_boost(typeTraduction_boost tT_boost, traducItem[] list)
    {
        foreach (traducItem tI in list)
        {
            if (tI.typeTradBoost == tT_boost)
            {
                return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
            }
        }
        return "";
    }

    private string getTrad_shop(typeTraduction_shop tT_shop)
    {
        foreach (traducItem[] list in new List<traducItem[]>() { shopTrad, boostTrad })
        {
            foreach (traducItem tI in list)
            {
                if (tI.typeTradShop == tT_shop)
                {
                    return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
                }
            }
        }
    
        return "";
    }

    private string getTrad_online(typeTraduction_online tT_online)
    {
        foreach (traducItem[] list in new List<traducItem[]>() { connectTrad })
        {
            foreach (traducItem tI in list)
            {
                if (tI.typeTradOnline == tT_online)
                {
                    return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
                }
            }
        }

        return "";
    }
    private string getTrad_rank(typeTraduction_ranking tT_online)
    {
        foreach (traducItem[] list in new List<traducItem[]>() { rankTrad })
        {
            foreach (traducItem tI in list)
            {
                if (tI.typeTradRank == tT_online)
                {
                    return tI.tradValues[indexLanguage[currentLanguage]]._textValue;
                }
            }
        }

        return "";
    }

    private void setText(Text t, string s, bool childToo)
    {
        if (t == null)
        {
            Debug.Log("<color=red>Problème initialisation text : </color>" + s);
            return;
        }
        s = s.Replace("_", "\n");
        t.text = s;
        if (childToo)
        {
            t.transform.GetChild(0).GetComponent<Text>().text = s;
        }
    }







    private string gaucher_fr = "Gaucher", gaucher_en = "Left";
    private string droitier_fr = "Droitier", droitier_en = "Right";
    public string getTradSideHandParamMenu(bool right)
    {
        if (right)
            return currentLanguage == defautLanguage ? droitier_en : droitier_fr;
        else
            return currentLanguage == defautLanguage ? gaucher_en : gaucher_fr;
    }
    private string[] testDifficultyTrad_fr = new string[] { "TUTORIEL", "FACILE", "MOYEN", "DIFFICILE", "EXPERT", "LEGENDE" };
    private string[] testDifficultyTrad_en = new string[] { "TUTORIAL", "EASY", "MEDIUM", "DIFFICULT", "EXPERT", "LEGEND" };
    public string getTradTutoDifficulty(int levelDifficulty) // de 1 à 4
    {
        return currentLanguage == defautLanguage ? testDifficultyTrad_en[levelDifficulty] : testDifficultyTrad_fr[levelDifficulty];
        //return testDifficultyTrad[levelDifficulty];
    }

    private Dictionary<typeTutoAventure, string> infoTutoAventure_fr = new Dictionary<typeTutoAventure, string>
    {
        {typeTutoAventure.crown, "Reccuperes les<color=\"#FFB800\"> 3 COURONNES</color> du niveau" },
        {typeTutoAventure.bounce, "Rebondis sur les<color=\"#00ECFF\"> MURS BLEUS</color>" },
        {typeTutoAventure.pivot, "Le<color=\"#00FF2A\"> PIVOT</color> redirige automatiquement le palet" },
        {typeTutoAventure.portal, "Passes au travers des<color=\"#FF00FC\"> PORTAILS</color>" },
         {typeTutoAventure.push, "Envoies le palet dans la cible" },
        {typeTutoAventure.slow, "Appuies sur<color=\"#FFB800\"> B</color> pour activer la precision de tir" },
        {typeTutoAventure.checkP, "Restes appuyé sur<color=\"#FFB800\"> A</color>" +
            " pour t'accrocher au <color=\"#FF4F50\">Cercle Rouge</color>" },
        {typeTutoAventure.switchR, "Glisses<color=\"#FFB800\"> A</color> pour inverser le sens de rotation" },
    };
    private Dictionary<typeTutoAventure, string> infoTutoAventure_en = new Dictionary<typeTutoAventure, string>
    {
        {typeTutoAventure.crown, "Collect the<color=\"#FFB800\"> 3 CROWNS</color> of the level" },
        {typeTutoAventure.bounce, "Bounce off the<color=\"#00ECFF\"> BLUE WALLS</color>" },
        {typeTutoAventure.pivot, "The<color=\"#00FF2A\"> PIVOT</color> automatically redirects the puck" },
        {typeTutoAventure.portal, "Pass threw<color=\"#FF00FC\"> PORTALS</color>" },
         {typeTutoAventure.push, "Throw the puck into the target" },
        {typeTutoAventure.slow, "Click on<color=\"#FFB800\"> B</color> to activate puck throw precision" },
        {typeTutoAventure.checkP, "Keep pressing<color=\"#FFB800\"> A</color>" +
            " to hang on the <color=\"#FF4F50\">Red Circle</color>" },
        {typeTutoAventure.switchR, "Slide<color=\"#FFB800\"> A</color> to reverse the direction of rotation"},
    };

    public string getTradInfoTutoAventure(typeTutoAventure type)
    {
        return currentLanguage == defautLanguage ? infoTutoAventure_en[type] : infoTutoAventure_fr[type];
    }

    private Dictionary<typeResultDeleteAccount, string> statusDeleteAccount_fr = new Dictionary<typeResultDeleteAccount, string>
    {
        {typeResultDeleteAccount.result_error, "Erreur" },
        {typeResultDeleteAccount.result_success, "Supprimé" },
        {typeResultDeleteAccount.result_noData, "Aucun(e)" },
        {typeResultDeleteAccount.result_noConnexion, "Pas connecté" },
    };
    private Dictionary<typeResultDeleteAccount, string> statusDeleteAccount_en = new Dictionary<typeResultDeleteAccount, string>
    {
        {typeResultDeleteAccount.result_error, "Error" },
        {typeResultDeleteAccount.result_success, "Deleted" },
        {typeResultDeleteAccount.result_noData, "None" },
        {typeResultDeleteAccount.result_noConnexion, "Not connected" },
    };

    public string getTradStatusDeleteAccount(typeResultDeleteAccount type)
    {
        return currentLanguage == defautLanguage ? statusDeleteAccount_en[type] : statusDeleteAccount_fr[type];
    }


    private string[] letterDateList_fr = new string[] { "j", "h", "m" };
    private string[] letterDateList_en = new string[] { "d", "h", "m" };

    public string[] getListLetterDate()
    {
        return currentLanguage == defautLanguage ? letterDateList_en : letterDateList_fr;
    }
}
