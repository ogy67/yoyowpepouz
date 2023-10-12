using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackerPlayerStuck : MonoBehaviour
{
    private Vector3 prevPosPlayer;

    /*private bool test_keyStuckOn = false;
    // Update is called once per frame
    void Update()
    {
        if (isSecondTracker)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (!test_keyStuckOn)
                    playerIsStuck();
            }
        }
    }*/

    public Rigidbody2D playerRB2D;

    public static bool isTrackingPlayer = false;
    public void playerPopGame()
    {
        prevPosPlayer = transform.position;
        listPreviousSpeed = new List<float>();
        isTrackingPlayer = true;


        StartCoroutine(trackPlayer());
    }

    // pierreT | gameManager.die(), appelle this.playerDieGame(), puis check si pierreT
    public void playerDieGame()
    {
        isTrackingPlayer = false;
    }

    public void playerUseBombe()
    {
        isTrackingPlayer = false;
    }

    public void playerWinGame()
    {
        isTrackingPlayer = false;
    }


    private static int trackerId = 0; // eviter double routine
    private IEnumerator trackPlayer()
    {
        float prevTime = Time.time;
        float frameTime;
        float speed;

        trackerId++;
        int myTrackerId = trackerId;

        while (isTrackingPlayer
            && myTrackerId == trackerId)
        {
            yield return new WaitForFixedUpdate();
            if (myTrackerId == trackerId)
            {
                frameTime = Time.time - prevTime;
                speed = Vector2.Distance(transform.position, prevPosPlayer) / frameTime;

                prevPosPlayer = transform.position;
                prevTime = Time.time;

                registerSpeed_2(speed);
            }
        }
    }

    private List<float> listPreviousSpeed = new List<float>();
    public int nbFrameTooSlow_playerStuck = 30;
    public float averageSpeedBug = 0.25f;
    public float test_valueAverage = 0f;
    private float currentSumList = 0f;
    public trackerPlayerStuck tracker_2;
    private void registerSpeed_2(float speed)
    {
        if (listPreviousSpeed.Count == 0)
        {
            listPreviousSpeed.Add(speed);
            currentSumList = speed;
            test_valueAverage = 0f;
        }
        else
        {
            if (listPreviousSpeed.Count < nbFrameTooSlow_playerStuck)
            {
                listPreviousSpeed.Add(speed);
                currentSumList += speed;
            }
            else
            {
                currentSumList -= listPreviousSpeed[0];
                listPreviousSpeed.RemoveAt(0);
                listPreviousSpeed.Add(speed);
                currentSumList += speed;

                test_valueAverage = currentSumList / nbFrameTooSlow_playerStuck;
                if (test_valueAverage < averageSpeedBug)
                {
                    Debug.Log("<color=red> PLAYER IS STUCK</color>");
                    tracker_2.playerIsStuck();
                }
            }
        }
    }

    // FONCTIONS pour 2e tracker (tjr actif)

    [Header("SECOND TRACKER")]
    public menuPlayerStuck menuStuck;
    public playerMovment pM;
    public bool isSecondTracker = false;
    private void playerIsStuck()
    {
        /*test_keyStuckOn = true;
        Debug.Log("stuck - UNPOP PLAYER");*/
        isTrackingPlayer = false;

        // depop player

        pM.forceDie_stuck();
        menuStuck.openMenuStuck();
    }

    public gameManager gM;
    public infinisManager iM;
    public void rePopPlayer()
    {
        /*test_keyStuckOn = false;
        Debug.Log("stuck - REPOP PLAYER");*/
        // repop player

        gM.unstuckPlayer();

        if (infinisManager.infinis_Mode_On)
        {
            iM.StartCoroutine(iM.reactiveBtnChangeRot(false, false));
        }
    }
}
