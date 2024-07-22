using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum typeBoostGame
{
    bouclier,
    fusee,
    super_viseur,
    pierre_tombale,
    tirelire,
    souris,
    ghost,
    ancre,
    ressort,
    medic,
    switch_rotation,
    aimant,
    bombe,
    aigle,
    tortue,
    canon,
    none
}

[System.Serializable]
public struct infoBoostGame
{
    public typeBoostGame type_boost;
    public GameObject boost_object;
}

public class itemBoostGame : MonoBehaviour
{
    public infoBoostGame[] infos_boost;
    public Image progBarBoost;

    public itemBoost currentBoost;
    private infoBoostGame currentInfo;

    public GameObject lockBoost_expert;


    private Dictionary<typeRarety, int> indexColor_byRarety = new Dictionary<typeRarety, int>()
    {
        {typeRarety.common, 0 },
        {typeRarety.rare, 1 },
        {typeRarety.epic, 2 },
        {typeRarety.legendary, 3 },
    };

    private List<typeBoostGame> boost_alwaysOn_list = new List<typeBoostGame>()
    {
        typeBoostGame.aimant,
        typeBoostGame.ancre,
        typeBoostGame.medic,
        typeBoostGame.tirelire,
        typeBoostGame.souris,
        typeBoostGame.fusee,
    };

    private List<typeBoostGame> boost_with_counter = new List<typeBoostGame>()
    {
        typeBoostGame.medic,
        typeBoostGame.super_viseur,
        typeBoostGame.bouclier,
        typeBoostGame.bombe,
    };

    private List<typeBoostGame> boost_notLocked_expert = new List<typeBoostGame>()
    {
        //typeBoostGame.aimant,
        //typeBoostGame.tirelire,
    };

    private List<typeBoostGame> boost_no_counter_no_regen = new List<typeBoostGame>()
    {
        typeBoostGame.aimant,
        typeBoostGame.tirelire,
        typeBoostGame.souris,
        typeBoostGame.fusee,
    };

    public static bool open = false;
    private void OnEnable()
    {
        // game start
        if(currentBoost != null)
            loadVoyant();
        open = true;
        
        boostIngameManager.getInstance().initValues();
    }

    public void playerReplay()
    {
        // game start
        if (currentBoost != null)
            loadVoyant();
        open = true;

        boostIngameManager.getInstance().initValues();
    }

    private void OnDisable()
    {
        // game end
        if (currentBoost != null)
            cleanVoyant();
        open = false;
    }


    public void loadBoost(itemBoost toLoad)
    {
        //Debug.Log(toLoad == null ? "Load empty boost" : "Voyant ingame load boost : " + toLoad.typeBoost_name);
        currentBoost = toLoad;
        Color c = bordVoyant.color;
        bordVoyant.color = new Color(c.r, c.g, c.b, toLoad == null ? 0.5f : 1f);

        if (currentBoost != null)
        {
            currentInfo = getInfo_byBoostType(toLoad.typeBoost_name);
        }
    }

    private infoBoostGame getInfo_byBoostType(typeBoostGame type)
    {
        foreach(infoBoostGame i in infos_boost)
        {
            if (i.type_boost == type)
                return i;
        }
        return new infoBoostGame();
    }



    private void cleanVoyant()
    {
        bordVoyant.color = Color.white;
        Color c = bordVoyant.color;
        bordVoyant.color = new Color(c.r, c.g, c.b, currentBoost == null ? 0.5f : 1f);

        if (currentBoost != null)
            currentInfo.boost_object.SetActive(false);
        useBoostEvent = false;
        regenIsPaused = true;
        waitingToUseBoost = false;
        compteurGo.SetActive(false);
        nbRestant_text.text = "";
        //Debug.Log("Prog set OFF clean");
        progBarBoost.fillAmount = 0f;
    }

