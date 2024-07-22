using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boostManager : MonoBehaviour
{
    public Transform initContent;
    public Transform lockContent, unlockContent;
    //public RectTransform mainContent;

    public voyantBoost boost_SP;
    public voyantBoost boost_1;
    public voyantBoost boost_2;

    public static List<itemBoost> allTheBoost = new List<itemBoost>();
    private static boostManager instance;
    private List<object[]> data;

    // mit à true quand boost unlocked et menuBoost pas activé
    // daily offer booster ( QUE SI NEW )
    // daily offer boost ( QUE SI NEW )
    // defi manager boost 
    public static bool shouldBeRebuildWhenOpen = true; // par défaut , init deplace boost

    public static boostManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<boostManager>();
        return instance;
    }

    /*
     * typeRarety getRaretyBoost(int id)
     * typeRarety getRaretyFromId(int id)
     * itemBoost getBoostById(int id)
    */
    #region fonction pour autres programmes

    public bool boostIsUnlocked_byType(typeBoostGame tBG)
    {
        foreach(itemBoost iB in allTheBoost)
        {
            if(iB.typeBoost_name == tBG)
            {
                return iB.unlocked;
            }
        }
        return false;
    }
    private typeRarety getRaretyBoost(int id)
    {
        foreach (Transform t in initContent)
        {
            if (t.GetComponent<itemBoost>().idBoost == id)
                return t.GetComponent<itemBoost>().type;
        }
        return typeRarety.none;
    }

    public typeRarety getRaretyFromId(int id)
    {
        foreach (itemBoost iT in initContent)
        {
            if (iT.idBoost == id)
                return iT.type;
        }
        return typeRarety.none;
    }

    public itemBoost getBoostById_initContent(int id)
    {
        foreach (Transform t in initContent)
        {
            itemBoost iT = t.GetComponent<itemBoost>();
            if (iT.idBoost == id)
                return iT;
        }
        return null;
    }

    public itemBoost getBoostById(int id)
    {
        foreach (itemBoost iT in allTheBoost)
        {
            if (iT.idBoost == id)
                return iT;
        }
        return null;
    }

    public itemBoost getBoostSwitch(bool next, int id)
    {
        // on regarde quel est le boost suivant ou précédent
        int index = 0;
        foreach(Transform t in new Transform[] { unlockContent, lockContent })
        {
            foreach (Transform t_2 in t)
            {
                if (t_2.GetComponent<itemBoost>().idBoost == id)
                {
                    int max = unlockContent.childCount + lockContent.childCount;
                    int lookFor = index + (next ? 1 : -1);

                    // si lookFor depasse les borne, on lui donne les bonnes valeures
                    lookFor = lookFor == -1 ?
                        max - 1 : lookFor == max ?
                        0 : lookFor;
                    bool checkFirstList = lookFor < unlockContent.childCount;
                    // on donne a lookFor sa 'vraie' valeure en fonction de la liste ou il est
                    lookFor = lookFor >= unlockContent.childCount ?
                        lookFor - unlockContent.childCount : lookFor;
                    return (checkFirstList ? unlockContent : lockContent).GetChild(lookFor).GetComponent<itemBoost>();
                }
                index++;
            }
        }
        return new itemBoost();
    }

    public int getRandomBoost_byRarety(typeRarety tR)
    {
        List<int> ids = new List<int>();
        foreach (itemBoost iB in allTheBoost)
            if (iB.type == tR)
                ids.Add(iB.idBoost);
        return ids[Random.Range(0, ids.Count)];
    }
    #endregion

    /*
     * void initBoostData()
     * object[] getItemValueById(int id)
     * void sortBoostByRarety()
     * bool checkIdIsNew(int id)
    */
    #region initialisation des valeure
    public void initBoostData()
    {
        initInfoUsed(); // a faire avant que soient deplacés de la premiere liste
        // data : idBoost , isUnlocked , currentLevel , currentNbCards
        data = (List<object[]>)SaveSystem.loadData(typeSave.SHOP_BOOST);
        sortBoostByRarety_2(); // les deplace de la liste initContent à lockContent


        int i = 0;
        foreach (KeyValuePair<voyantBoost, typeRarety> e in emplacementUsed)
        {
            int id = e.Key.getCurrentId();
            boostGame_voyants[i].loadBoost(id == -1 ? new itemBoost() : getBoostById(id));
            if(id!=-1)
                boostGame_voyants[i].currentBoost.boostChangeSelectionState(true);
            i++;
        }

        // si liste vide, enregistrer data vide
        if (data.Count == 0)
        {
            foreach(Transform t in lockContent)
            {
                data.Add(new object[] { t.GetComponent<itemBoost>().idBoost, false, 1, 0 });
            }
            SaveSystem.Save(typeSave.SHOP_BOOST, data);
        }
        // si nouvelles data, les ajouter
        else if(data.Count != lockContent.childCount)
        {
            foreach(Transform t in lockContent)
            {
                if (!checkIdIsNew(t.GetComponent<itemBoost>().idBoost))
                    data.Add(new object[] { t.GetComponent<itemBoost>().idBoost, false, 1, 0 });
            }
            SaveSystem.Save(typeSave.SHOP_BOOST, data);
        }

        int nbUnlock = 0;
        // on donne a chaque boost ses valeures initiales
        foreach(Transform t in lockContent)
        {
            object[] d = getItemValueById(t.GetComponent<itemBoost>().idBoost);

            if ((bool)d[1]) // unlock
            {
                t.GetComponent<itemBoost>().setInitValues(true, (int)d[2], (int)d[3]);
                StartCoroutine(setParentLate(t, unlockContent));
                nbUnlock++;
            }
            else
            {
                t.GetComponent<itemBoost>().setInitValues(false, 1, 0);
            }
        }

        nbBoostUnlocked = nbUnlock;
        updateCounterBoosteUnlocked();

        //Debug.Log("<color=blue>OFFRE DAILYE CAN BE LOADED</color>");
        // on dit à offre quotidienne que load finit
        offreQuotidienne.getInstance().canBeLoaded();
    }

    private IEnumerator setParentLate(Transform t, Transform parent)
    {
        //Debug.Log("Set parent late");
        yield return new WaitForSeconds(0.1f);
        t.SetParent(parent);
        //Debug.Log("<color=red>A été deplacé, nouveau parent : </color>" + t.transform.parent);
    }

    private object[] getItemValueById(int id)
    {
        foreach(object[] o in data)
        {
            if ((int)o[0] == id)
                return o;
        }
        return new object[] { };
    }

    private void sortBoostByRarety_2()
    {
        while (initContent.childCount > 0)
        {
            Transform t = initContent.GetChild(0);
            allTheBoost.Add(t.GetComponent<itemBoost>());
            t.SetParent(lockContent);
        }
    }
    private void sortBoostByRarety()
    {
        Dictionary<typeRarety, int> indexInsertion = new Dictionary<typeRarety, int>()
        {
            {typeRarety.legendary, 0 },
            {typeRarety.epic, 0 },
            {typeRarety.rare, 0 },
            {typeRarety.common, 0 },
        };
        Dictionary<typeRarety, int> poidBoost = new Dictionary<typeRarety, int>()
        {
            {typeRarety.legendary, 1 },
            {typeRarety.epic, 2 },
            {typeRarety.rare, 3 },
            {typeRarety.common, 4 },
        };

        while (initContent.childCount > 0)
        {
            Transform t = initContent.GetChild(0);

            typeRarety tR = t.GetComponent<itemBoost>().type;
            if (poidBoost[tR] < 4)
                indexInsertion[typeRarety.common]++;
            if (poidBoost[tR] < 3)
                indexInsertion[typeRarety.rare]++;
            if (poidBoost[tR] < 2)
                indexInsertion[typeRarety.epic]++;

            t.SetParent(lockContent);
            t.SetSiblingIndex(indexInsertion[tR]);

            if (poidBoost[tR] == 1)
                indexInsertion[typeRarety.legendary]++;
            if(poidBoost[tR] == 4)
                indexInsertion[typeRarety.common]++;
        }
        //rebuildLayout();
    }

    public RectTransform toRebuild_boostMenu;
    public void rebuildLayout()
    {
        //Debug.Log("<color=green>Boost menu is rebuilt</color>");
        LayoutRebuilder.ForceRebuildLayoutImmediate(toRebuild_boostMenu);

        // ne fonctionne pas si menu off ( booster defi , booster daily offer , boost daily offer )
        //LayoutRebuilder.ForceRebuildLayoutImmediate(lockContent.GetComponent<RectTransform>());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(unlockContent.GetComponent<RectTransform>());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(lockContent);
    }
    // ouverture menu shop
    public void reloadVoyantsBoost()
    {
        foreach (voyantBoost t in new voyantBoost[] { boost_SP, boost_1, boost_2 })
        {
            t.openShopCleanAnimVoyant();
        }
    }

    private bool checkIdIsNew(int id)
    {
        foreach(object[] o in data)
        {
            if ((int)o[0] == id)
                return true;
        }
        return false;
    }

    #endregion // check que les unlock


    private Dictionary<voyantBoost, typeRarety> emplacementUsed;
    public itemBoostGame[] boostGame_voyants;
    /*
     * void initInfoUsed()
     * void btnUseItem(Transform caller)
     * void saveBoostIds()
    */
    #region Use boost
    // transform - typeUsed
    private void initInfoUsed()
    {
        List<int> ids = (List<int>)SaveSystem.loadData(typeSave.SHOP_BOOST_SELECTED);
        int i = 0;
        if (ids.Count == 0)
            ids = new List<int>() { 0, 0, 0 };

        emplacementUsed = new Dictionary<voyantBoost, typeRarety>();
        foreach (voyantBoost t in new voyantBoost[] { boost_SP, boost_1, boost_2 })
        {
            //Debug.Log("Init game , initInfoUsed , id boost save : " + ids[i]);
            if(ids[i] >0) // ids commencent à 1, 0 : valeure vide
            {
                t.setVoyant(ids[i], false, getBoostById_initContent(ids[i]));
                emplacementUsed.Add(t, getRaretyBoost(ids[i]));
                //Debug.Log("Boost set, test exists : "+t.currentItemBoost.idBoost);
            }
            else
            {
                emplacementUsed.Add(t, typeRarety.none);
            }
            i++;
        }
    }


    public void boostIsDeleted(voyantBoost vB)
    {
        disableGlowItemSelected(vB);
    }
    private void disableGlowItemSelected(voyantBoost vB)
    {
        getBoostById(vB.getCurrentId()).boostChangeSelectionState(false);
    }

    public void btnUseItem(Transform caller)
    {
        itemBoost iB = caller.GetComponent<itemBoost>();
        
        // est déja en train d'utiliser le boost
        if(new List<int>() {
            boost_SP.getCurrentId(),
            boost_1.getCurrentId(),
            boost_2.getCurrentId() }.Contains(iB.idBoost))
        {
            return;
        }

        int rangSelectedItem = 0;

        if (iB.type == typeRarety.legendary)
        {
            if (boostGame_voyants[0].currentBoost != null)
                disableGlowItemSelected(boost_SP);
            boost_SP.setVoyant(iB.idBoost, true, iB);
            rangSelectedItem = 0;

        }
        else
        {
            bool set = false;
            foreach (voyantBoost t in new voyantBoost[] { boost_1, boost_2 })
            {
                // on s'en fous de la rareté
                // si a la meme rareté que un boost , le remplacer
                if (emplacementUsed[t] == iB.type)
                {
                    disableGlowItemSelected(t);
                    t.setVoyant(iB.idBoost, true, iB);
                    set = true;
                    rangSelectedItem = t == boost_1 ? 1 : 2;
                    break;
                }
            }
            if (!set)
            {
                // si un boost a sur la place, decaler

                // 
                //boost_1.
                if (emplacementUsed[boost_1] != typeRarety.none)
                {
                    // PB , boost_1 et _2 n'ont pas .currentItemBoost set au lancement jeu
                    Debug.Log("Decale voyant boost 1 sur voyant 2, id boost_1 : "+boost_1.getCurrentId()+" , type boost_1 : "+boost_1.currentItemBoost.idBoost);
                    if (emplacementUsed[boost_2] != typeRarety.none)
                        disableGlowItemSelected(boost_2);

                    boost_2.setVoyant(boost_1.getCurrentId(), false, boost_1.currentItemBoost);
                    boostGame_voyants[2].loadBoost(boost_1.currentItemBoost);// getBoostById(boost_1.getCurrentId()));
                    emplacementUsed[boost_2] = emplacementUsed[boost_1];
                }
                boost_1.setVoyant(iB.idBoost, true, iB);
                //boostGame_voyants[1].loadBoost(iB);
                rangSelectedItem = 1;
                emplacementUsed[boost_1] = iB.type;
            }
        }
        boostGame_voyants[rangSelectedItem].loadBoost(iB);
        saveBoostIds();
    }

    public void btnDeleteItem(voyantBoost vB)
    {
        // on reload les itemBoostGame
        int rang = vB == boost_SP ? 0 : vB == boost_1 ? 1 : 2;
        boostGame_voyants[rang].loadBoost(null);
        emplacementUsed.Remove(vB);

        if(rang != 0)
            emplacementUsed[vB] = typeRarety.none;

        saveBoostIds();
    }

    public void saveBoostIds()
    {
        SaveSystem.Save(typeSave.SHOP_BOOST_SELECTED, getBoostsUsedId());
    }

    public List<int> getBoostsUsedId()
    {
        return new List<int>() { boost_SP.getCurrentId(), boost_1.getCurrentId(), boost_2.getCurrentId() };
    }

    public List<int> getBoostUsedLevel()
    {
        int lvl1 = boost_SP.getCurrentId() == -1 ? -1 : getBoostById(boost_SP.getCurrentId()).niveauBoost;
        return new List<int>()
        {
             boost_SP.getCurrentId() == -1 ? -1 : getBoostById(boost_SP.getCurrentId()).niveauBoost,
             boost_1.getCurrentId() == -1 ? -1 : getBoostById(boost_1.getCurrentId()).niveauBoost,
             boost_2.getCurrentId() == -1 ? -1 : getBoostById(boost_2.getCurrentId()).niveauBoost
        };
    }
    #endregion


    public GameObject boostPan;
    public Text nbBoostUnlockedText;
    private int nbBoostUnlocked = 0;
    /*
     * void boostIsUnlocked(int id)
     * void updateBoostSavedValues(int id, int nbCard, int niveau)
     * void updateCounterBoosteUnlocked()
    */
    #region Reccup boost
    public void boostIsUnlocked(int id)
    {
        Debug.Log("Boost is unlocked");
        Transform t = null;
        foreach (Transform t_2 in lockContent)
        {
            if (t_2.GetComponent<itemBoost>().idBoost == id)
            {
                t = t_2;
                break;
            }
        }
        // on cherche l'index qu'il aura dans les own
        // les index sont triés par ordre croissant
        int indexInsertion = 0;
        while (unlockContent.childCount > indexInsertion 
            && unlockContent.GetChild(indexInsertion).GetComponent<itemBoost>().idBoost < id)
                indexInsertion++;
        // on le déplace des locked vers les unlocked
        t.transform.SetParent(unlockContent);
        t.SetSiblingIndex(indexInsertion);

        if (!boostPan.activeInHierarchy)
        {
            shouldBeRebuildWhenOpen = true;
            Debug.Log("<color=red>Boost menu should be rebuild when open</color>");
        }
        else
        {
            rebuildLayout();
        }

        nbBoostUnlocked++;
        updateCounterBoosteUnlocked();
    }

    public void updateBoostSavedValues(int id, int nbCard, int niveau)
    {
        int i = 0;
        // on modifie data et on le save
        foreach (object[] o in data)
        {
            if ((int)o[0] == id)
            {
                data[i] = new object[] { o[0], true, niveau, nbCard };
                SaveSystem.Save(typeSave.SHOP_BOOST, data);
                return;
            }
            i++;
        }         
    }

    private void updateCounterBoosteUnlocked()
    {
        nbBoostUnlockedText.text = traductionManager.getInstance().getTradShop(typeTraduction_shop.shop_pan_boostPocessedNumber)
            +" " +nbBoostUnlocked+"/12";
    }
    #endregion


    public ScrollRect scrollRect;
    public RectTransform viewPort_scrollBoost;
    public RectTransform contentPanel;
    public Vector2 offset;

    /*
     * void SnapTo(RectTransform target)
     * IEnumerator moveAnim(RectTransform toMove, Vector2 toPos, int id)
    */
    #region centrer scroll view sur item
    public void SnapTo_off(RectTransform target)
    {
        Vector2 v1 = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position);
        Vector2 v2 = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        Vector2 pos = (v1 - v2);

        pos = new Vector2(0, pos.y - scrollRect.GetComponent<RectTransform>().sizeDelta.y / 2);
        move_id++;
        StartCoroutine(moveAnim(contentPanel, pos + offset, move_id));

        // ?
        Canvas.ForceUpdateCanvases();
    }

    public void SnapTo_2(RectTransform target)
    {

        Vector2 v1 = (Vector2)viewPort_scrollBoost.transform.InverseTransformPoint(contentPanel.position);
        Vector2 v2 = (Vector2)viewPort_scrollBoost.transform.InverseTransformPoint(target.position);
        Vector2 pos = (v1 - v2);

        pos = new Vector2(0, pos.y - viewPort_scrollBoost.GetComponent<RectTransform>().sizeDelta.y / 2);
        move_id++;
        StartCoroutine(moveAnim(contentPanel, pos + offset, move_id));

        // ?
        Canvas.ForceUpdateCanvases();
    }

    public Transform posBottomScroll, posBottomContent;
    private int move_id = 0; // terminer anim si clique cash sur autre boost
    private IEnumerator moveAnim(RectTransform toMove, Vector2 toPos, int id)
    {
            float t = 0f, timeMove = 0.2f;
            Vector2 startPos = toMove.anchoredPosition;
            bool up = startPos.y > toPos.y;

            while (t < timeMove && id == move_id &&
                (up || posBottomContent.position.y < posBottomScroll.position.y))
            {
                yield return null;
                t += Time.deltaTime;

                float prog = t / timeMove;
                toMove.anchoredPosition = Vector2.Lerp(startPos, toPos, prog);
            }
    }

    #endregion

}
