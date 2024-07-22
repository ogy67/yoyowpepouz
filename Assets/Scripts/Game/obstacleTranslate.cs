using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleTranslate : MonoBehaviour
{
    public float tpsMove;
    private Transform target;
    private Transform mur;
    public Transform firstChild;
    public Transform secondChild;

    private LineRenderer firstLR, secondLR;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.GetChild(1);
        mur = transform.GetChild(0);

        firstLR = firstChild.GetComponent<LineRenderer>();
        secondLR = secondChild.GetComponent<LineRenderer>();

        firstLR.positionCount = 2;
        secondLR.positionCount = 2;

        posFollowZ = firstChild.position.z - 1;
    }

    public void initializeOnline()
    {
        target = transform.GetChild(1);
    }

    private Vector3 posFollow;
    private float posFollowZ;
    // Update is called once per frame
    void Update()
    {
        posFollow = target.position;
        posFollow.z = firstChild.position.z - 1;

        if (Vector2.Distance(mur.position, posFollow) > 0.05f)
            //mur.position != target.position)
        {
            //Vector2 v = 
            
            mur.position = Vector2.MoveTowards((Vector2)mur.position, (Vector2)posFollow, Time.deltaTime / tpsMove);
        }
        else
        {
            target = firstChild == target ? secondChild : firstChild;
        }      
    }

    private void LateUpdate()
    {
        firstLR.SetPosition(0, (Vector2)mur.position);
        firstLR.SetPosition(1, (Vector2)firstChild.position);

        secondLR.SetPosition(0, (Vector2)mur.position);
        secondLR.SetPosition(1, (Vector2)secondChild.position);
    }
}
