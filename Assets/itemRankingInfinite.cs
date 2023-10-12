using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemRankingInfinite : MonoBehaviour
{

    public Text playerNameText;
    public Text rangPlayerText;
    public Text scorePlayerText;

    public void loadItem(string playerName, int rang, int bestInfinite, bool isLocalPlayer, bool popAnim)
    {
        // check si ce player est le player Local

        setText(playerNameText, playerName, true);
        setText(rangPlayerText, rang.ToString(), true);
        setText(scorePlayerText, intScoreToStringScore(bestInfinite), false);

        StartCoroutine(animPop(popAnim, isLocalPlayer));
    }

    private IEnumerator animPop(bool anim, bool localP)
    {
        if (anim)
        {
            GetComponent<Animation>().Play("itemRankingPop");
            yield return new WaitForSeconds(0.1f);
        }
        if (localP)
        {
            GetComponent<Animation>().Play("itemRankingLocalP");
        }
    }

    private string intScoreToStringScore(int bestInfinite)
    {
        int nbZ = 3 - bestInfinite.ToString().Length;
        string ret = "";
        while (nbZ > 0)
        {
            nbZ--;
            ret += '0';
        }
        return "<color=grey>" + ret + "</color>" + bestInfinite;
    }

    private static void setText(Text t, string s, bool childToo)
    {
        t.text = s;
        if (childToo)
        {
            t.transform.GetChild(0).GetComponent<Text>().text = s;
        }
    }
}
