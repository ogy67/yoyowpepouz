using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class superPalet : MonoBehaviour
{
    public bool isHiddenAnim;

    public Image[] imgPalet;

    public superPalet itemShop;
    public Image imgHidden;
    private bool animOn = true;

    // Update is called once per frame
    void Update()
    {
        if (isHiddenAnim)
        {
            // si pas select
            if (!skinManager.superPionOn)
            {
                // si pas dans menu skin : pas d'anim principale
                if (!itemShop.gameObject.activeInHierarchy && animOn)
                {
                    animOn = false;
                    GetComponent<Animation>()["pionMultiColorShop"].speed = 0f;
                }
                // sinon si dans menu skin : anim
                else if(itemShop.gameObject.activeInHierarchy)
                {
                    if (!animOn)
                    {
                        animOn = true;
                        GetComponent<Animation>()["pionMultiColorShop"].speed = 1f;
                    }
                }
                if(animOn)
                    skinManager.getInstance().stopAnim_pionChanged();
            }
            // select
            else
            {
                // si l'anim principale n'est pas active : activer
                if (!animOn)
                {
                    animOn = true;
                    GetComponent<Animation>()["pionMultiColorShop"].speed = 1f;
                }
                
                // animer l'affichage principal
                skinManager.getInstance().animColorSuperPalet(imgHidden.color);
            }
            // si dans menu skin : animer l'item
            if (itemShop.gameObject.activeInHierarchy)
            {
                itemShop.animPaletShop(imgHidden.color);
            }

            if (animRewardPionBuy)
                foreach(Image i in toAnim)
                    i.color = new Color(imgHidden.color.r, imgHidden.color.g, imgHidden.color.b, i.color.a);
        }
    }

    public void animPaletShop(Color c)
    {
        foreach (Image i in imgPalet)
            i.color = new Color(c.r, c.g, c.b, 1f);
    }

    private bool animRewardPionBuy = false;
    private Image[] toAnim;
    public void animPaletBuy(Image[] anim)
    {
        animRewardPionBuy = true;
        toAnim = anim;
        StartCoroutine(waitRewardEnds());
    }

    private IEnumerator waitRewardEnds()
    {
        yield return new WaitUntil(predicate: () => menuRewardSkin.menuRewardOpen == false);
        animRewardPionBuy = false;
    }
}
