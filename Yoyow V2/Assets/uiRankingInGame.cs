using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiRankingInGame : MonoBehaviour
{
    private Transform toFollow;
    public Text rankingText;

    public void setRankingText(int rangPlayer)
    {
        rankingText.text = rangPlayer.ToString();
    }

    public void intializeRankingUI(Transform ghostToFollow)
    {
        setRankingText(0);
        GetComponent<followTarget>().target = ghostToFollow;
        // on met tout transparent
        changeVisibility(false);
        toFollow = ghostToFollow;
    }

    public void changeVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void gameEnd()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        // appelé quand le ghost_bomb d'un autre joueur "meurt"
        if(toFollow == null)
        {
            Destroy(gameObject);
        }
    }
}
