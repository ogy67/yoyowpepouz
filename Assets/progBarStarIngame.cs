using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progBarStarIngame : MonoBehaviour
{
    public Image progBar_1, progBar_2;
    public GameObject content;
    public void loadBar()
    {
        if (!infinisManager.infinis_Mode_On
            && expertMode.modeExpertOn)
        {
            // reset les voyants
            content.SetActive(true);
            progBar_1.fillAmount = 0f;
            progBar_2.fillAmount = 0f;
            currLevelProgBar = 0;
        }
        else if(content.activeSelf)
            content.SetActive(false);
    }

    public void playerReplay()
    {
        // reset les voyants
        progBar_1.fillAmount = 0f;
        progBar_2.fillAmount = 0f;
        currLevelProgBar = 0;
    }

    public Animation myAnim;
    private int currLevelProgBar;
    private List<string> nameAnims = new List<string>() { "progBarStar_win1", "progBarStar_win2", "progBarStar_win3" };
    public void playerWinLevel()
    {
        if (expertMode.modeExpertOn)
        {
            // anim progBar avance
            myAnim.Play(nameAnims[currLevelProgBar]);
            currLevelProgBar++;
        }
    }
}
