using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemPlayerRanking : MonoBehaviour
{
    private infoPlayerTest myInfo;
    private int myRank;
    public Text nameText, nameText_ombre, scoreText, rankText, rankText_ombre;
    public GameObject fondLocalPlayer, fondOtherPlayer;
    public GameObject fondPrevWeek, fondNormal;

    public Animation myAnim;

    public GameObject rewardWeekGo;
    public Text nbGemText, nbGemText_ombre;

    public GameObject playerFilledGo, playerEmptyGo;
    public GameObject playerEmptyNormalGo, playerEmptyPrevWeekGo;
    public GameObject btnOpenInfoGo;

    private bool isEmpty;
    public void itemInstantiated_empty(typeRankList typeRank)//bool initList)
    {
        isEmpty = true;
        playerFilledGo.SetActive(false);
        playerEmptyGo.SetActive(true);
        rewardWeekGo.SetActive(false);

        if (typeRank == typeRankList.rankPrevWeek)
        {
            playerEmptyNormalGo.SetActive(false);
            playerEmptyPrevWeekGo.SetActive(true);
        }
        //if(animItem)
        //    myAnim.Play("itemPlayerPop");

        //if (initList)
            contentItem.SetActive(false);
        //else if (animItem)
        //    myAnim.Play("itemPlayerPop");
        //btnOpenInfoGo.SetActive(false);
    }

    public GameObject contentItem;

    private typeRankList typeRankItem;
    //private bool isInWeekList;
    public void itemInstantiated(infoPlayerTest info, int rangPlayer, bool isLocalPlayer
        , int nbGem, typeRankList typeRank)//, bool animItem
        //, bool initList)
    {
        isEmpty = false;
        typeRankItem = typeRank;
        //isInWeekList = week;
        myInfo = info;
        myRank = rangPlayer;

        nameText.text = info.nomPlayer;
        nameText_ombre.text = info.nomPlayer;
        scoreText.text = rankingInfinis.getStringScore(typeRankItem == typeRankList.rankWeek ?
            info.scoreWeek : typeRankItem == typeRankList.rankRecord ?  info.scoreRecord : info.scorePrevWeek);
        rankText.text = rangPlayer.ToString();
        rankText_ombre.text = rangPlayer.ToString();

        fondLocalPlayer.SetActive(isLocalPlayer);
        fondOtherPlayer.SetActive(!isLocalPlayer);
        fondPrevWeek.SetActive(typeRank == typeRankList.rankPrevWeek);
        fondNormal.SetActive(typeRank != typeRankList.rankPrevWeek);
        playerFilledGo.SetActive(true);
        playerEmptyGo.SetActive(false);



        rewardWeekGo.SetActive(typeRankItem == typeRankList.rankWeek);
        if (typeRankItem == typeRankList.rankWeek)
        {
            nbGemText.text = nbGem.ToString();
            nbGemText_ombre.text = nbGem.ToString();
        }

        //if (initList)
            contentItem.SetActive(false);
        //else if(animItem)
        //    myAnim.Play("itemPlayerPop");
    }

    public void reopenItem(bool animItem)
    {
        if(animItem)
            myAnim.Play("itemPlayerPop");
        else
            myAnim.Play("itemPlayerReset");
    }

    public void closeItem(bool animItem)
    {
        if (animItem)
            myAnim.Play("itemPlayerUnpop");
        else
            myAnim.Play("itemPlayerOff");
    }

    public void btnClickPlayer()
    {
        // anim item clicked
        myAnim.Play("itemPlayerClicked");
        if (!isEmpty && !rankingInfinis.switchLocked)
        {
            // on ouvre menu info player
            string rank = myRank == -1 ? "-" : myRank.ToString();
            menuInfoPlayerRanking.getInstance().openMenuInfoPlayer(myInfo, typeRankItem, rank);
        }
        else
        {
            sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.infoPlayer_clickEmpty);
        }
    }

    // sur 4 digit
}
