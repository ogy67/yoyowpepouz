using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fitPlayerDirection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (isAttached) {
            transform.up = (Vector2)playerMovment.playerDirection - (Vector2)transform.position;
        }
        if (myAnim != null )
        {
            if (playerMovment.finish == false && !animOn)
            {
                myAnim["trailSpiral"].speed = 1f;
                animOn = true;
            }
            else if (animOn && playerMovment.finish == true)
            {
                myAnim["trailSpiral"].speed = 0f;
                animOn = false;
            }
            //StartCoroutine(play_stop_trailAnim());
        }
    }

    private bool animOn = false;


    private bool isAttached = true;
    public void trailUnattached() // passe dans portail
    {
        isAttached = false;
    }

    public Animation myAnim;
    // exclusivement pour double spiral
    /*private void OnEnable()
    {
        Debug.Log("Enabled, Animation null ? " + myAnim == null);
        if (myAnim != null)
        {
            StartCoroutine(play_stop_trailAnim());            
        }
    }*/

    /*private void OnEnable()
    {
        Debug.Log("Enable");
    }
    private void OnDisable()
    {
        Debug.Log("Disable");
    }

    // dans update
    // si routine pas lancée, lancer
    // sinon si routine precedente a ete bloqué

    private bool animCHeckerOn = false;
    //private bool waitFinish = false;

    //private int num_phase;
    //private bool stop_ended_well = true;
    private IEnumerator play_stop_trailAnim()
    {
        //num_phase = 0;
        //stop_ended_well = false;
        //waitFinish = false;
        animCHeckerOn = true;

        yield return new WaitUntil(predicate: () => playerMovment.finish == false);
        Debug.Log("Anim start");
        myAnim["trailSpiral"].speed = 1f;

        //num_phase = 1;
        //waitFinish = true;
        yield return new WaitUntil(predicate: () => playerMovment.finish == true);
        myAnim["trailSpiral"].speed = 0f;
        Debug.Log("Anim pause");
        animCHeckerOn = false;
        //waitFinish = false;
        //num_phase = 2;
        //stop_ended_well = true;
    }*/

}
