using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeRewardBuyed
{
    none,
    gem_1,
    gem_2,
    gem_3,
    booster, // offre spe avec gem et palet
    pack_debutant,
}
public class ensurePurchaseReward : MonoBehaviour
{
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
        }
    }*/
    // ce script doit assurer que le joueur reçoit la recompense de son achat
    // prevenir cas : buy process end, start process reward ET close app = pas reçu reward de l'achat

    private static ensurePurchaseReward instance;
    public static ensurePurchaseReward getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("ensurePurchaseReward").GetComponent<ensurePurchaseReward>();
        return instance;
    }

    // à l'initialisation ( après menuP loaded )
    // regarder si une save rewardBuy exist
    // si oui, donner directement recompense non reçu

    private static bool shouldGiveRewardBuyInitApp = false;
    void Start()
    {
        // on regarde si une save exist
        var l = SaveSystem.loadData(typeSave.ENSURE_REWARD_BUY);
        // si la save existe
        if (l != null)
        {
            currentList = (List<typeRewardBuyed>)l;
            // lancer reward
            shouldGiveRewardBuyInitApp = true;
        }
    }

    private List<typeRewardBuyed> currentList;
    public void playerBuyedListReward(List<typeRewardBuyed> listRew)
    {
        currentList = listRew;
        // on save la liste en local
        SaveSystem.Save(typeSave.ENSURE_REWARD_BUY, currentList);
    }

    public void playerGetBuyedReward(typeRewardBuyed rew)
    {
        if (currentList.Contains(rew))
        {
            currentList.Remove(rew);

            if (currentList.Count == 0)
            {
                // on supprime la save locale
                SaveSystem.deleteFileSave(typeSave.ENSURE_REWARD_BUY);
            }
            else
            {
                // on save la list
                SaveSystem.Save(typeSave.ENSURE_REWARD_BUY, currentList);
            }
        }
    }


    private List<typeRewardBuyed> listRewardGem = new List<typeRewardBuyed>() {
        typeRewardBuyed.gem_1,
        typeRewardBuyed.gem_2,
        typeRewardBuyed.gem_3,
    };
    public Animation fondAnim;
    // /!\ pour l'instant, juste gem
    public void backUpReward()
    {
        if (shouldGiveRewardBuyInitApp)
        {
            Debug.Log("<color=red>BACK UP REWARD</color>");
            fondAnim.Play("backUpRewardFondOn");
            // on donne la reward reçu
            typeRewardBuyed rew = currentList[0];
            if (listRewardGem.Contains(rew))
            {
                StartCoroutine(animGem(rew));
            }
        }
        openOfferPack.getInstance().openApp();
    }

    private Dictionary<typeRewardBuyed, int> nbGemPerOffer = new Dictionary<typeRewardBuyed, int>() {
        { typeRewardBuyed.gem_1, 90 },
        { typeRewardBuyed.gem_2, 500 },
        { typeRewardBuyed.gem_3, 1200 },
    };

    private IEnumerator animGem(typeRewardBuyed rew)
    {
        yield return new WaitForSeconds(0.5f);
        popYowGemManager.getInstance().popRec_main(typeReward_game.gem, nbGemPerOffer[rew], fondAnim.transform.position);
        sfxGame.getInstance().playSfx_listShopMenu(typeSfx_shopMenu.animReward_validAchat);

        yield return new WaitForSeconds(1.5f);
        playerGetBuyedReward(rew);
        fondAnim.Play("backUpRewardFondOff");
    }
}
