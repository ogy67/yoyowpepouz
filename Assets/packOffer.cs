using CompleteProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typePackOffer
{
    pack_debutant,
    pack_avancee,
    pack_gratuitTikTok,
    none,
}

public enum typeStatePack
{
    state_isShowing,
    state_finished,
}

[Serializable]
public struct packOfferInfo
{
    public typePackOffer typePack;
    public typeStatePack typeState;
    public double timeOfferStarted;
}


public class packOffer : MonoBehaviour
{
    public typePackOffer type_pack;
    public int packDebutant_crownRequired = 30; // specialement pour offre débutant

    private static Dictionary<typePackOffer, packOfferInfo> list_info_offers;
    private static bool localDataLoaded = false;
    //private static bool anotherOfferIsOn = false; // /!\ pcq je veut 1 seule offre à la fois
    
    // est ce que bien initialisé par rapport à save ? -> OUI quand load data
    private static List<typePackOffer> currentPackShownList = new List<typePackOffer>();

    private static void modifyPackShownStatu(typePackOffer type, bool isShown)
    {
        if(isShown && !currentPackShownList.Contains(type))
            currentPackShownList.Add(type);
        else if(!isShown && currentPackShownList.Contains(type))
            currentPackShownList.Remove(type);
    }

    private static Dictionary<typePackOffer, List<typePackOffer>> listOfferLocked = new Dictionary<typePackOffer, List<typePackOffer>>()
    {
        {typePackOffer.pack_avancee, new List<typePackOffer>() {typePackOffer.pack_debutant } },
    };
    private static bool checkIfOfferIsLockedByAnother(typePackOffer type)
    {
        bool ret = false;
        if (listOfferLocked.ContainsKey(type))
        {
            foreach(typePackOffer t in listOfferLocked[type])
            {
                if (currentPackShownList.Contains(t))
                {
                    // ne peut pas être montré
                    ret = true;
                }
            }
        }
        return ret;
    }

