using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typePlayerGhost
{
    none,
    bombe, 
    pierreT
}
public class playerGhost : MonoBehaviour
{
    public void pivotPush(Vector2 dir)
    {
        float f = dir.magnitude / releaseVelocity.magnitude;
        releaseVelocity = new Vector2(dir.x / f, dir.y / f);
    }


    public static Vector2 pierreTombale_endPos;
    private Image progBarTime;
    public GameObject progBar_can;
    private GameObject can_instance;
    public static typePlayerGhost typeGhost = typePlayerGhost.none;

    // ghost rebondis partout
    // meurt : previent player
    // gagne prévient player
    public void ghost_spawn(Vector2 posSpawn, Vector2 direction, float time)
    {
        can_instance = Instantiate(progBar_can);
        can_instance.SetActive(true);
        can_instance.transform.GetChild(0).GetComponent<followTarget>().target = transform;
        progBarTime = can_instance.transform.GetChild(0).GetComponent<Image>();

        // player lui envoit le vecteur de bounce, pas de mort
        releaseVelocity = direction;
        //posOnRelease = posSpawn;
        transform.position = posSpawn;
        myRB2D = GetComponent<Rigidbody2D>();
        enterredCheckP = false;
        progBarTime.fillAmount = 1f;
        gameObject.SetActive(true);
        StartCoroutine(countDownBeforeDissapear(time));
    }

    // enregistre la position des ghost quand finissent
    // playerMov utilise la liste pour savoir ou pop medic
    public static List<Vector2> posEndGhost = new List<Vector2>();
    private bool enterredCheckP = false;
    private IEnumerator countDownBeforeDissapear(float t)
    {
        float t_2 = 0f;
        while(!enterredCheckP &&t_2 < t)
        {
            yield return null;
            t_2 += Time.deltaTime;
            progBarTime.fillAmount = 1 - t_2 / t;
        }
        progBarTime.fillAmount = 0;

        if (!enterredCheckP)
        {
            particleObject.playerGhost_deadParticles(transform.position);

            if (typeGhost == typePlayerGhost.pierreT)
                pierreTombale_endPos = transform.position;
            else
            {
                posEndGhost.Add(transform.position);
            }
            Destroy(gameObject);
            Destroy(can_instance);
        }
    }

    public void anotherGhostEnterredCheckP()
    {
        if (!enterredCheckP) // tous appelé par manager
        {
            enterredCheckP = true;
            progBarTime.fillAmount = 0;
            StartCoroutine(slowDown_arrivee());
        }
    }

    public void ghostEnterredEndCheckP()
    {
        enterredCheckP = true;
        progBarTime.fillAmount = 0;
        StartCoroutine(slowDown_arrivee());
    }

    private IEnumerator slowDown_arrivee()
    {
        float t = 0f;
        while (t < 0.5f)
        {
            releaseVelocity *= 0.92f;
            yield return null;
            t += Time.deltaTime;
        }
        particleObject.playerGhost_deadParticles(transform.position);
        Destroy(gameObject);
        Destroy(can_instance);
    }

    //private Vector2 posOnRelease;
    public Vector2 releaseVelocity;
    private Rigidbody2D myRB2D;
    private bool BounceThisFrame = false;

    private void FixedUpdate()
    {
        if (BounceThisFrame && colThisFrame.Count != 0)
        {
            Vector2 newNorme;
            //Vector2 posParticle = lesPosCol[0];
            if (lesPosCol.Count == 2
                && Vector2.Distance(lesPosCol[0], lesPosCol[1]) > distMinToConsiderTwoContact)
            {
                Vector2 centre = lesPosCol[1] - (lesPosCol[1] - lesPosCol[0]) / 2;
                newNorme = playerMovment.getNormTwoCol(lesPosCol[0], lesPosCol[1],
                    centre, transform);
                //StartCoroutine(lockColAfterDoubleCol());
            }
            else
            {
                newNorme = colThisFrame[0];
            }

            releaseVelocity = playerMovment.getReflexion(newNorme.normalized, releaseVelocity);

            sfxGame.getInstance().playSfx_listTuto(typeSfx_tuto.ghostPlayer_bounce);
            //posOnRelease = transform.position;
        }

        Vector3 speed = releaseVelocity * Time.deltaTime * playerMovment.factorTranslate;
        myRB2D.MovePosition(
            transform.position
            + speed);

        BounceThisFrame = false;

        //sourisAlreadyCollideThisFrame = false;
        //BounceThisFrame = false;
        //almostDeadThisFrame = false;
        colThisFrame = new List<Vector2>();
        lesPosCol = new List<Vector2>();
        catchedVector = Vector2.zero;
    }


