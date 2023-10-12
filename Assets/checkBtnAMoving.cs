using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkBtnAMoving : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.position != targetFollow.position)
            followBtnA();
    }

    public void playerPop_btnAOpen()
    {
        transform.position = targetFollow.position;
        setValues_perDistance();
    }

    public void btnAClickDown()
    {
        transform.position = targetFollow.position;
        setValues_perDistance();
    }





    public Transform targetFollow;
    public float speedFollow = 1f;

    public float maxDistanceRepos = 70f;
    public float speedReposFactor = 2.4f;
    private void followBtnA()
    {
        float factor = Vector2.Distance(transform.position, targetFollow.position) > maxDistanceRepos ?
            speedReposFactor : 1f;
        transform.position = Vector2.MoveTowards(transform.position, targetFollow.position, Time.deltaTime * speedFollow * factor);
        setValues_perDistance();
    }

    public float distanceFromBtnAToSwitch = 20f;
    public Image test_imgFollow;
    public Color test_colorOn, test_colorOff;
    private bool btnIsMoving = false;
    private void setValues_perDistance()
    {
        float distance = Vector2.Distance(transform.position, targetFollow.position);
        btnIsMoving = distance >= distanceFromBtnAToSwitch;
        //test_imgFollow.color = btnIsMoving ? test_colorOn : test_colorOff;
    }

    public bool isBtnMoving()
    {
        return btnIsMoving;
    }
}
