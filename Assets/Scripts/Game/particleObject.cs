using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleObject : MonoBehaviour
{
    public bool instance; // l'instance est tt le temps active et ne doit pas être supprimé
    ParticleSystem pS;

    private static bool allInstanceNotNull = false;
    // Start is called before the first frame update
    void Start()
    {
        pS = GetComponent<ParticleSystem>();

        if (allInstanceNotNull == false)
        {
            //if (particleD == null)
                particleD = GameObject.Find("deathParticles");
            //if (particleDGhost == null)
                particleDGhost = GameObject.Find("deathParticlesGhost");
            //if (particleB == null)
                particleB = GameObject.Find("bounceParticle");
            //if (particleP == null)
                particleP = GameObject.Find("portalParticles");
            //if (particleD_ghostPlayer == null)
                particleD_ghostPlayer = GameObject.Find("deathParticles_ghostPlayer");

            //if (particleD_ghostOnline == null)
                particleD_ghostOnline = GameObject.Find("particleD_ghostOnline");
            //if (particleD_ghostOnlineDemi == null)
                particleD_ghostOnlineDemi = GameObject.Find("particleD_ghostOnlineDemi");
            allInstanceNotNull = true;
        }

        if (instance)
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!instance && !pS.isPlaying)
            Destroy(gameObject);
    }

    public void loadParticles()
    {
        instance = false;
        GetComponent<ParticleSystem>().Play();
    }

    private static GameObject particleD, particleDGhost, particleB, particleP, 
        particleD_ghostPlayer;

    public static void deathParticle(Vector2 pos, Vector2 target, bool ghost)
    {
        //Debug.Log("Death particles");
        GameObject p = Instantiate(!ghost ? particleD : particleDGhost);
        p.transform.position = pos;
        p.transform.up = target;

        p.SetActive(true);
        p.GetComponent<particleObject>().loadParticles();
    }

    private static GameObject particleD_ghostOnline, particleD_ghostOnlineDemi;

    public static void deatParticlesGhostOnline(bool fullCircle, Vector2[] posInfos)
    {
        GameObject p = Instantiate(fullCircle ? particleD_ghostOnline : particleD_ghostOnlineDemi);

        Vector2 pos = posInfos[0];
        p.transform.position = pos;

        if(!fullCircle)
        {          
            Vector2 target = posInfos[1];
            p.transform.up = target;
        }

        p.SetActive(true);
        p.GetComponent<particleObject>().loadParticles();
    }

    public static void bounceParticle(Vector2 pos, Vector2 target)
    {
        GameObject p = Instantiate(particleB);
        p.transform.position = pos;
        p.transform.up = target;

        p.SetActive(true);
        p.GetComponent<particleObject>().loadParticles(); 
    }

    public static void portalParticle(Vector2 pos, Vector2 target)
    {
        GameObject p = Instantiate(particleP);
        p.transform.position = pos;
        p.transform.up = target;

        p.SetActive(true);
        p.GetComponent<particleObject>().loadParticles();
    }

    public static void playerGhost_deadParticles(Vector2 pos)
    {
        GameObject p = Instantiate(particleD_ghostPlayer);
        p.transform.position = pos;
        //p.transform.up = target;

        p.SetActive(true);
        p.GetComponent<particleObject>().loadParticles();
    }
}
