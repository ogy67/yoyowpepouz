using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemFond3D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
            initialize();
    }

    private bool initialized = false;
    private void initialize()
    {
        imgCubeTr = transform.GetChild(0);
        brillerImg = imgCubeTr.GetChild(0).GetComponent<Image>();
        cubeImg = imgCubeTr.GetComponent<Image>();
    }

    public animFond3DGame manager3D;
    public Transform playerTr;

    private Image brillerImg;
    private Transform imgCubeTr;
    private Image cubeImg;

    private Vector2 curGoalPosY;
    public float curDistanceWithPlayer;
    public Vector2 posItem;
    private void FixedUpdate()
    {
        if (manager3D.playerMoving)
        {
            // regarder à quelle position monter le cube
            float distanceWithPlayer = Vector2.Distance(
                (Vector2)transform.position + manager3D.offsetY,//(Vector2)Camera.main.ScreenToWorldPoint(transform.position), 
                (Vector2)playerTr.position);

            // si distance > max et que pos = 0, rien faire
            if (!(distanceWithPlayer >= manager3D.distanceMax && imgCubeTr.localPosition.y == 0))
            {
                posItem = transform.position;
                curDistanceWithPlayer = distanceWithPlayer;

                curGoalPosY = new Vector2(0, manager3D.getPosY_byDistance(distanceWithPlayer));
                imgCubeTr.localPosition = Vector2.MoveTowards(imgCubeTr.localPosition, curGoalPosY, Time.fixedDeltaTime * manager3D.speedCubeGoGoal);
                brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byHeight(imgCubeTr.localPosition.y));

                //imgCubeTr.localPosition = new Vector2(0, manager3D.getPosY_byDistance(distanceWithPlayer));
                //brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byDistance(distanceWithPlayer));
            }
        }
    }

    private void OnEnable()
    {
        if (!initialized)
            initialize();
        brillerImg.color = new Color(1, 1, 1, 0);
        imgCubeTr.localPosition = new Vector2(0, 0);
    }

    public void initItem_playerPoped()
    {
        // lance animFond
        StartCoroutine(animPlayerPop());
    }

    private IEnumerator animPlayerPop()
    {
        float posYGoal = manager3D.getPosY_byDistance(
            Vector2.Distance((Vector2)transform.position + manager3D.offsetY, playerTr.position));
        if (posYGoal > 0)
        {
            float initT = Time.time;
            while (imgCubeTr.localPosition.y < posYGoal
                && Time.time - initT < 0.5f) // tps player pop ( playerMovment.spawn() )
            {
                float newY;
                newY = imgCubeTr.localPosition.y + manager3D.vitesseItemMoveOnde * Time.deltaTime;
                imgCubeTr.localPosition = new Vector2(0, newY);
                brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byHeight(newY));
                /*Vector2.MoveTowards(
                    imgCubeTr.localPosition
                    , new Vector2(0, manager3D.posUpYMax)
                    , Time.deltaTime * manager3D.vitesseItemMoveOnde);*/
                yield return null;
            }
        }
    }



    public void playerWon()
    {
        // on lance l'anim "onde"

        // en fonction de distance, l'anim se lance + ou moin tard

        // si pos Y de l'item est maxUp -> cash redescendre
        // sinon -> goMaxUp PUIS redescendre

        StartCoroutine(animOndeWin());
    }

    private IEnumerator animOndeWin()
    {
        //Vector2 p = Camera.main.ScreenToWorldPoint(transform.position);
        float distanceWithPlayer = Vector2.Distance((Vector2)transform.position + manager3D.offsetY, playerTr.position);


        // en fonction de distance, lancer routine avec timer
        yield return new WaitForSeconds(distanceWithPlayer * manager3D.vitesseOnde);

        float newY;

        if(distanceWithPlayer > manager3D.distanceMin)
        {
            //Debug.Log("Distance with player : " + distanceWithPlayer);
            // on anim up
            while (imgCubeTr.localPosition.y < manager3D.posUpYMax)
            {
                newY = imgCubeTr.localPosition.y + manager3D.vitesseItemMoveOnde * Time.deltaTime;
                imgCubeTr.localPosition = new Vector2(0, newY);
                brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byHeight(newY));
                /*Vector2.MoveTowards(
                    imgCubeTr.localPosition
                    , new Vector2(0, manager3D.posUpYMax)
                    , Time.deltaTime * manager3D.vitesseItemMoveOnde);*/
                yield return null;
            }
        }
        // ensuite, on anim down
        while (imgCubeTr.localPosition.y > 0)
        {
            newY = imgCubeTr.localPosition.y - manager3D.vitesseItemMoveOnde * Time.deltaTime;
            newY = newY < 0 ? 0 : newY;
            imgCubeTr.localPosition = new Vector2(0, newY);
            brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byHeight(newY));
            yield return null;
        }
    }


    public void playerDisactivated()
    {
        if(imgCubeTr.localPosition.y > 0)
            StartCoroutine(playerDisactivated_enum());
    }

    private IEnumerator playerDisactivated_enum()
    {
        float newY;
        while (imgCubeTr.localPosition.y > 0)
        {
            newY = imgCubeTr.localPosition.y - manager3D.vitesseItemMoveOnde * Time.deltaTime;
            newY = newY < 0 ? 0 : newY;
            imgCubeTr.localPosition = new Vector2(0, newY);
            brillerImg.color = new Color(1, 1, 1, manager3D.getBrillerValue_byHeight(newY));
            yield return null;
        }
    }




    public void changeColorCube(Color c)
    {
        if (!initialized)
            initialize();
        cubeImg.color = new Color(c.r, c.g, c.b, cubeImg.color.a);
    }



    public void changeImgFond(Sprite sp)
    {
        if (!initialized)
            initialize();
        brillerImg.sprite = sp;
        cubeImg.sprite = sp;
    }
}
