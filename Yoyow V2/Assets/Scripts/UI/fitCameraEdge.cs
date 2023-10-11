using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fitCameraEdge : MonoBehaviour
{
    public GameObject referencePosBorderRight;

    // Start is called before the first frame update
    void Start()
    {
        pos();
    }

    public bool reposition = false;
    private void Update()
    {
        if (reposition)
        {
            reposition = false;
            pos();
        }
    }

    private void pos()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(referencePosBorderRight.transform.position);
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        //Debug.Log("ConvertedPos : " + Camera.main.ScreenToWorldPoint(referencePosBorderRight.transform.position));
    }

}
