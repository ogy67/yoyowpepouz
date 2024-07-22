using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secretOffer : MonoBehaviour
{
    /*public bool test_openMenu = false;
    public bool test_openMenu_2 = false;
    public bool test_closeMenu = false;
    private void Update()
    {
        if (test_openMenu)
        {
            test_openMenu = false;
            openMenuSecretOffer();
        }
        /*if (test_closeMenu)
        {
            test_closeMenu = false;
            btnCloseMenuSecretOffer();
        }*
        if (test_openMenu_2)
        {
            test_openMenu_2 = false;
            testOpenMenu_2();
        }
    }*/


    private static secretOffer instance;
    public static secretOffer getInstance()
    {
        if(instance == null)
            instance = GameObject.Find("secretOfferCanvas").transform.GetChild(0).GetComponent<secretOffer>();
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(gameStarted());
        loadCompteurData();
    }

    /*private bool secretOfferTimerFinished = false; // par défaut au lancement
    public float timeInGame_toEnableOffer = 5 * 60f; // 5 minutes*/

    /*private IEnumerator gameStarted()
    {
        yield return new WaitForSeconds(timeInGame_toEnableOffer);
        secretOfferTimerFinished = true;
    }*/




    // si refuse, augmanter range re-proposer ?


    public int goalDownloadedApp = 60;
    public int goalNormal = 100;
    public int maxDiffGoal = 10;

    // compteur win level
    private int currentGoalCompteur;
    private int currentCompteur;
    private void loadCompteurData()
    {
        object o = SaveSystem.loadData(typeSave.SPECIAL_OFFER_INFO);
        if(o == null)
        {
            // créeer les valeures de reference
            currentCompteur = 0;
            currentGoalCompteur = goalDownloadedApp;
            saveCompteurData();
        }
        else
        {
            int[] tab = (int[])o;
            currentCompteur = tab[0];
            currentGoalCompteur = tab[1];
        }
    }

    private void saveCompteurData()
    {
        SaveSystem.Save(typeSave.SPECIAL_OFFER_INFO, new int[] { currentCompteur, currentGoalCompteur });
    }

    private void closeOffre_resetValues()
    {
        // on recalcul le compteur
        currentCompteur = 0;
        currentGoalCompteur = goalNormal + Random.Range(0, maxDiffGoal);
        saveCompteurData();
    }


    private int compteurInfinis = 0;
    private int nbWinInfinis_toOnePoint = 2; // une game sur 2 est comptabilisé
    public void playerPassedCheckP()
    {
        if (infinisManager.infinis_Mode_On)
        {
            compteurInfinis++;
            if (compteurInfinis == nbWinInfinis_toOnePoint)
            {
                compteurInfinis = 0;
                currentCompteur++;
            }
        }
        else
        {
            // incrementer le compteur
            currentCompteur++;
        }
    }

    private bool offerGivenThisSession = false;
    public void playerEndGame()
    {
        saveCompteurData();
        //Debug.Log("End game, compteur secretOffer : " + currentCompteur+" , goal : "+currentGoalCompteur);

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //    return;

        // si a atteind le bon nombre pour activer offre speciale
        // et que n'a pas déjà eut l'offre cette session de jeu
        if (currentCompteur >= currentGoalCompteur
        && !offerGivenThisSession)
        {
            StartCoroutine(launcher.checkInternetConnection((isConnected) => {
                if(isConnected && managerAdMob.getInstance().AdIsReadyToShow(typeAd.special_offer_ad))
                {
                    StartCoroutine(waitOpenOffer());
                }
            }));

            /* check si pub peut être lancé
            if (managerAdMob.getInstance().AdIsReadyToShow(typeAd.special_offer_ad))
            {
                StartCoroutine(waitOpenOffer());
            }*/
        }
    }





    /*private void testOpenMenu_2()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("<color=red>Pas d'internet</color>");
            return;
        }
        secretOfferTimerFinished = true;
        StartCoroutine(waitOpenOffer());
    }*/

    private bool isDoingSomethingElse()
    {
        // multijoueur comprit dans infoManager
        return shopManager.menuShopOpen
            || defiManager.menuDefiOpen
            || paramManager.menuParamOpen
            || infoMenuManager.menuInfoOpen
            || expertMode.menuStarOpen
            || managerRankInfinisV2.isInConnectionProcess
            || rankingInfinis.menuOpen
            || notifDefiDone.isAnimatingNotif
            || notifStarUnlocked.isAnimating;
        // encore scroll
    }

    public float timeWaitEndAction = 1.5f;
    private IEnumerator waitOpenOffer()
    {
        // attend d'abord que timer end
        // se fait couper si entre dans une session de jeu
        while (/*!secretOfferTimerFinished
            // gameOn = true de start -> end anim star
            &&*/ managerNiveau.gameOn)
            yield return null;
        /*if (managerNiveau.gameOn)
            yield break;*/

        /*yield return new WaitForSeconds(timeWaitEndAction);

        if (managerNiveau.gameOn)
            yield break;*/

        bool EndLoop = false;
        float timeDoNothing;
        while (!EndLoop)
        {
            // En premier, on attend que finit son action
            while (!managerNiveau.gameOn
                && isDoingSomethingElse())
            {
                // on attend finit ce qu'il fait 
                yield return new WaitForSeconds(0.5f);
            }
            if (managerNiveau.gameOn)
                yield break;
            // Ensuite , on attend que ne fait rien x secondes
            timeDoNothing = Time.time;
            while (!managerNiveau.gameOn
                && !isDoingSomethingElse()
                && Time.time - timeDoNothing < timeWaitEndAction)
            {
                yield return new WaitForSeconds(0.2f);
            }
            if (managerNiveau.gameOn)
                yield break;
            // Si est en train de faire quelque chose à ce moment, il faut relancer la boucle

            if (!isDoingSomethingElse())
            {
                EndLoop = true;
                // on ouvre le menu offre speciale
                openMenuSecretOffer();
            }
        }
    }




    public Animation animMenu;
    public animFondStar animFond;
    private void openMenuSecretOffer()
    {
        animMenu.Play("openSecretOffer");
        animFond.playAnim();
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
    }

    public void btnCloseMenuSecretOffer()
    {
        //Debug.Log("Anim close menu offer");
        animMenu.Play("closeSecretOffer");
        animFond.stopAnim();
        closeOffre_resetValues();
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
    }

    public void btnGetOffer()
    {
        // lancer request load ad
        managerAdMob.getInstance().ShowRewardedAd(typeAd.special_offer_ad);
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);
    }

    public void resultGetOffer(bool offerIsGiven)
    {
        if (offerIsGiven)
        {
            offerGivenThisSession = true;
            StartCoroutine(animGiveReward());
        }
        else
        {
            btnCloseMenuSecretOffer();
        }
    }

    public int nbGemOffer = 50;
    public float timeWaitBlastReward = 25 / 60f;
    public Transform posBlastReward;
    private IEnumerator animGiveReward()
    {
        animMenu.Play("closeSecretOffer_getReward");
        yield return new WaitForSeconds(timeWaitBlastReward);
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbGemOffer, posBlastReward.position);
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);
        animFond.stopAnim();
        closeOffre_resetValues();
    }
}
