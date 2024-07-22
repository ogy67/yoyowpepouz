using System.Collections;
using UnityEngine;

public enum typeReward_game
{
    yow,
    gem,
    life
}

public class itemRewardYowGem : MonoBehaviour
{
    public static float factorValues = -1; // valeures par défaut, indique si ratio is set
    // quand un item pop : ++
    // quand un item arrive posFinale : --
    // si arrive posFinale && est dernier, avertir posYowGem de desactiver raycastTarget fond
    private static int nbItemPoped = 0;



    public static float timeBlast = 0.25f, timeStatic = 0.4f, timeStatic_arrivee = 0.1f;
    //public float factorSpeedReduceBlast = 3f;
    // vitesse deplacement pendant blast
    public float percentSpeedDiffMax_blast = 0.4f; // ecart possible entre 2 items : { speedBlast - percentSpeedDiffMax_blast ; speedBlast + percentSpeedDiffMax_blast }
    // vitesse deplacement vers compteur
    public float percentSpeedDiffMax_compteur = 0.25f; 

    private typeReward_game currentTypeR;
    //private bool _giveYow = false;
    private int _nbRecGiven = 0;
    public void pop(Vector2 positionSpawn, Vector2 directionBlast, Transform posFinale,
        int nbRecGiven, typeReward_game typeR, float speedBlast, float facteurDistance, bool ingame)
    {
        currentTypeR = typeR;
        //_giveYow = yow;
        _nbRecGiven = nbRecGiven;
        nbItemPoped++;
        setRewardLook(typeR);
        // on pop a positionSpawn
        transform.position = positionSpawn;

        /*animItem_v2(directionBlast, posFinale,
            ingame ? speedBlast : speedBlast + Random.Range(-(speedBlast * percentSpeedDiffMax_blast), (speedBlast * percentSpeedDiffMax_blast)),
            maxSpeedGoCompteur + Random.Range(-(maxSpeedGoCompteur * percentSpeedDiffMax_compteur), (maxSpeedGoCompteur * percentSpeedDiffMax_compteur)),
            facteurDistance
            );*/
        float speed = getSpeedItemByDistance(positionSpawn, posFinale.position); ;
        StartCoroutine(animItem(directionBlast, posFinale,
            ingame? speedBlast : speedBlast + Random.Range(-(speedBlast * percentSpeedDiffMax_blast), (speedBlast * percentSpeedDiffMax_blast)),
            speed + Random.Range(-(speed * percentSpeedDiffMax_compteur), (speed * percentSpeedDiffMax_compteur)),
            facteurDistance,
            ingame
            )); 
    }

    public static float speedGoCompteur = 1500f;
    // Test pour le moment
    public float minDistanceToChangeSpeed = 360f;
    public float maxDistanceToChangeSpeed = 535f;
    public float maxSpeedOnMaxDistance = 2500f;
    private float getSpeedItemByDistance(Vector2 positionItem, Vector2 positionCompteur)
    {
        float distanceFromCompteur = Vector2.Distance(positionItem, positionCompteur);
        //Debug.Log("Distance itemReward from compteur : " + distanceFromCompteur+" (min:"+minDistanceToChangeSpeed+")");
        if (distanceFromCompteur > minDistanceToChangeSpeed)
        {
            float diffSpeed_minMax = maxSpeedOnMaxDistance - speedGoCompteur;
            // si distance = minDistanceToChangeSpeed, vitesse = normale
            // si distance = maxDistanceToChangeSpeed, vitesse = max

            if (distanceFromCompteur > maxDistanceToChangeSpeed)
                distanceFromCompteur = maxDistanceToChangeSpeed;
            // calcul prog entre 100f -> 500f | 100f = 0% | 500f = 100%
            float prog = (distanceFromCompteur - minDistanceToChangeSpeed) / (maxDistanceToChangeSpeed - minDistanceToChangeSpeed);

            float newSpeed = speedGoCompteur + (diffSpeed_minMax * prog);

            //Debug.Log("New speed item : " + newSpeed);
            return newSpeed;
        }
        else
            return speedGoCompteur;
    }

