using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * enum typeDefi
 * enum typeFinishDefi // SUPPRIMER
 * struct infoDefi
 * struct infoDefi_normal
 * struct infoDefi_static
 * struct defiType_byBoostType
*/
#region enum / struct
public enum typeDefi
{
    crown,
    star,
    infinis,

    bounce,
    checkP,
    portail,
    pivot,

    boost_bouclier,
    boost_bombe,
    boost_superVisee,

    boost_fantome,
    boost_ressort,

    boost_pierreTombale,
    boost_souris,

    none,
}

public enum typeFinishDefi
{
    accumulate_once, // infinis
    accumulate_total, // crown , star 
    activate_and_win, // bouclier , bombe , superVisee
    activate_normal, // souris , fantome , ressort , pierreTombale
    use_obstacle, // bounce , checkP , portail , pivot 
}

[System.Serializable]
public struct infoDefi
{
    public typeDefi type;
    public string defi_description;
    public int[] prog_values;
    public int current_progVal;
    public int current_level; // savoir si a reccup rec ou non

    public bool dailyTaken;

    public typeTraduction_app typeNomDefi;
}

[System.Serializable]
public struct infoDefi_normal
{
    public infoDefi _infos;
    public int compteurOff_toBeReusable;
    public bool defi_normal_isUsed;
    public bool defi_normal_isDaily;
    public typeFinishDefi type_finish;
}

[System.Serializable]
public struct infoDefi_static
{
    public infoDefi _infos;
}

[System.Serializable]
public struct defiType_byBoostType
{
    public typeDefi typeDefiBoost;
    public typeBoostGame typeBoostRef;
}

#endregion


public class defiManager : MonoBehaviour
{
    // checkPointEnd en a besoin
    public GameObject lifeRecGo, monnaieRecGo;

    public List<infoDefi_normal> _defi_normal_list;
    public List<infoDefi_static> _defi_static_list;
    public List<defiType_byBoostType> _defi_by_boost_type;
    
    public defiItem defiCrown;
    public defiItem defiStar;
    public defiItem dailyDefi;
    public defiItem defi_1;
    public defiItem defi_2;

    public static int nbGemmesDailyDefi = 25;

    // les defis static sont toujours activés
    private readonly List<typeDefi> list_static = new List<typeDefi>()
        { typeDefi.crown, typeDefi.star };
    private readonly List<typeDefi> list_boost = new List<typeDefi>()
        {typeDefi.boost_bombe, typeDefi.boost_bouclier, typeDefi.boost_superVisee,
        typeDefi.boost_ressort, typeDefi.boost_souris, typeDefi.boost_pierreTombale, typeDefi.boost_fantome };
    private readonly  List<typeDefi> list_boost_SP = new List<typeDefi>()
        {typeDefi.boost_bombe, typeDefi.boost_bouclier, typeDefi.boost_superVisee};
    private readonly List<typeDefi> list_obstacles = new List<typeDefi>()
        { typeDefi.bounce, typeDefi.pivot, typeDefi.portail, typeDefi.checkP };

    /*private void Start()
    {
        StartCoroutine(waitDailyOfferLoadedToLoadData());
    }*/

    // appelé par dailyOffer quand a finit de load
    public void canBeLoaded()
    {
        loadDefiData();
        //Debug.Log("Load defi data");
    }
    /*private IEnumerator waitDailyOfferLoadedToLoadData()
    {
        yield return new WaitUntil(predicate: () => offreQuotidienne.initDailyIsDone == true);

    }*/

    private static defiManager instance;
    public static defiManager getInstance()
    {
        if(instance == null) instance = GameObject.Find("newDefi_test").GetComponent<defiManager>();
        return instance;
    }

    public static bool menuDefiOpen = false;
    /*
     * openMenu()
     * closeMenu()
    */
    #region ouverture / fermeture menu
    public void openMenu()
    {
        if (!notifDefiDone.isAnimatingNotif && !notifStarUnlocked.isAnimating)
        {
            //performanceManager.instance.openSousMenuP();
            menuDefiOpen = true;
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
            //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_shopDefiInfinis);
            GetComponent<Animation>().Play("defi_open");
            // on set active
            StartCoroutine(animDefiItemCome());
        }
    }
    public float timeBetweenItemAnimCome = 0.12f;
    public float timeBetweenItemAnimOut = 0.12f;
    private IEnumerator animDefiItemCome()
    {
        yield return null;
        // on dit aux items de briller si finits
        foreach (defiItem dI in new defiItem[] { dailyDefi, defiCrown, defiStar, defi_1, defi_2 })
        {
            dI.gameObject.SetActive(true);
            dI.openMenuDefi();
            yield return new WaitForSeconds(timeBetweenItemAnimCome);
        }
    }

