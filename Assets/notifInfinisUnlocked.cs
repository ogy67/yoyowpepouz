using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notifInfinisUnlocked : MonoBehaviour
{
    public bool test_openNotifInfinis = false;


    public Animation notifAnim;
    private static notifInfinisUnlocked instance;
    public static notifInfinisUnlocked getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("animInfinisUnlocked").GetComponent<notifInfinisUnlocked>();
        return instance;
    }

    private bool waitingEndPhaseGame = false;
    public void playerUnlockedInfinis()
    {
        waitingEndPhaseGame = true;
    }

    public void endPhaseGame()
    {
        if (waitingEndPhaseGame || test_openNotifInfinis)
        {
            waitingEndPhaseGame = false;
            StartCoroutine(animEnum());
        }
    }

    public float time_reloadNextNotif = 115f;
    public float timeWaitSoundNotif = 20f;
    public float timeDelayDefiNotif = 0.15f;
    public static bool isAnimating = false;
    private IEnumerator animEnum()
    {
        isAnimating = true;
        int idNotif = notifDefiDone.changeAnimNotifState(true, -1);

        yield return new WaitForSeconds(timeDelayDefiNotif);
        notifAnim.Play("infinisNotif");

        yield return new WaitForSeconds(timeWaitSoundNotif / 60f);
        sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.notifPop);

        float t_left = (time_reloadNextNotif/60f) -
            (timeDelayDefiNotif + (timeWaitSoundNotif / 60f)); 
        yield return new WaitForSeconds(t_left);
        notifDefiDone.changeAnimNotifState(false, idNotif);
        isAnimating = false;
    }
}
