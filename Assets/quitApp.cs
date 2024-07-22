using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quitApp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static bool quitAppOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (quitAppOpen)
            return;
        // si gameOn = false
        if (managerNiveau.gameOn 
            || shopManager.menuShopOpen 
            || defiManager.menuDefiOpen 
            || paramManager.menuParamOpen
            || onlineMenuPreview.onlinePreviewOpen
            )
            return;

        if (Input.GetKeyDown(KeyCode.Escape))// || Input.GetKeyDown(KeyCode.Return))
        {
            // ouvrir le menu quit
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
            GetComponent<Animation>().Play("quitAppOpen");
            quitAppOpen = true;
        }
    }

    public Animation animBtnValidQuit, animBtnDenyQuit;
    public void btnValidQuit()
    {
        // fermer l'app
        lockClickOnQuit.SetActive(true);
        animBtnValidQuit.Play("btnQuitClick");
        StartCoroutine(closeGameLate());
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);
    }

    public GameObject lockClickOnQuit;
    private IEnumerator closeGameLate()
    {
        yield return new WaitForSeconds(2 / 3f);
        Application.Quit();
    }

    public void btnNoQuit()
    {
        // anim ferme menu
        GetComponent<Animation>().Play("quitAppClose");
        quitAppOpen = false;
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        //sfxManager.getInstance().playSound_menuP(typeSound_menuP.quitApp_cancel);
        animBtnDenyQuit.Play("btnQuitClick");
    }

    public void btnNoQuit_background()
    {
        GetComponent<Animation>().Play("quitAppClose");
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        quitAppOpen = false;
    }
}
