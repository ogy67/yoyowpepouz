using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeReward
{
    monney,
    palet,
    tickets_normal,
    tickets_infinis,
    trail,
    fond_jeu,
    gemmes,
    none
}

public class rewardMenu : MonoBehaviour
{
    private static rewardMenu instance;
    public static rewardMenu getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("recompenseMenu").GetComponent<rewardMenu>();
        return instance;
    }

    public Image pionImg, trailImg, fondJeuImg;
    public GameObject pionReward, /*coinReward, gemmeReward, ticketsReward_normal,*/ ticketsReward_infinis
        , fondReward, trailReward;
    private Animation my_anim;

    private Dictionary<typeReward, GameObject> type_go;

    private Dictionary<trailType, int> indexTrails = new Dictionary<trailType, int>() {
        { trailType.circle_line, 4 },
        { trailType.fore_line, 3 },
        { trailType.two_line_spiral, 1 },
        { trailType.shadow_line, 5 },
        { trailType.two_line, 2 },
        { trailType.fleche_line, 6 },
    };

    private void Start()
    {
        type_go = new Dictionary<typeReward, GameObject>()
        {
            { typeReward.palet, pionReward },
            { typeReward.tickets_infinis, ticketsReward_infinis },
            { typeReward.trail, trailReward },
            { typeReward.fond_jeu, fondReward }
        };
        my_anim = GetComponent<Animation>();
    }

    private typeReward prevReward = typeReward.none;
    private void openMenu(typeReward tR)
    {
        menuRewardOpen = true;

        if (prevReward != typeReward.none)
            type_go[prevReward].gameObject.SetActive(false);

        prevReward = tR;
        my_anim.Play("openMenuReward");
        type_go[tR].SetActive(true);
        type_go[tR].GetComponent<Animation>().Play("initReward");

        if (tR == typeReward.trail) // met on le bon trail et met off les autres
        {
            foreach (trailType tT in new trailType[] {
                trailType.two_line, trailType.circle_line, trailType.two_line_spiral,
                trailType.shadow_line, trailType.fore_line})
            {
                trailReward.transform.GetChild(indexTrails[tT]).gameObject.SetActive(tT == (trailType)dataRec[0]);
            }
        }
        else if(tR == typeReward.palet)
        {
            Sprite _paletImg = (Sprite)dataRec[0];
            Color32 _trailColor = (Color32)dataRec[1];
            if (_paletImg == pionBuySprite)
                hiddenAnimPionSpecial.animPaletBuy(new Image[] { pionImg, trailImg });
            pionImg.sprite = _paletImg;
            trailImg.color = _trailColor;
        }
        /*else if(tR == typeReward.fond_jeu)
        {
            if ((int)dataRec[1] == iS_superFond.id_skin)
                hiddenAnimFondSpecial.animFondBuy(new Image[] { fondJeuImg });
            else
            {
                fondJeuImg.color = (Color)dataRec[0];
            }
        }*/
    }

    public itemSkin iS_superFond;
    //public superFond hiddenAnimFondSpecial;
    public superPalet hiddenAnimPionSpecial;
    public Sprite pionBuySprite;

    private void closeMenu()
    {
        my_anim.Play("closeMenuReward");
        menuRewardOpen = false;
    }



    private object[] dataRec;

    /*public void giveReward(typeReward tR, object[] data)
    {
        gettingReward = false;
        dataRec = data;
        openMenu(tR);
    }*/

    public void btnReccupReward()
    {
        if (!gettingReward)
        {
            //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_getReward);
            gettingReward = true;
            StartCoroutine(animReccupRec());
        }
    }

    private bool gettingReward = false;
    public static bool menuRewardOpen = false;

    private IEnumerator animReccupRec()
    {    
        type_go[prevReward].GetComponent<Animation>().Play("giveReward");

        switch (prevReward)
        {
            case (typeReward.tickets_infinis): // validé
                yield return new WaitForSeconds(0.4f);
                break;
            case (typeReward.palet): // validé
                
                yield return new WaitForSeconds(0.4f);
                break;
            case (typeReward.trail): // validé
                trailType _trailType = (trailType)dataRec[0];
                yield return new WaitForSeconds(0.4f);
                break;
            case (typeReward.fond_jeu): // validé
                //Color32 _fondColor = (Color32)dataRec[0];
                yield return new WaitForSeconds(0.5f);
                break;
            default:
                break;
        }

        closeMenu();
    }

}
