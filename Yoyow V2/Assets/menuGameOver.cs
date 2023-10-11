using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuGameOver : MonoBehaviour
{
    private static menuGameOver instance;
    public static menuGameOver getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("menuGameOver").GetComponent<menuGameOver>();
        return instance;
    }

    public Animation myAnim;
    public GameObject revive1Go, revive2Go, scoreAventureGo, scoreInfinisGo;
    public Color crownOnColor, crownOffColor;
    private bool modeInfinisOn = false, modeStarOn = false;
    private int currentScore;


    public Text scoreInfinisText_best, scoreInfinisText_current;
    public Text scoreInfinisTextOmbre_best, scoreInfinisTextOmbre_current;
    public Image[] progBarCrownImgs;
    public static bool hasRevived = false; // ne peut pas beneficier du bonus 2 fois par partie
    private bool playerDied = false;

    public void typeGameSwitched()
    {
        modeInfinisOn = !modeInfinisOn;
    }

    public void openMenu(bool canRevive, int score, bool died)
    {
        modeStarOn = expertMode.modeExpertOn && !infinisManager.infinis_Mode_On;
        //modeInfinisOn = infinisManager.infinis_Mode_On;
        currentScore = score;
        playerDied = died;

        fillGameOverInfos();
        StartCoroutine(openMenuLate(canRevive, score));
    }

    public float waitPlayerDiedTime = 1f;
    public scrollInfinisManager scrollManager;
    private IEnumerator openMenuLate(bool canRevive, int score)
    {
        yield return new WaitForSeconds(waitPlayerDiedTime);

        sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.gameOver_open);

        myAnim.Play(canRevive ? "openRevive_2" : "openGameOver");

        zoomCamera(false);
        // ouvrir menu Revive
        if (canRevive)
        {
            // on ouvre le menu revive
            revive1Go.SetActive(modeStarOn);
            revive2Go.SetActive(!modeStarOn);

            // lancer le timer (IEnumerator)
            StartCoroutine(waitTimeAdOut());
        }
        // ouvrir menu GameOver
        else
        {
            musicManager.getInstance().gameOverOpenned();

            scoreAventureGo.SetActive(!modeInfinisOn);
            scoreInfinisGo.SetActive(modeInfinisOn);

            if (!modeInfinisOn && modeStarOn && score == 3)
            {
                // anim speciale pour etoile obtenue
                StartCoroutine(giveStar());
                gameManager.viePerdue = false; // sinon ne donne pas etoile
            }
        }
    }

    public void zoomCamera(bool zoomIn)
    {

        cameraGame.getInstance().zoomLevel(!zoomIn, 3f, modeInfinisOn);
        /*if (!modeInfinisOn)
        {
            Debug.Log("Zoom mode aventure");
            cameraGame.getInstance().zoomLevel(!zoomIn, 3f);
        }
        else
        {
            Debug.Log("Zoom mode infinis");
            scrollManager.zoomGameOver(zoomIn);
        }*/
    }

    /*
     * IEnumerator closeGameOverMenu()
     * IEnumerator giveStar()
     * void fillGameOverInfos()
    */
    #region GAME OVER
    public float timeCloseAnim = 15 / 60f;
    public gameManager gM;
    private IEnumerator closeGameOverMenu()
    {
        zoomCamera(true);

        //cameraGame.getInstance().zoomLevel(false, 3f);

        sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.gameOver_close);

        myAnim.Play("closeGameOver");
        yield return new WaitForSeconds(timeCloseAnim);
        scoreAventureGo.SetActive(false);
        scoreInfinisGo.SetActive(false);

        StartCoroutine(gM.closeGame(false, playerDied, false, true));
        if (infinisManager.infinis_Mode_On)
            appliManager.instance.quitInfiniteMode(false);

        /*if (!infinisManager.infinis_Mode_On)
            StartCoroutine(gM.closeGame(false, false, false, true));
        else
        {
            StartCoroutine(gM.closeGame(true, false, false, true));
            appliManager.instance.quitInfiniteMode(false);
        }*/
    }

    public float timeAnimOpenGameOver = 23 / 60f;
    private bool givingStar = false;
    private IEnumerator giveStar()
    {
        givingStar = true;
        yield return new WaitForSeconds(timeAnimOpenGameOver);
        myAnim.Play("openStar");
        sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.gameOver_getStar);
        yield return new WaitForSeconds(90 / 60f);
        givingStar = false;
    }

    private void fillGameOverInfos()
    {
        // mettre à jour l'affichage en fonction du score
        if (modeInfinisOn)
        {
            int bestScoreInfinis = SaveSystem.record_infinis;

            scoreInfinisText_current.text = currentScore.ToString();
            scoreInfinisTextOmbre_current.text = currentScore.ToString();
            scoreInfinisText_best.text = bestScoreInfinis.ToString();
            scoreInfinisTextOmbre_best.text = bestScoreInfinis.ToString();
        }
        else
        {
            int i = 1;
            foreach(Image img in progBarCrownImgs)
            {
                img.color = i <= currentScore ? crownOnColor : crownOffColor;
                i++;
            }
        }
    }
    #endregion

    /*
     * IEnumerator waitTimeAdOut()
     * void canReviveClose()
     * IEnumerator getLifeAnim()
     * IEnumerator switchReviveToGameOver()
    */
    #region REVIVE
    public Image progBarImg;
    public float timeWaitClickAd = 4f;
    public float initDelayTime = 2 / 6f;
    private bool waitAdOn = false; // peut être mit sur false si btn pass ad
    //public animFondStar animStarFondRevive;
    private IEnumerator waitTimeAdOut()
    {
        // Appelé quand menu revive est ouvert

        //animStarFondRevive.playAnim();

        progBarImg.fillAmount = 1;
        waitAdOn = true;

        //yield return new WaitForSeconds(0.8f); // temps de l'anim de mort
     
        yield return new WaitForSeconds(initDelayTime);

        float initTime = Time.time;
        while(Time.time < initTime + timeWaitClickAd 
            && waitAdOn)
        {
            yield return null;
            // update prog bar
            progBarImg.fillAmount = 1 - 1 * (Time.time - initTime) / timeWaitClickAd;
        }

        // si pas click pass ad OU watch ad
        if (waitAdOn)
        {
            canReviveClose(true, false, false);
            waitAdOn = false;
        }
    }

    public void canReviveClose(bool timeIsOut, bool btnWatchAd, bool btnPassAd)
    {
        if (timeIsOut || btnPassAd)
        {
            // anim Revive to Game Over
            StartCoroutine(switchReviveToGameOver());
            sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.gameOver_switch);
        }
        else if (btnWatchAd)
        {
            // montrer pub
            // anim Close Revive
            StartCoroutine(getLifeAnim());
        }
    }

    // appelé par btnAction()
    public float timeAfterAnimLife = 0.3f;
    //public float timeBeforeBlast = 0.2f;// après anim getReviveOffer lancée
    //public float timeCloseAfterBlast = 0.7f;
    public float timePlayerRepopAfterRevive = 0.7f;
    public static int adLifeSucced = 0;
    public Transform posPopLife;
    private IEnumerator getLifeAnim()
    {
        adLifeSucced = 0;
        launcher.getInstance().UserChoseToWatchAd(typeAd.revive_ad);

        // la pub passe
        while (adLifeSucced == 0) // on met en pause le jeu le temps de la pub
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        // est fermé

        if (adLifeSucced == 1)
        {
            hasRevived = true;
            // anim getReviveOffer
            myAnim.Play("getReviveOffer_2");
            sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.getLife);

            if (expertMode.modeExpertOn && !infinisManager.infinis_Mode_On)
            {
                yield return new WaitForSeconds(0.3f);
                gameManager.getInstance().giveRecompense(recompense.VIE, true);
                //gameManager.viePerdue = false; // sinon ne donne pas etoile
                //yield return new WaitForSeconds(timeAfterAnimLife - 0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
                gameManager.getInstance().giveRecompense(recompense.VIE, true);
                yield return new WaitForSeconds(0.25f);
                gameManager.getInstance().giveRecompense(recompense.VIE, true);
                //yield return new WaitForSeconds(timeAfterAnimLife - 0.5f);
            }

            //yield return new WaitForSeconds(timeBeforeBlast);

            // on ne pop plus
            // on pop les recompenses en blast
            /*popYowGemManager.getInstance().popRec_game(
                expertMode.modeExpertOn && !infinisManager.infinis_Mode_On ? new List<typeReward_game>() { typeReward_game.life }
                : new List<typeReward_game>() { typeReward_game.life, typeReward_game.life }
                , posPopLife.position);*/

            //yield return new WaitForSeconds(timeCloseAfterBlast);

            //sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.gameOver_close);
            myAnim.Play("closeRevive_2");
            //animStarFondRevive.stopAnim();

            zoomCamera(true);
            //cameraGame.getInstance().zoomLevel(false, 3f);

            yield return new WaitForSeconds(timePlayerRepopAfterRevive);

            if (infinisManager.infinis_Mode_On)
                StartCoroutine(infinisManager.getInstance().reactiveBtnChangeRot(false, true));
            else
                StartCoroutine(gM.restart(true));
        }
        else
        {
            StartCoroutine(switchReviveToGameOver());
        }
    }

    // appelé par btnAction() ou btnFondMenu()
    public float timeMiddleAnimSwitch = 1 / 6f;
    //public GameObject btnAdImg, btnBackImg;
    private IEnumerator switchReviveToGameOver()
    {
        //animStarFondRevive.stopAnim();
        myAnim.Play("reviveToGameOver_2");
        musicManager.getInstance().gameOverOpenned();

        yield return new WaitForSeconds(timeMiddleAnimSwitch);

        scoreInfinisGo.SetActive(modeInfinisOn);
        scoreAventureGo.SetActive(!modeInfinisOn);

        if (modeInfinisOn || !modeStarOn)
            revive2Go.SetActive(false);
        else
            revive1Go.SetActive(false);

        // switch img btn ad -> back
        //btnAdImg.SetActive(false);
        //btnBackImg.SetActive(true);

        fillGameOverInfos();
    }
    #endregion

    public void btnAction()
    {
        if (waitAdOn)
        {
            
            sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.btn_action);
            waitAdOn = false;
            canReviveClose(false, true, false);
        }
        else
        {
            if (!givingStar)
            {
                sfxGame.getInstance().playSfx_listgameOver(typeSfx_gameOver.btn_action);
                StartCoroutine(closeGameOverMenu());
            }
        }
    }

    // appelé par le fond, close - go gameOver
    public void btnFondMenu()
    {
        // si dans phase ad, switch to gameover
        if (waitAdOn)
        {
            waitAdOn = false;
            canReviveClose(false, false, true);
        }
        // si dans phase gameOver, close gameover
        else
        {
            if (!givingStar)
                StartCoroutine(closeGameOverMenu());
        }
    }
}
