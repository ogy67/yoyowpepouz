using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chechpointEnd : MonoBehaviour
{
    public GameObject monnaieGo;
    public GameObject vieGo;
    private GameObject winParticles;
    // Start is called before the first frame update
    void Start()
    {
        winParticles = GameObject.Find("winParticles");
    }

    public static bool win = false;
    /*private IEnumerator waitWin()
    {
        //win = true;
        //float t = infinisManager.infinis_Mode_On ? 10f : 6f; // 10f , si anim level up
        yield return new WaitForSeconds(6f);
        win = false;
    }*/

    // si mode infinis, trigger arrivée désactivé jusqu'à disable object
    private void OnDisable()
    {
        if (infinisManager.infinis_Mode_On)
        {
            win = false;
            //Debug.Log("Reactive trigger, my parent name "+transform.parent.name+", mine "+gameObject.name);
        }
    }

    private bool ghost_available = true;
    private IEnumerator resetGhost_availability() // si 2 ghost arrivent en même tempsd ans l'arrivée
    {
        ghost_available = false;
        yield return new WaitForSeconds(1f);
        ghost_available = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Player" || (collision.gameObject.tag == "Player_ghost" && ghost_available))
            && !win)
        {
            if (collision.gameObject.tag == "Player_ghost")
            {
                StartCoroutine(resetGhost_availability());
                boostIngameManager.getInstance().playerGhostEnterredEndCheckpoin();
                collision.gameObject.GetComponent<playerGhost>().ghostEnterredEndCheckP();
            }
            win = true;
            /*if (!infinisManager.infinis_Mode_On)
                StartCoroutine(waitWin());*/

            winParticles.transform.position = transform.position;
            winParticles.GetComponent<ParticleSystem>().Play();
            ondeArrivee.animOndeArrivee();

            if (!give) // eviter rebondis et re-entre dans checkP
            {
                sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.player_winLevel);
                secretOffer.getInstance().playerPassedCheckP();
                switch (playerMovment.comportement)
                {
                    case typeBehaviour.aventure:
                        GetComponent<Animation>().Play("arrivee");
                        StartCoroutine(giveRecompense(false));
                        gameManager.getInstance().win();
                        break;
                    /*case typeBehaviour.tuto:
                        GetComponent<Animation>().Play("arrivee");
                        TutoManager.getInstance().winLevel();
                        StartCoroutine(giveRecompense(false));
                        break;*/
                    case typeBehaviour.infinis:
                        gameManager.getInstance().win();
                        StartCoroutine(giveRecompense(true));
                        GetComponent<Animation>().Play("arrivee_infinis");
                        infinisManager.winLevel();
                        break;
                }
            }
        }
        /*else if (collision.gameObject.tag == "ghostTuto")
        {
            collision.GetComponent<ghostAideTuto>().ghostDiedWon(false);
        }*/
    }



    bool give = false;
    private const float timeWaitGiveReward = 0.3f;//0.6f
    IEnumerator giveRecompense(bool infinis)
    {
        give = true;
        yield return new WaitForSeconds(timeWaitGiveReward);
        Vector2 myPosInUI = Camera.main.WorldToScreenPoint(transform.position);
        List<typeReward_game> rewards = new List<typeReward_game>()
                {
                    typeReward_game.yow
                };

        if (!infinis)
        {
            rewards.Add(typeReward_game.yow);
            if (gameManager.getInstance().currentNiveau.GetComponent<packNiveau>().isLastLevel())
                rewards.Add(typeReward_game.yow);

            else if (!expertMode.modeExpertOn)
                rewards.Add(typeReward_game.life);
        }
        else
        {
            // level 0 , 1 : 2 or
            // level 2 , 3 : 3 or
            // level suivants : 4 or
            int nbOrSupp = roadMapInfinis.currentLevelRoadMap < 2 ? 1 :
                roadMapInfinis.currentLevelRoadMap < 4 ? 2 : 3;

            // infinis donner nb yow en fonction level difficulty actuel
            // 1 yow d'office
            //int currLevelDifficulty = roadMapInfinis.currentLevelRoadMap; // menuLevelUpInfinis.currentDifficultyLevel;
            while (nbOrSupp > 0)
            {
                nbOrSupp--;
                rewards.Add(typeReward_game.yow);
            }
        }

        // pas de compteur open
        //popYowGemManager.getInstance().openCompteurYow_inGame();
        popYowGemManager.getInstance().popRec_game(rewards, myPosInUI);

        give = false;
    }


    /*private void popRec(GameObject o)
    {
        GameObject p = Instantiate(o,
            !TutoManager.tutoOn ? defiManager.getInstance().transform.parent : TutoManager.getInstance().mainCanvas);
        //p.transform.SetParent(
          //  !tutorielManager.tutoOn ? menuDefi.instance.transform : tutorielManager.getInstance().mainCanvas); // le main canvas
        p.transform.localScale = o.transform.localScale;
        p.GetComponent<recompenseDefi>().spawn(
            Camera.main.WorldToScreenPoint(transform.position));
    }*/
}
