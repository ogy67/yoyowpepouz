using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeBooster {
    boosterArgent,
    boosterOr,
    boosterEpique,
    error
}

[System.Serializable]
public struct probaCard
{
    public typeBooster tB;
    public typeRarety[] tR;
    public int priceBooster;
    public recompense typeBuy;
    public float[] nbCards_perRarety;
    public int[] nbDiffCard_perRarety;
    public int nbCardInPack;
    public string nomBooster;
    public Color32 colorBooster;
}
public class openBoosterManager : MonoBehaviour
{

    public probaCard[] probas;

    private List<itemBoost> allTheBoost;

    private Dictionary<typeRarety, List<itemBoost>> dictBoost_byRarety
        = new Dictionary<typeRarety, List<itemBoost>>()
        {
            {typeRarety.common, new List<itemBoost>(){} },
            {typeRarety.rare, new List<itemBoost>(){} },
            {typeRarety.epic, new List<itemBoost>(){} },
            {typeRarety.legendary, new List<itemBoost>(){} }
        };

    private Dictionary<typeRarety, int> rangRarety = new Dictionary<typeRarety, int>()
    {
        {typeRarety.common, 0 },
        {typeRarety.rare, 1 },
        {typeRarety.epic, 2 },
        {typeRarety.legendary, 3 },
    };

    private Dictionary<int, float[]> partageCards = new Dictionary<int, float[]>()
    {
        {1, new float[]{1f} },
        {2, new float[]{2/6f, 4/6f } },
        {3, new float[]{ 1 / 6f, 1.5f /6f, 2.5f / 6f} },
    };


    private static openBoosterManager instance;
    public static openBoosterManager getInstance()
    {
        if (instance == null) instance = GameObject.Find("boosterOpenMenu").GetComponent<openBoosterManager>();
        return instance;
    }



    // /!\ SUPPRIMER , géré par menuBooster
    public void buyBooster(int rang, bool fromOfferMenu, typeBooster tB_offer)
    {
        if (fromOfferMenu)
        {
            openBooster(tB_offer);
            loadDesignBooster(rang);
        }
        else
        {
            typeBooster tB = rang == 0 ? typeBooster.boosterArgent : rang == 1 ? typeBooster.boosterOr : typeBooster.boosterEpique;
            // on ouvre le menuGetBoost en mode booster
            

            // SUPPRIMER
            if (validAchat(getInfoBooster(tB).priceBooster, getInfoBooster(tB).typeBuy))
            {
                openBooster(tB);
                loadDesignBooster(rang);
            }
        }
    }

    public void openBoosterFromDefi(typeBooster tB_offer)
    {
        openBooster(tB_offer);
        int rang = tB_offer == typeBooster.boosterArgent ? 0 : tB_offer == typeBooster.boosterOr ? 1 : 2; 
        loadDesignBooster(rang);
    }

    public void openBoosterFromPack(typeBooster tB_offer)
    {
        openBooster(tB_offer);
        int rang = tB_offer == typeBooster.boosterArgent ? 0 : tB_offer == typeBooster.boosterOr ? 1 : 2;
        loadDesignBooster(rang);
    }

    public bool validAchat(int montant, recompense typeBuy) // retourne true si l'achat a été validé
    {
        if(typeBuy == recompense.ARGENT
            && montant <= gameManager.monnaieVal)
        {
            StartCoroutine(gameManager.getInstance().debiteMonney(montant));
            return true;
        }
        else if(typeBuy == recompense.GEMME
            && gemmesManager.validAchat(montant))
        {
            return true;
        }
        return false;
    }

    public probaCard getInfoBooster(typeBooster tB)
    {
        foreach (probaCard pC in probas)
        {
            if (pC.tB == tB)
            {
                // ne pas generate booster à ouverture menu boost
                // ? OU DU COUP ?
                //List<int[]> booster = generateBooster(pC);
                return pC;
            }
        }
        return new probaCard();
    }

    [Header("Objet booster")]
    public Transform imgLogoBooster;
    public Image hautBooster_img, basBooster_img, flouColor;
    public Color argentColor, orColor, crystalColor;
    private void loadDesignBooster(int rang)
    {
        foreach (Transform t in imgLogoBooster)
            t.gameObject.SetActive(false);
        Color c = rang == 0 ? argentColor : rang == 1 ? orColor : crystalColor;
        imgLogoBooster.GetChild(rang).gameObject.SetActive(true);
        hautBooster_img.color = c;
        basBooster_img.color = c;
        flouColor.color = new Color(c.r, c.g, c.b, flouColor.color.a);
    }