    /*
     * void initAppCheckOfferOn()
     * static void loadLocalData()
     * static long getTime()
     * static long ElapsedTimeOffer(long initTime)
    */
    #region INITIALISATION DES OFFRES
    // appelé pendant la phase d'initialisation du shop à init app
    // ( avant que rebuildLayout )
    public void initAppCheckOfferOn()
    {
        if(!localDataLoaded)
            loadLocalData();

        if (currentPackShownList.Contains(type_pack))
        {
            // l'afficher direct
            activateOffer();
            initTimer(list_info_offers[type_pack].timeOfferStarted);
        }
        /*if (checkIfOfferIsLockedByAnother(type_pack))// anotherOfferIsOn)
        {
            Debug.Log("Another offer already showing");
            if(currentPackShownList.Contains(type_pack))// currentPackShown == type_pack)
            {
                // l'afficher
                activateOffer();
                initTimer(list_info_offers[type_pack].timeOfferStarted); //  getTime()); , pas getTime ( temps actuel
            }
        }*/
        else if (!checkIfOfferIsLockedByAnother(type_pack))
        {
            if (type_pack == typePackOffer.pack_debutant)
            {
                // si pas dans liste ( jamais proposé )
                // ET si a depassé X couronnes
                if (!list_info_offers.ContainsKey(typePackOffer.pack_debutant)
                && SaveSystem.nb_couronne >= packDebutant_crownRequired)
                {
                    activateOffer();
                    list_info_offers.Add(
                        typePackOffer.pack_debutant,
                        new packOfferInfo()
                        {
                            timeOfferStarted = getTime(),
                            typePack = typePackOffer.pack_debutant,
                            typeState = typeStatePack.state_isShowing
                        });
                    modifyPackShownStatu(type_pack, true);
                    //currentPackShown = type_pack;
                    SaveSystem.Save(typeSave.PACK_OFFER_INFO, list_info_offers);
                    initTimer(list_info_offers[type_pack].timeOfferStarted);
                }

            }
            else if (type_pack == typePackOffer.pack_gratuitTikTok)
            {
                // Initialisation appelé si pack dans liste de shopManager
                // avant maj spéciale pour pack gratuit, ré-ajouter item à la liste de shopManager

                /*Debug.Log(onlineRankingManager.firestoreEnabled ? "FIRESTORE DISPO" : "FIRESTORE PAS DISPO");
                if(onlineRankingManager.firestoreEnabled)
                    Debug.Log("TimeInBdd : " + onlineRankingManager.getTimeFirestoreBdd());*/

                // si offre est déja dans liste des offres : -> ( induit que offre n'est pas actuellement affichée ( check plus haut : "if (currentPackShownList.Contains(type_pack))" )
                /*if (!list_info_offers.ContainsKey(typePackOffer.pack_gratuitTikTok))
                {
                    //  - si prise : 
                    if(list_info_offers[type_pack].typeState == typeStatePack.state_finished)
                    {
                        // -> passer
                    }
                    /*  - si en cours : plus haut "if (currentPackShownList.Contains(type_pack))" a déjà activé l'offre, si était dans celles actuellement activées
                    else
                    {
                        //  -> si pas acces firestore 
                        if (!onlineRankingManager.firestoreEnabled)
                        {
                            // -> passer
                        }
                        //  -> si acces firestore /
                        else
                        {
                            double timerFireStore = onlineRankingManager.getTimeFirestoreBdd();
                            Debug.Log("TimeInBdd : " + timerFireStore);
                            // si date expiré 
                            if(timerFireStore >= timeEndOfferTikTok)
                            {
                                // -> mettre en mode "finit" -> passer
                            }
                            // sinon 
                            else
                            {
                                // -> ouvrir offre et set timer
                                activateOffer();
                            }
                        }
                    }
                }*/

                if (!list_info_offers.ContainsKey(typePackOffer.pack_gratuitTikTok))
                {
                    // - si pas accès firestore
                    if (!onlineRankingManager.firestoreEnabled)
                    {
                        // -> passer
                    }
                    // - si accès firestore, regarder timer                   
                    else
                    {
                        Debug.Log("Check offer free");
                        double timerFireStore = onlineRankingManager.getTimeFirestoreBdd() * 1000;
                        // - si timer - 7j > 7j 
                        if(timeEndOfferTikTok - timerFireStore > timeWeek)
                        {
                            Debug.Log("- not started (timeFirestore:" + timerFireStore + ", timeEndOffer :" + timeEndOfferTikTok);
                            // -> passer ( pas encore commencé )
                        }
                        // - si timer > goalTime 
                        else if(timerFireStore > timeEndOfferTikTok)
                        {
                            Debug.Log("- time passed (timeFirestore:"+timerFireStore+", timeEndOffer :"+timeEndOfferTikTok);
                            // -> passer ( dépassé )
                            // (l'ajouter à la liste des offres passées )
                            list_info_offers.Add(
                                typePackOffer.pack_gratuitTikTok,
                                new packOfferInfo()
                                {
                                    timeOfferStarted = 0,
                                    typePack = typePackOffer.pack_gratuitTikTok,
                                    typeState = typeStatePack.state_finished
                                });
                        }
                        // - sinon
                        else
                        {
                            Debug.Log("- open offer");
                            double tempsDejaPasse = timeWeek - (timeEndOfferTikTok - timerFireStore);
                            double tempsDevicePlayer = getTime();// / 1000f;

                            // -> ouvrir offre et set timer
                            activateOffer();
                            list_info_offers.Add(
                                typePackOffer.pack_gratuitTikTok,
                                new packOfferInfo()
                                {
                                    timeOfferStarted = tempsDevicePlayer - tempsDejaPasse,
                                    typePack = typePackOffer.pack_gratuitTikTok,
                                    typeState = typeStatePack.state_isShowing
                                });
                            modifyPackShownStatu(type_pack, true);
                            //currentPackShown = type_pack;
                            SaveSystem.Save(typeSave.PACK_OFFER_INFO, list_info_offers);
                            initTimer(list_info_offers[type_pack].timeOfferStarted);
                        }
                    }
                }
                


                // Quand date de sortie de la vidéo trouvé
                // -> calculer quel temps il sera 7j après sortie vidéo -> écrire ce temps dans une variable
                // - mettre à jour l'appli avec ce temps
            }
        }       
    }

    // x : 63854408316944,4 19/06/2024 17 : 39
    // y : x + (82 * oneDayTime) + (21 * oneMinuteTime) : le Lundi 9 Septembre 2024 à 18h

    // x : 63849583888,1039 : temps à 21:31 le 24/04/2024
    // y : x + 29*60 + 2*(60*60) : temps à minuit le 24/04/2024
    // z : y + 6*24*60*60 : temps à minuit le 30/04/2024 ( 6 jours après x )

