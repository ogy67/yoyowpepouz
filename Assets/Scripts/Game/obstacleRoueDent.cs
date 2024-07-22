using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleRoueDent : MonoBehaviour
{
    private float tpsMove = 1.7f; // pour regler sa vitesse
    private Transform currentTarget;

    public Transform targets;
    public Transform toMove;

    public bool linearMode;
    public bool noLine = false; // superposition de roueDent
    private int side = -1; // par defaut


    private int indexTarget = 0;
    private int nbTarget;

    // Start is called before the first frame update
    void Start()
    {
        nbTarget = targets.childCount;
        currentTarget = targets.GetChild(1);
        toMove.transform.position = targets.GetChild(0).position;
     
        prevScale = transform.localScale;
        prevPos = transform.position;
    }

    private void OnEnable()
    {
        if (!noLine)
            setLineWay();
    }

    private void OnDisable()
    {
        // on desactive les lR
        for (int i = 0; i < nbTarget; i++)
        {
            if (!(linearMode && i == nbTarget - 1)) // pas de boucle si linear
            {
                LineRenderer lR = targets.GetChild(i).GetComponent<LineRenderer>();
                lR.positionCount = 0;
            }
        }
    }

    private Vector3 prevScale, prevPos;


    private void LateUpdate()
    {
        if (!noLine)
            setLineWay();
    }
    // Update is called once per frame
    void Update()
    {
        // si zoom, swipe level
        if (transform.position != prevPos || transform.localScale != prevScale)
        {
            prevPos = transform.position;
            prevScale = transform.localScale;
        }

        if (toMove.position != currentTarget.position)
        {
            //Vector2 v = 
            toMove.position = Vector3.MoveTowards(toMove.position, currentTarget.position, Time.deltaTime / tpsMove);
        }
        else
        {
            setNextTarget();
            currentTarget = targets.GetChild(indexTarget);
            //target = firstChild == target ? secondChild : firstChild;
        }

        //firstLR.SetPosition(0, toMove.position);
        //firstLR.SetPosition(1, firstChild.position);

        //secondLR.SetPosition(0, toMove.position);
        //secondLR.SetPosition(1, secondChild.position);
    }

    private void setLineWay()
    {
        for(int i = 0; i < nbTarget; i++)
        {
            if (!(linearMode && i == nbTarget - 1)) // pas de boucle si linear
            {
                LineRenderer lR = targets.GetChild(i).GetComponent<LineRenderer>();
                lR.positionCount = 2;
                lR.SetPosition(0, targets.GetChild(i).position);
                lR.SetPosition(1, targets.GetChild((i + 1) % nbTarget).position);
            }
        }
    }

    private void setNextTarget()
    {
        transform.GetChild(0).GetComponent<Animation>().Play("roueDentCorner");
        if (!linearMode)
        {
            indexTarget = (indexTarget + 1) % nbTarget;
        }
        else
        {
            if (indexTarget == 0 || indexTarget == nbTarget - 1)
            {
                side = -side;
                toMove.GetComponent<obstacleRotate>().inverseRotation();
            }
            indexTarget += side;
        }
    }


}
