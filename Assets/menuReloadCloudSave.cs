using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuReloadCloudSave : MonoBehaviour
{
    public Animation myAnim;
    public void openMenu()
    {
        myAnim.Play("openMenuReloadAccount");
    }



    public dataInCloudManager cloudManager;

    public void btnYes()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        myAnim.Play("btnYesMenuReloadAccount");
        //btnYesMenuReloadAccount
        cloudManager.btnReloadData(true);
    }

    public void btnNo()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        // ouvrir 2e bouton
        myAnim.Play("btnNoMenuReloadAccount");
    }

    public void btnNo_confirm()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);

        myAnim.Play("btnNoValidMenuReloadAccount");
        cloudManager.btnReloadData(false);
    }   
}
