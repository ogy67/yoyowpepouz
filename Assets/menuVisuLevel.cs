using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuVisuLevel : MonoBehaviour
{
    public GameObject menuGo;
    public void initApp()
    {
        StartCoroutine(initApp_enum());
    }

    private IEnumerator initApp_enum()
    {
        yield return new WaitForSeconds(0.2f);
        // on pre-fill la liste avec les items
        menuGo.SetActive(true);
        fillListWithInitItems_2();

        yield return null;
        menuGo.SetActive(false);
    }

    public void initOpenMenuP()
    {
        prevCrownUpdate = SaveSystem.nb_couronne;
        prevStarUpdate = SaveSystem.nbStarObtain;

        compteurCrownText.text = SaveSystem.nb_couronne + "<color=\"#856A3D\"><size=23>/</size><size=20>" +
            (SaveSystem.nbNiveauAventure * 3) + "</size></color>";
        compteurStarText.text = SaveSystem.nbStarObtain + "<color=\"#4B7382\"><size=23>/</size><size=20>" +
           SaveSystem.nbNiveauAventure + "</size></color>";

        setInitInfo();
        reorganizeListToVisualPath();
    }

    public void menuPLevelStarUnlocked()
    {
        addWaitingReward();
    }



    private int prevCrownUpdate = -1;
    private int prevStarUpdate = -1;
    public GameObject instance, instance_empty;
    public Transform contentItem;
    public Text compteurCrownText, compteurStarText;
    private int prevLevelCentered = -1;
    private RectTransform itemSelected;
    public void openMenu(bool clickedLockedPan)
    {
        //if(clickedLockedPan)
            //animClick_compteur.Play("itemUIClicked");

        menuAnim.Play("openMenuVisuLevel");

        //bool initialisation = prevCrownUpdate == -1;
        bool update = prevCrownUpdate != SaveSystem.nb_couronne
            || prevStarUpdate != SaveSystem.nbStarObtain;

        if (update)
        {

            prevCrownUpdate = SaveSystem.nb_couronne;
            prevStarUpdate = SaveSystem.nbStarObtain;

            compteurCrownText.text = SaveSystem.nb_couronne + "<color=\"#856A3D\"><size=23>/</size><size=20>" +
                (SaveSystem.nbNiveauAventure * 3) + "</size></color>";
            compteurStarText.text = SaveSystem.nbStarObtain + "<color=\"#4B7382\"><size=23>/</size><size=20>" +
               SaveSystem.nbNiveauAventure + "</size></color>";

            updateInfoItemScroll();

        }


        // si le niveau centré n'est pas le meme que precedent, update imgSelected
        if (prevLevelCentered != managerNiveau.currentNiveau)
        {
            prevLevelCentered = managerNiveau.currentNiveau;
            itemVisuLevel v;
            foreach (Transform t in contentItem)
            {
                if (t.TryGetComponent<itemVisuLevel>(out v))
                {
                    if (v.numLevel - 1 == prevLevelCentered)
                    {
                        itemSelected = v.GetComponent<RectTransform>();
                        v.itemIsSelected();
                        break;
                    }
                }
            }
        }

        StartCoroutine(centerScrollToItem(itemSelected));
    }


    private void fillListWithInitItems_2()
    {
        int index = 0;
        foreach (Transform t in mN.lesNiveaux)
        {
            // on ajoute un nouvel item dans gridLayout
            GameObject o = Instantiate(instance, contentItem);
            o.SetActive(true);
            o.GetComponent<itemVisuLevel>().initApp(index);
            index++;
        }

        // on doit remplir toutes les colonnes ( pas 1 item seulement sur 4 )
        // mN.lesNiveaux.Length % 4
        // 78 % 4 -> 2 , bon
        // 77 % 4 -> 1 
        int nbItemsEmpty = 4 - mN.lesNiveaux.Length % 4;
        for (int e = 0; e < nbItemsEmpty; e++)
        {
            GameObject o2 = Instantiate(instance_empty, contentItem);
            o2.SetActive(true);
        }
    }

    private void setInitInfo()
    {
        bool[] listStar = mN.starObtain;
        int levelLock = ScrollRectSnap_CS.lockedLevelNum;
        int lastItemIndex = mN.lesNiveaux.Length - 1;
        int levelSelected = managerNiveau.currentNiveau;

        itemVisuLevel v;
        int indexLevel;
        foreach (Transform t in contentItem)
        {
            if(t.TryGetComponent<itemVisuLevel>(out v))
            {
                indexLevel = v.numLevel; 
                if (indexLevel >= levelLock - 1)
                {
                    // pan locked
                    v.initItem(indexLevel + 1, 0, false, indexLevel >= levelLock - 1, indexLevel == lastItemIndex
                        , indexLevel == levelSelected);
                }
                else
                {
                    // pan unlocked
                    bool hasStar = listStar[indexLevel];
                    int nbCrown = managerNiveau.records[indexLevel];
                    v.initItem(indexLevel + 1, nbCrown, hasStar, indexLevel >= levelLock - 1, indexLevel == lastItemIndex
                        , indexLevel == levelSelected);
                }
                // test
                if (lockStar.forceUnlock == false && paliersStar.Contains(indexLevel))
                {                   
                    int nbCoin = rewardCoinList[paliersStar.IndexOf(indexLevel)];
                    //Debug.Log("rewardTakenList.Count : " + rewardTakenList.Count + " , paliersStar.IndexOf(indexLevel) : " + paliersStar.IndexOf(indexLevel));
                    bool rewardTaken = rewardTakenList[paliersStar.IndexOf(indexLevel)];
                    v.setLockedStar(nbCoin, rewardTaken);
                    if(!rewardTaken && !v.isLocked)
                        addWaitingReward();
                    //Debug.Log("Set reward coin, levelIndex : " + indexLevel+" , nbCoin : "+nbCoin);
                }
            }
        }
    }



    /*private void fillListWithInitItems_off()
    {
        // on remplit la liste graçe aux infos des niveaux
        // liste prise dans manager niveau 
        int index = 0;
        Transform[] listNiveau = mN.lesNiveaux;




        bool[] listStar = mN.starObtain;
        int levelLock = ScrollRectSnap_CS.lockedLevelNum;
        int lastItemIndex = listNiveau.Length - 1;
        int levelSelected = managerNiveau.currentNiveau;
        foreach (Transform t in listNiveau)
        {
            // on ajoute un nouvel item dans gridLayout
            GameObject o = Instantiate(instance, contentItem);
            o.SetActive(true);
            if (index >= levelLock - 1)
            {
                o.GetComponent<itemVisuLevel>().initItem(index + 1, 0, false, index >= levelLock - 1, index == lastItemIndex
                    , index == levelSelected);
            }
            else
            {
                bool hasStar = listStar[index];
                int nbCrown = managerNiveau.records[index];
                o.GetComponent<itemVisuLevel>().initItem(index + 1, nbCrown, hasStar, index >= levelLock - 1, index == lastItemIndex
                    , index == levelSelected);
            }
            index++;
        }

        // on doit remplir toutes les colonnes ( pas 1 item seulement sur 4 )
        for (int e = 0; e < listNiveau.Length % 4; e++)
        {
            GameObject o2 = Instantiate(instance_empty, contentItem);
            o2.SetActive(true);
        }
    }*/

    private void reorganizeListToVisualPath()
    {
        // on reorganise la liste
        // visuel 1 2 3 4 , 5 6 7 8
        // code 1 2 3 4 , 8 7 6 5
        int index_2 = 0;
        bool itemAddedDown = true;

        while (index_2 < contentItem.childCount)
        {
            if ((index_2 + 1) % 4 == 0
                && index_2 + 1 < contentItem.childCount)
            {
                if (itemAddedDown)
                {
                    // on réorganise les 4 niveaux suivants
                    for (int index_3 = 1; index_3 < 5; index_3++)
                    {
                        contentItem.GetChild(index_2 + 1).SetSiblingIndex(index_2 + (5 - index_3));
                    }
                }
                itemAddedDown = !itemAddedDown;
            }
            drawPath(index_2, itemAddedDown);
            index_2++;
        }
    }

    private void drawPath(int index_2, bool itemAddedDown)
    {
        itemVisuLevel v;
        bool isEdge;
        if (contentItem.GetChild(index_2).TryGetComponent<itemVisuLevel>(out v))
        {
            isEdge = (index_2 + 1) % 8 == 4 || ((index_2 + 1) % 8 == 5 && index_2 > 1);

            if (v.isLastItem)
            {
                // si on ajoute suis liste vers haut,
                // et si on est pas sur le coin du bas
                if (!itemAddedDown && !((index_2 + 1) % 8 == 4))
                {
                    v.setPathImg(1);// bas
                }
            }
            else
            {
                // si index % 8 == 4 , droite
                // si index % 8 == 0 et index > 0 , droite bas
                // si index % 8 == 5 , rien
                if (isEdge)
                {
                    if ((index_2 + 1) % 8 == 4)
                    {
                        v.setPathImg(2);// droite
                    }
                    else if ((index_2 + 1) % 8 == 5 && index_2 > 1)
                    {
                        v.setPathImg(2);// droite
                        v.setPathImg(1);// bas
                    }
                }
                else if ((index_2 + 1) % 8 != 0)
                {
                    v.setPathImg(1);// bas
                }
            }
        }
    }

    private void updateInfoItemScroll()
    {
        itemVisuLevel v;
        int index = 0;
        int levelLock = ScrollRectSnap_CS.lockedLevelNum;
        int levelSelected = managerNiveau.currentNiveau;

        //Transform[] listNiveau = managerNiveau.getInstance().lesNiveaux;
        bool[] listStar = managerNiveau.getInstance().starObtain;

        foreach (Transform t in contentItem)
        {
            if (t.TryGetComponent<itemVisuLevel>(out v))
            {
                int indexLevel = v.numLevel - 1;

                if (indexLevel >= levelLock - 1)
                {
                    // pas d'update si locked
                }
                else
                {                    
                    bool hasStar = listStar[indexLevel];
                    int nbCrown = managerNiveau.records[indexLevel];
                    v.updateItem(nbCrown, hasStar, false, v.numLevel == levelSelected + 1);
                }
            }
            index++;
        }
    }


    public Animation menuAnim;
    public void btnCloseMenu()
    {
        menuAnim.Play("closeMenuVisuLevel");
    }

    public managerNiveau mN;
    public void clickItemLevel(Transform t)
    {
        // ne peut que appuyer sur btn quand unlocked
        itemVisuLevel v = t.GetComponent<itemVisuLevel>();

        // si item reward, et reward peut être prise
        if (v.itemLockedStar)// est set lockedStar si reward pas encore prise
        {
            // donner reward, pas ouvrir level
            btnGetRewardPalier(v.numLevel - 1, v.nbCoinPalier, v);
        }
        // si n'est pas level actuel, selectionner
        else if (v.numLevel != managerNiveau.currentNiveau + 1)
        {
            itemSelected = t.GetComponent<RectTransform>();
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
            mN.menuVisu_forceGoLevel(v.numLevel);
            v.itemIsSelected();
            prevLevelCentered = managerNiveau.currentNiveau;
        }
    }

    public RectTransform contentRect;
    public ScrollRect scroll;
    private IEnumerator centerScrollToItem(RectTransform itemRect)
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        Vector2 pos = (Vector2)scroll.transform.InverseTransformPoint(contentRect.position)
            - (Vector2)scroll.transform.InverseTransformPoint(itemRect.position);
        contentRect.anchoredPosition = new Vector2(pos.x, contentRect.anchoredPosition.y);

        if (scroll.normalizedPosition.x < 0)
            scroll.normalizedPosition = new Vector2(0, scroll.normalizedPosition.y);
        if (scroll.normalizedPosition.x > 1)
            scroll.normalizedPosition = new Vector2(1, scroll.normalizedPosition.y);
    }







    // REWARD
    public static menuVisuLevel instanceCs;

    public static menuVisuLevel getInstance()
    {
        if (instanceCs == null)
            instanceCs = GameObject.Find("menuVisuLevelCs").GetComponent<menuVisuLevel>();
        return instanceCs;
    }

    private List<int> paliersStar;
    public void addPalierStar(int indexLevel)
    {
        if(paliersStar == null)
            paliersStar = new List<int>();
        paliersStar.Add(indexLevel);
    }

    private List<int> rewardCoinList = new List<int>();
    public int coinStart = 30;
    public int coinAddedEachPalier = 10;
    // appelé une fois que tout les paliers sont crées
    public void createListRewardCoin()
    {
        int tot = coinStart;
        for(int i = 0; i < paliersStar.Count; i++)
        {
            rewardCoinList.Add(tot);
            tot += coinAddedEachPalier;
        }
        loadRewardTakenList();
    }

    private List<bool> rewardTakenList;

    private void loadRewardTakenList()
    {
        rewardTakenList = (List<bool>)SaveSystem.loadData(typeSave.PROG_LEVEL_REWARD);
        if(rewardTakenList.Count == 0
            || rewardTakenList.Count < rewardCoinList.Count)
        {
            while(rewardTakenList.Count < rewardCoinList.Count)
            {
                rewardTakenList.Add(false);
            }
            SaveSystem.Save(typeSave.PROG_LEVEL_REWARD, rewardTakenList);
        }
    }

    private void btnGetRewardPalier(int numLevel, int nbCoin, itemVisuLevel item)
    {
        //Debug.Log(numLevel+" , should give reward Coin : "+nbCoin);

        item.getReward();
        sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.clickGetReward);
        popYowGemManager.getInstance().popRec_main(typeReward_game.yow, nbCoin, item.transform.position);

        rewardTakenList[paliersStar.IndexOf(numLevel)] = true;
        removeWaitingReward();
        SaveSystem.Save(typeSave.PROG_LEVEL_REWARD, rewardTakenList);
    }



    // NOTIF REWARD
    private int nbRewardWaitingTaken = 0;
    public GameObject notifGo;
    private void addWaitingReward()
    {
        nbRewardWaitingTaken++;
        if (nbRewardWaitingTaken == 1)
            changeNotifActivated(true);
    }
    private void removeWaitingReward()
    {
        nbRewardWaitingTaken--;
        if (nbRewardWaitingTaken == 0)
            changeNotifActivated(false);
    }
    private void changeNotifActivated(bool isOn)
    {
        notifGo.SetActive(isOn);
    }
}

