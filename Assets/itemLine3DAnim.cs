using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemLine3DAnim : MonoBehaviour
{
    // gérer l'animation static
    // se lance quand l'item est ouvert
    // se met en pause ( attend fin anim ) quand : va pause / va mort

    public Animation animLine;
    public void openItem()
    {
        animLine.Play("openLine3D");       
    }
    public void endOpenAllItems()
    {
        staticAnim_open();
    }

    // fermeture normale, ici quand gameover ( devra être changé pour effet zoom comme quand pause )
    public void closeItem()
    {
        animLine.Play("closeLine3D");
        staticAnim_goGameOver();
    }

    public void menuPauseOn()
    {
        animLine.Play("pauseOnLine3D");
        staticAnim_goPause();
    }

    public void menuPauseOff()
    {
        animLine.Play("pauseOffLine3D");
        staticAnim_unPause();
    }

    public void menuPauseQuit()
    {
        animLine.Play("pauseQuitLine3D");
    }

    public void playerBounce()
    {
        animLine.Play("bounceLine3D");
    }



    /*
     * void staticAnim_open()
     * IEnumerator loopAnimStatic()
     * 
     * void staticAnim_goPause()
     * void staticAnim_goGameOver()
     * void staticAnim_unPause()
     * void staticAnim_revive()
    */
    #region gestion dynamique de l'animation statique
    private void staticAnim_open()
    {
        //StartCoroutine(loopAnimStatic());
    }

    public Animation staticAnim;
    public string animStr = "animStaticFond3D", animStr_reverse = "animStaticFond3D_reverse";

    public float timeAnim = 60 / 60f, timeBetweenAnim = 0f;
    private bool animGoingReverse = false;
    private int currentIdAnim = 0;
    private List<int> idsAnimStopped = new List<int>();
    private IEnumerator loopAnimStatic()
    {
        currentIdAnim++;
        int curId = currentIdAnim;

        if (timeBetweenAnim != 0f)
            yield return new WaitForSeconds(timeBetweenAnim);
        while (curId == currentIdAnim && !idsAnimStopped.Contains(currentIdAnim))
        {
            /*str = useReverseAnim ? 
                animGoingReverse ? animStr : animStr_reverse
                : animStr;*/

            if (staticAnim.isPlaying)
                staticAnim[animStr].time = 0f;
            else
                staticAnim.Play(animStr);

            yield return new WaitForSeconds(timeAnim);
            if(curId == currentIdAnim && !idsAnimStopped.Contains(currentIdAnim))
            {
                animGoingReverse = !animGoingReverse;
                if(timeBetweenAnim != 0f)
                    yield return new WaitForSeconds(timeBetweenAnim);
            }
        }
    }

    private void staticAnim_goPause()
    {
        idsAnimStopped.Add(currentIdAnim);
    }
    private void staticAnim_goGameOver()
    {
        idsAnimStopped.Add(currentIdAnim);
    }
    private void staticAnim_unPause()
    {
        //StartCoroutine(loopAnimStatic());
    }
    private void staticAnim_revive()
    {
        StartCoroutine(loopAnimStatic());
    }

    #endregion
}
