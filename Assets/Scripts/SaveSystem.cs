using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;

public enum typeSave
{
    // V : appelé init sans action de player
    // regarder quelle save pourraient être corrompu ( et faire crash app )

    GAME_VERSION,// V-ok
    PARAM_STATE,
    PARAM_FREE_GEM_TAKEN,
    MONNEY,// V-ok
    GEMMES,// V-ok
    STAR,// V
    BEST_SCORE,// V
    RECORD_INFINIS,// V
    SCORES_INFINIS_ONLINE,
    RECORD_INFINIS_ONLINE,

    MISSED_ACHIEVMENTS,// V

    FIRESTORE_AUTH,
    FIRESTORE_PREV_WEEK_RANK,
    FIRESTORE_DEV,
    DEV_LIST_BOT,

    SHOP_BOOST,// V // contient data level
    SHOP_BOOST_SELECTED,// V
    SHOP_TRAILS,// V
    SHOP_PION,// V
    SHOP_SELECTED_SKINS,// V
    SHOP_DAILY_OFFER,// V
    SHOP_DAILY_DATE,// V
    SHOP_DAILY_ID_BOOST,// V
    SHOP_DAILY_OFFER_TAKEN,// V
    SHOP_DAILY_DATE_INIT,// V

    DEFI_PROG,// V

    MANETTE_SIDE_HAND,// V
    MANETTE_POS_BTN,// V

    SPECIAL_OFFER_INFO,// V
    ENSURE_REWARD_BUY,// V

    TUTO_MENUP,

    PROG_LEVEL_REWARD,

    MODE_EXPERT_ON,// V-ok

    NB_SAVE_DONE,
    ID_PLAYER_FIRESTORE,

    PACK_OFFER_INFO,

    ENSURE_PACK_GIVEN


    //ALL_TUTOS,
    //SHOP_FONDS,
    //ONLINE_NB_TROPHE,
    //ONLINE_GAME_ON,
    //ONLINE_NB_TICKETS,
    //ONLINE_ILLIMITED_TICKETS,
    //ONLINE_PLAYER_NAME,
    //ONLINE_AUTH_VALS,
    //PION_UNLOCKED_ALL,
    //SPEED_ROT,
    //TUTO,
    //DEFI,
}
class dataPage
{
    public string path;
    public object values;

    public dataPage(string s, object v)
    {
        path = s;
        values = v;
    }
}

public static class SaveSystem
{
    private static string code_version_game = "b-6.0.0";

    // "b-5.0.2" pour test, puis laisser nouvelle maj 
    // List des version qui ne vont pas entrainer une supression des données ( autres, non valides )
    // -> ajout 4 nouveau level, doit rebuild save players qui ont pas encore cette version
    // private static List<string> validVersion = new List<string>() { "b-5.0", "b-5.0.1", "b-5.0.2" };


    /*public static void testDeleteAllDatas()
    {
        Debug.Log("Path to data : " + Application.persistentDataPath);
        foreach(var name in Enum.GetNames(typeof(typeSave)))
        {
            typeSave s = (typeSave)Enum.Parse(typeof(typeSave), name);
            Debug.Log("Data deleted : " + s);
            if (dico.ContainsKey(s)
                && File.Exists(Application.persistentDataPath + dico[s].path))
            {
                Debug.Log("-- File exists, delete");
                File.Delete(Application.persistentDataPath + dico[s].path);
            }
        }
    }*/
    // la version "b-5.0" est la version à partir de laquelle les saves ne pourront plus jamais être supprimé
    /* si player avait une ancienne version du jeu, on supprime toutes les données qu'il avait
    public static void checkDataSavedNotCorrupted()
    {
        string prevSaveNumber = (string)loadData(typeSave.GAME_VERSION);
        if(prevSaveNumber != "")
        {
            if (!validVersion.Contains(prevSaveNumber))
            {
                Debug.Log("<color=red>Delete all previous saved files</color>");
                // on reset toutes les datas de player
                foreach (var name in Enum.GetNames(typeof(typeSave)))
                {
                    deleteFileSave((typeSave)Enum.Parse(typeof(typeSave), name));
                    /*Debug.Log("Data deleted : " + s);
                    if (dico.ContainsKey(s)
                        && File.Exists(Application.persistentDataPath + dico[s].path))
                    {
                        Debug.Log("-- File exists, delete");
                        File.Delete(Application.persistentDataPath + dico[s].path);
                    }
                }
            }
        }
    }

    */

