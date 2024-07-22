using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notifDefiDone : MonoBehaviour
{
    public Animation animNotif;
    private List<typeDefi> listWaitingNotif = new List<typeDefi>();

    // les defis appellent d'eux même quand sont finis
    // appelé ingame
    // endGame 
    public void newDefiIsUnlocked(typeDefi type)
    {
        // on save les infos à afficher, pour quand retour au menuP
        //Debug.Log("<color=green>New defi is unlocked</color> (will show notif"+type+")");

        if (!listWaitingNotif.Contains(type))
        {
            listWaitingNotif.Add(type);
        }
    }





    public static bool isAnimatingNotif = false;
    // une fois toute values mit à jour, on appelle la fonction d'affichage des notif
    public void showAllNotif()
    {
        if (listWaitingNotif.Count != 0)
        {
            // animation open notif
            StartCoroutine(animMultipleNotif());
        }
    }

    public float time_reloadNextNotif = 115f;
    private IEnumerator animMultipleNotif()
    {
        //isAnimatingNotif = true;
        int idNotif = changeAnimNotifState(true, -1);
        //Debug.Log("Anim multiple notif, nombre de notif : " + listWaitingNotif.Count);
        foreach(typeDefi t in listWaitingNotif)
        {
            StartCoroutine(animSoundLate());
            fillInfoNotif(t);
            if (animNotif.isPlaying)
                animNotif.Rewind("defiNotif");
            else
                animNotif.Play("defiNotif");
            yield return new WaitForSeconds(time_reloadNextNotif / 60f);
        }
        listWaitingNotif = new List<typeDefi>();

        //isAnimatingNotif = false;
        changeAnimNotifState(false, idNotif);
    }

    public float timeWaitSoundNotif = 20f;
    private IEnumerator animSoundLate()
    {
        yield return new WaitForSeconds(timeWaitSoundNotif / 60f);
        sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.notifPop);
    }

    private Dictionary<typeDefi, int> indexImgDefi = new Dictionary<typeDefi, int>()
    {
        {typeDefi.crown, 0 },
        {typeDefi.star, 1 },
        {typeDefi.infinis, 2 },
        {typeDefi.checkP, 3 },
        {typeDefi.pivot, 4 },
        {typeDefi.bounce, 5 },
        {typeDefi.portail, 6 },
        {typeDefi.boost_bombe, 7 },
        {typeDefi.boost_bouclier, 8 },
        {typeDefi.boost_superVisee, 9 },
        {typeDefi.boost_ressort, 10 },
        {typeDefi.boost_fantome, 11 },
        {typeDefi.boost_souris, 12 },
        {typeDefi.boost_pierreTombale, 13 },
    };
    public Transform imgDefiContent;
    private int prevIndexOpenned = 0;
    private void fillInfoNotif(typeDefi type)
    {
        imgDefiContent.GetChild(prevIndexOpenned).gameObject.SetActive(false);
        prevIndexOpenned = indexImgDefi[type];
        imgDefiContent.GetChild(prevIndexOpenned).gameObject.SetActive(true);
    }




    private static List<int> idsNotif = new List<int>();
    public static int changeAnimNotifState(bool open, int idClose)
    {
        // renvoit id notif
        int ret_id;
        if (open)
        {
            do
            {
                ret_id = Random.Range(0, 1000);
            } while (idsNotif.Contains(ret_id));

            idsNotif.Add(ret_id);
            isAnimatingNotif = true;
        }
        else
        {
            idsNotif.Remove(idClose);
            if (idsNotif.Count == 0)
                isAnimatingNotif = false;

            ret_id = -1;
        }
        return ret_id;
    }
}
