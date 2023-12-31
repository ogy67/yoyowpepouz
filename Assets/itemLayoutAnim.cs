﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemLayoutAnim : MonoBehaviour
{
    public RawImage imgAnim;
    public int layoutNumber;
    public float alphaStart, alphaEnd;

    public void setInitAlphaAnim()
    {
        Color c = imgAnim.color;
        imgAnim.color = new Color(c.r, c.g, c.b, alphaStart); 
    }

    public void animImageAlpha()
    {
        StartCoroutine(animAlpha_bounded(alphaStart, alphaEnd));
    }

    public float timeAnimAlpha = 1f;
    private IEnumerator animAlpha_bounded(float start, float end)
    {
        float initTime = Time.time;
        float prog, newAlpha;
        Color c = imgAnim.color;
        imgAnim.color = new Color(c.r, c.g, c.b, start);

        while (Time.time < initTime + timeAnimAlpha)
        {
            yield return null;
            prog = (Time.time - initTime) / timeAnimAlpha;
            newAlpha = start + (end - start) * prog;
            imgAnim.color = new Color(c.r, c.g, c.b, newAlpha);
        }
    }




    private static float alphaOpen = 0.4627451f;
    public void animInitGame_secondLevel()
    {
        // anim layer de 0 -> 0.5
        StartCoroutine(animAlpha_bounded(0f, alphaOpen));
    }

    public void animEndGame_forceAnimClose()
    {
        // anim layer de currentAlpha -> 0
        StartCoroutine(animAlpha_bounded(imgAnim.color.a, 0f));
    }


}
