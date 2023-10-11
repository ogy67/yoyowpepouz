using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostDetector : MonoBehaviour
{
    private List<string> obstacleNotGhosted = new List<string>()
    {
        "obstacle", "bordParticulesD", "bordParticulesF"
    };
    private static List<GameObject> obstacleTriggered = new List<GameObject>();
    /*
     * bool canGoThroughObstacle(GameObject toIgnore)
     * void OnTriggerEnter2D(Collider2D collision)
     * void obstacleChangeTrigger(GameObject toIgnore, bool isTrigger)
     * void forceResetObstacles()
    */
    #region changer collisions obstacle
    private bool canGoThroughObstacle(GameObject toIgnore)
    {
        string[] name_split = toIgnore.name.Split(' ');
        //Debug.Log("Try ghost, object name : " + name_split[0] + ", canBeGhosted : " + !obstacleNotGhosted.Contains(name_split[0]));
        return !obstacleNotGhosted.Contains(name_split[0]);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("<color=blue>GHOST DETECT OBSTACLE </color>"+ collision.gameObject.name+ " (ghostOn:"+ghostOn+")");
        if(!ghostOn && collision.gameObject.tag == "Obstacle"
            && canGoThroughObstacle(collision.gameObject))

        {
            //Debug.Log("<color=green>GHOST DETECT GHOSTED </color>");

            GameObject go = collision.gameObject;
            string n = go.name.Split(' ')[0];
            GameObject goLame = n == "obstacleLame" ? collision.gameObject : n == "triangle" ? collision.transform.parent.gameObject : null;
            bool obstacleLame = n == "obstacleLame" || n == "triangle";


            if (!obstacleLame)
            {
                //Debug.Log("Wait activate ghost : Track obstacle");
                obstacleChangeTrigger(collision.gameObject, true);
                StartCoroutine(trackObstacle(collision));
            }
            // bug , ne prend aucune des proposition sur certains obstacleLame
            else if (goLame != currentObstacleLameTracked)
            {
                //Debug.Log("Wait activate ghost : Track lame");
                currentObstacleLameTracked = goLame;
                obstacleChangeTrigger(goLame, true);
                StartCoroutine(trackObstacleLame(goLame));
            }
        }    
    }

    private void obstacleChangeTrigger(GameObject toIgnore, bool isTrigger)
    {
        List<Transform> modify = new List<Transform>();

        foreach (Transform t in toIgnore.transform)
            if (t.GetComponent<CircleCollider2D>()
                || t.GetComponent<BoxCollider2D>()
                || t.GetComponent<PolygonCollider2D>())
                modify.Add(t);

        modify.Add(toIgnore.transform);

        foreach (Transform t in modify)
        {
            if (t.GetComponent<CircleCollider2D>())
                t.GetComponent<CircleCollider2D>().isTrigger = isTrigger;
            else if (t.GetComponent<BoxCollider2D>())
                t.GetComponent<BoxCollider2D>().isTrigger = isTrigger;
            else if (t.GetComponent<PolygonCollider2D>())
                t.GetComponent<PolygonCollider2D>().isTrigger = isTrigger;
        }

        if (isTrigger)
            obstacleTriggered.Add(toIgnore);
    }

    // appelé quand gameEnd
    public void forceResetObstacles()
    {
        foreach (GameObject o in obstacleTriggered)
        {
            obstacleChangeTrigger(o, false);
        }
        obstacleTriggered = new List<GameObject>();
        currentObstacleLameTracked = null;

        // couroutine trackObstacle peut être coupé par désactivation du ghost 
        // si meurt cash après ghost ( obstacle trop près de contour map

        // si la coroutine a été coupé OU appel normal depuis ce script en fin de track
        if (ghostOn)
        {
            //Debug.Log("<color=red>ENUM cut by desactivation go</color>");
            ghostOn = false;
            gameObject.SetActive(false);
            _ghostBoostManager.ghostEnds();
        }
    }

    public void ghostActivated_resetAllObstaclesEcxeptGhosted(GameObject ghosted)
    {
        if (obstacleTriggered.Count > 1)
        {
            foreach (GameObject o in obstacleTriggered)
            {
                if (o != ghosted)
                    obstacleChangeTrigger(o, false);
            }
            obstacleTriggered = new List<GameObject>{ghosted};
        }
    }
    #endregion

    public Transform posBorderPlayer_souris;
    public Transform triggerBorderPos, playerBorderPos;
    public GhostBoostManager _ghostBoostManager;
    private float distanceTriggerBorder = 0f, distancePlayerBorder = 0f;
    public bool ghostOn = false;
    /*
     * float getDistanceTriggerBorder()
     * float getDistancePlayerBorder()
     * IEnumerator trackObstacle(Collider2D collision)
    */
    #region track obstacle
    private float getDistanceTriggerBorder()
    {
        if (distanceTriggerBorder == 0f)
            distanceTriggerBorder = Vector2.Distance(triggerBorderPos.position, transform.position);
        return distanceTriggerBorder;
    }
    private float getDistancePlayerBorder()
    {
        if (distancePlayerBorder == 0f)
            distancePlayerBorder = Vector2.Distance(playerBorderPos.position, transform.position);

        if (boostIngameManager._sourisOn)
            return Vector2.Distance(posBorderPlayer_souris.position, transform.position);
        else 
            return distancePlayerBorder;
    }
    private float getMinDistanceWithLame(GameObject obstacleLame)
    {
        float min_distance;
        min_distance = Vector2.Distance(
            obstacleLame.GetComponent<CircleCollider2D>().ClosestPoint(transform.position)
            , transform.position);
        float dist;

        foreach (Transform t in obstacleLame.transform)
        {
            if (t.GetComponent<PolygonCollider2D>())
            {
                dist = Vector2.Distance(t.GetComponent<PolygonCollider2D>().ClosestPoint(transform.position), transform.position);
                if (dist < min_distance)
                {
                    min_distance = dist;
                }
            }
        }
        return min_distance;
    }

    private GameObject currentObstacleLameTracked;
    private IEnumerator trackObstacleLame(GameObject obstacleLame)
    {
        float min_distance;

        do
        {
            yield return null;
            min_distance = getMinDistanceWithLame(obstacleLame);
        } while (min_distance > getDistancePlayerBorder() && min_distance < getDistanceTriggerBorder());

        if (ghostOn || min_distance > getDistanceTriggerBorder())
        {
            //Debug.Log("Change trigger to solid");
            obstacleChangeTrigger(obstacleLame, false);
        }
        else
        {
            //Debug.Log("Activate ghost");
            ghostOn = true;
            // activer le ghost
            _ghostBoostManager.ghostIsUsed();
            ghostActivated_resetAllObstaclesEcxeptGhosted(obstacleLame);
            // attendre que l'obstacle sort de trigger
            float min_distance_2;
            do
            {
                yield return null;
                min_distance_2 = getMinDistanceWithLame(obstacleLame);
            } while (min_distance_2 < getDistanceTriggerBorder());

            // on réactive tout les collider
            ghostOn = false;
            forceResetObstacles();
            _ghostBoostManager.ghostEnds();
            gameObject.SetActive(false);
            currentObstacleLameTracked = null;
        }
    }

    private IEnumerator trackObstacle(Collider2D collision)
    {
        // on attend que obstacle active ghost OU sort zone trigger
        float distance;
        do
        {
            yield return null;
            distance = Vector2.Distance(collision.ClosestPoint(transform.position), transform.position);
        } while (distance > getDistancePlayerBorder() && distance < getDistanceTriggerBorder());
        // si sort des limites, on regarde de quelle limite est sortit
        // si ghost déja activé, on ne transforme pas cet obstacle, player peut être va se le prendre
        // /!\ obstacle lame
        //Debug.Log("End track obstacle : " + collision.name);
        if (ghostOn || distance > getDistanceTriggerBorder())
        {
            //Debug.Log("Change trigger to solid");
            obstacleChangeTrigger(collision.gameObject, false);
        }
        else
        {
            //Debug.Log("Activate ghost");
            ghostOn = true;
            // activer le ghost
            _ghostBoostManager.ghostIsUsed();
            ghostActivated_resetAllObstaclesEcxeptGhosted(collision.gameObject);
            // attendre que l'obstacle sort de trigger
            do
            {
                yield return null;
                distance = Vector2.Distance(collision.ClosestPoint(transform.position), transform.position);
            } while (distance < getDistanceTriggerBorder());

            // on réactive tout les collider
            ghostOn = false;
            forceResetObstacles();
            _ghostBoostManager.ghostEnds();
            gameObject.SetActive(false);

        }
    }

    #endregion
}
