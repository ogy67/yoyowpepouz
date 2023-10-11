using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeOnde
{
    none,
    life,
    pierreT,
}
public class animOndeBoost : MonoBehaviour
{
    public float timeAnim = 0.5f;
    private static GameObject instance;
    public static void animateOnde(typeOnde type)
    {
        if (instance == null)
            instance = GameObject.Find("animOndeBoostNormal");
        GameObject o = Instantiate(instance);
        o.GetComponent<animOndeBoost>().anim(type);
    }


    public Color colorOnde_life, colorOnde_pierreT;
    public SpriteRenderer ondeImg;
    public Animation myAnim;
    private void anim(typeOnde type)
    {
        Color c = type == typeOnde.life ? colorOnde_life :
            colorOnde_pierreT;
        ondeImg.color = new Color(c.r, c.g, c.b, 0);

        myAnim.Play("ondeBoostNormal");
        StartCoroutine(destroyLate());
    }

    private IEnumerator destroyLate()
    {
        yield return new WaitForSeconds(timeAnim);
        Destroy(gameObject);
    }
}
