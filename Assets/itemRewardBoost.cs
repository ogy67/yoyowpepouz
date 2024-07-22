using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct rewardBoost_typeByGameObj
{
    public typeBoostGame type;
    public GameObject itemAssociated;
}
public class itemRewardBoost : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public popYowGemManager popCurrencyManager;
    public rewardBoost_typeByGameObj[] infoItem;
    private typeBoostGame currentTypeBoost = typeBoostGame.bouclier;

    public static float factorValues; // en fonction de taille ecran en pixel ( tt ecran mm speed )
                                      //public float speedFactor = 2f;

    private Vector2 initPosOnSpawn;
    private int idAnim;
    public void popItem(typeBoostGame type, Transform posSpawn, float speedBlast, int _idAnim)
    {
        idAnim = _idAnim;
        initPosOnSpawn = posSpawn.position;

        // on active la bonne imgAssocie
        getItemGo_byType(currentTypeBoost).gameObject.SetActive(false);
        getItemGo_byType(type).SetActive(true);
        currentTypeBoost = type;

        transform.position = posSpawn.position;
        Vector2 randomDir = popCurrencyManager.getRandomBlastDirection();
        directionIsInBoundaries(randomDir);
        while (!directionIsInBoundaries(randomDir))
            randomDir = popCurrencyManager.getRandomBlastDirection();

        float facteurDistance = popCurrencyManager.itemMenu_facteurDistanceBlast;
        //float speedGoCompteur = itemRewardYowGem.speedGoCompteur;

        StartCoroutine(animItemBlast(randomDir, speedBlast, facteurDistance));
    }

    public Transform posTop, posCenter, posLeft, posRight;

    private bool directionIsInBoundaries(Vector2 dir)
    {
        bool intersectLeft = lineSegmentsIntersect(posTop.position, (Vector2)posCenter.position + dir, posCenter.position, posLeft.position);
            //lineIntersect(posTop.position, (Vector2)posCenter.position + dir, posCenter.position, posLeft.position);
        bool intersectRight = lineSegmentsIntersect(posTop.position, (Vector2)posCenter.position + dir, posCenter.position, posRight.position);
        //lineIntersect(posTop.position, (Vector2)posCenter.position + dir, posCenter.position, posRight.position);
        //Debug.Log("Direction (" + dir + "), in zonePop ? left:" + intersectLeft+", right :"+intersectRight);
        return !intersectLeft && !intersectRight;
    }

    public static bool lineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
    {
        return (
            ((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x))
            != 
            (
            (lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) 
            && ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x))
            != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x))); }
    public bool lineIntersect(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
    {
        float cross = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);
        return cross != 0;

        /*    if (tmp == 0)
    {
        // No solution!
        found = false;
        return Vector2.zero;
    }
 
    float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
 
    found = true;
 
    return new Vector2(
        B1.x + (B2.x - B1.x) * mu,
        B1.y + (B2.y - B1.y) * mu
    );*/
    }

    private GameObject getItemGo_byType(typeBoostGame type)
    {
        int childIndexItem = 0;
        while (infoItem[childIndexItem].type != type) childIndexItem++;
        return infoItem[childIndexItem].itemAssociated;
    }

    public rewardBoostManager rewardBManager;
    public float timeItemStaticBlast = 0.6f;
    public float maxTimeWaitInit = 0.3f;
    public float factorSpeed = 2f;
    public float speedBlastMin = 0.7f;

    public float timeBlast_pop = 0.4f;
    public float timeBlast_unpop = 0.8f;
    private IEnumerator animItemBlast(Vector2 directionBlast, float speedBlast
        , float facteurDistance)
    {
        Vector2 goalScale = transform.localScale;
        transform.localScale = new Vector2(0, 0);

        // on attend un temps aléatoire
        int rand = Random.Range(0, 1000);
        yield return new WaitForSeconds((rand / 1000f) * maxTimeWaitInit);

        // on prend une vitesse aleatoire
        float speedBlast_2 = speedBlast -
            ((1 - speedBlastMin) * (rand / 1000f)) * speedBlast;


        // on déplace item sur direction pendant x sec
        // float timeBlast = itemRewardYowGem.timeBlast;
        float timer = Time.time;
        do
        {
            yield return null;
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + directionBlast * facteurDistance
                , Time.deltaTime * speedBlast_2 * factorValues * factorSpeed);
            transform.localScale = goalScale * getPercentage(timer, timer + timeBlast_pop, Time.time);
                //(Time.time / (timer + timeBlast));
        } while (Time.time < timer + timeBlast_pop);


        // on attend un peu
        yield return new WaitForSeconds(timeItemStaticBlast);


        // go target ( pos spawn )
        timer = Time.time;
        Vector2 scale = transform.localScale;
        do
        {
            yield return null;
            transform.position = Vector2.MoveTowards(transform.position, initPosOnSpawn
                , Time.deltaTime * speedBlast_2 * factorValues * factorSpeed);
            transform.localScale = scale * (1 - getPercentage(timer, timer + timeBlast_unpop, Time.time));// (1 - (Time.time / (timer + timeBlast)));
        } while (Time.time < timer + timeBlast_unpop);

        rewardBManager.itemTryAnimSfx(idAnim);

        // destruction de l'item
        Destroy(gameObject);
    }

    public static float getPercentage(float min, float max, float current)
    {
        float retour = (current - min) / (max - min);
        return retour > 1 ? 1 : retour;
    }
}
