using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum trailType
{
    none,
    normal_line,
    two_line,
    two_line_spiral,
    fore_line,
    square_line,
    circle_line,
    dotted_line,
    shadow_line,
    fleche_line,
}

[Serializable]
public struct infoTrail
{
    public trailType type;
    public GameObject line;
    public int nb_sous_line;
}

public class trailManager : MonoBehaviour
{
    public infoTrail[] _infos_trails;
    private GameObject currentLine;

    private static Color32 initColor, endColor;
    private static int indexType = 0;

    public static void setTrailColor(Color32 initC, Color32 endC)
    {
        initColor = initC;
        endColor = endC;
    }

    public trailType getType()
    {
        return _infos_trails[indexType].type;
    }

    public static void setType(int index)
    {
        indexType = index;
    }
    private void setColor()
    {

        int iter = _infos_trails[indexType].nb_sous_line;//nbIter[type];

        Transform c;
        for (int i = 0; i < iter; i++)
        {
            c = currentLine.transform.GetChild(i);

            if (c.GetComponent<TrailRenderer>())
            {
                c.GetComponent<TrailRenderer>().startColor = initColor;
                c.GetComponent<TrailRenderer>().endColor = endColor;
            }
            else // square line
            {
                c.GetComponent<SpriteRenderer>().color = endColor;
            }
        }
    }
    private void setLine()
    {
        currentLine = _infos_trails[indexType].line;
        currentLine.SetActive(true);
    }

    public TrailRenderer tutoTrail;
    public void clearTrail_child() // quand player activate
    {
        //Debug.Log("Cleartrail, tutoOn ? "+TutoManager.tutoOn);
        /*if (TutoManager.tutoOn)
        {
            // on nettoie seulement le trail de base
            tutoTrail.Clear();
        }
        else
        {*/
            foreach (Transform t in currentLine.transform)
            {
                if (t.GetComponent<TrailRenderer>())
                {
                    t.GetComponent<TrailRenderer>().Clear();
                }
            }
        //}
    }
    public void new_game_started()
    {
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);

        setLine();
        setColor();
    }
    public void superPalet_changeColor(Color32 cInit, Color32 cFin)
    {
        foreach (Transform t in currentLine.transform)
        {
            if (t.GetComponent<TrailRenderer>())
            {
                t.GetComponent<TrailRenderer>().startColor = cInit;
                t.GetComponent<TrailRenderer>().endColor = cFin;
            }
            else // square
            {
                t.GetComponent<SpriteRenderer>().color = cFin;
            }
        }
    }
    private void Awake()
    {
        transform.name = "TRAILS";
        setLine();
    }   
}
