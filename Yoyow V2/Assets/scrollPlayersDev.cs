using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollPlayersDev : MonoBehaviour
{
    public developerBddManager devBddManager;
    private List<string> allIdsBots;
    public void loadRanks(bool btnRefresh)
    {
        if (prevGlowOpen == null || btnRefresh)
        {
            if (!btnRefresh)
            {
                prevGlowOpen = glowWeek;
                prevContentOpen = contentWeek.gameObject;
            }

            allIdsBots = scrollBotM.getAllIdsBot();
            // on réccupère : prev week - week - record
            devBddManager.getWeekList();
            devBddManager.getRecordList();
            devBddManager.getPrevWeekList();
        }
    }

    public Transform contentWeek, contentRecord, contentPrevWeek;
    public void rankWeekLoaded(List<infoPlayerTest> list)
    {
        int rang = 1;
        foreach(infoPlayerTest info in list)
        {
            addItemToContent(contentWeek, rang, info.scoreWeek, info.nomPlayer, isABot(info.idPlayer));
            rang++;
        }
    }

    public void rankRecordLoaded(List<infoPlayerTest> list)
    {
        int rang = 1;
        foreach (infoPlayerTest info in list)
        {
            addItemToContent(contentRecord, rang, info.scoreRecord, info.nomPlayer, isABot(info.idPlayer));
            rang++;
        }
    }

    public void rankPrevWeekLoaded(List<infoPlayerPrevWeek> list)
    {
        int rang = 1;
        foreach (infoPlayerPrevWeek info in list)
        {
            addItemToContent(contentPrevWeek, rang, info.scorePrevWeek, info.nomPlayer, isABot(info.idPlayer));
            rang++;
        }
    }



    public GameObject itemPlayerRank;
    private void addItemToContent(Transform content,
        int rang, int score, string name, bool isBot)
    {
        GameObject o = Instantiate(itemPlayerRank, content);
        o.GetComponent<itemPlayerRankDev>().initItem(name, rang, score, isBot);
        o.SetActive(true);
    }

    public scrollBot scrollBotM;
    private bool isABot(string id)
    {
        return allIdsBots.Contains(id);
    }


    public Animation btnRefreshAnim;
    public void btnRefreshRank()
    {
        btnRefreshAnim.Play("btnRefreshRankDev");
        foreach (Transform list in new List<Transform>() { contentWeek, contentRecord, contentPrevWeek })
        {
            foreach (Transform t in list)
                Destroy(t.gameObject);
        }
        loadRanks(true);
    }




    public GameObject glowWeek, glowRecord, glowPrevWeek;
    private GameObject prevGlowOpen;
    private GameObject prevContentOpen;
    public ScrollRect scrollPlayers;
    public void btnWeek()
    {
        if (prevGlowOpen == glowWeek)
            return;
        prevGlowOpen.SetActive(false);
        prevGlowOpen = glowWeek;
        prevGlowOpen.SetActive(true);
        scrollPlayers.content = contentWeek.GetComponent<RectTransform>();

        prevContentOpen.SetActive(false);
        prevContentOpen = contentWeek.gameObject;
        prevContentOpen.SetActive(true);
    }

    public void btnRecord()
    {
        if (prevGlowOpen == glowRecord)
            return;
        prevGlowOpen.SetActive(false);
        prevGlowOpen = glowRecord;
        prevGlowOpen.SetActive(true);
        scrollPlayers.content = contentRecord.GetComponent<RectTransform>();

        prevContentOpen.SetActive(false);
        prevContentOpen = contentRecord.gameObject;
        prevContentOpen.SetActive(true);
    }

    public void btnPrevWeek()
    {
        if (prevGlowOpen == glowPrevWeek)
            return;
        prevGlowOpen.SetActive(false);
        prevGlowOpen = glowPrevWeek;
        prevGlowOpen.SetActive(true);
        scrollPlayers.content = contentPrevWeek.GetComponent<RectTransform>();

        prevContentOpen.SetActive(false);
        prevContentOpen = contentPrevWeek.gameObject;
        prevContentOpen.SetActive(true);
    }
}
