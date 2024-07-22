using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public class deleteCloudData : MonoBehaviour
{
    // deleteAccAlreadyDone

    public bool testModeOn = false;

    public Animation menuAnim;
    public waitDeleteAccount waitDeleteAcc;
    public void btnDeleteData()
    {
        if (!testModeOn)
        {
            // CHECK si internet -> afficher pas de co si essaie supprimer compte
            StartCoroutine(launcher.checkInternetConnection((isConnected) =>
            {
                if (!isConnected)
                {
                    sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
                    menuAnim.Play("privacyNoCo");
                }
                else
                {
                    sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
                    menuAnim.Play("clickDeleteData");
                }
            }));
        }
        else
        {
            sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
            menuAnim.Play("clickDeleteData");
        }
    }

    public void btnDenyDeleteData()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        menuAnim.Play("clickDenyDeleteData");
    }

    public void btnValidDeleteData()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        menuAnim.Play("clickValidDeleteData");
        waitDeleteAcc.openMenu(testModeOn);
        if (testModeOn)
        {
            StartCoroutine(test_deleteAccounts());
        }
        else
        {
            deleteDataAsync();
        }
    }
    private IEnumerator test_deleteAccounts()
    {
        yield return new WaitForSeconds(0.5f);

        waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteUnityCloud, typeResultDeleteAccount.result_success);
        yield return new WaitForSeconds(0.2f);
        waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataUnityCloud, typeResultDeleteAccount.result_noData);
        yield return new WaitForSeconds(0.7f);
        waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteFirestore, typeResultDeleteAccount.result_success);
        yield return new WaitForSeconds(0.4f);
        waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataFirestore, typeResultDeleteAccount.result_error);
        yield return new WaitForSeconds(0.4f);       
    }

    // quand menu wait delete closed, reset ce menu
    public void waitDeleteAccountEnded()
    {
        menuAnim.Play("clickDenyDeleteData");
    }



    private async void deleteDataAsync()
    {
        await Task.Delay(500);// en milisecondes

        // 1 : Si est connecté unity Cloud -> supprimer les datas
        if (dataInCloudManager.isConnectedToAnAccount)
        {
            bool succed = true;
            try
            {
                await AuthenticationService.Instance.DeleteAccountAsync();
            }
            catch (Exception e)
            {
                // Perform cleanup here.
                waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteUnityCloud, typeResultDeleteAccount.result_error);
                waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataUnityCloud, typeResultDeleteAccount.result_error);
                succed = false;
                // pas de return ici, on doit aussi forcer deleteFirestore ( qui est à la suite )
            }
            if (succed)
            {
                waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteUnityCloud, typeResultDeleteAccount.result_success);
                waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataUnityCloud, typeResultDeleteAccount.result_success);

                //dataInCloudManager.playerDeletedUnityAccount();
            }
        }
        else
        {
            waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteUnityCloud, typeResultDeleteAccount.result_noConnexion);
            waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataUnityCloud, typeResultDeleteAccount.result_noConnexion);
        }

        // 2 : Si est connecté Firestore -> supprimer les datas
        if(onlineRankingManager.localPlayerIdInBdd != "")
        {
            await onlineRankingManager.getInstance().deletePlayerAccount();
        }
        else
        {
            waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_compteFirestore, typeResultDeleteAccount.result_noConnexion);
            waitDeleteAcc.aRequestEnded(typeWaitDeleteAccount.wait_dataFirestore, typeResultDeleteAccount.result_noConnexion);
        }
    }

    // PROCESS de suppresion de données

    // menu sombre avec img wait

    // d'abord, supprimer UnityCloud:data avec requête qui supprimer infos
    // ensuite requête pour supprimer UnityCloud:compte

    // puis supprimer les infos de player dans Firestore
}
