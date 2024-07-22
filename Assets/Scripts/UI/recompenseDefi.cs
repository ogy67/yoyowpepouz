using System.Collections;
using UnityEngine;

public class recompenseDefi : MonoBehaviour
{
    public recompense typeRec;
    //private static float speed = 1000f;
    public Transform target;

    public void spawn(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        gameObject.GetComponent<Animation>().Play("pieceRecPop");
        StartCoroutine(destroyLate());
    }

    public static bool forStar = false;
    private IEnumerator destroyLate()
    {
        //if (!TutoManager.tutoOn)
        //{
            if (managerNiveau.gameOn && !forStar) // ingame
            {

                if (typeRec == recompense.VIE)
                {
                    gameManager.getInstance().giveRecompense(recompense.VIE, true);
                    yield return new WaitForSeconds(0.5f);
                    //sfxManager.getInstance().playSound_ingame(typeSound_ingame.getLife);
                }
                else
                {
                    if (infinisManager.infinis_Mode_On)
                    {
                        gameManager.getInstance().pickUpYowInfinis(false);
                    }
                    else
                    {
                        //Debug.Log("GIVE MONNEY recompenseDefi");
                        gameManager.getInstance().giveRecompense(recompense.ARGENT, false);
                    }
                }
                yield return new WaitForSeconds(4f / 6f);
            }
            else // menu defi
            {
                yield return new WaitForSeconds(4f / 6f);
                if (typeRec == recompense.ARGENT && !managerNiveau.gameOn)
                    gameManager.getInstance().giveRecompense(recompense.ARGENT, false);
                else if (typeRec == recompense.GEMME && !managerNiveau.gameOn)
                    gemmesManager.getInstance().addGemme(false);
                /*else if (typeRec == recompense.TICKET_ONLINE && !managerNiveau.gameOn)
                {
                    ticketOnlineManager.nbTickets++;
                    ticketOnlineManager.getInstance().setCompteurTicketMenuOnline();
                }*/
            }
        /*}
        else // tutoriel
        {
            yield return new WaitForSeconds(4f / 6f);
        }*/
        Destroy(gameObject);
    }
}
