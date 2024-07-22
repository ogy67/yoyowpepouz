using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLevelItem : MonoBehaviour
{
    public static bool isInGameMode = false;
    public Transform centerToCompare;
    public Transform posScaleMinTr, posScaleNormalTr;
    public Transform minScaleExampleTr, maxScaleExampleTr;

    public RectTransform rT;
    private float normalScalePos;
    private float minScalePos;

    // Start is called before the first frame update
    void Start()
    {
        rT = GetComponent<RectTransform>();
        normalScalePos = getPosX_compareCenter(posScaleNormalTr.position.x, true);
        minScalePos = getPosX_compareCenter(posScaleMinTr.position.x, true);
    }


    // ------------------------------------------------------- FONCTIONS principales ( mouvement des item UI )

    public Image imgColored;
    //public Color selectedColor, unselectedColor;
    private Vector2 prevPos = Vector2.zero;
    private bool isSelected = false;
    public ScrollRectSnap_CS _scrollRectCs;
    public Text numLevelText;
    public Text nbCrownRequiredText;
    public int numLevel;
    private int listItemSize = 5;
    /*
     * void LateUpdate()
     * void itemChangeSideScrollView(Vector2 newPos, bool up)
     * float getScale_byPos(float transformPosX, float minPosX, float maxPosX, float minScaleX, float maxScaleX)
     * float getPosX_compareCenter(float xToCompare)
    */
    #region GESTION scale Item en fonction pos sur ecran
    private void LateUpdate()
    {
        if (isInGameMode)
            return;

        //if (!testMode) // doit être appelé même pendant animSwitch
        levelFollowPosAndScale();

        if (transform.position.x == prevPos.x)
        {
            return;
        }

        float transformPos_abs = getPosX_compareCenter(transform.position.x, true);
        float transformPos = getPosX_compareCenter(transform.position.x, false);
        prevPos = transform.position;

        if (numLevel == 2)
            _scrollRectCs.scrollPosChanged_left(transformPos);
        if (numLevel == ScrollRectSnap_CS.maxNumLevel - 1)
            _scrollRectCs.scrollPosChanged_right(transformPos);

        if (transformPos_abs >= minScalePos)
        {
            transform.localScale = minScaleExampleTr.localScale;
            if (isSelected)
            {
                isSelected = false;
                //imgColored.color = unselectedColor;
            }
            levelIsInMinZone(); // parfois passe cash de middleZone à minZone, sans passer par normalScalePos
        }
        else if(transformPos_abs >= normalScalePos)
        {
            float newScale = getScale_byPos(transformPos_abs, minScalePos, normalScalePos,
                 minScaleExampleTr.localScale.x, maxScaleExampleTr.localScale.x);
            transform.localScale = new Vector2(newScale, newScale);
            levelIsInMinZone();
        }
        else
        {
            transform.localScale = maxScaleExampleTr.localScale;
            if (!isSelected)
            {
                isSelected = true;
                //imgColored.color = selectedColor;
                managerNiv.levelIsCentered(numLevel);
                // dire à scrollRectSnap que item centré a changé
                _scrollRectCs.centeredItemChangedWhileDragging(numLevel, transform.GetSiblingIndex());
            }
            levelIsInNormalZone();
        }
    }

    // PB seulement 1 fois, à l'initialisation
    // itemScroll 0 : niv 7 ( nivIndex = 6
    // BTN go bound 1
    // itemScroll 0 : niv 1 ( nivIndex = 0
    // itemScroll 0 reste au milieu && itemScroll 1 reste a droite
    // seul gauche et bound gauche change de pos && ferment niveau associé

    // appelé par ScrollRect_CS quand change de côté


    public void itemChangeSideScrollView(Vector2 newPos, bool up)
    {
        //Debug.Log("--- (" + numLevel + ") change side screen, is switching : "+isSwitching);
        //Debug.Log("Item " + transform.GetSiblingIndex() 
        //    + " change side (curPos.x : "+ rT.anchoredPosition.x+", newPos.x : "+newPos.x+")");
        //Debug.Log("<color=green>GO level '" + currentLevelAssociated.name + "' is disactivated</color>");
        rT.anchoredPosition = newPos;
        numLevel += (up ? listItemSize : -listItemSize);
        numLevelText.text = "LVL " + numLevel;
        _scrollRectCs.itemChangeNum(numLevel, transform);
        setLock(numLevel >= ScrollRectSnap_CS.lockedLevelNum ? true : false, false);

        //Debug.Log("Level closed : " + currentLevelAssociated.name);

        // numero se donnent differement pendant un switch, si change side pendant switch, faire le changement level en mode switch

        if (!isSwitching
            && currentLevelAssociated != null)
        {
            currentLevelAssociated.GetComponent<packNiveau>().levelIsClosedByScroll();
            managerNiv.closeLvl_V2(currentLevelAssociated.gameObject);
        }
        //currentLevelAssociated.gameObject.SetActive(false);// TEST
        if (!testMode)
            associateLevelTransform(numLevel, isSwitching);


    }

    private float getScale_byPos(float transformPosX, float minPosX, float maxPosX, float minScaleX, float maxScaleX)
    {
        float distance_normal_min = minPosX - maxPosX;
        float distance_item_normal = transformPosX - maxPosX;

        float factorAgrandissement = 1 - distance_item_normal / distance_normal_min;
        float diffScale_normal_min = maxScaleX - minScaleX;
        float newScale = minScaleX + (diffScale_normal_min * factorAgrandissement);
        if (newScale < 0f) newScale = 0f;
        return newScale;
    }

    private float getPosX_compareCenter(float xToCompare, bool getAbs)
    {
        return getAbs ? Mathf.Abs(xToCompare - centerToCompare.position.x) : xToCompare - centerToCompare.position.x;
    }
    #endregion



    public GameObject lockPanGo;
    public bool isLocked = false;
    /*
     * void setNumLevel(int initNum, bool isLocked, bool goPreciseLevel)
     * void aLevelIsUnlocked(int newLockedNum)
     * void setLock(bool _isLocked)
     * void btnInfoLock()
    */
    #region GESTION lock level & attribution numero
    public void setNumLevel(int initNum, bool isLocked, bool goPreciseLevel)
    {
        //Debug.Log("Set item Scroll infos : ");
        //Debug.Log("initNum : " + initNum + " , isLocked? : " + isLocked);
        /*if (goPreciseLevel)
        {
            Debug.Log("Go precise level, item become num : " + initNum);
        }*/
        numLevel = initNum;
        numLevelText.text = "LVL " + numLevel;
        _scrollRectCs.itemChangeNum(numLevel, transform);
        setLock(isLocked, false);
        //if (initNum <= ScrollRectSnap_CS.maxNumLevel)
        associateLevelTransform(numLevel, goPreciseLevel);
    }

    public void aLevelIsUnlocked(int newLockedNum)
    {
        if (isLocked)
        {
            setLock(!(numLevel < newLockedNum), false);
        }
        else if(numLevel >= newLockedNum)
        {
            setLock(true, false);
        }
    }

    public GameObject lockedCrown, lockedCrownNStar;
    public GameObject enoughCrownGo, enoughStarGo;
    public Text nbCrownLocked_noStar;
    public Text nbCrownLocked_star, nbStarLocked_star;
    public Color textLockedColor, textUnlockedColor;
    public void setLock(bool _isLocked, bool forceReload)
    {
        if(forceReload
            || isLocked != _isLocked)
        {
            isLocked = _isLocked;
            lockPanGo.SetActive(isLocked);
            bool starToo = managerNiveau.nbStarRequired_unlockLevel != 0;
            if (isLocked)
            {
                lockedCrown.SetActive(!starToo);
                lockedCrownNStar.SetActive(starToo);
                if (!starToo)
                {
                    nbCrownLocked_noStar.text = managerNiveau.nbCrownRequired_unlockLevel.ToString();
                    nbCrownLocked_noStar.color = textLockedColor;
                    isLockedByCrown = true;
                    //Debug.Log("1");
                }
                else
                {
                    //Debug.Log("2");
                    nbCrownLocked_star.text = managerNiveau.nbCrownRequired_unlockLevel.ToString();
                    nbStarLocked_star.text = managerNiveau.nbStarRequired_unlockLevel.ToString();

                    isLockedByCrown = managerNiveau.nbCrownRequired_unlockLevel > SaveSystem.nb_couronne;
                    isLockedByStar = managerNiveau.nbStarRequired_unlockLevel > SaveSystem.getNbStarObtain();

                    nbCrownLocked_star.color = isLockedByCrown ? textLockedColor : textUnlockedColor;
                    nbStarLocked_star.color = isLockedByStar ? textLockedColor : textUnlockedColor;

                    enoughCrownGo.SetActive(!isLockedByCrown);
                    enoughStarGo.SetActive(!isLockedByStar);
                }
            }
        }
    }

    // appelé quand a reccup les crown et les star en fin de game
    public void updateLock(int nbC, int nbS)
    {
        // si level locked
        if (isLocked)
        {
            // si double lock
            if (managerNiveau.nbStarRequired_unlockLevel != 0)
            {
                // mettre à jour voyant crown et voyant star ( si validés )

                isLockedByCrown = managerNiveau.nbCrownRequired_unlockLevel > nbC;
                isLockedByStar = managerNiveau.nbStarRequired_unlockLevel > nbS;

                nbCrownLocked_star.color = isLockedByCrown ? textLockedColor : textUnlockedColor;
                nbStarLocked_star.color = isLockedByStar ? textLockedColor : textUnlockedColor;

                enoughCrownGo.SetActive(!isLockedByCrown);
                enoughStarGo.SetActive(!isLockedByStar);
            }
        }
    }

    private bool isLockedByCrown = false;
    private bool isLockedByStar = false;
    public infoMenuManager infoMenu;
    public void btnInfoLock()
    {
        if (notifDefiDone.isAnimatingNotif || notifStarUnlocked.isAnimating)
            return;

        // pas assez de couronnes
        if (isLockedByCrown)
        {
            infoMenu.btnInfo_crownCompteur();
        }
        // pas assez de star
        else
        {
            infoMenu.btnInfo_star();
        }
    }
    #endregion


    // ------------------------------------------------------- FONCTIONS secondaires ( ajout de niveaux réels )

    private static bool testMode = false; // true : test sont fait indépendament du jeu
    public float offsetPosXItem, offsetPosYItem;
    public static Vector3 normalScaleLevel = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 normalScaleItem;
    public Transform test_lexNiveaux;
    public Transform imgAnimSwitchTr;
    public managerNiveau managerNiv;
    private static List<Transform> _itemDisableSwitch = new List<Transform>();
    private static List<Transform> _itemEnableSwitch = new List<Transform>();
    private Transform currentLevelAssociated;
    /*
     * void associateLevelTransform(int numLvl, bool goPreciseLevel)
     * void levelFollowPosAndScale()
    */
    #region GESTION niveau associé : suivrePos/suivreScale/suivreAnim

    private void associateLevelTransform(int numLvl, bool goPreciseLevel)
    {
        //Debug.Log("Associate num level to item : " + numLevel);
        if (numLevel < 1)
            return;

        //Debug.Log("(Item index"+transform.GetSiblingIndex()+") Associate new level, num : " + numLevel + " , index : " + (numLevel - 1));
        // si associe par bounds dépassé
        if (!goPreciseLevel)
        {
            // check si est dans liste disable
            // liste pas refresh avant check ?
            
            if (currentLevelAssociated != null)
                //&& !_itemEnableSwitch.Contains(currentLevelAssociated))
            {
                // est desactivé tout seul en fin de switch

                //Debug.Log("--- (" + numLevel + ") desactive level precedent : "+ currentLevelAssociated.gameObject.name);
                currentLevelAssociated.gameObject.SetActive(false);
            }

            if (numLevel <= ScrollRectSnap_CS.maxNumLevel)
            {
                currentLevelAssociated = managerNiv.lesNiveaux[numLvl - 1];
                currentLevelAssociated.gameObject.SetActive(true);
                //Debug.Log("--- (" + numLevel + ") activate level : " + currentLevelAssociated.gameObject.name);
            }
            //Debug.Log("--- (" + numLevel + ") (change side), get new level transform, active ? : "+currentLevelAssociated.gameObject.activeSelf);
        }
        // sinon si associe quand switch
        else
        {
            //Debug.Log("(" + numLevel + ") Associate level Transform");
            // si un autre item va l'utiliser, ne pas désactiver
            if (currentLevelAssociated != null
                && !_itemEnableSwitch.Contains(currentLevelAssociated))
            {
                //Debug.Log("--- (" + numLevel + ") Previous level ("+ currentLevelAssociated.gameObject.name + ") should be set to OFF");
                _itemDisableSwitch.Add(currentLevelAssociated);
            }

            
            // pour level qui n'est pas desactivé ? 
            // Pb dans : item se repositionne après ces fonctions ?

            if (numLvl <= ScrollRectSnap_CS.maxNumLevel)
            {
                currentLevelAssociated = managerNiv.lesNiveaux[numLvl - 1];
                currentLevelAssociated.gameObject.SetActive(true);
                _itemEnableSwitch.Add(currentLevelAssociated);
                //Debug.Log("--- (" + numLevel + ") Association normale, active le level associe");


                 // peut enlever le if grace au while
                // si un autre item l'utilisait, ne pas desactiver
                //if (_itemDisableSwitch.Contains(currentLevelAssociated))
                //{
                //    Debug.Log("--- (" + numLevel + ") Next level est dans liste desactivation, est retiré");
                    // peut être mit plusieurs fois dans la liste
                    while(_itemDisableSwitch.Contains(currentLevelAssociated))
                        _itemDisableSwitch.Remove(currentLevelAssociated);
                    //Debug.Log(currentLevelAssociated+" in close level list, remove it");
                //}
            }
        }
        if (numLvl <= ScrollRectSnap_CS.maxNumLevel)
            levelFollowPosAndScale();
    }

    private void levelFollowPosAndScale()
    {
        //if (currentLevelAssociated == null)
        //    return;

        Vector2 myScale;

        // si anim switch en court, suivre scale de imgAnimScale
        if (ScrollRectSnap_CS.isSwitching
            || isOpenningOrClosing)
        {
            // Si est en scaleMin et pas scaleNormal
            // => suivre scale AUSSI en fonction de scaleMin
            float factor = transform.localScale.x / maxScaleExampleTr.localScale.x;
            myScale = imgAnimSwitchTr.localScale * factor;
        }
        else
        {
            myScale = transform.localScale;
        }

        Vector2 myPos = Camera.main.ScreenToWorldPoint(transform.position);

        float facteurAgrandissement = myScale.x / normalScaleItem.x;

        /*if (currentLevelAssociated.name == "Niveau 39")
        {
            Debug.Log(currentLevelAssociated.name + " , facteur agrandissement : " + facteurAgrandissement + " , myScale : " + myScale);
            Debug.Log("normalScaleLevel : "+normalScaleLevel);
        }*/

        currentLevelAssociated.localScale = normalScaleLevel * facteurAgrandissement;
        currentLevelAssociated.position = myPos;
    }
    #endregion


    // -------------------------------------- FONCTIONS animations

    private Transform waitLevelChangeTr;
    private static bool isSwitching = false;
    /*
     * static void endSwitchDisableItems()
     * void levelSwitch_anim()
     * IEnumerator waitBeforeGlowNewLevel()
    */
    #region GESTION anim switch , couplé à glow
    public static void endSwitchDisableItems()
    {
        //Debug.Log("<color=red>End go bounds</color>");
        foreach (Transform t in _itemDisableSwitch)
        {
            //Debug.Log("-- Close : " + t.name);
            t.gameObject.SetActive(false);
            managerNiveau.getInstance().closeLvl_V2(t.gameObject);
        }
        _itemDisableSwitch = new List<Transform>();
        _itemEnableSwitch = new List<Transform>();
        isSwitching = false;
    }

    public static void startSwitch()
    {
        _itemDisableSwitch = new List<Transform>();
        _itemEnableSwitch = new List<Transform>();
        isSwitching = true;
    }

    public void levelSwitch_anim()
    {
        myAnim.Play("scrollItemSwitch");
        // seulement pour level centered
        if (glowIsOn)
        {
            changeGlowState(false, 3f);
            // on reactive le glow car item center reste au centre malgrè changement de niveau
            waitLevelChangeTr = currentLevelAssociated;
            StartCoroutine(waitBeforeGlowNewLevel());
            managerNiv.closeLvl_V2(currentLevelAssociated.gameObject);
        }
    }

    private IEnumerator waitBeforeGlowNewLevel()
    {
        while (waitLevelChangeTr == currentLevelAssociated)
            yield return null;
        // une fois le nouveau level centered, on active son glow
        changeGlowState(true, 1f);
    }
    #endregion


    public Animation myAnim;
    public float timeAnimClose = 7 / 60f;
    public float timeAnimOpen = 12 / 60f;
    private bool isOpenningOrClosing = false; // pas static, si level milieu pas close, ne pas être bloqué par autre lvl qui close
    /*
     * void closeLevel()
     * void openLevel()
     * IEnumerator openCloseEnum(bool open)
    */
    #region GESTION ouverture/fermeture level, quand startGame/goInfinis/closeInfinis 
    public void closeLevel() // pour aller mode infinis, ou alors commencer game aventure
    {
        StartCoroutine(openCloseEnum(false));
    }

    public void openLevel()
    {
        StartCoroutine(openCloseEnum(true));
    }

    private IEnumerator openCloseEnum(bool open)
    {
        isOpenningOrClosing = true;
        if (open)
        {
            currentLevelAssociated.gameObject.SetActive(true);
            //Debug.Log("(openCloseEnum) Anim open item " + currentLevelAssociated.name);
            myAnim.Play("scrollItemOpen");
            yield return new WaitForSeconds(timeAnimOpen);
        }
        else
        {
            myAnim.Play("scrollItemClose");
            //Debug.Log("(openCloseEnum) Anim close item " + currentLevelAssociated.name);
            yield return new WaitForSeconds(timeAnimClose);
            managerNiv.closeLvl_V2(currentLevelAssociated.gameObject);
            currentLevelAssociated.gameObject.SetActive(false);
            //currentLevelAssociated.GetComponent<packNiveau>().levelIsClosedByScroll();
        }
        isOpenningOrClosing = false;
    }
    #endregion


    private bool glowIsOn = false;
    /*
     * void levelIsInNormalZone()
     * void levelIsInMinZone()
     * void changeGlowState(bool isOn, float speedAnim)
    */
    #region GESTION glow level
    // is centered est appelé par rapport à pos de curLevel
    private void levelIsInNormalZone()
    {
        // si glow desactiver, activer glow
        if (!glowIsOn)
        {         
            changeGlowState(true, 1f);           
        }
    }
    private void levelIsInMinZone()
    {
        // si glow activé, désactiver glow
        if (glowIsOn)
        {         
            changeGlowState(false, 1f);
        }
    }

    private void changeGlowState(bool isOn, float speedAnim)
    {
        //Debug.Log("Go " + currentLevelAssociated.name + " glow goes " + (isOn ? "On" : "Off"));
        if (isOn)
        {
            glowIsOn = true;
            currentLevelAssociated.GetComponent<Animation>().Play("switchLightLevel");
            currentLevelAssociated.GetComponent<Animation>()["switchLightLevel"].speed = 1f;
        }
        else
        {
            glowIsOn = false;
            currentLevelAssociated.GetComponent<Animation>().Play("switchLightLevelOff");
            // quand switch, glowOff doit être aussi rapide que closeLevel, sinon reste illuminé
            currentLevelAssociated.GetComponent<Animation>()["switchLightLevelOff"].speed = speedAnim;
        }
    }
    #endregion

}