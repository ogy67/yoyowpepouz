using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeBonus
{
    ARGENT,
    VIE,
    BOOST,
    GEMME
}

[System.Serializable]
public class posBonus
{
    public bool easy;
    public Transform pos;
    public int lvlRequired;
    public int maxLvl;
    public bool used = false;
}

public class combiPatern : MonoBehaviour
{
    public bool combiCanBeBonus = false;
    public GameObject[] lesLevel;
    public posBonus[] lesPosBonus;
    private List<GameObject> bonusPop;

    public bool notInFirstLevels = false;
    public bool forceSideRotation = false;
    public int sideRotation = 1;

    public GameObject bordParticleF;

    public void openLevel(int lvl)//, List<typeBonus> bonus) // activer les niveaux jusqu'à lvl
    {
        int i = 0;
        while (i < lvl)
        {
            lesLevel[i].SetActive(true);
            i++;
        }
        lvl_2 = lvl;
        //bonus_2 = bonus;
        //StartCoroutine(popBonus(bonus, lvl));
    }

    // PB : reward générés quand level pop haut; phaseBonus = false, 
    int lvl_2;
    //List<typeBonus> bonus_2;
    public void popBonusLevel(List<typeBonus> bonus)
    {
        popBonus(bonus, lvl_2);
    }

    public void closeLevel() // fermer tt les level sauf le premier
    {
        int i = 1;
        while(i < lesLevel.Length)
        {
            if(lesLevel[i] != null)
                lesLevel[i].SetActive(false);
            i++;
        }
        if (bonusPop != null)
        {
            foreach (GameObject o in bonusPop)
            {
                if (o != null) Destroy(o);
            }
            bonusPop = new List<GameObject>();
        }
    }

    public void popBonus(List<typeBonus> bonus, int currentLvlNum)
    {
        //yield return new WaitForSeconds(0.3f);

        bonusPop = new List<GameObject>();
        foreach (typeBonus tB in bonus)
        {
                Vector3 pos = getNewPosBonus(tB, currentLvlNum);
                if (pos != Vector3.zero)
                {
                    GameObject b = recompenseInfinis.popRecompense(pos, tB, transform.parent.parent);
                    bonusPop.Add(b);
                }

        }
        // on reinitialise de suite les "used"
        foreach (posBonus pB in lesPosBonus)
        {
            pB.used = false;
        }
    }

    // les bonus a pop sont appelé par ordre : vie - boost - piece
    // pour remplir d'abord les posHard avec bonus rare

    // on remplit d'abord bonus hard avec VIE/SB
    // on remplit ensuite bonus facile avec reste
    // si reste encore boost ( pos facile plein ), alors remplir pos hard
    public Vector3 getNewPosBonus(typeBonus bonus, int currentLvlNum)
    {
        List<posBonus> listPosEasy = new List<posBonus>(); // contient que les emplacement libre et de type facile
        List<posBonus> listPosHard = new List<posBonus>();

        foreach (posBonus pB in lesPosBonus)
        {
            if (!pB.used && pB.lvlRequired <= currentLvlNum && (pB.maxLvl == 0 || pB.maxLvl <= currentLvlNum))
            {
                if (pB.easy)
                {
                    listPosEasy.Add(pB);
                }
                else
                {
                    listPosHard.Add(pB);
                }
            }
        }

        Vector3 posPop = Vector3.zero;

        if (bonus == typeBonus.VIE || bonus == typeBonus.BOOST )//|| bonus == typeBonus.GEMME)
        {
            if(listPosHard.Count != 0)
            {
                int rand = Random.Range(0, listPosHard.Count);
                listPosHard[rand].used = true;
                posPop = listPosHard[rand].pos.position;
            }
            else if(listPosEasy.Count != 0)
            {
                int rand = Random.Range(0, listPosEasy.Count);
                listPosEasy[rand].used = true;
                posPop = listPosEasy[rand].pos.position;
            }
        }
        else
        {
            if (listPosEasy.Count != 0)
            {
                int rand = Random.Range(0, listPosEasy.Count);
                listPosEasy[rand].used = true;
                posPop = listPosEasy[rand].pos.position;
            }
            else if (listPosHard.Count != 0)
            {
                int rand = Random.Range(0, listPosHard.Count);
                listPosHard[rand].used = true;
                posPop = listPosHard[rand].pos.position;
            }
        }

        return posPop;
        /*List<posBonus> posFree = new List<posBonus>();
        foreach (posBonus pB in lesPosBonus)
        {
            if (!pB.used && pB.lvlRequired <= currentLvlNum && (pB.maxLvl == 0 || pB.maxLvl <= currentLvlNum))
                // les vie ET gemmes ne peuvent pas être sur un emplacement facile
                // dans tout les autres cas on ajoute la pos aux pos potentielles
                if (!((bonus == typeBonus.VIE || bonus == typeBonus.GEMME) && pB.easy))
                    posFree.Add(pB);
        }

        Vector3 retour = Vector3.zero;
        if (posFree.Count != 0)
        {
            int rand = Random.Range(0, posFree.Count);
            posFree[rand].used = true;
            retour = posFree[rand].pos.position;
        }
        return retour;*/
    }
}
