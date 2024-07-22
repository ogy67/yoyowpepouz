using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fitInScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        setScreenRatio();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void fitScreenSize()
    {
        
    }

    float minRatioToChange = 18 / 9f, maxRatio = 20 / 9f;
    //float minScale = 6.8f, maxScale = 7.3f;

    public Vector3 normalScaleV3, minScaleV3;
    private void setScreenRatio()
    {
        //float newScale = minScale;
        float myRatio = (float)Screen.height / (float)Screen.width;

        Vector3 v_new = normalScaleV3;
        if (myRatio >= minRatioToChange)
        {
            // donne le
            float rapport = (myRatio - minRatioToChange) / (maxRatio - minRatioToChange);
            v_new = normalScaleV3 - ((normalScaleV3 - minScaleV3) * rapport);


            //newScale = minScale + (maxScale - minScale) * rapport;
        }
        // si ratio > 2 , 'dezommer' 
        //originalScale = (float)Screen.height / (float)Screen.width >= 2f ? 6.8f : 6.5f;
        transform.localScale = v_new;
    }
}
