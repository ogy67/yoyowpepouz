using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.PlayerLoop.PreUpdate;

public class botInfoCard : MonoBehaviour
{
    public string myId;
    public void newBotCreated(string idBot)
    {
        myId = idBot;
    }

    public itemBoostBot itemBoostWeek1,
        itemBoostWeek2,
        itemBoostWeek3;
    public itemBoostBot itemBoostRecord1,
        itemBoostRecord2,
        itemBoostRecord3;

    public scrollBot scrollBotManager;

    public void resetScoreWeek()
    {
        scoreSemaine_ref = "0";
        weekInput.text = "0";
        // /!\ , met update On
    }
    public void resetBotUpdate()
    {
        nbUpdate = 0;
        fondCardImg.color = updateOff;

        namePlayer_ref = nameInput.text;
        nbStarPlayer_ref = starInput.text;
        scoreSemaine_ref = weekInput.text;
        scoreRecord_ref = recordInput.text;
        nameChanged = false;
        starChanged = false;
        weekChanged = false;
        recordChanged = false;

        itemBoostWeek1.resetBoostUpdate();
        itemBoostWeek2.resetBoostUpdate();
        itemBoostWeek3.resetBoostUpdate();
        itemBoostRecord1.resetBoostUpdate();
        itemBoostRecord2.resetBoostUpdate();
        itemBoostRecord3.resetBoostUpdate();
    }

    public int nbUpdate = 0;
    private void nbUpdateChanged(bool addUpdate)
    {
        int prevNbUpdate = nbUpdate;

        nbUpdate += addUpdate ? 1 : -1;
        if (nbUpdate == 0)
        {
            fondCardImg.color = updateOff;
            scrollBotManager.nbUpdateChanged(addUpdate);
        }
        else if (nbUpdate == 1 && prevNbUpdate == 0)
        {
            //Debug.Log("Activate color update on ( normal )");
            fondCardImg.color = updateOn;
            scrollBotManager.nbUpdateChanged(addUpdate);
        }
    }

    private int nbUpdate_boost = 0;
    public Image fondCardImg;
    public Color updateOff, updateOn;
    private void nbUpdateChanged_boost(bool addUpdate)
    {
        int prevNbUpdate = nbUpdate_boost;

        nbUpdate_boost += addUpdate ? 1 : -1;
        if (nbUpdate_boost == 0)
        {
            fondCardImg.color = updateOff;
            nbUpdateChanged(addUpdate);
        }
        else if(nbUpdate_boost == 1 && prevNbUpdate == 0)
        {
            Debug.Log("Activate color update on ( boost )");
            fondCardImg.color = updateOn;
            nbUpdateChanged(addUpdate);
        }
    }


    private List<itemBoostBot> listItemBoostUpdate = new List<itemBoostBot>(); 
    // appelé par un des boost quand sa valeure change
    public void itemBoostValueChanged(bool inInitState, itemBoostBot boost)
    {
        // si le boost n'est pas encore dans la liste d'update
        if (!listItemBoostUpdate.Contains(boost))
        {
            // on le rajoute
            listItemBoostUpdate.Add(boost);
            nbUpdateChanged_boost(true);
        }

        // si le boost retourne dans son état initiale
        if (inInitState)
        {
            // annuler update
            listItemBoostUpdate.Remove(boost);
            nbUpdateChanged_boost(false);
        }
    }


    public GameObject validDeleteGo;
    private bool valdDeleteOpen = false;
    public void btnRemoveBot(bool btnValidDelete)
    {
        if (!btnValidDelete)
        {
            if (!valdDeleteOpen)
            {
                valdDeleteOpen = true;
                validDeleteGo.SetActive(true);
                StartCoroutine(closeValidDeleteLate());
            }
        }
        else
        {
            scrollBotManager.btnRemoveBot(gameObject);
            // si update, on annule
            if (nbUpdate > 0)
            {
                nbUpdate = 1;
                nbUpdateChanged(false);
            }
        }
    }
    private IEnumerator closeValidDeleteLate()
    {
        yield return new WaitForSeconds(2f);
        validDeleteGo.SetActive(false);
        valdDeleteOpen = false;
    }


    private string namePlayer_ref = "", nbStarPlayer_ref = "", scoreSemaine_ref = "", scoreRecord_ref = "";
    public InputField nameInput, starInput, weekInput, recordInput;
    private bool nameChanged = false, starChanged = false, weekChanged = false, recordChanged = false;
    public void nameBotChanged()
    {
        if (initialisationOn)
            return;
        if(nameInput.text == namePlayer_ref)
        {
            if (nameChanged)
            {
                nbUpdateChanged(false);
                nameChanged = false;
            }
        }
        else if (!nameChanged)
        {
            nbUpdateChanged(true);
            nameChanged = true;
        }
    }
    public void nbStarChanged()
    {
        if (initialisationOn)
            return;
        if (starInput.text == nbStarPlayer_ref)
        {
            if (starChanged)
            {
                nbUpdateChanged(false);
                starChanged = false;
            }
        }
        else if (!starChanged)
        {
            nbUpdateChanged(true);
            starChanged = true;
        }
    }
    public void scoreSemaineChanged()
    {
        if (initialisationOn)
            return;
        if (weekInput.text == scoreSemaine_ref)
        {
            if (weekChanged)
            {
                nbUpdateChanged(false);
                weekChanged = false;
            }
        }
        else if (!weekChanged)
        {
            nbUpdateChanged(true);
            weekChanged = true;
        }
    }
    public void scoreRecordChanged()
    {
        if (initialisationOn)
            return;
        if (recordInput.text == scoreRecord_ref)
        {
            if (recordChanged)
            {
                nbUpdateChanged(false);
                recordChanged = false;
            }
        }
        else if (!recordChanged)
        {
            nbUpdateChanged(true);
            recordChanged = true;
        }
    }