    public Color[] colorRarety;
    public Color[] colorRarety_off;
    public Image bordVoyant;
    public GameObject compteurGo;
    public Text nbRestant_text;
    public Animation boostRegenEndAnim;
    private void loadVoyant()
    {
        //Debug.Log("Init game, load voyant, type boost : " + currentBoost.typeBoost_name);
        currentInfo.boost_object.SetActive(true);
        Color c = colorRarety[indexColor_byRarety[currentBoost.type]];
        bordVoyant.color = c;
        currentInfo.boost_object.GetComponent<Image>().color = c;

        //Debug.Log("<color=red>Reset anim prog</color>");
        //. // pb, reset prog tirelire après que set ( anim lancé end frame )
        // ne fonctionne pas, si boost ressort avant, pouvait être prit, alors doit reset
        //.
        if (currentBoost.type != typeRarety.legendary)
            //&& currentBoost.typeBoost_name != typeBoostGame.tirelire)
            boostRegenEndAnim.Play("boostRegenReset");

        //Debug.Log("Prog set OFF load");
        progBarBoost.fillAmount = 0f;
        nbBoostRestant = -1;

        bool boostIsLocked = !infinisManager.infinis_Mode_On
            && expertMode.modeExpertOn
            && !boost_notLocked_expert.Contains(currentBoost.typeBoost_name);
        lockBoost_expert.gameObject.SetActive(boostIsLocked);

        // medic - bouclier - superViseur - fusee
        if (boost_with_counter.Contains(currentBoost.typeBoost_name)
            && !boostIsLocked)
        {
            nbBoostRestant = currentBoost.getNbBoost();
            nbRestant_text.text = nbBoostRestant.ToString();
            compteurGo.SetActive(true);
            compteurGo.GetComponent<Image>().color = c;
        }
        else
        {
            // ancre
            if (boost_alwaysOn_list.Contains(currentBoost.typeBoost_name))
            {
                // tirelire - aimant - souris
                if (boost_no_counter_no_regen.Contains(currentBoost.typeBoost_name))
                {
                    if (currentBoost.typeBoost_name == typeBoostGame.tirelire)
                    {
                        loadTirelire();
                        bordVoyant.color = colorRarety_off[indexColor_byRarety[currentBoost.type]];
                        progBarBoost.color = new Color(c.r, c.g, c.b, progBarBoost.color.a);
                    }
                }
            }
            // pierre tombale - ghost - switch - ressort
            else // boost regen
            {
                bordVoyant.color = colorRarety_off[indexColor_byRarety[currentBoost.type]];
                currentInfo.boost_object.GetComponent<Image>().color = colorRarety_off[indexColor_byRarety[currentBoost.type]];
                if (!boostIsLocked)
                {
                    progBarBoost.color = new Color(c.r, c.g, c.b, progBarBoost.color.a);
                    StartCoroutine(loopBoost_regen());
                }
            }
        }
    }

    private IEnumerator loopBoost_regen()
    {
        float time_regen = currentBoost.getTimeRegen();
        float current_time = 0f;
        //Debug.Log("Prog set OFF loop regen");
        progBarBoost.fillAmount = 0f;
        Color c = colorRarety[indexColor_byRarety[currentBoost.type]];
        Color c_off = colorRarety_off[indexColor_byRarety[currentBoost.type]];

        yield return new WaitForSeconds(0.1f);
        // on active directement boost
        bool forceActivateInit = true;

        while (true)
        {
            if (!forceActivateInit)
            {
                yield return null;

                if (regenIsPaused)
                    yield return new WaitUntil(predicate: () => regenIsPaused == false);

                current_time += Time.deltaTime;
                //Debug.Log("Load progBar regen");
                progBarBoost.fillAmount = current_time / time_regen;
            }

            if(forceActivateInit
                || current_time >= time_regen)
            {
                progBarBoost.fillAmount = 1f;
                forceActivateInit = false;

                //Debug.Log(gameObject.name);
                boostRegenEndAnim.Play("boostRegenFinished");
                bordVoyant.color = c;
                //progBarBoost.fillAmount = 0f;
                currentInfo.boost_object.GetComponent<Image>().color = c;

                // on attend qu'il l'utilise
                waitingToUseBoost = true;

                // si on utilise ghost, on active le detecteur de ghost
                if(currentInfo.type_boost == typeBoostGame.ghost)
                {
                    GhostBoostManager.getInstance().ghostCanBeUsed();
                }

                yield return new WaitUntil(predicate: () => useBoostEvent == true);

                //Debug.Log("<color=red>Reset anim prog</color>");
                boostRegenEndAnim.Play("boostRegenReset");
                waitingToUseBoost = false;
                useBoostEvent = false;
                // on relance le timer
                current_time = 0f;
                bordVoyant.color = c_off;
                compteurGo.GetComponent<Image>().color = c_off;
            }
        }
    }

    private bool waitingToUseBoost = false;
    private bool useBoostEvent = false;

    // par défaut player est sur initCheckp en début de partie
    public static bool regenIsPaused = true;



    public typeBoostGame getBoostType()
    {
        return currentBoost != null ? currentBoost.typeBoost_name : typeBoostGame.none;
    }

    public int nbBoostRestant;
    public bool canUseBoost()
    {
        if(nbBoostRestant != -1)
        {
            return nbBoostRestant != 0;
        }
        else
        {
            return waitingToUseBoost; // en attente que player utilise le boost
        }
    }

    public bool boostEmpty()
    {
        return currentBoost == null;
    }

