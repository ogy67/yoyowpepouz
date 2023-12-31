﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollBot : MonoBehaviour
{
    public Text nbUpdateText, nbUpdateText_ombre;

    private bool loaded = false;
    public void loadBots()
    {
        if (!loaded)
        {
            changeNbBotText(0);
            nbUpdateText.text = "0";
            nbUpdateText_ombre.text = "0";

            loadSavedListBot();
            loaded = true;
        }
    }

    public void btnResetScoreWeekBots()
    {
        //botInfoCard b;
        foreach (Transform t in contentScroll)
        {
            if (t.gameObject.activeSelf)
            {
                t.GetComponent<botInfoCard>().resetScoreWeek();
                //.
                //listInfoBot.Add(t.GetComponent<botInfoCard>().cardInfoToStringSaveLocal());
                // reset update de l'item
                //b.resetBotUpdate();
            }
        }
    }

public GameObject botItem;
    public Transform contentScroll;
    public Text nbBotText, nbBotText_ombre;
    private int nbBots = 0;

    // quand appuie sur update, ajoutes nouveaux bots - supprime bot de bdd - update anciens bots
    private List<botInfoCard> botAddedBdd = new List<botInfoCard>();
    private List<botInfoCard> botRemovedBdd = new List<botInfoCard>();

    public Animation btnAddBotAnim, btnSaveBotAnim;
    public void btnAddBot(bool init)
    {
        GameObject o = Instantiate(botItem, contentScroll);
        o.SetActive(true);
        nbBots++;
        changeNbBotText(nbBots);
        if(!init)
            btnAddBotAnim.Play("btnAddBot");

        botInfoCard b = o.GetComponent<botInfoCard>();
        b.newBotCreated(getNewIdBot());
        botAddedBdd.Add(b);
    }

    private string idChar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private int nbCharInId = 15;
    private string getNewIdBot()
    {
        string s = "";
        while (s.Length != nbCharInId)
            s += idChar[UnityEngine.Random.Range(0, idChar.Length)];
        return "bot-"+s;
    }

    public void btnRemoveBot(GameObject cardRemoved)
    {
        cardRemoved.SetActive(false);
        nbBots--;
        changeNbBotText(nbBots);

        botInfoCard c = cardRemoved.GetComponent<botInfoCard>();
        if (botAddedBdd.Contains(c))
            botAddedBdd.Remove(c);
        else
            botRemovedBdd.Add(c);
    }

    private void changeNbBotText(int nb)
    {
        nbBotText.text = nb.ToString();
        nbBotText_ombre.text = nb.ToString();
    }


    private int nbUpdate = 0;
    public void nbUpdateChanged(bool addUpdate)
    {
        nbUpdate += addUpdate ? 1 : -1;
        nbUpdateText.text = nbUpdate.ToString();
        nbUpdateText_ombre.text = nbUpdate.ToString();
    }


    public void btnSaveListBot()
    {
        //btnSaveBotAnim.Play("btnAddBot");
        List<string> listInfoBot = new List<string>();
        botInfoCard b;
        foreach (Transform t in contentScroll)
        {
            if (t.gameObject.activeSelf)
            {
                b = t.GetComponent<botInfoCard>();
                listInfoBot.Add(t.GetComponent<botInfoCard>().cardInfoToStringSaveLocal());
                // reset update de l'item
                b.resetBotUpdate();
            }
        }
        
        // reset nb update compteur
        nbUpdate = 0;
        nbUpdateText.text = "0";
        nbUpdateText_ombre.text = "0";



        // on fait save en local
        SaveSystem.Save(typeSave.DEV_LIST_BOT, listInfoBot);

        // fin update, reinitialise
        botRemovedBdd = new List<botInfoCard>();
        botAddedBdd = new List<botInfoCard>();
    }

    private void loadSavedListBot()
    {
        List<string> savedList = (List<string>)SaveSystem.loadData(typeSave.DEV_LIST_BOT);// load saved liste
        if (savedList.Count != 0)
        {
            foreach (string s in savedList)
            {
                btnAddBot(true);
                botAddedBdd[botAddedBdd.Count - 1].loadCardWithSavedInfo(s);
            }
        }
    }

    public List<string> getAllIdsBot()
    {
        List<string> retour = new List<string>();
        //GameObject o;
        foreach (Transform t in contentScroll)
        {
            //o = t.gameObject;
            retour.Add(t.gameObject.GetComponent<botInfoCard>().myId);
        }
        return retour;
    }
    /*public bool testIdIsABot(string id)
    {
        GameObject o;
        foreach (Transform t in contentScroll)
        {
            o = t.gameObject;
            if (o.activeSelf && o.GetComponent<botInfoCard>().myId == id)
            {
                return true;
            }
        }
        return false;
    }*/

    public List<infoPlayerTest> getListInfoBots()
    {
        List<infoPlayerTest> list = new List<infoPlayerTest>();
        GameObject o;
        foreach(Transform t in contentScroll)
        {
            o = t.gameObject;
            if (o.activeSelf)
            {
                list.Add(o.GetComponent<botInfoCard>().toInfoPlayer());
            }
        }
        return list;
    }

    public List<infoPlayerTest> getListBotRemoved()
    {
        List<infoPlayerTest> retour = new List<infoPlayerTest>();
        foreach(botInfoCard b in botRemovedBdd)
        {
            retour.Add(b.toInfoPlayer());
        }

        return retour;
    }
}
