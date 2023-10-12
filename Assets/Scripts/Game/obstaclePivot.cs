using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstaclePivot : MonoBehaviour
{
    public Transform posVisee;
    private LineRenderer lineVisee;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnDisable()
    {
        lineVisee.positionCount = 0;
    }
    private void OnEnable()
    {
        if (lineVisee == null)
        {
            lineVisee = gameObject.GetComponent<LineRenderer>();
        }

        lineVisee.positionCount = 2;
    }

    private void LateUpdate()
    {
        Vector3 back = new Vector3(0, 0, 1);
        lineVisee.SetPosition(0, transform.position + back);
        lineVisee.SetPosition(1, posVisee.position + back);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player_ghost")
        {
            pushGhost(collision.transform);
        }
        if (collision.tag == "Player" && !playerMovment.firstCheckP)
        {
            if(!isPushing)
                StartCoroutine(pushPlayer(getPlayer()));
        }
    }

    private static playerMovment _player;
    private static playerMovment getPlayer()
    {
        if(_player == null)
        {
            _player = boostIngameManager.getInstance().playerMov;
        }
        return _player;
    }

    public static bool isPushing = false;

    private void pushGhost(Transform g)
    {
        if (gameObject.tag == "ObstaclePivot")
            GetComponent<Animation>().Play("pivotPush");
        else
            GetComponent<Animation>().Play("pivotPushRond");

        g.GetComponent<playerGhost>().pivotPush(posVisee.position - transform.position);
    }
    private IEnumerator pushPlayer(playerMovment p)
    {
        isPushing = true;
        //cameraGame.getInstance().pivotCamEffect();
        if(gameObject.tag == "ObstaclePivot")
            GetComponent<Animation>().Play("pivotPush");
        else
            GetComponent<Animation>().Play("pivotPushRond");
        yield return new WaitForSeconds(3f/60f);
        p.pivotPush(posVisee.position - transform.position);
        yield return new WaitForSeconds(5f / 60f);
        isPushing = false;
    }
}
