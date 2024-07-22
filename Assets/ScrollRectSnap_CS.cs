using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour
{
    public managerNiveau _managerNiveau;
    public RectTransform scrollPanel; // to hold the scrollPanel
    public ScrollRect _scrollRect;
    public Transform[] lesItems;
    public RectTransform center_right, center_left, center_middle; // center to compare the distance for each button
    public GameObject btnGauche, btnDroite, btnStart;

    public Animation bordSombreAnim_gauche, bordSombreAnim_droite;
    private bool bordGaucheOn = false, bordDroiteOn = false;

    public int sizeScroll = 1400;
    public float minDistanceToChangePos = 3f;
    public float minDistanceToAccelerate = 20f;
    public float normalLerpSpeed = 5f;
    public float accelerateLerpSpeed = 30f;
    private bool targetNearestItem = false; // true dès que dragStart
    private float[] distance;
    private float[] distReposition;

    private bool isDragging = false;
    private bool messageSend = false;
    private int itemDistance;
    private int indexBtnCentered;
    private int itemLength;

    public static int maxNumLevel = 80;
    public int minNumLevel = 1;
    public static int lockedLevelNum = 1000; // (SI) ne lock rien si = maxNumLevel + 1
    public static bool isLockedWithStar = false;
    private int currentNumLevel = 1; // par défaut

    // ajuster en fonction de screenRatio
    public float normalScrollScale = 1f; // à partir de aspect ratio = 9:18
    public float minScrollScale = 0.925f; // jusqu'à aspect ratio min = 9:21
    public float minLevelScale = 0.425f;
    private float minRatioToChange = 18 / 9f, maxRatio = 21 / 9f;
    private static bool scrollIsInitialized = false;

    public Transform posScaleMiddle, posScaleOut;
    /*
     * void Start()
     * void initializeScroll()
     * void initializeOnPreciseLevel(int numLevel)
    */
    #region INITIALISATION

    private void Start()
    {
        if (!scrollIsInitialized)
            initializeScroll();
    }

    public float facteurDistance_byRatio = 1.05f;
    private void initializeScroll()
    {
        // on doit rescale scrollPan en fonction ratio screen size, si très etroit, diminuer la taille
        float myRatio = (float)Screen.height / (float)Screen.width;

        itemDistance = (int)Mathf.Abs(lesItems[1].GetComponent<RectTransform>().anchoredPosition.x
            - lesItems[0].GetComponent<RectTransform>().anchoredPosition.x);

        if (myRatio > minRatioToChange)
        {
            float rapport = (myRatio - minRatioToChange) / (maxRatio - minRatioToChange);
            float newScale = normalScrollScale - (normalScrollScale - minScrollScale) * rapport;
            //_scrollRect.transform.localScale = new Vector3(newScale, newScale, newScale);
            float rapport_2 = newScale / _scrollRect.transform.localScale.x;


            float curLevelScaleX = ScrollLevelItem.normalScaleLevel.x;
            float newScaleLevel = curLevelScaleX - (curLevelScaleX - minLevelScale) * rapport;
            //Debug.Log("curLevelScaleX : " + curLevelScaleX + " , minLevelScale : " + minLevelScale + " , rapport : "+ rapport
              //  + ", newScaleLevel : " + newScaleLevel);
            ScrollLevelItem.normalScaleLevel = new Vector3(newScaleLevel, newScaleLevel, newScaleLevel);

            rapport_2 /= facteurDistance_byRatio;
            //Debug.Log("<color=red>Rapport_2 : </color>" + rapport_2);
            // on doit rescale itemDistance en fonction de la diminution de taille des items
            itemDistance = (int)(itemDistance * rapport_2);

            posScaleMiddle.localPosition = new Vector2(posScaleMiddle.localPosition.x
                * rapport_2, posScaleMiddle.localPosition.y);// / facteurDistance_byRatio, 0);
            posScaleOut.localPosition = new Vector2(posScaleOut.localPosition.x
                 * rapport_2, posScaleOut.localPosition.y);// / (facteurDistance_byRatio * 1.2f), 0);

            // on doit modifier la distance entre les 5 item
            int x_distance = 0;
            foreach (Transform t in lesItems)
            {
                t.localPosition = new Vector2(x_distance, t.localPosition.y);
                x_distance += itemDistance;
            }
        }

        indexBtnCentered = 0;
        itemLength = lesItems.Length;
        distance = new float[itemLength];
        distReposition = new float[itemLength];

        

        // en fonction nombre de pixels en hauteur, changer valeure de sizeScroll
        float y_pixel_ref = 2160;
        float y_pixel_current = Screen.height;
        float ratio = y_pixel_current / y_pixel_ref;
        sizeScroll = (int)(ratio*(float)sizeScroll);
        scrollIsInitialized = true;
    }


    // appelé à l'initialisation du menu
    private void initializeOnPreciseLevel(int numLevel)
    {
        // On doit pouvoir changer de numLevel depuis n'importe quel bouton
        // Créer liste btn en fonction place actuelle ( btn centré = item index 0 )
        if (!scrollIsInitialized)
            initializeScroll();

        
        /*if(lockedLevelNum == maxNumLevel)
        {
            lockedLevelNum = 1000;
        }*/

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.initApp_popLevel);

        int _currentNumLvl;
        //bool lastLevelUnlocked;
        for (int i = 0; i < lesItems.Length; i++)
        {
            _currentNumLvl = numLevel + i;
            //Debug.Log("Is going to set init level infos (maxNum : "+maxNumLevel+")");
            //Debug.Log("_currentNumLvl : " + _currentNumLvl + " , lockedLevelNum : " + lockedLevelNum);
            //lastLevelUnlocked = _currentNumLvl == maxNumLevel && lockedLevelNum == maxNumLevel;
            //Debug.Log("Is last level & unlocked ? " + lastLevelUnlocked);

            /*if (_currentNumLvl == maxNumLevel && lockedLevelNum == maxNumLevel)
            {
                lesItems[i].GetComponent<ScrollLevelItem>().setNumLevel(_currentNumLvl, false, true);
            }*/
            //else
            //{
                lesItems[i].GetComponent<ScrollLevelItem>().setNumLevel(_currentNumLvl,
                    _currentNumLvl < lockedLevelNum ? false : true,
                    true);
            //} // revenir en arrière

            if (_currentNumLvl == maxNumLevel
                || _currentNumLvl == lockedLevelNum - 1)
            {
                currentLimit_toCompare = lesItems[i];
            }

            if(numLevel != 1
                && (_currentNumLvl > maxNumLevel || _currentNumLvl > numLevel + 2) )
                changeBtnSide(lesItems[i], true);
        }
    }
    #endregion

    public float minSpeedScrollPanToSnap = 150f;
    private float currentSpeedScrollPan;
    private float prevAnchoredPosXScrollPan = 0f;
    private bool isSnapingItem = false;
    private Transform level1_limit; // peut pas aller en dessous level 1
    public Transform levelMax_limit; // peut pas aller au dessus de level lock
    private bool lockedOnLeft = false, lockedOnRight = false;
    public Button btnFirst, btnLast;
    public ScrollRect scrollRectLevels;
    private bool endDragCalculVitesse; // mit à true dans endDrag
    private bool endDragWaitFrameSpeed; // mit à true dans lateUpdate, attend qu'une frame passe pour calculer la vitesse
    public float minSpeedScrollPanToSnap_2; // calculé à la fin de endDrag, en fonction vitesse drag, ajuster la vitesse min ou vas snap
    public float aimant_directSnapSpeed = 120;
    public float aimant_minSpeed = 400;
    public float aimant_maxSpeed = 700;
    public float aimant_minForce = 100;
    public float aimant_maxForce = 400;

    /*
     * void LateUpdate()
     * void Update()
     * void changeBtnSide(Transform itemTr, bool goToLeft)
     * void lerpToItem(float position)
     * void itemChangeNum(int numLevel, Transform trItem)
     * void itemIsCentered(int itemIndex)
     */
    #region Gestion event pendant scroll
    // Détécter lors de drag scroll, quand scrollPan atteind vitesse min -> cash snap item le + proche
    private void LateUpdate()
    {
        // n'est que appelé pendant que scroll sans drag
        if (!(!isDragging && targetNearestItem))
            return;

        // n'est pas appelé pendant que snap
        if (isSnapingItem)
            return;

        if (isAboveLimits())
        {
            //Debug.Log("<color=yellow>Debug SCROLL : isAboveLimits</color>");
            _scrollRect.StopMovement();
            scrollPanel.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-currentLimit_toCompare.GetComponent<RectTransform>().anchoredPosition.x, scrollPanel.GetComponent<RectTransform>().anchoredPosition.y);
            return;
        }

        float distanceFromPrevPos = Mathf.Abs(scrollPanel.anchoredPosition.x - prevAnchoredPosXScrollPan);
        currentSpeedScrollPan = distanceFromPrevPos / Time.deltaTime;

        // premiere frame endrag, ne rien faire, attendre la prochaine
        if (endDragCalculVitesse)
        {
            endDragCalculVitesse = false;
            endDragWaitFrameSpeed = true;
        }
        // next frame endrag, calculer vitesse
        else if (endDragWaitFrameSpeed)
        {
            //Debug.Log("Vitesse scroll end Drag = " + currentSpeedScrollPan);

            // si vitesse en dessous seuil "snap direct sur suivant"
            if (currentSpeedScrollPan < aimant_minSpeed)
            {
                // et au dessus seuil minim
                if(currentSpeedScrollPan > aimant_directSnapSpeed
                    // si l'item centré a changé pendant le drag, alors ne pas check ce qu'il y a en dessous
                    && !itemCenteredChangedWhileDragging)
                {
                    
                    int max = lockedLevelNum > maxNumLevel ? maxNumLevel : lockedLevelNum;
                    // on va direct sur l'item d'à côté

                    //Debug.Log("VA DIRECTEMENT ITEM D'à COTE, currentNum : "+currentNumLevel+" (max:"+ max + ") , indexBtnCentered : "+ indexBtnCentered);
                    bool goLeft = scrollPanel.anchoredPosition.x > prevAnchoredPosXScrollPan;
                    // vérifier que n'est pas sur une limite, et la franchit

                    // PB, si level suivant est locked, empeche aller dessus 
                    // MAIS si level suivant est tout dernier ne fait rien, devrait aller dessus
                    bool canGoRight = lockedLevelNum >= maxNumLevel ?
                        currentNumLevel < max ? true : false
                        : currentNumLevel < max - 1 ? true : false;

                    if (currentNumLevel == 1 && goLeft
                        || !canGoRight)
                    {
                        // rien faire
                        // se repositionne seul sur l'item centré

                        //Debug.Log("Empêche rescroll autre sens et depasse bounds");
                        minSpeedScrollPanToSnap_2 = aimant_maxForce;
                    }
                    else {
                        isSnapingItem = true;
                        goToItem_snap(goLeft ? indexBtnCentered == 0 ? lesItems.Length - 1 : indexBtnCentered - 1
                            : indexBtnCentered + 1 == lesItems.Length ? 0 : indexBtnCentered + 1);
                        return;
                    }
                }
                // sinon, vitesse beaucoup trop faible
                else
                {
                    // se repositionne seul sur l'item centré
                    minSpeedScrollPanToSnap_2 = aimant_maxForce;                   
                }
            }               
            else if (currentSpeedScrollPan > aimant_maxSpeed)
                minSpeedScrollPanToSnap_2 = aimant_minForce;
            else {
                float range = (aimant_maxSpeed - aimant_minSpeed); // 1000 - 500 -> 500
                float prog = (currentSpeedScrollPan - aimant_minSpeed) / range; // 516 - 500 / 500
                minSpeedScrollPanToSnap_2 = aimant_maxForce -
                    ( aimant_maxForce - aimant_minForce ) * prog;
            }
            endDragWaitFrameSpeed = false;
            itemCenteredChangedWhileDragging = false;
        }

        if(currentSpeedScrollPan <= minSpeedScrollPanToSnap_2)
        {
            isSnapingItem = true;
        }

        prevAnchoredPosXScrollPan = scrollPanel.anchoredPosition.x;
    }

    private bool itemCenteredChangedWhileDragging = false;
    public void centeredItemChangedWhileDragging(int newNumLevel, int indexItemScrollCentered)
    {
        // on regarde si est bien appelé pendant le drag
        if (isDragging)
        {
            //Debug.Log("Item centré change pendant drag");
            //Debug.Log("Prev num : " + currentNumLevel + ", new num : " + newNumLevel);
            currentNumLevel = newNumLevel;
            indexBtnCentered = indexItemScrollCentered;
            itemCenteredChangedWhileDragging = true;
        }
    }

    // Système de 'roulement' des items, se repositionnent pour effet 'list infinis'
    // Appel lerpToItem() pour centrer
    private void Update()
    {
        // Ne pas appeler si position actuelle est la même que précédente
        /*if (!prevPosLoaded)
        {
            prevScrollPositionX = scrollPanel.anchoredPosition.x;
            prevPosLoaded = true;
        }
        if (scrollPanel.anchoredPosition.x != prevScrollPositionX)
        {
            prevScrollPositionX = scrollPanel.anchoredPosition.x;*/

        for (int i = 0; i < lesItems.Length; i++)
        {
            distance[i] = Mathf.Abs(center_middle.transform.position.x - lesItems[i].position.x);
            distReposition[i] = center_middle.GetComponent<RectTransform>().position.x
                - lesItems[i].GetComponent<RectTransform>().position.x;

            if (Mathf.Abs(distReposition[i]) > sizeScroll)
            {
                ScrollLevelItem sI = lesItems[i].GetComponent<ScrollLevelItem>();

                if (distReposition[i] > sizeScroll)
                {
                    // celui tout a gauche avance tout a droite
                    lockedOnLeft = false;
                    if (sI.numLevel + lesItems.Length <= maxNumLevel)
                    {
                        changeBtnSide(lesItems[i], false);
                    }
                }
                else if (distReposition[i] < -sizeScroll)
                {
                    // celui tout a droite recule tout a gauche
                    if (sI.numLevel - lesItems.Length > 0)
                    {
                        if (lesItems[i] == currentLimit_toCompare)
                        {
                            //Debug.Log("Move item, scroll not locked on right");
                            lockedOnRight = false;
                            //Debug.Log("Desactive max limit");
                            levelMax_limit = null;
                            currentLimit_toCompare = null;
                        }
                        changeBtnSide(lesItems[i], true);
                    }

                }
            }
        }
        //}
        if (targetNearestItem)
        {
            if (!goingToItemByClick)
            {
                if (!isSnapingItem)
                    return;

                float minDistance = Mathf.Min(distance);

                for (int a = 0; a < lesItems.Length; a++)
                {
                    if (minDistance == distance[a])
                    {
                        indexBtnCentered = a;
                    }
                }
            }

            if (!isDragging)
            {
                //Debug.Log("<color=yellow>lesItems[indexBtnCentered]</color>" + lesItems[indexBtnCentered]);
                    lerpToItem(-lesItems[indexBtnCentered]
                        .GetComponent<RectTransform>().anchoredPosition.x);
            }
        }
    }

    private void changeBtnSide(Transform itemTr, bool goToLeft)
    {
        float curX = itemTr.GetComponent<RectTransform>().anchoredPosition.x;
        float curY = itemTr.GetComponent<RectTransform>().anchoredPosition.y;
        Vector2 newAnchoredPos;
        if (goToLeft)
            newAnchoredPos = new Vector2(curX - (itemLength * itemDistance), curY);
        else
            newAnchoredPos = new Vector2(curX + (itemLength * itemDistance), curY);
        itemTr.GetComponent<ScrollLevelItem>().itemChangeSideScrollView(newAnchoredPos, !goToLeft);
    }


    private void lerpToItem(float position)
    {

        // on desactive inertia si n'est pas désactivé
        if (scrollRectLevels.inertia)
            scrollRectLevels.inertia = false;

        // timeFactor ne doit jamais dépasser 1 -> sinon depasse targetPos
        float timeFactor = Time.deltaTime *
            (Mathf.Abs(position - scrollPanel.anchoredPosition.x) < minDistanceToAccelerate ? accelerateLerpSpeed : normalLerpSpeed);
        timeFactor = timeFactor > 1 ? 1 : timeFactor;
        float newX = Mathf.Lerp(scrollPanel.anchoredPosition.x, position, timeFactor);

        if (Mathf.Abs(position - newX) == 0)
        {
            targetNearestItem = false;
        }

        if (Mathf.Abs(position - newX) < minDistanceToChangePos)
        {
            _scrollRect.StopMovement();
            newX = position;
            // reactiver les controle btn first & btn last
            btnFirst.interactable = true;
            btnLast.interactable = true;
        }

        if (!messageSend
            && Mathf.Abs(newX) >= Mathf.Abs(position) - minDistanceToChangePos && Mathf.Abs(newX) <= Mathf.Abs(position) + minDistanceToChangePos)
        {
            messageSend = true;
            itemIsCentered(indexBtnCentered);
            goingToItemByClick = false; // que quand click leftRight, mais pas de 'if' -> abréger
        }

        Vector2 newPosition = new Vector2(newX, scrollPanel.anchoredPosition.y);
        scrollPanel.anchoredPosition = newPosition;
    }

    // activer les bornes quand limite atteinte
    public void itemChangeNum(int numLevel, Transform trItem)
    {
        if (numLevel == lockedLevelNum - 1
            || (numLevel == maxNumLevel && !lockedOnRight)) // si levelBoundMax load right mais que limiteMax déja On pour level lock : ignorer levelBoundMax
        {
            lockedOnRight = true;
            levelMax_limit = trItem;
            currentLimit_toCompare = trItem;
        }
        else if (numLevel == minNumLevel)
        {
            lockedOnLeft = true;
            level1_limit = trItem;
        }
    }

    // appelé en fin de scroll quand item centré
    // réactiver btn gauche/droite en fonction sur borne ou non
    private void itemIsCentered(int itemIndex)
    {
        scrollRectLevels.inertia = true;

        currentNumLevel = lesItems[itemIndex].GetComponent<ScrollLevelItem>().numLevel;

        btnStart.SetActive(true);
        //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(true)</color>");
        // btns ont été désactiver début drag
        if (currentNumLevel == minNumLevel)
        {

            btnDroite.SetActive(true);
            // sauf si click btn gauche/droite
            if (goingToItemByClick)
                btnGauche.SetActive(false);
        }
        else if (currentNumLevel == lockedLevelNum - 1
            || currentNumLevel == maxNumLevel)
        {
            btnGauche.SetActive(true);
            if (goingToItemByClick)
                btnDroite.SetActive(false);
        }
        else
        {
            btnGauche.SetActive(true);
            btnDroite.SetActive(true);
        }
    }
    #endregion

    private bool goingToItemByClick = false; // true quand btn droit/gauche cliqué -> scroll sans drag

    /*
     * void goToItem_snap(int itemIndex)
     * IEnumerator btnGD_Clicked_enum()
     * void btnGaucheClicked()
     * void btnDroiteClicked()
    */
    #region Gestion event scroll (sans drag : btnGauche/Droite) 
    public void goToItem_snap(int itemIndex)
    {
        //Debug.Log("<color=blue>Debug SCROLL : goToItem_snap("+ itemIndex + ")</color>");
        messageSend = false;
        targetNearestItem = true;
        goingToItemByClick = true;
        indexBtnCentered = itemIndex;
        currentNumLevel = lesItems[itemIndex].GetComponent<ScrollLevelItem>().numLevel;
    }

    public float delayBetweenTwoClickGD = 0.3f;
    private bool btnLockedByDelay = false;
    private IEnumerator btnGD_Clicked_enum()
    {
        btnLockedByDelay = true;
        yield return new WaitForSeconds(delayBetweenTwoClickGD);
        btnLockedByDelay = false;
    }
    public void btnGaucheClicked()
    {
        if (currentNumLevel == minNumLevel) // click rapide sur btn -> depasse limite avant que btn desactivé
            return;

        if (!btnLockedByDelay)
        {
            StartCoroutine(btnGD_Clicked_enum());
            btnStart.SetActive(false);
            //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(false)</color>");

            if (!btnDroite.activeSelf) // quand était sur limite et revient en arrière
                btnDroite.SetActive(true);
            // List de 5 item qui ne changent jamais d'index dans leurs parents
            // Si sur btn1-lvl10-index0 , clique gauche , doit aller sur btn5-lvl10-index4
            goToItem_snap(indexBtnCentered == 0 ? lesItems.Length - 1 : indexBtnCentered - 1);
            _managerNiveau.btnG();
        }
    }
    public void btnDroiteClicked(bool goUnlockedLevel)
    {
        if (currentNumLevel == lockedLevelNum - 1
            || currentNumLevel == maxNumLevel)// click rapide sur btn -> depasse limite avant que btn desactivé
            return;

        if (goUnlockedLevel || !btnLockedByDelay)
        {
            StartCoroutine(btnGD_Clicked_enum());
            btnStart.SetActive(false);
            //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(false)</color>");

            /*if (!bordGaucheOn)
            {
                bordSombreAnim_gauche.Play("bordSombreLevelOn");
                bordGaucheOn = true;
            }*/
            if (!btnGauche.activeSelf) // quand était sur limite et revient en arrière
                btnGauche.SetActive(true);
            goToItem_snap(indexBtnCentered + 1 == lesItems.Length ? 0 : indexBtnCentered + 1);
            _managerNiveau.btnD();
        }
    }

    #endregion

    public Transform currentLimit_toCompare;
    // Gérer le déplacement du ScrollRect hors des limites
    // Cas 1 : pendant scroll sans drag
    // Cas 2 : pendant scroll avec drag
    /*
     *  bool isAboveLimits()
     *  IEnumerator reEnableScrollRectLate()
    */
    #region Gestion sortie limit du scrollRect 
    private bool isAboveLimits()
    {
        if (lockedOnLeft || lockedOnRight)
        {
            if (lockedOnLeft && level1_limit.position.x - center_left.position.x > 0)
            {
                // on désactive btn gauche
                if (btnGauche.activeSelf)
                    btnGauche.SetActive(false);
                currentLimit_toCompare = level1_limit;
                return true;
            }
            else if (lockedOnRight && levelMax_limit.position.x - center_right.position.x < 0)
            {
                // on désactive btn droite
                if (btnDroite.activeSelf)
                    btnDroite.SetActive(false);
                currentLimit_toCompare = levelMax_limit;
                return true;
            }
        }
        return false;
    }

    // appelé par onDrag quand depasse limite
    private IEnumerator reEnableScrollRectLate()
    {
        yield return null;
        _scrollRect.enabled = true;
    }
    #endregion


    public bool isScrollingList()
    {
        return !btnFirst.interactable;
    }
    /*
     * void startDrag()
     * void endDrag()
     * void onDrag()
    */
    #region Gestion event drag
    public void startDrag()
    {
        // si : drag / undrag -> snapItem / drag avant fin snapItem, inertia pas desactivé par itemCentered()
        scrollRectLevels.inertia = true;

        // désactiver les contrôles btn first & last
        btnFirst.interactable = false;
        btnLast.interactable = false;

        if (!scrollRectIsEnabled)
            return;
        isDragging = true;
        messageSend = false;
        goingToItemByClick = false;

        targetNearestItem = true;
        isSnapingItem = false;

        btnGauche.SetActive(false);
        btnDroite.SetActive(false);
        //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(false)</color>");
        btnStart.SetActive(false);
    }

    public void endDrag()
    {
        if (!scrollRectIsEnabled)
            return;
        isDragging = false;

        endDragCalculVitesse = true;
        minSpeedScrollPanToSnap_2 = 0f;
    }

    public void onDrag()
    {
        if (!scrollRectIsEnabled)
            return;
        if (isAboveLimits())
        {
            isDragging = false;
            _scrollRect.enabled = false;
            StartCoroutine(reEnableScrollRectLate());

            scrollPanel.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(-currentLimit_toCompare.GetComponent<RectTransform>().anchoredPosition.x, scrollPanel.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    public float offsetBord = 64f;
    public void scrollPosChanged_left(float posLevelLeft)
    {
        if(posLevelLeft >= offsetBord && bordGaucheOn)
        {
            bordGaucheOn = false;
            bordSombreAnim_gauche.Play("bordSombreLevelOff");
        }else if(posLevelLeft < offsetBord && !bordGaucheOn)
        {
            bordGaucheOn = true;
            bordSombreAnim_gauche.Play("bordSombreLevelOn");
        }
    }
    public void scrollPosChanged_right(float posLevelRight)
    {
        if (posLevelRight < -offsetBord && bordDroiteOn)
        {
            bordDroiteOn = false;

            //Debug.Log("Desactive bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOff");
        }
        else if (posLevelRight >= -offsetBord && !bordDroiteOn)
        {
            bordDroiteOn = true;
            //Debug.Log("Active bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOn");
        }
    }
    #endregion




    // ------------------------ GERER EVENT SUPLEMENTAIRE
    /*
     * Aller cash à un level précis : pour les boutons go bounded level
     * Debloquer un niveau et bloquer le suivant
     */


    public InputField lvlToGoInputField;
    public float timeAnimSwitch = 2f / 6f;
    public static bool isSwitching = false;
    /*
     * void btnGoToLevel()
     * void goToLevel(int numLevel)
     * void lockUnlockUi_byNumLevel(int numLevel)
     */
    #region Déplacement sur level precis depuis n'importe quel level
    public void btnGoToLevel()
    {
        if (isSwitching)
            return;
        StartCoroutine(animGoToLevel(int.Parse(lvlToGoInputField.text)));
    }

    private IEnumerator animGoToLevel(int numLevel)
    {
        btnStart.SetActive(false);
        //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(false)</color>");
        btnGauche.SetActive(false);
        btnDroite.SetActive(false);

        if (bordDroiteOn)
        {
            //Debug.Log("Desactive bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOff");
            bordDroiteOn = false;
        }
        if (bordGaucheOn)
        {
            bordSombreAnim_gauche.Play("bordSombreLevelOff");
            bordGaucheOn = false;
        }

        ScrollLevelItem.startSwitch();

        isSwitching = true;
        enableDisableScrollRect(false, false);
        foreach(Transform t in lesItems)
        {
            t.GetComponent<ScrollLevelItem>().levelSwitch_anim();
        }
        yield return new WaitForSeconds(timeAnimSwitch / 2f);
        goToLevel(numLevel);

        yield return new WaitForSeconds(timeAnimSwitch / 2f);
        btnStart.SetActive(true);
        //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(true)</color>");
        isSwitching = false;
        enableDisableScrollRect(true, false);
    }

    // se deplacer sur level précis depuis n'importe quel level
    private void goToLevel(int numLevel)
    {
        int nbLevelLeft_fromCenter;
        if (currentNumLevel < 3)
            nbLevelLeft_fromCenter = currentNumLevel == 2 ? 1 : 0;
        else if (currentNumLevel > maxNumLevel - 2)
            nbLevelLeft_fromCenter = currentNumLevel == maxNumLevel ? 4 : 3;
        else
            nbLevelLeft_fromCenter = 2;

        int indexLeft = indexBtnCentered - nbLevelLeft_fromCenter;
        if (indexLeft < 0)
            indexLeft = lesItems.Length + indexLeft;

        // Commencer loop sur index item left de lecran ( pas sur 0 de listItem )

        List<int> orderIndexChangeNum = new List<int>();
        int curIndex = indexLeft;
        while (orderIndexChangeNum.Count != lesItems.Length)
        {
            orderIndexChangeNum.Add(curIndex);
            curIndex = curIndex == lesItems.Length - 1 ? 0 : curIndex + 1;
        }

        int startNum = numLevel - nbLevelLeft_fromCenter;
        int newNum = startNum;

        // Attribuer à tout les boutons leurs nouveau numéro

        foreach (int i in orderIndexChangeNum)
        {
            //Debug.Log("(2) Associate new level, num : " + newNum + ", index : " + i);// (newNum - 1));
            lesItems[i].GetComponent<ScrollLevelItem>().setNumLevel(newNum, newNum < lockedLevelNum ? false : true, true);

            if (newNum == maxNumLevel
                || newNum == lockedLevelNum - 1)
                currentLimit_toCompare = lesItems[i];
            // si numItem > max -> deplacer de lautre côté
            if (newNum > maxNumLevel)
                changeBtnSide(lesItems[i], true);
            // si numItel < min -> deplacer de lautre côté
            if (newNum < 1)
                changeBtnSide(lesItems[i], false);
            newNum++;
        }

        ScrollLevelItem.endSwitchDisableItems();
        // UPDATE variables limit & lock après changement de numéro ( et de place sur ecran )
        lockUnlockUi_byNumLevel(numLevel);
        currentNumLevel = numLevel;
    }

    private void lockUnlockUi_byNumLevel(int numLevel)
    {
        if (numLevel > 3) // 1 disparait de l'ecran quand 4 au milieu
        {
            lockedOnLeft = false;
            level1_limit = null;
        }

        if (
            ((lockedLevelNum > maxNumLevel && numLevel <= maxNumLevel - 3)) // cas sans lock level
            || (lockedLevelNum <= maxNumLevel && numLevel < lockedLevelNum - 3) // cas avec lock level // ( <= ) avant test réels
            )
        {
            // 72, 1001, 72
            //Debug.Log("Scroll pas lock a droite, numLevel : "+numLevel+", lockedLevelNum : "+lockedLevelNum+", maxNum : "+maxNumLevel);
            lockedOnRight = false;
            levelMax_limit = null;
        }


        bool activateLeft = !(numLevel == minNumLevel);
        bool activateRight = !(numLevel == maxNumLevel
                || numLevel == lockedLevelNum - 1);
        bool activateOmbreDroite = numLevel != maxNumLevel;
        // Recheck LOCK des btns en fonctions pos sur ecran
        if (activateOmbreDroite)
        {
            if (!bordDroiteOn)
            {
                //Debug.Log("Active bord sombre droite");
                bordSombreAnim_droite.Play("bordSombreLevelOn");
                bordDroiteOn = true;
            }
        }
        if (activateLeft)
        {
            if (!bordGaucheOn)
            {
                bordSombreAnim_gauche.Play("bordSombreLevelOn");
                bordGaucheOn = true;
            }
        }

        btnGauche.SetActive(activateLeft);
        if (!activateRight)
        {
            btnDroite.SetActive(false);
            currentLimit_toCompare = lesItems[indexBtnCentered];
        }
        else
        {
            btnDroite.SetActive(true);
        }
    }
    #endregion


    private int curUnlockLvlNum;// enregistrer lequel a été unlock : si click notif go unlocked
    /*
     * void btnChangeToNextLock()
     * void levelIsUnlocked(int numNewLock)
     * void btnGoToUnlockedLevel()
     * void goToUnlockedLevel()
    */
    #region Unlock level , aller dessus si click notif
    public void btnChangeToNextLock() // appelé quand unlock level, mettre la limite suivante
    {
        levelIsUnlocked(lockedLevelNum + 3 > maxNumLevel ? 1000 : lockedLevelNum + 3);
    }

    public void resetTextLock()
    {
        ScrollLevelItem i;
        foreach(Transform t in lesItems)
        {
            i = t.GetComponent<ScrollLevelItem>();
            i.setLock(i.isLocked, true);
        }
    }

    public void updateTextLock()
    {
        ScrollLevelItem i;
        int nbC = SaveSystem.nb_couronne;
        int nbS = SaveSystem.getNbStarObtain();
        foreach (Transform t in lesItems)
        {
            i = t.GetComponent<ScrollLevelItem>();
            i.updateLock(nbC, nbS);
        }
    }

    public void levelIsUnlocked(int numNewLock)
    {
        // numNewLock : niveau qui vient d'être débloqué
        Debug.Log("(SCROLL) Nouveau niveau débloqué : " + numNewLock);
        // on met à jour le lock des btns sur l'écran
        foreach(Transform t in lesItems)
        {
            t.GetComponent<ScrollLevelItem>().aLevelIsUnlocked(numNewLock);
        }

        curUnlockLvlNum = lockedLevelNum;
        lockedLevelNum = numNewLock;
        //curUnlockLvlNum = lockedLevelNum;

        if (currentNumLevel < maxNumLevel - 2) // enlever lock si maxLevel n'est pas présent sur lécran
        {
            levelMax_limit = null;
            lockedOnRight = false;
            Debug.Log("Max level not on screen, scroll right not locked");
        }
        else // sinon, mettre à jour la nouvelle limite qui est maxLevel
        {
            foreach(Transform t in lesItems)
            {
                if(t.GetComponent<ScrollLevelItem>().numLevel == maxNumLevel)
                {
                    levelMax_limit = t;
                    currentLimit_toCompare = t;
                    break;
                }
            }        
        }
        // mettre a jour btnDroit si etait a coté de lockLevel
        if (currentNumLevel == curUnlockLvlNum - 1)
            btnDroite.SetActive(true);
    }

    public void btnGoToUnlockedLevel()
    {
        goToUnlockedLevel();
    }

    private void goToUnlockedLevel()
    {
        // si le niveau est sur l'ecran, on le snap
        if (curUnlockLvlNum - currentNumLevel <= 2)
        {
            if (!btnGauche.activeSelf) // quand était sur limite et revient en arrière
                btnGauche.SetActive(true);
            int diffNumLevel = curUnlockLvlNum - currentNumLevel;
            while (diffNumLevel > 0)
            {
                diffNumLevel--;
                btnDroiteClicked(true);
            }
        }
        // sinon on goToLevel()
        else
        {
            StartCoroutine(animGoToLevel(curUnlockLvlNum));
        }
    }
    #endregion



    // managerNiveau l'appelle
    public void levelFinished_goNext()
    {
        if (currentNumLevel < maxNumLevel)
            btnDroiteClicked(false);
    }



    private bool prevState_btnGauche, prevState_btnDroite;
    private bool scrollRectIsEnabled = true;
    /*
     * void btnCloseAllLevels()
     *  void closeLevelsToInfinite()
     * void btnOpenAllLevels()
     *  void openLevelsFromInfinite()
     *  
     *  void btnStartClicked()
     *  void gameEnd_reopenLevels()
     *  
     *  void enableDisableScrollRect(bool isEnable)
    */
    #region ANIM fermeture/ouverture scroll, start/end game
    public void btnCloseAllLevels()
    {
        closeLevelsToInfinite();
    }
    public void closeLevelsToInfinite()
    {
        foreach (Transform t in lesItems)
            t.GetComponent<ScrollLevelItem>().closeLevel();
        enableDisableScrollRect(false, false);
    }

    public void btnOpenAllLevels()
    {
        openLevelsFromInfinite();
    }
    public void openLevelsFromInfinite()
    {
        foreach (Transform t in lesItems)
            t.GetComponent<ScrollLevelItem>().openLevel();
        enableDisableScrollRect(true, true);
    }


    public void btnStartClicked()
    {
        // on ferme tout les niveaux sauf celui du centre
        foreach(Transform t in lesItems)
        {
            if (t.GetSiblingIndex() != indexBtnCentered)
                t.GetComponent<ScrollLevelItem>().closeLevel();
        }
        enableDisableScrollRect(false, false);
        StartCoroutine(disableItemLate_onStart());
    }

    private IEnumerator disableItemLate_onStart()
    {
        yield return new WaitForSeconds(9f / 60f);//scrollLevelItem:timeAnimClose
        ScrollLevelItem.isInGameMode = true;
    }

    // appelé par managerNiveau
    public void gameEnd_reopenLevels()
    {
        ScrollLevelItem.isInGameMode = false;

        // on ouvre tout les niveaux sauf celui du centre déja ouvert
        foreach (Transform t in lesItems)
        {
            if (t.GetSiblingIndex() != indexBtnCentered)
                t.GetComponent<ScrollLevelItem>().openLevel();
        }
        enableBordSombre_endGame();
    }

    // en revenant du mode jeu, on reactive scrollRect une fois couronne et etoile reccupere
    public void gameEnd_allRewardGiven()
    {
        //Debug.Log("<color=blue>gameEnd_reopenLevels():enableDisableScrollRect(true)</color>");
        enableDisableScrollRect(true, false);
    }


    public Image[] disableRaycastImages; 

    private void enableBordSombre_endGame()
    {
        if (bordDroiteOn)
        {
            //btnDroite.SetActive(true);
            //Debug.Log("Active bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOn");
            //bordDroiteOn = true;
        }
        if (bordGaucheOn)
        {
            //btnGauche.SetActive(true);
            bordSombreAnim_gauche.Play("bordSombreLevelOn");
            //bordGaucheOn = true;
        }
    }
    private void enableDisableScrollRect(bool isEnable, bool backFromInfinis)
    {       
        if (isEnable)
        {
            // pb, si est sur level lock, va infinis
            // -> prevState_btnDroite = false;
            // --> ne reactive pas bordSombreDroite

            if (prevState_btnDroite)
            {
                btnDroite.SetActive(true);
                if (!bordDroiteOn || backFromInfinis)
                {
                    //Debug.Log("Active bord sombre droite");
                    bordSombreAnim_droite.Play("bordSombreLevelOn");
                    bordDroiteOn = true;
                }
            }
            else if (bordDroiteOn && backFromInfinis)
            {
                bordSombreAnim_droite.Play("bordSombreLevelOn");

            }


            if (prevState_btnGauche)
            {
                btnGauche.SetActive(true);
                if (!bordGaucheOn || backFromInfinis)
                {
                    bordSombreAnim_gauche.Play("bordSombreLevelOn");
                    bordGaucheOn = true;
                }
            }
            btnStart.SetActive(true);
            //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(true)</color>");
        }
        else
        {
            prevState_btnGauche = btnGauche.activeSelf;
            prevState_btnDroite = btnDroite.activeSelf;

            if (prevState_btnGauche)
            {
                btnGauche.SetActive(false);
            }
            if (prevState_btnDroite)
            {
                btnDroite.SetActive(false);
            }

            if (bordGaucheOn)
            {
                bordSombreAnim_gauche.Play("bordSombreLevelOff");
                //bordGaucheOn = false;
            }
            if (bordDroiteOn)
            {
                //Debug.Log("Active bord sombre droite");
                bordSombreAnim_droite.Play("bordSombreLevelOff");
                //bordDroiteOn = false;
            }

            btnStart.SetActive(false);
            //Debug.Log("<color=green>Debug SCROLL : btnStart.setActive(false)</color>");
        }

        scrollRectIsEnabled = isEnable;
        foreach (Image i in disableRaycastImages)
            i.raycastTarget = isEnable;
        //scrollPanel.gameObject.SetActive(isEnable);
        _scrollRect.enabled = isEnable;
        _scrollRect.transform.GetComponent<Image>().raycastTarget = isEnable;
    }
    #endregion


    // appelé par managerNiveau
    public void initGame_void(int initLevel)
    {
        // on active le scroll quand menu initialisé
        gameObject.SetActive(true);
        maxNumLevel = SaveSystem.nbNiveauAventure;

        // aller direct à un level précis
        initializeOnPreciseLevel(initLevel);
        currentNumLevel = initLevel;

        if (currentNumLevel == minNumLevel)
        {
            btnGauche.SetActive(false);

            //Debug.Log("Active bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOn");
            bordDroiteOn = true;

        }
        else if (currentNumLevel == lockedLevelNum - 1
            || currentNumLevel == maxNumLevel)
        {
            btnDroite.SetActive(false);

            bordSombreAnim_gauche.Play("bordSombreLevelOn");
            bordGaucheOn = true;

        }
        else
        {
            //Debug.Log("Active bord sombre droite");
            bordSombreAnim_droite.Play("bordSombreLevelOn");
            bordDroiteOn = true;
            bordSombreAnim_gauche.Play("bordSombreLevelOn");
            bordGaucheOn = true;
        }
    }

    // appelé après initGame
    // appelé par managerNiveau quand
    // init LockNum, change LockNum
    public void setLockNum(int lockNum)
    {
        if (lockNum == lockedLevelNum)
            return;
        //Debug.Log("Set num level lock scroll : " + lockNum);
        lockedLevelNum = lockNum;

        ScrollLevelItem item;
        // activer lock sur levels si necessaire
        foreach (Transform t in lesItems)
        {
            item = t.GetComponent<ScrollLevelItem>();
            item.aLevelIsUnlocked(lockNum);
            itemChangeNum(item.numLevel, t);
        }

        lockUnlockUi_byNumLevel(currentNumLevel);
    }

    public void goToBoundedLevel(int numLevel)
    {
        if (isSwitching)
            return;
        StartCoroutine(animGoToLevel(numLevel));
    }
}


// 939 lignes -> 822 lignes
// 925 l -> 880 l
