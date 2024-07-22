using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class speedProgBar : MonoBehaviour
{

    public int r = 0; // max : 7
    public GameObject[] items;
    public Color colorItemOff, colorItemOn, colorItemSelected;

    public void setBarValues(int vitesse) // vitesse : valeure du rang de vitesse
    {
        r = vitesse;

        foreach(GameObject g in items)
        {
            int r2 = g.transform.GetSiblingIndex();

            g.GetComponent<Image>().color = r2 == vitesse ?
                colorItemSelected : r2 < vitesse ?
                colorItemOn : colorItemOff;
        }
    }

    // changement de vitesse : ouvrir barre si pas ouverte
    // après dernier changement : attendre 1s avant de refermer

    private float timeBarOpen = 2.5f, currentTime;
    private bool barOpen = false, changingState = false;

    public void changeVal(bool Up, int vitesse)
    {
        StartCoroutine(changeBarValue(Up, vitesse));
    }
    private IEnumerator changeBarValue(bool Up, int vitesse)
    {
        currentTime = timeBarOpen;
        if (!barOpen)
        {
            setBarValues(vitesse);
            StartCoroutine(openBar());
            yield break;
        }
        // /!\ Gerer quand change value en meme temps que l'anim d'ouverture
        while (changingState == true)
            yield return null;

        if (!changingValueAnim)
            StartCoroutine(animChangeValue());
        currentWaitChangeTime = timeWaitAnimChange;

        // si r < 7 et Up : monter 
        // si r > 0 et down : descendre
        if( Up && r < 7 || !Up && r > 0 )
        {
            int r_prev = r, r_change = Up ? 1 : -1;
            r += r_change;
            yield return new WaitForSeconds(8f / 60f);

            items[r_prev].GetComponent<Animation>().Play(
                Up ? "itemSpeed_selected_to_on" : "itemSpeed_selected_to_off");
            items[r_prev + r_change].GetComponent<Animation>().Play(
                Up ? "itemSpeed_off_to_selected" : "itemSpeed_on_to_selected");
        }
    }

    // bar vitesse rester ouverte tant que changement
    private IEnumerator openBar()
    {
        barOpen = true;

        changingState = true;
        GetComponent<Animation>().Play("progSpeedOpen");
        yield return new WaitForSeconds(8f / 60f); // temps anim ouverture
        changingState = false;

        // on attend que tout les changements ont été effectués
        while (currentTime > 0f)
        {
            yield return null;
            currentTime -= Time.deltaTime;
        }

        // fermer la barre
        barOpen = false;
        GetComponent<Animation>().Play("progSpeedClose");
    }

    // anim changement : divisée en 2 anim, " grossit " " rappeticit "
    //   grossit et le reste tant que changements
    //   rapeticit quand fin changement

    // grossir tant que changement de vitesse, petit quand stop
    float timeWaitAnimChange = 2 / 6f, currentWaitChangeTime;
    bool changingValueAnim = false;
    private IEnumerator animChangeValue()
    {
        changingValueAnim = true;
        // anim ouverture
        GetComponent<Animation>().Play("progSpeedChangeBig");
        yield return new WaitForSeconds(1 / 6f); // grossit

        // tant que changement : reste "grossit"
        while (currentWaitChangeTime > 0)
        {
            currentWaitChangeTime -= Time.deltaTime;
            yield return null;
        }

        // anim fermeture
        changingValueAnim = false;
        GetComponent<Animation>().Play("progSpeedChangeLit");
        yield return new WaitForSeconds(1 / 6f); // rapeticit
        
    }
}
