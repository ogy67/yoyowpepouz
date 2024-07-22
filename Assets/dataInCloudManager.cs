using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using System.Threading.Tasks;
using System.Linq;
using System;

public static class ReverseDictionary
{
    public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this IDictionary<TKey, TValue> source)
    {
        var dictionary = new Dictionary<TValue, TKey>();
        foreach (var entry in source)
        {
            if (!dictionary.ContainsKey(entry.Value))
                dictionary.Add(entry.Value, entry.Key);
        }
        return dictionary;
    }
}

// ici, gérer la save des données de player dans Unity Cloud
public class dataInCloudManager : MonoBehaviour
{
    public static bool isConnectedToAnAccount = false;

    // initial, appelé quand TEST connexion avec Google / IOS, est finit
    public void endWaitConnecToAccount()
    {
        // - est connecté à un compte
        if (isConnectedToAnAccount)
        {
            nameSaveInCloud_reversed = nameSaveInCloud.Reverse();
            initTimeUpdate = Time.time;
            initializeUnityCloudData();
        }
        // - aucune connexion à un compte
        else
        {
            // sinon cash lancer l'app
            launcher.getInstance().endCheckCloudAccount();
        }
    }

    /* appelé depuis menu param, si suppression a réussit
    public static void playerDeletedUnityAccount()
    {
        isConnectedToAnAccount = false;
        cloudSave_idFirestore = "";
        cloudSave_poidSave = 0;
        onlineRankingManager.playerHasChangedDevice = false;
    }*/

    /*
     *  async void initializeUnityCloudData() 
     *  async Task TestPlayerHasDataInBdd()
     *  public void btnReloadData()
     *  async void ReplaceLocalData_withCloudData()
     *  async Task ReplaceCloudData_withLocalData()
     *  async void DeleteCloudData()
    */
    #region INITIALISATION DES DONNEES DE UnityCloud ET Local

    public menuReloadCloudSave menuReload;
    private bool cloudSave_hasData = false;
    private bool cloudSave_canReplace = false;
    private static int cloudSave_poidSave = 0;
    private static int localSave_poidSave = 0;
    public static string cloudSave_idFirestore = "";

    // (1) regarde si save dans UnityCloud
    // -- OUI : regarde si saveCloud + recente
    // (2)--- OUI : proposer reload data
    // (3)--- NON : check saveCloud à jour
    // ---------- OUI : rien faire
    // (4)------- NON : mettre à jour saveCloud
    // -- NON : rien faire, se sauvegardera + tard

    async void initializeUnityCloudData() 
    {
        try
        {
            await TestPlayerHasDataInBdd();// (1)
        }
        catch (Exception e)
        {
            Debug.Log("Erreur quand test player has an account");
            launcher.getInstance().endCheckCloudAccount();
            return;
        }

        if (cloudSave_canReplace) // (2)
        {
            Debug.Log("Try open menu Reload account");
            // ouvrir le menu 'proposition de load cloud save'
            menuReload.openMenu();
        }
        else // (3)
        {
            // si !cloudSave_hasData, alors poidSaveCloud(local) = 0
            // check si peut mettre cloud à jour avec dataLocal
            if (!cloudSave_hasData && localSave_poidSave > 0
                || localSave_poidSave > cloudSave_poidSave)
            {
                cloudSave_poidSave = localSave_poidSave;
                try
                {
                    await CloudSaveService.Instance.Data.Player.SaveAsync(getAllInfos_initGlobalUpdate());// (4)
                }
                catch (Exception)
                {
                    Debug.Log("Erreur quand essaie mettre à jour dataPlayer dans UnityCloud");
                }
            }
            // -> appeler launcher pour lancer APP
            launcher.getInstance().endCheckCloudAccount();
        }
    }

