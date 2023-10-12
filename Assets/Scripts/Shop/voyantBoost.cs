using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct bonusImage
{
    public Transform imageAssocie;
    public int id;
    public typeRarety tR;
}

[System.Serializable]
public struct colorByRarety
{
    public Transform contourAssocie;
    public typeRarety tR;
    public Color32 color;
}
public class voyantBoost : MonoBehaviour
{
    public Transform emptyBord;
    public bonusImage[] infosImages;
    public colorByRarety[] infosFonds;
    public Animation myAnim;

    public bool voyantSP;


    public void openShopCleanAnimVoyant()
    {
        // prevent BUG , change boost -> ferme cash shop -> réouverture : anim bloquée
        myAnim.Play("initVoyantBoost");
    }

    private void clearVoyant()
    {
        foreach (bonusImage bI in infosImages)
        {
            bI.imageAssocie.gameObject.SetActive(false);
        }
        if (!voyantSP)
        {
            foreach (colorByRarety cR in infosFonds)
            {
                cR.contourAssocie.gameObject.SetActive(false);
            }
        }
    }

    private void deleteVoyant()
    {
        foreach (bonusImage bI in infosImages)
        {
            bI.imageAssocie.gameObject.SetActive(false);
        }
        if (!voyantSP)
        {
            foreach (colorByRarety cR in infosFonds)
            {
                cR.contourAssocie.gameObject.SetActive(
                    cR.tR != typeRarety.none ? false : true);
            }
        }
        currentItemBoost = null;
    }

    public int getCurrentId()
    {
        return currentVoyantId;
    }
    private int currentVoyantId = -1;


    public Image flouAnimImg, ondeAnimImg;
    //public Animation voyantAnim;
    public itemBoost currentItemBoost;
    public void setVoyant(int id, bool animate, itemBoost iB)
    {
        //Debug.Log("Set voyant, boostItem : " + iB.name);
        currentVoyantId = id;
        currentItemBoost = iB;
        clearVoyant();
        Color c = emptyBord.GetChild(0).GetComponent<Image>().color;

        if (voyantSP)
            emptyBord.GetChild(0).GetComponent<Image>().color 
                = new Color(c.r, c.g, c.b, id == -1 ? 150 / 255f : 1f);

        if (id == -1) // vide
        {
            // ouvrir image vide en fonction emplacement
            if (!voyantSP) // contour voyant SP tjr ouvert; 
            {
                emptyBord.gameObject.SetActive(true);
            }
            return;
        }

        // fermer prev open

        // on cherche l'image bonus à ouvrir
        bonusImage toOpen = new bonusImage();
        foreach (bonusImage bI in infosImages)
        {
            if (bI.id == id)
            {
                toOpen = bI;
                toOpen.imageAssocie.gameObject.SetActive(true);
                toOpen.imageAssocie.GetComponent<Image>().color = getColorByRarety(bI);
                break;
            }
        }

        // on cherche l'image contour à ouvrir 
        foreach (colorByRarety cR in infosFonds)
        {
            if (cR.tR == toOpen.tR)
            {
                cR.contourAssocie.gameObject.SetActive(true);

                if (animate)
                {
                    flouAnimImg.color = cR.color;
                    ondeAnimImg.color = cR.color;
                    myAnim.Play("voyantBoost");
                }
            }
        }
    }

    private Color32 getColorByRarety(bonusImage bI)
    {
        foreach (colorByRarety cR in infosFonds)
        {
            if (cR.tR == bI.tR)
            {
                return cR.color;
            }
        }
        return new Color32();
    }




    public GameObject voyantDelete;
    private int phaseDelete = 0;
    /*
     * void btnDeleteBoost()
     * IEnumerator closeVoyantDeleteLate()
    */
    #region delete boost
    public void btnDeleteBoost()
    {
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_voyantBoost);
        if (currentVoyantId == -1)
            return;

        if (phaseDelete == 0)
        {
            phaseDelete++;
            voyantDelete.SetActive(true);
            StartCoroutine(closeVoyantDeleteLate());
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.btn_preDeleteBoost);
        }
        else // phaseDelete == 1 : confirme supprimer
        {
            //if(currentItemBoost != null)
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.btn_deleteBoost);
            boostManager.getInstance().boostIsDeleted(GetComponent<voyantBoost>());
            //currentItemBoost.itemBoostDeletedFromUsed();

            voyantDelete.SetActive(false);
            phaseDelete--;
            // clean le voyant
            currentVoyantId = -1;
            deleteVoyant();
            boostManager.getInstance().btnDeleteItem(GetComponent<voyantBoost>());

            if (voyantSP)
            {
                Color c = emptyBord.GetChild(0).GetComponent<Image>().color;
                emptyBord.GetChild(0).GetComponent<Image>().color
                    = new Color(c.r, c.g, c.b, 150 / 255f);
            }
        }
    }

    private IEnumerator closeVoyantDeleteLate()
    {
        yield return new WaitForSeconds(1.5f);
        if (phaseDelete == 1) // n'a pas recliqué sur le bouton delete
        {
            voyantDelete.SetActive(false);
            phaseDelete--;
        }
    }

    #endregion
}
