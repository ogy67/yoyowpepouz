using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum typeTutoAventure
{
    none,
    crown,
    bounce,
    pivot,
    portal,
}

public enum typeTuto_tuto
{
    none,
    push,
    slow,
    checkP,
    switchR,
}
public class menuTutoAventure : MonoBehaviour
{
    public Transform contentSideAnim, btnBImg;
    public void setSideHand(bool right)
    {
        if (!right)
        { 
            contentSideAnim.localScale *= new Vector2(-1, 1);
            btnBImg.localScale *= new Vector2(-1, 1);
        }
    }


    private List<bool> listTutoAlreadySeen = new List<bool>();
    public int levelPivotNum = 4;
    public int levelPortalNum = 7;
    public int nbCrown_noTuto = 19; 
    // renvoit true si on doit ouvrir un tuto pour ce niveau
    public bool tryOpenTuto(int nbCrown, int currentLevel)
    {
        // si a + de x couronnes, aucun tuto : a déjà tout fait
        if (nbCrown >= nbCrown_noTuto)
            return false;

        // si on a pas encore load les infos
        if (listTutoAlreadySeen.Count == 0)
        {
            // on load save
            // defaut : {false, false, false, false}
            listTutoAlreadySeen = (List<bool>)SaveSystem.loadData(typeSave.TUTO_MENUP);
        }
        if (listTutoAlreadySeen == new List<bool>() { true, true, true, true })
            return false;

        switch (currentLevel + 1)
        {
            case 1:
                if (listTutoAlreadySeen[0] == false)
                {
                    openTutoAventure(typeTutoAventure.crown);
                    listTutoAlreadySeen[0] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                    return true;
                }
                return false;
            case 2:
                if (listTutoAlreadySeen[1] == false)
                {
                    openTutoAventure(typeTutoAventure.bounce);
                    listTutoAlreadySeen[1] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                    return true;
                }
                return false;
            case 3:
                if (listTutoAlreadySeen[1] == false)
                {
                    openTutoAventure(typeTutoAventure.bounce);
                    listTutoAlreadySeen[1] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                    return true;
                }
                return false;
            case 4:
                if (listTutoAlreadySeen[2] == false)
                {
                    openTutoAventure(typeTutoAventure.pivot);
                    listTutoAlreadySeen[2] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                    return true;
                }
                return false;
            case 7:
                if (listTutoAlreadySeen[3] == false)
                {
                    openTutoAventure(typeTutoAventure.portal);
                    listTutoAlreadySeen[3] = true;
                    SaveSystem.Save(typeSave.TUTO_MENUP, listTutoAlreadySeen);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    private Dictionary<typeTutoAventure, int> indexAnim = new Dictionary<typeTutoAventure, int>() {
        { typeTutoAventure.crown, 0 },
        { typeTutoAventure.bounce, 1 },
        { typeTutoAventure.pivot, 2 },
        { typeTutoAventure.portal, 3 },
    };
    private Dictionary<typeTutoAventure, string> nameAnim = new Dictionary<typeTutoAventure, string>() {
        { typeTutoAventure.crown, "aideAventureCrown" },
        { typeTutoAventure.bounce, "aideAventureBounce" },
        { typeTutoAventure.pivot, "aideAventurePivot" },
        { typeTutoAventure.portal, "aideAventurePortail" },
    };
    [Header("NORMAL")]
    public GameObject[] animGoTab;
    public Animation aideAnim;
    private int prevIndexOpenned = -1;


    public Animation menuAnim;
    public Text infoText;
    public GameObject animTuto_tutoGo;
    // appelé après anim level start, ouvrir tuto
    private void openTutoAventure(typeTutoAventure type)
    {
        if (animTuto_tutoGo.activeSelf)
            animTuto_tutoGo.SetActive(false);

        if (infoText_tuto.gameObject.activeSelf)
        {
            infoText_tuto.gameObject.SetActive(false);
            infoText.gameObject.SetActive(true);
        }

        if(prevIndexOpenned != -1)
        {
            animGoTab[prevIndexOpenned].SetActive(false);
        }
        prevIndexOpenned = indexAnim[type];
        animGoTab[prevIndexOpenned].SetActive(true);

        menuAnim.Play("tutoAventureOpen");
        aideAnim.Play(nameAnim[type]);
        infoText.text = traductionManager.getInstance().getTradInfoTutoAventure(type);
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
    }




    public TutoManager tutoM;
    public gameManager gameM;
    public void btnClose()
    {
        if (animLocked)
            return;

        menuAnim.Play("tutoAventureClose");
        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_closeMenu);

        if(!TutoManager.tutoOn)
            gameM.StartCoroutine(gameM.loadGame_passTuto(true));
        else
        {
            tutoM.btnOkMenuAnim();
        }
    }




    [Header("TUTO")]

    public Text infoText_tuto;
    private Dictionary<typeTuto_tuto, string> nameAnim_tuto = new Dictionary<typeTuto_tuto, string>() {
        { typeTuto_tuto.push, "aideTutoPush" },
        { typeTuto_tuto.slow, "aideTutoSlow" },
        { typeTuto_tuto.checkP, "aideTutoCheckP" },
        { typeTuto_tuto.switchR, "aideTutoSwitch" },
    };

    private Dictionary<typeTuto_tuto, int> timeAnim_tuto = new Dictionary<typeTuto_tuto, int>() {
        { typeTuto_tuto.push, 302 },
        { typeTuto_tuto.slow, 395 },
        { typeTuto_tuto.checkP, 315 },
        { typeTuto_tuto.switchR, 378 },
    };

    public typeTuto_tuto currentType;

    public void openTuto_tuto(typeTuto_tuto type, bool forceOpen)
    {
        Debug.Log("Call open menu, force? " + forceOpen);
        menuAnim.Play("tutoAventureOpen");
        aideAnim[nameAnim_tuto[type]].time = 0;
        aideAnim.Play(nameAnim_tuto[type]);

        if (forceOpen)
        {
            currentType = type;
            infoText_tuto.text = traductionManager.getInstance().getTradInfoTuto_tuto(type);
            StartCoroutine(lockAnim(type));
        }

        sfxGame.getInstance().playSfx_listMainMenu(typeSfx_mainMenu.click_openMenu);
    }

    public detectorClickWhileTutoAnim detectorClickOnAnimGo;
    public Animation waitAnim_anim;
    public GameObject panLockAnimGo;
    public Image progBar, progBar_2;
    public int offsetTimeAnim = 10;
    private bool animLocked = false;
    private IEnumerator lockAnim(typeTuto_tuto type)
    {
        panLockAnimGo.SetActive(true);
        waitAnim_anim.Play("waitAnimEndOpen");
        animLocked = true;
        detectorClickOnAnimGo.contentWarningSetOn();

        float timeAnim = (timeAnim_tuto[type] + offsetTimeAnim)/ 60f;
        float initTime = Time.time;
        progBar.fillAmount = 0f;
        progBar_2.fillAmount = 0f;
        do
        {
            yield return null;
            progBar.fillAmount = 1 - ( (Time.time - initTime) / timeAnim );
            progBar_2.fillAmount = 1 - ((Time.time - initTime) / timeAnim);
        } while (Time.time - initTime < timeAnim);

        waitAnim_anim.Play("waitAnimEndClose");
        animLocked = false;

        yield return new WaitForSeconds(30f / 60f);

        detectorClickOnAnimGo.contentWarningSetOff();
        panLockAnimGo.SetActive(false);
}
}
