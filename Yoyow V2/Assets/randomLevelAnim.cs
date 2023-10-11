using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct levelOnlineInfo
{
    public int idLevel;
    public GameObject associatedGo;
}
public class randomLevelAnim : MonoBehaviour
{
    private int prevLevelPlayed = -1;
    public int getNewLevel()
    {
        int ret;
        do
        {
            ret = Random.Range(minIdLevel, maxIdLevel + 1);
        } while (ret == prevLevelPlayed);
        prevLevelPlayed = ret;
        return ret;
    }

    public levelOnlineInfo[] infosLevels;
    public float timeSearch = 2f;
    private int finalIdLevel;

    public void animateRandomLevel(int idLevel)
    {
        // on anim pendant 2 secondes puis affiche level choisit
        finalIdLevel = idLevel;
        StartCoroutine(animLevel());
    }

    public Animation switchAnim;
    //public float timeAnimSwitch;
    public float delayInit = 0.5f;
    private IEnumerator animLevel()
    {
        yield return new WaitForSeconds(delayInit);
        float init_t = Time.time;
        float time_anim = switchAnim["switchNormal"].length;
        while (Time.time - init_t < timeSearch)
        {
            // on change le niveau affiché
            displayLevel(false, (timeSearch - (Time.time - init_t)) < time_anim);
            yield return null; // attendre que l'anim soit bien lancée
            yield return new WaitUntil(predicate: () => switchAnim.isPlaying == false);
            //yield return new WaitForSeconds(timeAnimSwitch);
        }
        // on attend que l'animation soit finit
        //while (switchAnim.isPlaying) yield return null;

        // => on affiche le niveau final
        displayLevel(true, false);
    }

    public int maxIdLevel, minIdLevel;
    private int prevDisplayedId = -1;
    private GameObject prevDisplayedGo;
    //public menuOnlineManager _menuOnlineManager;
    private void displayLevel(bool finalLevel, bool avoidDisplayFinal)
    {
        if (prevDisplayedGo != null && prevDisplayedGo.activeSelf)
            prevDisplayedGo.SetActive(false);

        GameObject toShow;
        if (finalLevel)
        {
            switchAnim.Play("switchFinal");
            toShow = getLevelGoById(finalIdLevel);
            ///_menuOnlineManager.finalLevelLoaded();
        }
        else
        {
            int newId;

            List<int> avoidIds = new List<int>() { prevDisplayedId};

            // si le level a load après celui load actuel EST final
            // level a load ne peut pas être final
            if (avoidDisplayFinal)
                avoidIds.Add(finalIdLevel);

            do
            {
                newId = Random.Range(minIdLevel, maxIdLevel + 1);
            } while (avoidIds.Contains(newId));

            toShow = getLevelGoById(newId);

            switchAnim.Play("switchNormal");
        }

        prevDisplayedGo = toShow;
        toShow.SetActive(true);
    }

    private GameObject getLevelGoById(int id)
    {
        foreach ( levelOnlineInfo lI in infosLevels)
        {
            if (lI.idLevel == id)
                return lI.associatedGo;
        }
        return null;
    }

    public void resetLevels()
    {
        if (prevDisplayedGo.activeSelf)
            prevDisplayedGo.SetActive(false);
    }

}