    private async Task TestPlayerHasDataInBdd()// (1)
    {
        // On regarde si player a déjà des infos dans bdd
        //string id_nom_version = nameSaveInCloud[typeSave.GAME_VERSION];
        string id_nb_save = nameSaveInCloud[typeSave.NB_SAVE_DONE];
        string id_firestore_id = nameSaveInCloud[typeSave.ID_PLAYER_FIRESTORE];
        localSave_poidSave = (int)SaveSystem.loadData(typeSave.NB_SAVE_DONE);

        Dictionary<string, Item> playerData;
        try
        {
            playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            //id_nom_version,
            id_nb_save,
            id_firestore_id,
            });
        }
        catch(Exception) 
        {
            Debug.Log("Erreur quand test player has an account (2)");
            cloudSave_hasData = false;
            cloudSave_poidSave = 0;
            cloudSave_canReplace = false;
            cloudSave_idFirestore = "";
            return;
        }
        
        if (playerData.TryGetValue(id_nb_save, out var keyNbSave)) // si n'a meme pas ce file -> aucune save
        {
            // -> player a déjà des infos dans bdd
            cloudSave_hasData = true;

            // En fonction de save du cloud, proposer de réccupérer les infos du compte
            cloudSave_poidSave = keyNbSave.Value.GetAs<int>();
            cloudSave_canReplace = cloudSave_poidSave > 0 && cloudSave_poidSave > localSave_poidSave;
            
            if (playerData.TryGetValue(id_firestore_id, out var keyIdFirestore))
            {
                cloudSave_idFirestore = keyIdFirestore.Value.GetAs<string>();
            }
        }
    }

    public void btnReloadData(bool reload)
    {
        if (reload)
        {
            ReplaceLocalData_withCloudData();
        }
        else
        {
            // => on save directement UnityCloud:poidSave à 0 :
            // -> ne propose donc plus de remplacement de compte à init,
            // les données seront donc écrasées à la prochaine maj
            // local:idFirestore n'ecrasera pas UnityCloud:idFirestore, parce que uniquement set quand passe AccountPhase de ranking = account BIEN reload si besoin 
            DeleteCloudData();

            // -> appeler launcher pour lancer APP
            launcher.getInstance().endCheckCloudAccount();
        }
    }

    // appelé quand clique sur btn reload
    async void ReplaceLocalData_withCloudData()// (2)
    {
        HashSet<string> keys = new HashSet<string>();
        foreach(KeyValuePair<typeSave, string> k in nameSaveInCloud)
        {
            keys.Add(k.Value);
        }

        Dictionary<string, Item> playerData = null;
        try
        {
            playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
        }
        catch (Exception)
        {
            // -> appeler launcher pour lancer APP
            launcher.getInstance().endCheckCloudAccount();
            return;
        }

        // pour chaque donnée dans liste, l'enregistrer en local
        foreach(string s in keys)
        {
            if (playerData.TryGetValue(s, out var key))
            {
                // on save en local
                SaveSystem.Save(nameSaveInCloud_reversed[s], key.Value.GetAs<object>());
            }
        }

        // -> appeler launcher pour lancer APP
        launcher.getInstance().endCheckCloudAccount();
    }

    // Cette fonction met juste à 0 le nombre de save de bdd
    // => ne sera plus jamais considéré comme ancienne save
    async void DeleteCloudData()
    {
        Dictionary<string, object> newPoid = new Dictionary<string, object>() { { nameSaveInCloud[typeSave.NB_SAVE_DONE], 0 } };
        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(newPoid);
        }
        catch (Exception)
        {
            // pas de return, sinon nbSaveCloud local n'est pas mit à jour
        }
        cloudSave_poidSave = 0;
    }

    #endregion

    /*
     * void playerConnectedToGooglePlay(string token, string playerUnityAuthId)
     * void playerConnectedToICloud()
     * static void playerAutoIdentifiedToFirestore(string id_firestore)
     * static void playerPassedAccountPhase(bool hasReloadPreviousAccount)
    */
    #region APPELS DE CONNEXION REUSSIT

    // Android
    // quand appelé, token google retrieve, et connecté à Unity Auth
    public void playerConnectedToGooglePlay()//string token, string playerUnityAuthId)
    {
        isConnectedToAnAccount = true;
    }

    // IOS
    // à implementer + tard
    public void playerConnectedToICloud()
    {
        isConnectedToAnAccount = true;
    }

    private static string current_id_firestore;  // local
    // appelé par FireStore quand auto auth succed
    // UnityAuth DOIT ETRE prioritaire par rapport à FireStoreAuth -> launcher attend fin initialisation data
    public static void playerAutoIdentifiedToFirestore(string id_firestore)
    {
        // Quand change de device : UnityCloud:idFirestore est set ( sauf si ... (2) )
        // Firestore:id change
        // ( logiquement pas encore de save de Firestore:id en local )

        // (2) Si UnityCloud:idFirestore n'est pas set
        // Veut dire que, dans ancienne partie, n'a pas save Firestore:id en local
        // -> n'a jamais été connecté à internet

        // On save Firestore:id dans UnityCloud:idFirestore quand : 
        // - assez d'update
        // - init app met toutes les infos à jours

        // On save Firestore:id en local quand : 
        // NON - se connecte à Firestore ( le faire directement, ne pose aucun problème, on ne se refere jamais à la save )
        // Passe étape création/remplacement compte

        current_id_firestore = id_firestore;
        // si n'est pas connecté à unity cloud, on ne fait rien du tout
        if(isConnectedToAnAccount)
        {
            // si UnityCloud:idFirestore est différent de Firestore:id
            // On doit alors changer les infos de player dans Firestore ( quand ouvrira ranking )
            if (cloudSave_idFirestore != "" && cloudSave_idFirestore != id_firestore)
            {
                onlineRankingManager.playerHasChangedDevice = true;
            }
            // sinon, veut dire que n'avait pas de compte avant -> process normal de creation de compte
        }

        // V (1) Si ne se connecte pas directement UnityCloud et va jusqu'à créer compte dans Firestore
        // Quand re-ouvrira app, et se connectera UnityCloud
        // -> player aura déjà créer un compte dans bdd
        // Imaginons ici que dans UnityCloud, autre idFirestore différent
        // -> ce n'est pas un changement de compte, c'est UNE SUPPRESION ANCIEN COMPTE seulement
        // => RIEN faire de spécial, les données firestore seront simplement écrasé ( et pas crée pour le coup )



        // V (2, lié à 1) Perdre ancien idFirestore (:UnityCloud) avant d'avoir fais les maj
        // Imaginons init app, UnityCloud:idFirestore != Firestore:idFirestore
        // -> save le nouvel id en local et attend ouverture de ranking pour changer compte
        // -> le poid de update:UnityCloud lance une update, idFirestore est set dans UnityCloud
        // -> NE LANCE PAS LE RANKING, reset app, revient, UnityCloud:idFirestore maintenant = Firestore:idFirestore => PAS DE CHANGEMENT
        // => A PERDU ANCIEN ID et ne peut pas supprimer ancien compte
        // ===> Seulement save id ranking en local et cloud une fois qu'a complété le processus de création/changement/auth de compte

        // V (3) Player met à jour, à partir d'une version qui n'avait pas UnityCloud installé
        // -> Tt se passe normal

    }


    // => BLOQUER l'appel à une seule et unique fois par session, lors de première connexion au ranking
    // - Est appelé après ( par ordre de priorité ) replaceAccount /ou hasAccountInFirestore /ou createAccount
    // - Les infos sont alors bien à jour dans bddFirestore -> peut les save dans UnityCloud ( ET DONC FAIRE 1 SEULE FOIS )
    private static bool hasAlreadyConnectedToRankingThisSession = false;
    public static void playerPassedAccountPhase(bool hasReloadPreviousAccount) // appelé par ranking quand s'est connecté à firestore
    {
        if (isConnectedToAnAccount && !hasAlreadyConnectedToRankingThisSession)
        {
            hasAlreadyConnectedToRankingThisSession = true;

            // si reload account && id_firestore était set dans UnityCloud -> id_firestore est saved en local quand reload compte
            // MAIS SI différent de firestore:idfirestore
            // ne vas pas dans 'if' (1) ( est déjà saved )
            // ne va pas dans 'else if' (2) ( n'est pas vide )
            // => alors check si différent de Firestore, Oui -> save dans UnityCloud
            if (hasReloadPreviousAccount)
            {
                aSaveChanged(typeSave.ID_PLAYER_FIRESTORE, current_id_firestore, poidSave[typeSave.ID_PLAYER_FIRESTORE]);
            }
            else // si n'a pas reload prev account, n'a pas save en local UnityCloud:firestoreId ( peut quand même déjà y avoir une save )
            {
                // si pour cet appareil, on a pas encore save l'id firestore, on le fait ici ( ne changera plus jamais )
                string id_saved = (string)SaveSystem.loadData(typeSave.ID_PLAYER_FIRESTORE);
                if (id_saved == "") // (1)
                    SaveSystem.Save(typeSave.ID_PLAYER_FIRESTORE, current_id_firestore); // (ajouté par SaveSYstem dans liste attente )

                // => tant que n'est pas set dans cloud, ajouter à la liste update à init
                // ' else if ' , parce que si entre dans if du dessus, alors est déjà dans liste
                else if (cloudSave_idFirestore == "") // (2)
                    aSaveChanged(typeSave.ID_PLAYER_FIRESTORE, id_saved, poidSave[typeSave.ID_PLAYER_FIRESTORE]);
            }
        }
    }

    #endregion


    /*
     * static void aSaveChanged(typeSave type, object infos, int poidSaveLocal)
     * static async void updateCloudData(int poidSaveLocal)
     * Dictionary<string, object> localPlayerInfos_toList()
    */
    #region GESTION UPDATE UNITYCLOUD quand localData change

    private static Dictionary<typeSave, string> nameSaveInCloud = new Dictionary<typeSave, string>()
    {
        { typeSave.GAME_VERSION, "GAME_VERSION" },
        { typeSave.TUTO_MENUP, "TUTO_MENUP" },
        { typeSave.DEFI_PROG, "DEFI_PROG" },
        { typeSave.PARAM_STATE, "PARAM_STATE" },
        { typeSave.PARAM_FREE_GEM_TAKEN, "PARAM_FREE_GEM_TAKEN" },

        { typeSave.STAR, "STAR" },
        { typeSave.BEST_SCORE, "BEST_SCORE" },
        { typeSave.MODE_EXPERT_ON, "MODE_EXPERT_ON" },
        { typeSave.PROG_LEVEL_REWARD, "PROG_LEVEL_REWARD" },
        { typeSave.SPECIAL_OFFER_INFO, "SPECIAL_OFFER_INFO" },

        { typeSave.MONNEY, "MONNEY" },
        { typeSave.GEMMES, "GEMMES" },

        { typeSave.RECORD_INFINIS, "RECORD_INFINIS" },
        { typeSave.SCORES_INFINIS_ONLINE, "SCORES_INFINIS_ONLINE" },
        { typeSave.RECORD_INFINIS_ONLINE, "RECORD_INFINIS_ONLINE" },

        { typeSave.MISSED_ACHIEVMENTS, "MISSED_ACHIEVMENTS" },
        { typeSave.ENSURE_REWARD_BUY, "ENSURE_REWARD_BUY" },

        { typeSave.SHOP_BOOST, "SHOP_BOOST" },
        { typeSave.SHOP_BOOST_SELECTED, "SHOP_BOOST_SELECTED" },
        { typeSave.SHOP_TRAILS, "SHOP_TRAILS" },
        { typeSave.SHOP_PION, "SHOP_PION" },
        { typeSave.SHOP_SELECTED_SKINS, "SHOP_SELECTED_SKINS" },
        { typeSave.SHOP_DAILY_OFFER, "SHOP_DAILY_OFFER" },
        { typeSave.SHOP_DAILY_DATE, "SHOP_DAILY_DATE" },
        { typeSave.SHOP_DAILY_ID_BOOST, "SHOP_DAILY_ID_BOOST" },
        { typeSave.SHOP_DAILY_OFFER_TAKEN, "SHOP_DAILY_OFFER_TAKEN" },
        { typeSave.SHOP_DAILY_DATE_INIT, "SHOP_DAILY_DATE_INIT" },

        { typeSave.MANETTE_SIDE_HAND, "MANETTE_SIDE_HAND" },
        { typeSave.MANETTE_POS_BTN, "MANETTE_POS_BTN" },

        { typeSave.NB_SAVE_DONE, "NB_SAVE_DONE" },
        { typeSave.ID_PLAYER_FIRESTORE, "ID_PLAYER_FIRESTORE" },

        { typeSave.PACK_OFFER_INFO, "PACK_OFFER_INFO" },

        { typeSave.ENSURE_PACK_GIVEN, "ENSURE_PACK_GIVEN" },
    };
    private Dictionary<string, typeSave> nameSaveInCloud_reversed = new Dictionary<string, typeSave>();

    // 1 : très fréquent ( 1 min )
    // 5 : fréquent ( 5 min )
    // 10 : rare ( 15 min )
    // 20 : très rare et très important
    //private static int currentPoidUpdate = 0;
    private static int limitPoidToUpdate = 200; // faire rarement pendant que joue
    private static int timeMaxToUpdate = 60 * 25;
    private static int nbUpdateCalled = 0; // TEST
    public static Dictionary<typeSave, int> poidSave = new Dictionary<typeSave, int>()
    {
        { typeSave.GAME_VERSION, 10 },
        { typeSave.TUTO_MENUP, 5 },
        { typeSave.DEFI_PROG, 1 },
        { typeSave.PARAM_STATE, 1 },
        { typeSave.PARAM_FREE_GEM_TAKEN, 10 },

        { typeSave.STAR, 1 },
        { typeSave.BEST_SCORE, 1 },
        { typeSave.MODE_EXPERT_ON, 5 },
        { typeSave.PROG_LEVEL_REWARD, 10 },
        { typeSave.SPECIAL_OFFER_INFO, 1 },

        { typeSave.MONNEY, 1 },
        { typeSave.GEMMES, 1 },

        { typeSave.RECORD_INFINIS, 5 },
        { typeSave.SCORES_INFINIS_ONLINE, 5 },
        { typeSave.RECORD_INFINIS_ONLINE, 5 },

        { typeSave.MISSED_ACHIEVMENTS, 20 },
        { typeSave.ENSURE_REWARD_BUY, 20 },

        { typeSave.SHOP_BOOST, 1 },
        { typeSave.SHOP_BOOST_SELECTED, 1 },
        { typeSave.SHOP_TRAILS, 5 },
        { typeSave.SHOP_PION, 5 },
        { typeSave.SHOP_SELECTED_SKINS, 1 },
        { typeSave.SHOP_DAILY_OFFER, 10 },
        { typeSave.SHOP_DAILY_DATE, 5 },
        { typeSave.SHOP_DAILY_ID_BOOST, 5 },
        { typeSave.SHOP_DAILY_OFFER_TAKEN, 5 },
        { typeSave.SHOP_DAILY_DATE_INIT, 10 },

        { typeSave.MANETTE_SIDE_HAND, 1 },
        { typeSave.MANETTE_POS_BTN, 1 },

        { typeSave.NB_SAVE_DONE, 0 },
        { typeSave.ID_PLAYER_FIRESTORE, 20 },

        { typeSave.PACK_OFFER_INFO, 5 },

        { typeSave.ENSURE_PACK_GIVEN, 5 },
    };

    // quand on lance une update, on le fait seulement pour les saves mise à jour
    private static Dictionary<string, object> saveWaitingNewUpdate = new Dictionary<string, object>();

    private static float initTimeUpdate;
    public static void aSaveChanged(typeSave type, object infos, int poidSaveLocal)
    {
        string k = nameSaveInCloud[type];
        if (saveWaitingNewUpdate.ContainsKey(k))
            saveWaitingNewUpdate[k] = infos;
        else
            saveWaitingNewUpdate.Add(k, infos);

        localSave_poidSave = poidSaveLocal;
        //Debug.Log("<color=green>New Poid Update : </color>" + poidSaveLocal);

        if (isConnectedToAnAccount)
        {
            bool update = false;
            if (type == typeSave.ID_PLAYER_FIRESTORE)
            {
                // force save
                Debug.Log("<color=blue>ID firestore change, update player in UnityCloud</color>");
                update = true;
            }
            else
            {
                if (Time.time > initTimeUpdate + timeMaxToUpdate
                    && localSave_poidSave > cloudSave_poidSave)
                {
                    // force save
                    Debug.Log("<color=blue>Time out, update player in UnityCloud</color>");
                    update = true;

                }
                else if (localSave_poidSave > cloudSave_poidSave + limitPoidToUpdate)
                {
                    Debug.Log("<color=blue>Max Poid atteind (nbUpdate déjà faite : " + nbUpdateCalled + ")</color>");
                    update = true;
                }
            }

            if (update)
            {
                initTimeUpdate = Time.time;
                nbUpdateCalled++;
                // ajouter à la liste de mise à jour, le nombre de save local
                saveWaitingNewUpdate.Add(nameSaveInCloud[typeSave.NB_SAVE_DONE], poidSaveLocal);
                // mettre à jour
                updateCloudData(poidSaveLocal);
            }
        }
    }
    static async void updateCloudData(int poidSaveLocal)
    {
        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(saveWaitingNewUpdate);
        }catch (Exception)
        {
            return; // sinon reset les saves à envoyer ( juste en dessous )
        }
        // vider liste
        saveWaitingNewUpdate = new Dictionary<string, object>();
        cloudSave_poidSave = poidSaveLocal;
    }

    private Dictionary<string, object> getAllInfos_initGlobalUpdate()
    {
        // ici, seulement mettre les save que l'on veut mettre à jour à chaque initialisation

        // enlevé : ne doit pas être automatique, seulement quand valide connexion de compte
        // -> sinon peut ecraser UnityCloud:IdFirestore avec saveLocal="" ( si ne s'est jamais connecté (firestore) avec cette device )
        //{ "ID_PLAYER_FIRESTORE", SaveSystem.loadData(typeSave.ID_PLAYER_FIRESTORE)}, // ajouter id de connexion FireStore
        return new Dictionary<string, object>()
        {
            { "GAME_VERSION", SaveSystem.loadData(typeSave.GAME_VERSION)},
            { "TUTO_MENUP", SaveSystem.loadData(typeSave.TUTO_MENUP)},
            { "DEFI_PROG", SaveSystem.loadData(typeSave.DEFI_PROG)},
            { "PARAM_STATE", SaveSystem.loadData(typeSave.PARAM_STATE)},
            { "PARAM_FREE_GEM_TAKEN", SaveSystem.loadData(typeSave.PARAM_FREE_GEM_TAKEN)},

            { "STAR", SaveSystem.loadData(typeSave.STAR)},
            { "BEST_SCORE", SaveSystem.loadData(typeSave.BEST_SCORE)},
            { "MODE_EXPERT_ON", SaveSystem.loadData(typeSave.MODE_EXPERT_ON)},
            { "PROG_LEVEL_REWARD", SaveSystem.loadData(typeSave.PROG_LEVEL_REWARD)},
            { "SPECIAL_OFFER_INFO", SaveSystem.loadData(typeSave.SPECIAL_OFFER_INFO)},

            { "MONNEY", SaveSystem.loadData(typeSave.MONNEY)},
            { "GEMMES", SaveSystem.loadData(typeSave.GEMMES)},

            { "RECORD_INFINIS", SaveSystem.loadData(typeSave.RECORD_INFINIS)},
            { "SCORES_INFINIS_ONLINE", SaveSystem.loadData(typeSave.SCORES_INFINIS_ONLINE)},
            { "RECORD_INFINIS_ONLINE", SaveSystem.loadData(typeSave.RECORD_INFINIS_ONLINE)},

            { "MISSED_ACHIEVMENTS", SaveSystem.loadData(typeSave.MISSED_ACHIEVMENTS)},
            { "ENSURE_REWARD_BUY", SaveSystem.loadData(typeSave.ENSURE_REWARD_BUY)},

            { "SHOP_BOOST", SaveSystem.loadData(typeSave.SHOP_BOOST)},
            { "SHOP_BOOST_SELECTED", SaveSystem.loadData(typeSave.SHOP_BOOST_SELECTED)},
            { "SHOP_TRAILS", SaveSystem.loadData(typeSave.SHOP_TRAILS)},
            { "SHOP_PION", SaveSystem.loadData(typeSave.SHOP_PION)},
            { "SHOP_SELECTED_SKINS", SaveSystem.loadData(typeSave.SHOP_SELECTED_SKINS)},
            { "SHOP_DAILY_OFFER", SaveSystem.loadData(typeSave.SHOP_DAILY_OFFER)},
            { "SHOP_DAILY_DATE", SaveSystem.loadData(typeSave.SHOP_DAILY_DATE)},
            { "SHOP_DAILY_ID_BOOST", SaveSystem.loadData(typeSave.SHOP_DAILY_ID_BOOST)},
            { "SHOP_DAILY_OFFER_TAKEN", SaveSystem.loadData(typeSave.SHOP_DAILY_OFFER_TAKEN)},
            { "SHOP_DAILY_DATE_INIT", SaveSystem.loadData(typeSave.SHOP_DAILY_DATE_INIT)},

            { "MANETTE_SIDE_HAND", SaveSystem.loadData(typeSave.MANETTE_SIDE_HAND)},
            { "MANETTE_POS_BTN", SaveSystem.loadData(typeSave.MANETTE_POS_BTN)},
           
            // ajouter nb save faite
            { "NB_SAVE_DONE", SaveSystem.loadData(typeSave.NB_SAVE_DONE)},
            { "PACK_OFFER_INFO", SaveSystem.loadData(typeSave.PACK_OFFER_INFO)},
            { "ENSURE_PACK_GIVEN", SaveSystem.loadData(typeSave.ENSURE_PACK_GIVEN) },
        };
    }

    #endregion
}
