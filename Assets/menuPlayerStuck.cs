using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuPlayerStuck : MonoBehaviour
{
    public trackerPlayerStuck tracker_2;
    public Animation menuAnim;
    public Transform posPopGem;
    public void openMenuStuck()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);

        menuAnim.Play("openMenuPlayerStuck");
    }

    public void btnCloseMenu()
    {
        sfxGame.getInstance().playSfx_listParamMenu(typeSfx_paramMenu.btn_simple);
        // forcer ouvrir compteur gem
        StartCoroutine(animGiveReward());
    }

    private IEnumerator animGiveReward()
    {
        menuAnim.Play("closeMenuPlayerStuck");
        StartCoroutine(soundCloseLate());
        yield return new WaitForSeconds(0.1f);

        sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.clickGetReward);
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, 10, posPopGem.position);
        tracker_2.rePopPlayer();
        yield return new WaitForSeconds(0.7f);

        gameManager.getInstance().giveRecompense(recompense.GEMME, false);
    }

    private IEnumerator soundCloseLate()
    {
        yield return new WaitForSeconds(35 / 60f);
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
    }
}
