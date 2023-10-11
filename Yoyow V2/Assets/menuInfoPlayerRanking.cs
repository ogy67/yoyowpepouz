using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuInfoPlayerRanking : MonoBehaviour
{
    private static menuInfoPlayerRanking instance;
    public static menuInfoPlayerRanking getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("panInfoPlayer").GetComponent<menuInfoPlayerRanking>();
        return instance;
    }

    public voyantBoostRanking voyantSB, voyant2, voyant3;
    public Text scoreText;
    public Text playerName;
    public Text rankText;
    public Text nbStarText;
    public Animation myAnim;

    public void btnCloseMenu()
    {
        myAnim.Play("infoPlayerRankingClose");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.infoPlayer_close);
    }

    private int[] strToListIds(string s)
    {
        string[] l = s.Split(',');
        Debug.Log("STR boost used rank info : " + s);
        int[] retour = new int[] { int.Parse(l[0]), int.Parse(l[1]), int.Parse(l[2]) };
        return retour;
    }
    public void openMenuInfoPlayer(infoPlayerTest infoPlayer, typeRankList typeRankItem, string rank)
    {
        myAnim.Play("infoPlayerRankingOpen");
        sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.infoPlayer_open);

        // fill nom / rank / score / star
        scoreText.text = rankingInfinis.getStringScore(typeRankItem == typeRankList.rankWeek ?
            infoPlayer.scoreWeek : typeRankItem == typeRankList.rankRecord ? infoPlayer.scoreRecord : infoPlayer.scorePrevWeek);
        playerName.text = infoPlayer.nomPlayer;
        playerName.transform.GetChild(0).GetComponent<Text>().text = infoPlayer.nomPlayer; 
        rankText.text = rank;
        rankText.transform.GetChild(0).GetComponent<Text>().text = rank;
        nbStarText.text = infoPlayer.nbStar.ToString();
        nbStarText.transform.GetChild(0).GetComponent<Text>().text = infoPlayer.nbStar.ToString();

        // fill info boost used
        int[] boostUsed = strToListIds(typeRankItem == typeRankList.rankWeek ? infoPlayer.idsBoostUsed 
            : typeRankItem == typeRankList.rankRecord ? infoPlayer.idsBoostUsed_record : infoPlayer.idsBoostUsed_prevWeek)
            ;
        int[] levelBoostUsed = strToListIds(typeRankItem == typeRankList.rankWeek ? infoPlayer.levelBoostUsed 
            : typeRankItem == typeRankList.rankRecord ? infoPlayer.levelBoostUsed_record : infoPlayer.levelBoostUsed_prevWeek);
        int id;
        voyantBoostRanking voyant;
        for (int i = 0; i < 3; i ++)
        {
            id = boostUsed[i];
            voyant = i == 0 ? voyantSB : i == 1 ? voyant2 : voyant3;
            if (id == -1)
            {
                voyant.setInfoVoyant(typeBoostGame.none, -1);
            }
            else
            {
                voyant.setInfoVoyant(
                    typeBoostByNumBoost[id]
                    , levelBoostUsed[i]);
            }
        }
    }

    private Dictionary<int, typeBoostGame> typeBoostByNumBoost = new Dictionary<int, typeBoostGame>()
    {
        { 0, typeBoostGame.none },
        { 1, typeBoostGame.bouclier},
        { 2, typeBoostGame.super_viseur},
        { 3, typeBoostGame.bombe},
        { 11, typeBoostGame.pierre_tombale},
        { 12, typeBoostGame.tirelire},
        { 13, typeBoostGame.souris},
        { 21, typeBoostGame.ghost},
        { 22, typeBoostGame.fusee},
        { 23, typeBoostGame.ressort},
        { 31, typeBoostGame.medic},
        { 32, typeBoostGame.aimant},
        { 33, typeBoostGame.ancre},
    };

    public rankingInfinis rankingInstance;
    public Animation btnScoreWeekAnim, btnScoreRecordAnim;
    public void btnScoreLocal(bool week)
    {
        if (!rankingInfinis.switchLocked)
        {
            // si n'est pas ranked, ne pas ouvrir le menu
            int rank;
            typeRankList typeRank;
            //infoPlayerTest infoLocal = new infoPlayerTest();
            if (week)
            {
                btnScoreWeekAnim.Play("btnScoreWeekClicked");
                if (rankingInfinis.prevWeekOn)
                {
                    // info player à reccup dans liste week, pas directement localPlayerData de rankingInfinis
                    rank = rankingInstance.localRankPrevWeek;
                    typeRank = typeRankList.rankPrevWeek;
                    //infoLocal = 
                }
                else
                {
                    rank = rankingInstance.localRankWeek;
                    typeRank = typeRankList.rankWeek;
                    //infoLocal = rankingInstance.localPlayerInfos;
                }
                if (rank != -1)
                    btnScoreWeekAnim.Play("btnScoreWeekClicked");
                else
                {
                    btnScoreWeekAnim.Play("btnScoreWeekClickedRefuse");
                    sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.infoPlayer_deny);
                }
            }
            else
            {              
                rank = rankingInstance.localRankRecord;
                typeRank = typeRankList.rankRecord;
                if(rank != -1)
                    btnScoreRecordAnim.Play("btnScoreRecordClicked");
                else
                {
                    btnScoreRecordAnim.Play("btnScoreWeekClickedRefuse");
                    sfxGame.getInstance().playSfx_listRankOnline(typeSfx_rankOnline.infoPlayer_deny);
                }
                //infoLocal = rankingInstance.localPlayerInfos;
            }

            if (rank != -1)
            {
                string rankString = rank == -1 ? "-" : rank.ToString();
                openMenuInfoPlayer(rankingInstance.localPlayerInfos, typeRank, rankString);
            }

            // si week, peut être currentWeek / prevWeek
            // si rankingInfinis.prevWeekOn = true
            // sinon si rankingRecord
            // btnScoreRecordAnim

            // ouvrir menu info avec info local player
            //(week ? btnScoreWeekAnim : btnScoreRecordAnim).Play(week ? "btnScoreWeekClicked" : "btnScoreRecordClicked");
            //infoPlayerTest infoLocal = rankingInstance.localPlayerInfos;

            //int rank = week ? rankingInstance.localRankWeek : rankingInstance.localRankRecord;
            //string rankString = rank == -1 ? "-" : rank.ToString();
            //openMenuInfoPlayer(infoLocal, week ? typeRankList.rankWeek : typeRankList.rankPrevWeek, rankString);
        }
    }
}
