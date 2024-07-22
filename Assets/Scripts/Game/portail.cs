using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portail : MonoBehaviour
{
    public bool inverseTeleport; // inverse l'angle par rapport a la normale du portail
    public bool inverseX; // inverse la position sur x par rapport au centre du portail
    public bool inverseSide; // inverse le coté d'apparition
    public bool inverseXSide; // inverse le coté d'apparition sur horizontalement
    public GameObject otherPortal;
    public GameObject player;
    public bool hasTranzited = false;

    public static bool isTeleporting = false;
    public static bool isTeleportingLong = false;

    private static bool ghostTeleporting = false;

    private IEnumerator cleanGhostTrail(TrailRenderer tR)
    {
        tR.enabled = false;
        yield return new WaitForSeconds(0.1f);

        if (tR != null)
        {
            tR.Clear();
            tR.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            portalsInTriggerWithPlayer--;
        }
    }

    public static int portalsInTriggerWithPlayer = 0;
    private static int nbTeleport = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player_ghost")
        {
            if (!ghostTeleporting)
            {
                collision.transform.position = getPointSpawn(collision.transform.position, false);
                ghostTeleporting = true;
                playerGhost _playerGhost = collision.transform.GetComponent<playerGhost>();

                StartCoroutine(cleanGhostTrail(collision.transform.GetChild(0).GetComponent<TrailRenderer>()));
                particleObject.portalParticle(
                collision.transform.position
                , _playerGhost.releaseVelocity);
            }
            else
            {
                ghostTeleporting = false;
            }
            return;
        }

        if(collision.tag == "Player")
        {
            //Debug.Log(Time.time+ "<color=green>Player triggers portal </color>"+gameObject.name);
            portalsInTriggerWithPlayer++;

            if (!isTeleporting // quand 2 portails sont collés
           && playerMovment.portalIsInVisor
           && !otherPortal.GetComponent<portail>().hasTranzited)
            {
                nbTeleport++;
                //Debug.Log("<color=" + (nbTeleport % 2 == 1 ? "red>" : "blue>") + "TELEPORTE</color>");
                defiManager.getInstance().defiIsIncremented(typeDefi.portail, 1);
                gameManager.getInstance().eventIngame(typeEffectColor.portal);

                isTeleporting = true;
                isTeleportingLong = true;

                sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_portal);
                //sfxManager.getInstance().playSound_ingame(typeSound_ingame.portal);
                hasTranzited = true;

                contentTrailGame.getInstance().playerPassThrewPortal();
                /*GameObject playerTr = player.transform.GetChild(0).gameObject; // trail renderer
                playerTr.transform.parent = null;
                GameObject o = Instantiate(playerTr);
                StartCoroutine(copyTrail(playerTr));*/

                player.transform.position = posSpawn_player;
                playerMovment.posOnRelease = posSpawn_player;

                if (inverseSide)
                {
                    Vector2 v = playerMovment.releaseVelocity;
                    playerMovment.releaseVelocity = inverseXSide ? new Vector2(-v.x, v.y) : new Vector2(v.x, -v.y);
                }

                animFlash.animateFlash(playerMovment.releaseVelocity, typeFlash.portal);
                //ghostManager.registerGhostEvent(ghostEvent.portalOut, player.transform);
                //StartCoroutine(player.GetComponent<playerMovment>().stopLine());

                particleObject.portalParticle(
                    player.transform.position
                    , playerMovment.releaseVelocity);


                /*o.transform.parent = player.transform;
                o.transform.SetSiblingIndex(0);
                o.transform.localPosition = new Vector3(0, 0, 0);*/

                StartCoroutine(unTeleporte(/*posSpawn, (Vector2)endPos[1], collision.GetComponent<playerMovment>().cordeViseeGo*/));
            }
        }
        // RaycastHit hit;
       
    }

    //private GameObject cordePortal; // le met a jour pendant que player teleport ( s'en occupe a sa place )

    #region Essaie teleport sans coroutine
    // valeures utilisées par portail
    // portail est lock tant que 
    // 1 - player n'a pas exit portail 1
    // 2 - player n'est pas entré dans portail 2

    // est mit à true quand player entre dans un portail et que n'attendait pas other
    // est mit à false quand player entre dans un portail et attendait other
    //private static bool waitingOtherPortal_ToTriggerPlayer = false;
    //private static GameObject currentPairGo_needed;

    /*    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player_ghost")
        {
            if (!ghostTeleporting)
            {
                collision.transform.position = getPointSpawn(collision.transform.position);
                ghostTeleporting = true;
                playerGhost _playerGhost = collision.transform.GetComponent<playerGhost>();

                StartCoroutine(cleanGhostTrail(collision.transform.GetChild(0).GetComponent<TrailRenderer>()));
                particleObject.portalParticle(
                collision.transform.position
                , _playerGhost.releaseVelocity);
            }
            else
            {
                ghostTeleporting = false;
            }
            return;
        }

        if (collision.tag == "Player")
        {
            portalsInTriggerWithPlayer++;

            Debug.Log(Time.time + "<color=green>Player triggers portal </color>" + gameObject.name);

            if (!waitingOtherPortal_ToTriggerPlayer && playerMovment.portalIsInVisor)
            {
                waitingOtherPortal_ToTriggerPlayer = true;
                currentPairGo_needed = otherPortal;
                // on teleporte

                nbTeleport++;
                Debug.Log("<color=" + (nbTeleport % 2 == 1 ? "red>" : "blue>") + "TELEPORTE</color>");
                defiManager.getInstance().defiIsIncremented(typeDefi.portail, 1);

                isTeleporting = true;
                sfxManager.getInstance().playSound_ingame(typeSound_ingame.portal);
                hasTranzited = true;

                GameObject playerTr = player.transform.GetChild(0).gameObject; // trail renderer
                playerTr.transform.parent = null;
                GameObject o = Instantiate(playerTr);
                StartCoroutine(copyTrail(playerTr));

                player.transform.position = posSpawn;
                playerMovment.posOnRelease = posSpawn;

                if (inverseSide)
                {
                    Vector2 v = playerMovment.releaseVelocity;
                    playerMovment.releaseVelocity = inverseXSide ? new Vector2(-v.x, v.y) : new Vector2(v.x, -v.y);
                }
                ghostManager.registerGhostEvent(ghostEvent.portalOut, player.transform);
                //StartCoroutine(player.GetComponent<playerMovment>().stopLine());

                particleObject.portalParticle(
                    player.transform.position
                    , playerMovment.releaseVelocity);


                o.transform.parent = player.transform;
                o.transform.SetSiblingIndex(0);
                o.transform.localPosition = new Vector3(0, 0, 0);

                StartCoroutine(unTeleporte());
            }
            else if (waitingOtherPortal_ToTriggerPlayer && gameObject == currentPairGo_needed)
            {
                // un unteleport
                waitingOtherPortal_ToTriggerPlayer = false;
                currentPairGo_needed = null;
            }
        }
    }*/
    #endregion

    // valeures utilisées par player
    IEnumerator unTeleporte()
    {
        yield return new WaitForSeconds(0.05f);
        hasTranzited = false;
        isTeleporting = false;
        yield return new WaitForSeconds(0.1f);
        isTeleportingLong = false;
    }


    Vector2 posSpawn_player;
    public Vector2 getPointSpawn(Vector2 pointCol, bool player) // afficher le viseur à travers le portail
    {
        Vector2 distVect = (pointCol - (Vector2)transform.position);

        // INVERSER LA POSITION DE SPAWN SUR LA TRANCHE
        if (inverseX) // 180 deg par rapport au centre du portail ( mirroir )
        {
            distVect.x = -distVect.x;
            distVect.y = -distVect.y;
        }

        Vector2 ret = (Vector2)otherPortal.transform.position + distVect;
        if (player)
        {
            posSpawn_player = ret;
        }

        return ret;
    }
    /*IEnumerator copyTrail(GameObject dest)
    {
        float t = 0;
        while(t < 2f && player.activeSelf)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            t += Time.deltaTime;
        }
        Destroy(dest);
    }*/
}