    public string cardInfoToStringSaveLocal()
    {
        return nameInput.text
            + '|' + starInput.text
            + '|' + weekInput.text
            + '|' + recordInput.text
            + '|' + itemBoostWeek1.itemBoostToStringSaveLocal()
            + '|' + itemBoostWeek2.itemBoostToStringSaveLocal()
            + '|' + itemBoostWeek3.itemBoostToStringSaveLocal()
            + '|' + itemBoostRecord1.itemBoostToStringSaveLocal()
            + '|' + itemBoostRecord2.itemBoostToStringSaveLocal()
            + '|' + itemBoostRecord3.itemBoostToStringSaveLocal()
            + '|' + myId;
    }

    public developerBddManager devManager;
    public infoPlayerTest toInfoPlayer()
    {
        double timeBdd = devManager.getTimeFirestoreBdd(); // demander à onlineRankManager

        infoPlayerTest info = new infoPlayerTest();
        info.nomPlayer = nameInput.text;
        info.nbStar = starInput.text == "" ? 0 : int.Parse(starInput.text);
        info.idPlayer = myId;

        info.scoreWeek = weekInput.text == "" ? 0 : int.Parse(weekInput.text);
        info.scoreRecord = recordInput.text == "" ? 0 : int.Parse(recordInput.text);
        if (recordInput.text != "" && recordInput.text != "0")
        {
            info.timeScoreRecord = timeBdd;
        }
        if (weekInput.text != "" && weekInput.text != "0")
        {
            info.timeScoreWeek = timeBdd;
        }

        info.idsBoostUsed = itemBoostWeek1.getIdBoost() + "," + itemBoostWeek2.getIdBoost() + "," + itemBoostWeek3.getIdBoost();
        info.idsBoostUsed_record = itemBoostRecord1.getIdBoost() + "," + itemBoostRecord2.getIdBoost() + "," + itemBoostRecord3.getIdBoost();

        info.levelBoostUsed = itemBoostWeek1.getLevelBoost() + "," + itemBoostWeek2.getLevelBoost() + "," + itemBoostWeek3.getLevelBoost();
        info.levelBoostUsed_record = itemBoostRecord1.getLevelBoost() + "," + itemBoostRecord2.getLevelBoost() + "," + itemBoostRecord3.getLevelBoost();

        return info;
    }

    public void updateToStitchBddInfos(infoPlayerTest info)
    {
        nameInput.text = info.nomPlayer;
        namePlayer_ref = info.nomPlayer;
        myId = info.idPlayer;

        starInput.text = info.nbStar.ToString();
        nbStarPlayer_ref = info.nbStar.ToString();

        weekInput.text = info.scoreWeek.ToString();
        scoreSemaine_ref = info.scoreWeek.ToString();

        recordInput.text = info.scoreRecord.ToString();
        scoreRecord_ref = info.scoreRecord.ToString();

        string[] w_id = info.idsBoostUsed.Split(",");
        string[] w_lvl = info.levelBoostUsed.Split(",");
        itemBoostWeek1.loadBoostWithSavedInfo(w_id[0] + "," + w_lvl[0], true);
        itemBoostWeek2.loadBoostWithSavedInfo(w_id[1] + "," + w_lvl[1], true);
        itemBoostWeek3.loadBoostWithSavedInfo(w_id[2] + "," + w_lvl[2], true);

        string[] r_id = info.idsBoostUsed_record.Split(",");
        string[] r_lvl = info.levelBoostUsed_record.Split(",");
        itemBoostRecord1.loadBoostWithSavedInfo(r_id[0] + "," + r_lvl[0], true);
        itemBoostRecord2.loadBoostWithSavedInfo(r_id[1] + "," + r_lvl[1], true);
        itemBoostRecord3.loadBoostWithSavedInfo(r_id[2] + "," + r_lvl[2], true);

        // annuler update si était en mode waitUpdate
        if (nbUpdate > 0)
        {
            scrollBotManager.nbUpdateChanged(false);
            resetBotUpdate();
        }
    }

    public void loadCardWithSavedInfo(string info)
    {
        StartCoroutine(initCard());
        string[] tab = info.Split('|');
        nameInput.text = tab[0];
        namePlayer_ref = tab[0];
        starInput.text = tab[1];
        nbStarPlayer_ref = tab[1];
        weekInput.text = tab[2];
        scoreSemaine_ref = tab[2];
        recordInput.text = tab[3];
        scoreRecord_ref = tab[3];
        itemBoostWeek1.loadBoostWithSavedInfo(tab[4], false);
        itemBoostWeek2.loadBoostWithSavedInfo(tab[5], false);
        itemBoostWeek3.loadBoostWithSavedInfo(tab[6], false);
        itemBoostRecord1.loadBoostWithSavedInfo(tab[7], false);
        itemBoostRecord2.loadBoostWithSavedInfo(tab[8], false);
        itemBoostRecord3.loadBoostWithSavedInfo(tab[9], false);
        myId = tab[10];
    }

    private bool initialisationOn = false;
    private IEnumerator initCard()
    {
        initialisationOn = true;
        yield return new WaitForSeconds(0.5f);
        initialisationOn = false;
    }
}
