using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraInfinisManager : MonoBehaviour
{
    public scrollInfinisManager scrollManager;
    public GameObject camInstance;
    public Transform contentCams;
    private List<itemCameraInfinis> camInstantiatedList = new List<itemCameraInfinis>();

    public void initGameInfinis(Transform secondLevel)
    {
        itemCameraInfinis newCam = stickCamToNewLevel(secondLevel, 1, false);
        // on anim directement fade alpha de la nouvelle cam crée
        newCam.animInitGame_secondLevel();
    }

    public void removeCamFromScroll(GameObject cam, Transform levelAssociated)
    {
        camInstantiatedList.Remove(cam.GetComponent<itemCameraInfinis>());
        Destroy(cam);
        //scrollManager.setActivateGo_level(levelAssociated.gameObject, false);
        //setActivateGo_test(levelAssociated.gameObject, false);
    }

    public void goNextPhase(Transform nextLevel)
    {
        stickCamToNewLevel(nextLevel, 0, false);
        foreach(itemCameraInfinis cam in camInstantiatedList)
        {
            cam.goNextPhase();
        }
    }

    public itemCameraInfinis stickCamToNewLevel(Transform levelTr, int phase, bool firstLevel)
    {
        itemCameraInfinis newCam = Instantiate(camInstance, contentCams).GetComponent<itemCameraInfinis>();
        newCam.associeLevelToCam(levelTr, phase, firstLevel);
        camInstantiatedList.Add(newCam);
        /*-----------------------------//!\\ , ne doit plus le faire de lui même
        scrollManager.setActivateGo_level(levelTr.gameObject, true);*/
        return newCam;
    }



    // OUVERTURE FERMETURE MENU

    //public int centeredPhase = 2;
    public void loadFirstLevel_openMenu(Transform initLevel)
    {
        stickCamToNewLevel(initLevel, scrollInfinisManager.centeredPhase, true);
    }

    public void closeCurrentCenteredLevel_closeMenu()
    {
        foreach(itemCameraInfinis item in camInstantiatedList)
        {
            if(item.myPhase == scrollInfinisManager.centeredPhase)
            {
                item.closeItem();
                break;
            }
        }
        camInstantiatedList = new List<itemCameraInfinis>();
    }

    public void closeGame_fadeLevelOut()
    {
        StartCoroutine(closeGame());
    }
    private IEnumerator closeGame()
    {
        // d'abord animClose
        List<itemCameraInfinis> closed = new List<itemCameraInfinis>();
        foreach (itemCameraInfinis item in camInstantiatedList)
        {
            if (item.myPhase != scrollInfinisManager.centeredPhase)
            {
                item.myLayoutAnim.animEndGame_forceAnimClose();
                closed.Add(item);
            }
        }
        yield return new WaitForSeconds(itemCameraInfinis.timeAnimAlpha);
        scrollManager.endGame_closeCam();
        // puis close
        foreach (itemCameraInfinis item_2 in closed)
        {
            item_2.closeItem();
        }
    }
}
