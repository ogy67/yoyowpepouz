using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fondJeu : MonoBehaviour
{
    public float posMin;
    public float posMax;
    public float goal;
    public float speed;

    private RectTransform rT;
    // Start is called before the first frame update
    void Start()
    {
        rT = gameObject.GetComponent<RectTransform>();
        setGoal(posMin);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rT.localPosition.x) <= posMax)
        {
            transform.position = Vector2.MoveTowards(rT.position, new Vector2(goal, rT.position.y), Time.deltaTime * speed);
            //Debug.Log(Mathf.Abs(rT.localPosition.x) + " " + posMax);
        }
        else
            setGoal(rT.localPosition.x);
    }

    void setGoal(float x)
    {
        //Debug.Log("Set goal " + x);
        rT.localPosition = new Vector2(x >= posMax ? posMax : posMin, rT.localPosition.y);
        goal = x >= posMax ? posMin : posMax;
    }
}
