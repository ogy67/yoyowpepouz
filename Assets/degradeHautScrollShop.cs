using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class degradeHautScrollShop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public RectTransform[] contentTab;
    private int prevIndexCentered = -1;
    private float prevPosY = -20000;
    // Update is called once per frame
    void Update()
    {
        if (shopManager.menuShopOpen)
        {
            // regarde quel est le scroll centré
            int indexCentered = doubleScrollShop.currentIndexCentered;
            float posY = contentTab[indexCentered].localPosition.y;
            if (indexCentered == prevIndexCentered)
            {
                // juste regarder si pos a changé
                if (prevPosY == -20000
                    || contentTab[indexCentered].localPosition.y != prevPosY)
                {
                    prevPosY = posY;
                    scrollSectionChangePos(prevPosY);
                }
            }
            else // a changé de scroll
            {
                prevIndexCentered = indexCentered;
                prevPosY = posY;
                scrollSectionChangePos(prevPosY);
            }
        }
    }


    public float posYActivateDegrade = 20f;
    private bool degradeActivated = false;
    private void scrollSectionChangePos(float currentPosY)
    {
        // si menu activé

        if(currentPosY > 20f)
        {
            if (!degradeActivated)
            {
                degradeActivated = true;
                activateDegrade();
            }
        }
        else
        {
            if (degradeActivated)
            {
                degradeActivated = false;
                desactivateDegrade();
            }
        }
    }

    public Animation animDegrade;
    // appelé quand scroll actuel descend en dessous de limite degrade
    private void activateDegrade()
    {
        animDegrade.Play("degradeShopOn");
    }

    private void desactivateDegrade()
    {
        animDegrade.Play("degradeShopOff");
    }

    public Image degradeImg;
    public void clickVoyantGoldGem()
    {
        // cash reactiver degrade
        degradeActivated = true;
        degradeImg.color = new Color(0, 0, 0, 255);
    }








    // drag change section
    public void playerDragHorizontal()
    {
        // desactiver degrade si actif
    }

    // avec btnSection
    public void changeSection()
    {
        // desactiver degrade si actif
    }

}
