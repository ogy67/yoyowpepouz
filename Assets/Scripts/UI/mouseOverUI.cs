using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouseOverUI : MonoBehaviour
{

    /*public static bool closed = false; // eviter double clique
    // Update is called once per frame
    void Update()
    {
        if (!closed && Input.GetMouseButton(0) /*&& !IsPointerOverUIObject())
        {
            //StartCoroutine(menuShop.getInstance().closeSuperBounce());
            //StartCoroutine(close());
        }
    }

    IEnumerator close()
    {
        transform.parent.GetComponent<Animation>().Play("achatBoostClose");
        closed = true;
        yield return new WaitForSeconds(1.3f);
        transform.parent.gameObject.SetActive(false);
        closed = false;
    }
    */

    


    /*public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results[0].gameObject.name == "cardPan" ? true : false;
    }*/

}
