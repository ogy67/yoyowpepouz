using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getStarMenuP : MonoBehaviour
{
    public Transform parentStarTransform;
    public Transform uiStarTransform;

    public Animation parentStarAnim;
    public Animation starObtainAnim;

    // gameManager.CloseGame()
    // -> managerNiveau.obtainStar()  ( fait l'anim du voyant star )

    public static float timeAnimGetStar = 3f;
    public float timeAnimFade = 0.5f;

    public void playerGetStar()
    {
        StartCoroutine(animGetStar());
    }

    private IEnumerator animGetStar()
    {
        // on met l'anim de get star au premier plan
        Transform initParentUiStar = uiStarTransform.parent;
        uiStarTransform.SetParent(parentStarTransform);

        // on anim mise au premier plan
        parentStarAnim.Play("animGetStarMenuPOn");

        yield return new WaitForSeconds(timeAnimGetStar - timeAnimFade);

        // anim fade off
        parentStarAnim.Play("animGetStarMenuPOff");

        yield return new WaitForSeconds(timeAnimFade);

        // on met l'anim de get star au second plan
        uiStarTransform.SetParent(initParentUiStar);
    }
}
