using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuRewardRank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool test_btnOpenMenu = false;
    public bool test_btnNextStep = false;
    public int test_rank = 1;
    public int test_gem = 450;
    // Update is called once per frame
    void Update()
    {
        if (test_btnOpenMenu)
        {
            test_btnOpenMenu = false;
            initValues(test_rank, test_gem);
            openRewardMenu();
        }
        else if (test_btnNextStep)
        {
            test_btnNextStep = false;
            btnPassStep();
        }
    }

    public int[] framePause;
    public Animation animOpen;
    private IEnumerator animGetReward()
    {
        //float timeWait;
        if(currentStep == 0) // ouvre voyant rank
        {
            animOpen.Play("openRewardRanking");
            int currentTimeAnimator = 0;
            while(currentTimeAnimator < framePause[0])
            {
                yield return null;
                currentTimeAnimator = (int)(animOpen["openRewardRanking"].time * 60);
            }
            animOpen["openRewardRanking"].speed = 0f;

        }
        else if(currentStep == 1) // ouvre voyant gem
        {
            animOpen["openRewardRanking"].speed = 1f;
            int currentTimeAnimator = (int)(animOpen["openRewardRanking"].time * 60);
            while(currentTimeAnimator < framePause[1])
            {
                yield return null;
                currentTimeAnimator = (int)(animOpen["openRewardRanking"].time * 60);
            }
            animOpen["openRewardRanking"].speed = 0f;
        }
        else // ferme menu
        {
            animOpen["openRewardRanking"].speed = 1f;
            popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbGemReward, posPop.position);
        }
    }

    private int nbGemReward, rankLocalPlayer;
    public void initValues(int rankPlayer, int nbGemWon)
    {
        nbGemReward = nbGemWon;
        rankLocalPlayer = rankPlayer;
    }

    public Transform posPop;
    public Text rankText, rankOmbreText, rewardText, rewardOmbreText;
    private int currentStep;
    public void openRewardMenu()
    {
        currentStep = 0;
        animGetReward_2();
        rankText.text = rankLocalPlayer.ToString();
        rankOmbreText.text = rankLocalPlayer.ToString();
        rewardText.text = nbGemReward.ToString();
        rewardOmbreText.text = nbGemReward.ToString();
    }

    public void btnPassStep()
    {
        //if (animOpen["openRewardRanking"].speed == 1f)
        //    return;
        currentStep++;
        animGetReward_2();
    }

    private void animGetReward_2()
    {
        if(currentStep == 0)
        {
            animOpen.Play("openRewardRanking");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_open);
        }
        else if(currentStep == 1)
        {
            animOpen.Play("openRewardRanking_2");
            StartCoroutine(getRewardClick());
            //sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_get);
        }
        else if(currentStep == 2)
        {
            animOpen.Play("openRewardRanking_3");
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_get);
            popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbGemReward, posPop.position);
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_close);
        }
    }

    public float timeWaitSfxRewardCome = 0.45f;
    private IEnumerator getRewardClick()
    {
        yield return new WaitForSeconds(timeWaitSfxRewardCome);
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_anim);
    }

    /*public float timeAnimPopGem = 1f;
    private IEnumerator animSfxRewardRank()
    {
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_anim);
        yield return new WaitForSeconds(timeAnimPopGem);
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.rewardWeek_get);
    }*/
}
