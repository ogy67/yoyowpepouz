using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lockInfinis : MonoBehaviour
{
    public int crownRequired = 30;//, recordsInfinisRequired = 15;

    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        instance = gameObject.GetComponent<lockInfinis>();
    }

    private static lockInfinis instance;
    //public static lockInfinis instance_lockOnline; // mit à jour par appliManager

    public static lockInfinis getInstance()
    {
        //if (instance == null)
        //    instance = GameObject.Find("lockInfiniteModePan").GetComponent<lockInfinis>();
        return instance;
    }

    /*public static lockInfinis getInstance_online()
    {
        return instance_lockOnline;
    }*/

    public static bool isUnlocked = false;
    public void checkVerouillage(int nbC, bool initApp)//, bool checkOnline)
    {
        int nbCToUnlock = crownRequired;//checkOnline ? recordsInfinisRequired : crownRequired;

        if (initApp)
        {
            setTextInfo(false, nbCToUnlock);
            if (nbC >= nbCToUnlock)
            {
                gameObject.SetActive(false);
                isUnlocked = true;
            }
        }
        else if(!initApp && nbC == nbCToUnlock)
        {
            isUnlocked = true;
            StartCoroutine(unlockBtnLate());
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.unlock_infinis);
            notifInfinisUnlocked.getInstance().playerUnlockedInfinis();
        }
        // si init app, et que deverouillé, pas d'anim dévérouillage
        // si recoit couronne -> dévérouillage, anim dévérouillage
    }

    public float timeWaitUnlockBtn = 100 / 60f;
    private IEnumerator unlockBtnLate()
    {
        yield return new WaitForSeconds(timeWaitUnlockBtn);
        anim.Play("unlockInfiniteMode");
    }

    private bool infoOpenned = false, switching = false;
    private int switchCount = 0;

    public void btnInfoLock()
    {
        if (!switching)
        {
            switchCount++;

            StartCoroutine(switchInfo(infoOpenned));

            if (!infoOpenned)
                anim.Play("lockInfinisOpenInfos");
            else
                anim.Play("lockInfinisCloseInfos");

            infoOpenned = !infoOpenned;
        }
    }

    private IEnumerator switchInfo(bool open)
    {
        int refe = switchCount;
        yield return new WaitForSeconds(0.5f);
        switching = false;

        if (!open) // Fermeture automatique
        {
            yield return new WaitForSeconds(1.5f);
            if ( switchCount == refe )
            {
                btnInfoLock();
            }
        }
    }

    public Text infoLockText, nbCText;
    public void setTextInfo(bool setOnline, int nbCToUnlock)
    {
        nbCText.text = nbCToUnlock.ToString();

        if (!setOnline)
        {
            //string[] infos = Translator.getText(keyText.MENU_I_Lock_infos).Split(' ');//("Réccupérer Couronnes").Split(' ');
            string nbToUnlock = "\n<color=\"#FFCB00\"><size=32>" + nbCToUnlock + "\n</size></color>";
            //infoLockText.text = infos[0] + nbToUnlock + infos[1];

        }
        else
        {
            infoLockText.text = 
                "Record de"
                + "\n<color=\"#FFCB00\"><size=32>" + nbCToUnlock + "\n</size></color>"
                + "mode infinis";
        }
    }
}
