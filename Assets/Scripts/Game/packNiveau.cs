using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeLevel
{
    none,
    precision,
    endurance,
    agilite
}


/*class effectColorInfo
{
    public typeEffectColor _typeEffectColor;
    public Color32 _colorEffect;
}*/

public class packNiveau : MonoBehaviour
{
    public typeLevel type;
    public string code_save_niveau; // code unique a chaque niveau
    public int levelDifficulty;
    public int crownToUnlock;
    public int starToUnlock;

    public Transform initCenter;
    public Transform initPos;

    public Transform[] lesNiveaux; // /!\ supprimer la liste par la suite
    public int currentLevel = 0;

    public bool forceSideRotation = false;
    public int sideRotation = 0;

    public bool isALevelTuto = false;
    public List<int> listIndexLevelWithTuto;
    public typeTutoAventure[] listTypeTuto;
    public typeTutoAventure getTypeTutoByLevel()
    {
        /*Debug.Log("Try get type tuto level");
        Debug.Log("Current level : " + currentLevel + 1);
        Debug.Log("List index available : " + listIndexLevelWithTuto);*/
        if (listIndexLevelWithTuto.Contains(currentLevel + 1))
        {
            return listTypeTuto[listIndexLevelWithTuto.IndexOf(currentLevel + 1)];
        }
        return typeTutoAventure.none;
    }

    public void initNiveauGame()
    {
        if (levelGlowLight.Count == 0)
        {
            foreach (Transform t in transform)
            {
                if (t.name.Contains("bonusLight"))
                    levelGlowLight.Add(t.GetComponent<SpriteRenderer>());
            }
            if (levelGlowLight.Count == 0)
            {
                Debug.Log("<color=red>BONUS LIGHT not found");
                return;
            }
        }
    }
    public void startGame()
    {
        playerMovment.initCenter = initCenter;
        playerMovment.initPos = initPos.position;
    }
    public void playerReplay()
    {
        partieFinit();
        gameEnd();// reset la gemme
    }

    public bool niveauClean()
    {
        return gameManager.SCORE == lesNiveaux.Length;
    }

    public Transform gemmePos;
    public GameObject recompenseGo;
    private GameObject currentGemme; // on l'enregistre, si != null fin de game -> destroy

    public void lvlClean()
    {
        if (currentLevel + 1 != lesNiveaux.Length)
        {
            currentLevel++;
            lesNiveaux[currentLevel].gameObject.SetActive(true);
            lesNiveaux[currentLevel].GetComponent<Animation>().Play("newLevel");

            if (currentLevel + 1 == lesNiveaux.Length
                && gemmePos != null)
            {
                // si on est sur le dernier niveau, on pop une gemme
                StartCoroutine(popGemmeLate());
            }
        }
    }

    private IEnumerator popGemmeLate() // sinon animnew level en même temps que gemme pop
    {
        yield return new WaitForSeconds(0.5f);
        GameObject g = Instantiate(recompenseGo);
        g.transform.position = gemmePos.position;
        //g.SetActive(true); // déja actif de base
        g.GetComponent<recompenseInfinis>().pop(typeBonus.GEMME);

        currentGemme = g;
    }

    public void partieFinit() // appelé quand recommence ou va au menu principal
    {
        for (int i = 1; i <= currentLevel; i++) // laisse le premier niveau activé
        {
            lesNiveaux[i].gameObject.SetActive(false);
        }
        currentLevel = 0;
    }

    public void gameEnd()
    {
        if (currentGemme != null)
        {
            StartCoroutine(currentGemme.GetComponent<recompenseInfinis>().cleanRecNotTaken());
        }
        /*else
        {
            Debug.Log("<color=red>SAVE GEMME</color>");
            // réccupéré
            gemmesManager.saveNbGemmes();
        }*/
    }

    public bool isLastLevel()
    {
        return lesNiveaux.Length - 1 == currentLevel;
    }

    public bool proposerVieSupp()
    {
        /*int l = lesNiveaux.Length;

        return l == 3 && currentLevel == 2 ?
            true :
            l > 3 && currentLevel > lesNiveaux.Length - 3 ? true : false;*/

        return currentLevel == 2 ? true : false;
        /*expertMode.modeExpertOn ?
            currentLevel == 3 ? true : false
            : currentLevel >= 2 ? true : false;*/
    }

    // NIVEAU doit être appelé quand change side ( est close sans anim )
    // -> reinitialiser couleure glowLight ( si était en anim par exemple )
    // OU QUAND change side ( est close sans anim )
    public void levelIsClosedByScroll()
    {
        if (getAnim().isPlaying)
            getAnim().Stop();
        foreach (SpriteRenderer sP in levelGlowLight)
            sP.color = new Color(1f, 1f, 1f, 0f);
    }



    public static float timeAnimGlowEffect = 35f/60f;
    private List<SpriteRenderer> levelGlowLight = new List<SpriteRenderer>();

    // si anim en court, la stopper

    public void glowEffectIngame(Color _colorEffect)
    {
        
        // en fonction typeEffet, changer couleure ( win - loose - portal - pivot )
        StartCoroutine(animEffectColorIngame_enum(_colorEffect));
    }

    private int id_effect = 0;
    private IEnumerator animEffectColorIngame_enum(Color colorEffect)
    {
        if (getAnim().isPlaying)
            getAnim().Stop();
        id_effect++;
        int this_id = id_effect;
        // on set couleure = couleure effect
        foreach (SpriteRenderer sP in levelGlowLight)
            sP.color = colorEffect;
        //levelGlowLight.color = colorEffect;
        // on fait anim alpha 0 -> goal -> 0
        getAnim().Play("glowEffectIngame");

        yield return new WaitForSeconds(timeAnimGlowEffect);

        if (id_effect == this_id)
        {
            //levelGlowLight.color = new Color(1f, 1f, 1f, 0f);
            foreach (SpriteRenderer sP in levelGlowLight)
                sP.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    private Animation myAnim;
    private Animation getAnim()
    {
        if (myAnim == null)
            myAnim = GetComponent<Animation>();
        return myAnim;
    }
}