    public static void deleteFileSave(typeSave s)
    {
        Debug.Log("Data deleted : " + s);
        if (dico.ContainsKey(s)
            && File.Exists(Application.persistentDataPath + dico[s].path))
        {
            Debug.Log("-- File exists, delete");
            File.Delete(Application.persistentDataPath + dico[s].path);
        }
    }

    //private static int numberOfLevel = 38; // SUPPRIMER , se fait automatiquement
    //private static int nbPions = 9;
    public static int nbNiveauAventure = 80;
    //public static int nbPionsBuy = 7;

    public static int nb_couronne = 0;
    public static int record_infinis = 0;
    /*public static int nb_bounce = 0;
    public static int nb_checkP = 0;
    
    public static int nb_bounce_SB = 0;
    public static int nb_pivot = 0;
    public static int nb_portal = 0;*/

    public static List<object> missedGooglePlayValues; // crown - infinis - star - allIsOk


    public static int nbStarObtain = -1;
    public static void playerGetStar()
    {
        nbStarObtain++;
    }
    public static int getNbStarObtain()
    {
        if (nbStarObtain == -1)
        {
            bool[] unlocked = (bool[])loadData(typeSave.STAR);
            int tot = 0;
            for (int i = 0; i < unlocked.Length; i++)
            {
                if (unlocked[i]) tot++;
            }
            nbStarObtain = tot;
        }
        return nbStarObtain;
    }


