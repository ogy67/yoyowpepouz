using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DottedLine : MonoBehaviour
{
    // Inspector fields
    public Sprite Dot;
    [Range(0.01f, 1f)]
    public float Size;
    [Range(0.1f, 2f)]
    public float Delta;

    public int LayerDottedLine;

    //Static Property with backing field
    //private DottedLine instance;
    /*public static DottedLine Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DottedLine>();
            return instance;
        }
    }*/

    //Utility fields
    List<Vector2> positions = new List<Vector2>();
    List<GameObject> dots = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (positions.Count > 0)
        {
            DestroyAllDots();
            positions.Clear();
        }

    }

    private void DestroyAllDots()
    {
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();
    }

    GameObject GetOneDot()
    {
        var gameObject = new GameObject();
        gameObject.transform.localScale = Vector3.one * Size;
        gameObject.transform.parent = transform;

        var sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Dot;
        return gameObject;
    }

    public void DrawDottedLine(Vector2 start, Vector2 end, bool orderInversed, bool fade)
    {
        DestroyAllDots();

        Vector2 point = start;
        Vector2 direction = (end - start).normalized;

        while ((end - start).magnitude > (point - start).magnitude)
        {
            positions.Add(point);
            point += (direction * Delta);
        }

        Render(orderInversed, fade);
    }

    private void Render(bool orderInversed, bool fade)
    {
        int nb = positions.Count, compteur = 0;

        foreach (var position in positions)
        {
            var g = GetOneDot();
            g.transform.position = position;
            g.gameObject.layer = LayerDottedLine;

            if (fade)
            {
                float alph = 1f - (compteur * (1f / nb));
                if (orderInversed) alph = 1f - alph;
                g.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alph);
            }
            dots.Add(g);
            compteur++;
        }
    }
}
