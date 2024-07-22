using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progBarGame : MonoBehaviour
{

    public bool change = false; 
    public GameObject item;
    public GameObject conteneurItem;
    public int espace = 25;

    public GameObject[] instances;

    public int currentLevel;
    private static int nombreNiv;
    public static float timeBetweenPop = 1f / 6f;

    //public bool barLoaded = false;
    public IEnumerator animBarStart(int nbLvl)
    {
        nombreNiv = 0;
        //barLoaded = false;
        destroyPrev();
        int middle = (nbLvl - 1) * espace / 2; // a avoir pour être au milieu
        instances = new GameObject[nbLvl];
        //nombreNiv = nbLvl;

        for (int i = 0; i < nbLvl; i++)
        {
            GameObject ins = Instantiate(item);
            int posX = i * espace - middle;
            ins.transform.SetParent(conteneurItem.transform, false);
            ins.transform.localScale = item.transform.localScale;
            ins.GetComponent<RectTransform>().localPosition = new Vector2(posX, 0);
            ins.SetActive(true);
            ins.GetComponent<Animation>().Play("progBarNivPop");
            instances[i] = ins;
            nombreNiv++;
            //managerAudio.playSound(typeSon.PROG_BAR_POP);
        }
        yield return new WaitForSeconds(15/60f);
        instances[0].GetComponent<Animation>().Play("progBarNivBig");
        //barLoaded = false;
    }

    public void animBarStop()
    {
        for (int i = 0; i < nombreNiv; i++)
        {
            instances[nombreNiv - 1 - i].GetComponent<Animation>().Play("progBarNivUnPop");
            instances[nombreNiv - 1 - i].SetActive(false);
        }
    }

    public void destroyPrev()
    {
        currentLevel = 0;
        foreach (Transform t in conteneurItem.transform)
            Destroy(t.gameObject);
    }

    public void initBar(int nbLvl)
    {
        StartCoroutine(animBarStart(nbLvl));
    }

    public Animation voyantAnim;
    public void passNextLvl(bool becomeCrown)
    {
        if (currentLevel < nombreNiv)
        {
            voyantAnim.Play("aventureLevelUp");
            instances[currentLevel].GetComponent<Animation>().Play("progBarNivLitlle");
            if (becomeCrown)
            {
                StartCoroutine(switchToCrown(instances[currentLevel]));
            }
        }
        if (currentLevel + 1 < nombreNiv)
        {
            instances[currentLevel + 1].GetComponent<Animation>().Play("progBarNivBig");
            currentLevel++;
        }
    }

    public Sprite crownSp;
    public IEnumerator switchToCrown(GameObject toSwitch)
    {
        toSwitch.GetComponent<Animation>().Play("switchToCrown");
        yield return new WaitForSeconds(1 / 6f);
        //toSwitch.GetComponent<Image>().sprite = crownSp;
    }
}
