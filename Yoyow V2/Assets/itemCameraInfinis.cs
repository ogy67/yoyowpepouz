using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemCameraInfinis : MonoBehaviour
{
    public List<itemLayoutAnim> lesAnimList;
    public cameraInfinisManager camManager;
    public int myPhase;
    private const int maxPhase = 4;
    private Transform levelAssociated;
    public itemLayoutAnim myLayoutAnim;
    public int defaultLayerNumber = 0;

    public static float timeAnimAlpha = 1f;

    public void associeLevelToCam(Transform level, int phaseLevel, bool firstLevel)
    {
        myPhase = phaseLevel;
        levelAssociated = level;
        
        if (!firstLevel)
        {
            myLayoutAnim = lesAnimList[myPhase];
            myLayoutAnim.setInitAlphaAnim();
            setLayerRecursive(levelAssociated, myLayoutAnim.layoutNumber); // -1 ?
        }
    }

    public void goNextPhase()
    {
        StartCoroutine(animGoNextPhase());
    }

    private IEnumerator animGoNextPhase()
    {
        // d'abord on anim
        if(myPhase == scrollInfinisManager.centeredPhase)
        {
            myLayoutAnim = lesAnimList[myPhase];
            setLayerRecursive(levelAssociated, myLayoutAnim.layoutNumber);
        }
        /*if(myLayoutAnim == null) // first level, first anim après init game
        {
            myLayoutAnim = lesAnimList[myPhase];
            setLayerRecursive(levelAssociated, myLayoutAnim.layoutNumber);
        }*/
        myLayoutAnim.animImageAlpha();

        yield return new WaitForSeconds(timeAnimAlpha);

        // puis passe au layer suivant
        myPhase++;
        if (myPhase == maxPhase)
        {
            closeItem();
        }
        else
        {
            myLayoutAnim = lesAnimList[myPhase];
            if (myPhase != scrollInfinisManager.centeredPhase)
            {
                myLayoutAnim.setInitAlphaAnim(); // remettre anim à zero
                setLayerRecursive(levelAssociated, myLayoutAnim.layoutNumber);
            }
            else
            {
                setLayerRecursive(levelAssociated, defaultLayerNumber);
            }
        }
    }
    /*public void gameEnd_isCentered()
    {
        // on change le layer de level to default
        setLayerRecursive(levelAssociated, defaultLayerNumber);
    }*/


    // appelé seulement quand gameStart pour level 2
    public void animInitGame_secondLevel()
    {
        // anim layer de 0 -> 0.5
        myLayoutAnim.animInitGame_secondLevel();
    }

    public void closeItem()
    {
        setLayerRecursive(levelAssociated, defaultLayerNumber);
        camManager.removeCamFromScroll(gameObject, levelAssociated);
    }

    private void setLayerRecursive(Transform item, int layer)
    {
        item.gameObject.layer = layer;
        foreach (Transform child in item)
        {
            child.gameObject.layer = layer;

            Transform hasChild = child.GetComponentInChildren<Transform>();
            if (hasChild != null)
                setLayerRecursive(child, layer);
        }
    }
}
