using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Transform_2
{
    public Vector3 localScale;
    public Vector3 localPosition;
}
public class safeArea : MonoBehaviour
{
    private static safeArea instance;
    public static safeArea getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("safeAreaTopScreenCanvas").GetComponent<safeArea>();
        return instance;
    }
    /*public bool btnApplySafeArea_test = false;
    public bool btnResetSafeAreaPos_test = false;
    private void Update()
    {
        if (btnApplySafeArea_test)
        {
            btnApplySafeArea_test = false;
            ApplySafeArea();
        }
        if (btnResetSafeAreaPos_test)
        {
            btnResetSafeAreaPos_test = false;
            resetUIPos_test();
        }
    }*/

    private void resetUIPos_test()
    {
        coinTr.localScale = coinCopy.localScale;
        coinTr.localPosition = coinCopy.localPosition;

        gemTr.localScale = gemCopy.localScale;
        gemTr.localPosition = gemCopy.localPosition;

        paramTr.localPosition = paramCopy.localPosition;
    } 
    

    public RectTransform parentRectTransform;
    public RectTransform safeArea_test;
    public RectTransform safeAreaRectTr;
    public bool test = false;

    private void Start()
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        if (test)
        {
            safeAreaRectTr.anchorMin = safeArea_test.anchorMin;
            safeAreaRectTr.anchorMax = safeArea_test.anchorMax;
            safeAreaRectTr.anchoredPosition = safeArea_test.anchoredPosition;
            safeAreaRectTr.sizeDelta = safeArea_test.sizeDelta;
        }
        else
        {
            Rect safeAreaRect = Screen.safeArea;

            float scaleRatio = parentRectTransform.rect.width / Screen.width;

            var left = safeAreaRect.xMin * scaleRatio;
            var right = -(Screen.width - safeAreaRect.xMax) * scaleRatio;
            var top = -safeAreaRect.yMin * scaleRatio;
            var bottom = (Screen.height - safeAreaRect.yMax) * scaleRatio;

            RectTransform rectTransform = safeAreaRectTr;
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(right, top);
        }
    }

    // Doit être appelé une fois que canvas app loaded ( sinon positions faussées )
    public void appIsOpenned()
    {
        setMovingAreaPos();
    }

    public Transform topSafeArea, topMovingUI;
    public Transform posMinMovingUI, posCenterMovingUI;
    public Transform movingAreaTr;
    private void setMovingAreaPos()
    {
        if(topSafeArea.position.y < topMovingUI.position.y)
        {
            float initPosY = topMovingUI.position.y;
            float distance = initPosY - topSafeArea.position.y;

            // decaler movingArea pour être sur pos de top area
            movingAreaTr.position = new Vector2(movingAreaTr.position.x, movingAreaTr.position.y - distance);

            // Si centre moving area est passé en dessous de centerMinY, decaler
            if (posCenterMovingUI.position.y < posMinMovingUI.position.y)
                movingAreaTr.position = new Vector2(movingAreaTr.position.x, posMinMovingUI.position.y);

            //distanceMoveY = movingAreaTr.position.y - initPosY;
            moveIngameUiToFitScreen(true);
        }
        else
            moveIngameUiToFitScreen(false);
    }

    public Transform coinTr, gemTr, paramTr;
    public Transform contentVoyantHaut;
    private Transform_2 coinCopy, gemCopy, paramCopy;
    // doit seulement être appelé à start APP
    private void moveIngameUiToFitScreen(bool changePos)
    {
        if(changePos)
            foreach (Transform t in new Transform[] { coinTr, gemTr, paramTr })
                t.position = new Vector2(t.position.x, movingAreaTr.position.y);//, t.position.y + distanceMoveY);

        coinCopy.localScale = coinTr.localScale;
        coinCopy.localPosition = coinTr.localPosition;
        gemCopy.localScale = gemTr.localScale;
        gemCopy.localPosition = gemTr.localPosition;
        paramCopy.localScale = paramTr.localScale;
        paramCopy.localPosition = paramTr.localPosition;
    }

    // appelé par gameManager à initialisation game
    public float timeVoyantGoUp = 2 / 6f;
    public void animVoyantHautGameStart()
    {
        StartCoroutine(initGameEnum());
    }

    private IEnumerator initGameEnum()
    {
        yield return new WaitForSeconds(timeVoyantGoUp);
        // anim monte
        float posYcoin = coinTr.position.y, posYgem = gemTr.position.y, posYparam = paramTr.position.y;

        // anim finit, parent anim se recentre, voyant restent sur la même position
        contentVoyantHaut.localPosition = new Vector2(0, 0);
        coinTr.position = new Vector2(0, posYcoin);
        gemTr.position = new Vector2(0, posYgem);
        paramTr.position = new Vector2(paramTr.position.x, posYparam);
    }

    // appelé après fermeture de niveau, s'assurer que les compteurs ont bien reprit la bonne place
    // si referme gameover trop vite alors compteur peut rester bloqué
    // -> le premier reset est annulé parce que anim se termine et annule reset
    /*public void forceCloseClean()
    {
        coinTr.localScale = coinCopy.localScale;
        coinTr.localPosition = coinCopy.localPosition;

        gemTr.localScale = gemCopy.localScale;
        gemTr.localPosition = gemCopy.localPosition;

        paramTr.localPosition = paramCopy.localPosition;

        //popYowGemManager.resetPosCompteurs_endGame();
    }*/

    public void animVoyantHautGameEnd()
    {
        StartCoroutine(endGameEnum());
    }

    public float timeCloseGameOver_toResetPos = 0.8f;
    private IEnumerator endGameEnum()
    {
        yield return new WaitForSeconds(timeCloseGameOver_toResetPos);

        popYowGemManager.resetPosCompteurs_endGame();

        coinTr.localScale = coinCopy.localScale;
        coinTr.localPosition = coinCopy.localPosition;

        gemTr.localScale = gemCopy.localScale;
        gemTr.localPosition = gemCopy.localPosition;

        paramTr.localPosition = paramCopy.localPosition;
    }
}
