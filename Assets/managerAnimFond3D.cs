using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managerAnimFond3D : MonoBehaviour
{
    /*public bool test_btnOpenAnim = false;
    public bool test_btnCloseAnim = false;
    // Update is called once per frame
    void Update()
    {
        if (test_btnOpenAnim)
        {
            test_btnOpenAnim = false;
            StartCoroutine(animOpenEnum());
        }
        if (test_btnCloseAnim)
        {
            StartCoroutine(animCloseEnum());
            test_btnCloseAnim = false;
        }
    }*/

    public itemLine3DAnim[] linesAnims;
    public float timeBetweenEachAnim = 0.03f;
    private IEnumerator animOpenEnum()
    {
        foreach (itemLine3DAnim a in linesAnims)
        {
            a.openItem();
            yield return new WaitForSeconds(timeBetweenEachAnim);
        }
        foreach (itemLine3DAnim a in linesAnims)
        {
            a.endOpenAllItems();
        }
    }

    private IEnumerator animCloseEnum()
    {
        itemLine3DAnim a;
        for (int i = 0; i < linesAnims.Length; i++)
        {
            a = linesAnims[linesAnims.Length - 1 - i];
            a.closeItem();
            yield return new WaitForSeconds(timeBetweenEachAnim);
        }

        /*foreach (Animation a in linesAnims)
        {
            a.Play("closeLine3D");
            yield return new WaitForSeconds(timeBetweenEachAnim);
        }*/
    }


    private static managerAnimFond3D instance;
    public static managerAnimFond3D getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("animFond3DCanvas").GetComponent<managerAnimFond3D>();
        }
        return instance;
    }

    // /!\ gérer les couleurs

    // OUVERTURE
    // - lancement partie
    // - zoom entre level

    // FERMETURE
    // - fin partie
    // - dezoom entre level
    
    public void gameOpen()
    {
        // animOpen
        StartCoroutine(animOpenEnum());

        // OK aventure
        // OK infinis
    }

    public void menuPauseOn()
    {
        foreach (itemLine3DAnim a in linesAnims)
        {
            a.menuPauseOn();
        }
    }

    public void menuPauseOff()
    {
        foreach (itemLine3DAnim a in linesAnims)
        {
            a.menuPauseOff();
        }
    }

    public void menuPauseQuit()
    {
        StartCoroutine(animClosePAUSEEnum());
    }

    private IEnumerator animClosePAUSEEnum()
    {
        itemLine3DAnim a;
        for (int i = 0; i < linesAnims.Length; i++)
        {
            a = linesAnims[linesAnims.Length - 1 - i];
            a.menuPauseQuit();
            yield return new WaitForSeconds(timeBetweenEachAnim);
        }
    }

    /*public void menuPause_quit()
    {
        // animClose
        StartCoroutine(animCloseEnum());

        // OK aventure
        // OK infinis
    }*/

    private Color curColorFond;
    // appelé quand
    // change mode aventure <-> expert
    // change mode aventure <-> infinis
    // change level ingame aventure
    // level up ingame infinis
    public void setCurrentFondColor(Color c)
    {
        // on change la couleure de tout les fonds ( sans changer leurs c.alpha )
    }


    public void zoom_level()
    {

        // animOpen
        StartCoroutine(animOpenEnum());

        // OK aventure
        //  infinis
    }

    public void dezoom_level()
    {
        // animClose
        StartCoroutine(animCloseEnum());

        // OK aventure
        //  infinis
    }

    public void playerBounce()
    {
        foreach (itemLine3DAnim a in linesAnims)
        {
            a.playerBounce();
        }
    }
}
