using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progBarModeOnline : MonoBehaviour
{
    public GameObject progItem;
    public GameObject curseur;
    public Color32 itemColorNotWin;
    public float distanceBetItem = 1f;
    public int nbItem;
    private int progBarValue;

    #region test
    // Update is called once per frame
    void Update()
    {
        if (setBar)
        {
            setBar = false;
            progBarValue = 0;
            initProgBar(test_ListId);
        }
        else if (passNextProgBar)
        {
            passNextProgBar = false;
            progBarPassNext(curseur, progBarValue);
            progBarValue += 1;
        }
        else if (initOthersCursorBtn)
        {
            initOthersCursorBtn = false;
            initOthersCursor(test_ListId);
        }
        else if (moveOtherCursorBtn)
        {
            moveOtherCursorBtn = false;
            progBarPassNext(othersCursor[1], lvlOther);
            lvlOther += 1;
        }
        else if (cleanBarAnimBtn)
        {
            cleanBarAnimBtn = false;
            StartCoroutine(animCleanBar());
        }
    }
    public bool cleanBarAnimBtn = false;
    public bool setBar = false, passNextProgBar = false, initOthersCursorBtn = false, moveOtherCursorBtn = false;
    //public int moveCursorID;
    private int lvlOther = 0;

    #endregion

    private static progBarModeOnline instance;
    public static progBarModeOnline getInstance()
    {
        if(instance == null)
            instance = GameObject.Find("progBarModeOnline").GetComponent<progBarModeOnline>();
        return instance;
    }


    List<GameObject> itemsBarList = new List<GameObject>();
    List<float> posItems = new List<float>();

    public void localPlayerWin()
    {
        progBarPassNext(curseur, progBarValue);
        progBarValue += 1;
    }

    public void progBarPassNext(GameObject toMove, int progVal)
    {
        /*if (forceQuit)
        {
            toMove.GetComponent<Animation>().Play("cursorProgBarOnlineOff");
            cursorFinished.Add(toMove);
        }
        else
        {*/
            bool localP = toMove == curseur;

            if (progVal < itemsBarList.Count)
            {
                if (localP)
                    itemsBarList[progVal].GetComponent<Animation>().Play("progBarOnlineItemUnselected");
                if (progVal == itemsBarList.Count - 1)
                {
                    toMove.GetComponent<Animation>().Play("cursorProgBarOnlineOff");
                    cursorFinished.Add(toMove);
                    //Debug.Log("Curseurs finis : " + (toMove == curseur ? "curseur" : "anotherCurseur"));
                }
            }

            if (progVal + 1 < itemsBarList.Count)
            {
                StartCoroutine(animCurseur(toMove, progVal + 1));
                if (localP)
                    itemsBarList[progBarValue + 1].GetComponent<Animation>().Play("progBarOnlineItemSelected");
            }
        //}
        
    }

    private IEnumerator animCurseur(GameObject toAnim, int progVal)
    {
        allCursorIndex[toAnim] = allCursorIndex[toAnim] + 1;
        toAnim.GetComponent<Animation>().Play("cursorProgBarOnlineMove");
        yield return new WaitForSeconds(1f / 3f);
        toAnim.GetComponent<RectTransform>().anchoredPosition = new Vector3(posItems[progVal], 0, 0);
        RectTransform rT = toAnim.transform.GetChild(0).GetComponent<RectTransform>();
        rT.localPosition = new Vector3(0, rT.localPosition.y, 0);
    }

    public void initProgBar(List<int> othersID)
    {
        nbItem = infinisManager.nbLevelInfinis_online;
        cleanBar();
        for(int i = 0; i < nbItem; i++)
        {
            GameObject item = Instantiate(progItem);
            //item.SetActive(true);
            item.transform.SetParent(transform.GetChild(0));

            item.transform.localScale = progItem.transform.localScale;
            float xPos = -(nbItem * distanceBetItem - distanceBetItem) / 2f
                + i * distanceBetItem;
            posItems.Add(xPos);
            item.GetComponent<RectTransform>().localPosition = new Vector3(xPos, 0, 0);

            // couleure initiales
            if (i != 0)
                item.GetComponent<Image>().color = itemColorNotWin;

            itemsBarList.Add(item);
        }
        StartCoroutine(animInitBar(othersID));
    }

    private IEnumerator animInitBar(List<int> othersID)
    {
        // anim pop de la barre
        foreach(GameObject i in itemsBarList)
        {
            i.SetActive(true);
            i.GetComponent<Animation>().Play("progBarOnlineItemPop");
            yield return new WaitForSeconds(0.08f);
        }

        curseur.GetComponent<RectTransform>().anchoredPosition = new Vector3(posItems[0], 0, 0);
        curseur.GetComponent<Animation>().Play("cursorProgBarOnlineOn");
        allCursorIndex.Add(curseur, 0);

        itemsBarList[0].GetComponent<Animation>().Play("progBarOnlineItemSelected");
        initOthersCursor(othersID);
    }

    public IEnumerator animCleanBar()
    {
        for(int i = 0; i < itemsBarList.Count; i++)
        {
            int index = itemsBarList.Count - 1 - i;
            GameObject item = itemsBarList[index];
            item.GetComponent<Animation>().Play("progBarOnlineItemUnpop");


            // si le curseur local se situe a cet endroit, on le off
            if (eliminatedGo != curseur
                && allCursorIndex.ContainsKey(curseur) && allCursorIndex[curseur] == index
                && !cursorFinished.Contains(curseur))
            {
                curseur.GetComponent<Animation>().Play("cursorProgBarOnlineOff");
                allCursorIndex.Remove(curseur);
            }

            else if (allCursorIndex.ContainsValue(index))
            {
                foreach (KeyValuePair<GameObject, int> c in allCursorIndex)
                    if (c.Value == index && !cursorFinished.Contains(c.Key))
                        c.Key.GetComponent<Animation>().Play("cursorProgBarOnlineOff");
            }

            yield return new WaitForSeconds(1/12f);
            //item.SetActive(false);
        }
        eliminatedGo = null;
        cleanBar();
    }


    private void cleanBar()
    {
        foreach(GameObject o in itemsBarList)
        {
            Destroy(o);
        }
        foreach(KeyValuePair<int, GameObject> oC in othersCursor)
        {
            Destroy(oC.Value);
        }
        othersCursor = new Dictionary<int, GameObject>();
        itemsBarList = new List<GameObject>();
        cursorFinished = new List<GameObject>();
        allCursorIndex = new Dictionary<GameObject, int>();
        progBarValue = 0;
    }



    Dictionary<int, GameObject> othersCursor = new Dictionary<int, GameObject>();
    private List<GameObject> cursorFinished = new List<GameObject>();
    private Dictionary<GameObject, int> allCursorIndex = new Dictionary<GameObject, int>();


    public GameObject curseurOtherGo;

    private List<int> test_ListId = new List<int>() { 1, 2, 3 };
    private void initOthersCursor(List<int> ids)
    {
        foreach(int id in ids)
        {
            GameObject item = Instantiate(curseurOtherGo);
            item.SetActive(true);
            item.transform.SetParent(transform);

            item.transform.localScale = curseurOtherGo.transform.localScale;
            item.GetComponent<RectTransform>().anchoredPosition = new Vector3(posItems[0], 0, 0);
            item.GetComponent<Animation>().Play("cursorProgBarOnlineOn");

            othersCursor.Add(id, item);
            allCursorIndex.Add(item, 0);
        }
    }

    // appelé seulement par ghost quand franchi arrivé
    // alors forceQuit tjr egal false
    public void moveOtherCursor(int id, int lvl)
    {
        GameObject toMove = othersCursor[id];
        progBarPassNext(toMove, lvl - 1);
    }

    private GameObject eliminatedGo;
    public void aPlayerIsEliminated(int id, bool itsMe)
    {
        GameObject cursor = itsMe ? curseur : othersCursor[id];
        eliminatedGo = cursor;
        StartCoroutine(destroyCursorLeftPlayer(cursor, id));
    }

    public void aPlayerLeftGame(int id) // appelé seulement si left sans avoir finit
    {
        GameObject toSupp = othersCursor[id];    
        StartCoroutine(destroyCursorLeftPlayer(toSupp, id));
    }

    private IEnumerator destroyCursorLeftPlayer(GameObject toSupp, int id)
    {
        toSupp.GetComponent<Animation>().Play("cursorProgBarOnlineOff");
        yield return new WaitForSeconds(1 / 6f);
        if (toSupp != curseur)
        {
            othersCursor.Remove(id);
            Destroy(toSupp);
        }
        allCursorIndex.Remove(toSupp);
    }
}
