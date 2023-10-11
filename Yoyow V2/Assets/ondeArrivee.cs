using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ondeArrivee : MonoBehaviour
{
    private static ondeArrivee instance;
    public static void animOndeArrivee()
    {
        if (instance == null)
            instance = GameObject.Find("animOnde_win").GetComponent<ondeArrivee>();
        instance.GetComponent<Animation>().Play("ondeWinLevel");
    }
}