    private static Dictionary<typeSave, dataPage> dico = new Dictionary<typeSave, dataPage>()
    {
        {typeSave.GAME_VERSION, new dataPage("/game_version.data", "")},
        {typeSave.TUTO_MENUP, new dataPage("/tutoMenuP.data", new List<bool>(){ false, false, false, false, false, false, false}) },  // /!\ si taille liste augmente
        {typeSave.DEFI_PROG, new dataPage("/defiProg.date", null) },
        {typeSave.PARAM_STATE, new dataPage("/param.data", null)},
        {typeSave.PARAM_FREE_GEM_TAKEN, new dataPage("/paramFreeGem.data", false)},

        {typeSave.STAR, new dataPage("/star.data", new bool[]{ })},       
        {typeSave.BEST_SCORE,  new dataPage("/player.data", null)},
        {typeSave.MODE_EXPERT_ON, new dataPage("/modeExpertOn.data", false) },
        {typeSave.PROG_LEVEL_REWARD, new dataPage("/progLevelReward.data", new List<bool>(){ }) }, // menu progression mode aventure
        {typeSave.SPECIAL_OFFER_INFO, new dataPage("/infoSpecialOffer.data", null) },

        {typeSave.MONNEY, new dataPage("/monnaie.data", 0)},
        {typeSave.GEMMES, new dataPage("/gemmes.data", 0)},
        
        {typeSave.RECORD_INFINIS, new dataPage("/recordInfinite", 0) },
        {typeSave.SCORES_INFINIS_ONLINE, new dataPage("/scoreOnline", new List<List<string>>()) },
        {typeSave.RECORD_INFINIS_ONLINE, new dataPage("/recordOnline", new List<string>()) },

        {typeSave.MISSED_ACHIEVMENTS, new dataPage("/missedAchievments", new List<object>() { 0, 0, 0, true }) },
        {typeSave.ENSURE_REWARD_BUY, new dataPage("/ensureRewardBuy.data", null) },

        {typeSave.SHOP_BOOST, new dataPage("/shopBoost", new List<object[]>() ) },
        {typeSave.SHOP_BOOST_SELECTED, new dataPage("/shopBoostSelected", new List<int>()) },
        {typeSave.SHOP_TRAILS, new dataPage("/trails.data", new List<int>())},       
        {typeSave.SHOP_PION, new dataPage("/palets.data", new List<int>())},
        {typeSave.SHOP_SELECTED_SKINS, new dataPage("/selectedSkins.data", new List<int>())},
        {typeSave.SHOP_DAILY_OFFER, new dataPage("/dailyOffer.data", new Dictionary<typeOffre, sousTypeOffre>())},
        {typeSave.SHOP_DAILY_DATE, new dataPage("/dailyDate.date", (int)-1) },
        {typeSave.SHOP_DAILY_ID_BOOST, new dataPage("/dailyIdBoost.date", null) },
        {typeSave.SHOP_DAILY_OFFER_TAKEN, new dataPage("/dailyOfferTaken.date", null) },
        {typeSave.SHOP_DAILY_DATE_INIT, new dataPage("/dailyDateInit.date", true) },

        {typeSave.MANETTE_SIDE_HAND, new dataPage("/manetteInfos.data", -1000) },
        {typeSave.MANETTE_POS_BTN, new dataPage("/manettePosBtn.data", new List<string>()) },

        {typeSave.FIRESTORE_PREV_WEEK_RANK, new dataPage("/prevWeekRank", new List<object>()) },
        {typeSave.FIRESTORE_DEV, new dataPage("/firestoreDev", new List<object>()) },
        {typeSave.DEV_LIST_BOT, new dataPage("/devListBot", new List<string>()) },

        {typeSave.NB_SAVE_DONE, new dataPage("/nbSave", 0) },
        {typeSave.ID_PLAYER_FIRESTORE, new dataPage("/idPlayerFirestore", "") },

        {typeSave.PACK_OFFER_INFO, new dataPage("/packOfferInfo", new Dictionary<typePackOffer, packOfferInfo>()) },

        {typeSave.ENSURE_PACK_GIVEN, new dataPage("/ensurePackGiven", null) },

        // VERIFIER SI ENCORE UTILISE
        /*
        {typeSave.SHOP_FONDS, new dataPage("/fonds.data", new List<int>())},
        {typeSave.ONLINE_GAME_ON, new dataPage("/onlineGameOn", false) },
        {typeSave.ONLINE_NB_TICKETS, new dataPage("/nbTicketsOnline", 10) },
        {typeSave.ONLINE_ILLIMITED_TICKETS, new dataPage("/onlineIllimitedTickets", false) },
        {typeSave.ONLINE_PLAYER_NAME, new dataPage("/onlinePlayerName", "") },
        {typeSave.ONLINE_AUTH_VALS, new dataPage("/onlineAuthVals", new List<string>()) },
        {typeSave.SPEED_ROT, new dataPage("/speedRot.data", 10)},
        {typeSave.TUTO, new dataPage("/tuto.data", false)},
        {typeSave.ALL_TUTOS, new dataPage("/allTutos.data", null) },
        {typeSave.DEFI, new dataPage("/defi.data", new object[0])},
        {typeSave.PION_UNLOCKED_ALL, new dataPage("/pionsUnlocked.data", new List<object[]>())},
        {typeSave.ONLINE_NB_TROPHE, new dataPage("/nbTrophe", 0) },*/

        // ANCIENS

        //{typeSave.LVL_SUPER_BOUNCE, new dataPage("/nvSBounce.data", new int[] { 1, 60, 120, 240, 480 })},
        //{typeSave.LVL_TIME_SUPER_BOUNCE, new dataPage("/nvTimeSBounce.data", new int[] { 1, 40, 90, 200, 400 })},
        //{typeSave.LVL_VIE, new dataPage("/nvVie.data", new int[] { 1, 120, 240, 480, 790 })},
        //{typeSave.LVL_CAPACITE_VIE, new dataPage("/nvCapaciteVie.data", new int[] { 1, 40, 90, 200, 400 })},
        //{typeSave.SELECTED_TRAIL, new dataPage("/selectedTrails.data", 0)},
        //{typeSave.PION_SELECTED, new dataPage("/pionsSelected.data", "")},
        //{typeSave.PION_SELECTED_ID, new dataPage("/pionsSelectedID.data", 1)},
        //{typeSave.PION_BUYED, new dataPage("/pionsBuyed.data", new bool[nbPionsBuy])},
        //{typeSave.PION_REAL_MONNEY, new dataPage("/pionsRealMonney.data", false)},
        //{typeSave.FOND_BUYED, new dataPage("/fondBuyed.data", new object[] { true, false, false, false, false, 0 })},
        //{typeSave.SPECIAL_OFFER, new dataPage("/specialOffer", new int[]{0, 0, 0}) },
        //{typeSave.PION_UNLOCK, new dataPage("/pions.data", new bool[nbPions])},
        //{typeSave.VIBE, new dataPage("/vibe.data", true)},
    };

