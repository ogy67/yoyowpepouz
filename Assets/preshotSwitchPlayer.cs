using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class preshotSwitchPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position != targetFollow.position)
            followBtnA();
    }

    public checkBtnAMoving checkAMoving;
    public void playerPop_btnAOpen()
    {
        transform.position = targetFollow.position;
        setValues_perDistance();

        checkAMoving.playerPop_btnAOpen();
    }

    public void btnAClickDown()
    {
        transform.position = targetFollow.position;
        setValues_perDistance();

        checkAMoving.btnAClickDown();
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
    private bool canSwitchPlayer = false;
    private void setValues_perDistance()
    {
        float distance = Vector2.Distance(transform.position, targetFollow.position);
        canSwitchPlayer = distance >= distanceFromBtnAToSwitch;
        //test_imgFollow.color = canSwitchPlayer ? test_colorOn : test_colorOff;
    }

    public Vector2 getPosItemFollow()
    {
        return transform.position;
    }

    public playerMovment pM;
    // return true si switch
    public bool playerHangCheckP()
    {
        //if (TutoManager.tutoOn && TutoManager.currentPhase.type_phase != typeTutoPhase.changeRotationSide)
        //    return false;

        //Debug.Log("Hang checkP, test if switch PRESHOT");
        if (canSwitchPlayer)
        {
            //Debug.Log("PRESHOT");
            StartCoroutine(switchPlayerLate());
            return true;
        }
        else
        {
            //Debug.Log("NO PRESHOT");
            return false;
        }
    }

    // attendre que a bien hang le checkP
    public float timeWaitHang = 0.1f;
    private IEnumerator switchPlayerLate()
    {
        yield return new WaitForSeconds(timeWaitHang);
        if(!playerMovment.finish)
            pM.btnChangeRotation();
    }
}

// 82 l
