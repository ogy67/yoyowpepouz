using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollInfinisManager : MonoBehaviour
{
    public Transform levelStart;
    public Animation animScrollMenu, animScroll2;
    private bool scrollIsInInitState;
    public float timeAnimClose = 1 / 6f;
    public float timeAnimSwitch = 1 / 3f;
    /*
     * void openMenuInfinis(bool fromAdventure)
     * void closeMenuInfinis()
     * IEnumerator closeScrollLate()
     * void endGame()
     * void startGame(Transform secondLevel)
     * IEnumerator startGameLate(bool scrollInitialized, Transform secondLevel)
    */
    #region Ouverture Menu / Transition to Game
    public void openMenuInfinis(bool fromAdventure)
    {
        scrollIsInInitState = true;
        // ouvre le 1er niveau ( anim : scrollInfinisOpen )
        if (fromAdventure)
        {
            scroll.nbLevelPassed = 0;
            scroll.currentLevelInfinis = 0;
            animScrollMenu.Play("scrollInfinisOpen");
        }

        // on met level start dans le 1er item de scrollLevel
        scroll.loadFirstLevel_openMenu(levelStart, fromAdventure);
        camInfinis.loadFirstLevel_openMenu(levelStart);
    }

    public void closeMenuInfinis()
    {
        // ferme le niveau actuel ( anim : scrollInfinisClose )
        animScrollMenu.Play("scrollInfinisClose");

        // on close le level centré
        StartCoroutine(closeScrollLate());

        // anim level glow OFF
    }

    private IEnumerator closeScrollLate()
    {
        yield return new WaitForSeconds(timeAnimClose);
        scroll.closeCurrentCenteredLevel_closeMenu(); // s'occupe de reinitialiser level ( close - setParent )
        camInfinis.closeCurrentCenteredLevel_closeMenu();
    }

    public GameObject playerGo;

    // bloquer render:trail pendant zoom gameOver ( player pas desactivé a la mort )
    public void zoomGameOver(bool zoomIn)
    {
        animScroll2.Play(zoomIn ? "scrollZoomIn" : "scrollZoomOut");
        if (!zoomIn)
            playerGo.SetActive(false);
    }
    public void endGame()
    {
        playerGo.SetActive(false);
        // zoom OUT sur le niveau actuel ( anim : scrollInfinisEndGame )
        animScroll2.Play("scrollEndInfinis");
        animScrollMenu.Play("scrollInfinisEndGame");
        // forcer tt les items sauf center à animCamera OUT , puis supp de scroll et camera
        scroll.closeGame_levelOut(); // s'occupe de reinitialiser level ( close - setParent )
        camInfinis.closeGame_fadeLevelOut();

        scroll.nbLevelPassed = 0;
        scroll.currentLevelInfinis = 0;
    }

    public void endGame_closeCam()
    {
        animCamScroll.SetActive(false);
    }

    public GameObject animCamScroll;
    public void startGame(Transform secondLevel)
    {
        StartCoroutine(startGameLate(scrollIsInInitState, secondLevel));
    }

    private IEnumerator startGameLate(bool scrollInitialized, Transform secondLevel)
    {
        if (!scrollInitialized)
        {
            // si on est pas sur level init, le switch va incrémenter automatiquement les variables
            //Debug.Log("Force set nbLevelPassed : 0");
            scroll.nbLevelPassed = 0;
            //Debug.Log("Force set currentLevelInfinis : 0");
            scroll.currentLevelInfinis = 0;
        }
        else
        {
            // pas de switch, cash mettre variable état initial ( revient à l'état après le switch )
            //Debug.Log("Force set nbLevelPassed : 1");
            scroll.nbLevelPassed = 1;
            //Debug.Log("Force set currentLevelInfinis : 1");
            scroll.currentLevelInfinis = 1;
        }


        if (!scrollInitialized)
        {
            // (anim : scrollInfinisSwitchStart)
            animScrollMenu.Play("scrollInfinisSwitchStart");
            // anim close level center
            StartCoroutine(closeScrollLate());
            yield return new WaitForSeconds(timeAnimSwitch / 2);//temps que level s'inverse
            // anim open level center
            openMenuInfinis(false);
            
            yield return new WaitForSeconds(timeAnimSwitch / 2);//temps que level open
        }
        else
        {
            scroll.animLevelLight(levelStart, "switchLightLevel_infinis");
        }

        // relier startGame() et initGameInfinis()
        initGameInfinis(secondLevel);
        // zoom IN sur le niveau actuel ( anim : scrollInfinisStartGame )
        animScroll2.Play("scrollStartInfinis");
        animScrollMenu.Play("scrollInfinisStartGame");
    }
    #endregion


    public static int centeredPhase = 2;
    public scrollInfinis scroll;
    public cameraInfinisManager camInfinis;
    /*
     * void initGameInfinis(Transform secondLevel)
     * void goNextPhase(Transform nextLevel)
     * void setActivateGo_level(GameObject go, bool activated)
     * void gameOverOpen()
     * void gameOverClose()
    */
    #region Ingame
    public void initGameInfinis(Transform secondLevel)
    {
        animCamScroll.SetActive(true);
        scroll.initGameInfinis(secondLevel);
        camInfinis.initGameInfinis(secondLevel);
    }

    public void goNextPhase(Transform nextLevel)
    {
        scroll.goNextPhase(nextLevel);
        camInfinis.goNextPhase(nextLevel);
        if (scrollIsInInitState)
            scrollIsInInitState = false;
        contentTrailGame.getInstance().infinisStartSwitchLevel();
    }

    public void setActivateGo_level(GameObject go, bool activated)
    {
        //Debug.Log("Set, " + go.name + " ,active = " + activated);
        if (!activated)
        {
            StartCoroutine(go.GetComponent<packNiveau_infinis>().forceTurnOff(go != levelStart));
        }
        else
        {
            go.SetActive(true);
        }
        //go.SetActive(activated);
    }

    public void gameOverOpen()
    {
        // anim zoom out
        animScrollMenu.Play("scrollInfinisZoomOut");
    }

    public void gameOverClose()
    {
        // anim zoom in
        animScrollMenu.Play("scrollInfinisZoomIn");
    }
    #endregion
}

// 200l -> 167l
// 167l -> 195l
