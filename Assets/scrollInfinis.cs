using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollInfinis : MonoBehaviour
{
    public scrollInfinisManager scrollManager;
    public Transform posCenter, posDistance;
    private float distanceBetweenLevels_local;
    /*private float get_distanceBetweenLevels_world()
    {
        return Vector2.Distance(posCenter.position, posDistance.position);
    }*/
    //private float distanceBetweenLevels_world;



    // appelé une seule fois au démarage
    private bool scrollInitialized = false;
    private void initScrollValues()
    {
        scrollInitialized = true;
        distanceBetweenLevels_local = Vector2.Distance(posCenter.localPosition, posDistance.localPosition);
        //distanceBetweenLevels_world = Vector2.Distance(posCenter.position, posDistance.position);
        initCenterPosY = posCenter.localPosition.y;
        itemList = new List<itemScrollInfinis>();
    }

    public void initGameInfinis(Transform secondLevel)
    {
        resetUI();
        currentLevelInfinis = 1; // premier level déjà ajouté : pos = center + 2 fois distance
        if (!scrollInitialized)
            initScrollValues();
        // on met firstLevel dans l'item qui va animer sa position
        // createNewItemInScrollList(firstLevel, 2);
        // on met secondLevel dans l'item qui va animer sa position
        createNewItemInScrollList(secondLevel, 1);
    }



    private float initCenterPosY;

    private int currentLevelInfinis;
    public void goNextPhase(Transform nextLevel)
    {
        createNewItemInScrollList(nextLevel, 0);
        // le scroll descend sur prochaine target
        StartCoroutine(animScrollDown());

        foreach(itemScrollInfinis itemS in itemList)
        {
            itemS.goNextPhase();
        }

        //currentLevelInfinis++;
    }

    private List<itemScrollInfinis> itemList;
    public GameObject itemScrollInstance;
    private int nbLevelPassed = 0;
    private void createNewItemInScrollList(Transform newLevel, int phase)
    {
        nbLevelPassed++;

        scrollManager.setActivateGo_level(newLevel.gameObject, true);
        //setActivateGo_test(newLevel.gameObject, true);

        GameObject item = Instantiate(itemScrollInstance, animScrollTransform);
        item.SetActive(true);
        itemScrollInfinis i = item.GetComponent<itemScrollInfinis>();
        itemList.Add(i);

        float newYPosition = initCenterPosY + distanceBetweenLevels_local * currentLevelInfinis;
        currentLevelInfinis++;
        newLevel.GetComponent<packNiveau_infinis>().setNumArrivee(nbLevelPassed);

        i.associeLevelToScrollItem(newLevel, phase, newYPosition);
    }

    public void removeItemFromScroll(GameObject scrollItem, GameObject levelAssociated)
    {
        itemList.Remove(scrollItem.GetComponent<itemScrollInfinis>());
        Destroy(scrollItem);
        scrollManager.setActivateGo_level(levelAssociated, false);
        //setActivateGo_test(levelAssociated, false);
    }



    // MOUVEMENT
    public float timeAnimScroll = 0.8f;
    public float timeAnimWaitScroll = 15 / 60f;
    public Animation animScroll;
    public Transform animScrollTransform;
    //public Transform initYOffsetTr;
    //public Transform posCenterTr_test, posGoalTr_test;

    private IEnumerator animScrollDown()
    {
        animScroll.Play("scrollInfinisZoomLevel");
        yield return new WaitForSeconds(timeAnimWaitScroll);

        /*float yOffset = 0;
        if (currentLevelInfinis == 3)
        {
            Debug.Log("FIrst level end, set Y Offset");
            yOffset = initYOffsetTr.localPosition.y;
        }*/
        //float yOffset = currentLevelInfinis == 1 ? initYOffsetTr.position.y : 0f;

        float initPosY = animScrollTransform.localPosition.y;// - distanceBetweenLevels_world * currentLevelInfinis;
        //posCenterTr_test.position = new Vector2(0, initPosY);
        float newTargetPosY = animScrollTransform.localPosition.y - distanceBetweenLevels_local;// + yOffset;// * (currentLevelInfinis+1);
        //posGoalTr_test.position = new Vector2(0, newTargetPosY);
        float posX = animScrollTransform.localPosition.x;

        float initTime = Time.time;
        float prog, progY;
        while (Time.time < initTime + timeAnimScroll)
        {
            yield return null;
            prog = (Time.time - initTime) / timeAnimScroll;
            prog = prog > 1 ? 1 : prog;
            progY = initPosY + (newTargetPosY - initPosY) * prog;
            animScrollTransform.localPosition = new Vector2(posX, progY);
        }
        animScrollTransform.localPosition = new Vector2(posX, newTargetPosY);
    }


    // OUVERTURE FERMETURE MENU

    private const string animLevelLightOn = "switchLightLevelOn";
    private const string animLevelLightOff = "switchLightLevelOff";
    public void animLevelLight(Transform level, string anim)
    {
        //Debug.Log("Switch light level " + level.name + " (" + anim + ")");
        //Debug.Log("Anim level light : "+anim);
        level.GetComponent<Animation>().Play(anim);
    }


    public void loadFirstLevel_openMenu(Transform initLevel, bool fromAdventure)
    {
        itemList = new List<itemScrollInfinis>();
        //Debug.Log("Load first level infinis, open menu");
        nbLevelPassed = 0;

        if (!scrollInitialized)
            initScrollValues();

        createNewItemInScrollList(initLevel, scrollInfinisManager.centeredPhase);
        if(fromAdventure)
            animLevelLight(initLevel, animLevelLightOn);
    }

    public void closeCurrentCenteredLevel_closeMenu()
    {
        // on cherche l'item en phase 2 dans liste
        foreach(itemScrollInfinis item in itemList)
        {
            if(item.myPhase == scrollInfinisManager.centeredPhase)
            {
                item.closeItem();

                // on ne doit pas animer ( est mit off )
                // cash enlever glow

                Transform imgGlow = item.levelAssociated.GetChild(1);
                Color32 c = imgGlow.GetComponent<SpriteRenderer>().color;
                imgGlow.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0);

                //animLevelLight(item.levelAssociated, animLevelLightOff);
                break;
            }
        }
        itemList = new List<itemScrollInfinis>();

        currentLevelInfinis = 0;

        resetUI();
    }

    public void resetUI()
    {
        // remettre scrollContent sur position initiale
        animScrollTransform.localPosition = new Vector2(0, 0);//animScrollTransform.position.x, initCenterPosY);
    }

    public void closeGame_levelOut()
    {
        // on supprime les ITEMS de liste sauf milieu en fin d'anim de camera
        // fermer tt les items sauf celui du milieu
        StartCoroutine(closeGame());
    }

    private IEnumerator closeGame()
    {
        List<itemScrollInfinis> closed = new List<itemScrollInfinis>();
        itemScrollInfinis current = null;
        foreach (itemScrollInfinis item in itemList)
        {
            if (item.myPhase != scrollInfinisManager.centeredPhase)
                closed.Add(item);
            else
                current = item;
                //animLevelLight(item.levelAssociated, animLevelLightOn);
        }

        yield return new WaitForSeconds(0.5f);
        animLevelLight(current.levelAssociated, animLevelLightOn);

        // *0.7f parce que être sur de le fermer avant fin anim cam ( cam le remet sur layer 0 une fois detruit 
        yield return new WaitForSeconds((itemCameraInfinis.timeAnimAlpha - 0.5f)*0.7f);

        foreach (itemScrollInfinis item_2 in closed)
            item_2.closeItem();



        /*int i = 0;
        Debug.Log("Nb items in list : " + itemList.Count);
        while(i < itemList.Count)
        {
            Debug.Log("Parcourt liste, item.phase : " + itemList[i].myPhase);
            if(itemList[i].myPhase != scrollInfinisManager.centeredPhase)
            {
                itemList[i].closeItem();
                // item est enlevé de liste par itemList[i]
            }
            else
            {
                Debug.Log("Anim glow ON close game");
                animLevelLight(itemList[i].levelAssociated, animLevelLightOn);
                i++;
            }
        }*/

        /* on cherche l'item en phase 2 dans liste
        foreach (itemScrollInfinis item in itemList)
        {
            if (item.myPhase != scrollInfinisManager.centeredPhase)
            {
                item.closeItem();
                //break;
            }
            else
            {
                animLevelLight(item.levelAssociated, animLevelLightOn);
            }
        }*/
    }
}

   
