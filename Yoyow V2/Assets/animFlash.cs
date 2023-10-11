using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeFlash
{
    none,
    bounce,
    pivot,
    portal,
    fusee,
    souris,
    ghost
}

public class animFlash : MonoBehaviour
{
    public float timeAnim = 0.5f;
    private static GameObject instance;
    public static void animateFlash(Vector2 dirLookAt, typeFlash type)
    {
        if (instance == null)
            instance = GameObject.Find("animFlash");
        GameObject o = Instantiate(instance);
        o.GetComponent<animFlash>().anim(dirLookAt, type);
    }

    public Color colorFlash_bounce, colorFlash_pivot, colorFlash_portal, colorFlash_fusee, colorFlash_souris, colorFlash_ghost;
    public SpriteRenderer flashImg;

    public Transform centerTr;
    public Transform posLookAtTr;
    public Animation myAnim;
    private void anim(Vector2 dirLookAt, typeFlash type)
    {
        //posLookAtTr.position = (Vector2)centerTr.position + dirLookAt;
        transform.up = ((Vector2)transform.position + dirLookAt) - (Vector2)transform.position;
        //transform.LookAt(posLookAtTr);

        Color c = type == typeFlash.bounce ? colorFlash_bounce :
            type == typeFlash.pivot ? colorFlash_pivot :
            type == typeFlash.portal ? colorFlash_portal :
            type == typeFlash.fusee ? colorFlash_fusee :
            type == typeFlash.ghost ? colorFlash_ghost :
            colorFlash_souris;
        flashImg.color = new Color(c.r, c.g, c.b, 0);

        myAnim.Play("flash");
        StartCoroutine(destroyLate());
    }

    private IEnumerator destroyLate()
    {
        yield return new WaitForSeconds(timeAnim);
        Destroy(gameObject);
    }
}
