using System.Collections;
using UnityEngine;

public class obstacleTrianglePop : MonoBehaviour
{
    public float rangeTimeAnim;
    public float delayStartAnim;

    private Animation myAnim;
    private Animation getAnim()
    {
        if (myAnim == null)
            myAnim = GetComponent<Animation>();
        return myAnim;
    }
    private void OnEnable()
    {
        getAnim().Play("triangleInit");
        StartCoroutine(loopAnim());
    }

    private IEnumerator loopAnim()
    {
        yield return new WaitForSeconds(delayStartAnim);
        while (true)
        {
            getAnim().Play("trianglePop");
            yield return new WaitForSeconds(rangeTimeAnim);
        }
    }
}
