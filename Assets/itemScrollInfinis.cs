using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemScrollInfinis : MonoBehaviour
{
    public scrollInfinis scroll;
    public int myPhase;
    private const int maxPhase = 4;
    public Transform levelAssociated;
    private Transform initParentLevelAssociated;

    public float defaultScaleLevelInItem = 0.50028f;

    public void associeLevelToScrollItem(Transform level, int phaseLevel, float posYInParent)
    {
        myPhase = phaseLevel;
        initParentLevelAssociated = level.parent;

        levelAssociated = level;
        levelAssociated.SetParent(transform);
        levelAssociated.localPosition = new Vector2(0, 0);
        levelAssociated.localScale = new Vector3(defaultScaleLevelInItem, defaultScaleLevelInItem, defaultScaleLevelInItem);

        transform.localPosition = new Vector2(0, posYInParent);
    }

    public void goNextPhase()
    {
        StartCoroutine(animGoNextPhase());
    }

    private IEnumerator animGoNextPhase()
    {
        yield return new WaitForSeconds(itemCameraInfinis.timeAnimAlpha);

        myPhase++;
        if (myPhase == maxPhase)
        {
            closeItem();
        }
    }

    public void closeItem()
    {
        levelAssociated.SetParent(initParentLevelAssociated);
        scroll.removeItemFromScroll(gameObject, levelAssociated.gameObject);
    }
}