    // collision avec obstacle normal / rebondis
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(playerMovment.colBehindPlayer(releaseVelocity, collision.contacts[0].point, collision.contacts[0].normal))
        {
            return;
        }
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "ObstacleBounce")
            //|| collision.gameObject.tag == "Player_ghost") // ghost rebondir entre eux
        {
            // rebondir

            computeNorm(collision);
            BounceThisFrame = true;
            //deathValues = new Vector2[] { collision.contacts[0].point, currentNormContact };
        }
    }


    //private float timeContact = 0f;
    private float distMinToConsiderTwoContact = 0.02f; // contact avec pike si inferieur
    private Vector2 catchedVector = Vector2.zero; // met a jour col dans fixedUpdate
    //private bool lockCol = false;

    private List<Vector2> colThisFrame = new List<Vector2>();
    private List<Vector2> lesPosCol = new List<Vector2>();
    //private bool BounceThisFrame = false;
    private Vector2 currentNormContact;
    private Vector3 contactP;

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
                playerMovment.getNormTwoCol(collision.contacts[0].point
                , collision.contacts[1].point
                , milieuVec
                , transform);
            Debug.Log("DOUBLE CONTACT MEME OBJET : " + collision.contacts.Length);

            contactP = milieuVec;

        }

        lesPosCol.Add(contactP);
        colThisFrame.Add(currentNormContact);
    }




    // cas bombe online
    // enregistrer l'id checkP a la création
    // quand passe un checkP, update rank player local

    // 4 ghost , on met a jour rank seulement pour le ghost le plus loin
    public static Dictionary<GameObject, int> idPalier_perGhost = new Dictionary<GameObject, int>();
    private int lapGhost = 0;

    // tout les ghosts s'enregistrent
    public void setModeOnline(int currentPalier)
    {
        idPalier_perGhost.Add(gameObject, currentPalier);
    }

    /*public void ghostPassOnlineCheckP(int newPalier)
    {
        Debug.Log("Ghost palier:" + idPalier_perGhost[gameObject] + ", passe dans palier:" + newPalier);

        // si le ghost a déja un tour de retard et ne passe pas la ligne d'arrivée
        // => ne rien faire du tout
        if (idPalier_perGhost[gameObject] == -1)
        {
            if (newPalier == 0)
                idPalier_perGhost[gameObject] = 0;
            return;
        }

        // était à 0, va en 18 -> 18 - 0 > 1 => arrière
        // était à 1, va en 1 -> 1 - 1 <= 1 => avant
        // était à 1, va en 2 -> 2 - 1 <= 1 => avant
        // était à 0, va en 1 -> 0 - 1 <= 1 => avant
        bool isGoingForward = newPalier - idPalier_perGhost[gameObject] <= 1;
        int newIdPalier = isGoingForward ? newPalier : idPalier_perGhost[gameObject] - 1;

        // si le ghost va en arrière
        if (!isGoingForward)
        {
            //  passe la ligne d'arrivée
            if (idPalier_perGhost[gameObject] == 0)
            {
                // on fait passer le nombre de lap en dessous de 0
                // => ne pas augmenter nombre de lap par erreur si repasse sur ligne arrivée
                idPalier_perGhost[gameObject] = -1;
            }
            return;
        }
        else
        {
            idPalier_perGhost[gameObject] = newPalier;
        }

        // maintenant , on regarde si c'est le ghost qui a la plus grande avance
        bool hasTheHighestValue = true;
        foreach(KeyValuePair<GameObject, int> kV in idPalier_perGhost)
        {
            if(kV.Key != gameObject && kV.Value >= newPalier)
            {
                hasTheHighestValue = false;
                break;
            }
        }

        // si hasTheHighestValue, mettre à jour ranking de localP
        if (hasTheHighestValue)
        {
            Debug.Log("Has the highest value, update ranking");

            gameOnlineManager.getInstance().localPlayerGoThrewPalier(newPalier);
        }
    }*/
}
