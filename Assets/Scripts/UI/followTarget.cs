using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTarget : MonoBehaviour
{
    public Transform target;
    public bool iAmUi;
    public bool iAmUi_overlay = false;
    public bool isActive = true;

    private RectTransform rect;
    // Update is called once per frame
    void Update()
    {
        if (!isActive || target == null)
            return;
        if (target.gameObject.activeSelf)
        {
            if (iAmUi) { 
                if(rect == null)
                    rect = GetComponent<RectTransform>();
                rect.position = new Vector3(target.position.x, target.position.y, rect.position.z); 
            }
            else
                transform.position = new Vector2(target.position.x, target.position.y);
        }
    }

    /*private void FixedUpdate()
    {
        if (!isActive)
            return;
        if (target.gameObject.activeSelf)
        {
            if (iAmUi) GetComponent<RectTransform>().position = target.position;
            transform.position = target.position;
        }
    }*/
}
