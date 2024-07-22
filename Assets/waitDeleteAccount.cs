using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeWaitDeleteAccount
{
    wait_compteUnityCloud,
    wait_dataUnityCloud,
    wait_compteFirestore,
    wait_dataFirestore,
}

public enum typeResultDeleteAccount
{
    result_error,
    result_success,
    result_noData,
    result_noConnexion,
}

[Serializable]
public struct itemWait
{
    public typeWaitDeleteAccount typeWait;
    public Text infoProgText, infoProgText_ombre;
    public GameObject waitImg;

}
public class waitDeleteAccount : MonoBehaviour
{
    public itemWait[] lesItems;
    public Animation menuAnim;
    public deleteCloudData deleteManager;
    private Dictionary<typeWaitDeleteAccount, itemWait> itemWaitDic = new Dictionary<typeWaitDeleteAccount, itemWait>();

    private bool initialized = false;

    private bool testModeOn;
    public void openMenu(bool testMode)
    {
        testModeOn = testMode;

        if (!initialized)
        {
            initialized = true;
            foreach(itemWait wait in lesItems)
            {
                itemWaitDic.Add(wait.typeWait, wait);
            }
        }

        foreach(itemWait wait in lesItems)
        {
            // activer image wait
            wait.waitImg.SetActive(true);
            // desactiver text info
            wait.infoProgText.text = "";
            wait.infoProgText_ombre.text = "";
        }

        currentNbRequestEnded = 0;

        // anim open menu
        // deleteCloudData doit attendre que menu ouvert pour lancer requêtes
        menuAnim.Play("openWaitDeleteAccount");
    }

    public void btnOk()
    {
        // fermer le menu
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        /*menuAnim.Play("closeWaitDeleteAccount");
        deleteManager.waitDeleteAccountEnded();*/

        // -> fermer l'appli
        Application.Quit();
        // ( UnityCloud et Firestore doivent être reset )
        // ex : supp compte et veut ré-ouvrir ranking ( qui avait déjà été ouvert -> ne check pas nouvel account ) 
    }


    private int nbRequestToEnd = 4;
    private int currentNbRequestEnded;

    public void aRequestEnded(typeWaitDeleteAccount typeWait, typeResultDeleteAccount typeRes)
    {
        itemWaitDic[typeWait].infoProgText.text = getResultString_byTypeResult(typeRes, false);
        itemWaitDic[typeWait].infoProgText_ombre.text = getResultString_byTypeResult(typeRes, true);
        itemWaitDic[typeWait].waitImg.SetActive(false);

        currentNbRequestEnded++;
        if(currentNbRequestEnded == nbRequestToEnd) 
        {
            // -> fin
            // ouvrir btn OK
            sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_switchState);
            menuAnim.Play("openBtnOKWaitDeleteAccount");
        }
    }

    private string getResultString_byTypeResult(typeResultDeleteAccount typeRes, bool ombre)
    {
        // demander à traduction
        string trad = traductionManager.getInstance().getTradStatusDeleteAccount(typeRes);
        if (!ombre)
        {
            trad = getColorString_byTypeResult(typeRes) + trad + "</color>";
        }

        return trad;
    }

    public string colorSucced, colorFailed, colorNoData;
    private string getColorString_byTypeResult(typeResultDeleteAccount typeRes)
    {
        return typeRes == typeResultDeleteAccount.result_success ?
            colorSucced : typeRes == typeResultDeleteAccount.result_noData ?
            colorNoData : colorFailed;
    }
}
