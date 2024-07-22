using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum typeRankList
{
    rankWeek,
    rankPrevWeek,
    rankRecord
}

[Serializable]
public struct infoPlayerTest
{
    public string nomPlayer;
    public string idPlayer;
    public int nbStar;
    public int scoreWeek;
    public int scoreRecord;
    public double timeScoreWeek;
    public double timeScoreRecord;

    public string idsBoostUsed;
    public string levelBoostUsed;
    public string idsBoostUsed_record;
    public string levelBoostUsed_record;

    // infos ajoutés en local

    public int scorePrevWeek; // seulement set lors de transsformation liste infoPlayerPrevWeek en infoPlayerTest
    public string idsBoostUsed_prevWeek;
    public string levelBoostUsed_prevWeek;
    //public bool rewardTaken;
}

[Serializable]
public struct infoPlayerPrevWeek
{
    public string nomPlayer;
    public string idPlayer;
    public int nbStar;
    public int scorePrevWeek;

    public string idsBoostUsed;
    public string levelBoostUsed;
    public double timeScore;
    public bool rewardTaken;
}

public class rankingInfinis : MonoBehaviour
{
    /*public infoPlayerTest[] test_infoPlayer;
    public infoPlayerPrevWeek[] test_infoPlayerPrevWeek; // est pré-classé lors du calcul end week
    public bool test_btnSwitch = false;
    public bool test_btnOpenRankingInfinis = false;
    public bool test_btnCloseRankingInfinis = false;
    public bool test_btnCleanList = false;
    //public string test_idLocalPlayer = "1";
    // Update is called once per frame
    void Update()
    {
        if (test_btnSwitch)
        {
            test_btnSwitch = false;
            switchRanking();
        }
        if (test_btnOpenRankingInfinis)
        {
            test_btnOpenRankingInfinis = false;
            openMenuRankingInfinis(true);
        }
        if (test_btnCloseRankingInfinis)
        {
            test_btnCloseRankingInfinis = false;
            animCloseMenu();
        }

        if (test_btnCleanList)
        {
            /*test_btnCleanList = false;
            while (content_week.childCount > 0)
                Destroy(content_week.GetChild(0).gameObject);
            while (content_record.childCount > 0)
                Destroy(content_record.GetChild(0).gameObject);
        }
    }*/



    public Text timeLeftText, timeLeftText_ombre;
    /*
     * void openMenuRankingInfinis()
     * void openMenuRankingInfinis_suiteDataLoaded(bool anyRanksHasBeenUpdated)
     * IEnumerator StartTimerBdd()
     * void setTimerWithSeconds(double seconds, bool anim)
     * IEnumerator animTimerClick(string ombreS, string timeS)
     * IEnumerator animOpenMenu()
     * void animCloseMenu()
     */
    #region Ouverture initiale
    public List<infoPlayerTest> liste_week;
    public List<infoPlayerTest> liste_record;
    public List<infoPlayerTest> liste_prevWeek;
    public Text scoreWeekText, scoreRecordText;
    public infoPlayerTest localPlayerInfos;

