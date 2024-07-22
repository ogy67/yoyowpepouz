using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using SpriteTrailRender;

public enum typeBehaviour
{
    //tuto,
    aventure,
    infinis,
    online
}

public class playerMovment : MonoBehaviour
{
    #region VARIABLES
    public squeeze _sourisGo;
    public static typeBehaviour comportement;

    [Header("Objets à referencer")]
    public GameObject animOnde;
    public ParticleSystem checkPointParticle;
    public Sprite mySprite;
    public SpriteRenderer mySpriteRenderer, glowSprite;
    public GameObject cordeGo, cordeViseeGo, cordeViseePortalGo;
    public Image boostProgBar; // la desactiver quand meurt
    public menuPause menuPauseGame;
    public Animation btnQuit;

    public static Vector3 posOnRelease; // enregistrer la position quand on relache un checkpoint
    public static Transform initCenter;
    public static Vector3 initPos;
    public static bool finish = true;
    public static bool firstCheckP = true; // gameManager, savoir quand est dans la partie
    public static Transform center; // enregistrer le checkpoint courant

    private Rigidbody2D myRB2D;
    private DottedLine /*cordeLine,*/ cordeViseeLine, cordeViseePortalLine;
    private bool spawning = false; // ne rien faire quand spawn
    public static bool hasReleased = false; // au debut tourne autour checkpoint de depart
    private bool varLoaded = false; // load valeure de ses fils 1 seule fois
    public static Vector3 axis; // donne le sens dans lequel on tourne
    public static int sens = 1; // valeure du sens de rotation
    private LineRenderer corde;

    //private float prevClickTime = 0f;
    //private float timeDoubleClick = 0.5f;

    public static Vector3 releaseVelocity;
    private float prev_speed; // vitesse de la frame précédente ( après calculs )

    public static float rotationSpeedFactor = 1f;
    public float DEG_SPEED_DIF_B_UNLOCKED = 30;
    public float DEG_SPEED_CONST = 220; // 200 // 180 // 150 avant
    public const float SPEED_CONST = 1.2f; // vitesse constante quand fait rien // 1f avant
    public const float speedOnBounce = 1.4f; // vitesse au rebond // 1.2 avant
    public const float factorTranslate = 2.2f; // 2.0 // vitesse de translation
    public const float timeForDecceleration = 3f;
    #endregion 

    public trackerPlayerStuck trackerStuck;
    //private List<int> nbTouchAllowForHangCheckP = new List<int>() { 1, 2 };
    void Start()
    {
        //cordeLine = cordeGo.GetComponent<DottedLine>();
        cordeViseeLine = cordeViseeGo.GetComponent<DottedLine>();
        cordeViseePortalLine = cordeViseePortalGo.GetComponent<DottedLine>();
        myRB2D = GetComponent<Rigidbody2D>();
    }

    public manetteManager manetteM;
    // appelé par zoneDetectionManette.cs (btnA)
    public void btnManette_tryHang()
    {
        if (hasReleased)
        {
            Hang();
        }
    }
    public void btnManetteFirstClickInitCheckP()
    {
        if (center == initCenter && comportement != typeBehaviour.online
            && menuPause.btnPauseVisible)
            menuPauseGame.menuPauseDisabled();
    }

    // appelé par zoneDetectionManette.cs (btnA)
    public void btnManette_tryRelease()
    {
        if (!hasReleased)
        {
            Release();
            manetteM.playerUndraggedACheckP();
        }
    }

