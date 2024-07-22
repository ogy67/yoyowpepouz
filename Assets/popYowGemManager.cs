using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class popYowGemManager : MonoBehaviour
{
    public GameObject monnaieAnim_go, gemAnim_go;
    void Start()
    {
        setRatioScreen();
        monnaieAnim = monnaieAnim_go.GetComponent<Animation>();
        gemAnim = gemAnim_go.GetComponent<Animation>();
    }

    private static popYowGemManager instance;
    public static popYowGemManager getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("managerRecYowGems").GetComponent<popYowGemManager>();
        }
        return instance;
    }


    public float refScreenHeightPixels = 2160f;
    private void setRatioScreen()
    {
        // en fonction du ratio de screen actuel
        float currentScreenHeightPixels = Camera.main.pixelHeight; //Screen.height; fonctionne seulement pour telephone
        itemRewardYowGem.factorValues = currentScreenHeightPixels / refScreenHeightPixels;
        itemRewardBoost.factorValues = currentScreenHeightPixels / refScreenHeightPixels;
    }


    /* Utilisés par rewardBoost
     * Vector2 getRandomBlastDirection()
     * float getSpeedBlast(int nbPop)
     * */
    public Vector2 getRandomBlastDirection()
    {
        if (itemRewardYowGem.factorValues == -1)
        {
            setRatioScreen();
        }

        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
                * 40 * itemRewardYowGem.factorValues;
    }
    public float getSpeedBlast(int nbPop)
    {
        float speed = minSpeedItemBlast;
        if (nbPop > 10)
        {
            float f = (nbPop - 10) / (maxItemToPop - 10);
            speed += (maxSpeedItemBlast - minSpeedItemBlast) * (f > 1 ? 1 : f);
        }
        return speed;
    }

    public float getSpeedBlast_boost(int nbPop)
    {
        float speed = minSpeedItemBlast;
        if (nbPop > 10)
        {
            float f = (nbPop - 10) / (maxItemToPop - 10);
            speed += (maxSpeedItemBlast_boost - minSpeedItemBlast) * (f > 1 ? 1 : f);
        }
        return speed;
    }

    //public Image _screenLocker;
    public int maxItemToPop = 70;
    public float maxSpeedItemBlast = 1000f, maxSpeedItemBlast_boost = 600f, minSpeedItemBlast = 400f;
    public float itemMenu_facteurDistanceBlast = 1.3f;
    public float itemGame_facteurDistanceBlast = 1.5f;
    public GameObject rewardItem;
    public Transform rewardParent;
    public Transform targetCompteurYow, targetCompteurYow_tuto, targetCompteurGem, targetCompteurLife;
    public static bool isGivingRec = false;

    private void popRec(typeReward_game typeR, int nbPop, Vector2 posOnScreen, float facteurDistance)
    {
        // à partir de 10 nbRecGiven ( star ) , on augmente la vitesse jusqu'a vitesse max
        float speed = getSpeedBlast(nbPop);

        isGivingRec = true;
        // on bloque toutes les UI, jusqu'à plus aucun itemOn
        //_screenLocker.raycastTarget = true;

        Transform posFinale;
        switch (typeR)
        {
            case typeReward_game.yow:
                posFinale = targetCompteurYow;
                break;
            case typeReward_game.gem:
                posFinale = targetCompteurGem;
                break;
            default:
                posFinale = targetCompteurLife;
                break;

        }
        Vector2 directionBlastRandom;

        float nbRecPerItem = (float)nbPop / maxItemToPop;

        if (nbRecPerItem < 1)
            nbRecPerItem = 1f;

        int tour = (nbPop < maxItemToPop ? nbPop : maxItemToPop);

        int totalGiven = 0;
        float totalGivenFloat = 0f;
        int given_single;

        // on pop toute les rec sur posOnScreen
        for (int i = 0; i < tour; i++)
        {
            directionBlastRandom = getRandomBlastDirection();
            given_single = (int)nbRecPerItem;

            // si on ne depasse pas la limite de pop, on fait des pop simple
            if (nbRecPerItem > 1)
            {
                if (i != tour - 1)
                {
                    totalGiven += (int)nbRecPerItem;
                    totalGivenFloat += nbRecPerItem;

                    // gérer les virgules
                    // float: 1.3 -> 2.6 -> 3.9 -> 5.2 !!!
                    // int: 1 -> 2 -> 3 -> 4 !!! -> 5
                    if (totalGivenFloat - totalGiven >= 1)
                    {
                        given_single++;
                        totalGiven++;
                    }
                }
                else // dernier item
                {
                    // on donne le nombre de rec restante
                    given_single = nbPop - totalGiven;
                }
            }

            // on instantie
            GameObject item = Instantiate(rewardItem, rewardParent);
            item.SetActive(true);
            item.GetComponent<itemRewardYowGem>()
                .pop(posOnScreen, directionBlastRandom, posFinale, given_single, typeR, speed, facteurDistance, false);
        }
    }

    public int minAngleBetween_rewardIngame = 40;

    private void popRecGame(List<typeReward_game> typeR_tab, Vector2 posOnScreen, float facteurDistance)
    {
        float speed = minSpeedItemBlast;
        //isGivingRec = true;
        Transform posFinale;
        Vector2 directionBlastRandom;

        // on enregistre directionBlast dans tab
        // on ne choisit pas une direction avec angle trop proche d'une autre recompense
        List<Vector2> allDirections = new List<Vector2>();
        bool checkNotNear = typeR_tab.Count <= 3;

        for (int i = 0; i < typeR_tab.Count; i++)
        {
            posFinale = typeR_tab[i] == typeReward_game.yow ? 
                /*TutoManager.tutoOn ? targetCompteurYow_tuto :*/ targetCompteurYow 
                : targetCompteurLife;
            directionBlastRandom = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
                * 40 * itemRewardYowGem.factorValues;

            if (checkNotNear)
            {
                if (allDirections.Count == 0)
                {
                    allDirections.Add(directionBlastRandom);
                }
                else
                {
                    bool foundAngle = false;
                    while (foundAngle == false)
                    {
                        bool angleEnoughFar = true;
                        foreach (Vector2 v in allDirections)
                        {
                            if (Mathf.Abs(Vector2.Angle(directionBlastRandom, v)) < minAngleBetween_rewardIngame)
                            {
                                angleEnoughFar = false;
                            }
                        }
                        if (angleEnoughFar)
                        {
                            allDirections.Add(directionBlastRandom);
                            foundAngle = true;
                        }
                        else
                        {
                            directionBlastRandom = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
                                * 40 * itemRewardYowGem.factorValues;
                        }
                    }
                }
            }

            // on instantie
            GameObject item = Instantiate(rewardItem, rewardParent);
            item.SetActive(true);
            item.GetComponent<itemRewardYowGem>()
                .pop(posOnScreen, directionBlastRandom, posFinale, 1, typeR_tab[i], speed, facteurDistance, true);
        }
    }

    // popRecGame : 
    // les 3 recompense ( max 3 ) ne doivent pas se supperposer quand pop
    public void popRec_game(List<typeReward_game> typeR_tab, Vector2 posOnScreen)
    {
        foreach(typeReward_game t in typeR_tab)
        {
            if (t == typeReward_game.yow)
                popCoinGame();
        }
        popRecGame(typeR_tab, posOnScreen, itemGame_facteurDistanceBlast);
    }
    // quand appelé , pop la rec a la position demandé
    public void popRec_main(typeReward_game typeR, int nbPop, Vector2 posOnScreen)
    {    
        popRec(typeR, nbPop, posOnScreen, itemMenu_facteurDistanceBlast);
    }


    // seulement pour ingame

    public void popGemLevelUp(int nbPop, Vector2 posOnScreen)
    {
        // ouvrir le compteur de gems inGame
        gemAnim.Play("diamondComeGame");
        popRec_main(typeReward_game.gem, nbPop, posOnScreen);
        StartCoroutine(closeGemLevelUp());
    }
    public float timeBeforeCloseGem = 1.5f;
    private IEnumerator closeGemLevelUp()
    {
        yield return new WaitForSeconds(timeBeforeCloseGem);
        gemAnim.Play("diamondOutGame");
    }


    private static Animation monnaieAnim, gemAnim;
    private static void animMonnaie(string s)
    {
        monnaieAnim.Play(s);
    }

    public static void resetPosCompteurs_endGame()
    {
        // anim appelé mm si na rien gagné -> fait nimp
        if (monnaieAnim.isPlaying)
        {
            monnaieAnim.Stop();
        }
        if (gemAnim.isPlaying)
        {
            gemAnim.Stop();
        }
    }
    // pour l'instant, plusieurs blast peuvent être géré par compteur
    // mais ne gère pas pick item sur map

    // BUG ; close n'est pas appelé si appel n'a pas ouvert le compteur
    public void openCompteurYow_inGame()
    {
        nbOpenStack++;

        if (nbOpenStack == 1)
        {
            // gérer cas : appelle ouverture pendant anim fermeture
            if (monnaieAnim.isPlaying)
                monnaieAnim.Stop();
            animMonnaie("monneyComeGame");
        }
    }

    // empecher situation : tirelirePopYow , endLevelPopYow , tirelireClose ALORS que endLevelReward pas finit chemin
    // pour pouvoir close le compteur, il faut être le dernier à l'avoir appelé
    private static int nbOpenStack = 0;

    private static int nbCoinPopedGame = 0;
    public static void popCoinGame()
    {
        //if (TutoManager.tutoOn)
        //    return;

        nbCoinPopedGame++;
    }
    public static void coinEndAnimGame()
    {
        //if (TutoManager.tutoOn)
        //    return;

        nbCoinPopedGame--;
        if(nbCoinPopedGame == 0)
        {
            animMonnaie("monneyOutGame");
            nbOpenStack = 0;
        }
    }

    
    // PB : lastItemFinished() : fonctionne bien dans menu, PAS game
    // - double appel peut être fait ingame
    public void lastItemFinished(typeReward_game typeR)
    {
            isGivingRec = false;
            //_screenLocker.raycastTarget = false;
            if (typeR != typeReward_game.life)
                StartCoroutine(SaveLate(typeR));
    }

    // attendre que reward bien prise en compte
    private IEnumerator SaveLate(typeReward_game typeR)
    {
        yield return new WaitForSeconds(0.3f);
        // on sauvegarde
        if (typeR == typeReward_game.yow)
        {
            SaveSystem.Save(typeSave.MONNEY, gameManager.monnaieVal);
        }
        else if(typeR == typeReward_game.gem)
        {
            gemmesManager.saveNbGemmes();
        }
    }
}
