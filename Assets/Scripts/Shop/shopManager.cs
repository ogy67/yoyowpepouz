using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeM
{
    menu_boost,
    menu_pion,
    menu_piece
}

public class shopManager : MonoBehaviour
{
    private typeM currentMenu = typeM.menu_boost;
    private Dictionary<int, typeM> rangMenu_inverse = new Dictionary<int, typeM>()
    {
        {0, typeM.menu_boost },
            { 2,typeM.menu_piece },
            { 1, typeM.menu_pion }
    };
    private Dictionary<typeM, int> rangBtnMenuByType = new Dictionary<typeM, int>() {
        { typeM.menu_boost, 0 },
        { typeM.menu_pion, 1 },
        { typeM.menu_piece, 2 }
    };

    public void Start()
    {
        GetComponent<boostManager>().initBoostData();
        GetComponent<skinManager>().initValues();
        StartCoroutine(preloadShopMenu());
    }


    public Animation[] btnMenuAnim;
    public doubleScrollShop doubleScroll;
    public void btnSousMenu(int rang_menu)
    {
        typeM tM = rangMenu_inverse[rang_menu];
        if (tM == currentMenu || doubleScrollShop.isSnapingAnotherItem)
            return;

        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.switch_section);
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_section);
        btnMenuAnim[rang_menu].Play("btnMenuShopOn");
        btnMenuAnim[rangBtnMenuByType[currentMenu]].Play("btnMenuShopOff");

        doubleScroll.goToItem_fromShop(rang_menu);
        currentMenu = tM;
    }

    public void endDrag_changeBtnSousMenu(int rang_menu)
    {
        typeM tM = rangMenu_inverse[rang_menu];
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_section);
        if (tM == currentMenu)// || doubleScrollShop.isSnapingAnotherItem)
            return;

        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.switch_section);
        btnMenuAnim[rang_menu].Play("btnMenuShopOn");
        btnMenuAnim[rangBtnMenuByType[currentMenu]].Play("btnMenuShopOff");

        currentMenu = tM;
    }

    public GameObject menuShop;
    public static bool menuShopOpen = false;
    public void btnCloseMenu()
    {
        menuShopOpen = false;
        GetComponent<Animation>().Play("shop_close");

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);

        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_close);
        //performanceManager.instance.closeSousMenuP();
    }

    public GameObject customMenuGo, dailyMenuGo;
    public List<packOffer> packOfferList;
    // fonction spéciale appelée en tout debut game
    // preload menu shop pour pas de latence après
    public IEnumerator preloadShopMenu()
    {
        // fonctionnement actuel
        // load boost
        // load palets
        // load defis
        // puis peut ouvrir menu

        //Debug.Log("preload shop menu ui");
        menuShop.gameObject.SetActive(true);
        defiManager.getInstance().setNbCrown_initGame();

        yield return new WaitForSeconds(0.3f);

        boostManager.getInstance().reloadVoyantsBoost();
        boostManager.getInstance().rebuildLayout();
        boostManager.shouldBeRebuildWhenOpen = false;
        skinManager.getInstance().rebuildLayout();
        skinManager.shouldBeRebuildWhenOpen = false;

        yield return new WaitForSeconds(0.3f);

        foreach (packOffer packOffer in packOfferList)
        {
            packOffer.initAppCheckOfferOn();
        }

        yield return new WaitForSeconds(0.1f);
        Canvas.ForceUpdateCanvases();
        /*if (boostManager.shouldBeRebuildWhenOpen)// && boostManager.getInstance().boostPan.activeInHierarchy)
        {
            boostManager.getInstance().rebuildLayout();
            boostManager.shouldBeRebuildWhenOpen = false;
        }

        if (skinManager.shouldBeRebuildWhenOpen)// && skinManager.getInstance().skinPan.activeInHierarchy)
        {
            skinManager.getInstance().rebuildLayout();
            skinManager.shouldBeRebuildWhenOpen = false;
        }*/
        yield return new WaitForSeconds(0.4f);
        menuShop.gameObject.SetActive(false);

        customMenuGo.SetActive(false);
        dailyMenuGo.SetActive(false);
    }

    public void btnMenuShop(bool clickOnShopButton)
    {
        if (!managerNiveau.gameOn
            && !notifDefiDone.isAnimatingNotif
            && !notifStarUnlocked.isAnimating
            && !notifInfinisUnlocked.isAnimating)
        {
            menuShopOpen = true;
                menuShop.gameObject.SetActive(true);
            boostManager.getInstance().reloadVoyantsBoost();
            doubleScroll.checkIfRebuildLayout(doubleScrollShop.currentIndexCentered);
            GetComponent<Animation>().Play("shop_open");
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);

            if (clickOnShopButton && notifDailyReward.notifDailyOffer_isOn)
            {
                degradeScroll.clickVoyantGoldGem();
                forceGoCurrencySection_onOpen(false, false, true);
            }
        }
    }



    public RectTransform piecePos, gemmePos;
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public Vector2 offset;
    public degradeHautScrollShop degradeScroll;

    public void SnapTo(RectTransform target)
    {
        Vector2 v1 = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position);
        Vector2 v2 = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        Vector2 pos = (v1 - v2);

        pos = new Vector2(0, pos.y - scrollRect.GetComponent<RectTransform>().sizeDelta.y / 2);
        contentPanel.anchoredPosition = pos;

        Canvas.ForceUpdateCanvases();
    }

    public void btnOpenGems()
    {
        openCurrencyMenu(gemmePos, true);
    }

    public void btnOpenCoins()
    {
        openCurrencyMenu(piecePos, false);
    }

    private void openCurrencyMenu(RectTransform toSnap, bool gem)
    {
        if (managerNiveau.gameOn
            || notifDefiDone.isAnimatingNotif || notifInfinisUnlocked.isAnimating || notifStarUnlocked.isAnimating
            || infoBoostManager.upgrading
            || rewardMenu.menuRewardOpen
            || openBoosterManager.boosterMenu_open
            || doubleScrollShop.isSnapingAnotherItem
            || defiManager.menuDefiOpen)
            return;

        if (menuBooster.menuBoosterOpen)
            return;
            //menuInfoBoost.getInstance().closeMenu();
        else if (defiManager.menuDefiOpen)
            defiManager.getInstance().closeMenu();

        if (menuShopOpen)
        {
            if (currentMenu != typeM.menu_piece)
            {
                SnapTo(toSnap);
                btnSousMenu(2);
            }
            return;
        }

        degradeScroll.clickVoyantGoldGem();

        //SnapTo(toSnap);
        btnMenuShop(false);

        forceGoCurrencySection_onOpen(!gem, gem, false);
       
        //SnapTo(toSnap);
    }

    private void forceGoCurrencySection_onOpen(bool line_coin, bool line_gem, bool line_dailyOffer)
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
        if (currentMenu != typeM.menu_piece)
        {
            // dire a doubleScroll de changer de sous menu
            doubleScroll.goToItem_outShop(2);

            // On doit forcer le changement de couleure des boutons du menu
            btnMenuAnim[2].Play("btnMenuShopOn_noDelay");
            btnMenuAnim[rangBtnMenuByType[currentMenu]].Play("btnMenuShopOff_noDelay");
            currentMenu = typeM.menu_piece;
        }
        StartCoroutine(waitSnapToVoyant(line_coin, line_gem, line_dailyOffer));
    }

    public RectTransform posYOfferGemTr, posYOfferCoinTr, posYDailyOfferTr;
    private IEnumerator waitSnapToVoyant(bool line_coin, bool line_gem, bool line_dailyOffer)
    {
        while (!posYOfferGemTr.gameObject.activeInHierarchy)
            yield return null;
        snapScrollToItem.nextSnapIsDirectSnap = true;


        snapScrollToItem.nextSnapIsGemSnap = line_gem;
        snapScrollToItem.nextSnapIsDailyOfferSnap = line_dailyOffer;
        RectTransform rT = line_coin ? posYOfferCoinTr : line_gem ? posYOfferGemTr : posYDailyOfferTr;
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapOffer, rT);
    }

    
    // être sur que fermé même si ferme le shop par dessus anim
    public void animateNoConnec(GameObject noCoGo)
    {
        StartCoroutine(ienumNoCo(noCoGo));
    }

    private IEnumerator ienumNoCo(GameObject noCoGo)
    {
        noCoGo.SetActive(true);
        yield return new WaitForSeconds(1f);
        noCoGo.SetActive(false);
    }
}

// 385 l -> 253 l
// 259 l -> 150 l