    public static void Save(typeSave type, object data)
    {

        if(type == typeSave.BEST_SCORE
            // SaveSystem appel Save(BEST_SCORE) avec typeOf(Dict) = good data 
            //  Autres script appellent Save(BEST_SCORE) avec typeOf(int[]) = data need convertion
            && data.GetType() == typeof(int[]))
        {
            updateDictScoreFromIntArray((int[])data);
            data = currentScoreData;
        }
        else if( type == typeSave.STAR) // data == typeOf(bool[])
        {
            updateDictStarsFromBoolArray((bool[])data);
            data = currentScoreData;
        }

        string path = dico[type == typeSave.STAR ? typeSave.BEST_SCORE : type].path;

        BinaryFormatter formater = new BinaryFormatter();
        string full_path = Application.persistentDataPath + path;
        FileStream fs = new FileStream(full_path, FileMode.Create);

        formater.Serialize(fs, data);
        fs.Close();

        aSaveIsDone(type, data);
    }

    public static object loadData(typeSave type)
    {
        object retour;
        string path = dico[type == typeSave.STAR ? typeSave.BEST_SCORE : type].path;
        string full_path = Application.persistentDataPath + path;

        if (File.Exists(full_path))
        {
            /*if(type == typeSave.TUTO)
                retour = true;
            else
            {*/
            BinaryFormatter formater = new BinaryFormatter();
            FileStream fs = new FileStream(full_path, FileMode.Open);
            retour = formater.Deserialize(fs);

            /*if (type == typeSave.PION_BUYED || type == typeSave.PION_UNLOCK)
            {
                int nb = type == typeSave.PION_BUYED ? nbPionsBuy : nbPions;
                retour = checkDataBool(((bool[])retour), nb);
            }*/
            /*if (type == typeSave.LVL_SUPER_BOUNCE)
                retour = new int[] { ((int[])retour)[0], 75, 150, 300, 800 };
            else if (type == typeSave.LVL_VIE)
                retour = new int[] { ((int[])retour)[0], 150, 250, 400, 900 };*/
            if (type == typeSave.BEST_SCORE
                || type == typeSave.STAR)
            {
                currentScoreData = (Dictionary<string, object[]>)retour;
                if (type == typeSave.BEST_SCORE)
                    retour = convertDictToDataIntArray(currentScoreData);
                else
                    retour = convertDictToDataBoolArray(currentScoreData);
            }
            else if (type == typeSave.NB_SAVE_DONE && !counterSaveInitialized)
            {
                counterSaveInitialized = true;
                nbSaveDone = (int)retour;
            }

            fs.Close();
            //}
        }
        else
        {
            //JAMAIS pour -typeSave.BEST_SCORE- , est crée a l'initialisation de l'app si existe pas
            //Debug.Log("Type demandé : " + type);

            if (type == typeSave.STAR)
                retour = checkDataBool(new bool[] { }, nbNiveauAventure);
            /*if(type == typeSave.BEST_SCORE)
                retour = check*/
            else
                retour = dico[type].values;
        }
        if (type == typeSave.MISSED_ACHIEVMENTS)
            missedGooglePlayValues = (List<object>) retour;

        return retour;
    }

    private static bool counterSaveInitialized = false;
    public static int nbSaveDone;

    // ne pas les saves su rle cloud
    // soit mode devManager et aucune save cloud
    // soit prevWeekRank ( trop volumineux )
    private static List<typeSave> ignoreSaveForUnityCloud = new List<typeSave>() { typeSave.DEV_LIST_BOT, typeSave.FIRESTORE_DEV, typeSave.FIRESTORE_PREV_WEEK_RANK};
    private static void aSaveIsDone(typeSave type, object data)
    {
        if (!ignoreSaveForUnityCloud.Contains(type))
        {
            if (!counterSaveInitialized)
            {
                loadData(typeSave.NB_SAVE_DONE); // va mettre à jour nbSaveDone
            }
            nbSaveDone += dataInCloudManager.poidSave[type];
        }

        // Save ( ne rappelle pas Save() sinon boucle infinis
        string path = dico[typeSave.NB_SAVE_DONE].path;
        BinaryFormatter formater = new BinaryFormatter();
        string full_path = Application.persistentDataPath + path;
        FileStream fs = new FileStream(full_path, FileMode.Create);
        formater.Serialize(fs, nbSaveDone);
        fs.Close();

        // Prevenir dataCloudManager
        if(!ignoreSaveForUnityCloud.Contains(type))
            dataInCloudManager.aSaveChanged(type, data, nbSaveDone);
    }