    public GameObject yowLookTr, gemLookTr, lifeLookTr;
    private void setRewardLook(typeReward_game typeR)
    {
        // on montre gemme ou yow en fonction de _yow
        // on met la couleure de flou en fonction de _yow
        if (typeR == typeReward_game.yow)
            yowLookTr.SetActive(true);
        else if (typeR == typeReward_game.gem)
            gemLookTr.SetActive(true);
        else
            lifeLookTr.SetActive(true);
        /*if (_yow)
            yowLookTr.SetActive(true);
        else
            gemLookTr.SetActive(true);*/
    }

    public Animation myAnim;
    public float timeAnimPopOut = 25/60f;
    // ? bug phase deceleration cash pendant blast ?
    private IEnumerator animItem(Vector2 directionBlast, Transform posFinale, float speedBlast, float speedCompteur, float facteurDistance, bool ingame)
    {
        float currentTime = 0f;
        initScale = transform.localScale;
        transform.localScale = initScale * factorRetrecicementInit;
        int compteurFrame = 0;

        if (currentTypeR == typeReward_game.yow && ingame)
        {
            // cash changer compteur
            StartCoroutine(gameManager.getInstance().giveMoneyArrivee(timeBlast));
        }
            // anim blast pop
        while (currentTime < timeBlast)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + directionBlast * facteurDistance
                , Time.deltaTime * speedBlast * factorValues);
            currentTime += Time.deltaTime;
            transform.localScale = initScale *
                (factorRetrecicementInit + (1 - factorRetrecicementInit) * (currentTime / timeBlast));
            compteurFrame++;
            yield return null;
        }
        transform.localScale = initScale;

        // anim blast go compteur
        // ne plus faire cet anim ingame, remplacer par autre anim
        if(ingame)
        {
            yield return new WaitForSeconds(timeStatic_arrivee);

            myAnim.Play("itemRewardArriveePopOut");
            yield return new WaitForSeconds(timeAnimPopOut);
        }
        else
        {
            // une fois moveBlastFinit on attend 
            yield return new WaitForSeconds(timeStatic);

            initDistanceFromCompteur = Vector2.Distance(transform.position, posFinale.position);
            float currentDistance = Vector2.Distance(transform.position, posFinale.position);

            while (currentDistance > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, posFinale.position,
                    Time.deltaTime * speedCompteur * factorValues);


                // plus se rapproche de compteur, plus retrecit
                currentDistance = Vector2.Distance(transform.position, posFinale.position);
                transform.localScale = getScaleRetreci(initDistanceFromCompteur, currentDistance, initScale);
                yield return null;
            }
        }


        // une fois atteind l'arrivée, on donne la recompense 
        nbItemPoped--;
        if(nbItemPoped == 0)
        {
            popYowGemManager.getInstance().lastItemFinished(currentTypeR);
        }

        if (currentTypeR == typeReward_game.yow)
        {
            // ne pas faire d'anim de compteur
            // faire comme quand ramasse reward infinis
            if (ingame)
            {
                // ne rien faire, est fait + haut
            }
            else
            {
                if (managerNiveau.gameOn)
                {
                    gameManager.getInstance().giveRecompense(recompense.ARGENT, true);
                }
                else
                    gameManager.getInstance().rewardYowGem_giveYow(_nbRecGiven);
            }
        }
        else if (currentTypeR == typeReward_game.gem)
        {
            gemmesManager.getInstance().changeGemmeValue(true, _nbRecGiven);
            sfxGame.getInstance().playSfx_listCoin(typeSfx_coin.coin_getSingle);
        }
        else
        {
            gameManager.getInstance().giveRecompense(recompense.VIE, true);
        }

        Destroy(gameObject);
    }

    public float factorRetrecicementMax = 0.4f, factorRetrecicementInit = 0.3f;
    private Vector2 initScale;
    private float initDistanceFromCompteur;
    private Vector2 getScaleRetreci(float maxDistance, float currentDistance, Vector2 initScale)
    {
        // rapportDistance descend petit a petit 1 -> 0
        float rapportDistance = currentDistance / maxDistance;
        // si rapportDistance = 0.5 : rapportScale = 0.4 + 0.6 * rapportDistance 
        float rapportScale = factorRetrecicementMax + (1 - factorRetrecicementMax) * rapportDistance;
        return initScale * rapportScale;
    }
}
