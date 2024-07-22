using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ondeSuperB : MonoBehaviour
{
    private static ondeSuperB instance;
    public static void animOndeSuperB()
    {
        if (instance == null)
            instance = GameObject.Find("animOnde_superB").GetComponent<ondeSuperB>();
        instance.GetComponent<Animation>().Play("ondeSuperB");
    }
}
