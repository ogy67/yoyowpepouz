using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animFondStar : MonoBehaviour
{

    /*public bool testAnimFond = false;
    // Update is called once per frame
    void Update()
    {
        if (testAnimFond)
        {
            testAnimFond = false;
            playAnim();
        }
    }*/

    private bool isPlayingAnimFond = false;

    public void playAnim()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        isPlayingAnimFond = true;
        StartCoroutine(playAnimFondStar());
    }

    public void stopAnim()
    {
        isPlayingAnimFond = false;
    }

    public GameObject itemPopInstance;
    public Transform parentItemsPoped;
    public Transform[] posTab; // 11
    public int nbPopToBeReusable = 6;
    private List<Transform> fileAttente = new List<Transform>();

    private float timeBetweenPop = 0.25f;
    private float diffTimeMax = 0.05f;

    private List<Transform> freePosList = new List<Transform>();
    private List<Transform> usedPosList = new List<Transform>();

    private bool initialized = false;

    private void initialize()
    {
        foreach (Transform t in posTab)
        {
            freePosList.Add(t);
        }
    }
    
    private IEnumerator playAnimFondStar()
    {
        if (!initialized)
        {
            initialize();
            initialized = true;
        }

        while (isPlayingAnimFond)
        {
            GameObject item = Instantiate(itemPopInstance, parentItemsPoped);
            item.SetActive(true);
            item.GetComponent<Animation>().Play("animStarItemPop");

            int indexPos = Random.Range(0, freePosList.Count);
            //while(usedPosList.Contains(posTab[indexPos]))
            //    indexPos = Random.Range(0, posTab.Length);

            //Debug.Log("Nb item free : " + freePosList.Count + ", index want : " + indexPos);
            Vector2 posPop = freePosList[indexPos].position;
            item.transform.position = posPop;
            usedPosList.Add(freePosList[indexPos]);
            fileAttente.Add(freePosList[indexPos]);
            freePosList.RemoveAt(indexPos);

            if (fileAttente.Count == nbPopToBeReusable)
            {
                usedPosList.Remove(fileAttente[0]);
                freePosList.Add(fileAttente[0]);
                fileAttente.RemoveAt(0);
            }

            float timeWait = timeBetweenPop + Random.Range(-diffTimeMax, +diffTimeMax);

            StartCoroutine(destroyItemLate(item));
            yield return new WaitForSeconds(timeWait);
        }
    }

    public float timeAnimPop = 1f;
    private IEnumerator destroyItemLate(GameObject item)
    {
        yield return new WaitForSeconds(1f);
        Destroy(item);
    }
}
