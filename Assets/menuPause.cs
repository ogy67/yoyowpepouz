using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuPause : MonoBehaviour
{
    /* Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    private static menuPause instance;
    public static menuPause getInstance()
    {
        if (instance = null)
            instance = GameObject.Find("menuPauseIngameCanvas").GetComponent<menuPause>();
        return instance;
    }

    public Animation animMenu;
    public menuGameOver menuGameOverM; // permet de faire les anims de zoom
    public manetteManager manetteM;
    private bool btnPauseIsInPauseMode = true; // false : play mode
    public static bool btnPauseVisible = false;
    // enabled
    public void menuPauseIsEnabled()
    {
        //if (TutoManager.tutoOn)
        //    return;
        // 1.
        // ouvrir voyant btn pause
        animMenu.Play("menuPauseOn");
        btnPauseIsInPauseMode = true;
        btnPauseVisible = true;
    }
    // disabled
    public void menuPauseDisabled()
    {
        //if (TutoManager.tutoOn)
        //    return;
        // 1.
        // fermer voyant btn pause
        btnPauseVisible = false;
        animMenu.Play("menuPauseOff");
    }

    public void btnChangeStateGame() // switch entre play et pause
    {
        if (btnPauseIsInPauseMode)
            btnPause();
        else
            btnPlay();

        // zoomer le level
        menuGameOverM.zoomCamera(!btnPauseIsInPauseMode);
        // activer desactiver player, en fonction si continue jouer

        // activer desactiver btn manette ?


        btnPauseIsInPauseMode = !btnPauseIsInPauseMode;
    }

    // clicked
    private void btnPause()
    {
        // 1.
        // ouvrir menu pause
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.pause_open);
        animMenu.Play("menuPauseOpen");
        manetteM.manetteDisabled();

        animFond3DGame.getInstance().menuPauseOpen();
        //managerAnimFond3D.getInstance().menuPauseOn();
        // 2. 
        // player stop
        // level zoom
    }
    // btn play
    private void btnPlay()
    {
        // 1.
        // fermer menu pause
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.pause_close);
        animMenu.Play("menuPausePlay");
        manetteM.manetteEnabled();

        animFond3DGame.getInstance().menuPauseClose();
        //managerAnimFond3D.getInstance().menuPauseOff();
        // player spawn
        // level zoom
    }
    // btn quit
    public void btnQuit()
    {
        // 1.
        // fermer menu pause
        // anim fermeture mode jeu
        sfxGame.getInstance().playSfx_listIngame(typeSfx_ingame.pause_quit);
        animMenu.Play("menuPauseQuit");
        btnPauseVisible = false;

        // 2.
        // level zoom
        menuGameOverM.zoomCamera(true);
        StartCoroutine(quitLate());
    }

    private IEnumerator quitLate()
    {
        //managerAnimFond3D.getInstance().menuPauseQuit();
        yield return new WaitForSeconds(0.4f);
        appliManager.instance.quitGame();
    }
}
