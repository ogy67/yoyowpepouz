using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleRotate : MonoBehaviour
{
    public bool antiHorraire;
    public int degrePerSec;
    // Start is called before the first frame update
    void Start()
    {
        if (!antiHorraire)
        {
            degrePerSec = -degrePerSec;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, degrePerSec * Time.deltaTime);
    }

    public void inverseRotation()
    {
        degrePerSec = -degrePerSec;
    }


    // super visee
    public int offset;
    public void setInitPosVisee()
    {
        Vector2 toLookAt = playerMovment.releaseVelocity;
        Vector2 startAxis = new Vector2(1, 0);
        float angle = Vector2.Angle(startAxis, toLookAt);
        transform.rotation = Quaternion.Euler(0, 0, angle + offset);
    }
}
