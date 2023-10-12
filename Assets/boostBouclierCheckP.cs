using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostBouclierCheckP : MonoBehaviour
{
    private bool boostBouclier_isActivated = false;
    public void boostBouclier_activated()
    {
        boostBouclier_isActivated = true;
    }

    public void boostBouclier_desactivated()
    {
        boostBouclier_isActivated = false;
        if (isHangingCheckP)
        {
            isHangingCheckP = false;
            setPlayerCollider(false);
        }
    }


    private bool isHangingCheckP = false;
    public void hangCheckP()
    {
        if (boostBouclier_isActivated)
        {
            isHangingCheckP = true;
            setPlayerCollider(true);
        }
    }

    public void releaseCheckP()
    {
        if (boostBouclier_isActivated)
        {
            isHangingCheckP = false;
            setPlayerCollider(false);
        }
    }

    public Collider2D playerCollider;
    public Collider2D sourisCollider;
    private void setPlayerCollider(bool isTrigger)
    {
        // de/activer colliders player et souris
        playerCollider.isTrigger = isTrigger;
        sourisCollider.isTrigger = isTrigger;
    }
}
