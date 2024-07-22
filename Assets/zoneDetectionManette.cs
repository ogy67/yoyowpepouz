using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class zoneDetectionManette : MonoBehaviour, IPointerDownHandler,
    IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerUpHandler
{
    public static bool isOnPc;
    public btnManette btnManetteAssocie;
    public Transform btnGoAssocie;
    public typeButtonManette typeButton;

    public playerMovment pM;


    private void Start()
    {
        isOnPc = Application.isEditor;
        if(typeButton == typeButtonManette.btnA)
            getDistanceMinSwitch(); // initialise
    }
    // Update() : que btnA
    public bool playerDraggingBtn = false;
    private void Update()
    {
        if (typeButton == typeButtonManette.btnA
            && playerDraggingBtn)
        {
            if (playerMovment.hasReleased)
            {
                pM.btnManette_tryHang();
            }
            else
            {
                 // pb spécial
                // desactiver playerdraggingBtn quand zoneOff
                pM.btnManetteFirstClickInitCheckP();
            }
        }
    }

    public bool pointerDown = false;


    private bool waitingFirstClick_initPos = false;
    private Vector2 firstClickPos = Vector2.zero;
    public preshotSwitchPlayer preshotSwitch;
    //public Transform posBtnAInitDebug;
    // que pour btnA
    public void manetteActivated_resetClickZone()
    {
        nbPointerDownInZone = 0;

        // on met btnA sur posInit precedente
        if(typeButton == typeButtonManette.btnA)
            //&& firstClickPos != Vector2.zero)
        {
            //Debug.Log("RESET btn A sur pos INIT");
            waitingFirstClick_initPos = true;

            if (firstClickPos != Vector2.zero)
            {
                btnGoAssocie.position = firstClickPos;
                //posBtnAInitDebug.position = firstClickPos;
                checkIfBtnIsInSafeArea();
            }
            preshotSwitch.playerPop_btnAOpen();
        }      
    }

    public int nbPointerDownInZone = 0;
    private int idPointerCanSlide = -1;
    // detecter click down
    public void OnPointerDown(PointerEventData eventData)
    {
        // /!\ Input.touchDown prend en compte tout les click sur l'écran
        // normalement clique B quand A enfoncé : A met Input.touchCount à 1 d'office
        nbPointerDownInZone++;

        bool isValidPointer = nbPointerDownInZone == 1;
        //. //
        // Debug.Log("Btn down (" + typeButton + "), nbPointerDownInZone : "+nbPointerDownInZone+", isValidPointer ? "+isValidPointer);
        if (isValidPointer)
            idPointerCanSlide = eventData.pointerId;

        // d'abord, on regarde si c'est un clique valide
        if (btnManetteAssocie.btnActivated)
        {
            if (!isOnPc)
            {               
                if (!isValidPointer) return;
            }
            else
                if (!Input.GetMouseButtonDown(0)) return;
        }
        else
        {
            if (typeButton == typeButtonManette.btnB)
            {
                GetComponent<btnManette>().clickDownPreshotBtnB(eventData.position);
                pointerDown = true;

            }
            return;
        }


        pointerDown = true;
        // si btn activé , repositionner au clique
        if (btnManetteAssocie.btnActivated)
        {
            playerDraggingBtn = true;

            Vector2 newPos = eventData.position; //getPosClick();
            btnGoAssocie.position = newPos;

            if (typeButton == typeButtonManette.btnA)
                preshotSwitch.btnAClickDown();

            // check si hors tuto, ou alors dans tuto et derniere phase
            if (typeButton == typeButtonManette.btnA 
                && playerMovment.hasReleased == false
                && manetteM.menuTuto.canDisplaySwitchBtnA())
            {
                openZoneSwitch(newPos);
                StartCoroutine(animZoneSwitchEnum(false, false));
                //animZoneSwitch.Play("switchPlayerUi");.
            }


            // on doit regarder si player est sur init checkP
            // => activer direct fleche et btnB
            if (typeButton == typeButtonManette.btnA
                && playerMovment.firstCheckP)
            {
                manetteM.playerOnfirstCheckP();

                /*if (TutoManager.tutoOn)
                {
                    TutoManager.getInstance().playerFirstClick();
                }*/

                if (waitingFirstClick_initPos)
                {
                    waitingFirstClick_initPos = false;
                    firstClickPos = eventData.position;
                }
            }

            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.manette_clickDownBtn);
        }

        // une fois repositionné , les btn manette sont appelés
        btnManetteAssocie.onPointerDown_custom(eventData);
    }

    /*private Vector2 getPosClick()
    {
        Vector2 newPos = Vector2.zero;
        if (!isOnPc)
        {
            if (Input.touchCount < 2)
            {
                // repositionner le btn en fonction de l'endroit ou a cliqué
                Touch t = typeButton == typeButtonManette.btnA ?
                    Input.GetTouch(0)  // on regarde ou est le 1er click
                    : Input.GetTouch(1);  // on regarde ou est le 2e click

                .// attention , ne plus regarder pos click avec Input

                newPos = t.position;
            }
        }
        // PC
        else
        {
            if (Input.GetMouseButtonDown(0))
                newPos = Input.mousePosition;
        }
        return newPos;
    }*/

    public void OnPointerUp(PointerEventData eventData)
    {
        nbPointerDownInZone--;
        // quand pop manette, reset click à zero, si clique up appelé, fait descendre en dessous de 0
        if (nbPointerDownInZone < 0) nbPointerDownInZone = 0;



        if (!pointerDown)
            return;

        pointerDown = false;

        bool isValidPointer = eventData.pointerId == idPointerCanSlide;

        // appeler preclick off, si uncklick alors que manette pas active
        if (isValidPointer) {
            if (typeButton == typeButtonManette.btnB
                && !btnManetteAssocie.btnActivated)
            {
                GetComponent<btnManette>().clickUpPreshotBtnB(eventData.position);
                idPointerCanSlide = -1;
                return;
            }
        }
      

        // d'abord, on regarde si c'est un clique valide
        if (btnManetteAssocie.btnActivated)
        {
            if (!isOnPc)
            {
                if (!isValidPointer) return;
            }
            else
                if (!Input.GetMouseButtonUp(0)) return;
        }
        else
        {
            return;
        }
        // si btn activé
        if (btnManetteAssocie.btnActivated)
        {
            playerDraggingBtn = false;
        }

        idPointerCanSlide = -1;
        btnManetteAssocie.onPointerUp_custom(eventData);

        if(manetteM.menuTuto.canDisplaySwitchBtnA())
            closeZoneSwitch();
        checkIfBtnIsInSafeArea();
    }

    // seulement appelé par btn B
    public void preshotClickReposBtn(Vector2 posClick)
    {
        if(posClick != Vector2.zero)
            btnGoAssocie.position = posClick;// getPosClick();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!btnManetteAssocie.btnActivated)
            return;

        // PB , si btn B , input.GetTouch(0) est sur btn A et pas B ( qui est alors 1 )
        // Resolution ; pas simplement B = 1 et A = 0.
        // Si 3 clic sur A, puis sur B, B = 4
        //bool dragWithFirstClick = eventData.pointerId == idPointerCanSlide;
        if (eventData.pointerId != idPointerCanSlide)
            return;

        // si est btnB, et btnB pas activé, rien faire
        // si pas sur checkP , btn doit suivre position de drag ( sans dépasser les limites )

        // si btn B activated, btn A bouge seulement horizontalement && btnB bouge partout
        // Si btnB pas activé, btn A bouge partout

        // deplacement sur x et sur y
        // si : dans tuto et peut pas encore switch, enregistrer pos drag comme init pos

        if (typeButton == typeButtonManette.btnB
            || (typeButton == typeButtonManette.btnA && !btnManetteB.btnActivated))  
            //|| (typeButton == typeButtonManette.btnA && TutoManager.tutoOn && TutoManager.currentPhase.type_phase != typeTutoPhase.changeRotationSide))
        {
            // Input.Touch
            // créer variables myValidTouchId, enregistre x de Input.GetTouch(x)
            // se desincrémente à n'importe quel clic Up

            Vector2 posClick = isOnPc ? (Vector2)Input.mousePosition : eventData.position; // Input.GetTouch(0).position;
                                                                                           // ne plus utiliser input.getTouch

            if (posClick.x < posMinZone.position.x)
                posClick.x = posMinZone.position.x;
            else if (posClick.x > posMaxZone.position.x)
                posClick.x = posMaxZone.position.x;

            if (posClick.y < posMinZone.position.y)
                posClick.y = posMinZone.position.y;
            else if (posClick.y > posMaxZone.position.y)
                posClick.y = posMaxZone.position.y;

            // on déplace le groupe bouton
            // on ne doit pas depasser les limites de la zone clique
            btnGoAssocie.position = posClick;
        }
        // deplacement seulement sur x
        else if (typeButton == typeButtonManette.btnA)
        {
            
                // repositionner btn A horizontalement
                checkIfSwitch();
        }
    }
    // tr1 : dans le coin bas gauche : x = posMinX && y = posMinY
    // tr2 : dans le coin haut droit : x = posMaxX && y = posMaxY
    public Transform posMinZone, posMaxZone;
    public Transform posMinSafeZone, posMaxSafeZone;

    [Header("Zone btnA Only filled")]
    /*
     * void checkIfSwitch()
     * void OnEndDrag(PointerEventData eventData)
     * void OnBeginDrag(PointerEventData eventData)
     * 
     * void playerDraggedCheckP()
     * IEnumerator animZoneSwitchEnum()
     * float getDistanceMinSwitch()
     * void closeZoneSwitch()
     * void openZoneSwitch(Vector2 posSwitch)
     * */
    #region Gestion drag A
    public Transform maxPosSlide;
    public Transform btnAImg;
    public Transform arrow_content;
    public Transform arrow_image;
    public Transform middleOfZoneTr;
    public btnManette btnManetteB;
    public Animation btnAnimation;
    public manetteManager manetteM;
    // distance minimale pour pouvoir changer la position d'un bouton
    public float distanceMinToMoveBtn = 50f;



    private Vector2 posBtnASwitch;

    private void checkIfSwitch() // controll, que pour btnA, appelé si switch est activable
    {
        float posClickX = isOnPc ? Input.mousePosition.x : Input.GetTouch(0).position.x;
        float posClickY = isOnPc ? Input.mousePosition.y : Input.GetTouch(0).position.y;

        // d'abord, verifier que btnA ne depasse pas les limites
        float posY_inBound = posClickY < posMinZone.position.y ? posMinZone.position.y
            : posClickY > posMaxZone.position.y ? posMaxZone.position.y
            : posClickY;
        float posX_inBound = posClickX < posMinZone.position.x ? posMinZone.position.x
            : posClickX > posMaxZone.position.x ? posMaxZone.position.x
            : posClickX;

        btnGoAssocie.position = new Vector2(posX_inBound, posY_inBound);
        Vector2 newPosBtn = btnGoAssocie.position;


        float distance = Vector2.Distance(posBtnASwitch, newPosBtn);
        if (manetteM.menuTuto.canDisplaySwitchBtnA() 
            && distance >= getDistanceMinSwitch())
        {
            if (animSwitchOn)
            {
                //Debug.Log("<color=red>ANIM SWITCH PAR DESSUS SWITCH</color>");
            }
            else
            {
                // switch
                posBtnASwitch = btnGoAssocie.position;
                switchAreaGo.transform.position = newPosBtn;
                StartCoroutine(animZoneSwitchEnum(false, false));
                pM.btnChangeRotation();
            }
        }
    }

    public void checkIfBtnIsInSafeArea()//PointerEventData eventData)
    {
        // si sur pc, et que btnB, ne pas checkPosition
        if (isOnPc && typeButton == typeButtonManette.btnB)
        {
            // ne rien faire
        }
        else
        {
            Vector2 posSafe = btnGoAssocie.position;//isOnPc ? (Vector2)Input.mousePosition : eventData.position;
            bool posOutOfSafeArea = posSafe.x < posMinSafeZone.position.x
                || posSafe.x > posMaxSafeZone.position.x
                || posSafe.y < posMinSafeZone.position.y
                || posSafe.y > posMaxSafeZone.position.y;

            if (posOutOfSafeArea)
            {
                if (posSafe.x < posMinSafeZone.position.x)
                    posSafe.x = posMinSafeZone.position.x;
                else if (posSafe.x > posMaxSafeZone.position.x)
                    posSafe.x = posMaxSafeZone.position.x;

                if (posSafe.y < posMinSafeZone.position.y)
                    posSafe.y = posMinSafeZone.position.y;
                else if (posSafe.y > posMaxSafeZone.position.y)
                    posSafe.y = posMaxSafeZone.position.y;

                btnGoAssocie.position = posSafe;
            }
        }
    }
    // /!\ OnEndDrag() est appelé après clickUp
    // => btnManetteB.btnActivated = false tt le temps
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!btnManetteAssocie.btnActivated)
            return;

        // à la fin du drag, on regarde si la position du bouton est dans la safe zone
        // peut importe quel click appelle cette fonction
        checkIfBtnIsInSafeArea();


        // que pour btnA
        if (typeButton != typeButtonManette.btnA)
            return;

        resetPosBtnA_endDrag();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // que pour btnA
        if (typeButton != typeButtonManette.btnA)
            return;

        if (!btnManetteB.btnActivated)
            return;

        posBtnASwitch = btnGoAssocie.position;
    }

    // le bouton reste sur la dernière position drag de player
    public void resetPosBtnA_endDrag()
    {
        if (manetteM.menuTuto.canDisplaySwitchBtnA())
            closeZoneSwitch();
    }



    public GameObject switchAreaGo;
    public Transform posDistanceSwitch;
    public Animation animZoneSwitch;
    public checkBtnAMoving checkMoving;

    public void playerDraggedCheckP(bool preshot_switch)
    {
        Vector2 posReposSwitchArea = !preshot_switch ?
            checkMoving.isBtnMoving() ? preshotSwitch.getPosItemFollow() : (Vector2)btnGoAssocie.position
            : (Vector2)btnGoAssocie.position;
        posBtnASwitch = posReposSwitchArea;

        if (manetteM.menuTuto.canDisplaySwitchBtnA())
        {
            openZoneSwitch(posReposSwitchArea);
            StartCoroutine(animZoneSwitchEnum(preshot_switch, true));
        }
    }

    // pn zone switch pas bien positionnié quand pop -> double appel

    public float timeAnimZoneSwitch = 2 / 6f;
    public float timeFollowPos = 15 / 60f;
    private bool animSwitchOn = false;
    private IEnumerator animZoneSwitchEnum(bool preshot_switch, bool hangCheckP)
    {
        // ne pas mettre animSwitchOn = true quand hang checkP


        // continue de suivre btn A quand pop hang

        bool followPos = !hangCheckP || preshot_switch;
        /*if (preshot_switch)
        {
            animZoneSwitch.Play("switchPlayerUi");
            animSwitchOn = true;
            yield return new WaitForSeconds(timeAnimZoneSwitch);
            animSwitchOn = false;
        }
        else */if(followPos)
        {
            // la zone doit suivre btn A jusqu'à la fin de l'anim 
            float initT = Time.time;
            animZoneSwitch.Play("switchPlayerUi");
            animSwitchOn = true;
            // coupe la boucle si
            // - fin timer
            // - player relache btn A
            // - player meurt ( horloge )
            while (Time.time - initT < timeAnimZoneSwitch
                && zoneSwitchOpen
                && btnManetteAssocie.btnActivated)
            {
                if (Time.time - initT < timeFollowPos)
                {
                    switchAreaGo.transform.position = btnGoAssocie.position;
                    posBtnASwitch = btnGoAssocie.position;
                }
                yield return null;
            }
            animSwitchOn = false;
        }
    }

    private float getDistanceMinSwitch()
    {
        return Vector2.Distance(switchAreaGo.transform.position, posDistanceSwitch.position);
    }
    private bool zoneSwitchOpen = false;
    private void openZoneSwitch(Vector2 posSwitch)
    {
        if (!zoneSwitchOpen)
        {
            zoneSwitchOpen = true;
            switchAreaGo.SetActive(true);
            switchAreaGo.transform.position = posSwitch;
        }
    }

    private void closeZoneSwitch()
    {
        if (zoneSwitchOpen)
        {
            zoneSwitchOpen = false;
            switchAreaGo.SetActive(false);
        }
    }
    #endregion




}


// 559 l -> 336 l