    private double timeEndOfferTikTok = 63854408316944.4 + (82 * oneDayTime) + (21 * oneMinuteTime);
        //= (63849583888.1039 + (29 * 60 + 2 * (60 * 60)) + (3 * 24 * 60 * 60)) * 1000 + fiveYears;
    private double timeWeek = (7 * 24 * (60 * 60)) * 1000;
    //private const double fiveYears = oneWeekTime * 56 * 5;// 15724800000;// 15724800 * 1000;


    // appelé à init app, une seule des offres fait l'initialisation
    private static void loadLocalData()
    {
        localDataLoaded = true;
       
        list_info_offers = (Dictionary<typePackOffer, packOfferInfo>)SaveSystem.loadData(typeSave.PACK_OFFER_INFO);
        Dictionary<typePackOffer, packOfferInfo> secondaryList = new Dictionary<typePackOffer, packOfferInfo>(list_info_offers);

        // pour chaque item dans la liste, vérifier que temps n'est pas dépassé 
        // => passe en mode finished
        bool hasReset = false;
        foreach (KeyValuePair<typePackOffer, packOfferInfo> kV in secondaryList)
        {
            if (kV.Value.typeState == typeStatePack.state_isShowing)
            {
                double initTime = kV.Value.timeOfferStarted;
                if (ElapsedTimeOffer(initTime) >= oneWeekTime)
                {
                    // reset l'offre
                    packOfferInfo p = list_info_offers[kV.Value.typePack];
                    p.typeState = typeStatePack.state_finished;
                    list_info_offers[kV.Value.typePack] = p;
                    hasReset = true;
                }
                else
                {
                    //anotherOfferIsOn = true;
                    //currentPackShown = kV.Value.typePack;
                    modifyPackShownStatu(kV.Value.typePack, true);
                }
            }
        }
        if (hasReset)
        {
            SaveSystem.Save(typeSave.PACK_OFFER_INFO, list_info_offers);
        }
    }

    public static long getTime()
    {
        DateTime t = DateTime.Now;
        return (long)(t - new DateTime(1980, 1, 1)).TotalMilliseconds;
    }

    // donne le temps depuis le début de l'offre
    public static double ElapsedTimeOffer(double initTime)
    {
        DateTime t = DateTime.Now;
        return (double)(t - new DateTime(1980, 1, 1)).TotalMilliseconds - initTime;
    }
    #endregion

    public GameObject myGo, spaceUpGo, spaceDownGo;
    /*
     * void activateOffer()
     * void desactivateOffer()
    */
    #region ACTIVATION / DESACTIVATION UI
    private void activateOffer()
    {
        myGo.SetActive(true);
        spaceUpGo.SetActive(true);
        spaceDownGo.SetActive(true);
    }


    // appelé quand finit reccup offre
    // ou si reload offer open app et était activé
    public void offerFullyGiven()
    {
        Debug.Log("Fin offre, myGo open ? " + myGo.activeSelf+" , menu shop open ? "+ shopManager.menuShopOpen);
        if(myGo.activeSelf)
        {
            if (shopManager.menuShopOpen)
            {
                animBtnOffer.Play("offerDebutantTaken");
            }
            else // vient de init app
            {
                panOfferTaken.SetActive(true);
            }
        }
        desactivateOffer(false);
    }

    private void desactivateOffer(bool uiToo)
    {
        if (uiToo)
        {
            myGo.SetActive(false);
            spaceUpGo.SetActive(false);
            spaceDownGo.SetActive(false);
        }

        isGettingOffer = false;

        modifyPackShownStatu(type_pack, false);
        //currentPackShown = typePackOffer.none;

        // mettre l'offre en état 'finished' dans save
        packOfferInfo p = list_info_offers[type_pack];
        p.typeState = typeStatePack.state_finished;
        list_info_offers[type_pack] = p;
        SaveSystem.Save(typeSave.PACK_OFFER_INFO, list_info_offers);

        currentInstanceTryBuy = null;
    }

    #endregion

    public GameObject panOfferTaken;
    public shopManager shopM;
    public GameObject noCoGo;
    public Animation animBtnOffer;
    public static packOffer currentInstanceTryBuy; // pour purchaser
    public Purchaser purchaseManager;
    public static bool isGettingOffer = false;
    /*
     * void btnOffer()
     * void failedToPurchase()
     * void succedPurchase()
    */
    #region ACHAT DE L'OFFRE
    public void btnOffer()
    {
        animBtnOffer.Play("offerDebutantClicked");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);

