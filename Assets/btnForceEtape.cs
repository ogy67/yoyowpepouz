using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class btnForceEtape : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int etapeNumberToReload = 1;

    public GameObject forceEtapePan;
    public void btnOpen()
    {
        forceEtapePan.SetActive(true);
    }
    public void btnClose()
    {
        forceEtapePan.SetActive(false);
    }


    public developerBddManager devBddManager;
    public Image progBarConfirmClick;
    public float timeValidClickButton = 2f;
    private float initTimePressed;
    private bool clickOn = false;
    private bool clickLocked = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickLocked)
            return;

        clickOn = true;
        initTimePressed = Time.time;
        StartCoroutine(waitClickUp());
    }

    private IEnumerator waitClickUp()
    {
        while (clickOn)
        {
            float prog = (Time.time - initTimePressed) / timeValidClickButton;
            progBarConfirmClick.fillAmount = prog;
            if (prog >= 1)
            {
                clickOn = false;
                clickIsValidated();
            }
            else
            {
                yield return null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clickLocked)
            return;
        clickOn = false;
        progBarConfirmClick.fillAmount = 0f;
    }

    private void clickIsValidated()
    {
        progBarConfirmClick.fillAmount = 0f;
        StartCoroutine(lockClick());

        devBddManager.forceReloadEtape(etapeNumberToReload);
    }

    private IEnumerator lockClick()
    {
        clickLocked = true;
        yield return new WaitForSeconds(1f);
        clickLocked = false;
        progBarConfirmClick.fillAmount = 0f;
    }
}
