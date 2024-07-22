using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contentTrailGame : MonoBehaviour
{
    private static contentTrailGame instance;
    public static contentTrailGame getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("contentTrail").GetComponent<contentTrailGame>();
        return instance;
    }

    public GameObject currentGoTrail;
    public void playerPassThrewPortal()
    {
        GameObject o = Instantiate(currentGoTrail, transform);
        currentGoTrail.transform.SetParent(null);
        Destroy(currentGoTrail.GetComponent<followTarget>());
        StartCoroutine(destroyTrailLate(currentGoTrail));

        currentGoTrail = o;
        o.transform.localPosition = new Vector3(0, 0, 0);
    }

    private IEnumerator destroyTrailLate(GameObject o)
    {
        yield return new WaitForSeconds(2f);
        Destroy(o);
    }

    public void clearTrail()
    {
        currentGoTrail.GetComponent<trailManager>().clearTrail_child();
    }

    public trailManager getCurrentTrail()
    {
        return currentGoTrail.GetComponent<trailManager>();
    }


    public void new_game_started()
    {
        getCurrentTrail().new_game_started();
    }



    public void infinisStartSwitchLevel()
    {
        currentGoTrail.gameObject.SetActive(false);
    }

    public void playerStartPop()
    {
        currentGoTrail.gameObject.SetActive(false);
    }

    public void playerEndPop(Vector2 posSpawn)
    {
        currentGoTrail.gameObject.SetActive(true);
        currentGoTrail.transform.position = posSpawn;
        clearTrail();
    }

    public void playerDesactivated()
    {
        clearTrail();
        currentGoTrail.gameObject.SetActive(false);
    }
}
