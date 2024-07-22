using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum typeButtonManette
{
    btnA,
    btnB
}
public class btnManette : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // test pour btn b sur pc
    // quand btn B clavier préssé, simuler un clique sur écran
    private KeyCode slowCheckPKey = KeyCode.Space;
    // Update is called once per frame
    void Update()
    {
        if(typeButton == typeButtonManette.btnB)
        {
            if (Input.GetKeyDown(slowCheckPKey))
            {
                if (!btnActivated)
                {
                    clickDownPreshotBtnB(Vector2.zero);
                    // desactiver pointer down si clique pas valide ?
                    zoneDetectionAssocie.pointerDown = true;
                    preshotClickDown = false; // cash le redesactiver, avec pc que en clique, pas drag
                    //return;
                }
                else
                {
                    //. // /!\ , doit passer par verif de clique
                    
                    // Clique down
                    if (Input.GetKeyDown(slowCheckPKey))
                    {
                        // on inverse btnB ( allumé <-> etteint )

                        myBtnImg.color = btnBOn ? colorUnclicked : colorClicked;
                        btnBOn = !btnBOn;
                        // on active ou desactive le alo
                        myAnimation.Play(btnBOn ? "btnManetteBAloOn" : "btnManetteBAloOff");
                        if(btnBOn)
                            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.manette_clickDownBtn);
                        // on active ou desactive la super visee en fonction de btnBOn
                        gM.btnBClicked();
                    }
                    // Clique up
                    if (Input.GetKeyUp(slowCheckPKey))
                    {
                        // ne rien faire
                    }
                }
            }
                
        }
    }

    public typeButtonManette typeButton;
    public Animation myAnimation, activateAnim; // activate separé pour pas interferer dans anim
    public Image myBtnImg;
    public Color32 colorClicked, colorUnclicked;
    public bool btnActivated = false; // pour zoneDetection
    public gameManager gM;
    public playerMovment pM;

    [Header("Btn A Only filled")]
    public zoneDetectionManette zoneDetectionAssocie;
    public boostIngameManager boostIngameM;
    private static float timeDoubleClick = 0.5f;
    private float prevClickTime = 0f;
    private float prevSuperBoostActivationTime = 0f;

    // btnB Only
    private bool btnBOn = false;

    // /!\ Player die -> drag zoneA , btnAPop , undrag -> releaseCheckP ( devrait pas )
    private bool btnClickedDown = false;


    public static bool hasHangOnClick = false;


    // QUE pour btn B
    public bool preshotOn = false;
    private bool preshotClickDown = false;
    private Vector2 preshotPos;
    private float timePreshotClick;
    public float timePreshot = 0.5f;

    public void clickDownPreshotBtnB(Vector2 pos)
    {
        //Debug.Log("<color=blue>Register try preshot click B</color>");
        timePreshotClick = Time.time;
        preshotClickDown = true;
        preshotOn = true;
        preshotPos = pos;
    }

    public void clickUpPreshotBtnB(Vector2 pos)
    {
        // on enregistre le dernier temps
        timePreshotClick = Time.time;
        preshotClickDown = false;
        preshotPos = pos;
        // on attend ensuite 
        // V soit hang ( manette active )
        // soit manette desactivé
        // si desactivé, reset preshot
    }

    // NOUVELLE VERSION : hang checkP prioritaire sur activation de super boost
    public void onPointerDown_custom(PointerEventData eventData)
    {
        //Debug.Log("<color=yellow>Pointer down btn </color> , btnActivated : "+btnActivated + Time.time);
        if (!btnActivated)
            return;

        btnClickedDown = true;

        // BTN A
        if (typeButton == typeButtonManette.btnA)
        {
            bool boostIsLocked = !infinisManager.infinis_Mode_On && expertMode.modeExpertOn;

            // illuminer le boutton ( seulement alpha change )
            myBtnImg.color = new Color(myBtnImg.color.r, myBtnImg.color.g, myBtnImg.color.b, 1f);
            //Debug.Log("<color=green>Btn A allumé ( clique dessus ) </color> , "+Time.time);

            // check si super boost
            // ne pas checkFusee
            // ne pas checkHang

            // si hang : pas de fusee, ni super boost
            hasHangOnClick = false;

            if (playerDetector.checkpoint != null)
                pM.btnManette_tryHang();

            if (hasHangOnClick) // set by playerMovment.Hang() quand hang
            {
                //Debug.Log("Has hang on click " + Time.time);
                hasHangOnClick = false;
            }
            else if (!boostIsLocked) // SUPER BOOST ET FUSEE
            {
                //Debug.Log("Try controll super BOOST " + Time.time);
                // Si activé ne pas pM.btnManette_tryHang() ni activateFuseeLate()
                /*float t = Time.time - prevClickTime;
                if (t > 0.05f && t <= timeDoubleClick) // > 0.5f pcq sans raison est appelé 2 fois de suite en 1 clicl
                {
                    //Debug.Log("Contrôle superBoost");
                    // si peut activer le superBoost, on l'active
                    if (boostIngameM.superPowerClicked())
                    {
                        prevClickTime = Time.time;

                        // /!\ , si est deja en anim btnActivate, bloque anim -> btn mauvaise scale
                        myAnimation.Play("btnManetteSBoostOn");
                        prevSuperBoostActivationTime = Time.time;
                        //StartCoroutine(testSuperBoost());
                        return;
                    }
                    //boostIngameManager.getInstance().superPowerClicked();
                }*/

                prevClickTime = Time.time;

                // FUSEE
                if (!boostIsLocked
                    && boostIngameManager._fuseeOn && !fusee_on
                    && playerMovment.hasReleased == true
                    && pM.canUseFusee() // verifie si player peut Hang checkP
                    && boostIngameManager.superPowerOn == typeBoostGame.none // ne s'active pas si superBoost on
                    )
                {
                    // Deux cas de figure
                    // - Soit player inCheckP
                    //      => ne pas essayer d'activer fusee
                    // - Soit player outCheckP
                    //   /!\ est AUSSI considéré outCheckP quand vient de relacher un checkP ET est encore dessus
                    //      => essayer d'activer fusee
                    StartCoroutine(activateFuseeLate(Application.platform == RuntimePlatform.Android
                        || Application.platform == RuntimePlatform.IPhonePlayer));
                }
            }
            if (!boostIngameManager._bouclierOn && !boostIngameManager._superViseurOn && !boostIngameManager._bombeOn)
                myAnimation.Play("btnManetteBAloOn");

        }
        // BTN B
        else
        {
            // on inverse btnB ( allumé <-> etteint )
            myBtnImg.color = btnBOn ? colorUnclicked : colorClicked;
            btnBOn = !btnBOn;
            // on active ou desactive le alo
            myAnimation.Play(btnBOn ? "btnManetteBAloOn" : "btnManetteBAloOff");

            // on active ou desactive la super visee en fonction de btnBOn
             // active / desactive tortue
            gM.btnBClicked();
        }
    }

    // detecter click up
    public void onPointerUp_custom(PointerEventData eventData)
    {
        //Debug.Log("<color=yellow>Pointer up btn </color>, btnActivated : " + btnActivated+Time.time);
        if (!btnActivated)
            return;

        if (!btnClickedDown)
        {
            return;
        }
        btnClickedDown = false;

        //Debug.Log("Btn manette Unclicked");

        if (typeButton == typeButtonManette.btnA)
        {

            //float prevAlpha = myBtnImg.color.a;

            //myAnimation.Play("btnManetteToTransparent");
            // Des-illuminer le boutton
            //Debug.Log("Click Up btn A, remet color off ("+Time.time);
            myBtnImg.color = new Color(myBtnImg.color.r, myBtnImg.color.g, myBtnImg.color.b, 150/255f);

            // si pas anim superboost
            if (!boostIngameManager._bouclierOn && !boostIngameManager._superViseurOn && !boostIngameManager._bombeOn)
                myAnimation.Play("btnManetteBAloOff");

            // Verifier si pas click up directement après activation superBoost
            if (Time.time - prevSuperBoostActivationTime < 0.5f)
                return;

            bool boostIsLocked = !infinisManager.infinis_Mode_On && expertMode.modeExpertOn;
            if (!boostIsLocked
                && playerMovment.hasReleased) // SUPER BOOST
            {                //Debug.Log("Try controll super BOOST " + Time.time);
                // Si activé ne pas pM.btnManette_tryHang() ni activateFuseeLate()
                float t = Time.time - prevClickTime;
                if (t > 0.05f && t <= timeDoubleClick) // > 0.5f pcq sans raison est appelé 2 fois de suite en 1 clicl
                {
                    //Debug.Log("Contrôle superBoost");
                    // si peut activer le superBoost, on l'active
                    if (boostIngameM.superPowerClicked())
                    {
                        prevClickTime = Time.time;

                        // /!\ , si est deja en anim btnActivate, bloque anim -> btn mauvaise scale
                        myAnimation.Play("btnManetteSBoostOn");
                        prevSuperBoostActivationTime = Time.time;
                        //StartCoroutine(testSuperBoost());
                        return;
                    }
                    //boostIngameManager.getInstance().superPowerClicked();
                }

                prevClickTime = Time.time;
            }

            // Double clique pour superBoost



            // /!\ si clique sur A pendant boost
            // Verifier si boost fonctionne avec un 2e clique 
            // Sinon intercepter le clique
            if (boostIngameManager._superViseurOn) //&& !portail.isTeleporting)
            {
                pM.btnAClicked_stopSuperVisee(false);
                return;
            }


            pM.btnManette_tryRelease();
        }
    }

    public Animation bordZoneAnim;
    public Image glowBtnImg;

    // appelé par manetteManager.cs quand joueur peut controller le palet
    // OU par playerDragCheckPoint()
    public void manetteEnabled()
    {
        // si btn est en anim btnDraggedLight_on , btnManetteActivate set alpha:img ET empêche btnDraggedLight_on de set alpha:img


        // initialement, btnA,B pas activé du tout
        if (!activateAnim.gameObject.activeSelf)
            activateAnim.gameObject.SetActive(true);

        glowBtnImg.color = new Color(1, 1, 1, 0f);
        // activer btnA
        //if(zoneDetectionAssocie.pointerDown)


        // /!\ pop après mort , anim reste bloqué sur _click 
        //Debug.Log("Manette enabled , animPlayed : " + (!zoneDetectionAssocie.pointerDown ? "btnManetteActivate" : "btnManetteActivate_click"));
        //Debug.Log("Manette enabled, zoneDetectionAssocie.pointerDown : " + zoneDetectionAssocie.pointerDown);

        // on check aussi si preshot btnB
        bool preshot = false;
        if(typeButton == typeButtonManette.btnB)
        {
            preshot = preshotOn;

            if (preshot) {
                // faire en fonction de temps depuis dernier clique
                // ou si est en clique down à l'ouverture
                bool clickValid = preshotClickDown
                    || Time.time - timePreshotClick <= timePreshot;
                //Debug.Log("<color=red>TimeElapsed : </color>" + (Time.time - timePreshotClick));
                if (clickValid)
                {
                    // PB player pas ralentit, pas effet cam, pas glow btnB
                    //Debug.Log("<color=blue>PRESHOT CLICK VALID</color>");
                    // cash activer glow
                    btnBOn = true;
                    myAnimation.Play("btnManetteBAloOn");
                    gM.btnBClicked();
                    preshotClickDown = false;

                    
                    // ajouter camera anim

                    // repositionner btn ici, seulement si pas sur pc
                    if(!zoneDetectionManette.isOnPc)
                        zoneDetectionAssocie.preshotClickReposBtn(preshotPos);


                }
            }
            preshotClickDown = false;
            preshotOn = false;
        }

        
        bool btnIsActivated = preshot || zoneDetectionAssocie.pointerDown;
        myBtnImg.color = !btnIsActivated ? colorUnclicked : colorClicked;

        // reinitialiser compteur clique ici, si btn A
        /*if (typeButton == typeButtonManette.btnA)
            zoneDetectionAssocie.manetteActivated_resetClickZone();*/
        /*if(typeButton == typeButtonManette.btnA
            && btnIsActivated)
            myAnimation.Play("btnManetteBAloOn"); */ //anim btnA


        /*Debug.Log(!btnIsActivated ?
            "<color=purple>Btn A etteint ( manetteEnabled ) </color> , " + Time.time
            : "<color=purple>Btn A allumé ( manetteEnabled ) </color> , " + Time.time);*/

        activateAnim.Play("btnManetteActivate");
        bordZoneAnim.Play("bordZoneCliqueOn");
        //myAnimation.Play("btnManetteActivate");
        btnActivated = true;
    }

    // appelé par manetteManager.cs quand joueur ne peut plus controller le palet
    // OU par  playerUndragCheckPoint()
    public void manetteDisabled()
    {
        // Des-activer btnA
        activateAnim.Play("btnManetteDesactivate");
        bordZoneAnim.Play("bordZoneCliqueOff");
        zoneDetectionAssocie.nbPointerDownInZone = 0;

        if (typeButton == typeButtonManette.btnB)
        {
            btnBOn = false;
            //preshotClickDown = false;
            //preshotOn = false;
            
        }


        if (switchActivated) // cas ou meurt sans avoir release checkP
        {
            
            //arrowAnim.Play("flecheBtnA_desactivate");
            //myAnimation.Play("btnManetteArrowDesactivate");
            switchActivated = false;

            zoneDetectionAssocie.resetPosBtnA_endDrag();
        }
        //myAnimation.Play("btnManetteDesactivate");
        btnActivated = false;
        zoneDetectionAssocie.pointerDown = false;
    }

    private bool switchActivated = false;
    public preshotSwitchPlayer preshotSwitch;
    //public Animation arrowAnim;
    
    // appelé par manetteManager.cs quand joueur drag checkP
    public void playerDragCheckPoint(bool firstCheckP)
    {
        // activer btnB
        if(typeButtonManette.btnB == typeButton)
        {
            manetteEnabled();
        }



        // activer fleche de btnA
        if (typeButtonManette.btnA == typeButton)
        {
            // mettre la flèche du bon côté : déja fait + tôt par zoneDetec
            // zoneDetectionAssocie.setArrowSide();

            //arrowAnim.Play("flecheBtnA_activate");
            //myAnimation.Play("btnManetteArrowActivate");
            switchActivated = true;

            if (!firstCheckP)
            {                
                zoneDetectionAssocie.playerDraggedCheckP(preshotSwitch.playerHangCheckP());
            }


            // event particulier : 
            //   player UNDRAG checkP
            //   player click downA , fait switch avant DRAG ( btn A ne suit pas pos si pas sur checkP )
            //   player DRAG ; btnA et pointerDown très loin
            //   bouge un peu btnA -> drag appelé -> switch cash appelé SANS que btnA soit bonne pos
            //   => gros bug
            // solution
            //   quand drag checkP , SI pointerPos au dessus de limite
            //   d'abord mettre btnA sur bonne position
            //   puis checker si doit switch

            // lancer une frame de reposition du btnA dans zoneDetectionManager par rapport à mousePos
            //zoneDetectionAssocie.hangFinished_setBtnAPos();
        }
    }

    // appelé par manetteManager.cs quand joueur ne drag plus checkP
    public void playerUndragCheckPoint()
    {
        // Des-activer btnB
        if (typeButtonManette.btnB == typeButton)
        {
            if (btnBOn)
            {
                btnBOn = false;
                myBtnImg.color = colorUnclicked;
                // on desactive le alo
                myAnimation.Play("btnManetteBAloOff");
            }
            manetteDisabled();
        }

        // Des-activer fleche de btnA
        if (typeButtonManette.btnA == typeButton)
        {
            myAnimation.Play("btnManetteBAloOff");
            //arrowAnim.Play("flecheBtnA_desactivate");
            //myAnimation.Play("btnManetteArrowDesactivate");
            switchActivated = false;
        }
    }

    // appelé par boostIngameManager quand superBoost finit
    //  myAnimation.Play("btnManetteSBoostOn") appelé dans OnPointerDown
    public void superBoostEnded()
    {
        // si drag encore, anim -> alpha 1
        // si ne drag pas, anim -> alpha x
        if (zoneDetectionAssocie.playerDraggingBtn)
            myAnimation.Play("btnManetteSBoostOff_drag");
        else
            myAnimation.Play("btnManetteSBoostOff");
    }


    public static float factor_fusee = 1f;
    private bool fusee_on = false;
    private float timeDragForFuseeActivation = 0.2f;//0.05f;

    private IEnumerator activateFuseeLate(bool phone)
    {
        fusee_on = true;
        float t = 0f;
        while (t < timeDragForFuseeActivation && zoneDetectionAssocie.playerDraggingBtn)
        {
            yield return null;
            t += Time.deltaTime;
        }
        if (t >= timeDragForFuseeActivation
            && playerMovment.hasReleased == true
            && pM.canUseFusee())
        {
            StartCoroutine(fusee(phone, boostIngameManager.speedFusee));
            boostIngameManager.getInstance().use_fusee();
            animFlash.animateFlash(playerMovment.releaseVelocity, typeFlash.fusee);
        }
        else
            fusee_on = false;
    }

    // active la fusee jusqu'à enter check p ou mouseUp
    private IEnumerator fusee(bool phone, float maxSpeedFusee)
    {
        fusee_on = true;
        bool singleClickDown = true;
        float t = 0f;
        float time_acce_decce = 0.3f;
        float diff_speed = maxSpeedFusee - factor_fusee;


        while (singleClickDown && playerMovment.center == null
            && !playerMovment.finish)
        {
            yield return null;
            t += Time.deltaTime;

            // accélérer
            if (t < time_acce_decce)
                factor_fusee = 1 + diff_speed * (t / time_acce_decce);
            else if (factor_fusee != maxSpeedFusee)
                factor_fusee = maxSpeedFusee;

            singleClickDown = !phone && Input.GetKey(gameManager.hangKey)
                || phone && Input.touchCount >= 1;// seulement android ? -> non askip
        }

        if (!playerMovment.finish && playerMovment.center == null)
        {
            // on decelere
            t = 0f;
            while (t < time_acce_decce)
            {
                yield return null;
                t += Time.deltaTime;
                factor_fusee = maxSpeedFusee - diff_speed * (t / time_acce_decce);
            }
        }

        factor_fusee = 1f;
        fusee_on = false;
    }
}