    /*
 * int getRandomId_byRarety(typeRarety tR)
 * void generateBooster(probaCard pC)
 * List<itemBoost> setAllTheBoost()
*/
    #region generation de booster

    private List<itemBoost> setAllTheBoost()
    {
        if (allTheBoost == null)
        {
            allTheBoost = boostManager.allTheBoost;
            foreach (itemBoost i in allTheBoost)
            {
                dictBoost_byRarety[i.type].Add(i);
            }
        }
        return allTheBoost;
    }

    /*private int getRandomId_byRarety(typeRarety tR)
    {
        return dictBoost_byRarety[tR][Random.Range(0, dictBoost_byRarety[tR].Count)].idBoost;
    }*/

    public List<int[]> createContentBooster(typeBooster tB)
    {
        return generateBooster(getInfoBooster(tB));
    }
    private List<int[]> generateBooster(probaCard pC)
    {
        setAllTheBoost();
        List<int[]> idCardGived_nbCardGived = new List<int[]>();

        foreach (typeRarety tR in new typeRarety[] {
            typeRarety.common, typeRarety.rare, typeRarety.epic, typeRarety.legendary})
        {
            float totalCardToPop = pC.nbCards_perRarety[rangRarety[tR]];
            int nbCardDiff = pC.nbDiffCard_perRarety[rangRarety[tR]];


            // si totalCardToPop < 1f : on doit faire probablité
            if (totalCardToPop < 1f)
            {
                if (Random.Range(0, 1f) < totalCardToPop)
                {
                    //current_tB
                    //getRandomIDBoost_fromSeed(current_tB, tR);  

                    // proba passée, donner la carte
                    idCardGived_nbCardGived.Add(new int[] {
                        getRandomIDBoost_fromSeed(current_tB, tR)
                        //getRandomId_byRarety(tR)
                        , 1 });
                    // on arrête la boucle, exemple argent pack : ne pas donner 1 epic ET 1 legendaire
                    break;
                }
            }
            // sinon, on generate les cartes partagée entre nbCardDiff
            else
            {
                float[] cut = partageCards[nbCardDiff];
                int totGived = 0;
                List<int> used_id = new List<int>();

                for (int i = 0; i < nbCardDiff; i++)
                {
                    int id = getRandomIDBoost_fromSeed(current_tB, tR);
                    /*do
                    {
                        id = getRandomIDBoost_fromSeed(current_tB, tR);
                            //getRandomId_byRarety(tR);
                    } while (used_id.Contains(id));*/
                    used_id.Add(id);

                    int nb_gived;
                    if (i == nbCardDiff - 1)
                    {
                        nb_gived = (int)totalCardToPop - totGived;
                    }
                    else
                    {
                        nb_gived = (int)(cut[i] * totalCardToPop);
                        totGived += nb_gived;
                    }

                    idCardGived_nbCardGived.Add(
                        new int[] { id, nb_gived });
                }
            }
        }
        return idCardGived_nbCardGived;
    }

    #endregion



