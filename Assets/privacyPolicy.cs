using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class privacyPolicy : MonoBehaviour
{
    public void btnPrivacyPolicy()
    {
        StartCoroutine(launcher.checkInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                Application.OpenURL("https://yoyow.flycricket.io/privacy.html");
            }
        }));
    }
}