    public void playerUseBoost()
    {
        // si superBoost OU medic : désincrémenter
        if (currentBoost.isSuperPower
            || currentInfo.type_boost == typeBoostGame.medic)
        {
            nbBoostRestant -= 1;
            nbRestant_text.text = nbBoostRestant.ToString();
            // si pas de boost restant : desactiver voyant
            if(nbBoostRestant == 0)
            {
                Color c_off = colorRarety_off[indexColor_byRarety[currentBoost.type]];
                bordVoyant.color = c_off;
                currentInfo.boost_object.GetComponent<Image>().color = c_off;
            }
        }
        // si time regen : relancer la loop regen
        else
        {
            useBoostEvent = true;
        }
    }

    public void playerGetASuperPowerBoost()
    {
        nbBoostRestant += 1;
        nbRestant_text.text = nbBoostRestant.ToString();
        // si était désactivé, réactiver
        if (nbBoostRestant == 1) // est passé de 0 à 1
        {
            Color c_on = colorRarety[indexColor_byRarety[currentBoost.type]];
            bordVoyant.color = c_on;
            currentInfo.boost_object.GetComponent<Image>().color = c_on;
        }

        boostRegenEndAnim.Play("compteurBoostSPAdd");
    }

    public float getTimeEffectBoost()
    {
        return currentBoost.getTimeEffect();
    }

    public float getSizeAimant()
    {
        return currentBoost.getUpgradeValue()[0][0];
    }

    public float getSizeAncre()
    {
        return currentBoost.getUpgradeValue()[0][0];
    }

    public float getSizeSouris()
    {
        return currentBoost.getUpgradeValue()[0][0];
    }

    public float getTimePierreTombale()
    {
        return currentBoost.getUpgradeValue()[0][0];
    }



    public static int nbBounceTirelire = 0;
    private int nbBounceToFillTirelire;
    private int nbYow_tirelire;
    private void loadTirelire() // appelé quand partie lancée
    {
        StartCoroutine(setProgTirelireLate());
    }

    // attendre que reset progBar avec anim
    private IEnumerator setProgTirelireLate()
    {
        yield return new WaitForSeconds(0.5f);

        int[] info = currentBoost.getTirelireInfos();
        nbBounceToFillTirelire = info[0];
        nbYow_tirelire = info[1];

        //Debug.Log("Load tirelire init game, nbbounce : " + nbBounceTirelire + " , fill amount : " + ((nbBounceTirelire % nbBounceToFillTirelire) / (float)nbBounceToFillTirelire));
        progBarBoost.fillAmount = (nbBounceTirelire % nbBounceToFillTirelire) / (float)nbBounceToFillTirelire;
        //Debug.Log("Fill : " + progBarBoost.fillAmount);
        //StartCoroutine(checkBarProg(progBarBoost.fillAmount));
    }

    private int indexRef = 0;
    private IEnumerator checkBarProg(float forceProg)
    {
        indexRef++;
        int copyIndex = indexRef;
        while (copyIndex == indexRef)
        {
            //Debug.Log("Fill (loop:"+Time.time+") : " + progBarBoost.fillAmount);
            progBarBoost.fillAmount = forceProg;
            yield return new WaitForSeconds(1f);

        }
    }

    public bool incrementeTirelire()
    {
        // PB : init game, voyant tirelire prog est à 0 ( et pas a prog actuelle de tirelire )
        
        bool receiveCoins = false;
        nbBounceTirelire++;
        //Debug.Log("Player bounce, limit : " + nbBounceToFillTirelire + ", currentNb : " + nbBounceTirelire);
        if (nbBounceTirelire % nbBounceToFillTirelire == 0)
        {
            giveYowTirelire();
            receiveCoins = true;
        }
        //Debug.Log("Incremente tirelire");
        progBarBoost.fillAmount = (nbBounceTirelire % nbBounceToFillTirelire) / (float)nbBounceToFillTirelire;
        return receiveCoins;
    }

    public GameObject monnaieGo;
    private void giveYowTirelire()
    {
        List<typeReward_game> rewards = new List<typeReward_game>();
        for (int i = 0; i < nbYow_tirelire; i++)
        {
            rewards.Add(typeReward_game.yow);
        }
        //popYowGemManager.getInstance().openCompteurYow_inGame();
        popYowGemManager.getInstance().popRec_game(
            rewards, getPlayerTransform_pos());
    }

    private Transform player_tr;
    private Vector3 getPlayerTransform_pos()
    {
        if (player_tr == null)
            player_tr = boostIngameManager.getInstance().playerMov.transform;
        return Camera.main.WorldToScreenPoint(player_tr.position);
    }
}
