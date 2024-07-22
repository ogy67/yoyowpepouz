using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// posé sur les checkpoints
public class playerDetector : MonoBehaviour
{
    public static Transform checkpoint = null;
    public Animation activateCheckP_anim;
    private static Transform player_tr;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger enter : " + collision.tag);
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Player enter checkPoint");
            checkpoint = transform;//.GetChild(0);
            StartCoroutine(activateCheckP(collision.transform));
            if (player_tr == null)
            {
                player_tr = boostIngameManager.getInstance().playerMov.transform;
            }
        }
    }

    private void checkPPlayAnim(string anim)
    {
        if (activateCheckP_anim.isPlaying)
            activateCheckP_anim.Stop();
        activateCheckP_anim.Play(anim);
    }

    private bool checkPActivated = false;
    private IEnumerator activateCheckP(Transform player)
    {
        //bool activated = false;     
        // on regarde si distance bonne
        while (checkpoint != null)//playerMovment.center == transform) // checkpoint != null
        {
            // Si pas activé et player < distance max : activer
            if (!checkPActivated && playerIsInDetector(player.position))
            {
                checkPActivated = true;
                checkPPlayAnim("checkPointCanBeHang");
               // Debug.Log("CheckP ON");
                yield return new WaitForSeconds(1 / 6f);
            }
            // Sinon si activé et > distance max : desactiver
            else if(checkPActivated
                && !playerIsInDetector(player.position))
            {
                checkPActivated = false;
                checkPPlayAnim("checkPointLeave");
                //Debug.Log("CheckP OFF");
                yield return new WaitForSeconds(1 / 6f);
            }
            yield return null;
        }
        if (checkPActivated)
        {
            checkPPlayAnim("checkPointLeave");
            checkPActivated = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && playerMovment.center != playerMovment.initCenter)
            //&& playerMovment.center == transform) // parfois, exit trigger après mort
        {
            checkpoint = null;
            playerMovment.center = null;
            //Debug.Log("Player exit checkP " + gameObject.name);
        }
    }


    private bool min_max_set = false;
    private float max_distance, min_distance, recalibrate_distance;
    private void set_max_min()
    {
        max_distance = Vector2.Distance(transform.position, posMax.position);
        min_distance = Vector2.Distance(transform.position, posMin.position);
        recalibrate_distance = Vector2.Distance(transform.position, posCalibrated.position);
        min_max_set = true;
    }
    private bool playerIsInDetector(Vector2 posPlayer)
    {
        float p_distance = Vector2.Distance(posPlayer, transform.position);
        if (!min_max_set)
            set_max_min();
        //bool aboveMin = p_distance >= min_distance;
        bool underMax = p_distance <= max_distance;

        /* si check fermer checkp
        // on le ferme si player dépasse les limites
        if (!test_enter && underMax) 
        {
            return true;
        }*/

        // si player est entre max et min, peut hang
        if (underMax)
        {
            return true;
        }
        // sinon, si l'ancre peut être utilisé
        else if (boostIngameManager._ancreOn
            &&
            p_distance / Vector2.Distance(transform.position, posMax.position) <= boostIngameManager._factorAncre)
        {
            return true;
        }
        return false;
    }

    public Transform posMin,posMax, posCalibrated;

    // appelé pour savoir si la fusee peut être activé
    // savoir si player peut s'accrocher : non => fusee peut être activé
    public bool playerCanHang_fusee(Vector2 posPlayer)
    {
        if (_lockCheckP)
            return false;
        float p_distance = Vector2.Distance(posPlayer, transform.position);

        if (!min_max_set)
            set_max_min();
        bool aboveMin = p_distance >= min_distance;
        bool underMax = p_distance <= max_distance;

        // si player est en dessous de max, peut hang ( pour fusee )
        if (underMax)
        {
            return true;
        }
        // sinon, si l'ancre peut être utilisé
        else if (boostIngameManager._ancreOn && aboveMin
            &&
            p_distance / Vector2.Distance(transform.position, posMax.position) <= boostIngameManager._factorAncre)
        {
            return true;
        }
        return false;
    }

    public bool playerCanHang(Vector2 posPlayer)
    {
        if (_lockCheckP)
            return false;

        float p_distance = Vector2.Distance(posPlayer, transform.position);

        if (!min_max_set)
            set_max_min();
        bool aboveMin = p_distance >= min_distance;
        bool underMax = p_distance <= max_distance;

        //Debug.Log("try hang "+gameObject.name);
        // si player est entre max et min, peut hang
        if (aboveMin && underMax)
        {
            StartCoroutine(ancre_rapprocherPlayer(player_tr, 0.3f));
            //Debug.Log("succed hang");
            return true;
        }
        // sinon, si l'ancre peut être utilisé
        else if (
            !(expertMode.modeExpertOn && !infinisManager.infinis_Mode_On)
            && boostIngameManager._ancreOn && aboveMin
            && p_distance / Vector2.Distance(transform.position, posMax.position) <= boostIngameManager._factorAncre)
        {
            // activer l'ancre
            boostIngameManager.getInstance().ancreUsed();
            StartCoroutine(ancre_rapprocherPlayer(player_tr, 0.6f));
            return true;
        }
        return false;
    }

    private IEnumerator ancre_rapprocherPlayer(Transform player, float temps_rapprocher)
    {
        //Debug.Log("Ancre rapproche " + player.name);
        // on rapproche player jusqu'à distance max
        float t = 0f;
       
        float init_player_distance = Vector2.Distance(player.position, transform.position);
        float distance_to_sub = init_player_distance - recalibrate_distance;
        // recalibrate > player distance si player hang trop près du centre : le sortir du centre

        Vector2 newPos;
        Vector2 vecteur_player_checkp;

        // playerMovment.center != null : si player hang-release en 1 clic sur checkP
        while (t < temps_rapprocher && playerMovment.finish == false
            && playerMovment.center != null)
        {
            yield return null;
            t += Time.deltaTime;

            vecteur_player_checkp = player.position - transform.position;
            float rapport = (t / temps_rapprocher);
            rapport = rapport > 1 ? 1 : rapport;

            float next_distance = init_player_distance - distance_to_sub * rapport;
            float current_distance = Vector2.Distance(player.position, transform.position);
            float rapport_dimminution = current_distance / next_distance; // next peut > currdist
            newPos = (Vector2)transform.position + vecteur_player_checkp / rapport_dimminution;
            player.position = newPos;
        }
    }


    public void playerPopOnline(Transform playerTr)
    {
        checkpoint = transform;
        checkPActivated = true;
        GetComponent<Animation>().Play("checkpoint");
        checkPPlayAnim("checkPointCanBeHang");
        StartCoroutine(activateCheckP(playerTr));
    }



    public bool _lockCheckP = false;
    //private static float time

    public void lockCP() // eviter de re-hang
    {
        StartCoroutine(lockCheckP());
    }
    private IEnumerator lockCheckP()
    {
        _lockCheckP = true;
        Vector3 dirPlayer = playerMovment.releaseVelocity;
        float t = 0f;
        // on attend soit que timer out, soit que player change direction
        while(t < 0.5f && playerMovment.releaseVelocity == dirPlayer)
        {
            yield return null;
            t += Time.deltaTime;
        }
        _lockCheckP = false;
    }
}
