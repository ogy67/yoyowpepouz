using UnityEngine;

public class btnMenuPAnim : MonoBehaviour
{
    public Animation myAnim;
    public void btnMenuP_clicked()
    {
        if (!managerNiveau.gameOn && !notifDefiDone.isAnimatingNotif)
        {
            //Debug.Log("Btn shop clicked, time : "+Time.time);
            myAnim.Play("btnMenu_click");
        }
    }
}
