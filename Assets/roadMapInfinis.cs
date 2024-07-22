using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roadMapInfinis : MonoBehaviour
{
    /*public bool test_btnOpen = false;
    public bool test_goNextLevelOnOpen = false;
    public bool test_btnReset = false;
    
        // Update is called once per frame
    void Update()
    {
        if (test_btnOpen)
        {
            openMenuRoadMap(test_goNextLevelOnOpen, false);
            test_btnOpen = false;
        }
        if (test_btnReset)
        {
            resetMenuRoadMap();
            test_btnReset = false;
        }
    }*/

    // Bloquer btn close / info pendant anim nextLevel

    public Animation btnRoadMapAnim;
    public void btnOpenRoadMapFromPause()
    {
        btnRoadMapAnim.Play("btnRoadMap");
        openMenuRoadMap(false, false);
    }

    public static int currentLevelRoadMap = 1; // max 5
    public float timeAnimOpen = 45 / 60f;
    public float timeAnimReward = 110 / 60f;
    public Animation animMenu;
    public Transform[] posUiSpawnReward;
    public int[] nbGemRewards = new int[] { 0, 5, 20, 40, 80 };
    private string[] animNames = new string[] { "","levelInfinisToLevel2", "levelInfinisToLevel3" , "levelInfinisToLevel4" , "levelInfinisToLevelSurvie" };
    private bool isOpening = false;
    public Color32[] fondColorPerLevel;
    public cameraGame camAnim;

    private bool isOpennedToGoNextLevel = false;
    private bool isOpennedForInitGame = false;
    public void openMenuRoadMap(bool goToNextLevel, bool initGameOpen)
    {
        isOpennedToGoNextLevel = goToNextLevel;
        isOpennedForInitGame = initGameOpen;
        // appelé quand clique roadMap depuis menu pause
        // appelé début de game infinis
        // appelé pendant game si level up
        isOpening = true;
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.roadMap_open);
        StartCoroutine(animOpenEnum(goToNextLevel));
    }

    public Animation bordSurvieAnim;
    private IEnumerator animOpenEnum(bool goToNextLevel)
    {
        animMenu.Play("levelInfinisOpen");

        if (goToNextLevel)
        {
            yield return new WaitForSeconds(timeAnimOpen);
            // anim next level
            animMenu.Play(animNames[currentLevelRoadMap]);

            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.roadMap_next);

            yield return new WaitForSeconds(timeAnimReward);

            camAnim.playerChangedLevelInfinis(fondColorPerLevel[currentLevelRoadMap]);
            if (currentLevelRoadMap == 4)
            {
                bordSurvieAnim.Play("bordSurvieOn");
                modeSurvieOn = true;
            }
            // anim giveReward gem pop
            popYowGemManager.getInstance().popGemLevelUp(nbGemRewards[currentLevelRoadMap], posUiSpawnReward[currentLevelRoadMap].position);
            // on attend fin popReward
            yield return new WaitForSeconds(0.5f);

            currentLevelRoadMap++;
        }

        isOpening = false;
    }


    public infinisManager infinisM;
    public static bool modeSurvieOn = false;
    public void closeMenuRoadMap()
    {
        if (isOpening)
            return;

        // appelé quand clique sur btn close
        if (infoSurvieOpen)
            btnCloseInfoSurvie();
        else
        {
            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.roadMap_close);
            animMenu.Play("levelInfinisClose");

            if (isOpennedToGoNextLevel)
            {
                infinisM.levelUpEnd();
            }
            if (isOpennedForInitGame)
            {
                infinisM.initGameSuite();
            }
        }
    }

    public void resetMenuRoadMap()
    {
        // appelé quand quitte mode infinis
        if (currentLevelRoadMap == 5)
            bordSurvieAnim.Play("bordSurvieOff");
        currentLevelRoadMap = 1;
        animMenu.Play("levelInfinisClear");
        modeSurvieOn = false;
    }


    private bool infoSurvieOpen = false;
    public void btnOpenInfoSurvie()
    {
        if (isOpening)
            return;

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.roadMap_openInfo);
        infoSurvieOpen = true;
        animMenu.Play("openInfoSurvie");
    }

    public void btnCloseInfoSurvie()
    {
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.roadMap_closeInfo);
        infoSurvieOpen = false;
        animMenu.Play("closeInfoSurvie");
    }
}
