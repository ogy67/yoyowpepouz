using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum typeRewardDefi
{
    palet,
    booster,
    gemmes,
    yows
}


[System.Serializable]
public struct imgDefi_perType
{
    public typeDefi type;
    public GameObject img_associe;
}

public class defiItem : MonoBehaviour
{
    public typeRewardDefi rewardType;
    public List<imgDefi_perType> img_per_type;

    public Text nbYowText;
    private int[] recYowValPerLevel = new int[] { 30, 60, 90, 120, 150 };

    public Text progText;
    public Text nomDefiText;
    //public GameObject lockItemRaycastGo;
    public Image progBarImg, progBarImg_briller;
    //public float offsetProgBar_init = 0.069f;
    public float offsetProgBar_briller = 0.02f;
    public float maxProgBar = 1f, minProgBar = 0.069f;
    public Animation myAnim, rewardAnim;
    //public Image degradeTrail_1, degradeTrail_2;
    private GameObject currentImgDefiGo;

    private bool dailyOfferHasBeenTaken = false;

    public static int indexLevelDefi_gemmes = 1; // index 1

    public void openMenuDefi()
    {
        // si menu info activé, on le desactive
        if (infoDefiOpen) {
            infoDefiPan.SetActive(false);
            infoDefiOpen = false;
        }
        StartCoroutine(animFinishedLate());
        // animation come
        myAnim.Play("itemDefiCome");
    }

    private IEnumerator animFinishedLate()
    {
        yield return new WaitForSeconds(0.4f);
        rewardAnim["defiItem_finish"].time = 0f;
        checkIsFull(false);
        //Debug.Log("Fin ouverture menu defi, time " + Time.time);
    }

    public void closeMenuDefi()
    {
        StartCoroutine(closeMenuDefi_enum());
    }
    private IEnumerator closeMenuDefi_enum()
    {
        myAnim.Play("itemDefiOut");
        yield return new WaitForSeconds(1 / 3f);// temps anim close
        gameObject.SetActive(false);
    }

    private float getProgValue_inLimits(float progValue)
    {
        float range = (maxProgBar- offsetProgBar_briller) - minProgBar;
        return (progValue * range) + minProgBar;
    }
    public bool checkIsFull(bool noAnim)
    {
        if(progBarImg.fillAmount >= maxProgBar - offsetProgBar_briller && !dailyOfferHasBeenTaken)
        {
            if (!noAnim)
            {
                rewardAnim.Play("defiItem_finish");
                //Debug.Log("Lance anim full, time : " + Time.time);
            }
                //Debug.Log(transform.name + " check defi finished : true");
                return true;
        }
        if (rewardAnim.isPlaying) // stop anim reward si reccupéré ( defi pas static )
        {
            rewardAnim.Stop();
            Color c = rewardAnim.GetComponent<Image>().color;
            rewardAnim.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
            //degradeTrail_1.color = new Color(1, 1, 1, 0);
            //degradeTrail_2.color = new Color(1, 1, 1, 0);
            //brillerReward.color = new Color(1f, 1f, 1f, 0f);
        }
        //Debug.Log(transform.name + " check defi finished : false");
        return false;
    }
    public void loadDefiItem_static(infoDefi iD, int curProg, int nextProg)
    {
        // 40<color=grey><size=23>/50</size></color>
        progText.text = curProg + "<color=grey><size=23>/" + nextProg+ "</size></color>";
        progBarImg.fillAmount = getProgValue_inLimits((float)curProg / nextProg);// (curProg / (float)nextProg)*maxProgBar;
        progBarImg_briller.fillAmount = progBarImg.fillAmount + offsetProgBar_briller;
        checkIsFull(true);
        currenTypeDefi = iD.type;

        //Debug.Log("Load defi , name : " + traductionManager.getInstance().getTradDefi(iD.typeNomDefi));
        nomDefiText.text = traductionManager.getInstance().getTradDefi(iD.typeNomDefi);
    }

    public Text typeDefiText;
    public void loadDefiItem(infoDefi iD, bool daily, bool newDefi)
    {
        if (currentImgDefiGo != null)
            currentImgDefiGo.SetActive(false);
        currenTypeDefi = iD.type;
        int niveau = iD.current_level;
        int currentProg = iD.current_progVal;
        int maxProg = daily ? iD.prog_values[indexLevelDefi_gemmes] : iD.prog_values[niveau];

        if (daily)
        {
            //bool defiFinished = currentProg / (float)maxProg >= 1f;
            dailyOfferHasBeenTaken = iD.dailyTaken;
            //Debug.Log("Offer already taken = "+iD.dailyTaken);
            gemmesRewardDaily_go.SetActive(!dailyOfferHasBeenTaken);
            validateDailyDone_go.SetActive(dailyOfferHasBeenTaken);
        }

        foreach (imgDefi_perType imgInfo in img_per_type)
        {
            if (imgInfo.type == iD.type)
            {
                currentImgDefiGo = imgInfo.img_associe;
                break;
            }
        }

        currentImgDefiGo.gameObject.SetActive(true);
        progBarImg.fillAmount = getProgValue_inLimits((float)currentProg / maxProg);// (currentProg / (float) maxProg)*maxProgBar;
        progBarImg_briller.fillAmount = progBarImg.fillAmount + offsetProgBar_briller;

        // progText.text = curProg + "<color=grey><size=23>/" + nextProg+"</color>";
        progText.text = currentProg + "<color=grey><size=23>/" + maxProg + "</size></color>";
        nomDefiText.text = traductionManager.getInstance().getTradDefi(iD.typeNomDefi);
        if(!daily)
            typeDefiText.text = traductionManager.getInstance().getTradDefi(typeTraduction_app.defi_normalTitre);

        if (rewardType == typeRewardDefi.yows)
        {
            currentYowReward = recYowValPerLevel[niveau];
            setText(true, nbYowText, currentYowReward.ToString());
        }

        checkIsFull(true);
        
    }

