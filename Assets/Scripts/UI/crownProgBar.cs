using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class crownProgBar : MonoBehaviour
{
    //public static crownProgBar instance;

    // Start is called before the first frame update
    void Start()
    {
        //instance = GameObject.Find("crownLvlProgBar").GetComponent<crownProgBar>();
    }

    public static crownProgBar getInstance()
    {
        return GameObject.Find("crownLvlProgBar").GetComponent<crownProgBar>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (btnInit)
        {
            loadBar(5, 4);
            btnInit = false;
        }
        if (btnChangeBar)
        {
            StartCoroutine(changeBar());
            btnChangeBar = false;
        }*/
    }
    //public int max;
    //public int rec;
    //public bool btnChangeBar = false;

    //public bool btnInit = false;
    public GameObject crownGo;
    //public float crownOffAlpha = 0.3f;
    //public Sprite crownOn;
    public int espace = 20;

    public static int maxi;
    private int nbCrownProgB = 3;
    public void changeBarValues(int rec)
    {
        StartCoroutine(changeBar(rec));
    }
    public IEnumerator changeBar(int rec)
    {
        StartCoroutine(closeBar());
        yield return new WaitForSeconds(8f / 60f);
        loadBar(rec);
    }
    public void loadBar(int rec)
    {
        destroyPrev();
        int middle = (nbCrownProgB - 1) * espace / 2; // a avoir pour être au milieu
        maxi = rec;

        for (int i = 0; i < nbCrownProgB; i++)
        {
            int posX = i * espace - middle;
            GameObject c = Instantiate(crownGo);
            c.transform.SetParent(transform.parent);
            c.transform.localScale = crownGo.transform.localScale;
            c.transform.SetParent(transform);
   
            setVoyantCrownItem(c.transform, i < rec);

            /*if (i < rec)
            {
                c.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }*/
                //c.GetComponent<Image>().sprite = crownOn;
            c.GetComponent<RectTransform>().localPosition = new Vector2(posX, 0);
            c.SetActive(true);

            c.GetComponent<Animation>().Play("crownPopProgressBar");
        }
    }

    private void setVoyantCrownItem(Transform crownItem, bool isOn)
    {
        crownItem.GetChild(0).gameObject.SetActive(!isOn);
        crownItem.GetChild(1).gameObject.SetActive(isOn);
    }

    public void reOpenBar()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
            t.GetComponent<Animation>().Play("crownPopProgressBar");
        }
    }

    public IEnumerator closeBar()
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<Animation>().Play("crownUnPopProgressBar");
        }
        yield return new WaitForSeconds(8f / 60f);

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void destroyPrev()
    {
        //currentLevel = 0;
        foreach (Transform t in transform)
            Destroy(t.gameObject);
    }

    public void crownEnterBar()
    {
        Debug.Log("<color=yellow>- crown enter bar, go index : </color>" + maxi);
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.crown_enterBar);
        setVoyantCrownItem(transform.GetChild(maxi), true);
        //transform.GetChild(maxi).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        maxi++;
        //int rang = 0;
        //while (rang < transform.childCount && transform.GetChild(rang) != target) rang++;
    }

    public List<Transform> getFreeStatment()
    {
        List<Transform> ret = new List<Transform>();
        for(int i = maxi; i < transform.childCount; i++)
        {
            ret.Add(transform.GetChild(i));
        }
        return ret;
    }
}