        // check si internet
        StartCoroutine(launcher.checkInternetConnection((isConnected) => {
            if (!isConnected)
            {
                // sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                // non
                shopM.animateNoConnec(noCoGo);
            }
            else
            {
                // essaie acheter
                currentInstanceTryBuy = GetComponent<packOffer>();
                switch (type_pack)
                {
                    case typePackOffer.pack_debutant:
                        purchaseManager.BuyPackOffer(type_pack);
                        break;
                    case typePackOffer.pack_gratuitTikTok:
                        succedPurchase();
                        break;
                }                            
            }
        }));       
    }

    // appelé par purchaser.cs quand n'est pas initialisé OU n'a pas réussit à acheter OU player pas validé achat / pas de thunes
    public void failedToPurchase(bool animNoCo)
    {
        if(animNoCo)
            shopM.animateNoConnec(noCoGo);
    }

    public Transform posItemOffer;
    public void succedPurchase()
    {
        // ( NON , chaque offre appelle le son ) si achat validé
        // sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
        openOfferPack.getInstance().openPack(type_pack, true, 0, posItemOffer.position);
        isGettingOffer = true;
    }
    #endregion



    // gérer le timer


    private const long oneWeekTime = 7 * 24 * 60 * 60 * 1000;
    private const long oneDayTime = 24 * 60 * 60 * 1000;
    private const long oneHourTime = 60 * 60 * 1000;
    private const long oneMinuteTime = 60 * 1000;

    public GameObject timeOutGo;
    public string textColorTimer = "<color=\"#BC51B3\">";
    public Text timerText, timerText_ombre;
    private static double initTimeCurrentOffer;

    // appelé init app, si offre est choisit / en cours
    private void initTimer(double initTime)
    {
        initTimeCurrentOffer = initTime;
        // arrive toujours ici si le timer n'avait pas depassé 7 jours
        // si avait depassé 7 jours, offre pas activé

        setTimerWithTime(getTimeLeft_newOffer(initTime, getTime()));
    }

    // quand offre quotidienne se rafraichît, appelle ici en même temps
    public void refreshTimer()
    {
        double elapsed = ElapsedTimeOffer(initTimeCurrentOffer);
        if (elapsed > oneWeekTime)
        {
            // SI n'était pas en train de la prendre
            // ET que pas en mode "offre prise"
            if (!isGettingOffer
                && list_info_offers[type_pack].typeState != typeStatePack.state_finished)
            {
                // si n'est pas dans shop, le fermer
                if (!shopManager.menuShopOpen)
                {
                    desactivateOffer(true);
                }
                else
                {
                    // afficher "finit" sur offre
                    timeOutGo.SetActive(true);
                }
            }
            setTimerWithTime(new double[] { 0, 0, 0 });
        }
        else
        {
            setTimerWithTime(getTimeLeft_newOffer(initTimeCurrentOffer, getTime()));
        }
    }

    // traduire J / D
    private string[] listLetterDate = new string[] { };
    private void setTimerWithTime(double[] vals)
    {
        if (listLetterDate.Length == 0)
            listLetterDate = traductionManager.getInstance().getListLetterDate();

        timerText.text = vals[0] + textColorTimer + listLetterDate[0] + " </color>" + vals[1] + textColorTimer + listLetterDate[1] +" </color>" + vals[2] + textColorTimer + listLetterDate[2] +"</color>";
        timerText_ombre.text = vals[0] + listLetterDate[0] + " " + vals[1] + listLetterDate[1] + " " + vals[2] + listLetterDate[2];
    }


    public double[] getTimeLeft_newOffer(double initTime, double currentTime)
    {
        // long est un entier

        double ecartLong = (currentTime - initTime);
        double nbDay = (int)(ecartLong / oneDayTime);
        double totTimeDay = nbDay * oneDayTime;

        double nbHourPastLong = (int)((ecartLong - totTimeDay) / oneHourTime);
        double totTimeHour = nbHourPastLong * oneHourTime;

        double nbMinutePastLong = (int)((ecartLong - (totTimeDay + totTimeHour)) / oneMinuteTime);

        return new double[]
        {
            6 - nbDay,
            23 - nbHourPastLong,
            59 - nbMinutePastLong,
        };
    }
}
