using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animFond3DGame : MonoBehaviour
{



    public Vector2 offsetY;
    public Transform offsetTr, itemSampleTr;

    public float speedCubeGoGoal = 25f;

    // Start is called before the first frame update
    void Start()
    {
        posUpYMax = posUpMaxTr.localPosition.y;
        buildListItems();
        offsetY = new Vector2(0, offsetTr.position.y - itemSampleTr.position.y);
        currentSpriteFond = cube3DSprite;
    }

    public Transform listLine;
    private List<itemFond3D> listItemsCs = new List<itemFond3D>();
    private void buildListItems()
    {
        foreach(Transform t in listLine)
        {
            foreach (Transform t_2 in t)
                listItemsCs.Add(t_2.GetComponent<itemFond3D>());
        }
    }

    private static animFond3DGame instance;
    public static animFond3DGame getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("animFond3DCanvas V2").GetComponent<animFond3DGame>();
        }
        return instance;
    }

    public GameObject contentGo;
    public void gameOpen()
    {
        setImgFond();
        setColorFond();
        contentGo.SetActive(true);
        StartCoroutine(animOpenEnum());
    }
    public float timeBetweenEachAnim = 0.03f;
    public Animation[] listLineAnim;
    private IEnumerator animOpenEnum()
    {
        foreach (itemFond3D iF in listItemsCs)
        {
            iF.gameObject.SetActive(false); // s'assurer que aucun bloqué en mode ON
        }
        foreach (Animation a in listLineAnim)
        {
            a.Play("openLine3D");
            yield return new WaitForSeconds(timeBetweenEachAnim);
        }
    }

    private IEnumerator closeContentFondLate(float time)
    {
        yield return new WaitForSeconds(time);
        contentGo.SetActive(false);
    }

    public void playerBounce()
    {
        foreach(Animation a in listLineAnim)
        {
            if (a.isPlaying)
                a["bounceLine3D"].time = 0f;
            else
                a.Play("bounceLine3D");
        }
    }

    public void menuPauseOpen()
    {
        StartCoroutine(closeContentFondLate(18/60f));
        playerMoving = false;
        foreach (Animation a in listLineAnim)
        {
            a.Play("pauseOnLine3D");
        }
    }

    public void menuPauseClose()
    {
        contentGo.SetActive(true);
        playerMoving = true;
        foreach (Animation a in listLineAnim)
        {
            a.Play("pauseOffLine3D");
        }
    }

    public void zoomOut()
    {
        StartCoroutine(closeContentFondLate(18 / 60f));
        foreach (Animation a in listLineAnim)
        {
            a.Play("pauseOnLine3D");
        }
    }

    public void zoomIn()
    {
        contentGo.SetActive(true);
        setColorFond();
        foreach (Animation a in listLineAnim)
        {
            a.Play("pauseOffLine3D");
        }
    }


    // appelé quand
    // OK initialisation jeu
    // OK change, mode aventure <-> mode infinis
    // OK change, mode expert <-> mode aventure
    // OK levelUp, aventure
    // levelUp, infinis
    private Color currentColor;
    private Color goalColor;
    public void initializeColorFond(Color c)
    {
        goalColor = c;
        //setColorFond();
    }

    public void changeColorFond3D(Color c)
    {
        goalColor = c;
    }

    private void setColorFond()
    {
        if (goalColor != currentColor)
        {
            foreach (itemFond3D iF in listItemsCs)
            {
                iF.changeColorCube(goalColor);
            }
            currentColor = goalColor;
        }
    }



    public Sprite cube3DSprite, triangle3DSprite;
    private Sprite currentSpriteFond;

    private void setImgFond()
    {
        if (infinisManager.infinis_Mode_On)
        {
            // si le fond est en mode expert, changer en cube3D 
            if (currentSpriteFond == triangle3DSprite)
            {
                currentSpriteFond = cube3DSprite;
                changeItemImg(currentSpriteFond);
            }
        }
        else if (expertMode.modeExpertOn)
        {
            if (currentSpriteFond == cube3DSprite)
            {
                currentSpriteFond = triangle3DSprite;
                changeItemImg(currentSpriteFond);
            }
        }
        else if (currentSpriteFond == triangle3DSprite)
        {
            currentSpriteFond = cube3DSprite;
            changeItemImg(currentSpriteFond);
        }
    }

    private void changeItemImg(Sprite imgItem)
    {
        foreach (itemFond3D iF in listItemsCs)
        {
            iF.changeImgFond(imgItem);
        }
    }


    // ------------------------ ANIMATION DYNAMIQUE

    // en fonction de distance avec posPlayer, faire " monter " les cubes
    // si pos < distanceMin -> monter MAX
    // si pos < distanceMax -> monter en fonction de distance
    public float distanceMin = 3f;
    public float distanceMax = 8f;
    public float maxBriller = 0.4f;
    //public float speedCube = 0.3f;

    public float vitesseOnde = 0.3f;
    public float vitesseItemMoveOnde = 3f;

    public Transform posCenterTr;
    public Transform posUpMaxTr;

    public bool playerMoving = false;

    public void playerPop()
    {
        //if(!TutoManager.tutoOn)
            StartCoroutine(playerPop_enum());  
    }

    //public bool doPreAnimPop = false;
    private IEnumerator playerPop_enum()
    {
        yield return null; // temps que player soit repositionné
        /*if (doPreAnimPop)
        {
            foreach (itemFond3D item in listItemsCs)
            {
                item.initItem_playerPoped();
            }
        }*/
        //yield return new WaitForSeconds(1 / 2f); // regardé dans playerMovment.spawn()
        playerMoving = true;
    }

    public void playerWin()
    {
        //if (!TutoManager.tutoOn)
        //{
            playerMoving = false;
            // anim onde
            foreach (itemFond3D item in listItemsCs)
            {
                item.playerWon();
            }
        //}
    }

    public void playerDied()
    {
        //if (!TutoManager.tutoOn)
            playerDisactivated();
    }

    public void playerDisactivated()
    {
        playerMoving = false;
        // anim onde
        foreach (itemFond3D item in listItemsCs)
        {
            item.playerDisactivated();
        }
    }


    public float posUpYMax;
    public float getPosY_byDistance(float distance)
    {
        if (distance > distanceMax)
            return 0f;
        else
        {
            if (distance > distanceMin)
            {
                //float range = distanceMax - distanceMin;
                //float p = distance - distanceMin;
                //float facteurSize = 1 - (p / range);
                // si prog = 1 -> posUpY max

                return (1 -
                    ((distance - distanceMin) / (distanceMax - distanceMin))
                    )
                    * posUpYMax;
            }
            else
            {
                return posUpYMax;
            }
        }
    }

    public float getBrillerValue_byDistance(float distance)
    {
        if (distance > distanceMax)
            return 0f;
        else
        {
            if (distance > distanceMin)
            {
                //float range = distanceMax - distanceMin;
                //float p = distance - distanceMin;
                //float facteurSize = 1 - (p / range);
                // si prog = 1 -> posUpY max

                return (1 -
                    ((distance - distanceMin) / (distanceMax - distanceMin))
                    )
                    * maxBriller;
            }
            else
            {
                return maxBriller;
            }
        }
    }

    public float getBrillerValue_byHeight(float posY)
    {
        return (posY / posUpYMax) * maxBriller;
    }
}
