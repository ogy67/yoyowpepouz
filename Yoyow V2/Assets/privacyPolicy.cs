using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class privacyPolicy : MonoBehaviour
{
    public void btnPrivacyPolicy()
    {
        if(Application.internetReachability != NetworkReachability.NotReachable)
        {
            Application.OpenURL("https://yoyow.flycricket.io/privacy.html");
        }
    }
}