    /*private IEnumerator animDefiOut()
    {
        foreach (defiItem dI in new defiItem[] { dailyDefi, defiCrown, defiStar, defi_1, defi_2 })
        {
            dI.lockItemRaycastGo.SetActive(true);
            //dI.gameObject.SetActive(true);
            //dI.openMenuDefi();
            //yield return new WaitForSeconds(timeBetweenItemAnimCome);
            
        }

        // on dit aux items de briller si finits
        foreach (defiItem dI in new defiItem[] { dailyDefi, defiCrown, defiStar, defi_1, defi_2 })
        {
            dI.closeMenuDefi();
            //dI.gameObject.SetActive(true);
            //dI.openMenuDefi();
            //yield return new WaitForSeconds(timeBetweenItemAnimOut);
        }
    }*/

    public void closeMenu()
    {
        //performanceManager.instance.closeSousMenuP();
        menuDefiOpen = false;
        GetComponent<Animation>().Play("defi_close");

        foreach (defiItem dI in new defiItem[] { dailyDefi, defiCrown, defiStar, defi_1, defi_2 })
        {
            //dI.lockItemRaycastGo.SetActive(true);
            dI.closeMenuDefi();
        }

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_close);
    }
    #endregion





    /*
     * typeBoostGame getBoostType_byDefiName(typeDefi tD)
     * infoDefi getInfoByType(typeDefi tD)
     * infoDefi_normal getNormalDefiByType(typeDefi tD)
     * void setDefiUsingValue(bool isUsed, bool isDaily, typeDefi tD)
    */
    #region getSet
    private typeBoostGame getBoostType_byDefiName(typeDefi tD)
    {
        foreach (defiType_byBoostType dT in _defi_by_boost_type)
            if (dT.typeDefiBoost == tD)
                return dT.typeBoostRef;
        return typeBoostGame.none;
    }
    private infoDefi getInfoByType(typeDefi tD)
    {
        foreach(infoDefi_normal iD in _defi_normal_list)
        {
            if (iD._infos.type == tD)
                return iD._infos;
        }
        return new infoDefi();
    }

    private infoDefi_normal getNormalDefiByType(typeDefi tD)
    {
        foreach(infoDefi_normal iDN in _defi_normal_list)
        {
            if (iDN._infos.type == tD) return iDN;
        }
        return new infoDefi_normal();
    }
    private void setDefiUsingValue(bool isUsed, bool isDaily, typeDefi tD, bool replace)
    {
        int rang = 0;
        while (_defi_normal_list[rang]._infos.type != tD) rang++;
        infoDefi_normal iD = _defi_normal_list[rang];
        iD.defi_normal_isUsed = isUsed;
        iD.defi_normal_isDaily = isDaily;

        if (replace)
        {
            iD.compteurOff_toBeReusable = 3;
            iD._infos.dailyTaken = false;
            iD._infos.current_progVal = 0;
        }

        _defi_normal_list[rang] = iD;
    }

    #endregion

    private static bool initLoadDone = false;
    /*
     * void loadDefiData()
     * void saveDataDefi()
     * void loadDefiCrown()
     * void loadDefiStar()
     * 
     * void loadDefiUsedList()
    */
    #region gestion data defi (load / save)

    // data : daily , crown , star , none , none , none
    private void loadDefiData()
    {
        initLoadDone = true;
        //Debug.Log("init data defi");
        object data = SaveSystem.loadData(typeSave.DEFI_PROG);
        if (data == null)
        {
            loadFirstDefiData();
        }
        else
        {
            object[] data_2 = (object[])data;
            List<infoDefi_normal> savedNormalProg = (List<infoDefi_normal>)data_2[0];
            List<infoDefi_static> savedStaticProg = (List<infoDefi_static>)data_2[1];

            // Save defi enregistre tte les valeures d'un defi, dont les val de prog
            // Si maj modifie les val de prog, on doit alors mettre à jour la save
            int[] progValues;
            int index;
            // DEFI NORMAL
            infoDefi_normal intermediaire;
            foreach (infoDefi_normal info in savedNormalProg)
            {
                // on cherche l'item associe dans liste ( kaou items sont interchangés
                index = 0;
                while (index < _defi_normal_list.Count
                    && _defi_normal_list[index]._infos.type != info._infos.type)
                    index++;
                progValues = _defi_normal_list[index]._infos.prog_values;
                intermediaire = info;
                intermediaire._infos.prog_values = progValues;
                _defi_normal_list[index] = intermediaire;
            }
            // DEFI STATIC
            //Debug.Log("Load Defi static data");
            
            infoDefi_static intermediaire_2;
            foreach (infoDefi_static info in savedStaticProg)
            {
                //Debug.Log("Nombre de data dans liste static saved : " + info._infos.prog_values.Length);
                //Debug.Log("Nombre de data dans liste static app : " + _defi_static_list[1]._infos.prog_values.Length);
                // on cherche l'item associe dans liste ( kaou items sont interchangés
                index = 0;
                while (index < _defi_static_list.Count
                    && _defi_static_list[index]._infos.type != info._infos.type)
                    index++;

                progValues = _defi_static_list[index]._infos.prog_values;
                intermediaire_2 = info;
                intermediaire_2._infos.prog_values = progValues;
                _defi_static_list[index] = intermediaire_2;
            }

            StartCoroutine(loadDefiCrownLate(true));
            loadDefiStar();
            loadDefiUsedList();

            int e = 0;
            foreach (defiItem dI in new defiItem[] { dailyDefi, defi_1, defi_2 })
            {
                dI.loadDefiItem(getInfoByType(currentDefiUsed[e]), e == 0, false);
                e++;
            }
            checkDefiIsFinished();

            if (reloadDailyDefi) // on doit remplacer le defi daily de la liste
            {
                replaceDefi(dailyDefi, dailyDefi.currenTypeDefi);
                reloadDailyDefi = false;
            }
        }
    }

    private void saveDataDefi()
    {
        SaveSystem.Save(typeSave.DEFI_PROG, new object[] { _defi_normal_list, _defi_static_list });
    }


    [Header("Crown")]
    public List<itemSkin> freePalet_crown;
    public Image rewardPalet_img;
    public Image rewardPaletTrail_img;
    public Image rewardPaletBriller_img;
    public Image borderRewardImg;
    public Image trailIconImg;
    // waitInit , true seulement à initialisation
    // rewardDefiTaken, true seulement quand reward defi taken
    public void setNbCrown_initGame()
    {
        int[] records = (int[])SaveSystem.loadData(typeSave.BEST_SCORE);
        if (records != null)
        {
            int tot = 0;
            for (int i = 0; i < records.Length; i++)
            {
                tot += records[i];
            }
            SaveSystem.nb_couronne = tot;
        }
        else
        {
            SaveSystem.nb_couronne = 0;
        }
    }

    private IEnumerator loadDefiCrownLate(bool waitInit)
    {
        if(waitInit)
            yield return null; // à l'initialisation, attendre que SaveSystem soit mit à jour

        int nbCrown = SaveSystem.nb_couronne;
        infoDefi_static iDS = _defi_static_list[0];
        infoDefi iD = iDS._infos;

        // /!\ peut dépasser les limites de l'array
        int nextProg = iD.prog_values[iD.current_level];

        //Debug.Log("Init game, load crown id : " + iD + ", nbCrown : " + nbCrown + ", nextProg : "+nextProg);
        defiCrown.loadDefiItem_static(iD, nbCrown, nextProg);

        // on load le palet à gagner
        int niveau = iD.current_level;
        itemSkin toLoad = freePalet_crown[niveau];
        Color trailColor = toLoad.info_palet.paletTrailColor;

        rewardPalet_img.sprite = toLoad.info_palet.paletSprite;
        //rewardPalet_img.color = trailColor;
        rewardPaletTrail_img.color = trailColor;
        rewardPaletBriller_img.color = trailColor;
        borderRewardImg.color = trailColor;
        trailIconImg.color = trailColor;

        if(waitInit)
            checkDefiIsFinished();
    }

    [Header("Star")]

    public typeBooster[] rewardsBoosters_star;
    public Transform boosterContainer;
    private void loadDefiStar()
    {
        int nbStar = SaveSystem.getNbStarObtain();
        infoDefi_static iDS = _defi_static_list[1];

        

        infoDefi iD = iDS._infos;
        int nextProg = iD.prog_values[iD.current_level];

        /*Debug.Log("iD.current_level : " + iD.current_level);
            // /!\ peut dépasser les limites de l'array

        
        foreach(int i in iD.prog_values)
        {
            Debug.Log("Prog values i : "+i);
        }
        Debug.Log("Load defi star, next prog : "+nextProg); // Debug*/

        defiStar.loadDefiItem_static(iD, nbStar, nextProg);

        // on load le booster associe
        foreach (Transform t in boosterContainer)
        {
            t.gameObject.SetActive(false);
        }
        typeBooster tB = rewardsBoosters_star[iD.current_level];
        boosterContainer.GetChild(
            tB == typeBooster.boosterArgent ? 0 : tB == typeBooster.boosterOr ? 1 : 2).gameObject.SetActive(true);

    }

    private void loadDefiUsedList()
    {
        currentDefiUsed = new List<typeDefi>();
        foreach(infoDefi_normal iDN in _defi_normal_list)
        {
            if (iDN.defi_normal_isUsed)
            {
                if (iDN.defi_normal_isDaily)
                    currentDefiUsed.Insert(0, iDN._infos.type);
                else
                {
                    currentDefiUsed.Add(iDN._infos.type);
                }
            }
        }
    }

    #endregion


    private List<typeDefi> currentDefiUsed = new List<typeDefi>();
    /*
     * void loadFirstDefiData()
     * List<typeDefi> getAvailableDefi(bool notObstacle, bool notBoost, bool canUseLockedBoost, bool useInfinis)
     * typeDefi getNewDefi(bool daily)
     * void replaceDefi()
    */
    #region gestion creation / remplacement defi
    private void loadFirstDefiData()
    {
        // crown
        StartCoroutine(loadDefiCrownLate(true));
        // star
        loadDefiStar();
        // autres
        for (int i = 0; i < 3; i++)
        {
            typeDefi tD = getNewDefi(i == 0, true);
            currentDefiUsed.Add(tD);
            setDefiUsingValue(true, i == 0, tD, false);
        }

        int e = 0;
        foreach (defiItem dI in new defiItem[] { dailyDefi, defi_1, defi_2 })
        {
            dI.loadDefiItem(getInfoByType(currentDefiUsed[e]), e == 0, true);
            e++;
        }

        saveDataDefi();
    }

    private Dictionary<typeDefi, typeRarety> raretyBoost = new Dictionary<typeDefi, typeRarety>()
    {
        {typeDefi.boost_bombe, typeRarety.legendary },
        {typeDefi.boost_bouclier, typeRarety.legendary },
        {typeDefi.boost_superVisee, typeRarety.legendary },
        {typeDefi.boost_souris, typeRarety.epic },
        {typeDefi.boost_pierreTombale, typeRarety.epic },
        {typeDefi.boost_ressort, typeRarety.rare },
        {typeDefi.boost_fantome, typeRarety.rare }
    };
    private bool listContainsSameBoostRarety(typeDefi type)
    {
        typeRarety tR = raretyBoost[type];
        foreach (typeDefi tD in currentDefiUsed)
            if (raretyBoost.ContainsKey(tD) && raretyBoost[tD] == tR)
                return true;
        return false;
    }
    private List<typeDefi> getAvailableDefi(bool notObstacle, bool notBoost, bool canUseLockedBoost, bool useInfinis, bool daily)
    {
        // trier les defis par leurs niveaux
        // contient seulement les defis utilisables
        Dictionary<int, List<typeDefi>> level_defis = new Dictionary<int, List<typeDefi>>();
        List<typeDefi> retour = new List<typeDefi>();
        bool addValue;
        typeDefi type;

        foreach (infoDefi_normal iO in _defi_normal_list)
        {
            addValue = false;
            type = iO._infos.type;

            // current list ne doit pas contenir le defi
            if (iO.compteurOff_toBeReusable == 0 && !currentDefiUsed.Contains(type))
            {
                // boost
                // on doit checker que la liste ne contient pas un boost de la même rarete
                if (list_boost.Contains(type))
                {  
                    if (!notBoost && !listContainsSameBoostRarety(type))
                    {
                        bool unlocked = boostManager.getInstance().boostIsUnlocked_byType(getBoostType_byDefiName(type));
                        if ((canUseLockedBoost && !list_boost_SP.Contains(type)) || unlocked)
                        {
                            addValue = true;
                        }
                    }
                }
                // obstacle
                else if (list_obstacles.Contains(type))
                {
                    if (!notObstacle)
                    {
                        addValue = true;
                    }
                }
                // infinis
                else if (type == typeDefi.infinis)
                {
                    if (useInfinis)
                    {
                        addValue = true;
                    }
                }
                else
                {
                    addValue = true;
                }
            }
            if (addValue)
            {
                int level = iO._infos.current_level;
                if (!level_defis.ContainsKey(level))
                    level_defis.Add(level, new List<typeDefi> { type });
                else
                    level_defis[level].Add(type);
                retour.Add(type);
            }
        }
        
        if (!daily) // on retourne les defis de niveaux les plus bas
        {
            int maxLevel = 10; // on met une valeure 100% sur au dessus de tout les niveaux , et même de nivMaxDefi
            foreach(KeyValuePair<int, List<typeDefi>> kV in level_defis)
            {
                if(kV.Key < maxLevel)
                {
                    maxLevel = kV.Key;
                    retour = kV.Value;
                }
            }
        }

        return retour;
    }

    private typeDefi getNewDefi(bool daily, bool firstTimeLoad)
    {
        // daily :
        //   propose seulement des defi réalisable ( pas de boost non unlock - mode infinis si unlock )
        // None : propose au maximum 1 boost non possédé
        //   2 boost ET 1 obstacle , OU 2 obstacle et 1 boost

        int nbObstacles = 0;
        int nbBoost = 0;
        bool usingNotAvailableBoost = false;

        foreach (typeDefi tD in currentDefiUsed)
        {
            if (list_obstacles.Contains(tD))
                nbObstacles++;
            else if (list_boost.Contains(tD))
            {
                nbBoost++;
                // on regarde si c'est un boost notunlocked
                if (boostManager.getInstance().boostIsUnlocked_byType(getBoostType_byDefiName(tD)))
                    usingNotAvailableBoost = true;
            }
        }

        bool forceNoObstacle;
        bool forceNoBoost;
        bool canUseLockedBoost;
        if (firstTimeLoad)
        {
            forceNoObstacle = false;
            forceNoBoost = true;
            canUseLockedBoost = false;
        }
        else
        {
            forceNoObstacle = nbObstacles == 2;
            forceNoBoost = nbBoost == 2;

            if (daily && forceNoObstacle)
            {
                canUseLockedBoost = true;
            }
            else
                canUseLockedBoost = (!daily && !usingNotAvailableBoost);
        }

        List<typeDefi> available = getAvailableDefi(
            forceNoObstacle, forceNoBoost, canUseLockedBoost, lockInfinis.isUnlocked, daily);
        typeDefi newDefi = available[Random.Range(0, available.Count)];

        return newDefi;
    }
    private void replaceDefi(defiItem emplacement, typeDefi toRemove)
    {
        // on enlève le defi à remplacer
        int indexRemove = 0;
        while (currentDefiUsed[indexRemove] != toRemove) indexRemove++;
        // on l'enlève avant la recherche de defi , pour eviter de bloquer des obstacle / boost
        currentDefiUsed.Remove(toRemove);
        setDefiUsingValue(false, false, toRemove, true);


        // on désincrémente tout les compteurs off > 0
        int i = 0;
        List<infoDefi_normal> copyInfo = new List<infoDefi_normal>(_defi_normal_list);
        foreach(infoDefi_normal iDN in copyInfo)
        {
            infoDefi_normal iDN_copy = iDN;
            if(iDN_copy.compteurOff_toBeReusable != 0)
            {
                iDN_copy.compteurOff_toBeReusable -= 1;
                _defi_normal_list[i] = iDN_copy;
            }
            i ++;
        }

        // on cherche un nouveau defi
        typeDefi newD = getNewDefi(emplacement == dailyDefi, false);

        currentDefiUsed.Insert(indexRemove, newD);
        
        setDefiUsingValue(true, emplacement == dailyDefi, newD, false);
        // on load le nouveau defi
        StartCoroutine(replaceDefi_enum(emplacement, newD));

        // on save les nouvelle informations
        saveDataDefi();
    }

    private IEnumerator replaceDefi_enum(defiItem emplacement, typeDefi newD)
    {
        //Debug.Log("... with : " + newD);
        yield return new WaitForSeconds(35 / 60f);
        emplacement.loadDefiItem(getInfoByType(newD), emplacement == dailyDefi, true);
        checkDefiIsFinished();
    }

    #endregion

    private static bool reloadDailyDefi = false;
    public Text timerDailyText;
    /*
     * void initDailyDefiTime(int[] time, bool newDay) 
     * IEnumerator switchDailyOffer(bool animate)
     * void dailyOfferUpdateTime(int[] time)
    */
    #region gestion daily defi


    //private static string dailyString = "";
    /*private string getDailyTradString()
    {
        if (dailyString == "")
            dailyString = traductionManager.getInstance().getTradDefi(typeTraduction_app.defi_quotidienTitre);
        return dailyString;
    }*/
    // appelé par offre quotidienne à l'initialisation
    // appelé quand maj time et passe jour suivant
    public void initDailyDefiTime(long[] time, bool newDay) 
    {
        //Debug.Log("load daily offer from offerQuotidienne");
        timerDailyText.text = //getDailyTradString() +
            time[0] + "<color=grey>h</color> " +
            time[1] + "<color=grey>m</color>";

        // si on a fait le tout premier load de defi : ne pas remplacer daily
        // sinon newDay mais déja data

        if (newDay)
        {
            //Debug.Log("Reload defi quotidien, end timer");
            if (!initLoadDone)
            {
                // on reload dailyOffer au lancement du jeu , se fait SEULEMENT si pas first time ever 
                reloadDailyDefi = true;
                //Debug.Log("1");
            }
            else // changement in game
            {
                // on remplace le defi
                switchDailyOffer();// dailyDefi.gameObject.activeInHierarchy);
                //Debug.Log("2");
            }
        }
    }

    private void switchDailyOffer()//bool animate)
    {
        replaceDefi(dailyDefi, dailyDefi.currenTypeDefi);
    }

    public void dailyOfferUpdateTime(long[] time) // appelé par offre quotidienne quand temps update
    {
        timerDailyText.text = //getDailyTradString() +
            time[0] + "<color=grey>h</color> " +
            (time[1]+1) + "<color=grey>m</color>";
    }


    #endregion


    // on attend que player win pour valider les boost SP
    private Dictionary<typeDefi, int> incrementeEndGame = new Dictionary<typeDefi, int>();
    private Dictionary<typeDefi, int> incrementeFinishGame =
        new Dictionary<typeDefi, int>() {
            { typeDefi.checkP, 0 },
            { typeDefi.bounce, 0},
            { typeDefi.pivot, 0 },
            { typeDefi.portail, 0 }
        };
    private bool makeASaveEndOfGame = false;
    /*
     * void playerEndsLevel(bool win)
     * void defiIsIncremented(typeDefi type, int nbIncremente)
     * void changeDefiValue(typeDefi tD, int nbAdded)
     * void gameEnd()
    */
    #region gestion incrementation des defis
    public void playerEndsLevel(bool win)
    {
        if (incrementeEndGame.Count == 0)
            return;

        if (win)
        {
            List<typeDefi> list = new List<typeDefi>(incrementeEndGame.Keys);
            foreach(typeDefi tD in list)
            {
                //defiIsIncremented(tD, incrementeEndGame[tD]);
                changeDefiValue(tD, incrementeEndGame[tD]);
            }
            makeASaveEndOfGame = true;
        }
        incrementeEndGame = new Dictionary<typeDefi, int>();
    }
    
    public int getNextProgCrown()
    {
        infoDefi_static iDS_crown = _defi_static_list[0];
        return iDS_crown._infos.prog_values[iDS_crown._infos.current_level];
    }
    public int getNextProgStar()
    {
        infoDefi_static iDS_star = _defi_static_list[1];
        return iDS_star._infos.prog_values[iDS_star._infos.current_level];
    }

    // on save toutes les valeures en fin de game , si pas static
    // quand on incrémente un défi, on regarde si est finit -> afficher notif btn menu defi
    public void defiIsIncremented(typeDefi type, int nbIncremente)
    {
        // si le défi est courament utilisé, on l'incremente
        if (!currentDefiUsed.Contains(type) && !list_static.Contains(type))
            return;

        if (list_static.Contains(type))
        {
            if (type == typeDefi.crown)
            {
                // on reload crown
                StartCoroutine(loadDefiCrownLate(false));
            }
            else if (type == typeDefi.star)
            {
                // on reload star
                loadDefiStar();
            }
            checkDefiIsFinished();
        }
        else {
            if (list_boost_SP.Contains(type))
            {
                if (!incrementeEndGame.ContainsKey(type))
                    incrementeEndGame.Add(type, 1);
                else
                    incrementeEndGame[type]++;
            }
            else if (list_obstacles.Contains(type))
            {
                incrementeFinishGame[type]++;
            }
            else if(type == typeDefi.infinis)
            {
                changeDefiValue(type, nbIncremente);
                // on save cash
                saveDataDefi();
                checkDefiIsFinished();
            }
            // boost normal
            else { // attente de fin de game
                makeASaveEndOfGame = true;
                changeDefiValue(type, nbIncremente);
            }
        }
    }

    public notifDefiDone notifAnim;
    private void changeDefiValue(typeDefi tD, int nbAdded)
    {
        // on cherche l'item qui va être mit à jour
        foreach (defiItem dI_2 in new defiItem[] { dailyDefi, defi_1, defi_2 })
        {
            if (dI_2.currenTypeDefi == tD)
            {
                // puis on update data
                int index = 0;
                foreach (infoDefi_normal iDN in _defi_normal_list)
                {
                    if (iDN._infos.type == tD)
                    {
                        infoDefi_normal iDN_2 = iDN;

                        int curLevelDefi = iDN_2._infos.current_level;
                        if (curLevelDefi < iDN_2._infos.prog_values.Length)
                        {
                            int curProg = iDN_2._infos.current_progVal;
                            int nextProg = dI_2 == dailyDefi ? iDN_2._infos.prog_values[1]
                                : iDN_2._infos.prog_values[curLevelDefi];

                            /*Debug.Log("Liste prog values : ");
                            foreach (int i in iDN_2._infos.prog_values)
                                Debug.Log("- item : " + i);
                            Debug.Log("Current level defi : " + curLevelDefi);

                            Debug.Log("Check if will show notif, current prog : "+curProg+" , nextProg : "+nextProg+ " (+"+nbAdded+")");
                            */
                            
                            //Debug.Log("Change defi value, type : " + iDN_2._infos.typeNomDefi);
                            //Debug.Log("Cur val : " + curProg + " , next val : " + nextProg + " , added val : " + nbAdded);
                            if (curProg < nextProg && curProg + nbAdded >= nextProg)
                                notifAnim.newDefiIsUnlocked(iDN._infos.type);
                        }

                        iDN_2._infos.current_progVal += nbAdded;
                        _defi_normal_list[index] = iDN_2;

                        // et on update item à mettre à jour avec nouvelle datas
                        dI_2.changeValue(iDN_2._infos);

                        break;
                    }
                    index++;
                }
                break;
            }

        }
    }

    public void gameEnd()
    {
        // doit être appelé -> après couronnes reçus -> après star reçu 

        // on fait les saves de data
        // on active la notification si doit être activé
        foreach(typeDefi key in new List<typeDefi>(incrementeFinishGame.Keys))
        {
            if (incrementeFinishGame[key] != 0)
            {
                changeDefiValue(key, incrementeFinishGame[key]);
                incrementeFinishGame[key] = 0;
                makeASaveEndOfGame = true;
            }
        }
        if (makeASaveEndOfGame) // peut être mit a true dans la partie meme ( boost normaux )
        {
            makeASaveEndOfGame = false;
            saveDataDefi();
            checkDefiIsFinished();
        }
    }

    #endregion


    /*
     * void defiRewardIsTaken(typeDefi tD, bool daily)
     * void upgradeLevelDefi(bool normal, typeDefi tD)
    */
    #region Recompense / Upgrade level

    public void defiRewardIsTaken(typeDefi tD, bool daily)
    {
        StartCoroutine(defiRewardIsTaken_enum(tD, daily));
    }
    private IEnumerator defiRewardIsTaken_enum(typeDefi tD, bool daily)
    {
        if (list_static.Contains(tD))
        {
            infoDefi_static iDS = _defi_static_list[tD == typeDefi.crown ? 0 : 1];
            //Debug.Log("TYPE : " + tD);
            if (tD == typeDefi.crown)
            {
                // on donne la recompense palet              
                itemSkin rewardPalet = freePalet_crown[iDS._infos.current_level];
                skinManager.getInstance().getPaletFromDefi(rewardPalet.transform, rewardPalet.id_skin);
                menuRewardSkin.getInstance().giveReward(
                    new object[] { rewardPalet.info_palet.paletSprite, rewardPalet.info_palet.paletTrailColor }, true);
                //rewardMenu.getInstance().giveReward(typeReward.palet,
                //    new object[] { rewardPalet.info_palet.paletSprite, rewardPalet.info_palet.paletTrailColor });

                yield return new WaitForSeconds(1f);
                // on upgrade le defi
                upgradeLevelDefi(false, tD, false);
                // on load le palet suivant
                StartCoroutine(loadDefiCrownLate(false));
            }
            else if(tD == typeDefi.star)
            {
                // on donne la recompense Booster
                typeBooster currentType = rewardsBoosters_star[iDS._infos.current_level];
                menuGetBoost.getInstance().openMenuBooster(currentType);
                //openBoosterManager.getInstance().openBoosterFromDefi(currentType);

                yield return new WaitForSeconds(1f);
                // on upgrade le defi
                upgradeLevelDefi(false, tD, false);
                // on load le booster suivant
                loadDefiStar();
            }

        }
        else if (daily)
        {
            upgradeLevelDefi(true, tD, true);
            // on attend la fin du timer quotidien
        }
        else
        {
            upgradeLevelDefi(true, tD, false);
            // on load un nouveau défi a l'emplacement
        }

        yield return new WaitForSeconds(0.1f);
        checkDefiIsFinished();
        // non, testé
        // : est fait + tard une fois que  replaceDefi(dI, tD) finit
    }

    private void upgradeLevelDefi(bool normal, typeDefi tD, bool daily)
    {
        if (normal)
        {
            infoDefi_normal iDN = getNormalDefiByType(tD);
            infoDefi iD = iDN._infos;

            if (daily)
            {
                iD.dailyTaken = true;
            }
            else
            {
                // enlevé code dessous pour nouveau comportement :
                // - pas de système de niveau pour les défis 'Or', toujours 30 Or de recompense => gagne + souvent des récompenses
                /*int niv = iD.current_level;
                niv = niv + 1 == 5 ? // taille max
                    4 : niv + 1;
                iD.current_level = niv;*/
                iD.current_progVal = 0;
            }

            // on met 3 parce que la fonction de remplacement désincrémente de suite 
            // 3 daily parce que ne se replace pas tout de suite
            iDN.compteurOff_toBeReusable = 3; // défi pourra réaparaître dans 2 roulements de defis
            iDN._infos = iD;

            int i = 0;
            while (_defi_normal_list[i]._infos.type != tD) i++;
            _defi_normal_list[i] = iDN;

            if (daily) // on ne replace pas daily defi de suite
            {
                saveDataDefi();
            }
            else 
            { 
                foreach (defiItem dI in new defiItem[] { defiCrown, defiStar, dailyDefi, defi_1, defi_2 })
                {
                    if (dI.currenTypeDefi == tD)
                    {
                        // /!\ replaceDefi() fait la save
                        replaceDefi(dI, tD);
                        break;
                    }
                }
            }
        }
        else
        {
            infoDefi_static iDS = _defi_static_list[tD == typeDefi.crown ? 0 : 1];
            int max = tD == typeDefi.crown ? freePalet_crown.Count : rewardsBoosters_star.Length;



            infoDefi iD = iDS._infos;
            int niv = iD.current_level;

            Debug.Log("Taille max List : " + max);
            Debug.Log("Current Level reward : " + niv);

            niv = niv + 1 == max ? // taille max
                niv : niv + 1;

            Debug.Log("New level Defi : " + niv);
             // Debug dit 24 level alors que 2 ont été ajoutés (26 attendus) , liste pas mit à jour ?


            iD.current_level = niv;
            iD.current_progVal = 0;

            iDS._infos = iD;
            _defi_static_list[tD == typeDefi.crown ? 0 : 1] = iDS;

            // on save les nouvelle informations
            saveDataDefi();
        }
    }

    #endregion


    public void btnInfoStar()
    {
        StartCoroutine(openInfoStarFromDefi());
    }

    public infoMenuManager infoMenu;
    public float timeWaitOpenStar = 0.4f;
    private IEnumerator openInfoStarFromDefi()
    {
        // on ferme le menu
        closeMenu();
        yield return new WaitForSeconds(timeWaitOpenStar);
        // on ouvre menu star
        infoMenu.btnInfo_star();
    }


    public GameObject notifDefiEnd;
    private void checkDefiIsFinished()
    {
        //Debug.Log("CheckDefi is finished");
        //Debug.Log("Check defi is finished");
        bool finish = false;
        foreach (defiItem dI in new defiItem[] { defiCrown, defiStar, dailyDefi, defi_1, defi_2 })
        {
            if (dI.checkIsFull(true))
            {
                //Debug.Log("Defi " + dI.currenTypeDefi+" is finished");
                finish = true;
                // on les parcourt tous, permet d'activer l'animation finish
            }
        }

        notifDefiEnd.SetActive(finish);
    }
}