    public void openMenuRankingInfinis_suiteDataLoaded(bool anyRanksHasBeenUpdated)
    {
        initialisation = true;

        if (anyRanksHasBeenUpdated)
        {
            //Debug.Log("Refresh rank ui");
            //managerRankInfinis.getInstance().loadBarOpen();
            openProgBar();
            menuWaitFillRankAnim.Play("openWaitFillList");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.loadRank_open);
            StartCoroutine(preFillList());
        }
        else
        {
            //Debug.Log("Re-use rank ui");
            StartCoroutine(animOpenMenu());
            //menuWaitFillRankAnim.Play("waitInitConnecEndNoFillList");
        }
    }

    public static bool menuOpen;
    public Animation animOpenCloseMenu;
    public Animation resetRankAnim, resetBtnBottomAnim;
    public float timeFillListAfterOpen = 0.25f;

    private IEnumerator StartTimerBdd()
    {        
        // initialisation timer
        double timeLeftSec = onlineRankingManager.getInstance().getTimeLeft_toNextWeek();
        //Debug.Log("Time left bdd : " + timeLeftSec);

        int minutes = (int)((timeLeftSec / 60) % 60) + 1;
        setTimerWithSeconds(timeLeftSec, false);

        // empecher : timer tombe à 0 pendant initialisation menu, ajout '&& initialisation'
        //while (!menuOpen)
        Debug.Log("<color=red> WAIT load TIMER end week ( inRank )</color>");
        while (initialisation)
            yield return null;
        Debug.Log("<color=red> START TIMER end week ( inRank )</color>");


        // update timer
        int prevMin = minutes;
        float initTime = Time.time;
        float timeElapsed;
        double secondsLeft;
        int minutes_2;

        float time_range = 5f;
        float t;

        while (menuOpen)
        {
            t = 0f;
            while (t < time_range && menuOpen)
            {
                yield return new WaitForSeconds(0.2f);
                t += 0.2f;
            }
            if (menuOpen)
            {
                timeElapsed = Time.time - initTime;
                secondsLeft = timeLeftSec - timeElapsed;
                minutes_2 = secondsLeft <= 0 ? 0 : (int)((secondsLeft / 60) % 60) + 1;
                Debug.Log("--- Tick, time elapsed : " + timeElapsed + " , minutesRef : " + minutes_2);
                if (minutes_2 != prevMin)
                {
                    Debug.Log("------ Change timer");
                    prevMin = minutes_2;
                    setTimerWithSeconds(secondsLeft, true);
                    //Debug.Log("Update timer");
                }//else if(secondsLeft <= 0)
                 // setTimerWithSeconds(secondsLeft);
            }
        }
    }

    private void setTimerWithSeconds(double seconds, bool anim)
    {
        if (seconds <= 0)
        {
            //Debug.Log("Change timer display 1");
            //timeLeftText.text = "<color=\"#FFFFFF\">0</color>J<color=\"#FFFFFF\">:0</color>H<color=\"#FFFFFF\">:0</color>Min";
            //timeLeftText_ombre.text = "0J:00H:00Min";
            StartCoroutine(animTimerClick("0J:00H:00Min"
                , "<color=\"#FFFFFF\">0</color>J<color=\"#FFFFFF\">:0</color>H<color=\"#FFFFFF\">:0</color>Min"));
            // on ferme le menu pour afficher menu maintenance
            managerRankV2.rankTimeOut_GoMaintenance();
            //managerRankInfinis.getInstance().playerDisconnectedForMaintenance();
        }
        else
        {
            //Debug.Log("Change timer display 2");
            //double timeLeftSec = onlineRankingManager.getInstance().getTimeLeftBaseSecond();
            int minutes = (int)((seconds / 60) % 60) + 1;
            int hour = (int)((seconds / 60 / 60) % 24);
            int day = (int)((seconds / 60 / 60 / 24));
            string minS = minutes.ToString().Length == 1 ? '0' + minutes.ToString() : minutes.ToString();
            string hourS = hour.ToString().Length == 1 ? '0' + hour.ToString() : hour.ToString();
            if (anim && timerClickAnim.gameObject.activeInHierarchy)
            {
                StartCoroutine(animTimerClick(day + "J:" + hourS + "H:" + minS + "Min"
                    , "<color=\"#FFFFFF\">" + day + "</color>J<color=\"#FFFFFF\">:" + hourS + "</color>H<color=\"#FFFFFF\">:" + minS + "</color>Min"));
            }
            else
            {
                timeLeftText.text = "<color=\"#FFFFFF\">" + day + "</color>J<color=\"#FFFFFF\">:" + hourS + "</color>H<color=\"#FFFFFF\">:" + minS + "</color>Min";
                timeLeftText_ombre.text = day + "J:" + hourS + "H:" + minS + "Min";
            }
        }
    }

    public Animation timerClickAnim;
    private IEnumerator animTimerClick(string ombreS, string timeS)
    {
        timerClickAnim.Play("timerRankClick");
        yield return new WaitForSeconds(7 / 60f);
        timeLeftText.text = timeS;
        timeLeftText_ombre.text = ombreS;
    }

    private IEnumerator animOpenMenu()
    {

        /*double timeLeftSec = onlineRankingManager.getInstance().getTimeLeftBaseSecond();
        int minutes = (int)((timeLeftSec / 60) % 60);
        int hour = (int)((timeLeftSec / 60 / 60) % 24);
        int day = (int)((timeLeftSec / 60 / 60 / 24));
        timeLeftText.text = "<color=\"#FFFFFF\">" + day + "</color>J<color=\"#FFFFFF\">:" + hour + "</color>H<color=\"#FFFFFF\">:" + minutes + "</color>Min";
        timeLeftText_ombre.text = day + "J:" + hour + "H:" + minutes + "Min";*/


        initialisation = true;
        menuOpen = true;
        StartCoroutine(StartTimerBdd());

        if (!isInWeekMenu || prevWeekOn)
        {
            resetRankAnim.Play("resetSwitchAnim");
            //resetBtnBottomAnim.Play("resetBtnBotom");
            prevWeekOn = false;
            isInWeekMenu = true;

            string s = "S<size=22>EMAINE ACTUELLE</size>";// : "S<size=22>EMAINE PRECEDENTE</size>";
            textBtnPrevWeek.text = s;
            textBtnPrevWeek.transform.GetChild(0).GetComponent<Text>().text = s;
            // <color="#FFFFFF">4</color>J<color="#FFFFFF">:15</color>H<color="#FFFFFF">:26</color>Min

            yield return null;
        }

        timerClickAnim.Play("timerRankReset");

        //Debug.Log("Score week player retrieved : " + localPlayerInfos.scoreWeek);
        scoreWeekText.text = getStringScore(localPlayerInfos.scoreWeek);
        scoreRecordText.text = getStringScore(localPlayerInfos.scoreRecord);

        StartCoroutine(ApplyScrollPosition(1));

        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.menuRank_open);
        animOpenCloseMenu.Play("rankInfinisOpen");
        resetBtnBottomAnim.Play("btnRankingOpen");

        yield return new WaitForSeconds(timeFillListAfterOpen);
        StartCoroutine(fillRankingWithList(/*liste_week,*/ typeRankList.rankWeek));

        initialisation = false;
    }

    public void animCloseMenu()
    {
        scrollRContent.StopMovement();

        //managerRankInfinis.menuOpen = false;
        menuOpen = false;
        animOpenCloseMenu.Play(isInWeekMenu ? "rankInfinisClose" : "rankInfinisClose_fromRecord");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.menuRank_close);
        resetBtnBottomAnim.Play("btnRankingClose");
        // créer anim close liste items
        animBtnUpDownSwitch(); // desactiver btn up et down

        Vector2 bounds = getBoundsItemAnim();
        // anim close les items
        foreach (Transform t in currentContent)
        {
            if (!t.name.Contains("espace"))
            {
                t.GetComponent<itemPlayerRanking>().closeItem(
                t.GetSiblingIndex() >= bounds.x && t.GetSiblingIndex() <= bounds.y);//  <= nbItemAnimSwitchPop);
            }
        }

        // reset menu fin anim : 
        // si dans menu record
        // - fondRank bonne pos sur Y ( mode week )
        // - reset btn week / record
        // si prevWeek actif
        // - btn prev week couleure bleu
        // - 

    }


    /*private IEnumerator openMenuLate()
    {
        yield return null;
        animReActivateBtnDown();
    }*/



    #endregion

    /*
     * void setPrevWeekList(List<infoPlayerPrevWeek> liste)
     * IEnumerator preFillList()
     * void openProgBar()
     * void newCallWait()
     */
    #region Initialisation data

    public itemPlayerRanking itemPlayer;
    public Transform contentItemPlayers; // off
    public int nbPlayerLimitWeek = 100, nbPlayerLimitRecord = 200;

    public int nbItemAnimSwitchPop = 12;

    //public GameObject panWaitMenuRankLoaded;
    public Animation menuWaitFillRankAnim;
    private bool initialisation;
    public int localRankWeek = -1, localRankRecord = -1, localRankPrevWeek = -1;

    public GameObject notifRewardGo;
    public GameObject espaceItem;
    public menuRewardRank menuReward;
    //private List<int> rankEspace = new List<int>() { 10, 50, 100, 150 };

    public void setPrevWeekList(List<infoPlayerPrevWeek> liste)
    {
        liste_prevWeek = new List<infoPlayerTest>();
        infoPlayerTest item = new infoPlayerTest();

        foreach (infoPlayerPrevWeek i in liste)
        {
            item.idPlayer = i.idPlayer;
            item.nomPlayer = i.nomPlayer;
            item.nbStar = i.nbStar;
            item.scorePrevWeek = i.scorePrevWeek;
            item.idsBoostUsed = i.idsBoostUsed;
            item.levelBoostUsed = i.levelBoostUsed;
            item.idsBoostUsed_prevWeek = i.idsBoostUsed;
            item.levelBoostUsed_prevWeek = i.levelBoostUsed;
            //item.rewardTaken = i.rewardTaken;

            liste_prevWeek.Add(item);
        }
    }

    private IEnumerator preFillList()
    {
        localRankWeek = -1;
        localRankRecord = -1;
        localRankPrevWeek = -1;

        /*openProgBar();
        initialisation = true;
        //panWaitMenuRankLoaded.SetActive(true);
        menuWaitFillRankAnim.Play("openWaitFillList");*/

        // Transformer la liste infoPlayerPrevWeek en liste infoPlayerTest
        

        for (int numList = 0; numList < 3; numList++)
        {
            typeRankList typeList = numList == 0 ? typeRankList.rankWeek : numList == 1 ? typeRankList.rankRecord : typeRankList.rankPrevWeek;
            //bool week = numList == 0;
            List <infoPlayerTest> liste = numList == 0 ? liste_week : numList == 1 ? liste_record : liste_prevWeek;
            Transform content = numList == 0 ? content_week : numList == 1 ? content_record : content_prevWeek;

            bool itemsAlreadyCreated = content.childCount != 0; // on reutilise les items
            /*if(content.childCount != 0)
            {
                foreach (Transform t in content)
                    Destroy(t.gameObject);
            }*/
            // on remplit liste week et liste record
            int indexPerf = 0;
            int rank = 1;
            string idLocalPlayer = onlineRankingManager.getInstance().getIdLocalPlayer();
            bool isLocalPlayer;
            //bool anim;
            foreach (infoPlayerTest infoPlayer in liste)
            {
                isLocalPlayer = infoPlayer.idPlayer == idLocalPlayer;
                if (isLocalPlayer)
                {
                    //Debug.Log("Local player found in rank type : " + typeList+", num rank : "+rank);
                    //Debug.Log("Id player found : " + infoPlayer.idPlayer+ ", localId : "+ idLocalPlayer);
                    if (typeList == typeRankList.rankWeek)
                        localRankWeek = rank;
                    else if (typeList == typeRankList.rankRecord)
                        localRankRecord = rank;
                    else
                    {
                        // mettre  à jour player local avec rank week
                        localRankPrevWeek = rank;
                        localPlayerInfos.scorePrevWeek = infoPlayer.scorePrevWeek;
                        localPlayerInfos.idsBoostUsed_prevWeek = infoPlayer.idsBoostUsed_prevWeek;
                        localPlayerInfos.levelBoostUsed_prevWeek = infoPlayer.levelBoostUsed_prevWeek;
                    }
                }
                GameObject o = itemsAlreadyCreated ?
                    content.GetChild(rank - 1).gameObject 
                    : Instantiate(itemPlayer.gameObject, content);
                //anim = rank <= nbItemAnimSwitchPop;
                o.GetComponent<itemPlayerRanking>().itemInstantiated(
                    infoPlayer
                    , rank
                    , isLocalPlayer
                    , typeList == typeRankList.rankWeek ? getNbGemPerRank(rank) : 0
                    , typeList);
                    //, anim
                    //, true);
                o.SetActive(true);

                //if (rankEspace.Contains(rank))
                //    Instantiate(espaceItem, content);

                rank++;

                /*if (anim)
                {
                    newCallWait();
                    yield return null;
                }
                else
                {*/
                    indexPerf++;
                    if (indexPerf == nbStepPerf)
                    {
                        newCallWait();
                        yield return null;
                        indexPerf = 0;
                    }
                //}
            }

            // création des 'empty'
            int nbPlayers = liste.Count;
            int diffToMax = (typeList == typeRankList.rankRecord ?  nbPlayerLimitRecord : nbPlayerLimitWeek) - nbPlayers;

            if (diffToMax > 0)
            {
                for (int i = 0; i < diffToMax; i++)
                {
                    GameObject o = itemsAlreadyCreated ?
                    content.GetChild(rank - 1).gameObject
                    : Instantiate(itemPlayer.gameObject, content);

                    o.GetComponent<itemPlayerRanking>().itemInstantiated_empty(typeList);//liste.Count + i + 1 < nbItemAnimSwitchPop);
                    o.SetActive(true);
                    indexPerf++;
                    if (indexPerf == nbStepPerf) // ne pas tout créer d'un seul coup
                    {
                        newCallWait();
                        yield return null;
                        indexPerf = 0;
                    }
                    rank++;
                }
            }
            newCallWait();
            yield return null;
        }
        currentContent = content_week;
        StartCoroutine(lockEventScroll());
        initialisation = false;
        // une fois prefill finit, ouvrir rank

        // Notif reward
        if (localRankPrevWeek != -1)
        {
            if (!onlineRankingManager.getInstance().localPlayerHasTakenRewardPrevWeek())
            {
                notifRewardGo.SetActive(true);
                notifRewardGo.GetComponent<Animation>().Play("notifRewardRank");
                menuReward.initValues(localRankPrevWeek, getNbGemPerRank(localRankPrevWeek));
            }
        }

        menuWaitFillRankAnim.Play("closeWaitFillList");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.loadRank_close);
        yield return new WaitForSeconds(1 / 6f);
        StartCoroutine(animOpenMenu());
        //StartCoroutine(fillRankingWithList(/*liste_week,*/ typeRankList.rankWeek));
        //panWaitMenuRankLoaded.SetActive(false);

        //Debug.Log("Nb step progBar " + nbCallNextFrame);
    }

    private int nbUpdateStep = 53; // 52 call pour week:100 , prevWeek:100 , record:200
    private int nbCallNextFrame;
    public Image progImg, progImgBordBriller;
    private float diffProgBordBriller = 0.436f - 0.427f;
    public Text progText, progTextChild;
    private void openProgBar()
    {
        //int nbAnimPopWeek
        // Pré-calculer le nombre de frame d'update
        /*int nbFrameWeek = (liste_week.Count > nbItemAnimSwitchPop ?
            nbItemAnimSwitchPop : liste_week.Count) + (nbPlayerLimitWeek - liste_week.Count) / nbStepPerf;
        int nbFrameRecord = (liste_record.Count > nbItemAnimSwitchPop ?
            nbItemAnimSwitchPop : liste_record.Count) + (nbPlayerLimitRecord - liste_record.Count) / nbStepPerf;
        int nbFramePrevWeek = (liste_prevWeek.Count> nbItemAnimSwitchPop ?
            nbItemAnimSwitchPop : liste_prevWeek.Count) + (nbPlayerLimitWeek - liste_prevWeek.Count) / nbStepPerf;*/

        // si 100 joueurs prev week, 200 joueurs record
        // nbFrame prevWeek : 12 + (100 - 100)/12

        nbUpdateStep = (nbPlayerLimitWeek / nbStepPerf * 2) + nbPlayerLimitRecord / nbStepPerf + 3;
            //nbFrameWeek + nbFrameRecord + nbFramePrevWeek + 3;

        //Debug.Log("<color=blue>Nb step calculated : " + nbUpdateStep+"</color>");


        nbCallNextFrame = 0;

        progImg.fillAmount = 0f;
        progImgBordBriller.fillAmount = 0f;

        progText.text = "0%";
        progTextChild.text = "0%";
    }

    private float startProgImg = 0.1f;
    private float endProgImg = 0.9f;
    private void newCallWait()
    {
        nbCallNextFrame++;
        float prog = nbCallNextFrame / (float)nbUpdateStep;
        float diff = endProgImg - startProgImg;
        progImg.fillAmount = startProgImg + diff * prog;
        progImgBordBriller.fillAmount = progImg.fillAmount + diffProgBordBriller;

        progText.text = (int)(prog * 100) + "%";
        progTextChild.text = (int)(prog * 100) + "%";

        //Debug.Log("NbCall loadData : " + nbCallNextFrame);
    }
    #endregion

    /*
     * void btnRankingRecord()
     * void btnRankingWeek()
     * IEnumerator lockSwitchList()
     * void switchRanking()
     * IEnumerator lockEventScroll()
     */
    #region Switch ranking List

    public Animation animSwitchMenu, animSwitchBtns;
    private bool isInWeekMenu = true;
    public void btnRankingRecord()
    {
        if (switchLocked || !menuOpen)
            return;
        if (isInWeekMenu)
            switchRanking();
    }

    public void btnRankingWeek()
    {
        if (switchLocked || !menuOpen)
            return;
        if (!isInWeekMenu)
            switchRanking();
    }

    public float timeLockSwitchList = 0.8f;
    public static bool switchLocked = false;
    private IEnumerator lockSwitchList()
    {
        switchLocked = true;
        yield return new WaitForSeconds(timeLockSwitchList);
        switchLocked = false;
    }

    private void switchRanking()
    {
        StartCoroutine(lockEventScroll());
        StartCoroutine(lockSwitchList());

        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.menuRank_switch);
        if (isInWeekMenu)
        {
            animSwitchMenu.Play("switchRankingToRecord");
            animSwitchBtns.Play("btnWeekToRecord");
            StartCoroutine(fillRankingWithList_switch(typeRankList.rankRecord));
        }
        else
        {
            animSwitchMenu.Play("switchRankingToWeek");
            animSwitchBtns.Play("btnRecordToWeek");
            StartCoroutine(fillRankingWithList_switch(prevWeekOn ? typeRankList.rankPrevWeek : typeRankList.rankWeek));
        }
        isInWeekMenu = !isInWeekMenu;
    }

    // appelé à ouverture / quand switch, bloquer check si btn up down activé
    public float timeLockEventScroll = 0.5f;
    private bool eventScrollLocked = false;
    private IEnumerator lockEventScroll()
    {
        eventScrollLocked = true;
        yield return new WaitForSeconds(0.5f);
        eventScrollLocked = false;
    }

    #endregion

    public managerRankInfinisV2 managerRankV2;
    public static bool prevWeekOn = false;
    public void btnSwitchWeek_prevWeek()
    {
        if (!menuOpen || switchLocked)
            return;
        // ne pas vérifier la connexion ici
        // les ranks sont loaded et on a pas besoin d'internet une fois entré dans rank
        /*if (!onlineRankingManager.getInstance().isConnectedToInternet())
        {
            managerRankV2.playerDisconnecterFromInternet(typeDisconnectInternet.inRankMenu);
            //managerRankInfinis.getInstance().playerDisconnectedFromInternet_fromRankMenu();
        }
        else
        {*/
            StartCoroutine(lockSwitchList());
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.menuRank_switch);
            animSwitchMenu.Play(prevWeekOn ? "switchPrevWeekToWeek" : "switchWeekToPrevWeek");
            StartCoroutine(switchWeek_prevWeekAnim(prevWeekOn));
            StartCoroutine(fillRankingWithList_switch(prevWeekOn ? typeRankList.rankWeek : typeRankList.rankPrevWeek));
            prevWeekOn = !prevWeekOn;
        //}
    }

    public Text textBtnPrevWeek;
    public float timeWaitOpenReward = 0.4f;
    private IEnumerator switchWeek_prevWeekAnim(bool toWeek)
    {
        bool playerWonReward = !toWeek && notifRewardGo.activeSelf;

        if (playerWonReward)
            notifRewardGo.GetComponent<Animation>().Play("notifRewardOff");

        yield return new WaitForSeconds(9 / 60f);
        string s = toWeek ?
            traductionManager.getInstance().getTradRank(typeTraduction_ranking.current_week)
            : traductionManager.getInstance().getTradRank(typeTraduction_ranking.prev_week);
            //"S<size=22>EMAINE ACTUELLE</size>" : "S<size=22>EMAINE PRECEDENTE</size>";.
        textBtnPrevWeek.text = s;
        //Debug.Log("<color=red>Modif affichage score week</color>");
        textBtnPrevWeek.transform.GetChild(0).GetComponent<Text>().text = s;
        scoreWeekText.text = getStringScore(!toWeek ? localPlayerInfos.scorePrevWeek : localPlayerInfos.scoreWeek);

        if (playerWonReward)
        {
            yield return new WaitForSeconds(timeWaitOpenReward - 9 / 60f);
            notifRewardGo.SetActive(false);
            onlineRankingManager.getInstance().playerTryGetRewardRankPrevWeek();
        }

        StartCoroutine(ApplyScrollPosition(1));
    }

    public void resultGetRewardPrevWeek() // appelé : player a reccupéré la reward
    {
        menuReward.openRewardMenu();
    }

    /*
     * IEnumerator fillRankingWithList(List<infoPlayerTest> liste, bool week)
     * IEnumerator fillRankingWithList_switch(bool goWeekMenu)
     * Vector2 getBoundsItemAnim()
    */
    #region Anim remplissage de liste avec items
    public RectTransform content_week, content_record, content_prevWeek;
    public int nbStepPerf = 15;
    private RectTransform currentContent;
    private IEnumerator fillRankingWithList(/*List<infoPlayerTest> liste,*/ typeRankList typeRank)
    {
        bool anim;
        int indexPerf = 0;

        float time = Time.time;
        bool animBtnOn = false;

        currentContent = typeRank == typeRankList.rankWeek ? content_week :
            typeRank == typeRankList.rankRecord ? content_record : content_prevWeek;
        scrollRContent.content = currentContent;

        //StartCoroutine(ApplyScrollPosition(1));

        animReActivateBtnDown();

        foreach (Transform t in currentContent)
        {
            if (!t.name.Contains("espace"))
            {
                anim = t.GetSiblingIndex() + 1 <= nbItemAnimSwitchPop;
                t.GetComponent<itemPlayerRanking>().reopenItem(anim);
                if (anim)
                    yield return null;
                else
                {
                    indexPerf++;
                    if (indexPerf == nbStepPerf)
                    {
                        indexPerf = 0;
                        yield return null;
                    }
                }

                if (!animBtnOn && Time.time - time >= timeAnimSwitch)
                {
                    animReActivateBtnDown();
                    animBtnOn = true;
                }
            }
        }
        if (!animBtnOn)
        {
            yield return new WaitForSeconds(timeAnimSwitch - (Time.time - time));
            animReActivateBtnDown();
        }
    }

    public ScrollRect scrollRContent;
    public float timeAnimSwitch = 1 / 3f;
    public float timeAnimUnpop = 8 / 6f;
    private IEnumerator fillRankingWithList_switch(typeRankList typeRank)// bool goWeekMenu)
    {
        animBtnUpDownSwitch(); // desactiver btn up et down
        Vector2 bounds = getBoundsItemAnim();
        // anim close les items
        foreach (Transform t in currentContent)
        {
            if (!t.name.Contains("espace"))
            {
                t.GetComponent<itemPlayerRanking>().closeItem(
                t.GetSiblingIndex() >= bounds.x && t.GetSiblingIndex() <= bounds.y);//  <= nbItemAnimSwitchPop);
            }
        }

        yield return new WaitForSeconds(timeAnimUnpop);
        foreach (Transform t in currentContent)
        {
            if (!t.name.Contains("espace"))
            {
                t.GetChild(0).gameObject.SetActive(false);
            }
        }


        StartCoroutine(fillRankingWithList(typeRank)); /*goWeekMenu ? liste_week : liste_record,*/
            //goWeekMenu ? typeRankList.rankWeek : typeRankList.rankRecord););
        StartCoroutine(ApplyScrollPosition(1));
    }

    public int upOut = 3;
    public Transform posMaxAnimItem, posMinAnimItem;
    private Vector2 getBoundsItemAnim()
    {
        int indexStart = 0;
        int indexInList = 0;
        while (currentContent.GetChild(indexInList).position.y > posMaxAnimItem.position.y)
        {
            indexStart++;
            indexInList++;
        }
        while (indexInList < currentContent.childCount - 1
            && currentContent.GetChild(indexInList).position.y > posMinAnimItem.position.y)
        {
            indexInList++;
        }

        int indexEnd = indexInList;
        int indexStart_2 = indexStart - upOut < 0 ? 0 : indexStart - upOut;

        return new Vector2(indexStart_2, indexEnd);
    }
    #endregion

    /*
     * void scrollPosChanged()
     * void animBtnUpDownSwitch()
     * void animReActivateBtnDown()
     * void btnDownList()
     * void btnUpList()
    */
    #region Gestion btn bounds
    public Animation btnUpAnim, btnDownAnim;
    public Transform posDownLimit, posUpLimit;
    private bool btnUpOn = false, btnDownOn = false;

    // appelé par scroll quand pos change
    // modifier état btn bound
    public void scrollPosChanged()
    {
        // ne pas check si val change pcq ouverture / switch
        if (eventScrollLocked || initialisation || switchLocked)
            return;

        //Debug.Log("Activate btn Up Down");

        Transform tFirst = currentContent.GetChild(0);
        Transform tLast = currentContent.GetChild(currentContent.childCount - 1);

        if (tFirst.position.y >= posUpLimit.position.y
            && !btnUpOn)
        {
            btnUpAnim.Play("btnUpRankingFadeOn");
            btnUpOn = true;
        }
        else if (tFirst.position.y < posUpLimit.position.y
            && btnUpOn)
        {
            btnUpAnim.Play("btnUpRankingFadeOff");
            btnUpOn = false;
        }

        if (tLast.position.y <= posDownLimit.position.y
            && !btnDownOn)
        {
            btnDownAnim.Play("btnDownRankingFadeOn");
            btnDownOn = true;
        }
        else if (tLast.position.y > posDownLimit.position.y
            && btnDownOn)
        {
            btnDownAnim.Play("btnDownRankingFadeOff");
            btnDownOn = false;
        }
    }

    // appelé à init switch
    private void animBtnUpDownSwitch()
    {
        //Debug.Log("Desactivate btn Up Down");
        // btn up off si activé
        if (btnUpOn)
            btnUpAnim.Play("btnUpRankingFadeOff");
        // btn down off si activé
        if (btnDownOn)
            btnDownAnim.Play("btnDownRankingFadeOff");

        btnUpOn = false;
        btnDownOn = false;
    }

    // est appelé après que liste ait pop
    // appelé à init menu aussi
    private void animReActivateBtnDown()
    {
        btnDownAnim.Play("btnDownRankingFadeOn");
        btnDownOn = true;
    }

    public void btnDownList()
    {
        if (switchLocked)
            return;
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.btnUpDown);
        StartCoroutine(ApplyScrollPosition(0));
    }

    public void btnUpList()
    {
        if (switchLocked)
            return;
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.btnUpDown);
        StartCoroutine(ApplyScrollPosition(1));
    }
    #endregion

    /*
     * IEnumerator ApplyScrollPosition(float verticalPos)
     * string getStringScore(int score)
     * int getNbGemPerRank(int rank)
     * void DebugListPlayer(List<infoPlayerTest> liste, bool week)
     */
    #region Fonctions diverses
    private IEnumerator ApplyScrollPosition(float verticalPos)
    {
        yield return new WaitForEndOfFrame();
        scrollRContent.verticalNormalizedPosition = verticalPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRContent.transform);
    }

    public static string getStringScore(int score)
    {
        int nbDigit = score.ToString().Length;
        int nbZero = 4 - nbDigit;

        string zeroS = "";
        while (nbZero > 0)
        {
            zeroS += '0';
            nbZero--;
        }
        string numberS = score.ToString();
        return "<color=\"#4B828A\">" + zeroS + "</color>" + numberS;
    }

    private int getNbGemPerRank(int rank)
    {
        // 1 : 450
        // 2 5 : 340
        // 6 15 : 230
        // 16 50 : 160
        // 51 100 : 100
        return rank == 1 ? 450 :
            rank < 6 ? 340 :
            rank < 16 ? 230 :
            rank < 51 ? 160 : 100;
    }

    private void DebugListPlayer(List<infoPlayerTest> liste, bool week)
    {
        Debug.Log("LISTE PLAYER RANK " + (week ? "WEEK" : "RECORD"));
        foreach (infoPlayerTest i in liste)
        {
            Debug.Log("Player: " + i.nomPlayer + ", Score Week: " + i.scoreWeek + ", Score Record: " + i.scoreRecord);
        }
    }

    /*private infoPlayerPrevWeek getInfoPlayerPrevWeek(string id)
    {
        int r = 0;
        while (r < test_infoPlayerPrevWeek.Length && test_infoPlayerPrevWeek[r].idPlayer != id)
            r++;
        return test_infoPlayerPrevWeek[r];
    }*/

    /*public bool checkIfNameAlreadyUsed(string name)
    {
        bool sameName = false;
        foreach(infoPlayerTest info in test_infoPlayer)
        {
            if (info.nomPlayer == name)
                sameName = true;
        }
        return sameName;
    }*/

    #endregion
}

// 279 l -> 205 l
// 353 l -> 313 l
// 447 l -> 375 l
// 670 l -> 487 l
// 639 l -> 549 l
