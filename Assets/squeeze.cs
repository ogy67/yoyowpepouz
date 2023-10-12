using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squeeze : MonoBehaviour
{
    public Transform posMax, posBorderPlayer;
    public playerMovment _player;
    public Transform _childCollider;
    public Transform _childTrigger;

    // Start is called before the first frame update
    void Start()
    {
        _initScaleCollider = _childCollider.localScale;
        _initScaleTrigger = _childTrigger.localScale;
    }

    public void forceCloseEndGame()
    {
        nbObjectInTrigger = 0;
        prevPosTrigger = new List<Vector2>();
        currentTriggerClosestPoint = new List<Vector2>();
        ignoredObstacles = new List<GameObject>();
        isRescalingAutomatic = false;
        waitingForSouris = false;
        _childCollider.localScale = _initScaleCollider;
        _childTrigger.localScale = _initScaleTrigger;
        gameObject.SetActive(false);
    }


    private float _maxDistanceCollide = -1, _borderPlayerDistance = -1;
    /*
     * float getMaxDistance()
     * float getPlayerBorderDistance()
    */
    #region getDistance
    private float getMaxDistance()
    {
        if(_maxDistanceCollide == -1)
        {
            _maxDistanceCollide = Vector2.Distance(posMax.position, transform.position);
        }
        return _maxDistanceCollide;
    }
    private float getPlayerBorderDistance()
    {
        if(_borderPlayerDistance == -1)
        {
            _borderPlayerDistance = Vector2.Distance(posBorderPlayer.position, transform.position);
        }
        return _borderPlayerDistance;
    }
    #endregion

    private List<Vector2> currentTriggerClosestPoint = new List<Vector2>();
    private List<Vector2> prevPosTrigger = new List<Vector2>();
    private static int nbObjectInTrigger = 0;
    private int nbTriggerThisFrame;
    public bool waitingForSouris = false;
    // les obstacles qui sont derrière les obstacles rebondis
    private List<GameObject> ignoredObstacles = new List<GameObject>();
    /*
     * void triggerEnter(Collider2D collision)
     * void triggerStay(Collider2D collision)
     * void triggerExit(Collider2D collision)
     * bool checkIfObstacleBounce_frontObstacle(Vector2 direction, Vector2 closestPoint, GameObject obstacle)
     * void FixedUpdate()
    */
    #region fonction PHYSIQUE appelés par player
    public void triggerEnter(Collider2D collision)
    {
        // squeeze que entre des obstacles
        if (collision.gameObject.tag != "Obstacle")
        {
            // on ne fait rien
            return;
        }
        else if (checkIfObstacleBounce_frontObstacle(playerMovment.releaseVelocity
            , collision.ClosestPoint(transform.position), collision.gameObject))
        {
            ignoredObstacles.Add(collision.gameObject);
            return;
        }

        nbObjectInTrigger++;
        currentTriggerClosestPoint.Add(collision.ClosestPoint(transform.position));
    }
    public void triggerStay(Collider2D collision)
    {
        // squeeze que entre des obstacles
        if (collision.gameObject.tag != "Obstacle")
        {
            // on ne fait rien
            return;
        }
        currentTriggerClosestPoint.Add(collision.ClosestPoint(transform.position));
    }
    public void triggerExit(Collider2D collision)
    {
        // squeeze que entre des obstacles
        if (collision.gameObject.tag != "Obstacle")
        {
            // on ne fait rien
            return;
        }
        else if (ignoredObstacles.Contains(collision.gameObject))
        {
            ignoredObstacles.Remove(collision.gameObject);
            return;
        }

        if (nbObjectInTrigger > 0)
            nbObjectInTrigger--;

        if (nbObjectInTrigger == 0)
        {
            cleanTriggerPositionTooFar();
            // il n'y a plus aucun point de colision dans le champ
            if (prevPosTrigger.Count == 0)
            {
                // on redonne la taille normale
                _childCollider.localScale = _initScaleCollider;
                _childTrigger.localScale = _initScaleTrigger;
            }
            // il y a encore des points de collision
            else if (!isRescalingAutomatic
                && !playerMovment.finish)
            {
                StartCoroutine(rescaleWhileNotNormal());
            }
        }
    }
    private bool checkIfObstacleBounce_frontObstacle(Vector2 direction, Vector2 closestPoint, GameObject obstacle) 
    {
        if (Vector2.Angle(
            playerMovment.releaseVelocity, closestPoint - (Vector2)_player.transform.position) > 90f)
            return true; // on ignore l'obstacle si est derrière

        // si obstacle rebondis OU portal devant obstacle : ne pas le prendre en compte
        // sinon taille est diminuée alors que player rebondis OU se teleporte seulement

        RaycastHit2D[] ray = Physics2D.RaycastAll(transform.position, direction, 2f);
        bool obstacleFront = false;
        bool obstacleFound = false;

        foreach (RaycastHit2D hit in ray)
        {
            if (!obstacleFront
                && hit.collider.tag == "ObstacleBounce" || hit.collider.tag == "ObstaclePortal")
            {
                obstacleFront = true;
            }
            else if (hit.collider.gameObject == obstacle)
            {
                obstacleFound = true;
                break;
            }
        }
        return obstacleFront && obstacleFound ? true : false;
    }
    private void FixedUpdate()
    {
        nbTriggerThisFrame = currentTriggerClosestPoint.Count;
        // on change la taille en fonction du point de collision le plus proche
        if (nbTriggerThisFrame > 0)
        {
            Vector2 closest = currentTriggerClosestPoint[0];
            float distance = Vector2.Distance(transform.position, closest);

            if (nbTriggerThisFrame > 1)
            {
                foreach (Vector2 v in currentTriggerClosestPoint)
                {
                    if (Vector2.Distance(transform.position, v) < distance)
                    {
                        closest = v;
                        distance = Vector2.Distance(transform.position, closest);
                    }
                }
            }
            if (!playerMovment.finish && !portail.isTeleporting)
                setScale(distance);

            prevPosTrigger.Add(closest);
            currentTriggerClosestPoint = new List<Vector2>();
        }
    }
    #endregion


    private bool isRescalingAutomatic = false;
    private Vector2 _initScaleCollider;
    private Vector2 _initScaleTrigger;
    /*
     * IEnumerator rescaleWhileNotNormal()
     * void setScale(float distance)
     * void cleanTriggerPositionTooFar()
    */
    #region fonction SCALE SOURIS 
    private IEnumerator rescaleWhileNotNormal()
    {
        isRescalingAutomatic = true;
        int nbPt = prevPosTrigger.Count;
        int maxPt = nbPt;
        float distance;


        while (nbPt > 0 && maxPt >= nbPt) // on arrête si player entre en contact avec de nouveau coliders
        {
            cleanTriggerPositionTooFar();
            // on se scale en fonction du plus proche
            distance = 20f;

            foreach (Vector2 v in prevPosTrigger)
            {
                if (Vector2.Distance(transform.position, v) < distance)
                {
                    distance = Vector2.Distance(transform.position, v);
                }
            }
            if (waitingForSouris && distance > getPlayerBorderDistance())
            {
                waitingForSouris = false;

                // on utilise la souris seulement si :
                // - pas win
                // - pas sur checkP
                // - pas sur pivot
                bool p = obstaclePivot.isPushing;
                bool w = playerMovment.finish;
                bool c = playerMovment.hasReleased;

                if(!obstaclePivot.isPushing &&
                    !playerMovment.finish &&
                    playerMovment.hasReleased)
                    boostIngameManager.getInstance().sourisUsed();
            }

            float rapport = (distance / getMaxDistance());
            if (rapport > 1) rapport = 1;
            _childCollider.localScale = _initScaleCollider * rapport;
            _childTrigger.localScale = _initScaleTrigger * rapport;

            nbPt = prevPosTrigger.Count;
            yield return null;
            maxPt = prevPosTrigger.Count;
        }
        isRescalingAutomatic = false;
    }
    private void setScale(float distance)
    {
        float rapport;
        if (distance <= getPlayerBorderDistance())
        {
            if (waitingForSouris)
                return;
            rapport = getPlayerBorderDistance() / getMaxDistance();
            waitingForSouris = true;

        }
        else
            rapport = (distance / getMaxDistance());

        if (rapport > 1) rapport = 1;

        _childCollider.localScale = _initScaleCollider * rapport;
        _childTrigger.localScale = _initScaleTrigger * rapport;
    }
    private void cleanTriggerPositionTooFar()
    {
        int i = 0;
        // on supprime tout les points de colision qui ne sont hors portée de trigger
        while (i != prevPosTrigger.Count)
        {
            if (Vector2.Distance(prevPosTrigger[i], transform.position) > getMaxDistance())
                prevPosTrigger.Remove(prevPosTrigger[i]);
            else
                i++;
        }
    }
    #endregion


    private Vector2 initLocalScale = Vector2.zero;
    /*
     * Vector2 getInitLocalScaleTrigger()
     * void initGame(float sizeSouris)
    */
    #region fonction initialisation taille souris en fonction niveau

    private Vector2 getInitLocalScaleTrigger()
    {
        if (initLocalScale == Vector2.zero)
            initLocalScale = _childTrigger.localScale;
        return initLocalScale;
    }
    public void initGame(float sizeSouris)
    {
        // si 0.8 -> 1+(1-0.8) -> 1.2; le facteur de taille est 1.2
        float sizeFactor = 1 + (1 - sizeSouris);
       
        Vector2 newSizeTrigger = getInitLocalScaleTrigger() * sizeFactor;
        _childTrigger.localScale = newSizeTrigger;
    }
    #endregion

}
