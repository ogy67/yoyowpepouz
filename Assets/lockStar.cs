using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lockStar : MonoBehaviour
{
    public static bool forceUnlock = false;
    public static int crownRequired = 17;
    private Animation anim;

    public GameObject lockPanDefi;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        instance = gameObject.GetComponent<lockStar>();
        if (forceUnlock)
            gameObject.SetActive(false);
    }

    private static lockStar instance;
    public static lockStar getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("lockStar").GetComponent<lockStar>();
        return instance;
    }

    public GameObject compteurGo;
    public Text nbCText, nbCText_defi;
    public static bool isUnlocked = false;
    public void checkVerouillage(int nbC, bool initApp)//, bool checkOnline)
    {
        if(forceUnlock) { return; }

        int nbCToUnlock = crownRequired;//checkOnline ? recordsInfinisRequired : crownRequired;

        if (initApp)
        {
            nbCText.text = nbCToUnlock.ToString();
            nbCText_defi.text = nbCToUnlock.ToString();

            if (nbC >= nbCToUnlock)
            {
                gameObject.SetActive(false);
                lockPanDefi.SetActive(false);

                isUnlocked = true;
                compteurGo.SetActive(true);
            }
        }
        // vient de se faire deverouiller
        else if (!initApp && nbC == nbCToUnlock)
        {
            isUnlocked = true;
            
            sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.unlock_infinis);
            notifStarUnlocked.getInstance().playerUnlockedStar();
            StartCoroutine(animUnlockLate());
        }
        // si init app, et que deverouillé, pas d'anim dévérouillage
        // si recoit couronne -> dévérouillage, anim dévérouillage
    }

    public float delayTimeAnimUnlock = 0.5f;
    private IEnumerator animUnlockLate()
    {
        yield return new WaitForSeconds(delayTimeAnimUnlock);
        anim.Play("unlockStarMode");
        compteurGo.SetActive(true);
        lockPanDefi.SetActive(false);
    }


    public void btnInfoLock()
    {
        anim.Play("lockStarClicked");
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_simpleBtn);
    }
}
