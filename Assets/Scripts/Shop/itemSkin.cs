using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeSkin
{
    skin_palet,
    skin_trail
    //skin_fond
}

public enum typePay
{
    ad,
    yow,
    diamond,
    ingame,
    none,
    money,
}

[System.Serializable]
public struct skinInfo_palet
{
    public Sprite paletSprite;
    public Color32 paletTrailColor;
    public int diamondPrice;
    public Transform imgPaletTr;
    //public int yow_price;
}

[System.Serializable]
public struct skinInfo_fond
{
    //public Sprite paletSprite;
    public Color32 fondColorMenu;
    public Color32 fondColorUnlock;
    public Color32 fondColorGame;
    //public int yow_price;
}

public class itemSkin : MonoBehaviour
{
    public int id_skin;
    public typeSkin type_skin;
    public typePay type_pay;
    public Transform lock_cadenas, lock_monney;
    public GameObject curseurSelected;
    public Animation clickAnim;

    private bool is_unlocked = true;
    //private static int trailPrice_yow = 150;

    private Dictionary<typeSkin, int> priceSkin = new Dictionary<typeSkin, int>()
    {
        {typeSkin.skin_palet, 150 },
        {typeSkin.skin_trail, 200 },
        //{typeSkin.skin_fond, 150 },
    };

    [Header("Si palet")]
    public skinInfo_palet info_palet;

    [Header("Si fond")]
    public skinInfo_fond info_fond;

    [Header("Si trail")]
    public trailType tT;



    private static Dictionary<typeSkin, itemSkin> selectedSkins_byType = new Dictionary<typeSkin, itemSkin>()
    {
        //{typeSkin.skin_fond, null },
        {typeSkin.skin_palet, null },
        {typeSkin.skin_trail, null }
    };
    public void setCursorState(bool selected)
    {
        curseurSelected.gameObject.SetActive(selected);

        // si on est l'item qui se fait select
        if(selected)
        {
            // on desactive le selected precedent si pas null
            if(selectedSkins_byType[type_skin] != null)
                selectedSkins_byType[type_skin].setCursorState(false);
            // on devient l'item selected
            selectedSkins_byType[type_skin] = GetComponent<itemSkin>();
        }

    }
    public void btnSelectItem()
    {
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_itemSkin);
        clickAnim.Play("itemSkinClicked");
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapSkin, GetComponent<RectTransform>());

        if (is_unlocked)
        {
            sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.click_item);
            skinManager.getInstance().setSkin(type_skin, transform, false, true);
        }
        else  // acheter
        {
            if (type_pay == typePay.ingame)
            {
                sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                return;
            }               

            else if (type_pay == typePay.yow) // peut beuguer, certains item ont none en typePay
            {
                object[] n = new object[] { };
                switch (type_skin)
                {
                    case typeSkin.skin_palet:
                        n = new object[] { info_palet.paletSprite, info_palet.paletTrailColor };
                        break;
                    case typeSkin.skin_trail:
                        n = new object[] { tT };
                        break;
                    /*case typeSkin.skin_fond:
                        n = new object[] { (Color)info_fond.fondColorUnlock, id_skin };
                        break;*/
                }
                skinManager.getInstance().btnBuyItem(id_skin, type_skin, priceSkin[type_skin], n, transform, false);
            }

            else if(type_pay == typePay.diamond)
            {
                object[] n = new object[] { };
                if (typeSkin.skin_palet == type_skin)
                {
                    n = new object[] { info_palet.paletSprite, info_palet.paletTrailColor };
                }/*else if(typeSkin.skin_fond == type_skin)
                {
                    n = new object[] { (Color)info_fond.fondColorUnlock, id_skin };
                   
                }*/
                skinManager.getInstance().btnBuyItem(id_skin, type_skin, info_palet.diamondPrice, n, transform, true);
            }
        }
    }



    public void initPaletValues(bool isUnlocked)
    {
        is_unlocked = isUnlocked;
        //Debug.Log("Palet d'id " + id_skin + " is unlocked : " + isUnlocked);
        if (isUnlocked)
        {
            if (lock_monney != null)
                lock_monney.gameObject.SetActive(false);
            else if (lock_cadenas != null)
                lock_cadenas.gameObject.SetActive(false);
            gameObject.SetActive(true);
            // mettre l'image palet au bon emplacement
            //Vector2 v = imgPaletTr.transform.localPosition;
            //info_palet.imgPaletTr.localPosition = new Vector2(0, 0);
        }
    }

    public void initTrailValues(bool isUnlocked)
    {
        if (type_pay == typePay.none)
            isUnlocked = true;
        /*if(type_skin == typeSkin.skin_palet)
            info_palet.imgPaletTr.localPosition = new Vector2(0, 0);*/
        is_unlocked = isUnlocked;
        if (isUnlocked)
            lock_monney.gameObject.SetActive(false);
    }

    public void initFondValues(bool isUnlocked)
    {
        if (type_pay == typePay.none)
            isUnlocked = true;
        is_unlocked = isUnlocked;
        if (isUnlocked)
            lock_monney.gameObject.SetActive(false);
    }

    public void unlockItem()
    {
        // on appel initTrail value, fonction fonctionne pour les fonds et les palets
        initTrailValues(true);
    }
}
