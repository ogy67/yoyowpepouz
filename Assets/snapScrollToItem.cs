using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeSnapShop
{
    snapBoost,
    snapSkin,
    snapOffer,
}
public class snapScrollToItem : MonoBehaviour
{
    // tout les sous-menus ont le même offset
    public Vector2 offset;
    public Transform posBottomScroll, posTopScroll;


    public ScrollRect scrollRect_boost, scrollRect_custom, scrollRect_offer;
    public RectTransform viewPort_boost, viewPort_custom, viewPort_offer;
    public RectTransform contentPanel_boost, contentPanel_custom, contentPanel_offer;
    public Transform posBottomContent_boost, posBottomContent_custom, posBottomContent_offer;
    public Transform posTopContent_boost, posTopContent_custom, posTopContent_offer;

    private static snapScrollToItem instance;
    public static snapScrollToItem getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("snapScrollToItem").GetComponent<snapScrollToItem>();
        return instance;
    }

    public void snapToItem(typeSnapShop type, RectTransform target)
    {
        switch (type)
        {
            case typeSnapShop.snapBoost:
                viewPort = viewPort_boost;
                contentPanel = contentPanel_boost;
                posBottomContent = posBottomContent_boost;
                posTopContent = posTopContent_boost;
                break;
            case typeSnapShop.snapSkin:
                viewPort = viewPort_custom;
                contentPanel = contentPanel_custom;
                posBottomContent = posBottomContent_custom;
                posTopContent = posTopContent_custom;
                break;
            case typeSnapShop.snapOffer:
                viewPort = viewPort_offer;
                contentPanel = contentPanel_offer;
                posBottomContent = posBottomContent_offer;
                posTopContent = posTopContent_offer;
                break;
        }
        SnapTo(target);
    }

    private RectTransform viewPort;
    private RectTransform contentPanel;
    public ScrollRect scrollDailyOffer;
    private Transform posBottomContent;
    private Transform posTopContent;

    public static bool nextSnapIsDirectSnap = false;
    public static bool nextSnapIsGemSnap = false;
    public static bool nextSnapIsDailyOfferSnap = false;
    private void SnapTo(RectTransform target)
    {

        Vector2 v1 = (Vector2)viewPort.transform.InverseTransformPoint(contentPanel.position);
        Vector2 v2 = (Vector2)viewPort.transform.InverseTransformPoint(target.position);
        Vector2 pos = (v1 - v2);

        pos = new Vector2(0, pos.y - viewPort.GetComponent<RectTransform>().sizeDelta.y / 2);
        move_id++;
        if (nextSnapIsDirectSnap)
        {
            nextSnapIsDirectSnap = false;

            if(nextSnapIsGemSnap)
                scrollDailyOffer.normalizedPosition = new Vector2(0, 0f);
            else if(nextSnapIsDailyOfferSnap)
                scrollDailyOffer.normalizedPosition = new Vector2(0, 1f);
            else
                contentPanel.anchoredPosition = pos + offset;

            /*if (!nextSnapIsGemSnap)
                contentPanel.anchoredPosition = pos + offset;
            else
                scrollDailyOffer.normalizedPosition = new Vector2(0, 0f);*/
            nextSnapIsGemSnap = false;
        }
        else
        {
            StartCoroutine(moveAnim(contentPanel, pos + offset, move_id));
        }
    }


    private int move_id = 0; // terminer anim si clique cash sur autre item
    private IEnumerator moveAnim(RectTransform toMove, Vector2 toPos, int id)
    {
        float t = 0f, timeMove = 0.2f;
        Vector2 startPos = toMove.anchoredPosition;
        bool up = startPos.y > toPos.y;

        while (t < timeMove && id == move_id &&
             ((!up && posBottomContent.position.y < posBottomScroll.position.y)
             || (up && posTopContent.position.y > posTopScroll.position.y))
             )
        {
            // si veut aller sur item haut et que depasse limite
            if (toMove.anchoredPosition.y <= 0 && up)
                break;

            yield return null;
            t += Time.deltaTime;

            float prog = t / timeMove;
            toMove.anchoredPosition = Vector2.Lerp(startPos, toPos, prog);
        }
    }
}
