using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBoostManager : MonoBehaviour
{
    private static GhostBoostManager instance;
    public static GhostBoostManager getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.Find("playerObject").GetComponent<GhostBoostManager>();
        }
        return instance; 
    }

    public GameObject _ghostDetectorGo;
    public ghostDetector _ghostDetector;
    public void ghostCanBeUsed()
    {
        //Debug.Log("ghostCanBeUsed() : activate ghostDetector");
        // on active le detecteur
        _ghostDetectorGo.SetActive(true);
    }

    public void bouclierUsed_stopGhost(float timeBombe)
    {
        if (_ghostDetectorGo.activeSelf)
        {
            StartCoroutine(disableGhost_bouclier(timeBombe));
        }
    }

    private bool reloadGhostAfterBombe;
    private IEnumerator disableGhost_bouclier(float timeBombe)
    {
        //Debug.Log("disableGhost_bouclier() : desactivate ghostDetector");
        gameEnd(false); // reinitialise ghost
        reloadGhostAfterBombe = true;
        yield return new WaitForSeconds(timeBombe);
        if (reloadGhostAfterBombe)
        {
            _ghostDetector.gameObject.SetActive(true);
            //Debug.Log("disableGhost_bouclier() : re-activate ghostDetector");
        }
    }

    public static bool _ghostPhaseActivation = false;
    public void ghostIsUsed()
    {
        // appelé quand ghost a detecté obstacle dans zone player
        // active le boost
        boostIngameManager.getInstance().playerUse_ghost();
        _ghostPhaseActivation = true;
    }

    public void ghostEnds()
    {
        // appelé quand obstacle sort zone border player
        _ghostPhaseActivation = false;
        // ghostDetector se desactive lui même
    }

    // appelé quand player stop()
    public void gameEnd(bool reloadGhostWhenPlayerActivated)
    {
        //Debug.Log("<color=red> Game end appelé, reload ? </color>"+reloadGhostWhenPlayerActivated);
        if (!reloadGhostWhenPlayerActivated)
            _reloadGhostWhenPlayerActivated = false;

        if (!_ghostDetectorGo.activeSelf)
        {
            _reloadGhostWhenPlayerActivated = false;
            return;
        }

        /*if (reloadGhostWhenPlayerActivated)
            Debug.Log("Will be reload");*/

        // dans le cas ou : ghost activable && utilise bouclier -> arrivée avant endBouclier
        // on desactive la reactivation
        if (reloadGhostAfterBombe)
            reloadGhostAfterBombe = false;

        // si ghost activé ingame
        // on reactive les collider de tout les obstacles

        // si ghost en train d'être activé 
        // ou alors 
        if (_ghostDetector.ghostOn)
        {
            ghostEnds();
            _ghostDetector.forceResetObstacles(); // arrête totalement le ghost ( etait activé )
            _reloadGhostWhenPlayerActivated = false;
        }
        else
        {
            //Debug.Log("Game end, ghost detector will be reopen");
            _ghostDetector.forceResetObstacles(); // desactive juste les obstacle triggered
            _ghostDetectorGo.SetActive(false);
            _reloadGhostWhenPlayerActivated = true;
        }
        _ghostPhaseActivation = false; // si l'était


        //_reloadGhostWhenPlayerActivated = reloadGhostWhenPlayerActivated;
    }

    private bool _reloadGhostWhenPlayerActivated = false;

    // quand ghost peut être utilisé, meurt, desactive detecteur, comment le réactiver ?
    public void playerActivated()
    {
        if (_reloadGhostWhenPlayerActivated)
        {
            // if else resoud : ghost full, quitte jeu, utilise autre boost, ghost n'est plus utilisé mais repop
            if (boostIngameManager.getInstance().canUseBoost(typeBoostGame.ghost))
            {
                _ghostDetectorGo.SetActive(true);
            }
            else
                _reloadGhostWhenPlayerActivated = false;
        }
    }
}
