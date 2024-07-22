using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum recompense
{
    ARGENT,
    VIE,
    CROWN,
    BOOST,
    GEMME,
    TICKET_ONLINE,
    INFINIS
}

public class recompenseLvl : MonoBehaviour
{
    public recompense type;
    private static float speed = 7f;
    public float posZ = 0f;
    public Transform target;
    private Vector3 goal;

    public appliManager appManager;


    // convertir pos en word

    private Animation myAnim;
    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myAnim.isPlaying)
        {
            if (goal == Vector3.zero)
            {
                goal = Camera.main.ScreenToWorldPoint(target.position);
                goal = new Vector3(goal.x, goal.y, posZ);

                //Debug.Log("Distance avec target : " + Vector3.Distance(transform.position, goal));
                setSpeed();
            }

            if (transform.position != goal)
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(goal.x, goal.y, posZ), Time.deltaTime * speed);
            else // quand arrive a destination
            {
                /*if (TutoManager.tutoOn)
                {
                    //tutorielManager.getInstance().winCoin();
                    Destroy(gameObject);
                }
                else
                {*/
                    if (type == recompense.CROWN)
                    {
                        crownProgBar.getInstance().crownEnterBar();
                    }
                    if(type == recompense.INFINIS)
                    {
                        appliManager.itemInfinisDestroyed(gameObject);
                        appManager.infinisBestIncremented();
                    }
                    //Debug.Log("GM");
                    gameManager.getInstance().giveRecompense(type, true);
                    Destroy(gameObject);
                //}
            }
        }
    }

    public void setSpeed()
    {
        // Arrivera tjr en x seconde a la cible
        float refSpeed = 7f;
        float refDist = 4f;
        // si facteur 2 fois + grand, vitesse 2 fois plus grand
        speed = refSpeed * (Vector3.Distance(transform.position, goal) / refDist);
        speed = speed < 5 ? 5f : speed;
    }

    public void spawn(Vector2 pos, Vector2 t)
    {
        if (t != Vector2.zero)
        {
            goal = Camera.main.ScreenToWorldPoint(t);
            goal = new Vector3(goal.x, goal.y, posZ);
        }
        //StartCoroutine(gameManager.getInstance().winMonnaie(gameObject));
        transform.position = new Vector3(pos.x, pos.y, posZ);
        gameObject.SetActive(true);
    }

    /*IEnumerator waitBeforeMove()
    {
        yield return new WaitForSeconds(0.5f);
    }*/
}
