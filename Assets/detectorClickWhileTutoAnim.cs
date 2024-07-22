using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class detectorClickWhileTutoAnim : MonoBehaviour, IPointerDownHandler
{
    // l'animation commence : elle active le content warning -> les clicks peuvent être intercéptés

    public Animation animWarning;
    // si click pendant que anim, alors messageLogWait
    public void OnPointerDown(PointerEventData eventData)
    {
        // l'animation de warning désactive le click : ne peut pas lancer 2 anims en mm temps
        animWarning.Play("warningWaitEndAnim");
    }

    public GameObject contentWarning;
    public void contentWarningSetOn()
    {
        contentWarning.gameObject.SetActive(true);
    }

    public void contentWarningSetOff()
    {
        if (animWarning.isPlaying)
        {
            StartCoroutine(waitEndAnimToClose());
        }
        else
        {
            contentWarning.gameObject.SetActive(false);
        }
    }

    private IEnumerator waitEndAnimToClose()
    {
        while (animWarning.isPlaying)
            yield return null;
        contentWarning.gameObject.SetActive(false);
    }
}
