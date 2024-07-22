using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuUpgradeBoost : MonoBehaviour
{
    /*private bool testMode;
   [Header("TEST")]
    public int nbUpgrades_test = 1;
    public int currentLevel_test;
    public typeBoostGame typeBoost_test;
    public typeUpgrade typeItemUp1_test, typeItemUp2_test;
    public float currValItem1_test, currValItem2_test;
    public float currAddedItem1_test, currAddedItem2_test;
    public bool btnOpenMenu_test = false;

    /*
     * Update()
    /
    #region TEST
    // Update is called once per frame
    void Update()
    {
        if (btnOpenMenu_test)
        {
            btnOpenMenu_test = false;

            openMenuUpgradeFromShop(currentLevel_test, typeBoost_test, nbUpgrades_test == 1, true);
            endOpen();
            //switchMenuUpgrade_openState(); // switch anim on
        }
    }
    #endregion*/

    [Header("INIT OPEN CLOSE")]
    /*
     * void openMenuUpgradeFromShop(
     * void switchMenuUpgrade_openState()
     * void openOneUpgrade(
     * void openTwoUpgrade(
    */
    // shop appelle openMenuUpgradeFromShop() PUIS openOneUpgrade() OU openTwoUpgrade()
    #region APPEL OUVERTURE, INIT
    private bool menuIsOpen = false;
    private bool oneItemUp;
    public void openMenuUpgradeFromShop(int currentLevelBoost, typeBoostGame typeBoostUp, bool _oneItemUp)//, bool test)
    {
        //testMode = test;

        currentStarLevel = currentLevelBoost;
        currentBoostType = typeBoostUp;

        oneItemUp = _oneItemUp;

        /*if (test)
        {
            if (oneItemUp)
                openOneUpgrade(false, typeItemUp1_test, currValItem1_test, currAddedItem1_test);
            else
                openTwoUpgrade(typeItemUp1_test, currValItem1_test, currAddedItem1_test,
                    typeItemUp2_test, currValItem2_test, currAddedItem2_test);
        }*/
    }

    public void endOpen() // doit être appelé après que pos item à jour
    {
        switchMenuUpgrade_openState(); // switch anim on
    }

    public void switchMenuUpgrade_openState()
    {
        menuIsOpen = !menuIsOpen;
        StartCoroutine(menuIsOpen ? animOpenMenu() : animCloseMenu());
    }

    public Transform item1Tr, item2Tr;
    public Transform pos1_2items, pos2_2items, pos1_1item;
    public void openOneUpgrade(bool callByOpenTwoUpgrade,
        typeUpgrade typeItem, float currValItem, float currAddedItem, string valueStr)// value str = "s" si secondes
    {
        // affiche en mode 1 upgrade
        currentTypeUp1 = typeItem;
        item1CurrentVal = currValItem;
        item1CurrentAdded = currAddedItem;
        item1CurrentValTypeStr = valueStr;

        if (!callByOpenTwoUpgrade)
            item1Tr.position = new Vector2(item1Tr.position.x, pos1_1item.position.y);
    }

    public void openTwoUpgrade(
        typeUpgrade type1Item, float curr1ValItem, float curr1AddedItem, string value1Str,
        typeUpgrade type2Item, float curr2ValItem, float curr2AddedItem, string value2Str)
    {
        openOneUpgrade(true, type1Item, curr1ValItem, curr1AddedItem, value1Str);
        // affiche en mode 2 upgrade
        currentTypeUp2 = type2Item;     
        item2CurrentVal = curr2ValItem;
        item2CurrentAdded = curr2AddedItem;
        item2CurrentValTypeStr = value2Str;

        item1Tr.position = new Vector2(item1Tr.position.x, pos1_2items.position.y);
        item2Tr.position = new Vector2(item2Tr.position.x, pos2_2items.position.y);
    }
    #endregion

    public GameObject btnCloseMenuUp;
    public void btnCloseMenu()
    {
        switchMenuUpgrade_openState(); // switch anim off
        btnCloseMenuUp.SetActive(false);
    }


    [Header("FILL MENU INFOS")]
    /*
    * void setCurrentStarLevel()
    * void setCurrentImageBoost()
    * void setCurrentItemImg()
    * void setCurrentItemValues()
    * void updateItemValueAfterAnimUpgrade()
    * float roundFloat(float f1)
    */
    #region SET UI INFOS
    public Transform etoileBarTr;
    private GameObject currentBarOpen;
    private int currentStarLevel;
    private void setCurrentStarLevel()
    {
        etoileAnim.Play("upgradeStarReset");
        GameObject o = etoileBarTr.GetChild(currentStarLevel - 1).gameObject;
        o.SetActive(true);
        currentBarOpen = o;
    }

    public boostMenu_infos[] infoBoosts;
    private typeBoostGame currentBoostType;
    private GameObject currentBoostImgOpen;
    private void setCurrentImageBoost()
    {
        foreach (boostMenu_infos i in infoBoosts)
        {
            if (i.typeB == currentBoostType)
            {
                if (currentBoostImgOpen != null)
                    currentBoostImgOpen.SetActive(false);

                currentBoostImgOpen = i.imgBoostTr_associated.gameObject;
                currentBoostImgOpen.SetActive(true);
                break;
            }
        }
    }

    public upInfo[] infoItemUp;
    public Transform contentImgUp1, contentImgUp2;
    private GameObject currentImgUp1, currentImgUp2;
    private typeUpgrade currentTypeUp1, currentTypeUp2;
    private void setCurrentItemImg()
    {
        Debug.Log("SetCurrentItemImg");
        GameObject _currentImgUp;
        typeUpgrade _typeUpImg;

        for (int i = 0; i < 2; i++)
        {
            _typeUpImg = i == 0 ? currentTypeUp1 : currentTypeUp2;
            _currentImgUp = i == 0 ? currentImgUp1 : currentImgUp2;

            Debug.Log("SetCurrentItemImg, item " + i + " , typeUp : " + _typeUpImg);

            if (_currentImgUp != null)
                _currentImgUp.SetActive(false);

            foreach (upInfo u in infoItemUp)
            {
                if (u.uPInfo == _typeUpImg)
                {
                    Debug.Log("Index img to open : " + u.index);
                    if (i == 0)
                    {
                        currentImgUp1 = contentImgUp1.GetChild(u.index).gameObject;
                        currentImgUp1.SetActive(true);
                    }
                    else
                    {
                        currentImgUp2 = contentImgUp2.GetChild(u.index).gameObject;
                        currentImgUp2.SetActive(true);
                    }
                    //(i == 0 ? currentImgUp1 : currentImgUp2).SetActive(true);
                }
            }
        }

        if (currentImgUp1 != null)
            currentImgUp1.SetActive(false);
        foreach (upInfo u in infoItemUp)
        {
            if (u.uPInfo == currentTypeUp1)
            {
                currentImgUp1 = contentImgUp1.GetChild(u.index).gameObject;
                currentImgUp1.SetActive(true);
            }
        }
    }

    public Text item1TextCurrent, item2TextCurrent;
    //public Text item1TextAdded, item2TextAdded;
    private float item1CurrentVal, item2CurrentVal;
    private float item1CurrentAdded, item2CurrentAdded;
    private string item1CurrentValTypeStr, item2CurrentValTypeStr;
    private void setCurrentItemValues()
    {
        item1TextCurrent.text = item1CurrentVal.ToString() + item1CurrentValTypeStr;       
        //item1TextAdded.text = (item1CurrentAdded > 0f ? "+" : "") + item1CurrentAdded.ToString() + item1CurrentValTypeStr;     

        if (!oneItemUp)
        {
            item2TextCurrent.text = item2CurrentVal.ToString() + item2CurrentValTypeStr;
            //item2TextAdded.text = (item2CurrentAdded > 0f ? "+" : "") + item2CurrentAdded.ToString()+ item2CurrentValTypeStr;
        }
    }
    private void updateItemValueAfterAnimUpgrade()
    {
        //float f1 = item1CurrentVal + item1CurrentAdded;
        //float f2 = roundFloat(item1CurrentVal + item1CurrentAdded);
        //Debug.Log("Current val : " + item1CurrentVal + ", Added val : "+ item1CurrentAdded+" Res rounded: "+ f2+ ", res"+f1);
        item1TextCurrent.text = (item1CurrentVal + item1CurrentAdded).ToString() + item1CurrentValTypeStr;
        if (!oneItemUp)
            item2TextCurrent.text = /*roundFloat*/(item2CurrentVal + item2CurrentAdded).ToString() + item2CurrentValTypeStr;
    }
    /*private float roundFloat(float f1)
    {
        return ((int)(f1 * 100)) / 100f;
    }*/
    #endregion


    [Header("ANIM MENU")]
    /*
     * IEnumerator animOpenMenu()
     * IEnumerator animUpgrade()
     * IEnumerator animCloseMenu()
     * void animStarLevel()
    */
    #region ANIM MENU
    public float timePopItemUp = 25 / 60f;
    public float timeAnimItemPop = 34 / 60f;
    public float timeEndOpenToUpgrade = 2 / 6f;
    public Animation menuAnim, itemUp1Anim, itemUp2Anim;
    private IEnumerator animOpenMenu()
    {
        menuAnim.gameObject.SetActive(true);
        menuAnim.Play("menuUpgradeOpen");
        sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.upgrade_open);
        setCurrentStarLevel();
        setCurrentImageBoost();
        setCurrentItemImg();
        setCurrentItemValues();

        yield return new WaitForSeconds(timePopItemUp);

        itemUp1Anim.gameObject.SetActive(true);
        itemUp2Anim.gameObject.SetActive(!oneItemUp);
        itemUp1Anim.Play("statBoostUpgradePop");
        if(!oneItemUp)
            itemUp2Anim.Play("statBoostUpgradePop");

        yield return new WaitForSeconds(timeAnimItemPop + timeEndOpenToUpgrade);

        btnNextPhaseGo.SetActive(true);
        //StartCoroutine(animUpgrade());
    }

    public GameObject btnNextPhaseGo;
    public void btnNextPhase()
    {
        btnNextPhaseGo.SetActive(false);
        StartCoroutine(animUpgrade());
        //sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.click_pass);
    }

    public Animation etoileAnim, brillerAnim;
    public float timeAnimBrillerToStar = 1 / 6f;
    public float timeAnimBrillerToItemPop = 48 / 60f;
    public float timeAnimUpdateText = 11 / 60f;
    // temps avant update text : 11, entre anim start -> text off
    private IEnumerator animUpgrade()
    {
        brillerAnim.Play("bordUpgradeBriller");
        sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.animUp_briller);

        yield return new WaitForSeconds(timeAnimBrillerToStar);

        animStarLevel();

        yield return new WaitForSeconds(timeAnimBrillerToItemPop - timeAnimBrillerToStar);

        sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.animUp_star);

        itemUp1Anim.Play("statBoostUpgradeUp");
        if (!oneItemUp)
            itemUp2Anim.Play("statBoostUpgradeUp");

        //if (!testMode)
            infoBoostManager.getInstance().animEnded_updateMenuInfo();       

        yield return new WaitForSeconds(timeAnimUpdateText);

        sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.animUp_item);

        updateItemValueAfterAnimUpgrade();

        yield return new WaitForSeconds(0.5f - timeAnimUpdateText);

        btnCloseMenuUp.SetActive(true);
    }

    public float timeAnimCloseMenu = 15 / 60f;
    private IEnumerator animCloseMenu()
    {
        menuAnim.Play("menuUpgradeClose");
        sfxGame.getInstance().playSfx_listUpgradeBoost(typeSfx_upgradeBoost.upgrade_close);

        yield return new WaitForSeconds(timeAnimCloseMenu);

        menuAnim.gameObject.SetActive(false);
        itemUp1Anim.gameObject.SetActive(false);
        itemUp2Anim.gameObject.SetActive(false);
        currentBarOpen.SetActive(false);
    }

    private void animStarLevel()
    {
        etoileAnim.Play("upgradeBoostToLevel" + (currentStarLevel + 1).ToString());
        
    }

    #endregion

}