    private static bool[] checkDataBool(bool[] data, int max)
    {
        if (data.Length < max)
        {
            bool[] n = new bool[max];
            for (int i = 0; i < max; i++)
                n[i] = i < data.Length ? data[i] : false;
            data = n;
        }
        return data;
    }



    private static float percentCrownTotToUnlock = 0.92f;
    private static int maxDiffCrown = 2; // différence max entre maxCrownActuel ( nbLvl unlock * 3 ) et crownToUnlock
    private static int rangeLevelUnlocked = 4;

    public static int getDifficultyLevel(int numLevel)
    {
        int ret = 0;
        int i = 0;
        while ( i < 5
            && ret + nbLevelPerDifficulty[i] <= numLevel) {
            ret += nbLevelPerDifficulty[i];
            i++;
            
                }
        return i;
    }

    private static Dictionary<int, int> nbLevelPerDifficulty = new Dictionary<int, int>()
    {
        {0, 0 },
        {1, 0 },
        {2, 0 },
        {3, 0 },
        {4, 0 },
        {5, 0 },
    };

    public static float getProgLevel(int numLevel, int difficulty)
    {
        // exemple, demande pour level 50
        // niv 4 , va de 45 a 60 (15) pour niv 4, est donc a prog 5 / 15
        int tot = 0;
        int lvl = 0;
        while(lvl < difficulty)
        {
            tot += nbLevelPerDifficulty[lvl];
            lvl++;          
        }
        // tot = 45
        int rankLevelInDifficulty = numLevel - tot;
        int nbLevelInDifficulty = nbLevelPerDifficulty[difficulty];
        //Debug.Log("("+numLevel+") rankLevelInDifficulty : " + rankLevelInDifficulty + ", nbLevelInDifficulty : " + nbLevelInDifficulty);
        return rankLevelInDifficulty / (float)nbLevelInDifficulty;
    }

    // Initialise le nombre de couronnes necessaire pour unlock chaque niveau
    public static void loadCrownToUnlock()
    {
        //Debug.Log("DataPath : " + Application.persistentDataPath);
        Transform[] list_niveaux =
           GameObject.Find("APPLI").GetComponent<appliManager>().nivManager.GetComponent<managerNiveau>().lesNiveaux;
        int totCrown = 0, index = 0;
        int totCrown_2;

        int indexStar = 1;
        int totStar = 0;

        packNiveau p;
        foreach (Transform t in list_niveaux)
        {
            p = t.GetComponent<packNiveau>();
            p.crownToUnlock = 0;
            p.starToUnlock = 0;

            int diff = p.levelDifficulty;
            nbLevelPerDifficulty[diff] = nbLevelPerDifficulty[diff] + 1;

            if(index == 1)
            {
                p.crownToUnlock = 3;              
                t.name = t.name + " - N" + 3; // DEBUG
                //Debug.Log("(1) Name level : " + t.name);
            }
            else if(index == 3)
            {
                p.crownToUnlock = 9;
                t.name = t.name + " - N" + 9; // DEBUG
            }else if(index == 6)
            {
                p.crownToUnlock = 17;
                t.name = t.name + " - N" + 17; // DEBUG
            }
            else if (index > 6 && (index-1) % rangeLevelUnlocked == 0)
            {
                int nbDiff = totCrown - (int)(totCrown * percentCrownTotToUnlock);
                totCrown_2 = nbDiff > maxDiffCrown ? totCrown - maxDiffCrown : totCrown - nbDiff;
                p.crownToUnlock = totCrown_2;
                t.name = t.name + " - N" + totCrown_2; // DEBUG
                //Debug.Log("(2) Name level : " + t.name);

                if (indexStar == 1)
                {
                    totStar += //totStar == 0 ? 2 :
                        totStar >= 14 ? 4 : 3; // un palier est 17, pas prit dedans
                    indexStar = -1;
                    p.starToUnlock = totStar;
                    t.name = t.name + " - S" + totStar;

                    menuVisuLevel.getInstance().addPalierStar(index);
                }
                indexStar++;
            }



            totCrown += 3;//t.GetComponent<packNiveau>().lesNiveaux.Length;

            //t.name = t.name + " - " + totCrown + "C"; // DEBUG
            index++;

        }
        menuVisuLevel.getInstance().createListRewardCoin();
    }

