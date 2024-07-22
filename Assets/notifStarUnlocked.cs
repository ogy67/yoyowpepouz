using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notifStarUnlocked : MonoBehaviour
{
    //public bool test_openNotifStar = false;


    public Animation notifAnim;
    private static notifStarUnlocked instance;
    public static notifStarUnlocked getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("animStarUnlocked").GetComponent<notifStarUnlocked>();
        return instance;
    }

    public static bool waitingEndPhaseGame = false;
    public void playerUnlockedStar()
    {
        waitingEndPhaseGame = true;
    }

    public void endPhaseGame()
    {
        if (waitingEndPhaseGame)// || test_openNotifStar)
        {
            waitingEndPhaseGame = false;
            StartCoroutine(animEnum());
        }
    }

    public float time_reloadNextNotif = 115f;
    public float timeWaitSoundNotif = 20f;
    //public float timeDelayDefiNotif = 0.15f;
    public static bool isAnimating = false;
    private IEnumerator animEnum()
    {
        isAnimating = true;
        int idNotif = notifDefiDone.changeAnimNotifState(true, -1);

        //yield return new WaitForSeconds(timeDelayDefiNotif);
        notifAnim.Play("infinisNotif");

        yield return new WaitForSeconds(timeWaitSoundNotif / 60f);
        sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.notifPop);

        float t_left = (time_reloadNextNotif / 60f) - (timeWaitSoundNotif / 60f);
        yield return new WaitForSeconds(t_left);
        notifDefiDone.changeAnimNotifState(false, idNotif);
        isAnimating = false;

        gameManager.waitingEndAnimUnlockStar = false;
    }
}
