using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct voyantNbCardInfo
{
    public GameObject voyantOn;
    public GameObject voyantOff;
    public Text nbCardText_parent;
    public Text nbCardText;
}

public class menuBooster : MonoBehaviour
{
    private static menuBooster instance;
    public static menuBooster getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("fondMenuBooster_v2").GetComponent<menuBooster>();
        return instance;
    }

    public Animation animMenu;
    public GameObject btnBuyOn, btnBuyOff;
    public Text canBuyText, cannotBuyText;

    public List<voyantNbCardInfo> infoVoyant = new List<voyantNbCardInfo>();
    public Image[] imgSameColorBooster_tab;
    public GameObject[] boosterContent_tab;
    //public HorizontalLayoutGroup hLG;
    public ContentSizeFitter cSF;

    public GameObject boosterPosYGo;
    public Animation[] boosterClickAnim;

    public static bool menuBoosterOpen = false;

    private GameObject currentBoosterContent;
    private probaCard currentInfoBooster;
    private bool canBeBuyed = false;
    public void openBoosterMenu(int indexBooster, probaCard pC)
    {
        foreach (Animation a in animWaitOpen)
            a.enabled = false;
        currentInfoBooster = pC;
        snapScrollToItem.getInstance().snapToItem(typeSnapShop.snapBoost, boosterPosYGo.GetComponent<RectTransform>());
        //boosterClickAnim[indexBooster].Play("boosterClicked");

        Color c = pC.colorBooster;

        foreach (Image i in imgSameColorBooster_tab)
            i.color = new Color(c.r, c.g, c.b, i.color.a);

        if (currentBoosterContent != null)
            currentBoosterContent.SetActive(false);
        currentBoosterContent = boosterContent_tab[indexBooster];
        currentBoosterContent.SetActive(true);

        // activer desactiver voyant nb card + set text
        bool voyantOn;
        int nbCard;
        for (int i = 0; i < 4; i++)
        {
            nbCard = (int)pC.nbCards_perRarety[i];
            voyantOn = nbCard > 0;

            infoVoyant[i].voyantOn.SetActive(voyantOn);
            infoVoyant[i].voyantOff.SetActive(!voyantOn);

            infoVoyant[i].nbCardText_parent.text = "×" + nbCard;
            if (voyantOn)
                infoVoyant[i].nbCardText.text = "×" + nbCard;
        }

        // activer desactiver btnBuy
        canBeBuyed = gemmesManager.nbGemmes >= pC.priceBooster;
        btnBuyOn.SetActive(canBeBuyed);
        btnBuyOff.SetActive(!canBeBuyed);
        Text t = canBeBuyed ? canBuyText : cannotBuyText;
        t.text = pC.priceBooster.ToString();

        //infoVoyant[indexBooster].nbCardText.text = pC.priceBooster.ToString();

        // lancer anim ouverture menu
        animMenu.Play("openMenuBooster");
        StartCoroutine(open_late());
        menuBoosterOpen = true;
    }

    private IEnumerator open_late()
    {
        float t = Time.time;

        yield return null;
        cSF.enabled = false;
        cSF.enabled = true;

        while (Time.time < t + timeAnimOpen)
            yield return null;

        foreach (Animation a in animWaitOpen)
            a.enabled = true;
    }


    public openBoosterManager _openBoosterManager;
    public Animation[] animWaitOpen;
    public float timeAnimOpen = 46f / 60;
    public void btnOpenMenuBooster(int indexBooster)
    {
        probaCard pC = _openBoosterManager.getInfoBooster(
           indexBooster == 0 ? typeBooster.boosterArgent
           : indexBooster == 1 ? typeBooster.boosterOr : typeBooster.boosterEpique);
        openBoosterMenu(indexBooster, pC);
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.openSousMenu);
    }


    public void btnCloseMenuBooster()
    {
        foreach (Animation a in animWaitOpen)
            a.enabled = false;
        animMenu.Play("closeMenuBooster");
        menuBoosterOpen = false;
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.closeSousMenu);
    }

    public Animation btnBuyBoosterAnim;
    public void btnBuyBooster()
    {
        if (canBeBuyed)
        {
            validAchat.getInstance().openMenuValidAchat(false, currentInfoBooster.priceBooster, typeAchat.openBooster);
            btnBuyBoosterAnim.Play("btnBuyBooster");
        }
        else
        {
            sfxGame.getInstance().playSfx_listShopSousMenu(typeSfx_shopSousMenu.btnBuyNotOk);
        }
    }

    public void validAchatBooster()
    {
        // on ouvre le booster
        menuGetBoost.getInstance().openMenuBooster(currentInfoBooster.tB);

        //StartCoroutine(closeMenuLate(timeWaitClose));
        btnCloseMenuBooster();
    }


    public Color32 commonColor, rareColor, epicColor, legendaryColor;
    public Color32 getColorRarety(typeRarety tR)
    {
        return tR == typeRarety.common ? commonColor :
            tR == typeRarety.epic ? epicColor :
            tR == typeRarety.rare ? rareColor : legendaryColor;
    }

    /*
    public void btnBuyBooster()
    {
        if (!openSwitch)
        {
            _openBoosterManager.buyBooster(currentInfoBoosterIndex, false, typeBooster.error);
            sfxManager.getInstance().playSound_shopDefi(typeSound_menuShopDefi.btn_buyUpgrade);
        }
    }
    */
}
