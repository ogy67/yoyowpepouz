using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followTarget : MonoBehaviour
{
    public Transform target;
    public bool iAmUi;
    public bool iAmUi_overlay = false;
    public bool isActive = true;

    // Update is called once per frame
    void Update()
    {
        if (!isActive || target == null)
            return;
        if (target.gameObject.activeSelf)
        {
            if (iAmUi) GetComponent<RectTransform>().position = target.position;
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
