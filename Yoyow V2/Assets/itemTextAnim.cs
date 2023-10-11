using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class itemTextAnim : MonoBehaviour
{
    public TextMeshProUGUI ombreText, reliefText;
    private bool spaceChar = false;
    public void setItemText(string s, bool isSpaceChar)
    {
        ombreText.text = s;
        reliefText.text = s;

        changeColorText(0f);
        spaceChar = isSpaceChar;
    }

    public static float timeAnimFadeColor = 0.4f;
    private float initTimeAnim;
    public void animItemText_on(float initTimeWait)
    {
        if (!spaceChar)
        {
            initTimeAnim = Time.time;
            StartCoroutine(animItemText_enumOn(initTimeWait));
        }
    }

    public void animItemText_off()
    {
        if (!spaceChar)
        {
            initTimeAnim = Time.time;
            StartCoroutine(animItemText_enumOff());
        }
    }

    private IEnumerator animItemText_enumOff()
    {
        /*float t = initTimeWait;
        // on attend decompte pour lancer l'anim
        while (Time.time < initTimeWait)
            yield return null;*/

        float t = Time.time + timeAnimFadeColor;
        // change en focntion couleure actuelle
        float prog;
        float initAlpha = alphaTextInvisible;
        float newAlpha;

        while (Time.time < t)
        {
            yield return null;

            prog = (Time.time - initTimeAnim) / timeAnimFadeColor;
            newAlpha = initAlpha - (initAlpha * prog);
            newAlpha = newAlpha < 0f ? 0f : newAlpha;
            changeColorText(newAlpha);

            /* changer couleure en fonction prog
            float currentProg = (Time.time - initTimeAnim) / timeAnimFadeColor;
            currentProg = 1 - (currentProg < 1f ? currentProg : 1f);
            changeColorText(currentProg);*/
        }
    }

    private IEnumerator animItemText_enumOn(float initTimeWait)
    {
        float t = initTimeWait;
        // on attend decompte pour lancer l'anim
        while (Time.time < initTimeWait)
            yield return null;
        t += timeAnimFadeColor;

        while (Time.time < t)
        {
            yield return null;
            // changer couleure en fonction prog
            float currentProg = (Time.time - initTimeWait) / timeAnimFadeColor;
            currentProg = currentProg < 1f ? currentProg : 1f;
            changeColorText(currentProg);
        }
    }

    public float alphaTextInvisible = 0.5f;
    public float timeChangeVisibility = 0.3f;
    public void btnOkHelp(bool toGame)
    {
        if (spaceChar)
            return;

        StartCoroutine(btnOkHelp_enum(
        ombreText.color.a
        , toGame ? alphaTextInvisible : 1f));
    }
    private IEnumerator btnOkHelp_enum(float initAlpha, float goalAlpha)
    {
        float initTime = Time.time;
        float prog;
        float newAlpha;
        do
        {
            yield return null;
            prog = (Time.time - initTime) / timeChangeVisibility;
            newAlpha = initAlpha + (goalAlpha - initAlpha) * prog;
            // check si depasse goal
            newAlpha = initAlpha < goalAlpha ?
                // si va -> visible
                newAlpha > goalAlpha ? goalAlpha : newAlpha
                // si va -> invisible
                : newAlpha < goalAlpha ? goalAlpha : newAlpha;

            changeColorText(newAlpha);

        } while (Time.time < initTime + timeChangeVisibility);
    }

    private void changeColorText(float alpha)
    {
        Color c = ombreText.color;
        ombreText.color = new Color(c.r, c.g, c.b, alpha);
        c = reliefText.color;
        reliefText.color = new Color(c.r, c.g, c.b, alpha);
    }
}
