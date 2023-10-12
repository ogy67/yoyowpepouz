using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tropheRec : MonoBehaviour
{
    public Transform toGo;
    private static Vector2 toGoPos;

    private float speed;

    public void pop(Vector3 posSpawn)
    {
        gameObject.SetActive(true);
       transform.position = posSpawn;
        //transform.position = posSpawn;
        Debug.Log("Ma position" + transform.position);
        setSpeed();
        if (toGoPos == Vector2.zero)
            toGoPos = toGo.position;
        StartCoroutine(animTr());
    }

    private IEnumerator animTr()
    {
        GetComponent<Animation>().Play("tropheRecPop");
        yield return new WaitForSeconds(0.7f);

        while((Vector2)transform.position != toGoPos)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                   new Vector3(toGoPos.x, toGoPos.y, -7), Time.deltaTime * speed);
            yield return null;
        }

        //menuRankingGameOnline.getInstance().getTrophe();
        Destroy(gameObject);
    }

    public void setSpeed()
    {
        // Arrivera tjr en x seconde a la cible
        float refSpeed = 9f;
        float refDist = 4f;
        // si facteur 2 fois + grand, vitesse 2 fois plus grand
        speed = refSpeed * (Vector3.Distance(transform.position, toGo.position) / refDist);
        speed = speed < 5 ? 5f : speed;
    }
}