    public void changeValue(infoDefi iD) // endGame
    {
        int currentProg = iD.current_progVal;
        int niveau = iD.current_level;
        int maxProg = rewardType == typeRewardDefi.gemmes ? 
            iD.prog_values[indexLevelDefi_gemmes] : iD.prog_values[niveau];

        progBarImg.fillAmount = getProgValue_inLimits((float)currentProg / maxProg);//(currentProg / (float)maxProg)*maxProgBar;
        progBarImg_briller.fillAmount = progBarImg.fillAmount + offsetProgBar_briller;

        // progText.text = curProg + "<color=grey><size=23>/" + nextProg+"</color>";
        progText.text = currentProg + "<color=grey><size=23>/" + maxProg + "</size></color>";

        checkIsFull(true);
    }

    private int currentYowReward = 0;
    public typeDefi currenTypeDefi;

    private void setText(bool childToo, Text t, string s)
    {
        t.text = s;
        if (childToo)
        {
            t.transform.GetChild(0).GetComponent<Text>().text = s;
        }
    }

    public Animation updateItemAnim;
    public GameObject gemmesRewardDaily_go;
    public GameObject validateDailyDone_go;
    //public Image brillerReward;
    public GameObject infoDefiPan;
    private bool infoDefiOpen = false;
    public void btnGetReward(Transform posTr)
    {
        //sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_getDefiReward);

        if (progBarImg.fillAmount >= maxProgBar - offsetProgBar_briller && !dailyOfferHasBeenTaken)
        {
            sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.clickGetReward);
            myAnim.Play("itemDefiClicked");


            // on reset scale de l'anim reward
            //rewardAnim.Stop();
            //rewardAnim.transform.GetChild(2).transform.localScale = new Vector2(1f, 1f);
            //degradeTrail_1.color = new Color(1, 1, 1, 0);
            //degradeTrail_2.color = new Color(1, 1, 1, 0);

            updateItemAnim.Play("updateDefiItem");

            //rewardAnim.Play("defiItem_replace");
            // donner recompense
            switch (rewardType)
            {
                case typeRewardDefi.gemmes:
                    popYowGemManager.getInstance().popRec_main(typeReward_game.gem, defiManager.nbGemmesDailyDefi, posTr.position);
                    //rewardMenu.getInstance().giveReward(typeReward.gemmes, new object[] { defiManager.nbGemmesDailyDefi });
                    break;
                case typeRewardDefi.yows:
                    popYowGemManager.getInstance().popRec_main(typeReward_game.yow, currentYowReward, posTr.position);
                    //rewardMenu.getInstance().giveReward(typeReward.monney, new object[]{ currentYowReward });
                    break;
            }
            if (rewardType == typeRewardDefi.gemmes)
            {
                dailyOfferHasBeenTaken = true;
                // on désactive l'image diamand pour montrer qu'a bien été prise
                gemmesRewardDaily_go.SetActive(false);
                validateDailyDone_go.SetActive(true);
                // on désactive l'animation briller
                checkIsFull(false);
            }

            defiManager.getInstance().defiRewardIsTaken(currenTypeDefi, rewardType == typeRewardDefi.gemmes);
        }
        else
        {
            sfxGame.getInstance().playSfx_listDefiMenu(typeSfx_defiMenu.clickItem);
            // afficher les infos du defi
            if(infoDefiOpen == false)
            {
                if (currentItemDefiOpen != null)
                    currentItemDefiOpen.GetComponent<defiItem>().forceClose_anotherClicked();
                myAnim.Play("itemDefiClicked");
                infoDefiOpen = true;
                StartCoroutine(openInfoDefiEnum());
                currentItemDefiOpen = transform;
            }
            // des-afficher si infos déja ouvert
            else
            {
                infoDefiOpen = false;
                infoDefiPan.SetActive(false);
                currentItemDefiOpen = null;
            }
        }
    }

    private int idClick = 0;
    private static Transform currentItemDefiOpen;
    // à l'ouverture du pan info, attendre 4 secondes avant de refermer automatiquement
    private IEnumerator openInfoDefiEnum()
    {
        idClick++;
        int currentId = idClick;
        infoDefiPan.SetActive(true);
        yield return new WaitForSeconds(4f);

        if (infoDefiOpen == true && currentId == idClick)
        {
            infoDefiOpen = false;
            infoDefiPan.SetActive(false);
            currentItemDefiOpen = null;
        }
    }

    public void forceClose_anotherClicked()
    {
        infoDefiOpen = false;
        infoDefiPan.SetActive(false);
    }


    /*public void updateDailyDefi()
    {
        rewardAnim.Play("defiItem_replace");
    }*/
}
