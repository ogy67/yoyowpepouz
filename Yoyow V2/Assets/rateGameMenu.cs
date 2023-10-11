using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rateGameMenu : MonoBehaviour
{
    public bool freeGemAlreadyTaken; // gérer avec save memoire

    private bool dataLoaded = false;
    private void loadData()
    {
        if (!dataLoaded)
        {
            freeGemAlreadyTaken = (bool)SaveSystem.loadData(typeSave.PARAM_FREE_GEM_TAKEN);
            dataLoaded = true;
        }
    }

    public Animation animBtnRate;
    public void btnRateGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.Yoyow");
        animBtnRate.Play("btnRateGame");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
    }

    public float timeWaitBlast = 15 / 60f;
    public float timeWaitAfterBlast = 1f;
    public int nbFreeGem = 20;
    public Animation animGetFreeGem;
    public GameObject btnQuitMenuGo;
    public Transform posPopGemTr;
    public void btnGetFreeGem()
    {
        if (!freeGemAlreadyTaken)
        {
            freeGemAlreadyTaken = true;
            SaveSystem.Save(typeSave.PARAM_FREE_GEM_TAKEN, true);
            animGetFreeGem.Play("getRewardGem");
            StartCoroutine(giveGemLate());
            sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_getReward);
        }
    }
    private IEnumerator giveGemLate()
    {
        btnQuitMenuGo.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeWaitBlast);
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, 30, posPopGemTr.position);
        yield return new WaitForSeconds(timeWaitAfterBlast);
        animMenuRate.Play("switchMenuRateGame");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.switchToRate);
    }

    public Animation animMenuRate;
    public Animation btnLikeAnim;
    public GameObject sectionFreeGem, sectionRateGame;
    public void btnOpenMenuRate()
    {
        loadData();
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        sectionFreeGem.SetActive(!freeGemAlreadyTaken);
        sectionRateGame.SetActive(freeGemAlreadyTaken);
        animMenuRate.Play("openMenuRateGame");
        btnLikeAnim.Play("btnLikeClicked");
    }

    public void btnCloseMenuRate()
    {
        animMenuRate.Play("closeMenuRateGame");
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_close);
    }

}
