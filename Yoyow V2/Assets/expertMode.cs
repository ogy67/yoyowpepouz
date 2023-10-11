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

    /*public Animation switchBtnAnim;

    public void btnSwitchModeExpert()
    {
        if (animSwitch)
            return;
        modeExpertOn = !modeExpertOn;
        switchBtnAnim.Play(modeExpertOn ? "btnModeExpertOn" : "btnModeExpertOff");
        cam.modeExpertSwitched(modeExpertOn);
        StartCoroutine(animSwitch_enum());
    }

    public static bool animSwitch = false;
    public float timeAnimSwitch = 20f / 60f;
    private IEnumerator animSwitch_enum()
    {
        animSwitch = true;
        yield return new WaitForSeconds(timeAnimSwitch);
        animSwitch = false;
    }*/


    //public Text nbStarText, nbStarText_ombre;


    public static bool menuStarOpen = false;
    // appelé par le voyant etoile quand clique dessus
    //private int prevStar = -1;
    public void btnVoyantStarClicked()
    {
        menuAnim.Play("openMenuStar");
        if (!posVoyantInitialized)
        {
            posVoyantInitialized = true;
            voyantOnOffTr.position = posVoyantOnOff.position;
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

        cam.modeExpertSwitched(modeExpertOn);
    }
}
