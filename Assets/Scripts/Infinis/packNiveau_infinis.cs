using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class packNiveau_infinis : MonoBehaviour
{
    public bool useInOnlineMode;
    public bool levelIsCut;
    public bool levelIsLong;
    public bool levelIsFreeForm;

    public GameObject[] lesCombi;
    public GameObject ligneRecord;
    public GameObject bordParticulesD;
    public GameObject bordParticulesF;
    public GameObject arriveeGo;
    public GameObject initCheckP;
    public GameObject initGameCheckP;
    public GameObject playerGo;
    public bool notInFirstLevels = false;
    private static playerMovment pMov;

    public bool packIsMirrored = false;

    public void mirror()
    {
        packIsMirrored = !packIsMirrored;

        Vector3 newScale = transform.GetChild(0).localScale;
        newScale.x = -newScale.x;
        transform.GetChild(0).localScale = newScale;
        numArriveeText.transform.localScale *= new Vector2(-1, 1);
    }


    /*
     * void launchFirstPack()
     * void launchPack(int lvl, bool phaseBonus, bool atBestScore, int forceCombi)
     * void chooseNewCombi(int lvl)
     * void chooseNewCombi_online(int lvl, int indexCombi)
     * 
     * popBoostLate()
     * List<typeBonus> loadBonus()
     */
    #region Initialisation nouveau niveau

    // loadBonus()

    // chooseNewCombi
    private bool bonusPhase = false;
    private List<GameObject> prevCombi = new List<GameObject>();
    private int nbCombiToBeReusable = 1;
    public GameObject currentCombi;
    public combiPatern currentCombiCs;

    // empecher de pop superpower 2 level de suite
    //public static bool prevLevelPopedSuperPower = false;

    public void launchFirstPack()
    {
        if (pMov == null)
            pMov = playerGo.GetComponent<playerMovment>();
        //bordParticulesD.SetActive(true);
        //GetComponent<Animation>().Play("niveauGame");
        StartCoroutine(initPlayer(initGameCheckP, 0.2f, true));

    }

    public Text numArriveeText;
    public void setNumArrivee(int num)
    {
        numArriveeText.text = num.ToString();
    }


    // est appelé après avoir été choisit
    public void launchPack(int lvl, /*bool phaseBonus,*/ bool atBestScore, int forceCombi)
    {
        //bonusPhase = phaseBonus;

        if (forceCombi == -1)
        {
            setNewCombiPreviouslyChoosed(lvl);
            //chooseNewCombi(lvl);
            if (atBestScore)
                ligneRecord.SetActive(true);
        }
        /*else
            chooseNewCombi_online(lvl, forceCombi);*/

        bordParticulesD.SetActive(true);  
    }

    public void lanchPack_late(bool phaseBonus)
    {
        bonusPhase = phaseBonus;
    }

    /*private void chooseNewCombi_online(int lvl, int indexCombi)
    {
        //Debug.Log("NB combi : " + lesCombi.Length + ", veut acceder index : " + indexCombi);
        currentCombi = lesCombi[indexCombi];
        int max = currentCombi.GetComponent<combiPatern>().lesLevel.Length;
        int validLevel = lvl > max ? max : lvl;

        currentCombi.SetActive(true);
        currentCombi.GetComponent<combiPatern>().openLevel(validLevel, loadBonus());
    }*/

    private List<GameObject> ordrePassageCombi = new List<GameObject>();
    //private GameObject firstCombi;
    public bool checkIfCombiAvailable()
    {
        // 1 : si aucune liste n'existe ( initialisation ), créer la liste initiale de l'ordre de passage des combis
        if (ordrePassageCombi.Count == 0)
        {
            List<int> usableRand = new List<int>();
            for (int e = 0; e < lesCombi.Length; e++)
            {
                usableRand.Add(e);
            }
            // on va piocher dans la liste usableRand les index disponible, au hasard, puis les retirer de la liste
            int rand;
            while (usableRand.Count > 0)
            {
                rand = Random.Range(0, usableRand.Count);
                ordrePassageCombi.Add(lesCombi[usableRand[rand]]);
                usableRand.RemoveAt(rand);
            }
        }
        // 2 : On prend directement le premier niveau de la liste

        // Pour chaque combi de la liste, on regarde si peut être prise
        combiPatern cP;
        bool found = false;
        int indexCheck = 0;
        while (found == false && indexCheck < ordrePassageCombi.Count)
        {
            // si n'est pas la même que la précédente ( arrivé au bout de la liste )
            if (ordrePassageCombi[indexCheck] != currentCombi)
            {
                cP = ordrePassageCombi[indexCheck].GetComponent<combiPatern>();
                // si le niveau le permet
                if (!cP.notInFirstLevels ||
                    cP.notInFirstLevels && infinisManager.SCORE > infinisManager.rangUpDifficulty[0])
                {
                    // on choisit ce niveau
                    found = true;
                    break;
                }
            }
            indexCheck++;
        }

        if (found)
        {
            // set currentCombi
            currentCombi = ordrePassageCombi[indexCheck];
            // niveau choisit, le mettre à la fin de la file d'attente
            ordrePassageCombi.Add(ordrePassageCombi[indexCheck]);
            ordrePassageCombi.RemoveAt(0);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void setNewCombiPreviouslyChoosed(int lvlDifficulté)
    {
        // resoud un bug rare ; prevCombi pas close
        foreach (GameObject o in lesCombi)
        {
            if (o.activeSelf)
            {
                //Debug.Log("<color=red>BUG RARE patern : " + gameObject.name+"</color>");
                //Debug.Log("<color=red>Une combi n'a pas été fermé à fin level</color> : "+o.gameObject.name);
                if (o != null)
                {
                    //Debug.Log("Desactive go ( pas null )");
                    o.SetActive(false);
                    if (o.TryGetComponent(out combiPatern c))
                    {
                        //Debug.Log("combiPatern script pas null, appelle close level");
                        o.GetComponent<combiPatern>().closeLevel();
                    }
                }
            }
        }

        currentCombiCs = currentCombi.GetComponent<combiPatern>();

        // si pack libre ( plusieurs forme de fonds )
        if (levelIsFreeForm)
        {
            // bordParticleF est celui de cette combi
            bordParticulesF = currentCombiCs.bordParticleF;
        }

        int max = currentCombi.GetComponent<combiPatern>().lesLevel.Length;

        // si depasse level 2, alors difficulté stagne 1 fois
        // MAIS pour level "NotInFirstLevel", stagne si depasse level 3 ( sinon si stagne level 2, alors reste bloqué sur 'sans obstacle' pendant 2 palier de levels )
        int levelStagne = notInFirstLevels ? 3 : 2;        
        int validLevel = lvlDifficulté > levelStagne ? lvlDifficulté - 1 : lvlDifficulté;
        

        // ne pas depasser limite de combi
        validLevel = validLevel > max ? max : validLevel;

        currentCombi.SetActive(true);
        currentCombi.GetComponent<combiPatern>().openLevel(validLevel);
    }

    /*private void chooseNewCombi(int lvl)
    {
        // resoud un bug rare ; prevCombi pas close
        foreach (GameObject o in lesCombi)
        {
            if (o.activeSelf)
            {
                //Debug.Log("<color=red>BUG RARE patern : " + gameObject.name+"</color>");
                //Debug.Log("<color=red>Une combi n'a pas été fermé à fin level</color> : "+o.gameObject.name);
                if (o != null)
                {
                    //Debug.Log("Desactive go ( pas null )");
                    o.SetActive(false);
                    if(o.TryGetComponent(out combiPatern c))
                    {
                        //Debug.Log("combiPatern script pas null, appelle close level");
                        o.GetComponent<combiPatern>().closeLevel();
                    }
                }
            }
        }

        int i;
        do
        {
            i = Random.Range(0, lesCombi.Length);
        } while (prevCombi.Contains(lesCombi[i]));

        if (prevCombi.Count == nbCombiToBeReusable)
            prevCombi.RemoveAt(0);

        currentCombi = lesCombi[i];
        currentCombiCs = currentCombi.GetComponent<combiPatern>();
        prevCombi.Add(currentCombi);


        int max = currentCombi.GetComponent<combiPatern>().lesLevel.Length; // 3 ou 4
         // difficulty = 1 (0) -> level 1
        // difficulty = 2 (10) -> level 2
        // difficulty = 3 (25) -> level 2
        // difficulty = 4 (45) -> level 3 
        // difficulty = 5 (70) -> level 4 
        int validLevel = lvl > 2 ? lvl - 1 : lvl;
        // ne pas depasser limite de combi
        validLevel = validLevel > max ? max : validLevel;

        currentCombi.SetActive(true);
        currentCombi.GetComponent<combiPatern>().openLevel(validLevel);
    }*/

    public void popBoostLate()
    {
        currentCombi.GetComponent<combiPatern>().popBonusLevel(loadBonus());
    }

    // ne pas pop 2 vies 2 niveaux de suites
    // est mit à 1 quand pop une vie
    // prochain load de bonus
    // si = 1, pas pop de bonus, le mettre à 0

    // pop une vie : _nbLevelDone = 1
    // prochain loadBonus() : IF _nbLevelDone = 1 THEN  ( _nbLevelDone = 0 AND pas proposer bonus )
    // private static bool lastLevelPopedHeart = false;
    public static int maxBoostOneMap = 5;

    private int maxVie = 6;
    //private int maxBoost = 5;// a partir de max, + compliqué de faire pop bonus
    //private float /*probVie = 0.55f, probBoost = 0.35f, probArgent = 0.7f;*/ //, probGemme = 0.23f;

    private static int compteurLevelWithNoLife = 0;
    private static int compteurLevelWithNoBoost = 0;
    private const int levelMinPopLife = 1;
    private const int levelMinPopBoost = 3;
    private const int ecartLevelPopLife = 3;
    private const int ecartLevelPopBoost = 4;
    // 1/4 -> 2/8 -> 4/16
    private float[] probasPopLife = new float[] { 5 / 16f, 7 / 16f, 9 / 16f };
    private float[] probasPopBoost = new float[] { 1 / 3f, 1 / 2f };
    private const float factorProbaBonusPhase_life = 1.4f;
    private const float factorProbaBonusPhase_boost = 1.8f;

    private static int compteurLevelWithNoGem = 0;
    private const int levelMinPopGem = 4;
    private const int ecartLevelPopGem = 3;
    private const float probaGem = 0.6f, probaGemBonusPhase = 0.9f;

    private const float probaOr = 0.65f;

    public static void resetBonusVariables_initGame()
    {
        compteurLevelWithNoLife = 0;
        compteurLevelWithNoGem = 0;
        compteurLevelWithNoBoost = 0;
    }

    // /!\ proba bloqué si possède trop de vie ou trop de boost
    private List<typeBonus> loadBonus() // creer une liste de bonus a faire pop
    {
        List<typeBonus> toPop = new List<typeBonus>();

        compteurLevelWithNoLife++;
        compteurLevelWithNoGem++;
        compteurLevelWithNoBoost++;

        int nbSuperPower = boostIngameManager.isEquipedWithSP ? boostIngameManager.getInstance().getNbSuperPowerPlayerHas() : 0;
        bool lifeEnabled = !roadMapInfinis.modeSurvieOn 
            && infinisManager.SCORE >= levelMinPopLife - 1
            && gameManager.nbVies < maxVie;
        bool boostEnabled = infinisManager.SCORE >= levelMinPopBoost - 1
            && boostIngameManager.isEquipedWithSP //&& nbSuperPower < maxBoost
            && nbSuperPower < boostIngameManager.getInstance().getNbInitSuperBoost() + 2;// on doit check que le compteur n'est pas à 'nbBoostInit' + 2
        bool gemEnabled = infinisManager.SCORE >= levelMinPopGem - 1;

        if (infinisManager.SCORE == levelMinPopLife - 1)
        {
            //Debug.Log("Life can now pop");
            compteurLevelWithNoLife = ecartLevelPopLife;
        }
        if (infinisManager.SCORE == levelMinPopBoost - 1)
        {
            //Debug.Log("Boost can now pop");
            compteurLevelWithNoBoost = ecartLevelPopBoost;
        }
        if (infinisManager.SCORE == levelMinPopGem - 1)
        {
            //Debug.Log("Gem can now pop");
            compteurLevelWithNoGem = ecartLevelPopGem;
        }

        // on regarde si peut pop une vie
        if(lifeEnabled && compteurLevelWithNoLife >= ecartLevelPopLife)
        {
            int indexProba = compteurLevelWithNoLife - ecartLevelPopLife;
            indexProba = indexProba > probasPopLife.Length - 1 ? probasPopLife.Length - 1 : indexProba; 
            float probaPop = probasPopLife[indexProba];
            if (bonusPhase) probaPop *= factorProbaBonusPhase_life;

            if (gameManager.nbVies == maxVie - 1)
                probaPop *= 0.5f;

            if (Random.Range(0f, 1f) <= probaPop)
            {
                toPop.Add(typeBonus.VIE);
                compteurLevelWithNoLife = 0;
            }
        }

        // on regarde si peut pop un boost
        if(boostEnabled && compteurLevelWithNoBoost >= ecartLevelPopBoost)
        {
            int indexProba = compteurLevelWithNoBoost - ecartLevelPopBoost;
            indexProba = indexProba > probasPopBoost.Length - 1 ? probasPopBoost.Length - 1 : indexProba;
            float probaPop = probasPopBoost[indexProba];
            if (bonusPhase) probaPop *= factorProbaBonusPhase_boost;

            /*if(nbSuperPower == maxBoost - 1)
                probaPop *= 0.5f;*/

            if (Random.Range(0f, 1f) <= probaPop)
            {
                toPop.Add(typeBonus.BOOST);
                compteurLevelWithNoBoost = 0;
            }
        }

        // gem
        if (gemEnabled && compteurLevelWithNoGem >= ecartLevelPopGem)
        {
            if (Random.Range(0f, 1f) <= (bonusPhase ? probaGemBonusPhase : probaGem))
            {
                toPop.Add(typeBonus.GEMME);
                compteurLevelWithNoGem = 0;
            }
        }

        // or
        // si dans bonus phase, on remplit les places restantes avec or
        if (bonusPhase)
        {
            while(toPop.Count != maxBoostOneMap)
                toPop.Add(typeBonus.ARGENT);
        }
        else
        {
            if(toPop.Count < 2)
                toPop.Add(typeBonus.ARGENT);
            if(Random.Range(0f, 1f) < probaOr)
                toPop.Add(typeBonus.ARGENT);
        }
        // sinon
        // 

        /* Empecher de pop superBoost 2 fois d'affilé
        if (!prevLevelPopedSuperPower)
        {
            float pBoost_2 = boostIngameManager.isEquipedWithSP && boostIngameManager.getInstance().getNbSuperPowerPlayerHas() < maxBoost ?
                bonusPhase ? 0.8f : probBoost
                : 0f;
            if (Random.Range(0.01f, 1f) < pBoost_2)
            {
                toPop.Add(typeBonus.BOOST);
                prevLevelPopedSuperPower = true;
            }
        }
        else
        {
            prevLevelPopedSuperPower = false;
        }*/

        /*if (!roadMapInfinis.modeSurvieOn)
        {
            // Empecher de pop vie 2 fois d'affilé
            if (!lastLevelPopedHeart)
            {
                float pVie = gameManager.nbVies >= maxVie ?
                    0f
                    : bonusPhase ? 0.8f : probVie;
                // 0.01 , bloquer si p == 0 ( rare ) et que declenche quand meme le bonus
                if (Random.Range(0.01f, 1f) < pVie)
                {
                    toPop.Add(typeBonus.VIE);
                    lastLevelPopedHeart = true;
                }
            }
            else
            {
                lastLevelPopedHeart = false;
            }
        }*/

        /* Pop gemme
        if (Random.Range(0.01f, 1f) < probGemme)
            toPop.Add(typeBonus.GEMME);*/

        // Phase normale 3 bonus minimum ET maximum ( empecher semblant de bonus phase )
        // Phase bonus 5 bonus minimum
        //int boostNumber = bonusPhase ? maxBoostOneMap : 3;
        //int diff = boostNumber - toPop.Count;

        /*int boostNumber = bonusPhase ? maxBoostOneMap : 3;
        int diff = boostNumber - toPop.Count;
        // Si pas bonus phase
        if (!bonusPhase)
        {
            // si aucun bonus, pop 2 argent
            if (toPop.Count == 0)
                diff = 2;
            // sinon si déjà 2 boost, 1 chance sur 2 de faire pop une piece suplémentaire
            else if (toPop.Count == 2)
                diff = Random.Range(0f, 1f) > 0.5f ? 1 : 0;
        }

        if (!bonusPhase && toPop.Count == 0)
            diff = 2;

        if (diff > 0)
        {
            for(int i = 0; i < diff; i++)
                toPop.Add(typeBonus.ARGENT);
        }*/


        /*for (int i = 0; i < (bonusPhase ? 3 : 1); i++)
            if (Random.Range(0f, 1f) < probArgent)
                toPop.Add(typeBonus.ARGENT);

        if (bonusPhase && toPop.Count < maxBoostOneMap) // si bonus phase, assurer 3 bonus mini
            while (toPop.Count < maxBoostOneMap)
                toPop.Add(typeBonus.ARGENT);*/

        //Debug.Log("<color=blue>Nombre de bonus à pop : </color>"+toPop.Count);

        return toPop;
    }
    #endregion

    /*
     * void closeFromCenter()
     *  IEnumerator close_menu()
     * void levelPassed()
     *  IEnumerator packBecomePrevPack()
     * void turnOffLevel(bool goOutScreen)
     *  IEnumerator turnOff()
     */
    #region Transition niveau (fermeture)

    public void resetLevel()
    {
        if (currentCombi != null)
        {
            currentCombi.GetComponent<combiPatern>().closeLevel();
            currentCombi.SetActive(false);
            currentCombi = null;
        }
    }
    // niveau se ferme depuis le centre. ex : changement de mode
    public void closeFromCenter()
    {
        StartCoroutine(close_menu());
    }

    private IEnumerator close_menu()
    {
        //GetComponent<Animation>().Play("niveauOutCenter");
        yield return new WaitForSeconds(0.15f);

        resetLevel();
    }

    // appelé quand passe level
    public void levelPassed()
    {
        StartCoroutine(packBecomePrevPack());
    }

    private IEnumerator packBecomePrevPack()
    {
        bordParticulesD.SetActive(false);
        bordParticulesF.SetActive(true);

        playerGo.transform.GetChild(0).gameObject.SetActive(false);
        //GetComponent<Animation>().Play("infinis_niveau_go_down");
        yield return new WaitForSeconds(0.2f);
        arriveeGo.GetComponent<Animation>().Play("changeToCheckP_infinis");

        StartCoroutine(initPlayer(initCheckP, infinisManager.getTimeRepopInfinis(), true));
    }

    // reinitialiser le niveau, le faire disparaitre si demandé
    /*public void switchArriveToInitCheckP()//bool goOutScreen)
    {
        //initCheckP.transform.parent.GetComponent<Animation>().Play("changeToCheckP_infinis");

        if (ligneRecord.activeSelf) ligneRecord.SetActive(false);
        //if (goOutScreen)
        //    GetComponent<Animation>().Play("infinis_niveau_out");

        //if(reInitCurrLevel)
        StartCoroutine(turnOff());
        // reset init_checkP
        //Debug.Log(initCheckP.transform.parent);
        
    }

    private IEnumerator turnOff()
    {
        // PARTIE A ETE ZAPPE
        // combi level bonus (pivot) ouverte tte 2 en même temps
        //Debug.Log("<color=blue>TURN OFF LEVEL " + gameObject.name+"</color>");
        yield return new WaitForSeconds(0.25f);

        bordParticulesD.SetActive(true);
        bordParticulesF.SetActive(false);
        arriveeGo.GetComponent<Animation>().Play("resetToArrivee");
        if(currentCombi != null) currentCombi.GetComponent<combiPatern>().closeLevel();

        yield return new WaitForSeconds(1/6f);

        if (currentCombi != null)
        {
            currentCombi.SetActive(false);
            currentCombi = null;
        }
        //Debug.Log("<color=green>TURN OFF LEVEL ENDS</color>");

        gameObject.SetActive(false);

        Debug.Log("TURN OFF gameObject : " + gameObject.name);
    }
    */

    #endregion



    /*
     * void reset_level()
     *  IEnumerator initPlayer(GameObject init_cP, float t, bool new_lvl)
     */
    #region Reinitialisation du joueur
    // quand meurt et recommence
    public void reset_level(float timeBeforeRepop)
    {
        StartCoroutine(initPlayer(initCheckP, timeBeforeRepop, false));//(fromRevive ? 0f : 1.5f), false));
    }
    private IEnumerator initPlayer(GameObject init_cP, float t, bool new_lvl)
    {
        //Debug.Log("Init player");
        yield return new WaitForSeconds(t);
        //GameObject init = initGameCheckP == null ? initCheckP : initGameCheckP;
        //playerGo.SetActive(false);
        if (new_lvl)
        {
            //playerGo.transform.position = init_cP.transform.position;
            playerMovment.initCenter = init_cP.transform;
            playerMovment.initPos = init_cP.transform.GetChild(0).position;
        }
        pMov.activate();

        // player repop avant fin anim zoom
        // s'assurer ensuite que pos pop init soit bien set
        if (new_lvl)
        {
            yield return new WaitForSeconds(0.5f);
            playerMovment.initPos = init_cP.transform.GetChild(0).position;
        }
    }
    #endregion


    public IEnumerator forceTurnOff(bool isFirst)
    {
        if (ligneRecord.activeSelf) ligneRecord.SetActive(false);

        bordParticulesD.SetActive(true);
        if(bordParticulesF != null)
            bordParticulesF.SetActive(false);

        arriveeGo.GetComponent<Animation>().Play("resetToArrivee");
        yield return null;

        if (!isFirst)
        {
            currentCombi.GetComponent<combiPatern>().closeLevel();
            currentCombi.SetActive(false);
            currentCombi = null;
            //if (currentCombi != null)          
        }

        gameObject.SetActive(false);
    }
}
