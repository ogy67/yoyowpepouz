using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class doubleScrollShop : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{ 
    public RectTransform[] sectionShopTransforms;
    public RectTransform contentScroll;
    public shopManager shopM;

    //public Animation glowSwitchAnim;
    public float maxTime_quickDrag = 0.5f;
    public float minDistance_quickDrag = 50f;
    private float timeOnStartDrag;
    private float timeDrag;
    private Vector2 initDragPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Start drag souble scroll");
        timeOnStartDrag = Time.time;
        initDragPos = eventData.position;
    }

    public static int currentIndexCentered = 0;
    public void OnEndDrag(PointerEventData eventData)
    {
        // cherche la section la + proche du centre de l'écran
        RectTransform neareastFromCenter = sectionShopTransforms[0];
        float currentNearestDistanceFromCenter = getPosFromMiddle(sectionShopTransforms[0].position.x);
        float distanceToCompare;
        int indexCentered = 0;

        for (int i = 1; i < sectionShopTransforms.Length; i++)
        {
            if ((distanceToCompare = getPosFromMiddle(sectionShopTransforms[i].position.x)) < currentNearestDistanceFromCenter)
            {
                neareastFromCenter = sectionShopTransforms[i];
                currentNearestDistanceFromCenter = distanceToCompare;
                indexCentered = i;
            }
        }

        timeDrag = Time.time - timeOnStartDrag;
        // si le drag ne change pas de section, vérifier si quickDrag : forcer changement
        if (currentIndexCentered == indexCentered)
        {
            bool quickDrag = timeDrag < maxTime_quickDrag
                && Vector2.Distance(initDragPos, eventData.position) > minDistance_changeSpeed;
            if (quickDrag)
            {
                // regarde dans quel sens a quickDrag
                bool right = eventData.position.x < initDragPos.x;
                // si ne depasse pas limite scroll
                if(right && indexCentered < 2
                    || !right && indexCentered > 0)
                {
                    indexCentered += right ? 1 : -1;
                    neareastFromCenter = sectionShopTransforms[indexCentered];
                    StartCoroutine(snapToSection(getNewPosContent(neareastFromCenter), indexCentered));
                    //Debug.Log("Quick drag VALID, index section : "+indexCentered);
                }
                // on le recentre sur current item
                else
                {
                    StartCoroutine(snapToSection(getNewPosContent(neareastFromCenter), currentIndexCentered));
                }
            }
            else
            {
                StartCoroutine(snapToSection(getNewPosContent(neareastFromCenter), currentIndexCentered));
            }          
        }
        else
        {
            // anim glow
            //glowSwitchAnim.Play(indexCentered > currentIndexCentered ? "glowSwitchLeft" : "glowSwitchRight");
            // sinon si change section, anim
            StartCoroutine(snapToSection(getNewPosContent(neareastFromCenter), indexCentered));
        }

        currentIndexCentered = indexCentered;
        shopM.endDrag_changeBtnSousMenu(indexCentered);
        
    }

    public Animation[] animSectionTab;
    public GameObject[] contentSectionGoTab; 
    public void goToItem_fromShop(int indexSection)
    {
        //switchAnim_perf(currentIndexCentered, indexSection);
        //glowSwitchAnim.Play(indexSection > currentIndexCentered ? "glowSwitchLeft" : "glowSwitchRight");
        // on part du principe que btn section n'appelle pas cette fonction si déjà dans bonne section
        StartCoroutine(snapToSection(getNewPosContent(sectionShopTransforms[indexSection]), indexSection));
        currentIndexCentered = indexSection;
    }

    private void switchAnim_perf(int indexStart, int indexEnd)
    {
        //Debug.Log("Anim switch perf");
        // current section anim glow off -> desactiver
        animSectionTab[indexStart].Play("sectionShopScrollOff");
        StartCoroutine(disableSectionGoLate(contentSectionGoTab[indexStart]));
        // current section activer -> anim glow on
        contentSectionGoTab[indexEnd].SetActive(true);
        animSectionTab[indexEnd].Play("sectionShopScrollOn");

        // on regarde si on doit rebuild la section
        checkIfRebuildLayout(indexEnd);

    }

    public void checkIfRebuildLayout(int indexOpenned)
    {
        if (indexOpenned == 0 // ouvre section boost
            && boostManager.shouldBeRebuildWhenOpen)// && boostManager.getInstance().boostPan.activeInHierarchy)
        {
            boostManager.getInstance().rebuildLayout();
            boostManager.shouldBeRebuildWhenOpen = false;
        }

        else if (indexOpenned == 1 // ouvre section skin
            && skinManager.shouldBeRebuildWhenOpen)// && skinManager.getInstance().skinPan.activeInHierarchy)
        {
            skinManager.getInstance().rebuildLayout();
            skinManager.shouldBeRebuildWhenOpen = false;
        }
    }

    private IEnumerator disableSectionGoLate(GameObject sectionGo)
    {
        yield return new WaitForSeconds(1 / 6f);
        sectionGo.SetActive(false);
    }

    public void goToItem_outShop(int indexSection)
    {
        // on centre directement la section
        contentScroll.localPosition = new Vector2(
            getNewPosContent(sectionShopTransforms[indexSection])
            , contentScroll.localPosition.y);

        // desactiver section (perf)
        if (indexSection != currentIndexCentered)
        {
            contentSectionGoTab[currentIndexCentered].SetActive(false);
            animSectionTab[currentIndexCentered].GetComponent<CanvasGroup>().alpha = 0;

            contentSectionGoTab[indexSection].SetActive(true);
            animSectionTab[indexSection].GetComponent<CanvasGroup>().alpha = 1;
        }

        currentIndexCentered = indexSection;
    }

    // /!\ lock scrollRect raycastTarget pendant tt snap
    public float minProgStopSnap = 0.97f;
    public static bool isSnapingAnotherItem = false;
    private IEnumerator snapToSection(float posSnap, int indexSectionToGo)
    {
        float initPos = contentScroll.localPosition.x;
        float distanceToMove = posSnap - initPos;
        float totalDistanceDone = 0f;
        float progSnap;
        float speedSnap_2 = getSpeedByDistance(Mathf.Abs(distanceToMove));
        lockUnlockScroll(true);
        isSnapingAnotherItem = true;

        if(indexSectionToGo != currentIndexCentered)
            switchAnim_perf(currentIndexCentered, indexSectionToGo);

        do
        {
            totalDistanceDone += Time.deltaTime * speedSnap_2;
            if ((progSnap = totalDistanceDone / Mathf.Abs(distanceToMove)) > 1)
                progSnap = 1;
            contentScroll.localPosition = new Vector2(
                initPos + distanceToMove * progSnap
                , contentScroll.localPosition.y);
            yield return null;
        } while (progSnap < minProgStopSnap);
        isSnapingAnotherItem = false;
        lockUnlockScroll(false);
        contentScroll.localPosition = new Vector2(posSnap, contentScroll.localPosition.y);
    }

    public Image lockScrollImg;
    private void lockUnlockScroll(bool locked)
    {
        //Color c = lockScrollImg.color;
        //lockScrollImg.color = new Color(c.r, c.g, c.b, (locked ? 120 : 20) / 255f);
        lockScrollImg.raycastTarget = locked ? true : false;
    }



    public Transform contentParent;
    public Transform posMiddleImg;
    public float speedSnap = 20f;
    public float factorSpeedIncreasePerUnit = 1.02f;
    public float minDistance_changeSpeed = 50f;
    private float getNewPosContent(RectTransform rectToCenter)
    {
        // on décalle 'content' pour que section soit centrée
        float decalement = rectToCenter.position.x - posMiddleImg.position.x;
        float newPosContent_world = contentScroll.position.x - decalement;
        return contentParent.InverseTransformPoint(new Vector3(newPosContent_world, 0, 0)).x;
    }
    // en fonction de la distance , speedSnap change
    private float getSpeedByDistance(float itemDistanceFromSnapPos)
    {
        if (itemDistanceFromSnapPos > minDistance_changeSpeed)
        {
            float diff = itemDistanceFromSnapPos - minDistance_changeSpeed;
            return speedSnap + diff * factorSpeedIncreasePerUnit;
        }
        return speedSnap;
    }

    private float getPosFromMiddle(float posX)
    {
        return Mathf.Abs(posX - posMiddleImg.position.x);
    }
}

// 117.l -> 68.l
// 117.l -> 95.l