    /*
     * void loadApp_loadBestScores()
     * Dictionary<string, object[]> create_adventure_list_score()
     * Dictionary<string, object[]> sort_list_score_after_game_update(Dictionary<string, object[]> cur_list)
     * int[] convertDictToIntArray(Dictionary<string, object[]> d)
     * void updateDictScoreFromIntArray(int[] list_best_score)
     */
    #region Gérer la [modification de l'ordre]/[ajout] des niveaux sans impacter les données du joueur

    private static Dictionary<string, object[]> currentScoreData; // data des best scores


    // initialisation de l'appli, s'assurer que la liste des score existe, est bien triée
    public static void loadApp_loadBestScores()
    {
        string code = (string)loadData(typeSave.GAME_VERSION);
        if (code == "") // tout premier lancement
        {
            Save(typeSave.GAME_VERSION, code_version_game);
            create_adventure_list_score();
        }
        else if(code != code_version_game) // lancement après une maj
        {
            Save(typeSave.GAME_VERSION, code_version_game);
            loadData(typeSave.BEST_SCORE); // met a jour currentScoreData  
            sort_list_score_after_game_update(currentScoreData);
        }
    }

    // la liste est crée si le fichier de save n'existe pas préalablement
    private static void create_adventure_list_score()
    {
        Transform[] list_niveaux =
            GameObject.Find("APPLI").GetComponent<appliManager>().nivManager.GetComponent<managerNiveau>().lesNiveaux;
        // list_niveaux : [ code niveau , max level , record niveau , etoile obtenue ]
        Dictionary<string, object[]> bestScores = new Dictionary<string, object[]>();

        foreach (Transform t in list_niveaux)
        {
            bestScores.Add(
                t.GetComponent<packNiveau>().code_save_niveau
                , new object[] { t.GetComponent<packNiveau>().lesNiveaux.Length, 0, false });
        }
        Save(typeSave.BEST_SCORE, bestScores);
    }

    // si version du jeu differente de celle en memoire
    private static void sort_list_score_after_game_update(Dictionary<string, object[]> cur_list)
    {
        // list_niveaux : [ code niveau , max level , record niveau ]
        Transform[] list_niveaux =
            GameObject.Find("APPLI").GetComponent<appliManager>().nivManager.GetComponent<managerNiveau>().lesNiveaux;
        Dictionary<string, object[]> bestScores = new Dictionary<string, object[]>();

        foreach (Transform t in list_niveaux)
        {
            string code = t.GetComponent<packNiveau>().code_save_niveau;
            // on regarde si le niveau existait avant la maj
            if (cur_list.ContainsKey(code))
                bestScores.Add(code, cur_list[code]);
            // sinon on le rajoute a la liste
            else
                bestScores.Add(code, new object[] { t.GetComponent<packNiveau>().lesNiveaux.Length, 0, false });
        }

        if (bestScores != cur_list) // nouveau niveaux / modification d'odre dans la nouvelle maj
            Save(typeSave.BEST_SCORE, bestScores);
    }


    private static int[] convertDictToDataIntArray(Dictionary<string, object[]> d)
    {
        int[] ret = new int[d.Count];
        int i = 0;          
        foreach (object[] o in d.Values)
        {
            ret[i] = (int)o[1];
            i++;
        }
        return ret;
    }
    private static bool[] convertDictToDataBoolArray(Dictionary<string, object[]> d)
    {
        bool[] ret = new bool[d.Count];
        int i = 0;
        foreach (object[] o in d.Values)
        {
            ret[i] = (bool)o[2];
            i++;
        }
        return ret;
    }

    // Les autres scripts Save seulement les RECORDS avec un int[], 
    //  on passe les valeurs de ce int[] au Dict<> avant la Save
    private static void updateDictScoreFromIntArray(int[] list_best_score)
    {
        int i = 0;
        foreach(int best_score in list_best_score)
        {
            string code = currentScoreData.Keys.ElementAt(i);
            currentScoreData[code] = new object[] {
                currentScoreData[code][0],
                list_best_score[i],
                currentScoreData[code][2]};
            i++;
        }
    }

    // Les autres scripts Save seulement les STARS avec un bool[], 
    //  on passe les valeurs de ce bool[] au Dict<> avant la Save
    private static void updateDictStarsFromBoolArray(bool[] list_stars)
    {
        int i = 0;
        foreach (bool star in list_stars)
        {
            string code = currentScoreData.Keys.ElementAt(i);
            currentScoreData[code] = new object[] {
                currentScoreData[code][0],
                currentScoreData[code][1],
                list_stars[i]};
            i++;
        }
    }

    #endregion
}
