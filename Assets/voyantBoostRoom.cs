using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct imagePerTypeBoost
{
    public typeBoostGame typeBoost;
    public Transform imageAssociated;
    public Color color;
}
public class voyantBoostRoom : MonoBehaviour
{
    public imagePerTypeBoost[] imgBoost;

    public Image flouImg;
    private Transform prevOpen;
    private Color emptyColor = new Color(0, 0, 0, 0);
    public void setVoyant(typeBoostGame type)
    {
        if(prevOpen != null)
        {
            prevOpen.gameObject.SetActive(false);
            flouImg.color = emptyColor;
        }

        if (type != typeBoostGame.none)
        {
            // on active l'image boost
            int i = 0;
            while (i < imgBoost.Length && imgBoost[i].typeBoost != type) i++;

            prevOpen = imgBoost[i].imageAssociated;
            prevOpen.gameObject.SetActive(true);

            Color c = imgBoost[i].color;
            flouImg.color = new Color(c.r, c.g, c.b, 100 / 255f);
        }
    }
}
