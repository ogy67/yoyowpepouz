using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openOfferPack : MonoBehaviour
{
    private static openOfferPack instance;
    public static openOfferPack getInstance()
    {
        if (instance == null)
            instance = GameObject.Find("newShop_test").GetComponent<openOfferPack>();
        return instance;
    }

    // appelé après que ensurePurchase finit de check reward
    public void openApp()
    {
        // check si save pack 
        // continuer donner pack
        object curPack = SaveSystem.loadData(typeSave.ENSURE_PACK_GIVEN);
        if(curPack != null)
        {
            object[] info = (object[])curPack;
            if (info.Length > 0)
            {
                typePackOffer type = (typePackOffer)info[0];
                int prog = (int)info[1];
                openPack(type, true, prog, new Vector2(0, 0));
            }
        }
    }

    public packOffer pack_debutant, pack_tiktok;
    public skinManager skinM;
    public GameObject lockScreenGo;
    public static bool isWaitingItemPackGiven = false;

    typePackOffer currentTypePack;
    int currentIndexStep;
    private Vector2 currentPosSpawn;
    private bool currentStep_saveEarly;
    // callOpenMenu = true si appel init
    public void openPack(typePackOffer typePack, bool callOpenMenu, int startIndex, Vector2 posSpawn)
    {
        if (startIndex == 0)
        {
            // direct ouvrir une save de prog
            SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, 0 });
        }
        if (callOpenMenu)
        {
            currentTypePack = typePack;
            currentIndexStep = startIndex;
            currentPosSpawn = posSpawn;
            lockScreenGo.SetActive(true);// ouvrir bloquage de l'écran             
            // /!\ en dessous de UI get skin ET open booster
        }
        // tant que offre pas finit, continuer à donner suite

        if (typePack == typePackOffer.pack_debutant)
        {
            isWaitingItemPackGiven = true;
            switch (currentIndexStep)
            {
                case 0:
                    currentStep_saveEarly = false;
                    // donner or
                    popYowGemManager.getInstance().popRec_main(typeReward_game.yow, 1000, currentPosSpawn);
                    StartCoroutine(waitGiveOrGem());
                    break;
                case 1:
                    currentStep_saveEarly = false;
                    // donner skin
                    skinM.giveSkinPackOffer(currentTypePack);
                    break;
                case 2:
                    currentStep_saveEarly = true;// on doit save avant de donner booster -> player peut fermer app avant close, et nb booster given déjà sauvegardé
                    SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, currentIndexStep + 1 });
                    // donner booster 1
                    menuGetBoost.getInstance().openMenuBooster(typeBooster.boosterEpique);
                    break;
                case 3:
                    currentStep_saveEarly = true;
                    SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, currentIndexStep + 1 });
                    // donner booster 2
                    menuGetBoost.getInstance().openMenuBooster(typeBooster.boosterEpique);
                    break;
                case 4:
                    currentStep_saveEarly = true;
                    SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, currentIndexStep + 1 });
                    // donner booster 3
                    menuGetBoost.getInstance().openMenuBooster(typeBooster.boosterEpique);
                    break;
                default:
                    // finit
                    isWaitingItemPackGiven = false;
                    lockScreenGo.SetActive(false);
                    // supprimer la save
                    SaveSystem.deleteFileSave(typeSave.ENSURE_PACK_GIVEN);

                    // /!\ est null si ouvre à initApp
                    //if(packOffer.currentInstanceTryBuy != null) // si reload offer initApp, peut être null
                    pack_debutant.offerFullyGiven();

                    break;
            }
        }

        else if (typePack == typePackOffer.pack_gratuitTikTok)
        {
            isWaitingItemPackGiven = true;
            switch (currentIndexStep)
            {
                case 0:
                    currentStep_saveEarly = false;
                    // donner or
                    popYowGemManager.getInstance().popRec_main(typeReward_game.yow, 300, currentPosSpawn);
                    StartCoroutine(waitGiveOrGem());
                    break;
                case 1:
                    currentStep_saveEarly = false;
                    // donner skin
                    skinM.giveSkinPackOffer(currentTypePack);
                    break;
                case 2:
                    currentStep_saveEarly = true;// on doit save avant de donner booster -> player peut fermer app avant close, et nb booster given déjà sauvegardé
                    SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, currentIndexStep + 1 });
                    // donner booster 1
                    menuGetBoost.getInstance().openMenuBooster(typeBooster.boosterEpique);
                    break;
                default:
                    // finit
                    isWaitingItemPackGiven = false;
                    lockScreenGo.SetActive(false);
                    // supprimer la save
                    SaveSystem.deleteFileSave(typeSave.ENSURE_PACK_GIVEN);

                    // /!\ est null si ouvre à initApp
                    //if(packOffer.currentInstanceTryBuy != null) // si reload offer initApp, peut être null
                    pack_tiktok.offerFullyGiven();
                    break;
            }
        }
    }

    // anim static, n'attend pas action de player
    private IEnumerator waitGiveOrGem()
    {
        yield return new WaitForSeconds(1.3f); // gemmeManager.waitPurchase_moreGemmes() fait ce temps a peut près
        itemPackIsGiven();
    }

    // appelé quand un item du pack a bien été donné
    public void itemPackIsGiven()
    {
        isWaitingItemPackGiven = false;

        // passer à la récompense suivante
        currentIndexStep++;
        openPack(currentTypePack, false, -1, Vector2.zero);

        if (!currentStep_saveEarly)
        {
            // faire la save de la progression
            SaveSystem.Save(typeSave.ENSURE_PACK_GIVEN, new object[] { currentTypePack, currentIndexStep });
        }
    }
}
