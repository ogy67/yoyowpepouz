using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct boostImgInfos
{
    public typeBoostGame type;
    public GameObject img_associe;
    public Color ondeColor;
}

public class boostIngameManager : MonoBehaviour
{
    //public boostImgInfos[] _infos;
    public playerMovment playerMov;

    private static boostIngameManager instance;
    public static boostIngameManager getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("playerObject").GetComponent<boostIngameManager>();
        return instance;
    }


    public GameObject aimant_go;
    public itemBoostGame boost_superPower;
    public itemBoostGame boost_1, boost_2;
    private List<typeBoostGame> boost_used;
    private Dictionary<typeBoostGame, itemBoostGame> itemVoyant_byType;
    private bool loadIsLocked = false;
    public static bool isEquipedWithSP = false;

    public bool playerIsUsingBoost(typeBoostGame type)
    {
        return boost_used.Contains(type);
    }


    public void playerReplay()
    {
        foreach (itemBoostGame iB in new itemBoostGame[] { boost_superPower, boost_1, boost_2 })
        {
            iB.playerReplay();
        }
    }

    /*
     * void initValues()
     * void loadBoostComponents()
     * IEnumerator lockLoad()
    */
    #region initialisation Debut Game
    public void initValues() // les 3 voyants boost l'appellent
    {
        // 3 appel, on laisse que passer le premier
        if (loadIsLocked)
            return;
        StartCoroutine(lockLoad());

        boost_used = new List<typeBoostGame>();
        itemVoyant_byType = new Dictionary<typeBoostGame, itemBoostGame>();
        typeBoostGame tB;
        foreach(itemBoostGame iB in new itemBoostGame[] { boost_superPower, boost_1, boost_2 })
        {
            tB = iB.getBoostType();
            if (tB != typeBoostGame.none)
            {
                boost_used.Add(tB);
                itemVoyant_byType.Add(tB, iB);
            }
        }

        isEquipedWithSP = boost_superPower.getBoostType() != typeBoostGame.none;

        // s'assurer que pas de cut pendant les boost
        _ressortOn = false;
        //_ghostOn = false;
        _bouclierOn = false;
        _superViseurOn = false;
        _fuseeOn = false;
        _bombeOn = false;
        //speed_fusee = 1f;

        loadBoostComponents();
    }

    public Vector3 init_aimantScale;
    public static float _factorAncre;
    // active les boost sur player
    private void loadBoostComponents()
    {
        // souris : changes les collision de player
        // aimant : trigger de detection de bonus
        // 
        if (expertMode.modeExpertOn && !infinisManager.infinis_Mode_On)
        {
            _ancreOn = false;
            _fuseeOn = false;
            _sourisOn = false;
            _playerColider.enabled = true;
            aimant_go.SetActive(false);
            return;
        }

        aimant_go.transform.localScale = init_aimantScale;
        //ancre_go.transform.localScale = init_aimantScale; // a la même init scale

        if (boost_used.Contains(typeBoostGame.aimant))
        {
            aimant_go.SetActive(true);
            float size = itemVoyant_byType[typeBoostGame.aimant].getSizeAimant();
            aimant_go.transform.localScale *= size;
        }
        else
        {
            aimant_go.SetActive(false);
        }

        if (boost_used.Contains(typeBoostGame.ancre))
        {
            _ancreOn = true;
            //ancre_go.SetActive(true);
            _factorAncre = itemVoyant_byType[typeBoostGame.ancre].getSizeAncre();
            //ancre_go.transform.localScale *= _factorAncre;
        }
        else
        {
            //ancre_go.SetActive(false);
            _ancreOn = false;
        }

        if (boost_used.Contains(typeBoostGame.souris))
        {
            //playerMov.transform.GetComponent<CircleCollider2D>().isTrigger = true;
            _playerColider.enabled = false;
            souris_colider.SetActive(true);
            souris_colider.GetComponent<squeeze>().initGame(itemVoyant_byType[typeBoostGame.souris].getSizeSouris());
            //float size = maxSizeSouris * itemVoyant_byType[typeBoostGame.souris].getSizeSouris();
            //souris_colider.transform.localScale = new Vector2(size, size);
            _sourisOn = true;
        }
        else
        {
            _playerColider.enabled = true;
            //playerMov.transform.GetComponent<CircleCollider2D>().isTrigger = false;
            souris_colider.SetActive(false);
            _sourisOn = false;
        }

        _fuseeOn = boost_used.Contains(typeBoostGame.fusee);
        speedFusee = _fuseeOn ? itemVoyant_byType[typeBoostGame.fusee].getTimeEffectBoost() : 1f;
    }

    public static float speedFusee;

    public float maxSizeSouris;

    public static bool _sourisOn = false;

    //public GameObject souris_detector;
    public GameObject souris_colider;
    public CircleCollider2D _playerColider;

    private IEnumerator lockLoad()
    {
        loadIsLocked = true;
        yield return new WaitForSeconds(0.2f);
        loadIsLocked = false;
    }

    #endregion

    public bool canUseBoost(typeBoostGame tBG)
    {
        //if (TutoManager.tutoOn)
        //    return false;
        if (!boost_used.Contains(tBG))
            return false;
        return itemVoyant_byType[tBG].canUseBoost();
    }

    /*private boostImgInfos getInfoByType(typeBoostGame tBG)
    {
        foreach (boostImgInfos bII in _infos)
        {
            if (bII.type == tBG)
                return bII;
        }
        return new boostImgInfos();
    }*/

    public void infiniteReccupBoostSP()
    {
        boost_superPower.playerGetASuperPowerBoost();
    }

    public int getNbSuperPowerPlayerHas()
    {
        return boost_superPower.nbBoostRestant;
    }

    public int getNbInitSuperBoost()
    {
        return boost_superPower.currentBoost.getNbBoost();
    }



    private Dictionary<typeBoostGame, typeDefi> typeBoost_toTypeDefi =
        new Dictionary<typeBoostGame, typeDefi>() {
            { typeBoostGame.bombe, typeDefi.boost_bombe },
        { typeBoostGame.bouclier, typeDefi.boost_bouclier },
        { typeBoostGame.super_viseur, typeDefi.boost_superVisee },
        { typeBoostGame.ghost, typeDefi.boost_fantome },
        { typeBoostGame.pierre_tombale, typeDefi.boost_pierreTombale },
        { typeBoostGame.ressort, typeDefi.boost_ressort },
        { typeBoostGame.souris, typeDefi.boost_souris }};

    public animBoostUsedIngame instanceAnimBoost;

    public SpriteRenderer ondeBoostImg;
    private List<typeBoostGame> legendaryBoost = new List<typeBoostGame>() {
        typeBoostGame.bombe, typeBoostGame.bouclier, typeBoostGame.super_viseur};
    //private bool animatingBoost = false;
    //private GameObject close_endAnim;
    /*
     * void playerUseABoost(typeBoostGame tBG)
     * IEnumerator closeBoostLate(GameObject toClose)
    */
    #region animation Activation boost

    private void playerUseABoost(typeBoostGame tBG, bool isSuperPower)
    {
        itemVoyant_byType[tBG].playerUseBoost();

        // on dit a defi manager qu'un boost a été utilisé
        if (typeBoost_toTypeDefi.ContainsKey(tBG))
            defiManager.getInstance().defiIsIncremented(typeBoost_toTypeDefi[tBG], 1);

        /* verifie si en train anim SB : aucun autre boost ne peut se supperposer 
        if (animatingBoost)
        {
            if(tBG == typeBoostGame.bombe 
                || tBG == typeBoostGame.bouclier
                || tBG == typeBoostGame.fusee
                || tBG == typeBoostGame.super_viseur)
            {
                if (close_endAnim.activeSelf)
                    close_endAnim.SetActive(false);
            }
            else
                return;
        }*/


        // on anim le boost en fonction du boost utilisé
        /* doit être géré par instance
        boostImgInfos bI = getInfoByType(tBG);
        bI.img_associe.SetActive(true);
        ondeBoostImg.color = new Color(bI.ondeColor.r, bI.ondeColor.g, bI.ondeColor.b, 0f);

        // verifie si utiliser medic après mort de player ou de ghost
        if (tBG == typeBoostGame.medic && playerGhost.typeGhost != typePlayerGhost.none)
        {
            _useBoosAnim.GetComponent<followTarget>().isActive = false;
            Vector2 animPos;

            if (playerGhost.typeGhost == typePlayerGhost.pierreT)
            {
                animPos = playerGhost.pierreTombale_endPos;
                playerGhost.pierreTombale_endPos = Vector2.zero;
            }
            else
            {
                animPos = playerGhost.posEndGhost[Random.Range(0, playerGhost.posEndGhost.Count)];
                playerGhost.posEndGhost = new List<Vector2>();
            }

            playerGhost.typeGhost = typePlayerGhost.none;
            _useBoosAnim.transform.position = animPos;
        }*/

        instanceAnimBoost.animateNewBoost(tBG);

        if (!legendaryBoost.Contains(tBG))
        {
            sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_boostNormal);
        }
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.boostNormal);

        /*if(close_endAnim != null)
            close_endAnim.SetActive(false);

        _useBoosAnim.Play("use_boost");
        close_endAnim = bI.img_associe;
        StartCoroutine(closeBoostLate(close_endAnim));   */
    }
    /*private IEnumerator closeBoostLate(GameObject toClose)
    {
        //animatingBoost = true;
        yield return new WaitForSeconds(45/60f);
        if (toClose.activeSelf) // si n'a pas été désactivé par autre anim boost
        {
            toClose.SetActive(false);
            close_endAnim = null;
            // reinitialiser après pierreTombale->medic
            _useBoosAnim.GetComponent<followTarget>().isActive = true;
        }
        //animatingBoost = false;
    }*/
    #endregion

    public boostBouclierCheckP bouclierCheckPCs;
    public Transform progBarTime_parent;
    public Image progBarTime;
    public static bool _bouclierOn = false, _superViseurOn = false, _fuseeOn = false, _bombeOn = false;
    public static typeBoostGame superPowerOn = typeBoostGame.none;
    public GameObject shockWaveGo;
    public btnManette btnManetteA;
    /*
     * void superPowerClicked()
     * IEnumerator switch_On_Off_boolBoost(typeBoostGame type, float timeSuperPower)
     * IEnumerator activateProgBar(boostImgInfos bI)
     * IEnumerator activateSuperPower(typeBoostGame type)
     * IEnumerator shockWaveAnim()
     * void stopAnimBoost()
    */
    #region gestion SUPER POWER
    // Activation des boost
    public bool superPowerClicked() // return true si boost peut être activé
    {
        if (superPowerOn != typeBoostGame.none) // éviter double activation
            return false;
        // Super powers
        if (boost_superPower.boostEmpty() == false)
        {
            //Debug.Log("2_1");
            if (boost_superPower.canUseBoost() == true)
            {
                // on utilise le boost
                playerUseABoost(boost_superPower.getBoostType(), true);
                //boost_superPower.playerUseBoost();
                StartCoroutine(activateSuperPower(boost_superPower.getBoostType())); 
                ondeSuperB.animOndeSuperB();
                return true;
            }
        }
        return false;
    }

    private IEnumerator switch_On_Off_boolBoost(typeBoostGame type, float timeSuperPower)
    {
        switch (type)
        {
            case typeBoostGame.bouclier:
                _bouclierOn = true;
                yield return new WaitForSeconds(timeSuperPower);
                _bouclierOn = false;
                break;
            case typeBoostGame.bombe:
                _bombeOn = true;
                yield return new WaitForSeconds(timeSuperPower);
                _bombeOn = false;
                break;
            case typeBoostGame.super_viseur:
                _superViseurOn = true;
                // different : click pendant boost doit le stopper
                float t = 0f;
                while (t < timeSuperPower && _superViseurOn)
                {
                    yield return null;
                    t += Time.deltaTime;
                }
                _superViseurOn = false;
                playerMovment.speed_superViseur = 1f;
                break;
        }
    }

    private IEnumerator activateProgBar(boostImgInfos bI)
    {
        // on active progBar time et le boost
        progBarTime_parent.gameObject.SetActive(true);

        float t_wait = 0.4f, t = 0f;
        progBarTime.fillAmount = 1f;
        while (t < t_wait && superPowerOn != typeBoostGame.none)
        {
            progBarTime.color = new Color(bI.ondeColor.r, bI.ondeColor.g, bI.ondeColor.b, t / t_wait);
            yield return null;
            t += Time.deltaTime;
        }
        progBarTime.color = new Color(bI.ondeColor.r, bI.ondeColor.g, bI.ondeColor.b, 1f);
    }

    private IEnumerator shockWaveAnim()
    {
        shockWaveGo.SetActive(true);
        shockWaveGo.transform.position = playerMov.transform.position;
        shockWaveGo.GetComponent<Animation>().Play("shockWave");
        yield return new WaitForSeconds(2f);
        shockWaveGo.SetActive(false);
    }

    private IEnumerator activateSuperPower(typeBoostGame type)
    {
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_superB);
        //sfxManager.getInstance().playSound_ingame(typeSound_ingame.superPower);

        superPowerOn = type; // SUPPRIMER variable superPowerOn
        StartCoroutine(shockWaveAnim());
        //cameraGame.getInstance().bloomCamEffect();

        float timeSuperPower = boost_superPower.getTimeEffectBoost();

        StartCoroutine(switch_On_Off_boolBoost(type, timeSuperPower));

        /*if (type == typeBoostGame.fusee)
            StartCoroutine(useFusee(timeSuperPower));*/
        if (type == typeBoostGame.super_viseur)
        {
            //float timeEffect = itemVoyant_byType[type].getTimeEffectBoost();
            playerMov.activateSuperViseur(timeSuperPower);//timeEffect);
        }
        else if (type == typeBoostGame.bombe)
        {
            StartCoroutine(use_bombe(playerMovment.releaseVelocity, playerMov.transform.position, timeSuperPower));
            playerMov.activateBombe();
        }else if(type == typeBoostGame.bouclier)
        {
            GhostBoostManager.getInstance().bouclierUsed_stopGhost(timeSuperPower);
            bouclierCheckPCs.boostBouclier_activated();
        }

        // on active progBar time et le boost
        //progBarTime_parent.gameObject.SetActive(true);
        //bI.img_associe.SetActive(true);
        //progBarTime.color = new Color(bI.ondeColor.r, bI.ondeColor.g, bI.ondeColor.b, 1f);
         // commence à 1f, mais délais avant prend pos player

        yield return new WaitForSeconds(4 / 6f);
        float t = 4 / 6f;

        if (type != typeBoostGame.bombe)
        {
            StartCoroutine(activateProgBar(instanceAnimBoost.getInfoByType(type)));

            while (t < timeSuperPower
                && superPowerOn != typeBoostGame.none
                && !playerMovment.finish) // player died ou finished
                                          //&& itemBoostGame.open
            {
                yield return null;
                t += Time.deltaTime;
                progBarTime.fillAmount = 1 - t / timeSuperPower;
            }
            progBarTime.fillAmount = 0;
            progBarTime_parent.gameObject.SetActive(false);
        }
        else
        {
            while (t < timeSuperPower
                && superPowerOn != typeBoostGame.none)
                //&& itemBoostGame.open
                //&& !playerMovment.finish) // player died ou finished
            {
                yield return null;
                t += Time.deltaTime;
            }
            //    yield return new WaitForSeconds(timeSuperPower);
        }

        if (type == typeBoostGame.bouclier)
            bouclierCheckPCs.boostBouclier_desactivated();

        // /!\ si player meurt pendant utilisation du boost
        // 2 anim vont être appelées pour les contrôles : 'manetteOff' et 'boostOff'


        btnManetteA.superBoostEnded();
        superPowerOn = typeBoostGame.none;

    }

    // quand player win - dead - finished ( si pierre tombale : si finit )
    public void stopAnimBoost()
    {
        superPowerOn = typeBoostGame.none; // stop les animations
    }

    #endregion

    public void playerUse_ghost()
    {
        animFlash.animateFlash(playerMovment.releaseVelocity, typeFlash.ghost);
        playerUseABoost(typeBoostGame.ghost, false);
    }

    public static bool _ressortOn = false;
    /*
     * bool canUse_ressort()
     * IEnumerator useRessort()
    */
    #region RESSORT
    public bool canUse_ressort()
    {
        bool ret = false;
        if (canUseBoost(typeBoostGame.ressort))
        {
            playerUseABoost(typeBoostGame.ressort, false);
            StartCoroutine(useRessort());
            ret = true;
        }
        return ret;
    }
    private IEnumerator useRessort()
    {
        _ressortOn = true;
        yield return new WaitForSeconds(0.1f); // on attend que player ait bien contabilisé
        _ressortOn = false;
    }
    #endregion

    /*
     *  bool canUse_medic()
    */
    #region MEDIC
    public bool canUse_medic()
    {
        bool ret = false;
        if (canUseBoost(typeBoostGame.medic))
        {
            animOndeBoost.animateOnde(typeOnde.life);
            playerUseABoost(typeBoostGame.medic, false);
            ret = true;
        }
        return ret;
    }
    #endregion

    /*
     * void playerBounce(bool ressortBounce)
    */
    #region TIRELIRE

    public void playerBounce(bool ressortBounce) // si bounce avec boost ressort, ignorer
    {
        if (expertMode.modeExpertOn || ressortBounce)
            return;
        // Tirelire : incrementer
        if (boost_used.Contains(typeBoostGame.tirelire)
            && itemVoyant_byType[typeBoostGame.tirelire].incrementeTirelire())
            //&& canUseBoost(typeBoostGame.tirelire))
        {
            // incrementer la progBar itemBoost
            playerUseABoost(typeBoostGame.tirelire, false);
        }
    }

    #endregion

    public Animation aimantAnim2;
    public void aimantUsed()
    {
        playerUseABoost(typeBoostGame.aimant, false);
        aimantAnim2.Play("areaAimantGlow");
    }

        public static bool _ancreOn = false;
    public void ancreUsed()
    {
        playerUseABoost(typeBoostGame.ancre, false);
    }

    public void sourisUsed()
    {
        animFlash.animateFlash(playerMovment.releaseVelocity, typeFlash.souris);
        playerUseABoost(typeBoostGame.souris, false);
    }


    public GameObject ghost_item;
    private bool _pierreTombaleOn = false;
    private bool survivedPierreTombale = false;

    /*
     *  bool canUse_PierreTombale(Vector2 direction, Vector2 posPlayer)
     *  IEnumerator usePierreTombale(Vector2 direction, Vector2 posPlayer)
     *  void playerGhostEnterredEndCheckpoin()
    */
    #region PIERRE TOMBALE

    public bool canUse_PierreTombale(Vector2 direction, Vector2 posPlayer)
    {
        bool ret = false;
        if (canUseBoost(typeBoostGame.pierre_tombale))
        {
            survivedPierreTombale = false;
            _pierreTombaleOn = true;
            animOndeBoost.animateOnde(typeOnde.pierreT);
            playerUseABoost(typeBoostGame.pierre_tombale, false);
            StartCoroutine(usePierreTombale(direction, posPlayer));
            ret = true;
        }
        return ret;
    }
    private IEnumerator usePierreTombale(Vector2 direction, Vector2 posPlayer)
    {
        float time_effect = itemVoyant_byType[typeBoostGame.pierre_tombale].getTimePierreTombale();
        GameObject o = Instantiate(ghost_item);
        o.SetActive(true);
        o.GetComponent<playerGhost>().ghost_spawn(posPlayer, direction, time_effect);
        playerGhost.typeGhost = typePlayerGhost.pierreT;

        yield return new WaitForSeconds(time_effect + 0.1f); // +0.1F : laisser temps de mourrir a ghost
        _pierreTombaleOn = false;

        if(!survivedPierreTombale)
        {
            // dire a player que mort
            playerMovment.resultPierreTombale = 0;
        }
    }

    #endregion

    public void playerGhostEnterredEndCheckpoin()
    {
        if (_pierreTombaleOn)
        {
            // on dit à player que pas mort
            survivedPierreTombale = true;
            playerMovment.resultPierreTombale = 1;
        }
        else // bombe
        {
            survivedBombe = true;
            playerMovment.resultBombe = 1;
            // pour tout les ghost crées, on leurs dit de se stopper
            foreach (GameObject o in allBombeGhost)
            {
                o.GetComponent<playerGhost>().anotherGhostEnterredCheckP();
            }
        }
    }


    private static bool survivedBombe = false;
    float[] angles = new float[] { -90, -22.5f, 22.5f, 90 };
    private List<GameObject> allBombeGhost;// = new List<GameObject>();
    /*
     * void playerGhostEnterredEndCheckpoin()
     * IEnumerator use_bombe(Vector2 direction, Vector2 posPlayer, float timeEffect)
     * void ignoreCollision(List<GameObject> l)
     * static Vector2 Rotate(Vector2 v, float degrees)
    */
    #region BOMBE
    private IEnumerator use_bombe(Vector2 direction, Vector2 posPlayer, float timeEffect)
    {
        allBombeGhost = new List<GameObject>();
        survivedBombe = false;
        // toutes les animes sont faites avant

        List<GameObject> ignoreC = new List<GameObject>();
        ignoreC.Add(playerMov.gameObject);

        float time_effect = itemVoyant_byType[typeBoostGame.bombe].getTimeEffectBoost();

        for (int i = 0; i < 4; i++)
        {
            Vector2 direction_modified = Rotate(direction, angles[i]);
            GameObject o = Instantiate(ghost_item);
            allBombeGhost.Add(o);
            ignoreC.Add(o);
            //o.SetActive(true);
            o.GetComponent<playerGhost>().ghost_spawn(posPlayer, direction_modified, time_effect);


            playerGhost.typeGhost = typePlayerGhost.bombe;
        }

        if(playerMovment.comportement == typeBehaviour.online)
        {
            List<GameObject> onlineBombGhost = new List<GameObject>();
            for(int i = 1; i < ignoreC.Count; i++) // 1er item est palet local => ignore
            {
                onlineBombGhost.Add(ignoreC[i]);
            }
        }
        
        ignoreCollision(ignoreC);

        yield return new WaitForSeconds(timeEffect);
        if(!survivedBombe)
            playerMovment.resultBombe = 0;

    }

    private void ignoreCollision(List<GameObject> l)
    {
        while(l.Count > 1)
        {
            for (int i = 1; i < l.Count; i++)
                Physics2D.IgnoreCollision(
                    l[0].GetComponent<CircleCollider2D>(), l[i].GetComponent<CircleCollider2D>());
            l.Remove(l[0]);
        }
    }
    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    #endregion

    #region FUSEE
    public void use_fusee()
    {
        playerUseABoost(typeBoostGame.fusee, false);

    }
    #endregion





    /*
 * void playerBounce()
 * void playerHitObstacle()
*/
    #region gestion evenement Activation boost ( pas légendaire )


    public void playerHitObstacle()
    {
        // Fantôme : passer au travers obstacle
        // SI obstacle n'est pas un mur
        if (boost_used.Contains(typeBoostGame.ghost)
            && canUseBoost(typeBoostGame.ghost))
        {
            // transformer l'obstacle en trigger
            // ignorer la collision
            playerUseABoost(typeBoostGame.ghost, false);
        }
        // Ressort : rebondir
        else if (boost_used.Contains(typeBoostGame.ressort)
            && canUseBoost(typeBoostGame.ressort))
        {
            // rebondir sur l'obstacle
            // ignorer la collision
            playerUseABoost(typeBoostGame.ressort, false);
        }
        // Pierre tombale : continuer à rebondir x secondes
        if (boost_used.Contains(typeBoostGame.pierre_tombale)
            && canUseBoost(typeBoostGame.pierre_tombale))
        {
            // laisser mourrir
            playerUseABoost(typeBoostGame.pierre_tombale, false);
        }
    }

    public void playerDied()
    {
        // Medic : donner vie
        if (boost_used.Contains(typeBoostGame.medic)
            && canUseBoost(typeBoostGame.medic))
        {
            playerUseABoost(typeBoostGame.medic, false);
        }

    }
    #endregion
}
