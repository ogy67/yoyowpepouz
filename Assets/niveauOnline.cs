using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class niveauOnline : MonoBehaviour
{
    public int idLevelOnline;
    public int nbZoneTrigger;

    public Transform initCenter;
    public Transform initPos;

    public Animation myAnim;


    public List<Transform> obstaclesToPhase;
    private Dictionary<Transform, obstacleInfo> lesObstacleInfos = new Dictionary<Transform, obstacleInfo>();
    /*
     * class obstacleInfo
     * void setAllObstacles()
    */
    #region gestion du phasage des animation d'obstacle entre les joueurs
    internal class obstacleInfo
    {
        internal Vector3 posObstacle;
        internal Vector3 rotationObstacle;
        internal Transform obstacleTr;

        internal obstacleInfo(Vector3 position, Vector3 rotation, Transform t)
        {
            posObstacle = position;
            rotationObstacle = rotation;
            obstacleTr = t;
        }
    }
    private void setAllObstacles()
    {
        // si on a pas encore créer la liste des états initials, on la créer
        if(lesObstacleInfos.Count == 0)
        {
            foreach(Transform t in obstaclesToPhase)
            {
                lesObstacleInfos.Add(t, new obstacleInfo(t.localPosition, t.localRotation.eulerAngles, t));
            }
        }
        // sinon, on initialise les obstacle
        else
        {
            obstacleInfo info;
            foreach (Transform t in obstaclesToPhase)
            {
                if(t.name == "toMove") // obstacle translate
                {
                    // spécial pour move translate, changer le target initial
                    t.parent.GetComponent<obstacleTranslate>().initializeOnline();
                }

                info = lesObstacleInfos[t];
                t.localPosition = info.posObstacle;
                t.eulerAngles = info.rotationObstacle;
            }
        }
    }
    #endregion


    // tout les obstacles doivent être cadencés en même temps chez tout les joueurs
    // tout les joueurs executent la fonction d'ouverture des obstacles en même temps

    // appelé avant que le menu online se ferme pour laisser place au niveau
    public void openLevel()
    {
        // animation ouverture
        // atente de x seconde
        myAnim.Play("niveauComeCenter");

        // si master, appeler RPC anim ouverture obstacle
        // => appelera initializeObstacles()
    }

    // appelé quand le niveau se ferme définitivement
    public void closeLevel()
    {
        myAnim.Play("niveauOutCenter");
        // on referme les obstacles
        obstaclesAnim.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(closeLate());
    }

    private IEnumerator closeLate()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    public void zoomLevel(bool gameStart)
    {
        if (gameStart)
            myAnim.Play("niveauGame");
        else
            myAnim.Play("niveauGameEnd");
    }


    public Animation obstaclesAnim;

    // appelé par un RPC , tout les obstacles se mettent à jour en même temps chez tout les joueurs
    public void initializeObstacles()
    {
        // initialisation des obstacles
        setAllObstacles();

        // ouverture des obstacles
        // pas myAnim => si Master en avance, fait des bugs d'anims
        obstaclesAnim.Play("niveauOnlinePopObstacles");
    }

}
