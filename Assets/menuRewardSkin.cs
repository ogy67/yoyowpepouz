using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuRewardSkin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static menuRewardSkin instance;
    public static menuRewardSkin getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("recompenseSkinMenu").GetComponent<menuRewardSkin>();
        return instance;
    }

    public Animation myAnim;

    // palet : imgPion , couleureTrail
    // fond : couleureFond
    // trail : typeTrail
    private object[] dataRec;

    public void giveReward(object[] data, bool skinPalet)
    {
        dataRec = data;
        currentMenuIsPalet = skinPalet;
        menuRewardOpen = true;
        openMenu(skinPalet);
    }

    private bool currentMenuIsPalet = false;
    public GameObject skinPaletGo, skinTrailGo;

    public Image fondRewardImg;
    public Color trailColor;

    public Image pionImg, trailImg, glowImg;
    public Sprite pionBuySprite;
    public superPalet hiddenAnimPionSpecial;
    public GameObject trailReward;

    private Dictionary<trailType, int> indexTrails = new Dictionary<trailType, int>() {
        { trailType.circle_line, 4 },
        { trailType.fore_line, 3 },
        { trailType.two_line_spiral, 1 },
        { trailType.shadow_line, 5 },
        { trailType.two_line, 2 },
        { trailType.fleche_line, 6 },
    };
    public static bool menuRewardOpen = false;
    private void openMenu(bool skinPalet)
    {
        // on affiche le bon type de reward
        skinPaletGo.SetActive(skinPalet);
        skinTrailGo.SetActive(!skinPalet);
        myAnim.Play(currentMenuIsPalet ? "openRewardMenu_palet" : "openMenuReward_trail");

        // on affiche la reward
        if (skinPalet)
        {
            Sprite _paletImg = (Sprite)dataRec[0];
            Color32 _trailColor = (Color32)dataRec[1];
            if (_paletImg == pionBuySprite)
                hiddenAnimPionSpecial.animPaletBuy(new Image[] { pionImg, trailImg });
            pionImg.sprite = _paletImg;
            pionImg.color = Color.white;
            trailImg.color = _trailColor;
            glowImg.color = _trailColor;

            fondRewardImg.color = _trailColor;
        }
        else
        {
            foreach (trailType tT in new trailType[] {
                trailType.two_line, trailType.circle_line, trailType.two_line_spiral,
                trailType.shadow_line, trailType.fore_line, trailType.fleche_line})
            {
                trailReward.transform.GetChild(indexTrails[tT]).gameObject.SetActive(tT == (trailType)dataRec[0]);
            }

            fondRewardImg.color = new Color(trailColor.r, trailColor.g, trailColor.b, 0);
        }
    }


    public void btnGetReward()
    {
        myAnim.Play(currentMenuIsPalet ? "closeMenuReward_palet" : "closeMenuReward_trail");
        StartCoroutine(stopAnimLate());
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.closeSousMenu);

        // si l'avait ouvert pour un pack, passer à étape quivant du pack
        if (openOfferPack.isWaitingItemPackGiven)
        {
            openOfferPack.getInstance().itemPackIsGiven();
        }
    }

    private IEnumerator stopAnimLate()
    {
        yield return new WaitForSeconds(0.2f);
        menuRewardOpen = false;
    }
}