    void Update()
    {
        if (!spawning)
        {
            if (!finish)
            {
                /*bool phone = Application.platform == RuntimePlatform.Android;
                bool canCheck = !phone || phone && nbTouchAllowForHangCheckP.Contains(Input.touchCount);

                if (canCheck)
                {
                    bool click_down = !phone ? Input.GetKey(gameManager.hangKey) ? true : false
                        : Input.GetTouch(0).phase != TouchPhase.Ended ? true : false;
                    bool click_up = !phone ? Input.GetKeyUp(gameManager.hangKey) ? true : false
                        : Input.GetTouch(0).phase == TouchPhase.Ended ? true : false;

                    if (click_down && hasReleased && !gameManager.IsPointerOverUIObject())
                        Hang();
                    else if (click_up && !hasReleased && !gameManager.IsPointerOverUIObject())
                        Release();
                }*/

                if (!hasReleased)
                {
                    if (center == null) return;
                    /*cordeLine.DrawDottedLine(
                        transform.position + new Vector3(0, 0, 10)
                        , center.position + new Vector3(0, 0, 10)
                        , false, false);*/
                    setViseurRotation();
                }
                else
                {
                    if (!boostIngameManager._superViseurOn)
                    {
                        if (!portail.isTeleporting)
                            setViseur(false, Vector3.zero);
                        else
                            cordeViseePortalLine.DrawDottedLine(
                            currentFinalPosTrail
                            , currentPortalPosSpawn
                            , true, true);
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (BounceThisFrame && colThisFrame.Count != 0)
        {
            Vector2 newNorme;
            Vector2 posParticle = lesPosCol[0];
            if (lesPosCol.Count == 2
                && Vector2.Distance(lesPosCol[0], lesPosCol[1]) > distMinToConsiderTwoContact)
            {
                Vector2 centre = lesPosCol[1] - (lesPosCol[1] - lesPosCol[0]) / 2;
                newNorme = getNormTwoCol(lesPosCol[0], lesPosCol[1],
                    centre, transform);
                posParticle = centre;

                Debug.DrawLine(centre, centre + newNorme, Color.blue, 3.0f);

                Debug.DrawLine(transform.position, transform.position + releaseVelocity, Color.yellow, 3.0f);
                Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.Reflect(releaseVelocity, newNorme), Color.green, 3.0f);
                Debug.DrawLine(transform.position, (Vector2)transform.position + getReflexion(newNorme, releaseVelocity), Color.green, 3.0f);

                StartCoroutine(lockColAfterDoubleCol());
            }
            else
            {
                newNorme = colThisFrame[0];
            }

            releaseVelocity = getReflexion(newNorme.normalized, releaseVelocity);
           
            Debug.DrawLine(transform.position, (Vector2)transform.position + getReflexion(newNorme, releaseVelocity) / 5, Color.grey, 3.0f);

            posOnRelease = transform.position;
            //ghostManager.registerGhostEvent(ghostEvent.bounce, transform);


            if (!finish)
            {
                //if (!TutoManager.tutoOn)
                //{
                    boostIngameManager.getInstance().playerBounce(boostIngameManager._ressortOn);
                    animFond3DGame.getInstance().playerBounce();
                    //managerAnimFond3D.getInstance().playerBounce();
                //}

                animFlash.animateFlash(releaseVelocity, typeFlash.bounce);
                particleObject.bounceParticle(posParticle, newNorme);

                sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_bounce);
                //sfxManager.getInstance().playSound_ingame(typeSound_ingame.bounce);
                vibrationManager.vibe(30);
                cameraGame.getInstance().camEffectPlayerCollision(true);
                glowSprite.GetComponent<Animation>().Play("glowBounce");
                prev_speed = speedOnBounce;
            }
        }
        BounceThisFrame = false;
        colThisFrame = new List<Vector2>();
        lesPosCol = new List<Vector2>();
        catchedVector = Vector2.zero;

        // on move player ; CHECKP
        if (!spawning)
        {
            if (!finish)
            {
                if (!hasReleased)
                {
                    if (center == null) return;
                    transform.RotateAround(center.position, axis,
                        DEG_SPEED_CONST * rotationSpeedFactor * Time.deltaTime
                        * btnManette.factor_fusee/* * prev_speed*/);// );

                }
                else
                {
                    Vector3 speed = releaseVelocity * Time.deltaTime * prev_speed * factorTranslate;
                    speed *= btnManette.factor_fusee;
                    speed *= speed_superViseur;
                    myRB2D.MovePosition(
                        transform.position
                        + speed);

                    // après rebond
                    if (prev_speed != SPEED_CONST)
                    {
                        float diffSpeed = (speedOnBounce - SPEED_CONST) * (Time.deltaTime / timeForDecceleration);
                        prev_speed = prev_speed - diffSpeed < SPEED_CONST ? SPEED_CONST : prev_speed - diffSpeed;
                    }
                }
            }
            // Continuer d'avancer si finit et pas mort
            else if (finish && mySpriteRenderer.sprite != null)
            {
                if (prev_speed > 0)
                {
                    myRB2D.angularVelocity *= 0.85f;
                    myRB2D.MovePosition(transform.position + releaseVelocity * Time.deltaTime * prev_speed * factorTranslate);
                    prev_speed -= 2.5f * Time.deltaTime;
                }
                else myRB2D.velocity = Vector2.zero;
            }
        }
    } // update de la physique

    private void loadVar()
    {
        mySprite = mySpriteRenderer.sprite;
        corde = gameObject.GetComponent<LineRenderer>();
        varLoaded = true;
    }

    private bool speedIsSetToMax = false;
    // si btn b unlocked, vitesse + rapide
    private void setDegSpeed()
    {
        if (!speedIsSetToMax)
        {
            if(SaveSystem.nb_couronne >= 2)
            {
                speedIsSetToMax = true;
                DEG_SPEED_CONST += DEG_SPEED_DIF_B_UNLOCKED;
            }
        }
    }

    //public Transform trailParent;
    // appelé quand partie re-load : après mort / debut partie
    public void activate()
    {
        /*if(comportement != typeBehaviour.online)
            btnQuit.Play("btnQuitOpen");*/

        // Debug.Log("Player pop");
        //transform.parent.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

        if (!varLoaded)
            loadVar();
        if (mySpriteRenderer.sprite == null)
            mySpriteRenderer.sprite = mySprite;
        setDegSpeed();

        axis = new Vector3(0, 0, sens);
        hasReleased = false;
        firstCheckP = true;
        SpriteTrailRender.SpriteTrailRenderer.trailOn = true;

        transform.position = initPos;
        contentTrailGame.getInstance().getCurrentTrail().gameObject.SetActive(true);
        //trailParent.gameObject.SetActive(true);
        contentTrailGame.getInstance().clearTrail();
        center = initCenter;

        corde.positionCount = 0;

        if (GetComponent<CircleCollider2D>().isTrigger)
            GetComponent<CircleCollider2D>().isTrigger = false;
        gameObject.SetActive(true);

        //Debug.Log("Activate player, isSetOn");
        StartCoroutine(spawn());

        //glowSprite.gameObject.SetActive(true);
        //glowSprite.GetComponent<Animation>().Play("glowInit");

        if (!expertMode.modeExpertOn && boostIngameManager._sourisOn)
            _sourisGo.gameObject.SetActive(true);

        GhostBoostManager.getInstance().playerActivated();
    }

    //public Transform initParent;
    // appelé AUSSI quand va dans menu principal en quickQuit
    public void stop()
    {
        if (boostProgBar.fillAmount != 0) boostProgBar.fillAmount = 0;
        gameObject.SetActive(false);
        //animFond3DGame.getInstance().playerDisactivated();

        /*if (transform.parent != initParent)
            transform.SetParent(initParent);*/

        contentTrailGame.getInstance().playerDesactivated();
        //contentTrailGame.getInstance().clearTrail();
        //trailParent.GetComponent<trailManager>().clearTrail();
        hasReleased = false;
        if (corde != null)
            corde.positionCount = 0;

        if (_sourisGo.gameObject.activeSelf)
            _sourisGo.forceCloseEndGame();

        manetteM.manetteDisabled();

        SpriteTrailRender.SpriteTrailRenderer.trailOn = false;

        
        //trackerStuck.playerStopGame();
    }


    // appelé par
    // infinisManager -> switch level
    // aventure -> switch level
    public void changeLevel()
    {
        Debug.Log("Anim switch level called, player still active ? " + gameObject.activeSelf);
        // /!\ seulement appeler si player encore activé ( pas boost pierreTombale ou bombe )
        if(gameObject.activeSelf)
            GetComponent<Animation>().Play("playerDespawnArrivee");
    }

    public void stop_lastPlayerOnline()
    {
        // si venait de mourir pendant que doit être stopé
        // ( NB : si est en respawn, est cash stoppé )
        if (!finish)
        {
            finish = true;
            if (boostProgBar.fillAmount != 0) boostProgBar.fillAmount = 0;

            gameObject.SetActive(false);
            contentTrailGame.getInstance().clearTrail();
            hasReleased = false;
            if (corde != null)
                corde.positionCount = 0;

            if (_sourisGo.gameObject.activeSelf)
                _sourisGo.forceCloseEndGame();

            // particles explosion
            particleObject.deatParticlesGhostOnline(true, new Vector2[] { transform.position });
        }
    }


    private List<string> objectCutViseur = new List<string>()
    {
        "Obstacle",
        "ObstacleBounce",
        "ObstaclePivot",
        "ObstaclePortal",
        "ObstaclePivotRond"
    };

    public static bool portalIsInVisor = false;
    public static Vector3 playerDirection = Vector3.zero;
    public LineRenderer superViseeLine;
    private Transform currentPortal;
    private Vector2 currentFinalPosTrail, currentPortalPosSpawn;
    /*
     * void setViseurRotation()
     * void setViseur()
     * IEnumerator stopLine()
     * IEnumerator spawn()
     * 
     * REGION animation pion special
     */
    #region GRAPHIQUE POUR JOUEUR

    private void setViseurRotation()
    {
        Vector3 offsetFromCenter = transform.position - center.position;
        Vector3 travelDirection = Vector3.Cross(axis, offsetFromCenter).normalized;
        playerDirection = transform.position + travelDirection;

        cordeViseeLine.DrawDottedLine(
            transform.position
            , (Vector2)getFirstCollisionAim(transform.position, travelDirection, new List<Transform>())[1]
            , false, true
            );
    }
    public object[] getFirstCollisionAim(Vector2 initPos, Vector2 dir, List<Transform> toAvoid)
    {
        object[] ret = new object[] { null, Vector2.zero };
        RaycastHit2D[] hit = Physics2D.RaycastAll(initPos, dir);

        foreach (RaycastHit2D r in hit)
        {
            if (objectCutViseur.Contains(r.collider.tag) && !toAvoid.Contains(r.transform))
            {
                ret = new object[] { r.collider.transform, r.point };
                break;
            }
        }
        return ret;
    }


    private Vector2 lastPortalDirection = Vector2.zero;
    // mettre à jour le viseur pour savoir ou aller
    private void setViseur(bool superVisee, Vector3 forceDirection)
    {
        portalIsInVisor = false;
        object[] posInfo_normal = getFirstCollisionAim(transform.position, releaseVelocity, new List<Transform>());

        Transform posTr_normal = (Transform)posInfo_normal[0];
        Vector2 posPt_normal = (Vector2)posInfo_normal[1];

        // SET LINE
        if (!superVisee)
        {
            cordeViseeLine.DrawDottedLine(
                posPt_normal
                , transform.position
                , true, true);
            playerDirection = posPt_normal;
        }
        else if (superViseeLine.positionCount == 2)
        {
            object[] posInfo_superVisee = getFirstCollisionAim(
                transform.position,
                forceDirection,
            new List<Transform>());

            //Transform posTr_superVisee = (Transform)posInfo_superVisee[0];
            Vector2 posPt_superVisee = (Vector2)posInfo_superVisee[1];

            superViseeLine.SetPosition(0, transform.position);
            superViseeLine.SetPosition(1, posPt_superVisee);
        }

        if (posTr_normal.tag != "ObstaclePortal" && currentPortal != null)
        {
            currentPortal = null;
            lastPortalDirection = Vector2.zero;
            return;
        }
        if (posTr_normal.tag == "ObstaclePortal")
        {
            portalIsInVisor = true;

            if (currentPortal != posTr_normal || (Vector2)releaseVelocity != lastPortalDirection) // vient juste de le detecter, 1ere frame
                currentPortal = posTr_normal;
            else // on ne refait pas les calculs si rien n'a changé
            {
                if (!superVisee)
                {
                    // on montre juste trail visée portal
                    cordeViseePortalLine.DrawDottedLine(
                        currentFinalPosTrail
                        , currentPortalPosSpawn
                        , true, true);
                }
                return;
            }

            lastPortalDirection = releaseVelocity;

            Vector2 pos3 = posTr_normal.GetComponent<portail>().getPointSpawn(posPt_normal, true);

            portail p = posTr_normal.GetComponent<portail>();
            Vector2 v = releaseVelocity;

            if (superVisee)
                return; // on n'affiche pas la ligne visee portal quand on est en super visee

            if (p.inverseSide)
            {
                if (p.inverseXSide)
                    v.x = -v.x;
                else
                    v.y = -v.y;
            }

            object[] posInfo2 = getFirstCollisionAim(pos3, v,
                new List<Transform>() { posTr_normal.GetComponent<portail>().otherPortal.transform });

            currentFinalPosTrail = (Vector2)posInfo2[1];
            currentPortalPosSpawn = pos3;


            cordeViseePortalLine.DrawDottedLine(
            currentFinalPosTrail
            , currentPortalPosSpawn
            , true, true);
        }

    }

    public float timeSpawn = 15 / 60f;
    private IEnumerator spawn()
    {
        contentTrailGame.getInstance().playerStartPop();
        spawning = true;
        GetComponent<Animation>().Play("playerSpawn");
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_pop);

        if (infinisManager.infinis_Mode_On)
        {
            // on regarde si sensRotForced
            packNiveau_infinis p = infinisManager.currentPatern.GetComponent<packNiveau_infinis>();
            if (p.currentCombiCs != null &&
                p.currentCombiCs.forceSideRotation)
            {
                sens = p.currentCombiCs.sideRotation * -1; // *= -1 parce que pas bien set dans editor
                if (p.packIsMirrored)
                    sens *= -1;
                axis.z = sens;
            }
        }
        else //if(TutoManager.tutoOn == false)
        {
            // on regarde si sensRotForced
            packNiveau p = managerNiveau.currentNiveauGo.GetComponent<packNiveau>();
            if (p.forceSideRotation
                && p.currentLevel == 0)
            {
                sens = p.sideRotation * -1;
                axis.z = sens;
            }
        }

        animFond3DGame.getInstance().playerPop();
        yield return new WaitForSeconds(timeSpawn);
        chechpointEnd.win = false;
        if (comportement != typeBehaviour.online)
            menuPauseGame.menuPauseIsEnabled();
        finish = false;
        spawning = false;
        manetteM.manetteEnabled();
        contentTrailGame.getInstance().playerEndPop(transform.position);

        trackerStuck.playerPopGame();
    }



    private float timeFade = 0.5f; // d'une couleure à une autre
    public List<Color> colorStack; // liste des couleure à animer
    private int indexStack = 0; // couleure vers laquelle on fade

    // enregistrer tout les tr ( ex : ceux des portails )
    private List<Transform> trStack = new List<Transform>();
    private Transform curTr;

    private SpriteRenderer playerSp;
    private TrailRenderer playerTr;
    /*
    * void OnEnable()
    * void updateTrails(Color curCol)
    * IEnumerator fadeLoop()
    * IEnumerator fade(float timeAnim, Color initC, Color endC)
    */
    #region Animation pion special
    private void OnEnable()
    {
        if (playerSp == null)
            playerSp = mySpriteRenderer;
        if (skinManager.superPionOn)
        {
            StartCoroutine(fadeLoop());
        }
        else
        {
            playerSp.color = Color.white;
        }
    }

    private void updateTrails(Color curCol)
    {
        trailManager tM = contentTrailGame.getInstance().getCurrentTrail();
        if (curTr == null || curTr != tM)
        {
            curTr = tM.transform;
            trStack.Add(curTr);
        }
        foreach (Transform trail in trStack)
        {
            if (trail != null)
            {
                trail.GetComponent<trailManager>().superPalet_changeColor(
                    new Color(curCol.r, curCol.g, curCol.b, 1f),
                    new Color(curCol.r, curCol.g, curCol.b, 0f));
            }
        }
        glowSprite.color =
            new Color(curCol.r, curCol.g, curCol.b, 100 / 255f);
    }

    private IEnumerator fadeLoop()
    {
        while (1 != 0)
        {
            int nextColIndex = indexStack + 1 < colorStack.Count ? indexStack + 1 : 0;
            StartCoroutine(fade(timeFade, colorStack[indexStack], colorStack[nextColIndex]));

            yield return new WaitForSeconds(timeFade);
            indexStack = nextColIndex;
        }
    }

    private IEnumerator fade(float timeAnim, Color initC, Color endC)
    {
        float t = 0f;
        Color curCol;

        while (t < timeAnim)
        {
            curCol = initC + (endC - initC) * (t / timeAnim);

            playerSp.color = curCol;
            updateTrails(curCol);

            yield return null;
            t += Time.deltaTime;
        }
    }
    #endregion

    #endregion


    public boostBouclierCheckP bouclierCheckPCs;
    /*
     * void Release()
     * void Hang()
     * bool setCenter()
     * void arivee()
     * void pivotPush(Vector2 dir)
     */
    #region MOUVEMENT

    // checkpoint
    public void Release()
    {
        bouclierCheckPCs.releaseCheckP();

        itemBoostGame.regenIsPaused = false;
        prev_speed = SPEED_CONST;

        if (center != initCenter)
        {
            animOnde.transform.position = transform.position;
            animOnde.GetComponent<Animation>().Play("ondeRelease");
            glowSprite.GetComponent<Animation>().Play("glowReleaseCheckP");
        }

        Vector3 offsetFromCenter = transform.position - center.position;

        Vector3 travelDirection = Vector3.Cross(axis, offsetFromCenter).normalized;
        releaseVelocity = 1.7f * travelDirection;
        posOnRelease = transform.position;
        corde.positionCount = 0;
        hasReleased = true;

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_release);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.checkP_release);

