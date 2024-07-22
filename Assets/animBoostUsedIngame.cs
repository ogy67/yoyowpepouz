using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animBoostUsedIngame : MonoBehaviour
{
    public Transform parentAnimBoost;
    public GameObject instance;

    // l'instance gère l'animation de tout les autres items
    public void animateNewBoost(typeBoostGame tBG)
    {
        GameObject go = Instantiate(instance, parentAnimBoost);
        go.GetComponent<animBoostUsedIngame>().initAnim(tBG);
    }

    // appelé par l'instance
    public Animation myAnim;
    public boostIngameManager boostM;
    public SpriteRenderer ondeBoostImg;
    public boostImgInfos[] _infos; // copie celui de boost manager
    private GameObject close_endAnim;

    public void initAnim(typeBoostGame tBG)
    {
        //boostM._infos.CopyTo(_infos, 0);

        boostImgInfos bI = getInfoByType(tBG);
        bI.img_associe.SetActive(true);
        ondeBoostImg.color = new Color(bI.ondeColor.r, bI.ondeColor.g, bI.ondeColor.b, 0f);

        // verifie si utiliser medic après mort de player ou de ghost
        if (tBG == typeBoostGame.medic && playerGhost.typeGhost != typePlayerGhost.none)
        {
            myAnim.GetComponent<followTarget>().isActive = false;
            Vector2 animPos;

            if (playerGhost.typeGhost == typePlayerGhost.pierreT)
            {
                animPos = playerGhost.pierreTombale_endPos;
                playerGhost.pierreTombale_endPos = Vector2.zero;
            }
            else
            {
                animPos = playerGhost.posEndGhost[Random.Range(0, playerGhost.posEndGhost.Count)];
                playerGhost.posEndGhost = new List<Vector2>();
            }

            playerGhost.typeGhost = typePlayerGhost.none;
            myAnim.transform.position = animPos;
        }


        if (close_endAnim != null)
            close_endAnim.SetActive(false);

        myAnim.Play("use_boost");
        close_endAnim = bI.img_associe;
        StartCoroutine(closeBoostLate(close_endAnim));
    }

    public boostImgInfos getInfoByType(typeBoostGame tBG)
    {
        foreach (boostImgInfos bII in _infos)
        {
            if (bII.type == tBG)
                return bII;
        }
        return new boostImgInfos();
    }

    private IEnumerator closeBoostLate(GameObject toClose)
    {
        yield return new WaitForSeconds(50 / 60f);
        Destroy(gameObject);
    }
}
