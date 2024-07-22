using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class expertMode : MonoBehaviour
{
    public static bool modeExpertOn = false;
    public cameraGame cam;

    public Transform posVoyantOnOff;
    public Transform voyantOnOffTr;
    private bool posVoyantInitialized = false;

    public static bool menuStarOpen = false;
    // appelé par le voyant etoile quand clique dessus
    //private int prevStar = -1;
    public void btnVoyantStarClicked()
    {
        if (!posVoyantInitialized)
        {
            posVoyantInitialized = true;
            voyantOnOffTr.position = posVoyantOnOff.position;
        }

        if (initAppForceExpert)
        {
            initAppForceExpert = false;
            menuAnim.Play("openMenuStar_forceStar");
        }
        else
        {
            menuAnim.Play("openMenuStar");
        }

        menuStarOpen = true;
        //int nbStar= SaveSystem.getNbStarObtain();
        //nbStarText.text = "<color=white>" + nbStar + "</color><size=23>/" + SaveSystem.nbNiveauAventure + "</size>";
        //nbStarText_ombre.text = nbStar + "<size=23>/" + SaveSystem.nbNiveauAventure + "</size>";
    }

    // appelé par btn close menu star
    public void btnCloseMenuStar()
    {
        menuAnim.Play("closeMenuStar");
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);
        menuStarOpen = false;
    }

    public Animation menuAnim;
    //public static bool modeStarOn = false;
    public void btnSwitchMode()
    {
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.switch_star);

        if (modeExpertOn)
        {
            menuAnim.Play("btnSwitchToNormal");
        }
        else
        {
            menuAnim.Play("btnSwitchToStar");
        }
        modeExpertOn = !modeExpertOn;
        SaveSystem.Save(typeSave.MODE_EXPERT_ON, modeExpertOn);
        cam.modeExpertSwitched(modeExpertOn);
    }

    private bool initAppForceExpert = false;
    public void tryOpenExpertOpenApp()
    {
        // si était en mode expert avant d'ouvrir l'app
        // réouvrir en mode expert
        // bool modeExpertWasOpenLastOpenApp = (bool)SaveSystem.loadData(typeSave.MODE_EXPERT_ON);


        if ((bool)SaveSystem.loadData(typeSave.MODE_EXPERT_ON))
        {
            modeExpertOn = true;
            // anim forceOpenStar quand open
            initAppForceExpert = true;
            // changer couleure fond app
            cam.forceSetExpertMode_initApp();
        }
    }
}