        // si release init checkP online
        // on passe dans les 2 if suivant
        if (center != initCenter
            || comportement == typeBehaviour.online)
        {
            center.GetComponent<Animation>().Play("checkpointValide");
            center.GetComponent<playerDetector>().lockCP();
        }
        if(center == initCenter)
        {
            firstCheckP = false;
            /*if(comportement != typeBehaviour.online)
                btnQuit.Play("btnQuitClose");*/
            //if (comportement != typeBehaviour.online)
            //    menuPauseGame.menuPauseDisabled();
            //.
        }
        else
        {
            defiManager.getInstance().defiIsIncremented(typeDefi.checkP, 1);
        }

        center = null;
        //ghostManager.registerGhostEvent(ghostEvent.releaseCheckP, transform);
        //if (!TutoManager.tutoOn)
            
    }
    public void Hang()
    {
        //.
        if (boostIngameManager._superViseurOn)
            return;

        /* ANCIEN
        if (speed_superViseur != 1)
            return;*/



        myRB2D.velocity = Vector2.zero;
        myRB2D.angularVelocity = 0;

        if (setCenter())
        {
            bouclierCheckPCs.hangCheckP();
            // player dragged appelé avant que btn A repositionné sur posClick
            // -> si btnA loin de pos click : check switch et en fait un

            hasReleased = false;

            btnManette.hasHangOnClick = true;
            //Debug.Log("2");
            manetteM.playerDraggedACheckP(false);

            itemBoostGame.regenIsPaused = true;

            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_hang);
            //sfxManager.getInstance().playSound_ingame(typeSound_ingame.checkP_hang);
            checkPointParticle.transform.position = center.position;
            checkPointParticle.Play();

            // regarder dans quel sens tourner
            Vector2 point = center.position - posOnRelease;
            axis.z = -releaseVelocity.x * point.y + releaseVelocity.y * point.x > 0 ? -1 : 1;



            //if (!TutoManager.tutoOn)
            //    ghostManager.registerGhostEvent(ghostEvent.hangCheckP, transform);

            center.GetComponent<Animation>().Play("checkpoint");
            glowSprite.GetComponent<Animation>().Play("glowHangCheckP");
        }
    }

    // savoir si peut activer la fusee ou non dans le checkP
    public bool canUseFusee()
    {
        if (playerDetector.checkpoint == null)
            return true;
        if (!playerDetector.checkpoint.GetComponent<playerDetector>().playerCanHang_fusee(transform.position))
            return true;
        return false;
    }

    public bool setCenter()
    {
        // on prend le center ou est le player
        center = playerDetector.checkpoint;

        if (center != null
            && !center.GetComponent<playerDetector>()._lockCheckP)
        {
            float distanceBetween_player_posChangeDir = Vector2.Distance(transform.position, posOnRelease);
            float distanceBetween_center_posChangeDir = Vector2.Distance(
                    posOnRelease,
                    LineLineIntersection(
                        center.position, new Vector3(-releaseVelocity.y, releaseVelocity.x, 0) * 10));

            if (distanceBetween_player_posChangeDir >= distanceBetween_center_posChangeDir
                && center.GetComponent<playerDetector>().playerCanHang(transform.position))
                return true;
        }
        return false;
    }
    // checkpoint end
    public void arivee()
    {
        itemBoostGame.regenIsPaused = true;
        vibrationManager.vibe(100);
        finish = true;
        //ghostManager.registerGhostEvent(ghostEvent.win, transform);
        GhostBoostManager.getInstance().gameEnd(true);

        manetteM.manetteDisabled();

        trackerStuck.playerWinGame();
        animFond3DGame.getInstance().playerWin();
    }
    // pivot
    public void pivotPush(Vector2 dir)
    {
        posOnRelease = transform.position;

        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_pivot);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.pivot);
        float f = dir.magnitude / releaseVelocity.magnitude;
        releaseVelocity = new Vector2(dir.x / f, dir.y / f);
        animFlash.animateFlash(releaseVelocity, typeFlash.pivot);
        prev_speed = speedOnBounce;
        //ghostManager.registerGhostEvent(ghostEvent.pivot, transform);
        defiManager.getInstance().defiIsIncremented(typeDefi.pivot, 1);
        gameManager.getInstance().eventIngame(typeEffectColor.pivot);
    }

    #endregion


    private float timeContact = 0f;
    private float distMinToConsiderTwoContact = 0.02f; // contact avec pike si inferieur
    private Vector2 catchedVector = Vector2.zero; // met a jour col dans fixedUpdate
    private bool lockCol = false;

    private List<Vector2> colThisFrame = new List<Vector2>();
    private List<Vector2> lesPosCol = new List<Vector2>();
    private bool BounceThisFrame = false;
    private Vector2 currentNormContact;
    private Vector3 contactP;
    /*
     * REGION fonction supplémentaire de physique
     * void OnCollisionEnter2D(Collision2D collision)
     * void OnCollisionStay2D(Collision2D collision)
     */
    #region PHYSIQUE Player
    /*
     * Vector3 LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1)
     * void computeNorm(Collision2D collision)
     * Vector2 getReflexion(Vector2 n, Vector2 d)
     * bool colBehindPlayer(Vector2 dir, Vector2 contactPoint, Vector2 norm)
     * Vector2 getNormTwoCol(Vector2 pt1, Vector2 pt2, Vector2 ptCentre)
     * IEnumerator lockColAfterDoubleCol()
     * 
     * REGION boost pHysique , bombe , pierre tombale , souris
    */
    #region physique du jeu : fonction supplémentaire

    // chercher le point d'intersection entre la direction du J
    //  et la droite perpendiculaire qui passe par cette direction et le pt center
    public Vector3 LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1)
    {
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, releaseVelocity);
        return
            linePoint1
            + (lineVec1 * Vector3.Dot(Vector3.Cross(posOnRelease - linePoint1, releaseVelocity), crossVec1and2)
            / crossVec1and2.sqrMagnitude);
    }

    // calculer la norme d'une collision
    private void computeNorm(Collision2D collision)
    {
        catchedVector = Vector2.Reflect(
            catchedVector == Vector2.zero ? (Vector2)releaseVelocity : catchedVector, collision.contacts[0].normal.normalized);

        currentNormContact = collision.contacts[0].normal;
        contactP = collision.contacts[0].point;


        // si coin "pike" : se referer au premier contactPoint _ 0.02 valeure max observé pour pike , 0.1
        if (collision.contacts.Length > 1
            && Vector2.Distance(collision.contacts[0].point, collision.contacts[1].point) > distMinToConsiderTwoContact)
        {
            Vector2 milieuVec = collision.contacts[1].point - (collision.contacts[1].point - collision.contacts[0].point) / 2;
            currentNormContact =
                getNormTwoCol(collision.contacts[0].point
                , collision.contacts[1].point
                , milieuVec
                , transform);
            contactP = milieuVec;

        }

        lesPosCol.Add(contactP);
        colThisFrame.Add(currentNormContact);
    }

    // pour les rebons du joueur
    public static Vector2 getReflexion(Vector2 n, Vector2 d)
    {
        return d - 2 * (d.x * n.x + d.y * n.y) * n;
    }

    // Quand collision
    // regarder si col est derrière player par rapport a sa direction
    // si derriere, ne pas prendre en compte -> evite des bug
    public static bool colBehindPlayer(Vector2 dir, Vector2 contactPoint, Vector2 norm)
    {
        return Vector2.Angle(dir, -norm) > 90f;
    }

    // Double contact : renvoit la norme de ces deux contacts
    public static Vector2 getNormTwoCol(Vector2 pt1, Vector2 pt2, Vector2 ptCentre, Transform t)
    {
        Vector2 vec = pt2 - pt1;
        Vector2 n = new Vector2(vec.y, -vec.x); // calcul de la norme ( droite perpendiculaire a vec )

        //Debug.DrawRay(pt1, pt2 - pt1, Color.white, 3f);
        // inverser norme si pas bon sens
        if (ptCentre.y > t.position.y && n.y > 0
            || ptCentre.y < t.position.y && n.y < 0)
        {
            n = new Vector2(-n.x, -n.y);
        }
        return n;
    }

    // SEUL probème dans bouncing : double collision
    // après une double collision, dautre contact ont étés observés directement après bounce
    // ces contacts font beuguer le joueur, on empêche qu'ils soient prit en compte
    //  -> on laisse le joueur se dettacher de l'obstacle
    private IEnumerator lockColAfterDoubleCol()
    {
        lockCol = true;
        yield return new WaitForSeconds(0.10f);
        lockCol = false;
    }

    #endregion


    public static int resultPierreTombale;
    public static int resultBombe;
    /*
     * IEnumerator wait_pierreTombale()
     * void activateBombe()
     * IEnumerator wait_bombe()
    */
    #region boost pHysique , bombe , pierre tombale , souris
    private IEnumerator wait_pierreTombale()
    {
        itemBoostGame.regenIsPaused = true;
        resultPierreTombale = -1;
        if (_sourisGo.gameObject.activeSelf)
            _sourisGo.forceCloseEndGame();

        yield return new WaitUntil(predicate: () => resultPierreTombale != -1);

        playerGhost.typeGhost = typePlayerGhost.none;

        if (resultPierreTombale == 1)
        {
            //survit
        }
        else
        {
            //meurt
            if (!boostIngameManager.getInstance().canUse_medic())
            {
                //if (!gameOnlineManager.gameOnlineOn)
                    gameManager.nbVies -= 1;
            }

            if (comportement == typeBehaviour.infinis)
                infinisManager.died();

            trackerStuck.playerDieGame();
            gameManager.getInstance().died();
            //ghostManager.registerGhostEvent(ghostEvent.die, transform);
        }
    }

    public void activateBombe()
    {
        // player explose
        // si ghost enter endcheckp -> win
        // sinon recommence le niveau

        itemBoostGame.regenIsPaused = true;

        finish = true;
        vibrationManager.vibe(200);
        particleObject.playerGhost_deadParticles(transform.position);
        mySpriteRenderer.sprite = null;

        glowSprite.gameObject.SetActive(false);
        if (_sourisGo.gameObject.activeSelf)
            _sourisGo.forceCloseEndGame();

        GetComponent<CircleCollider2D>().isTrigger = true;
        myRB2D.velocity = Vector2.zero;
        myRB2D.angularVelocity = 0;
        cameraGame.getInstance().camEffectPlayerCollision(false);
        resultBombe = -1;
        StartCoroutine(wait_bombe());

        trackerStuck.playerUseBombe();
    }

    private IEnumerator wait_bombe()
    {
        yield return new WaitUntil(predicate: () => resultBombe != -1);

        playerGhost.typeGhost = typePlayerGhost.none;

        if (resultBombe == 1)
        {
            // survit
        }
        else
        {
            // meurt
            if (!boostIngameManager.getInstance().canUse_medic())
            {
                //if (!gameOnlineManager.gameOnlineOn)
                    gameManager.nbVies -= 1;
            }

            if (comportement == typeBehaviour.infinis)
                infinisManager.died();

            gameManager.getInstance().died();
            manetteM.manetteDisabled();
            //trackerStuck.playerDieGame();
        }
    }

    #endregion


    public bool Collide(Collision2D collision) // return true si collision ne conduit pas a la mort
    {
        //if (!hasReleased)
        //    return true;

        timeContact = 0f;
        BounceThisFrame = false;

        bool retour = false; // meurt par défaut;

        // Ne pas assayer d'activer un seul boost si : ( ressort, souris )
        // - a déjà gagné ( collide appelé après win )
        // - player est sur checkP

        //Debug.Log("Collision player avec obj : " + collision.collider.name+" , colBehind ? "+ colBehindPlayer(releaseVelocity, collision.contacts[0].point, collision.contacts[0].normal));


        bool colIsBehindPlayer = hasReleased ? colBehindPlayer(releaseVelocity, collision.contacts[0].point, collision.contacts[0].normal) : false;
        bool lockColBehindPortal = portail.isTeleportingLong && colIsBehindPlayer;

        // ne rien faire quand <se teleporte> et <coup de pivot> ( beug : rebondis - meurt )
        if (//!obstaclePivot.isPushing &&
            !lockCol
            && !lockColBehindPortal
            // col si n'a pas release le checkP -> prendre en compte OU col pas derriere player -> prendre en compte
            // si has release == true , regarde si col derrière player ou non
            // => ne plus accepter col que si devant ( des obs bougent et peuvent toucher player par derriere
            /*&&
            (hasReleased == false ||
            !colBehindPlayer(releaseVelocity, collision.contacts[0].point, collision.contacts[0].normal)
            )*/)
        {

            bool avoidObstacle = false;
            bool bounceOnObstacleAfterWin = finish && collision.gameObject.tag == "Obstacle";

            /*if (comportement == typeBehaviour.tuto)
            {
                avoidObstacle = collision.gameObject.tag == "Obstacle" ? false : true;
            }          
            else 
            {*/
            /*Debug.Log("Collision, boostIngameManager._bouclierOn : " + boostIngameManager._bouclierOn + ", collision.gameObject.tag :" + collision.gameObject.tag + ", hasReleased : " + hasReleased);
            if (!hasReleased && collision.gameObject.tag == "Obstacle" && boostIngameManager._bouclierOn)
            {
                btnChangeRotation();
            }
            else
            {*/
                if (collision.gameObject.tag != "Obstacle"
                    // peut mourrir de collision sur checkP seulement si horloge
                    //|| ( !hasReleased && !collision.gameObject.name.Contains("horloge"))
                    || boostIngameManager._bouclierOn)
                    avoidObstacle = true;
                // on peut avoid la collision si ressort peut encore être utilisé
                else if (!(expertMode.modeExpertOn && !infinisManager.infinis_Mode_On)
                    && collision.gameObject.tag == "Obstacle" && !boostIngameManager._bouclierOn
                // on active pas ressort si sur checkP OU player finit
                    && hasReleased == true && finish == false)
                {
                    if (boostIngameManager.getInstance().canUse_ressort())
                        avoidObstacle = true;
                }

                if (!avoidObstacle && bounceOnObstacleAfterWin)
                    avoidObstacle = true;

                if (!avoidObstacle)
                {
                    // mode de jeu normal
                    if (comportement == typeBehaviour.aventure
                                || comportement == typeBehaviour.infinis
                                || comportement == typeBehaviour.online)
                    {
                        forceDie(collision);
                    }
                }
                else if (collision.gameObject.tag == "ObstacleBounce"
                    ||// gerer double colision entre bounce et mur normal pendant super bounce
                    (collision.gameObject.tag == "Obstacle" && boostIngameManager._bouclierOn)
                    || // gerer ressort
                     (collision.gameObject.tag == "Obstacle" && boostIngameManager._ressortOn)
                     || bounceOnObstacleAfterWin)
                {
                    if (!finish && hasReleased)// && !TutoManager.tutoOn) // si pas finit lvl
                    {
                        defiManager.getInstance().defiIsIncremented(typeDefi.bounce, 1);
                        gameManager.getInstance().eventIngame(typeEffectColor.bounce);
                    }
                    if (!colIsBehindPlayer)
                    {
                        computeNorm(collision);
                        // on enregistre les valeure de collision pour fixed update
                        deathValues = new Vector2[] { collision.contacts[0].point, currentNormContact };
                        BounceThisFrame = true;
                    }
                }
            //}
        }

        return retour;
    }

    // pour ghost manager, afficher les death particules des ghost
    public static Vector2[] deathValues = new Vector2[] { Vector2.zero, Vector2.zero };
    //private int idCol = 0;

    private void forceDie(Collision2D collision)
    {
        itemBoostGame.regenIsPaused = true;
        if (!finish)
        {
            computeNorm(collision);

            vibrationManager.vibe(200);

            // il faut aussi faire l'animation undragCheckP sur manette
            if (!hasReleased) center.GetComponent<Animation>().Play("checkpointValide");

            deathValues = new Vector2[] { collision.contacts[0].point, currentNormContact };
            particleObject.deathParticle(deathValues[0], deathValues[1], false);
            mySpriteRenderer.sprite = null;
            GetComponent<CircleCollider2D>().isTrigger = true;
            finish = true;
            myRB2D.velocity = Vector2.zero;
            myRB2D.angularVelocity = 0;
            cameraGame.getInstance().camEffectPlayerCollision(false);
            if (boostProgBar.fillAmount != 0) boostProgBar.fillAmount = 0;
            glowSprite.gameObject.SetActive(false);

            manetteM.manetteDisabled();
            trackerStuck.playerDieGame();

            btnAClicked_stopSuperVisee(true);
            
            if (boostIngameManager._superViseurOn)
                boostIngameManager._superViseurOn = false;

            if (!(expertMode.modeExpertOn && !infinisManager.infinis_Mode_On))
            {
                // avant de déclarer la fin de partie, on regarde si pierre tombael peut être utilisé
                if (boostIngameManager.getInstance().canUse_PierreTombale(releaseVelocity, transform.position))
                {
                    StartCoroutine(wait_pierreTombale());
                    return;
                }
                if (!boostIngameManager.getInstance().canUse_medic())
                {
                     gameManager.nbVies -= 1;
                }
            }
            else
            {
                 gameManager.nbVies -= 1;
            }

            

            /*if (comportement == typeBehaviour.tuto)
            {
                TutoManager.getInstance().died();
            }
            else
            {*/
                if (comportement == typeBehaviour.infinis)
                    infinisManager.died();

                gameManager.getInstance().died();
                //ghostManager.registerGhostEvent(ghostEvent.die, transform);
            //}

            if (_sourisGo.gameObject.activeSelf)
                _sourisGo.forceCloseEndGame();

            GhostBoostManager.getInstance().gameEnd(true);

        }
        else
        {
            BounceThisFrame = true;
        }
    }

    public void forceDie_stuck()
    {
        itemBoostGame.regenIsPaused = true;
        vibrationManager.vibe(200);

        // il faut aussi faire l'animation undragCheckP sur manette
        if (!hasReleased && center != null) center.GetComponent<Animation>().Play("checkpointValide");
        hasReleased = false;

        mySpriteRenderer.sprite = null;
        GetComponent<CircleCollider2D>().isTrigger = true;
        finish = true;
        myRB2D.velocity = Vector2.zero;
        myRB2D.angularVelocity = 0;
        if (boostProgBar.fillAmount != 0) boostProgBar.fillAmount = 0;
        glowSprite.gameObject.SetActive(false);

        manetteM.manetteDisabled();

        btnAClicked_stopSuperVisee(true);

        if (boostIngameManager._superViseurOn)
            boostIngameManager._superViseurOn = false;


        /*if (comportement == typeBehaviour.tuto)
        {
            TutoManager.getInstance().died();
        }
        else
        {*/
            if (comportement == typeBehaviour.infinis)
                infinisManager.died_stuck();

            gameManager.getInstance().died_stuck();
        //}

        if (_sourisGo.gameObject.activeSelf)
            _sourisGo.forceCloseEndGame();


        GhostBoostManager.getInstance().gameEnd(true);

        // reprit de stop()
        gameObject.SetActive(false);
        contentTrailGame.getInstance().playerDesactivated();
        if (corde != null)
            corde.positionCount = 0;
        SpriteTrailRender.SpriteTrailRenderer.trailOn = false;
    }


    public squeeze sQ;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sQ.gameObject.activeSelf)
        {
            sQ.waitingForSouris = false;
        }
        // tester si fonctionne tjr bien en enlevant code en dessous
        /* si la collision est dèrriere un portail
        if (portalIsInVisor)
        {
            RaycastHit[] hit = Physics.RaycastAll(transform.position, releaseVelocity, 3);
            // on regarde si il y a un portail avant la collision
            //bool portalFront = false;
            foreach (RaycastHit h in hit)
            {
                if (h.collider.tag == "ObstaclePortal")
                {
                    //portalFront = true;
                }
                else if (h.collider == collision.gameObject)
                {
                    break; // on peut calculer la collision
                }
            }
            //Debug.Log(Time.time + "Portal is in front of collision ? " + portalFront);
        }*/

        // dans le cas : player touche le bord d'un portail, mais ne se teleporte pas
        // portail.portalsInTriggerWithPlayer = 1 -> pas de collision
        // on doit checker : si portail est dans le viseur && que player touche le portail : on ignore
        if (!(collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "ObstacleBounce"))
            return;

        Collide(collision);
    }

    // (SUP) resoud les bug de collision, n'est normalement jamais appelé
    private void OnCollisionStay2D(Collision2D collision)
    {
        timeContact += Time.deltaTime;
        if (timeContact > 0.12f
            && lesPosCol.Count >= 1 // == 1 ( normalement )
            && !colBehindPlayer(releaseVelocity, collision.contacts[0].point, collision.contacts[0].normal))
        {
            Debug.Log("<color=red>BUG : collisionStay player , +0.12s</color>");
            timeContact = 0f;
            releaseVelocity = Vector2.Reflect(releaseVelocity, collision.contacts[0].normal);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sQ.gameObject.activeSelf)
        {
            sQ.triggerEnter(collision);
        }

        //Debug.Log("Name colide : " + collision.gameObject.name);
        if (!hasReleased
            && boostIngameManager._bouclierOn
            && collision.gameObject.tag == "Obstacle"
            && !collision.gameObject.name.Contains("horloge") && !collision.gameObject.name.Contains("barre")
            )
        {
            btnChangeRotation();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (sQ.gameObject.activeSelf)
        {
            sQ.triggerExit(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (sQ.gameObject.activeSelf)
        {
            sQ.triggerStay(collision);
        }
    }
    #endregion

    /*
     * void changeSpeedRotation(float newValue)
     * void btnChangeRotation()
    */
    #region Interaction UI ingame
    public static void changeSpeedRotation(float newValue)
    {
        rotationSpeedFactor = newValue;
    }
    public void btnChangeRotation()
    {
        //Debug.Log("(" + Time.time + ") , player change rotation");

        //sens = sens == -1 ? 1 : -1;
        animOnde.transform.position = transform.position;
        Animation ondeAnim = animOnde.GetComponent<Animation>();
        if (ondeAnim.isPlaying)
        {
            ondeAnim["ondeRelease"].time = 0f;
        }
        else
        {
            ondeAnim.Play("ondeRelease");
        }
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.manette_switch);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.checkP_release);

        // enregistre le sens de rotation que le player choisit sur init checkP
        // => quand respawn, tournera dans le même sens choisit
        if (center == initCenter)
        {
            sens *= -1;
        }
        axis.z *= -1;
    }

    #endregion


    public Transform visee_rotate;
    public Transform target_superVisee;

    #region Super Viseur
    public void activateSuperViseur(float t)
    {
        StartCoroutine(superViseur_slow_down(t));
        StartCoroutine(superViseur_visee(t));
    }
    private IEnumerator superViseur_visee(float t_wait)
    {
        float t = 0f;
        visee_rotate.GetComponent<obstacleRotate>().setInitPosVisee();
        visee_rotate.gameObject.SetActive(true);
        visee_rotate.transform.position = transform.position;

        superViseeLine.positionCount = 2;

        //mouseUp_line = false;

        while (t < t_wait && boostIngameManager._superViseurOn && !finish)
        {
            yield return null;
            t += Time.deltaTime;

            Vector3 forcePos = target_superVisee.position - transform.position;
            setViseur(true, forcePos);
            /* // /!\ Modifier l'interception du clic : btnA doit le faire
            if (
                !portail.isTeleporting &&
                (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended
                || Input.GetKeyUp(gameManager.hangKey))
                )
            {
                if (mouseUp_line)
                {
                    click_superVisee(forcePos);
                    visee_rotate.gameObject.SetActive(false);

                    yield break;
                }
                mouseUp_line = true;
            }*/
        }

        //Debug.Log(Time.deltaTime+" (superViseur_visee) end superVisee , time out");

        superViseeLine.positionCount = 0;
        /*superViseeLine.positionCount = 0;
        visee_rotate.gameObject.SetActive(false);
        boostIngameManager._superViseurOn = false;*/
    }

    private IEnumerator superViseur_slow_down(float timeBoost)
    {
        //cameraGame.getInstance().activateAigle();
        // modifie la vitesse de player le temps que fusee on
        float t = 0;
        //float percent_time_acceleration = 0.15f, percent_time_decceleration = 0.1f;      
        float const_time = timeBoost - (acce_time);// + decce_time);
        float diff_speed = 1 - speed_superViseurMin;

        speed_superViseur = 1f;
        while (t < timeBoost && !finish) // && boostIngameManager._superViseurOn
        {
            yield return null;
            t += Time.deltaTime;

            if (!boostIngameManager._superViseurOn)
            {
                t = timeBoost;
            }



            if (t < acce_time) // decceleration
            {
                speed_superViseur = 1 - (t / acce_time) * diff_speed;
            }
            else if (t < acce_time + const_time) // vitesse constante
            {
                speed_superViseur = 1 - diff_speed;
            }
            else // acceleration
            {
                //cameraGame.getInstance().desactivateAigle();
                //float timeLeft = t - acce_time + const_time;
                float t_2 = t - (acce_time + const_time);
                speed_superViseur = 1 - (diff_speed - (diff_speed * (t_2 / decce_time)));
                if (speed_superViseur > 1)
                {
                    speed_superViseur = 1;
                }
            }
        }

        //Debug.Log(Time.deltaTime + " (superViseur_slow_down) end superVisee , time out");
        prev_speed = speedOnBounce;
        if (speed_superViseur != 1)
        {
            speed_superViseur = 1f;
            //cameraGame.getInstance().desactivateAigle();
        }
    }


    public void btnAClicked_stopSuperVisee(bool died)
    {
        //Debug.Log("end superVisee , btnA clicked while in boost");
        boostIngameManager._superViseurOn = false;
        if (!died)
        {
            Vector3 forcePos = target_superVisee.position - transform.position;
            click_superVisee(forcePos);
        }
        superViseeLine.positionCount = 0;
        visee_rotate.gameObject.SetActive(false);
        boostIngameManager._superViseurOn = false;
        //visee_rotate.gameObject.SetActive(false);
    }

    private void click_superVisee(Vector3 newDirection)
    {
        // anim onde
        animOnde.transform.position = transform.position;
        animOnde.GetComponent<Animation>().Play("ondeRelease");
        boostIngameManager.getInstance().stopAnimBoost();
        boostIngameManager._superViseurOn = false;
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_release);
        // player change releaseVelocity
        posOnRelease = transform.position;

        // // Vector3 speed = releaseVelocity * Time.deltaTime * prev_speed * factorTranslate;.
        releaseVelocity = 1.7f * newDirection.normalized;
        //releaseVelocity = newDirection.normalized;
        //releaseVelocity = (/*Time.deltaTime * */newDirection.normalized * SPEED_CONST * factorTranslate);
        superViseeLine.positionCount = 0;
    }

    private float decce_time = 0.4f;
    private float acce_time = 0.3f;
    public float speed_superViseurMin = 0.15f;
    public static float speed_superViseur = 1f;

    #endregion
}
