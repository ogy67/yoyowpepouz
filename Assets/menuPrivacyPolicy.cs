using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuPrivacyPolicy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Animation noCoPrivacyAnim;
    public Animation mailCopiedAnim;
    public Animation menuAnim;
    public Animation btnMenuAnim;

    public void btnOpenPrivacyMenu()
    {
        // animation ouverture menu
        menuAnim.Play("openPrivacyMenu");
        btnMenuAnim.Play("btnParamClicked");

        // son btn
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }

    public void btnClosePrivacyMenu()
    {
        // animation fermeture menu
        menuAnim.Play("closePrivacyMenu");
        // son btn close
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_close);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.btn_close);
    }

    public void btnMail()
    {
        ClipboardExtention.CopyToClipboard("ogyappstudio@gmail.com");
        // animation text copié
        mailCopiedAnim.Play("mailCopied");

        // son btn
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }

    //private bool noCoOn = false;

    public void btnPrivacyLink()
    {
        // son btn
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);

        StartCoroutine(launcher.checkInternetConnection((isConnected) =>
        {
            if(isConnected)
            {
                Application.OpenURL("https://yoyow.flycricket.io/privacy.html");
            }
            else
            {
                // animation pas de connexion internet
                noCoPrivacyAnim.Play("noCoPrivacy");
            }
        }));

        /*if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Application.OpenURL("https://yoyow.flycricket.io/privacy.html");
        }
        else
        {
            // animation pas de connexion internet
            noCoPrivacyAnim.Play("noCoPrivacy");
        }*/

        
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.param_onOffBtn);
    }
}

public static class ClipboardExtention
{
    public static void CopyToClipboard(this string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}
