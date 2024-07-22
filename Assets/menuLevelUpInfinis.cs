using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuLevelUpInfinis : MonoBehaviour
{
    /* Start is called before the first frame update
    void Start()
    {
        
    }

    public bool test_btnStartGame = false, test_btnOpenLevelUp = false;

    // Update is called once per frame
    void Update()
    {
        if (test_btnStartGame)
        {
            gameStart();
            test_btnStartGame = false;
        }
        if (test_btnOpenLevelUp)
        {
            levelUp();
            test_btnOpenLevelUp = false;
        }
    }*/


    // Donner gems quand level up
    // level 1 -> 2 (15) : 10 gems
    // level 2 -> 3 (35) : 25 gems
    // level 3 -> 4 (60) : 50 gems

    public int[] nbGemsPerLevel = new int[] { 10, 25, 50 };
    public GameObject animMenuGo;
    public Animation animMenu;
    public Animation[] itemsProgBarAnims;

    public static int currentDifficultyLevel;
    public void gameStart()
    {
        currentDifficultyLevel = 1;
    }

    public void levelUp()
    {
        animMenuGo.SetActive(true);

        // 1er appel après initGame, reset le menu
        if (currentDifficultyLevel == 1)
        {
            // reset les anims
            foreach (Animation a in itemsProgBarAnims)
            {
                // animMenu:close desactive les go, les reactiver
                if (!a.gameObject.activeSelf)
                    a.gameObject.SetActive(true);
                a.Play("progBarLevelInfinisReset");
            }
            levelText.text = "1";
            levelText_ombre.text = "1";
        }
        StartCoroutine(animOpenMenu());
    }

    public float timeBeforeLevelUpAnim = 0.7f;
    public float timeAnimSwitchLevelText = 0.5f;
    public float timeBeforeBlastReward = 0.8f;
    public float timeEndRewardAnim = 1f;
    public Text levelText, levelText_ombre;
    public Text nbGemRewardText;
    public GameObject btnCloseGo;
    public Transform posPopTr;
    public Animation rewardTextAnim;
    private IEnumerator animOpenMenu()
    {
        // 1 anim open
        animMenu.Play("openLevelUpInfinis");

        yield return new WaitForSeconds(timeBeforeLevelUpAnim);
        // 2 anim level up
        animMenu.Play("levelUpInfinis");
        // temps après canClose
        // 1 : 0.25s
        // 2 : 0.4s ( cumul : 0.65f )
        // 3 : 0.1s ( cumul : 0.75f ) + 0.5
        // doit être de 1.16s


        yield return new WaitForSeconds(timeAnimSwitchLevelText / 2); // 1
        int indexReward = currentDifficultyLevel - 1;
        int nbGemReward = nbGemsPerLevel[indexReward];
        nbGemRewardText.text = "+" + nbGemReward;

        itemsProgBarAnims[currentDifficultyLevel - 1].Play("progBarLevelInfinisUp");
        // change text
        currentDifficultyLevel++;
        levelText.text = currentDifficultyLevel.ToString();
        levelText_ombre.text = currentDifficultyLevel.ToString();

        yield return new WaitForSeconds(timeBeforeBlastReward); // 2
        // anim de tt les items
        itemsProgBarAnims[currentDifficultyLevel - 2].Play("progInfinisSelected");
        rewardTextAnim.Play("rewardLevelUpInfinis");

        /*int prevDifficulty = currentDifficultyLevel - 1;
        for (int i = 0; i < itemsProgBarAnims.Length; i++)
        {
            Animation a = itemsProgBarAnims[i];
            if (i == prevDifficulty - 1)
                a.Play("progInfinisSelected");
            else if(i > prevDifficulty - 1)
                a.Play("progInfinisUnselected");
        }*/
        // pop gem
        popYowGemManager.getInstance().popGemLevelUp(nbGemsPerLevel[indexReward], posPopTr.position);

        yield return new WaitForSeconds(timeEndRewardAnim - timeBeforeBlastReward); // 3

        // attend btn close menu
        btnCloseGo.SetActive(true);
    }

    public infinisManager infinisM;
    public void btnCloseMenuUp()
    {
        btnCloseGo.SetActive(false);
        animMenu.Play("closeLevelUpInfinis");
        infinisM.levelUpEnd();
    }

    // /!\ reset menu à initGame
}
