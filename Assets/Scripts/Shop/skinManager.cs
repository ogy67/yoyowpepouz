using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class skinManager : MonoBehaviour
{
    public RectTransform mainContent;

    private static skinManager instance;
    public static skinManager getInstance()
    {
        if (instance == null) instance = GameObject.Find("newShop_test").GetComponent<skinManager>();
        return instance;
    }

    public Transform paletContent, paletContent_unlocked;
    public Transform lesTrailsContent;//, lesFondsContent;
    List<int> data;
    /*
     * void initValues()
     * void initValues_type(typeSave tSave, typeSkin tSkin, Transform content)
     * IEnumerator setParentLate(Transform t, Transform parent)
     * bool skin_isUnlocked(List<object[]> d, int id)
     * Transform getSkinById(typeSkin tS, int id)
    */
    #region initValues

    public void initValues()
    {
        //Debug.Log("FILL SKIN MENU");
        initValues_type(typeSave.SHOP_TRAILS, typeSkin.skin_trail, lesTrailsContent);
        dataSkin.Add(typeSkin.skin_trail, data);
        //initValues_type(typeSave.SHOP_FONDS, typeSkin.skin_fond, lesFondsContent);
        //dataSkin.Add(typeSkin.skin_fond, data);
        initValues_type(typeSave.SHOP_PION, typeSkin.skin_palet, paletContent);
        dataSkin.Add(typeSkin.skin_palet, data);

        laodSkins_init();
    }

    private void initValues_type(typeSave tSave, typeSkin tSkin, Transform content)
    {
        data = (List<int>)SaveSystem.loadData(tSave);

        foreach (Transform t in content)
        {
            itemSkin iS = t.GetComponent<itemSkin>();
            switch (tSkin)
            {
                case (typeSkin.skin_trail):
                    iS.initTrailValues(skin_isUnlocked(data, iS.id_skin));
                    break;
                /*case (typeSkin.skin_fond):
                    iS.initFondValues(
                        skin_isUnlocked(data, iS.id_skin));
                    break;*/
                case (typeSkin.skin_palet):
                    bool isUnlocked = skin_isUnlocked(data, iS.id_skin);
                    iS.initPaletValues(isUnlocked);
                    if (isUnlocked)
                        StartCoroutine(setParentLate(t, paletContent_unlocked));
                    break;
            }
        }
    }

    private IEnumerator setParentLate(Transform t, Transform parent)
    {
        yield return new WaitForSeconds(0.2f);
        t.SetParent(parent);
    }

    private bool skin_isUnlocked(List<int> d, int id)
    {
        foreach (int i in d)
        {
            if (i == id)
            {
                return true;
            }
        }
        return false;
    }

    public Transform getSkinById(typeSkin tS, int id)
    {
        Transform[] list = tS == typeSkin.skin_palet ?
            new Transform[] { paletContent, paletContent_unlocked } :
            new Transform[] { lesTrailsContent };
            //new Transform[] { tS == typeSkin.skin_trail ? lesTrailsContent : lesFondsContent };

        foreach (Transform t in list)
        {
            foreach (Transform t2 in t)
            {
                if (t2.GetComponent<itemSkin>().id_skin == id)
                    return t2;
            }
        }
        return null;
    }
    #endregion


    private Dictionary<typeSkin, typeSave> pathSaveSkin = new Dictionary<typeSkin, typeSave>()
    {
        {typeSkin.skin_palet, typeSave.SHOP_PION },
        {typeSkin.skin_trail, typeSave.SHOP_TRAILS },
        //{typeSkin.skin_fond, typeSave.SHOP_FONDS },
    };
    private Dictionary<typeSkin, List<int>> dataSkin = new Dictionary<typeSkin, List<int>>() { };

    /*
     * void btnBuyItem(int id, typeSkin tS, int price, object[] addValues, Transform caller
        , bool diamonds)
     * void rebuildLayout()
     * 
     * IEnumerator giveSkinLate(Transform caller, typeSkin tS)
     * void getPaletFromDefi(Transform caller, int id)
     * bool validAchat(int montant)
     * void save_unlockItem(int id, typeSkin tS)
    */
    #region achat de skin


    private bool currentSkinPurchaseInCoin;
    private int currentId;
    private typeSkin currentTypeSkin;
    private object[] currentAddValues;
    private Transform currentCaller;


    public void btnBuyItem(int id, typeSkin tS, int price, object[] addValues, Transform caller
        , bool diamonds)
    {
        currentSkinPurchaseInCoin = !diamonds;
        currentId = id;
        currentTypeSkin = tS;
        currentAddValues = addValues;
        currentCaller = caller;

        if (!diamonds)
        {
            if (price <= gameManager.monnaieVal)
                validAchat.getInstance().openMenuValidAchat(true, price, typeAchat.buySkin);
            else
                sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
        }
        else
        {
            if(price <= gemmesManager.nbGemmes)
                validAchat.getInstance().openMenuValidAchat(false, price, typeAchat.buySkin);
            else
                sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
        }
    }

    public void validBuySkin()
    {
        if (currentSkinPurchaseInCoin)
        {
            switch (currentTypeSkin)
            {
                /*case (typeSkin.skin_fond):
                    rewardMenu.getInstance().giveReward(typeReward.fond_jeu,
                        new object[] { currentAddValues[0], -1 });
                    break;*/
                case (typeSkin.skin_palet):
                    menuRewardSkin.getInstance().giveReward(new object[] { currentAddValues[0], currentAddValues[1] }, true);
                    //rewardMenu.getInstance().giveReward(typeReward.palet,
                    //    new object[] { currentAddValues[0], currentAddValues[1] });
                    break;
                case (typeSkin.skin_trail):
                    menuRewardSkin.getInstance().giveReward(new object[] { currentAddValues[0] }, false);
                    //rewardMenu.getInstance().giveReward(typeReward.trail,
                    //    new object[] { currentAddValues[0] });
                    break;
            }
            StartCoroutine(giveSkinLate(currentCaller, currentTypeSkin, currentId));
        }
        else
        {
            if (typeSkin.skin_palet == currentTypeSkin)
            {
                //if (!gemmesManager.validAchat(price))
                //return;
                menuRewardSkin.getInstance().giveReward(new object[] { currentAddValues[0], currentAddValues[1] }, true);
                //rewardMenu.getInstance().giveReward(typeReward.palet,
                //           new object[] { currentAddValues[0], currentAddValues[1] });
                StartCoroutine(giveSkinLate(currentCaller, currentTypeSkin, currentId));
            }
            /*else if (typeSkin.skin_fond == currentTypeSkin)
            {
                //if (!gemmesManager.validAchat(250))
                //    return;
                rewardMenu.getInstance().giveReward(typeReward.fond_jeu,
                           new object[] { currentAddValues[0], currentAddValues[1] });
                StartCoroutine(giveSkinLate(currentCaller, currentTypeSkin, currentId));
            }*/
        }
    }

    public Transform skin_packDebutant;
    public Transform skin_packTikTok;
    // appelé par openOffertPack
    public void giveSkinPackOffer(typePackOffer typePack)
    {
        if(typePack == typePackOffer.pack_debutant)
        {
            itemSkin iS = skin_packDebutant.GetComponent<itemSkin>();
            // info_palet.paletSprite, info_palet.paletTrailColor };
            menuRewardSkin.getInstance().giveReward(new object[] {iS.info_palet.paletSprite, iS.info_palet.paletTrailColor}, true);
            StartCoroutine(giveSkinLate(skin_packDebutant, typeSkin.skin_palet, iS.id_skin));
        }
        else if( typePack == typePackOffer.pack_gratuitTikTok)
        {
            itemSkin iS = skin_packTikTok.GetComponent<itemSkin>();
            // info_palet.paletSprite, info_palet.paletTrailColor };
            menuRewardSkin.getInstance().giveReward(new object[] { iS.info_palet.paletSprite, iS.info_palet.paletTrailColor }, true);
            StartCoroutine(giveSkinLate(skin_packDebutant, typeSkin.skin_palet, iS.id_skin));
        }
    }




    public GameObject skinPan;
    public static bool shouldBeRebuildWhenOpen = true; // initialement
    public void rebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainContent);
    }

    private IEnumerator giveSkinLate(Transform caller, typeSkin tS, int id) // attendre que menu reward open
    {
        yield return new WaitForSeconds(0.5f);
        caller.GetComponent<itemSkin>().unlockItem();

        if (tS == typeSkin.skin_palet)
            caller.SetParent(paletContent_unlocked);
        caller.gameObject.SetActive(true); // les skin d'offre de pack son désactivés par défaut

        rebuildLayout();
        //Canvas.ForceUpdateCanvases();

        setSkin(tS, caller, false, true);

        // on save dans la liste associé
        save_unlockItem(id, tS);
    }

    public void getPaletFromDefi(Transform caller, int id)
    {
        caller.GetComponent<itemSkin>().initPaletValues(true);

        caller.SetParent(paletContent_unlocked);

        shouldBeRebuildWhenOpen = true;
        //Canvas.ForceUpdateCanvases();

        setSkin(typeSkin.skin_palet, caller, false, true);

        // on save dans la liste associé
        save_unlockItem(id, typeSkin.skin_palet);
    }

    public bool validAchat_skin(int montant) // retourne true si l'achat a été validé
    {
        if (montant <= gameManager.monnaieVal)
        {
            StartCoroutine(gameManager.getInstance().debiteMonney(montant));
            return true;
        }
        return false;
    }

    private void save_unlockItem(int id, typeSkin tS)
    {
        dataSkin[tS].Add(id);
        SaveSystem.Save(pathSaveSkin[tS], dataSkin[tS]);
        indexSkin[tS] = id;
        saveSkin_id();
        //Debug.Log("PLAYER UNLOCK PALET ID : " + id);
    }

    #endregion


    public Image affichagePlayerImg;
    public Transform lesTrails_affichage;
    public Image affichageFondImg;

    public GameObject player;
    // 0 : pion , 1 : trail , 2 : fond
    private Dictionary<typeSkin, int> indexSkin;
    /*
     * void laodSkins_init()
     * void saveSkin_id()
     * void setSkin(typeSkin tS, Transform caller, bool init, bool save)
     * void setPalet(Color32 cDeb, Color32 cFin, Sprite s)
     * void setTrailColor(Color32 cDeb, Color32 cFin)
     * void setCurrentTrail(int index)
    */
    #region load save data / set skin

    // initialise l'affichage de player dans menu shop
    // initialise les valeures de player ingame
    // appelé une seule fois en debut de game
    private void laodSkins_init()
    {
        List<int> selectedSkins_id = (List<int>)SaveSystem.loadData(typeSave.SHOP_SELECTED_SKINS);

        if (selectedSkins_id.Count == 0)
        {
            selectedSkins_id = new List<int>() { 1, 1, 1 };
            SaveSystem.Save(typeSave.SHOP_SELECTED_SKINS, selectedSkins_id);
        }
        indexSkin = new Dictionary<typeSkin, int>()
        {
            {typeSkin.skin_palet, selectedSkins_id[0] },
            {typeSkin.skin_trail, selectedSkins_id[1] },
            //{typeSkin.skin_fond, selectedSkins_id[2] },
        };

        foreach(typeSkin tS in new typeSkin[] {
            typeSkin.skin_palet
            , typeSkin.skin_trail })
            //, typeSkin.skin_fond})
        {
            setSkin(tS, getSkinById(tS, indexSkin[tS]), true, false);
        }
        //saveSkin_id();
    }

    private void saveSkin_id()
    {
        SaveSystem.Save(typeSave.SHOP_SELECTED_SKINS, new List<int>() {
                indexSkin[typeSkin.skin_palet]
                , indexSkin[typeSkin.skin_trail]});
        //, indexSkin[typeSkin.skin_fond]});
    }

    public int superPionId;
    public static bool superPionOn;//, fondBuyedOn;
    //public Transform animSelected;
    public void setSkin(typeSkin tS, Transform caller, bool init, bool save)
    {

        int id = caller.GetComponent<itemSkin>().id_skin;
        //Debug.Log("Set skin : " + tS + ", id :" + id +", curId:"+indexSkin[tS]);
        if (!init && id == indexSkin[tS])
            return;

        /*if (!init)
        {
            animSelected.position = caller.position;
            animSelected.GetComponent<Image>().color = caller.GetChild(0).GetComponent<Image>().color;
            animSelected.GetComponent<Animation>().Play("itemSkinSelected");
        }*/
       // Debug.Log("Passe");
        indexSkin[tS] = id;
        if(save)
            saveSkin_id();

        // on active le curseur
        itemSkin iS = caller.GetComponent<itemSkin>();
        iS.setCursorState(true);

        switch (tS)
        {
            case typeSkin.skin_palet:               
                superPionOn = iS.id_skin == superPionId;
                Color32 cDeb = iS.info_palet.paletTrailColor;

                setPalet(cDeb,
                    new Color32(cDeb.r, cDeb.g, cDeb.b, 0),
                    iS.info_palet.paletSprite);              
                break;
            case typeSkin.skin_trail:
                trailManager.setType(iS.id_skin - 1);
                setCurrentTrail(iS.id_skin - 1); 
                break;
            /*case typeSkin.skin_fond:
                fondBuyedOn = iS.type_pay == typePay.diamond;
                cameraGame.colorChooseShop = iS.info_fond.fondColorGame;
                affichageFondImg.color = iS.info_fond.fondColorMenu;
                break;*/
        }
    }

    public Image glowPlayer_affichage;
    private void setPalet(Color32 cDeb, Color32 cFin, Sprite s)
    {
        affichagePlayerImg.sprite = s;
        player.GetComponent<playerMovment>().mySprite = s;
        player.GetComponent<playerMovment>().glowSprite.color =
            new Color32(cDeb.r, cDeb.g, cDeb.b, 100);
        //Debug.Log("Set color glow : "+cDeb);
        player.GetComponent<playerMovment>().mySpriteRenderer.sprite = s;
        glowPlayer_affichage.color = new Color((float)cDeb.r / 255
                    , (float)cDeb.g / 255, (float)cDeb.b / 255, 1);
        //player.GetComponent<SpriteRenderer>().sprite = s;
        setTrailColor(cDeb, cFin);
    }
    private void setTrailColor(Color32 cDeb, Color32 cFin)
    {
        Color c;
        foreach (Transform t in lesTrails_affichage)
        {
            foreach (Transform t2 in t)
            {
                c = t2.GetComponent<Image>().color;
                if (!(c.r == 0 && c.g == 0 && c.b == 0 ))
                t2.GetComponent<Image>().color = new Color((float)cDeb.r / 255
                    , (float)cDeb.g / 255, (float)cDeb.b / 255, t2.GetComponent<Image>().color.a);
            }
        }
        trailManager.setTrailColor(cDeb, cFin);
    }

    private int currentTr_affichage = 0;
    private void setCurrentTrail(int index)
    {
        //Debug.Log("Set current trail");
        for(int i = 0; i < lesTrails_affichage.childCount; i++)
        {
            lesTrails_affichage.GetChild(i).gameObject.SetActive(i == index);
        }
        currentTr_affichage = index;
    }

    public int getIdPionUsed()
    {
        return indexSkin[typeSkin.skin_palet];
    }

    #endregion

    /*public int getRangFond()
    {
        return indexSkin[typeSkin.skin_fond] - 1;
    }*/

    // appelé par le palet même, transfert sa couleure de son anim
    public void animColorSuperPalet(Color next_color)
    {
        Transform toAnim = lesTrails_affichage.GetChild(currentTr_affichage);
        Color c;
        foreach (Transform t in toAnim.transform)
        {
            c = t.GetComponent<Image>().color;
            if (!(c.r == 0 && c.g == 0 && c.b == 0))
                t.GetComponent<Image>().color = new Color(next_color.r, next_color.g, next_color.b, t.GetComponent<Image>().color.a);
        }
        affichagePlayerImg.color = next_color;
        glowPlayer_affichage.color = next_color;
    }

    public void stopAnim_pionChanged()
    {
        affichagePlayerImg.color = Color.white;
    }
}