    private typeBooster current_tB;
    public void openBooster(typeBooster tB)
    {
        // SUPPRIMER Les APPELS à la FONCTION
        current_tB = tB;
        phase = -1;
        List<int[]> generated_booster = generateBooster(getInfoBooster(tB));
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.ouverturePacket);
        StartCoroutine(openBooster_anim(generated_booster));
    }

    [Header("Animation opening")]
    public int[] framePauseAnim_init; // les suivantes dépendent du nombre de carte différentes à donner
    public Animation boosterAnim;

    public Transform cardItem;
    public Transform cardAnimation;

    public Transform counterCard;
    public Text counterCard_text;
    public Image counterCard_imageBord;

    private int phase = -1;
    private int number_cardPhase; // si 2 cartes différentes : 2 cliques jusqu'à fermeture
    private static int number_initPhase = 4; // initialisation - grossit - dechirer, separer

    private bool animating = false;
    public static bool boosterMenu_open = false;
    /*
     * IEnumerator openBooster_anim(List<int[]> booster)
     * IEnumerator animCardCome(int id, int nbCard)
     * void btnNextPhaseOpenPack()
    */
    #region Animation opening
    private IEnumerator openBooster_anim(List<int[]> booster)
    {
        Debug.Log("Call open booster");
        boosterMenu_open = true;
        animating = true;
        int nbPhase = booster.Count + number_initPhase; // - 1 : end_init et init_boost se superposent
        int phaseBooster = 0;

        // on initialise
        phase++;
        boosterAnim["openBooster"].speed = 1;
        boosterAnim.Play("openBooster");
        GetComponent<Animation>().Play("openMenuBooster");
        yield return new WaitForSeconds(framePauseAnim_init[phase] / 60f);
        boosterAnim["openBooster"].speed = 0;

        // animation d'ouverture
        yield return new WaitForSeconds(40 / 60f);
        animating = false;

        int next;
        do
        {
            next = phase + 1;
            yield return new WaitUntil(predicate: () => phase == next);
            // quand phase est next, on continue les animation
            if (phase < number_initPhase)
            {
                //Debug.Log("<color=red>Phase " + phase + "</color>");
                //if (phase == 2)// dechirement
                //    sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.dechirement_packet);
                boosterAnim["openBooster"].speed = 1;
                yield return new WaitForSeconds((framePauseAnim_init[phase] - framePauseAnim_init[phase - 1]) / 60f);
                if(phase < number_initPhase - 1)
                    boosterAnim["openBooster"].speed = 0;
            }
            // si phase est la meme que la derniere de init, lancer en simultané
            if (phase >= number_initPhase - 1 && phase < nbPhase - 1)
            {
                // anim out de card
                if(phase > number_initPhase - 1){
                    cardAnimation.GetComponent<Animation>().Play("cardOut");
                    yield return new WaitForSeconds(10f / 60f);
                    //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);
                    if (cardAnimation.transform.childCount == 2)
                    {
                        cardAnimation.transform.GetChild(1).gameObject.SetActive(false);
                        cardAnimation.transform.GetChild(1).SetParent(reccapContainer_off);
                    }
                    yield return new WaitForSeconds(10f / 60f);
                }

                else if (phase == number_initPhase - 1)
                {
                    counterCard.GetComponent<Animation>().Play("counterCardsAppear");
                    //sfxManager.getInstance().playSound_menuP(typeSound_menuP.switch_level);
                }
                counterCard_text.text = (booster.Count - ( phaseBooster + 1)).ToString();

                StartCoroutine(animCardCome(booster[phaseBooster][0], booster[phaseBooster][1]));

                if (phaseBooster < booster.Count - 1)
                {
                    typeRarety tR = boostManager.getInstance().getRaretyFromId(booster[phaseBooster + 1][0]);
                    counterCard_imageBord.color = menuBooster.getInstance().getColorRarety(tR);
                        //menuInfoBoost.getInstance().getColorRarety(tR);                  
                }


                //Debug.Log("Donne carte id : " + booster[phaseBooster][0] + " , nb card : " + booster[phaseBooster][1]);
                phaseBooster++;
               
                // animation carte vient
            }
            // phase de reccap
            if(phase == nbPhase - 1)
            {
                cardAnimation.GetComponent<Animation>().Play("cardOut");
                yield return new WaitForSeconds(10f / 60f);
                {
                    cardAnimation.transform.GetChild(1).gameObject.SetActive(false);
                    cardAnimation.transform.GetChild(1).SetParent(reccapContainer_off);
                }

                StartCoroutine(reccapFin());
                counterCard.GetComponent<Animation>().Play("counterCardsDisappear");
                yield return new WaitUntil(predicate: () => reccapOn == false);
            }
            animating = false;
        } while (phase < nbPhase - 1);

        // on attend le clique pour fermer le menu
        next = phase + 1;
        yield return new WaitUntil(predicate: () => phase == next);

        GetComponent<Animation>().Play("closeMenuBooster");
        yield return new WaitForSeconds(10 / 60f);
        //menuInfoBoost.getInstance().reloadBtnBuy();
        reccapFin_close();
        boosterMenu_open = false;
    }

    private IEnumerator animCardCome(int id, int nbCard)
    {
        itemBoost iT = boostManager.getInstance().getBoostById(id);
        Transform t = Instantiate(iT.gameObject, cardAnimation).transform;
        t.GetComponent<itemBoost>().setModeOpenBooster();
        upReccap.Add(t);
        t.localScale = cardItem.localScale;
        t.position = cardItem.position;

        t.GetComponent<itemBoost>().loadItemBooster(nbCard);

        cardAnimation.GetComponent<Animation>().Play("cardCome");
        yield return new WaitForSeconds(15f / 60f);
    }
    public void btnNextPhaseOpenPack()
    {
        if (!animating)
        {
            //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.click_phase);
            animating = true;
            phase++;
        }
    }




    private List<Transform> upReccap = new List<Transform>();
    public Transform reccapContainer_off; // contient les img boost inactve, pour les reafficher après
    public Transform reccap_tr;
    private bool reccapOn = false;
    private Dictionary<int, int> indexReccap = new Dictionary<int, int>() {
        {2, 0 },
        {3, 1 },
        {4, 2 },
        {5, 3 },
        {6, 4 }
    };

    private void reccapFin_close()
    {
        //reccapOn = true;
        /*for (int i = 0; i < cur_reccap_tr.childCount; i++)
        {
            cur_reccap_tr.GetChild(i).GetComponent<Animation>().Play("reccapItemClose");
        //}
        //yield return new WaitForSeconds(11 / 60f);*/
        for (int i = 0; i < cur_reccap_tr.childCount; i++)
        {
            Destroy(cur_reccap_tr.GetChild(i).GetChild(0).gameObject);          
        }
        cur_reccap_tr.gameObject.SetActive(false);
        upReccap = new List<Transform>();
        //reccapOn = false;
    }

    private Transform cur_reccap_tr;
    private IEnumerator reccapFin()
    {
        reccapOn = true;
        Transform list_reccap = reccap_tr.GetChild(indexReccap[upReccap.Count]);
        list_reccap.gameObject.SetActive(true);
        cur_reccap_tr = list_reccap;
        for(int i = 0; i < upReccap.Count; i++)
        {
            Transform t_img = upReccap[i];
            Transform toShow = t_img.GetComponent<itemBoost>().imageBoost;
            Destroy(t_img.gameObject);

            Transform t_ref = list_reccap.transform.GetChild(i);

            toShow.SetParent(t_ref.parent);
            toShow.transform.position = t_ref.transform.position;
            //float maxi = t_ref.localScale.x > t_ref.localScale.y ? t_ref.localScale.x : t_ref.localScale.y;
            toShow.transform.localScale = t_ref.localScale;
            //Debug.Log(new Vector3(maxi, maxi, maxi));
            toShow.SetParent(t_ref);
            toShow.gameObject.SetActive(true);
            toShow.GetComponent<Image>().enabled = true;
            t_ref.GetComponent<Animation>().Play("reccapItemOpen");
            yield return new WaitForSeconds(5/60f);
        }
        reccapOn = false;

        // DETRUIRE TT LES OBJ A LA FIN DE L'nOUVERTURE

        yield return null;
    }
    #endregion










    private Dictionary<typeBooster, List<List<int>>> boosterSeeds = new Dictionary<typeBooster, List<List<int>>>()
    {
        {typeBooster.boosterArgent, new List<List<int>>()},
        {typeBooster.boosterOr, new List<List<int>>()},
        {typeBooster.boosterEpique, new List<List<int>>()},
    };
    private int getRandomIDBoost_fromSeed(typeBooster tB, typeRarety tR)
    {
        int rangSeed = tR == typeRarety.common ? 0 :
            tR == typeRarety.rare ? 1 :
            tR == typeRarety.epic ? 2 : 3;
        List<int> curSeed = boosterSeeds[tB][rangSeed];
        //Debug.Log("<color=yellow>Get number from seed : </color>");
        /*foreach (int u in curSeed)
            Debug.Log(" --- " + u);*/
        int index = curSeed[0];

        if(curSeed.Count == 1)
            curSeed = generateNewSeed(index);
        else
            curSeed.RemoveAt(0);
        boosterSeeds[tB][rangSeed] = curSeed;

        int idBoostAssocie = dictBoost_byRarety[tR][
            //Random.Range(0, dictBoost_byRarety[tR].Count)
            index - 1
            ].idBoost;

        return idBoostAssocie;
    }
    private void Start()
    {
        initSeed();
    }
    private void initSeed()
    {
        randomModificator = Random.Range(0, 9999) % 3;
        foreach (typeBooster tB in new List<typeBooster>()
        { typeBooster.boosterArgent, typeBooster.boosterOr, typeBooster.boosterEpique })
        {
            //Debug.Log("<color=blue>Generate seed from boost</color>" + tB);
            for(int i = 0; i < 4; i++) // 4 rarete differentes
            {
                boosterSeeds[tB].Add(generateNewSeed(-1));
            }
        }
    }

    /*private void Update()
    {
        if (generateNewSeed_test)
        {
            generateNewSeed_test = false;
            generateNewSeed();
        }
        if (showFrequency_test)
        {
            showFrequency_test = false;
            showFrequencyItems();
        }

        if (genNoModif_test)
        {
            test_genNoModif(nbGen);
            genNoModif_test = false;
        }
        if (genModif_test)
        {
            test_genModif(nbGen);
            genModif_test = false;
        }
    }*/
    //public bool generateNewSeed_test = false;
    //public bool showFrequency_test = false;

    //public int nbGen = 30;
    //public bool genNoModif_test = false;
    //public bool genModif_test = false;

    /*private void test_genNoModif(int nbGen)
    {
        parutionItem = new List<int> { 0, 0, 0, 0 };//
        for(int i = 0; i < nbGen; i++)
        {
            generateNewSeed();
        }
        showFrequencyItems();
    }*/
    /*private void test_genModif(int nbGen)
    {
        parutionItem = new List<int> { 0, 0, 0, 0 };//
        for (int i = 0; i < nbGen; i++)
        {
            generateNewSeed(-1);
        }
        showFrequencyItems();
    }*/
    /*private void showFrequencyItems()
    {
        Debug.Log("Frequency on " + parutionItem[0] + " generation");
        Debug.Log("- item 1, nb "+ parutionItem[1]+" , freq " + parutionItem[1] / (float)parutionItem[0]);
        Debug.Log("- item 2, nb " + parutionItem[2] + " , freq " + parutionItem[2] / (float)parutionItem[0]);
        Debug.Log("- item 3, nb " + parutionItem[3] + " , freq " + parutionItem[3] / (float)parutionItem[0]);
    }*/


    private int modifRandom(int random)
    {
        // si rand (après modif) > 3 , appliquer le modulo
        int rand2 = random + randomModificator;
        if (rand2 > 3)
            return rand2 % 3;
        else
            return rand2;
    }

    private int randomModificator = 0;
    // 0 : nombre generation
    // 1 : frequence item 1
    // 2 : frequence item 2
    // ...
    private List<int> parutionItem = new List<int> { 0, 0, 0, 0 };//
    public List<int> generateNewSeed(int indexFirst_lock)
    {
        //Debug.Log("Seed generated : ");
        List<int> retour = new List<int>();
        List<int> notUsed = new List<int>() { 1, 2, 3 };

        int rangItem = modifRandom(Random.Range(1, 3));
        if(indexFirst_lock != -1)
        {
            while(rangItem == indexFirst_lock)
                rangItem = modifRandom(Random.Range(1, 3));
        }

        retour.Add(rangItem);
        //Debug.Log("add : " + rangItem);
        parutionItem[0]++;
        parutionItem[rangItem]++;
        while (retour.Count != 5)
        {
            if(retour.Count == 4 && notUsed.Count == 1)
            {
                retour.Add(notUsed[0]);
                //Debug.Log("add : "+notUsed[0]);
                parutionItem[0]++;
                parutionItem[notUsed[0]]++;
            }
            else
            {
                rangItem = modifRandom(Random.Range(1, 3));
                if(retour[retour.Count - 1] != rangItem)
                {
                    retour.Add(rangItem);
                    //Debug.Log("add : " + rangItem);
                    parutionItem[0]++;
                    parutionItem[rangItem]++;
                    if (notUsed.Contains(rangItem))
                        notUsed.Remove(rangItem);
                }
            }
        }

        // randomModificator va de 0 à 2 {0, 1, 2}
        randomModificator = (randomModificator + Random.Range(0, 9999)) % 3;
        //randomModificator = (randomModificator + 1) % 3; // % nombre items differents

        return retour;
    }
}
